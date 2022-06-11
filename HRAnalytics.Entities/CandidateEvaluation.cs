using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.Entities
{
    public class CandidateEvaluation
    {
        public int JobId { get; set; }

        public int CriteriaId { get; set; }

        public string CriteriaName { get; set; }

        public string CriteriaDescription { get; set; }

        public int CriteriaScore { get; set; }

        public string CriteriaComments { get; set; }

        public int CandidateID { get; set; }
        public string CandidateName { get; set; }

        public int? ScheduleID{ get; set; }
    }
}
