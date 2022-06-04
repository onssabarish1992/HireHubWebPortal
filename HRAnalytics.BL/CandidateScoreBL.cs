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
    }
}
