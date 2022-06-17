using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.Entities
{
    public class Job
    {
        public int JobId { get; set; }

        public string? JobName { get; set; }

        public string? JobDescription { get; set; }

        public string? CriteriaName { get; set; }

        public string? DateCreated { get; set; }

        public int CriteriaID { get; set; }

        public string? SubCriteriaDescription { get; set; }

        public double Weightage { get; set; }

        public string? CreatedBy { get; set; }

        public string? CriteriaDescription { get; set; }

        public int Position { get; set; }

        public double Compensation { get; set; }

        public DateTime? ClosingDate { get; set; }

        public double SubCriteriaWeightage { get; set; }

        public int? JobCriteriaId { get; set; }

        public int? SubCriteriaId { get; set; }

        public string? Mode { get; set; }

    }

    public class JobCollection: List<Job>
    {

    }
}
