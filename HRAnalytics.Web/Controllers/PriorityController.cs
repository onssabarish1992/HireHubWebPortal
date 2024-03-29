﻿using HRAnalytics.Entities;
using HRAnalytics.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;


namespace HRAnalytics.Web.Controllers
{
    [Authorize(Roles = "HR")]
    public class PriorityController : HRAnalyticsBaseController
    {
        public PriorityController(IConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        /// Landing page for AHP
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Partial view to load AHP job pairs
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// AJAX Post method to save AHP ratings
        /// </summary>
        /// <param name="AHPPairs"></param>
        /// <returns></returns>
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

        /// <summary>
        /// API call to save AHP rating
        /// </summary>
        /// <param name="argPairs"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Convert AHP roles/criteria to entity which will be passed to API
        /// </summary>
        /// <param name="postData"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Convert Entity to View Model
        /// </summary>
        /// <param name="l_AHPJobPairs"></param>
        /// <returns></returns>
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

        /// <summary>
        /// API call to get AHP Pairs
        /// </summary>
        /// <param name="argEntityID"></param>
        /// <param name="argParentEntityID"></param>
        /// <returns></returns>
        public async Task<List<AHPPair>> GetAHPCreatedPairs(int argEntityID, int? argParentEntityID)
        {
            #region Declarations
            List<AHPPair> l_AHPPairs = new();
            string l_pairURL = string.Empty;
            #endregion
            try
            {
                var l_ParentEntityID = argParentEntityID.HasValue ? argParentEntityID.Value : 0;
                l_pairURL = apiBaseURL + "api/AHP/GetAHPPairs?argEntityID="+ argEntityID+ "&argParentEntityID=" + l_ParentEntityID;

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

        /// <summary>
        /// Default page for loading job criteria pairs
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Criteria()
        {
            await PopulateDropdownValues();
            return View();
        }

        /// <summary>
        /// This function is used to populate the criteria dropdown
        /// </summary>
        /// <returns></returns>
        private async Task<bool> PopulateDropdownValues()
        {
            bool l_Executed;
            try
            {
                //This API call which fetch only the list of job roles created in set criteria screen 
                var JobsCreated = await GetJobCriterias();

                ViewBag.Citeria = Enumerable.Empty<SelectListItem>();
                ViewBag.Jobs = new SelectList(JobsCreated, "JobId", "JobName");

                l_Executed = true;
            }
            catch (Exception)
            {
                throw;
            }
            return l_Executed;
        }

        /// <summary>
        /// Partial View for AHP roles/criteria
        /// </summary>
        /// <param name="argJobIdParam"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> GetAHHCriteriaPairs(int argJobIdParam)
        {
            try
            {
                var l_AHPJobPairs = await GetAHPCreatedPairs(2, argJobIdParam);

                List<AHPPairViewModel> l_AHPPairViewModel = ConvertPairsToViewModel(l_AHPJobPairs);

                return PartialView("_partialSlider", l_AHPPairViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
