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

        public void SavePairs(int argEntityID, int? argParentEntityID, string argLoggedInUser)
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
                        var l_Orderedjobs = l_jobs.OrderBy(x => x.JobCriteriaId).ToList();

                        List<string> l_Jobs = l_Orderedjobs.Select(x => Convert.ToString(x.JobId)).ToList();

                        List<string> l_PairsGenerated = generatePairs(l_Jobs);

                        l_FinalPairs = CreateAHPPairs(l_PairsGenerated, argEntityID);
                    }
                }
                else if(argEntityID == 2)
                {
                    //get the job criterias based on parent entity ID
                    var l_jobCriteria = _jobBL.GetEvaluationCriteria();

                    var l_FilteredjobCriteria = l_jobCriteria.Where(x => x.JobId == argParentEntityID).OrderBy(y => y.SubCriteriaId);

                    List<string> l_Criterias = l_FilteredjobCriteria.Select(x => Convert.ToString(x.CriteriaID)).ToList();

                    List<string> l_PairsGenerated = generatePairs(l_Criterias);

                    l_FinalPairs = CreateAHPPairs(l_PairsGenerated, argEntityID);
                }

                SavAHPPairs(argLoggedInUser, l_FinalPairs);
            }
            catch (Exception)
            {
                throw;
            }

        }


        private List<AHPPair> CreateAHPPairs(List<string> argPairs, int argEntityID)
        {
            List<AHPPair> l_AHPPairs = new List<AHPPair>();
            AHPPair l_AhpPair;
            try
            {
                foreach (var pair in argPairs)
                {
                    l_AhpPair = new AHPPair();
                    var pairDetails = pair.Split("|");
                    l_AhpPair.Pair1 = Convert.ToInt32(pairDetails[0]);
                    l_AhpPair.Pair2 = Convert.ToInt32(pairDetails[1]);

                    l_AhpPair.EntityId = argEntityID;

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
    }
}
