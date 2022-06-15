using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.BL.Interfaces
{
    public interface IAHP
    {
        List<string> generatePairs(List<string> argPairs);

        void SavePairs(int argEntityID, int? argParentEntityID, string argLoggedInUser);
    }
}
