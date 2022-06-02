namespace HRAnalytics.Entities
{
    public class Candidate
    {
        public int CandidateID { get; set; }

        public string CandidateName { get; set; }

        public string ProjectName{ get; set; }

        public string InterviewerID { get; set; }

        public DateTime InterviewTimeStamp { get; set; }

        public string? InterviewerName { get; set; }

        public string? JobName { get; set; }

        public int JobId { get; set; }

        public string? DateCreated { get; set; }

        public int ScheduleID { get; set; }
    }

    public class CandidateCollection: List<Candidate>
    {

    }
}