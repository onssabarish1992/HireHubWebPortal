using HRAnalytics.BL.Interfaces;
using HRAnalytics.DAL;
using HRAnalytics.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HRAnalytics.BL
{
    public class AHP: IAHP
    {
        private IJobBL _jobBL;
        private readonly IConfiguration _configuration;
        public AHP(IJobBL jobBL, IConfiguration configuration)
        {
            _jobBL = jobBL;
            _configuration = configuration;
        }

        public void SavePairs(int argEntityID, string argLoggedInUser)
        {
            List<AHPPair> l_FinalPairs=new List<AHPPair>();
            try
            {
                //Get the entity based on entity ID
                if(argEntityID == 1)
                {
                    //Get the job roles 
                    var l_jobs = _jobBL.GetRoles();

                    if(l_jobs!=null && l_jobs.Count > 0)
                    {
                        var l_Orderedjobs = l_jobs.OrderBy(x => x.JobId).ToList();

                        List<string> l_Jobs = l_Orderedjobs.Select(x => Convert.ToString(x.JobId)).ToList();

                        List<string> l_PairsGenerated = generatePairs(l_Jobs);

                        l_FinalPairs = CreateAHPPairs(l_PairsGenerated, argEntityID,null);

                        SavAHPPairs(argLoggedInUser, l_FinalPairs);
                    }
                }
                else if(argEntityID == 2)
                {
                    //get the job criterias based on parent entity ID
                    var l_jobCriteria = _jobBL.GetEvaluationCriteria();

                    var distinctJobIds = l_jobCriteria.Select(x => x.JobId).Distinct();

                    foreach (var job in distinctJobIds)
                    {
                        var l_FilteredjobCriteria = l_jobCriteria.Where(x => x.JobId == job).OrderBy(y => y.CriteriaID);

                        List<string> l_Criterias = l_FilteredjobCriteria.Select(x => Convert.ToString(x.CriteriaID)).ToList();

                        List<string> l_PairsGenerated = generatePairs(l_Criterias);

                        l_FinalPairs = CreateAHPPairs(l_PairsGenerated, argEntityID, job);

                        SavAHPPairs(argLoggedInUser, l_FinalPairs);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }


        private List<AHPPair> CreateAHPPairs(List<string> argPairs, int argEntityID, int? argParentEntityID)
        {
            #region Declarations
            List<AHPPair> l_AHPPairs = new List<AHPPair>();
            AHPPair l_AhpPair;
            #endregion
            try
            {
                foreach (var pair in argPairs)
                {
                    l_AhpPair = new AHPPair();
                    var pairDetails = pair.Split("|");
                    l_AhpPair.Pair1 = Convert.ToInt32(pairDetails[0]);
                    l_AhpPair.Pair2 = Convert.ToInt32(pairDetails[1]);
                    l_AhpPair.EntityId = argEntityID;
                    l_AhpPair.ParentEntityId = argParentEntityID.HasValue? argParentEntityID.Value:0;

                    l_AHPPairs.Add(l_AhpPair);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return l_AHPPairs;
        }

        /// <summary>
        /// This method is used to generate pairs for AHP
        /// </summary>
        /// <param name="argPairs"></param>
        /// <returns></returns>
        public List<string> generatePairs(List<string> argPairs)
        {
            List<string> l_pairs = new List<string>();
            try
            {
                if(argPairs!=null && argPairs.Count > 0)
                {
                    for (int i = 0; i < argPairs.Count; i++)
                    {
                        for (int j = i + 1; j < argPairs.Count; j++)
                        {
                            l_pairs.Add(string.Concat(argPairs[i], "|", argPairs[j]));
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return l_pairs;
        }


        public void SavAHPPairs(string argLoggedInUserID, List<AHPPair> argAHPPairs)
        {
            HRAnalyticsDBManager l_HRAnalyticsDBManager = new("HRAnalyticsConnection", _configuration);
            List<IDbDataParameter> l_Parameters = new();
            int l_LastID = 0;
            XElement l_pairXML;
            try
            {
                l_pairXML = GeneratePAIRSXML(argAHPPairs);

                l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.LOGGEDINUSER, argLoggedInUserID, DbType.String));
                l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.PAIRSXML, l_pairXML.ToString(), DbType.Xml));

                //Call stored procedure
                l_HRAnalyticsDBManager.Insert(StoredProcedure.SAVE_AHP_PAIRS, CommandType.StoredProcedure, l_Parameters.ToArray(), out l_LastID);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private XElement GeneratePAIRSXML(List<AHPPair> argAHPPairs)
        {
            XElement l_PairXML;
            try
            {
                l_PairXML = new XElement("Pairs",
                    from scr in argAHPPairs
                    select new XElement("Pair",
                    new XElement("Pair1", scr.Pair1),
                    new XElement("Pair2", scr.Pair2),
                    new XElement("EntityId", scr.EntityId),
                    new XElement("ParentEntityId", scr.ParentEntityId)
                    ));
            }
            catch (Exception)
            {
                throw;
            }

            return l_PairXML;
        }



        public List<AHPPair> GetAHPPairs(int argEntityID, int? argParentEntityID)
        {
            #region Declarations
            List<AHPPair> l_PairCollection = new();
            HRAnalyticsDBManager l_HRAnalyticsDBManager = new("HRAnalyticsConnection", _configuration);
            List<IDbDataParameter> l_Parameters = new();
            DataTable l_AHPPairDataTable;
            AHPPair l_AHPPair;
            int l_PairCount;
            #endregion
            try
            {
                l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.ENTITYID, argEntityID, DbType.Int32));
                l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.PARENTENTITYID, argParentEntityID.HasValue? argParentEntityID.Value: DBNull.Value,DbType.String));

                l_AHPPairDataTable = l_HRAnalyticsDBManager.GetDataTable(StoredProcedure.GET_AHPPAIRS, CommandType.StoredProcedure, l_Parameters.ToArray());

                if (l_AHPPairDataTable != null && l_AHPPairDataTable.Rows.Count > 0)
                {
                    l_PairCount = l_AHPPairDataTable.Rows.Count;
                    for (int i = 0; i < l_PairCount; i++)
                    {
                        l_AHPPair = new AHPPair();

                        DataRow l_Row = l_AHPPairDataTable.Rows[i];

                        l_AHPPair.PairId = l_Row["pair_id"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["pair_id"]);
                        l_AHPPair.Pair1 = l_Row["pair_1"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["pair_1"]);
                        l_AHPPair.Pair1Name = l_Row["pair1name"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["pair1name"]);
                        l_AHPPair.Pair2 = l_Row["pair_2"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["pair_2"]);
                        l_AHPPair.Pair2Name = l_Row["pair2name"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["pair2name"]);
                        l_AHPPair.Weightage = l_Row["weightage"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["weightage"]);
                        l_AHPPair.ParentEntityId = l_Row["parent_entity_id"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["parent_entity_id"]);
                        l_PairCollection.Add(l_AHPPair);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return l_PairCollection;
        }
    }
}
