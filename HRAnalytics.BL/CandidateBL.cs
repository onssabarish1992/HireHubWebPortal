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
    public class CandidateBL: ICandidateBL
    {
        private readonly IConfiguration _configuration;

        public CandidateBL(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// This function is used to save the candidate details and interview schedule
        /// </summary>
        /// <param name="argLoggedInUser"></param>
        /// <param name="argCandidate"></param>
        public void SaveCandidateInterviewSchedule(string argLoggedInUser, Candidate argCandidate)
        {
            #region Declarations
            HRAnalyticsDBManager l_HRAnalyticsDBManager = new("HRAnalyticsConnection", _configuration);
            List<IDbDataParameter> l_Parameters = new();
            int l_LastID = 0;
            #endregion
            try
            {
                if (argCandidate != null)
                {
                    //Create Parameters
                    l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.LOGGEDINUSER, argLoggedInUser, DbType.String));
                    l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.CANDIDATENAME, argCandidate.CandidateName, DbType.String));
                    l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.JOBID, argCandidate.JobId, DbType.Int32));
                    l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.PROJECTNAME, argCandidate.ProjectName, DbType.String));
                    l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.INTERVIEWERID, argCandidate.InterviewerID, DbType.String));
                    l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.INTERVIEWTIMESTAMP, argCandidate.InterviewTimeStamp, DbType.DateTime));

                    //Call stored procedure
                    l_HRAnalyticsDBManager.Insert(StoredProcedure.INSERT_INTERVIEWSCHEDULE, CommandType.StoredProcedure, l_Parameters.ToArray(), out l_LastID);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get all interviewers to schedule interview for candidate
        /// </summary>
        /// <param name="argUserType"></param>
        /// <returns></returns>
        public UserCollection GetAllInterviewers(string argUserType)
        {
            #region Declarations
            UserCollection l_UserCollection = new();
            HRAnalyticsDBManager l_HRAnalyticsDBManager = new("HRAnalyticsConnection", _configuration);
            List<IDbDataParameter> l_Parameters = new();
            DataTable l_InterviewersDataTable;
            User l_User;
            int l_UserCount;
            #endregion
            try
            {
                l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.USERTYPE, argUserType, DbType.String));

                l_InterviewersDataTable = l_HRAnalyticsDBManager.GetDataTable(StoredProcedure.GET_ALLUSERS, CommandType.StoredProcedure, l_Parameters.ToArray());

                if (l_InterviewersDataTable != null && l_InterviewersDataTable.Rows.Count > 0)
                {
                    l_UserCount = l_InterviewersDataTable.Rows.Count;
                    for (int i = 0; i < l_UserCount; i++)
                    {
                        l_User = new User();

                        DataRow l_Row = l_InterviewersDataTable.Rows[i];

                        l_User.UserID = l_Row["Id"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["Id"]);
                        l_User.UserName = l_Row["UserName"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["UserName"]);
                        l_User.FirstName = l_Row["FirstName"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["FirstName"]);
                        l_User.LastName = l_Row["LastName"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["LastName"]);
                        l_User.EmailID = l_Row["Email"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["Email"]);
                        l_User.FullName = String.Concat(l_User.FirstName, ' ', l_User.LastName);

                        l_UserCollection.Add(l_User);

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return l_UserCollection;
        }


        /// <summary>
        /// Get the candidate information using schedule ID and candidate ID
        /// </summary>
        /// <param name="argCandidateID"></param>
        /// <param name="argScheduleID"></param>
        /// <returns></returns>
        public Candidate GetCandidateInformation(int argCandidateID, int argScheduleID)
        {
            #region Declarations
            HRAnalyticsDBManager l_HRAnalyticsDBManager = new("HRAnalyticsConnection", _configuration);
            List<IDbDataParameter> l_Parameters = new();
            DataTable l_candidateDataTable;
            Candidate l_candidate=new();
            #endregion
            try
            {
                l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.CANDIDATEID, argCandidateID, DbType.Int32));
                l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.SCHEDULEID, argScheduleID, DbType.Int32));

                l_candidateDataTable = l_HRAnalyticsDBManager.GetDataTable(StoredProcedure.GET_CANDIDATEDETAILS, CommandType.StoredProcedure, l_Parameters.ToArray());

                if (l_candidateDataTable != null && l_candidateDataTable.Rows.Count > 0)
                {
                    DataRow l_Row = l_candidateDataTable.Rows[0];

                    l_candidate.CandidateID = l_Row["candidate_id"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["candidate_id"]);
                    l_candidate.CandidateName = l_Row["candidate_name"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["candidate_name"]);
                    l_candidate.JobId = l_Row["job_id"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["job_id"]);
                    l_candidate.JobName = l_Row["job_name"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["job_name"]);
                    l_candidate.InterviewerName = l_Row["interviewer_name"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["interviewer_name"]);
                    l_candidate.ProjectName = l_Row["project_name"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["project_name"]);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return l_candidate;
        }
       
        public CandidateCollection GetCandidateForInterviewer(string argInterviewerID)
        {
            #region Declarations
            CandidateCollection l_CandidateCollection = new();
            HRAnalyticsDBManager l_HRAnalyticsDBManager = new("HRAnalyticsConnection", _configuration);
            List<IDbDataParameter> l_Parameters = new();
            DataTable l_CandidateDataTable;
            Candidate l_Candidate;
            int l_CandidateCount;
            #endregion
            try
            {
                l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.INTERVIEWERID, argInterviewerID, DbType.String));

                l_CandidateDataTable = l_HRAnalyticsDBManager.GetDataTable(StoredProcedure.GET_CANDIDATEDETAILSFORINTERVIEW, CommandType.StoredProcedure, l_Parameters.ToArray());

                if (l_CandidateDataTable != null && l_CandidateDataTable.Rows.Count > 0)
                {
                    l_CandidateCount = l_CandidateDataTable.Rows.Count;
                    for (int i = 0; i < l_CandidateCount; i++)
                    {
                        l_Candidate = new Candidate();

                        DataRow l_Row = l_CandidateDataTable.Rows[i];

                        l_Candidate.CandidateID = l_Row["candidate_id"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["candidate_id"]);
                        l_Candidate.CandidateName = l_Row["candidate_name"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["candidate_name"]);
                        l_Candidate.DateCreated = l_Row["date_created"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["date_created"]);
                        l_Candidate.ProjectName = l_Row["project_name"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["project_name"]);
                        l_Candidate.InterviewTimeStamp = l_Row["interview_timestamp"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(l_Row["interview_timestamp"]);
                        l_Candidate.ScheduleID =  l_Row["schedule_id"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["schedule_id"]);
                        l_Candidate.JobId =  l_Row["job_id"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["job_id"]);
                        l_Candidate.JobName = l_Row["job_name"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["job_name"]);

                        l_CandidateCollection.Add(l_Candidate);

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return l_CandidateCollection;
        }
    }

}
