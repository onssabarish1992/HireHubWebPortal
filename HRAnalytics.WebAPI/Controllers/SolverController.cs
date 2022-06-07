
using Google.OrTools.LinearSolver;
using HRAnalytics.BL.Interfaces;
using HRAnalyticsPrescriptiveAPI.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRAnalytics.WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SolverController : ControllerBase
    {
        private readonly TelemetryClient _telemetryClient;

        private ISolverBL _solver;

        public SolverController(TelemetryClient telemetryClient, ISolverBL solverBL)
        {
            _telemetryClient = telemetryClient;
            _solver = solverBL;
        }

        /// <summary>
        /// API to solve prescriptive analytics 
        /// </summary>
        /// <param name="argInput"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Solve")]
        public IActionResult Solve([FromBody] Input argInput)
        {

            SolverResult result = new();
            try
            {


                result = _solver.SolveEquation(argInput);

                _telemetryClient.TrackTrace("Solve Equation returned result as " + ":" + result);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackTrace("Exception caught while solving model...");
                _telemetryClient.TrackException(ex);
            }

            return Ok(result);
        }
    }

        
}
