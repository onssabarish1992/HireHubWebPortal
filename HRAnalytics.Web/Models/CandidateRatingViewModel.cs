using System.ComponentModel.DataAnnotations;

namespace HRAnalytics.Web.Models
{
    public class CandidateRatingViewModel
    {
        public int CriteriaId { get; set; }

        public string? CriteriaName { get; set; }

        public string? CriteriaDescription { get; set; }

        [Required(ErrorMessage = "Please provide ratings")]
        public int? Rating { get; set; }

        [Required(ErrorMessage = "Please Enter Comments")]
        public string Comments { get; set; }

        public int? JobId { get; set; }
    }

    public class InterviewCandidateViewModel
    {
        public int? CandidateId { get; set; }

        public string? CandidateName { get; set; }

        public string? RoleName { get; set; }

        public int? ScheduleID { get; set; }
    }

    public class CandidateScoreViewModel
    {
        public InterviewCandidateViewModel? candidateDetail { get; set; }

        public List<CandidateRatingViewModel>? candidateRatings { get; set; }
    }
}
