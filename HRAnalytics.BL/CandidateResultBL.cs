using HRAnalytics.BL.Interfaces;
using HRAnalytics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.BL
{
    public class CandidateResultBL
    {
        private IJobBL _IJobBL;
        private ICandidateScore _IScoreBL;
        public CandidateResultBL(IJobBL argJobBL, ICandidateScore argScoreBL)
        {
            _IJobBL = argJobBL;
            _IScoreBL = argScoreBL;
        }

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

        private List<Candidate> GetCandidateRankings(JobCollection l_roles, JobCollection l_criterias, List<CandidateEvaluation> l_scores)
        {
            #region Declaration
            List<Candidate> l_candidates = new List<Candidate>();
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
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return l_candidates;
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
            List<CriteriaValue> l_CriteriaValues = new();
            #endregion
            try
            {
                var groupCandidatesByID = from candidate in argReqdCandidates
                                          group candidate by candidate.CandidateID into groupedCandidates
                                          orderby groupedCandidates.Key
                                          select groupedCandidates;

                foreach (var candidate in groupCandidatesByID)
                {
                    var l_reqdCandidates = argReqdCandidates.Where(x => x.CandidateID == candidate.Key);
                    l_alternative = new Alternative();

                    foreach (var cd in l_reqdCandidates)
                    {
                        //Form criteria values
                        l_criteriaValue = new CriteriaValue();
                        l_Criteria = argCriterias.Where(x => x.Name == Convert.ToString(cd.CriteriaId)).FirstOrDefault();

                        l_criteriaValue.criteria = l_Criteria;
                        l_criteriaValue.Value = cd.CriteriaScore;

                        //Add criter
                        l_CriteriaValues.Add(l_criteriaValue);

                    }

                    //Create alternatives
                    l_alternative.criteriaValues = l_CriteriaValues;
                    l_alternative.Name = Convert.ToString(candidate.Key);
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
                    l_Criteria.Weight = criteria.Weightage;
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
