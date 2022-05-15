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
    }

}
