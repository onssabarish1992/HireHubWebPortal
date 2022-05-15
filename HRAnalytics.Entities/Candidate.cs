namespace HRAnalytics.Entities
{
    public class Candidate
    {
        public int CandidateID { get; set; }

        public string CandidateName { get; set; }

        public string ProjectName{ get; set; }

        public string InterviewerID { get; set; }

        public DateTime InterviewTimeStamp { get; set; }

        public int JobId { get; set; }
    }
}