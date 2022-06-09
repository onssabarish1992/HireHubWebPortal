using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.Entities
{
    public class Alternative
    {
        public string Name { get; set; }

        public List<CriteriaValue> criteriaValues;

        public double calculatedPerformance;
    }
}
