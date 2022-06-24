using System;
using System.Text.Json;
using System.Net.Http;
using System.Configuration;
using System.Text;

namespace HRAnalytics.BL
{
	public class EmailService : IEmailService
	{
		public EmailService()
		{
		}

		public async Task<bool> SendEmail(String CandidateName, String PrjName, DateTime IntTime)
        {

            bool IsSuccess = false;

            // requires using System.Net.Http;
            var client = new HttpClient();
            // requires using System.Text.Json;
            var jsonData = JsonSerializer.Serialize(new
            {
                email = "hirehub@outlook.ie",
                due = CandidateName,
                task = "Hire Hub - Interview Scheduled Notification",
                ProjectName = PrjName,
                InterviewTime = IntTime.ToString("MM / dd / yyyy h: mm tt"),

            });

            HttpResponseMessage result = await client.PostAsync(
                "https://prod-13.centralus.logic.azure.com:443/workflows/f49931b8e1144a0e900d022d2a41f5ff/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=jVvSPcejc2QTLAt2l_ZuOMo-GQmWG9QNrt_wXqazzcI",
                new StringContent(jsonData, Encoding.UTF8, "application/json"));


            if (result != null && result.IsSuccessStatusCode)
            {
                IsSuccess = true;
            }


            return IsSuccess;
        }
	}
}

