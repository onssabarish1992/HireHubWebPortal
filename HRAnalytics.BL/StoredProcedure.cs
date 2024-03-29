﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.BL
{
    public class StoredProcedure
    {
        public const string GET_JOBDETAILS = "usp_getJobDetails";
        public const string INSERT_INTERVIEWSCHEDULE = "usp_sav_interviewSchedule";
        public const string GET_ALLUSERS = "usp_getAllUsers";
        public const string GET_CANDIDATEDETAILS = "usp_getCandidateDetails";
        public const string GET_CANDIDATEDETAILSFORINTERVIEW = "usp_getInterviewerCandidates";
        public const string GET_GETEVALUATIONCRITERIA = "usp_getEvaluationCriteria";
        public const string INSERT_SUBCRITERIA = "usp_sav_SubCriteria";
        public const string GET_CRITERIAFORJOB = "usp_getCriteriasForJob";
        public const string GET_JOBCRITERIA = "usp_get_JobCriteria";
        public const string SET_JOBCRITERIA = "usp_sav_JobCriteria";
        public const string GET_CANDIDATEINTERVIEWQUESTIONS = "usp_getInterviewQuestions";
        public const string INSERT_INTERVIEWERRATINGS = "usp_sav_InterviewRatings";
        public const string GET_GLOBAL_SCORE = "usp_getGlobalScores";
        public const string GET_ALLCANDIDATE_RATINGS = "usp_getAllCandidateRatings";
        public const string SAVE_GLOBAL_SCORE = "usp_sav_GlobalScores";
        public const string SAVE_AHP_PAIRS = "usp_sav_AHPPairs";
        public const string SAVE_RESULT = "usp_UpdateCompensation";
        public const string GET_AHPPAIRS = "usp_getAHPPairs";
        public const string SAVE_AHPWEIGHTAGE = "usp_sav_AHPWeightage";
        public const string SAVE_AHPFINALSCORES = "usp_sav_AHPFinalScores";

    }
}
