using System.ComponentModel.DataAnnotations;

namespace HRAnalytics.Web.Models
{
    public class SubcriteriaViewModel
    {
        [Required(ErrorMessage = "Please select Job Role")]
        public int JobId { get; set; }

        [Required(ErrorMessage = "Please select subcriteria")]
        public int CriteriaId { get; set; }

        [Required(ErrorMessage = "Please enter description")]
        public string CriteriaDescription { get; set; }
        public string? DisplayCriterias { get; set; }
        public string? JobName { get; set; }
        public string? CriteriaName { get; set; }
        public int? CriteriaCount { get; set; }
    }

    public class CriteriaViewModel
    {
        public SubcriteriaViewModel? Criteria { get; set; }

        public List<SubcriteriaViewModel>? CriteriasCreated { get; set; }
    }
}
