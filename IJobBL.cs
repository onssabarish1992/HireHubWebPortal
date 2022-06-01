using HRAnalytics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.BL.Interfaces
{
    public interface IJobBL
    {
        JobCollection GetAllJobs();
        JobCollection GetEvaluationCriteria();

        void SaveSubCriteria(string argLoggedInUser, Job argJob);
        JobCollection GetCriteriaForJob(int argJobId);

        JobCollection GetRoles();
        void SaveRole(string argLoggedInUser, Job argJob);
    }
}
