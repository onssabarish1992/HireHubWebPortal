using HRAnalytics.BL.Interfaces;
using HRAnalytics.DAL;
using HRAnalytics.Entities;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace HRAnalytics.BL
{
    public class JobBL: IJobBL
    {
        private readonly IConfiguration _configuration;

        public JobBL(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// This method is used to get all job details from the database details
        /// </summary>
        /// <returns>List of Job Details</returns>
        public JobCollection GetAllJobs()
        {
            #region Declarations
            JobCollection l_JobCollection = new();
            HRAnalyticsDBManager l_HRAnalyticsDBManager = new("HRAnalyticsConnection", _configuration);
            DataTable l_JobDataTable;
            Job l_Job;
            int l_JobCount;
            #endregion
            try
            {
                l_JobDataTable = l_HRAnalyticsDBManager.GetDataTable(StoredProcedure.GET_JOBDETAILS, CommandType.StoredProcedure);

                if (l_JobDataTable != null && l_JobDataTable.Rows.Count > 0)
                {
                    l_JobCount = l_JobDataTable.Rows.Count;
                    for (int i = 0; i < l_JobCount; i++)
                    {
                        l_Job = new Job();

                        DataRow l_Row = l_JobDataTable.Rows[i];

                        l_Job.JobId = l_Row["job_id"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["job_id"]);
                        l_Job.JobName = l_Row["job_name"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["job_name"]);
                        l_Job.JobDescription = l_Row["job_description"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["job_description"]);

                        l_JobCollection.Add(l_Job);

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return l_JobCollection;
        }

        public JobCollection GetEvaluationCriteria()
        {
            #region Declarations
            JobCollection l_JobCollection = new();
            HRAnalyticsDBManager l_HRAnalyticsDBManager = new("HRAnalyticsConnection", _configuration);
            DataTable l_JobDataTable;
            Job l_Job;
            int l_JobCount;
            #endregion
            try
            {
                l_JobDataTable = l_HRAnalyticsDBManager.GetDataTable(StoredProcedure.GET_GETEVALUATIONCRITERIA, CommandType.StoredProcedure);

                if (l_JobDataTable != null && l_JobDataTable.Rows.Count > 0)
                {
                    l_JobCount = l_JobDataTable.Rows.Count;
                    for (int i = 0; i < l_JobCount; i++)
                    {
                        l_Job = new Job();

                        DataRow l_Row = l_JobDataTable.Rows[i];

                        l_Job.DateCreated = l_Row["date_created"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["date_created"]);
                        l_Job.JobName = l_Row["job_name"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["job_name"]);
                        l_Job.CriteriaName = l_Row["criteria_name"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["criteria_name"]);

                        l_JobCollection.Add(l_Job);

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return l_JobCollection;
        }
    }
}