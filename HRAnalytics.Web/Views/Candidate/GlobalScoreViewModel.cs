using System;
using System.ComponentModel.DataAnnotations;

namespace HRAnalytics.Web.Models
{
	public class GlobalScoreViewModel
	{

		public int JobId { get; set; }

		public string? JobName { get; set; }

		public string? CandidateName { get; set; }

		public double? ActualCompensation { get; set; }

		public double? ProposedCompensation { get; set; }

		public bool? IsHired { get; set; }

		public string? Recommendation { get; set; }

		public int ScheduleId { get; set; }

		public int GlobalScoreId { get; set; }
	}
}

