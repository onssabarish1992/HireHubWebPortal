using HRAnalytics.BL.Interfaces;
using HRAnalytics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.BL
{
    public class CandidateResultBL: ICandidateResult
    {
        private IJobBL _IJobBL;
        private ICandidateScore _IScoreBL;
        private ITopsis _ITopsisBL;
        public CandidateResultBL(IJobBL argJobBL, ICandidateScore argScoreBL, ITopsis argTopsisBL)
        {
            _IJobBL = argJobBL;
            _IScoreBL = argScoreBL;
            _ITopsisBL = argTopsisBL;
        }

        /// <summary>
        /// This method is used to save 
        /// </summary>
        /// <returns></returns>
        public List<Candidate> GetCandidateResult()
        {
            #region Declaration
            List<Candidate> candidateList = new List<Candidate>();
            #endregion
            try
            {
                //Fetch all the roles
                var l_roles = _IJobBL.GetRoles();

                //Fetch all evaluation criterias
                var l_criterias = _IJobBL.GetEvaluationCriteria();

                //Get the scores obtained by candidate
                var l_scores = _IScoreBL.GetAllCandidateScores();

                //Get Rankings of candidates
                candidateList = GetCandidateRankings(l_roles, l_criterias, l_scores);

            }
            catch (Exception)
            {

                throw;
            }

            return candidateList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="l_roles"></param>
        /// <param name="l_criterias"></param>
        /// <param name="l_scores"></param>
        /// <returns></returns>
        private List<Candidate> GetCandidateRankings(JobCollection l_roles, JobCollection l_criterias, List<CandidateEvaluation> l_scores)
        {
            #region Declaration
            List<Alternative> l_aggregatedResults = new List<Alternative>();
            List<Candidate> l_candidatesSet = new List<Candidate>();
            #endregion

            try
            {
                foreach (var role in l_roles)
                {
                    //Get sub criterias based on the job id
                    var l_requiredCriterias = l_criterias.Where(x => x.JobId == role.JobId);

                    if(l_requiredCriterias!=null)
                    {
                        //Get all candidates for that job id
                        var l_reqdCandidates = l_scores.Where(x => x.JobId == role.JobId);

                        //Create criteria for Topsis
                        List<Criteria> criterias = GetCriteriasForRole(l_requiredCriterias);

                        //Create required alternatives for topsis
                        List<Alternative> alternatives = GetAlternativesForTheCriteria(l_reqdCandidates, criterias);

                        //Compute Topsis score for that job role
                        var alternativeResult = _ITopsisBL.ComputeTopsisScore(alternatives, criterias);

                        //Create the candidate dataset
                        l_candidatesSet.AddRange(CreateCandidateScores(alternativeResult, l_scores, role));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return l_candidatesSet;
        }

        /// <summary>
        /// Create the candidate object from alternatives 
        /// </summary>
        /// <param name="argAlternativeResult"></param>
        /// <param name="l_scores"></param>
        /// <param name="argJobId"></param>
        /// <returns></returns>
        private List<Candidate> CreateCandidateScores(List<Alternative> argAlternativeResult, 
                                                      List<CandidateEvaluation> l_scores, 
                                                      Job argRole)
        {
            #region Declaration
            List<Candidate> l_Candidate = new List<Candidate>();
            Candidate candidate;
            #endregion

            try
            {
                foreach (var alternative in argAlternativeResult)
                {
                    candidate = new Candidate();
                    candidate.JobId = argRole.JobId;
                    candidate.CandidateID = Convert.ToInt32(alternative.Name);
                    candidate.ScheduleID = l_scores.Where(x => x.CandidateID == candidate.CandidateID && x.JobId == argRole.JobId).Select(y => y.ScheduleID.Value).FirstOrDefault();
                    candidate.LocalScore = alternative.calculatedPerformance;
                    candidate.GlobalScore = candidate.LocalScore * argRole.Weightage;
                    candidate.ActualCompensation = argRole.Compensation;
                    l_Candidate.Add(candidate);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return l_Candidate;
        }

        /// <summary>
        /// Create alternatives for TOPSIS method
        /// </summary>
        /// <param name="l_reqdCandidates"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private List<Alternative> GetAlternativesForTheCriteria(IEnumerable<CandidateEvaluation> argReqdCandidates, 
                                                                List<Criteria> argCriterias)
        {
            #region Declarations
            List<Alternative> l_alternatives = new();
            Alternative l_alternative;
            CriteriaValue l_criteriaValue;
            Criteria l_Criteria;
            List<CriteriaValue> l_CriteriaValues;
            #endregion
            try
            {
                var distinctCandidates = argReqdCandidates.Select(x => x.CandidateID).Distinct();

                foreach (var candidate in distinctCandidates)
                {
                    l_alternative = new Alternative();
                    l_alternative.Name = Convert.ToString(candidate);

                    //Form the distinct criteria for each candidate
                    var candidateCriterias = argReqdCandidates.Where(x => x.CandidateID == candidate);

                    //Reset the list
                    l_CriteriaValues = new List<CriteriaValue>();

                    foreach (var criteria in candidateCriterias)
                    {
                        l_criteriaValue = new CriteriaValue();
                        l_criteriaValue.Value = argReqdCandidates.Where(x => x.CandidateID == candidate 
                                                && x.CriteriaId == criteria.CriteriaId)
                                                .Select(y=>y.CriteriaScore).FirstOrDefault();
                        l_criteriaValue.criteria = argCriterias.Where(x => x.Name == Convert.ToString(criteria.CriteriaId)).FirstOrDefault();
                        l_CriteriaValues.Add(l_criteriaValue);
                    }

                    l_alternative.criteriaValues = l_CriteriaValues;
                    l_alternatives.Add(l_alternative);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return l_alternatives;
        }

        /// <summary>
        /// Form criteria for Topsis
        /// </summary>
        /// <param name="l_requiredCriterias"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private List<Criteria> GetCriteriasForRole(IEnumerable<Job> argRequiredCriterias)
        {
            #region Declaration
            List<Criteria> l_criterias = new();
            Criteria l_Criteria;
            #endregion
            try
            {
                foreach (var criteria in argRequiredCriterias)
                {
                    l_Criteria = new Criteria();
                    l_Criteria.Name = Convert.ToString(criteria.CriteriaID);
                    l_Criteria.Weight = criteria.SubCriteriaWeightage;
                    l_Criteria.IsNegative = false; //Will be set as false for all the entries

                    l_criterias.Add(l_Criteria);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return l_criterias;
        }
    }
}
