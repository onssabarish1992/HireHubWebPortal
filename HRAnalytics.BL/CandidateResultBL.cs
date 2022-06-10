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
        public CandidateResultBL(IJobBL argJobBL)
        {
            _IJobBL = argJobBL;
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
                
                

            }
            catch (Exception)
            {

                throw;
            }

            return candidateList;
        }
    }
}
