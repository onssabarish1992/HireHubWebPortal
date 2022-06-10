using HRAnalytics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.BL.Interfaces
{
    public interface ICandidateResult
    {
        List<Candidate> GetCandidateResult();
    }
}
