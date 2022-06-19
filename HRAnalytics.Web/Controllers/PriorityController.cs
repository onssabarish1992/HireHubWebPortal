using HRAnalytics.Entities;
using HRAnalytics.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace HRAnalytics.Web.Controllers
{
    public class PriorityController : HRAnalyticsBaseController
    {
        public PriorityController(IConfiguration configuration) : base(configuration)
        {
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> GetAHPJobPairs()
        {
            try
            {
                var l_AHPJobPairs = await GetAHPCreatedPairs(1, null);

                List<AHPPairViewModel> l_AHPPairViewModel = ConvertPairsToViewModel(l_AHPJobPairs);

                return PartialView("_partialSlider", l_AHPPairViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveAHPRatings([FromBody]List<RatingViewModel> AHPPairs)
        {
            #region Declarations
            bool IsSuccess = false;
            HttpResponseMessage l_Message = new HttpResponseMessage();
            #endregion

            List<AHPPair> l_AHPPairs = ConvertAHPPairToEntity(AHPPairs);

            l_Message = await SaveAHPRating(l_AHPPairs);

            if (l_Message != null && l_Message.IsSuccessStatusCode)
            {
                IsSuccess = true;
            }

            return Json(IsSuccess);

        }

        private async Task<HttpResponseMessage> SaveAHPRating(List<AHPPair> argPairs)
        {
            #region Declarations
            HttpResponseMessage l_Response = new HttpResponseMessage();
            string l_SaveAHPRatingsURL = apiBaseURL + "api/AHP/SaveAHPWeightage?argLoggedInUserID=" + GetLoggedInUserID();
            #endregion
            try
            {
                if (argPairs != null)
                {
                   l_Response = await client.PostAsJsonAsync(l_SaveAHPRatingsURL, argPairs);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return l_Response;
        }

        private List<AHPPair> ConvertAHPPairToEntity(List<RatingViewModel> postData)
        {
            List<AHPPair> l_AHPPair=new List<AHPPair>();
            AHPPair l_Pair;
            try
            {
                foreach (var item in postData)
                {
                    l_Pair = new AHPPair();
                    l_Pair.PairId = item.PairId;
                    l_Pair.Weightage = item.Rating;
                    l_Pair.Pair1Name = String.Empty;
                    l_Pair.Pair2Name = String.Empty;
                    l_AHPPair.Add(l_Pair);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return l_AHPPair;
        }

        private List<AHPPairViewModel> ConvertPairsToViewModel(List<AHPPair> l_AHPJobPairs)
        {
            List<AHPPairViewModel> l_PairModel = new();
            AHPPairViewModel l_PairViewModel;
            try
            {
                foreach (var pr in l_AHPJobPairs)
                {
                    l_PairViewModel = new AHPPairViewModel();
                    l_PairViewModel.PairId = pr.PairId;
                    l_PairViewModel.Pair1Name = pr.Pair1Name;
                    l_PairViewModel.Pair2Name = pr.Pair2Name;
                    l_PairViewModel.Pair1 = pr.Pair1;
                    l_PairViewModel.Pair2 = pr.Pair2;
                    l_PairViewModel.EntityId = pr.EntityId;
                    l_PairViewModel.Weightage=pr.Weightage;
                    l_PairViewModel.ParentEntityId = pr.ParentEntityId;

                    l_PairModel.Add(l_PairViewModel);
                }
            }
            catch (Exception)
            { 
                throw;
            }

            return l_PairModel;
        }

        public async Task<List<AHPPair>> GetAHPCreatedPairs(int argEntityID, int? argParentEntityID)
        {
            #region Declarations
            List<AHPPair> l_AHPPairs = new();
            string l_pairURL = string.Empty;
            #endregion
            try
            {
                l_pairURL = apiBaseURL + "api/AHP/GetAHPPairs?argEntityID="+ argEntityID+ "&argParentEntityID ="+ argParentEntityID;

                HttpResponseMessage l_pairData = await client.GetAsync(l_pairURL);

                if (l_pairData != null && l_pairData.IsSuccessStatusCode)
                {
                    var l_JobResponse = l_pairData.Content.ReadAsStringAsync().Result;
                    l_AHPPairs = JsonConvert.DeserializeObject<List<AHPPair>>(l_JobResponse);
                }

            }
            catch (Exception)
            {
                throw;
            }
            return l_AHPPairs;
        }
    }
}
