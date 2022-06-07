using System.ComponentModel.DataAnnotations;

namespace HRAnalytics.Web.Models
{
    public class CandidateViewModel
    {
        [Required(ErrorMessage = "Please enter candidate name")]
        public string? CandidateName { get; set; }

        public string? Project { get; set; }

        public string? FullName { get; set; }

        [Required(ErrorMessage = "Please select interview schedule")]
        public DateTime InterviewSchedule { get; set; }

        public string? JobName { get; set; }

        [Required(ErrorMessage = "Please select role")]
        public int JobId { get; set; }

        [Required(ErrorMessage = "Please select interviewer")]
        public string? UserID { get; set; }

        public int? ScheduleID { get; set; }
    }
}
