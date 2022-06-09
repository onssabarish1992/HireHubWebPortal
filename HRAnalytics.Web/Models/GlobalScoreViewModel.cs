﻿using System;
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

		public int? IsHired { get; set; }
	}
}
