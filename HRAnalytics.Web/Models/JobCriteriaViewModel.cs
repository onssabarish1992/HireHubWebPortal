using System.ComponentModel.DataAnnotations;

namespace HRAnalytics.Web.Models
{
    public class JobCriteriaViewModel
    {
        [Required(ErrorMessage = "Job Role is required")]
        public int JobId { get; set; }

        [Required(ErrorMessage = "Please enter position value")]
        public int Position { get; set; }

        [Required(ErrorMessage = "Please enter compensation amount")]
        public double Compensation { get; set; }

        [Required(ErrorMessage = "Please enter Job Description")]
        public string? JobDescription { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ClosingDate { get; set; }

        public string? JobName { get; set; }

        public int? JobCriteriaID { get; set; }

        public string? Mode { get; set; }


    }

    public class JobRoleViewModel
    {
        public JobCriteriaViewModel? JobCriteriaViewModel { get; set; }

        public List<JobCriteriaViewModel>? CiiteriasCreated { get; set; }

        //public int? JobCrtID { get; set; }

        //public string? Mde { get; set; }

    }
}
