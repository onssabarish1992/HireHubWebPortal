using System;
namespace HRAnalytics.BL
{
	public interface IEmailService
	{
		Task<bool> SendEmail(String CandidateName, String PrjName, DateTime IntTime);

	}
}

