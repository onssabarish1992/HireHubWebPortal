using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.Entities
{
    public class CandidateScore
    {
        public Candidate Candidate { get; set; }
        public List<CandidateEvaluation> Evaluation { get; set; }

    }
}
