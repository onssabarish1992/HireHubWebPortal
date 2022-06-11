using HRAnalytics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.BL.Interfaces
{
    public interface ICandidateScore
    {
        CandidateScore GetCandidateScore(int argScheduleID);

        void SaveInterviewRatings(string argLoggedInUserID, int argScheduleID, List<CandidateEvaluation> argCandidateEvaluations);


        List<CandidateEvaluation> GetAllCandidateScores();

        void SaveGlobalScores(string argLoggedInUser, List<Candidate> argGlobalScores);

    }
}
