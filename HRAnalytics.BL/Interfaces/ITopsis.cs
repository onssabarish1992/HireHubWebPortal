using HRAnalytics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.BL.Interfaces
{
    public interface ITopsis
    {
        List<Alternative> ComputeTopsisScore(List<Alternative> argAlternatives, List<Criteria> argCriteria);
    }
}
