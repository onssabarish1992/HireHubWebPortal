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
    }

    public class JobCollection: List<Job>
    {

    }
}
