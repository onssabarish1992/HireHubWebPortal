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


        public void SavAHPWeightage(string argLoggedInUserID, List<AHPPair> argAHPPairs)
        {
            HRAnalyticsDBManager l_HRAnalyticsDBManager = new("HRAnalyticsConnection", _configuration);
            List<IDbDataParameter> l_Parameters = new();
            int l_LastID = 0;
            XElement l_pairXML;
            try
            {
                l_pairXML = GenerateWeightageXML(argAHPPairs);

                l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.LOGGEDINUSER, argLoggedInUserID, DbType.String));
                l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.WEIGHTAGEXML, l_pairXML.ToString(), DbType.Xml));

                //Call stored procedure
                l_HRAnalyticsDBManager.Insert(StoredProcedure.SAVE_AHPWEIGHTAGE, CommandType.StoredProcedure, l_Parameters.ToArray(), out l_LastID);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private XElement GenerateWeightageXML(List<AHPPair> argAHPPairs)
        {
            XElement l_PairXML;
            try
            {
                l_PairXML = new XElement("Pairs",
                    from scr in argAHPPairs
                    select new XElement("Pair",
                    new XElement("PairId", scr.PairId),
                    new XElement("Weightage", scr.Weightage)
                    ));
            }
            catch (Exception)
            {
                throw;
            }

            return l_PairXML;
        }


        /// <summary>
        /// Thus function is used to calculate AHP final scores
        /// </summary>
        /// <param name="argScores"></param>
        /// <returns></returns>
        public double[] CalculateAHPScores(double[,] argScores)
        {
            double[] argScorescolSum = new double[argScores.GetLength(1)];
            for (int j = 0; j < argScores.GetLength(1); j++)
            {
                argScorescolSum[j] = 0;
            }
            double[,] argScoresNormal = new double[argScores.GetLength(0), argScores.GetLength(1)];
            for (int j = 0; j < argScores.GetLength(1); j++)
            {
                for (int i = 0; i < argScores.GetLength(0); i++)
                {
                    argScorescolSum[j] += argScores[i, j];
                }
            }
            for (int i = 0; i < argScores.GetLength(0); i++)
            {
                for (int j = 0; j < argScores.GetLength(1); j++)
                {
                    argScoresNormal[i, j] = argScores[i, j] / argScorescolSum[j];
                }
            }

            double[] argScoresrowSum = new double[argScores.GetLength(0)];
            for (int i = 0; i < argScores.GetLength(0); i++)
            {
                argScoresrowSum[i] = 0;
            }
            for (int i = 0; i < argScores.GetLength(0); i++)
            {
                for (int j = 0; j < argScores.GetLength(1); j++)
                {
                    argScoresrowSum[i] += argScoresNormal[i, j];
                }
            }

            double[] argScoresrowsumNormal = new double[argScoresrowSum.Length];
            double argScoresrowsumSum = 0;
            for (int i = 0; i < argScoresrowSum.Length; i++)
            {
                argScoresrowsumSum += argScoresrowSum[i];
            }
            if (argScoresrowsumSum != 0)
            {
                for (int i = 0; i < argScoresrowSum.Length; i++)
                {
                    argScoresrowsumNormal[i] = (argScoresrowSum[i] / argScoresrowsumSum);
                }
            }

            return argScoresrowsumNormal;
        }


        public void SavAHPFinalScores(string argLoggedInUserID, int argEntityID, int? argJobId)
        {
            HRAnalyticsDBManager l_HRAnalyticsDBManager = new("HRAnalyticsConnection", _configuration);
            List<IDbDataParameter> l_Parameters = new();
            int l_LastID = 0;
            XElement l_pairXML;
            List<AHPFinalScore> l_FinalScores = new List<AHPFinalScore>();
            try
            {

                var l_Entities = GetAHPPairs(argEntityID, argJobId);

                if(l_Entities != null)
                {
                    var l_distinctRecordSet1 = l_Entities.Select(x => x.Pair1).Distinct();

                    var l_distinctRecordSet2 = l_Entities.Select(x => x.Pair2).Distinct();

                    var l_OverallRecordSet = l_distinctRecordSet1.Union(l_distinctRecordSet2);

                    var l_CriteriaCount = l_OverallRecordSet.Distinct().Count();

                    //Generate the matrix
                    var l_Matrix = createMatrix(l_Entities, l_CriteriaCount);

                    //Generate the score
                    var l_AHPScores = CalculateAHPScores(l_Matrix);

                    //Create the final scores list
                    l_FinalScores = GenerateFinalScoresData(l_AHPScores, argEntityID, argJobId);
                  
                    //Form the XML for stored procedure
                    l_pairXML = GenerateFinalScoresXML(l_FinalScores);

                    l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.LOGGEDINUSER, argLoggedInUserID, DbType.String));
                    l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.AHPFINALSCOREXML, l_pairXML.ToString(), DbType.Xml));
                    l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.ENTITYID, argEntityID, DbType.Int32));
                    l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.JOBID, argJobId.HasValue ? argJobId.Value : DBNull.Value, DbType.Int32));

                    //Call stored procedure
                    l_HRAnalyticsDBManager.Insert(StoredProcedure.SAVE_AHPFINALSCORES, CommandType.StoredProcedure, l_Parameters.ToArray(), out l_LastID);
                }

                
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// IMP: Create final data score
        /// </summary>
        /// <param name="argAHPScores"></param>
        /// <param name="argEntityID"></param>
        /// <param name="argJobId"></param>
        /// <returns></returns>
        private List<AHPFinalScore> GenerateFinalScoresData(double[] argAHPScores, int argEntityID, int? argJobId)
        {
            List<AHPFinalScore> l_FinalScores = new List<AHPFinalScore>();
            AHPFinalScore l_AHPScore;
            try
            {
                if(argEntityID == 1)
                {
                    //Get the job roles
                    var l_Jobs = _jobBL.GetRoles();

                    //Get the distinct jobs ordered based on job id
                    var l_OrderedJobs = l_Jobs.OrderBy(x => x.JobId).ToList();

                    for (int i = 0; i < l_OrderedJobs.Count(); i++)
                    {
                        l_AHPScore = new AHPFinalScore();
                        l_AHPScore.EvalID = l_OrderedJobs[i].JobId;
                        l_AHPScore.Score = argAHPScores[i];

                        l_FinalScores.Add(l_AHPScore);
                    }
                }
                else if(argEntityID == 2)
                {
                    //Get the subcriteria
                    var l_Criteria = _jobBL.GetEvaluationCriteria();

                    //Get evaluation criterias for required job
                    var l_ReqdCriterias = l_Criteria.Where(x=>x.JobId == argJobId).OrderBy(y => y.CriteriaID).ToList();

                    for (int i = 0; i < l_ReqdCriterias.Count(); i++)
                    {
                        l_AHPScore = new AHPFinalScore();
                        l_AHPScore.EvalID = l_ReqdCriterias[i].CriteriaID;
                        l_AHPScore.Score = argAHPScores[i];

                        l_FinalScores.Add(l_AHPScore);
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }

            return l_FinalScores;
        }

        /// <summary>
        /// This function is used to create matrix for AHP
        /// </summary>
        /// <param name="argAHPPairs"></param>
        /// <returns></returns>
        public double[,] createMatrix(List<AHPPair> argAHPPairs, int argDistinctRecordSet)
        {
            #region Declarations
            double[,] a = new double[argDistinctRecordSet, argDistinctRecordSet];
            int comparison_factors = (argDistinctRecordSet * argDistinctRecordSet - argDistinctRecordSet) / 2;
            double[] p = new double[comparison_factors];
            int m = 0;
            int k = 0;
            #endregion
            try
            {
                
                for (int i = 0; i < argAHPPairs.Count; i++)
                {
                    p[m] = argAHPPairs[i].Weightage < 0 ? (double)1 / Math.Abs(argAHPPairs[i].Weightage) : argAHPPairs[i].Weightage;
                    m++;
                }

                for (int i = 0; i < a.GetLength(0); i++)
                {
                    for (int j = 0; j < a.GetLength(1); j++)
                    {
                        if (i == j)
                        {
                            a[i, j] = 1;
                        }
                        else if (i < j)
                        {
                            a[i, j] = p[k];
                            k++;
                        }
                        else
                        {
                            a[i, j] = (1 / a[j, i]);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return a;
        }

        private XElement GenerateFinalScoresXML(List<AHPFinalScore> argAHPPairs)
        {
            XElement l_PairXML;
            try
            {
                l_PairXML = new XElement("Pairs",
                    from scr in argAHPPairs
                    select new XElement("Pair",
                    new XElement("EvalID", scr.EvalID),
                    new XElement("Weightage", scr.Score)
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
