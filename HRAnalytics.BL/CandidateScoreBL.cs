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
    public class CandidateScoreBL: ICandidateScore
    {
        private readonly IConfiguration _configuration;

        public CandidateScoreBL(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        /// <summary>
        /// Get candidate score based on schedule ID
        /// </summary>
        /// <param name="argScheduleID"></param>
        /// <returns></returns>
        public CandidateScore GetCandidateScore(int argScheduleID)
        {
            #region Declarations
            CandidateScore candidateScore = new();
            HRAnalyticsDBManager l_HRAnalyticsDBManager = new("HRAnalyticsConnection", _configuration);
            List<IDbDataParameter> l_Parameters = new();
            DataSet l_CandidateScoreDataset = new DataSet();
            DataTable l_CandidateInfoTable = new DataTable();
            DataTable l_CandidateScoreTable = new DataTable();
            List<CandidateEvaluation> l_CandidateEvaluation;
            CandidateEvaluation l_Evaluation;
            Candidate l_Candidateinfo;
            #endregion

            try
            {
                l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.SCHEDULEID, argScheduleID, DbType.Int64));
                l_CandidateScoreDataset = l_HRAnalyticsDBManager.GetDataSet(StoredProcedure.GET_CANDIDATEINTERVIEWQUESTIONS, CommandType.StoredProcedure, l_Parameters.ToArray());

                if (l_CandidateScoreDataset != null && l_CandidateScoreDataset.Tables.Count > 0)
                {
                    l_CandidateInfoTable = l_CandidateScoreDataset.Tables[0];
                    l_CandidateScoreTable = l_CandidateScoreDataset.Tables[1];
                }

                if (l_CandidateInfoTable != null && l_CandidateInfoTable.Rows.Count > 0)
                {
                    l_Candidateinfo = new Candidate();
                    l_Candidateinfo.ScheduleID = l_CandidateInfoTable.Rows[0]["schedule_id"] == DBNull.Value ? 0 : Convert.ToInt32(l_CandidateInfoTable.Rows[0]["schedule_id"]);
                    l_Candidateinfo.CandidateID = l_CandidateInfoTable.Rows[0]["candidate_id"] == DBNull.Value ? 0 : Convert.ToInt32(l_CandidateInfoTable.Rows[0]["candidate_id"]);
                    l_Candidateinfo.JobId = l_CandidateInfoTable.Rows[0]["job_id"] == DBNull.Value ? 0 : Convert.ToInt32(l_CandidateInfoTable.Rows[0]["job_id"]);
                    l_Candidateinfo.JobName = l_CandidateInfoTable.Rows[0]["job_name"] == DBNull.Value ? string.Empty : Convert.ToString(l_CandidateInfoTable.Rows[0]["job_name"]);
                    l_Candidateinfo.CandidateName = l_CandidateInfoTable.Rows[0]["candidate_name"] == DBNull.Value ? string.Empty : Convert.ToString(l_CandidateInfoTable.Rows[0]["candidate_name"]);
                    l_Candidateinfo.IsRated = l_CandidateInfoTable.Rows[0]["is_candidate_rated"] == DBNull.Value ? false : Convert.ToBoolean(l_CandidateInfoTable.Rows[0]["is_candidate_rated"]);

                    candidateScore.Candidate = l_Candidateinfo;
                }

                if (l_CandidateScoreTable != null && l_CandidateScoreTable.Rows.Count > 0)
                {
                    l_CandidateEvaluation = new List<CandidateEvaluation>();
                    for (int i = 0; i < l_CandidateScoreTable.Rows.Count; i++)
                    {
                        l_Evaluation = new CandidateEvaluation();
                        DataRow l_Row = l_CandidateScoreTable.Rows[i];
                        l_Evaluation.JobId = l_Row["job_id"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["job_id"]);
                        l_Evaluation.CriteriaId = l_Row["criteria_id"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["criteria_id"]);
                        l_Evaluation.CriteriaScore = l_Row["criteria_score"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["criteria_score"]);
                        l_Evaluation.CriteriaComments = l_Row["criteria_comments"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["criteria_comments"]);
                        l_Evaluation.CriteriaName = l_Row["criteria_name"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["criteria_name"]);
                        l_Evaluation.CriteriaDescription = l_Row["sub_criteria_description"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["sub_criteria_description"]);
                        l_CandidateEvaluation.Add(l_Evaluation);
                    }
                    candidateScore.Evaluation = l_CandidateEvaluation;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return candidateScore;
        }

        /// <summary>
        /// BL method to save interview ratings
        /// </summary>
        /// <param name="argLoggedInUserID"></param>
        /// <param name="argScheduleID"></param>
        /// <param name="argCandidateEvaluations"></param>
        public void SaveInterviewRatings(string argLoggedInUserID, int argScheduleID, List<CandidateEvaluation> argCandidateEvaluations)
        {
            HRAnalyticsDBManager l_HRAnalyticsDBManager = new("HRAnalyticsConnection", _configuration);
            List<IDbDataParameter> l_Parameters = new();
            int l_LastID = 0;
            XElement l_ratingXML;
            try
            {
                l_ratingXML = GenerateRatingXML(argCandidateEvaluations, argScheduleID);

                l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.LOGGEDINUSER, argLoggedInUserID, DbType.String));
                l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.RATINGXML, l_ratingXML.ToString(), DbType.Xml));

                //Call stored procedure
                l_HRAnalyticsDBManager.Insert(StoredProcedure.INSERT_INTERVIEWERRATINGS, CommandType.StoredProcedure, l_Parameters.ToArray(), out l_LastID);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Generate XML from schedule ID and ratings
        /// </summary>
        /// <param name="argCandidateEvaluations"></param>
        /// <param name="argScheduleID"></param>
        /// <returns></returns>
        private XElement GenerateRatingXML(List<CandidateEvaluation> argCandidateEvaluations, int argScheduleID)
        {
            XElement l_ratingXML;
            try
            {
                l_ratingXML = new XElement("Criterias",
                    from rating in argCandidateEvaluations
                    select new XElement("Criteria",
                    new XElement("ScheduleId", argScheduleID),
                    new XElement("JobId", rating.JobId),
                    new XElement("CriteriaId", rating.CriteriaId),
                    new XElement("CriteriaScore", rating.CriteriaScore),
                    new XElement("CriteriaComments", rating.CriteriaComments)
                    ));
            }
            catch (Exception)
            {
                throw;
            }

            return l_ratingXML;
        }


        /// <summary>
        /// Get All candidate scores for
        /// </summary>
        /// <returns></returns>
        public List<CandidateEvaluation> GetAllCandidateScores()
        {
            #region Declarations
            List<CandidateEvaluation> l_CandidateEvaluations = new List<CandidateEvaluation>();
            HRAnalyticsDBManager l_HRAnalyticsDBManager = new("HRAnalyticsConnection", _configuration);
            DataTable l_CandidateDataTable;
            CandidateEvaluation l_CandidateEvaluation;
            int l_CandidateCount;
            #endregion
            try
            {
                l_CandidateDataTable = l_HRAnalyticsDBManager.GetDataTable(StoredProcedure.GET_ALLCANDIDATE_RATINGS, CommandType.StoredProcedure);

                if (l_CandidateDataTable != null && l_CandidateDataTable.Rows.Count > 0)
                {
                    l_CandidateCount = l_CandidateDataTable.Rows.Count;
                    for (int i = 0; i < l_CandidateCount; i++)
                    {
                        l_CandidateEvaluation = new CandidateEvaluation();
                        DataRow l_Row = l_CandidateDataTable.Rows[i];

                        l_CandidateEvaluation.JobId = l_Row["job_id"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["job_id"]);
                        l_CandidateEvaluation.CriteriaId = l_Row["criteria_id"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["criteria_id"]);
                        l_CandidateEvaluation.CriteriaName = l_Row["criteria_name"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["criteria_name"]);
                        l_CandidateEvaluation.CriteriaDescription = l_Row["sub_criteria_description"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["sub_criteria_description"]);
                        l_CandidateEvaluation.CriteriaScore = l_Row["criteria_score"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["criteria_score"]);
                        l_CandidateEvaluation.CriteriaComments = l_Row["criteria_comments"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["criteria_comments"]);
                        l_CandidateEvaluation.CandidateID = l_Row["candidate_id"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["candidate_id"]);
                        l_CandidateEvaluation.CandidateName = l_Row["candidate_name"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["candidate_name"]);

                        l_CandidateEvaluations.Add(l_CandidateEvaluation);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return l_CandidateEvaluations;
        }


        public void SaveGlobalScores(string argLoggedInUserID, int argScheduleID, List<Candidate> argGlobalScores)
        {
            HRAnalyticsDBManager l_HRAnalyticsDBManager = new("HRAnalyticsConnection", _configuration);
            List<IDbDataParameter> l_Parameters = new();
            int l_LastID = 0;
            XElement l_ratingXML;
            try
            {
                l_ratingXML = GenerateScoresXML(argGlobalScores, argScheduleID);

                l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.LOGGEDINUSER, argLoggedInUserID, DbType.String));
                l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.SCORESXML, l_ratingXML.ToString(), DbType.Xml));

                //Call stored procedure
                l_HRAnalyticsDBManager.Insert(StoredProcedure.SAVE_GLOBAL_SCORE, CommandType.StoredProcedure, l_Parameters.ToArray(), out l_LastID);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private XElement GenerateScoresXML(List<Candidate> argGlobalScores, int argScheduleID)
        {
            XElement l_ratingXML;
            try
            {
                l_ratingXML = new XElement("Scores",
                    from scr in argGlobalScores
                    select new XElement("Score",
                    new XElement("candidate_id", scr.CandidateID),
                    new XElement("schedule_id", argScheduleID),
                    new XElement("job_id", scr.JobId),
                    new XElement("is_hired", scr.IsHired),
                    new XElement("proposed_compensation", scr.ProposedCompensation),
                    new XElement("actual_compensation", scr.ActualCompensation)
                    ));
            }
            catch (Exception)
            {
                throw;
            }

            return l_ratingXML;
        }
    }
}
