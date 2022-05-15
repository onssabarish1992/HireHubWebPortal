using System.ComponentModel.DataAnnotations;

namespace HRAnalytics.Web.Models
{
    public class CandidateViewModel
    {
        [Required(ErrorMessage = "Please enter candidate name")]
        public string CandidateName { get; set; }

        public string Project { get; set; }

        [Required(ErrorMessage = "Please select role")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Please select interviewer")]
        public string Interviewer { get; set; }

        [Required(ErrorMessage = "Please select interview schedule")]
        public DateTime InterviewSchedule { get; set; }
    }
}
