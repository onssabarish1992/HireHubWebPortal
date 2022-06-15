using HRAnalytics.BL.Interfaces;
using HRAnalytics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.BL
{
    public class AHP: IAHP
    {
        private IJobBL _jobBL;
        public AHP(IJobBL jobBL)
        {
            _jobBL = jobBL;
        }

        public void SavePairs(int argEntityID, int? argParentEntityID)
        {
            List<AHPPair> l_FinalPairs=new List<AHPPair>();
            try
            {
                //Get the entity based on entity ID
                if(argEntityID == 1)
                {
                    //Get the job roles 
                    var l_jobs = _jobBL.GetRoles();

                    if(l_jobs!=null && l_jobs.Count > 0)
                    {
                        var l_Orderedjobs = l_jobs.OrderBy(x => x.JobCriteriaId).ToList();

                        List<string> l_Jobs = l_Orderedjobs.Select(x => Convert.ToString(x.JobId)).ToList();

                        List<string> l_PairsGenerated = generatePairs(l_Jobs);

                        l_FinalPairs = CreateAHPPairs(l_PairsGenerated, argEntityID);
                    }
                }
                else if(argEntityID == 2)
                {
                    //get the job criterias based on parent entity ID
                    var l_jobCriteria = _jobBL.GetEvaluationCriteria();

                    var l_FilteredjobCriteria = l_jobCriteria.Where(x => x.JobId == argParentEntityID).OrderBy(y => y.SubCriteriaId);

                    List<string> l_Criterias = l_FilteredjobCriteria.Select(x => Convert.ToString(x.CriteriaID)).ToList();

                    List<string> l_PairsGenerated = generatePairs(l_Criterias);

                    l_FinalPairs = CreateAHPPairs(l_PairsGenerated, argEntityID);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }


        private List<AHPPair> CreateAHPPairs(List<string> argPairs, int argEntityID)
        {
            List<AHPPair> l_AHPPairs = new List<AHPPair>();
            AHPPair l_AhpPair;
            try
            {
                foreach (var pair in argPairs)
                {
                    l_AhpPair = new AHPPair();
                    var pairDetails = pair.Split("|");
                    l_AhpPair.Pair1 = Convert.ToInt32(pairDetails[0]);
                    l_AhpPair.Pair2 = Convert.ToInt32(pairDetails[1]);

                    l_AhpPair.EntityId = argEntityID;

                    l_AHPPairs.Add(l_AhpPair);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return l_AHPPairs;
        }

        /// <summary>
        /// This method is used to generate pairs for AHP
        /// </summary>
        /// <param name="argPairs"></param>
        /// <returns></returns>
        public List<string> generatePairs(List<string> argPairs)
        {
            List<string> l_pairs = new List<string>();
            try
            {
                if(argPairs!=null && argPairs.Count > 0)
                {
                    for (int i = 0; i < argPairs.Count; i++)
                    {
                        for (int j = 1 + 1; j < argPairs.Count; j++)
                        {
                            l_pairs.Add(string.Concat(argPairs[i], "|", argPairs[j]));
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return l_pairs;
        }
    }
}
