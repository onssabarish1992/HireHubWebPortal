using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.Entities
{
    public class AHPPair
    {
        public int PairId { get; set; }

        public int Pair1 { get; set; }

        public int Pair2 { get; set; }

        public int EntityId { get; set; }

        public int Weightage { get; set; }

        public int ParentEntityId { get; set; }
    }
}
