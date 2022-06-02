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

        public void SaveSubCriteria(string argLoggedInUser, Job argJob)
        {
            #region Declarations
            HRAnalyticsDBManager l_HRAnalyticsDBManager = new("HRAnalyticsConnection", _configuration);
            List<IDbDataParameter> l_Parameters = new();
            int l_LastID = 0;
            #endregion
            try
            {
                if (argJob != null)
                {
                    //Create Parameters
                    l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.CRITERIAID, argJob.CriteriaID, DbType.Int32));
                    l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.SUBCRITERIADESCRIPTION, argJob.SubCriteriaDescription, DbType.String));
                    l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.JOBID, argJob.JobId, DbType.Int32));
                    l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.CREATEDBY, argLoggedInUser, DbType.String));
                   
                    //Call stored procedure
                    l_HRAnalyticsDBManager.Insert(StoredProcedure.INSERT_SUBCRITERIA, CommandType.StoredProcedure, l_Parameters.ToArray(), out l_LastID);
                }
            }
            catch (Exception)
            {
                throw;
            }
      
        }

        public JobCollection GetCriteriaForJob(int argJobId)
        {
            #region Declarations
            JobCollection l_JobCollection = new();
            HRAnalyticsDBManager l_HRAnalyticsDBManager = new("HRAnalyticsConnection", _configuration);
            DataTable l_JobDataTable;
            List<IDbDataParameter> l_Parameters = new();
            Job l_Job;
            int l_JobCount;
            #endregion
            try
            {
                l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.JOBID, argJobId, DbType.Int32));

                l_JobDataTable = l_HRAnalyticsDBManager.GetDataTable(StoredProcedure.GET_CRITERIAFORJOB, CommandType.StoredProcedure);

                if (l_JobDataTable != null && l_JobDataTable.Rows.Count > 0)
                {
                    l_JobCount = l_JobDataTable.Rows.Count;
                    for (int i = 0; i < l_JobCount; i++)
                    {
                        l_Job = new Job();

                        DataRow l_Row = l_JobDataTable.Rows[i];


                        l_Job.CriteriaID = l_Row["criteria_id"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["criteria_id"]);
                        l_Job.JobId = l_Row["job_id"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["job_id"]);
                        l_Job.JobName = l_Row["job_name"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["job_name"]);
                        l_Job.CriteriaName = l_Row["criteria_name"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["criteria_name"]);
                        l_Job.CriteriaDescription = l_Row["criteria_description"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["criteria_description"]);

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

        public JobCollection GetRoles()
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
                l_JobDataTable = l_HRAnalyticsDBManager.GetDataTable(StoredProcedure.GET_JOBCRITERIA, CommandType.StoredProcedure);

                if (l_JobDataTable != null && l_JobDataTable.Rows.Count > 0)
                {
                    l_JobCount = l_JobDataTable.Rows.Count;
                    for (int i = 0; i < l_JobCount; i++)
                    {
                        l_Job = new Job();

                        DataRow l_Row = l_JobDataTable.Rows[i];

                        l_Job.JobId = l_Row["job_id"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["job_id"]);
                        l_Job.Position = l_Row["position_count"] == DBNull.Value ? 0 : Convert.ToInt32(l_Row["position_count"]);
                        l_Job.Compensation = l_Row["compensation"] == DBNull.Value ? 0 : Convert.ToDouble(l_Row["compensation"]);
                        l_Job.JobDescription = l_Row["job_description"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["job_description"]);
                        l_Job.JobName = l_Row["job_name"] == DBNull.Value ? string.Empty : Convert.ToString(l_Row["job_name"]);
                        l_Job.ClosingDate = l_Row["closing_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(l_Row["closing_date"]);
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

        public void SaveRole(string argLoggedInUser, Job argJob)
        {
            #region Declarations
            HRAnalyticsDBManager l_HRAnalyticsDBManager = new("HRAnalyticsConnection", _configuration);
            List<IDbDataParameter> l_Parameters = new();
            int l_LastID = 0;
            #endregion
            try
            {
                if (argJob != null)
                {
                    //Create Parameters
                    l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.JOBID, argJob.JobId, DbType.Int32));
                    l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.POSITION, argJob.Position, DbType.Int32));
                    l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.COMPENSATOIN, argJob.Compensation, DbType.Double));
                    l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.JOBDESCRITPTION, argJob.JobDescription, DbType.String));
                    l_Parameters.Add(l_HRAnalyticsDBManager.CreateParameter(ProcedureParams.CLOSINGDATE, argJob.ClosingDate, DbType.DateTime));

                    //Call stored procedure
                    l_HRAnalyticsDBManager.Insert(StoredProcedure.SET_JOBCRITERIA, CommandType.StoredProcedure, l_Parameters.ToArray(), out l_LastID);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}