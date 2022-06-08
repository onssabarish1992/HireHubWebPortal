using HRAnalytics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.BL.Interfaces
{
    public  interface ICandidateBL
    {
        void SaveCandidateInterviewSchedule(string argLoggedInUser, Candidate argCandidate);
        UserCollection GetAllInterviewers(string argUserType);

        Candidate GetCandidateInformation(int argCandidateID, int argScheduleID);
        CandidateCollection GetCandidateForInterviewer(string argInterviewerID);
        CandidateCollection GetGlobalScores();


    }
}
