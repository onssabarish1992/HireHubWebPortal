using HRAnalytics.BL.Interfaces;
using HRAnalytics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.BL
{
    public class Topsis: ITopsis
    {

        private double[] l_IdealBest;
        private double[] l_IdealWorst;
        private double[] l_distancesFromIdealWorst;
        private double[] l_distancesFromlIdealBest;

        /// <summary>
        /// Compute the global score of candidates
        /// </summary>
        /// <param name="argAlternatives"></param>
        /// <param name="argCriteria"></param>
        /// <returns></returns>
        public List<Alternative> ComputeTopsisScore(List<Alternative> argAlternatives, List<Criteria> argCriteria)
        {
            List<Alternative> l_alternatives = new List<Alternative>();
            int l_CriteriaCount = argCriteria.Count;
            int l_alternativesCount = argAlternatives.Count;
            try
            {
                
                //Step 1: Create score matrix
                double[,] l_scoreMatrix = createScoreMatrix(argAlternatives, l_CriteriaCount);

                //Step 2: Create Normalized matrix
                double[,] l_NormalizedMatrix = createNormalizedDecisionMatrix(argCriteria, l_scoreMatrix, l_alternativesCount);

                //Find best and worst solution
                findIdealBestAndWorst(l_CriteriaCount, l_alternativesCount, l_NormalizedMatrix, argCriteria);

                //Compute Euclidean distance from best and worst
                computeEuclidianDistancesFromIdealBestAndWorst(l_NormalizedMatrix, l_alternativesCount, l_CriteriaCount);

                //Compute performance score of all alternatives
                l_alternatives = computePerformanceScore(argAlternatives);
            }
            catch (Exception)
            {

                throw;
            }

            return l_alternatives;
        }

        /// <summary>
        /// Used to create score matrix based on alternatives
        /// </summary>
        private double[,] createScoreMatrix(List<Alternative> argalternatives, int argCriteriaCount)
        {
            double[,] scoreMatrix;
            try
            {
                int l_alternativesCount = argalternatives.Count;
                scoreMatrix = new double[l_alternativesCount, argCriteriaCount];
                
                int rowIndex = 0;
                int columnIndex = 0;
                foreach (var altr in argalternatives)
                {
                    columnIndex = 0;
                    foreach (var crt in altr.criteriaValues)
                    {
                        scoreMatrix[rowIndex, columnIndex] = crt.Value;
                        columnIndex++;
                    }
                    rowIndex++;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return scoreMatrix;
        }

        /// <summary>
        /// Create normalized decision matrix 
        /// </summary>
        /// <param name="argScoreMatrix"></param>
        /// <param name="argAlternativesCount"></param>
        /// <param name="argCriteriaCount"></param>
        /// <returns></returns>
        private double[,] createNormalizedDecisionMatrix(List<Criteria> argCriteria, double[,] argScoreMatrix, int argAlternativesCount)
        {
            int l_criteriaCount  = argCriteria.Count();
            double[,] normalizedDecisionMatrix = new double[argAlternativesCount, l_criteriaCount];
            try
            {
                for (int c = 0; c < l_criteriaCount; c++)
                {
                    double divisor = 0;
                    for (int a = 0; a < argAlternativesCount; a++)
                    {
                        divisor += argScoreMatrix[a, c] * argScoreMatrix[a, c];
                    }
                    divisor = Math.Pow(divisor, 0.5);

                    for (int a = 0; a < argAlternativesCount; a++)
                    {
                        double normalizedValue = argScoreMatrix[a, c] / divisor;
                        normalizedValue = normalizedValue * argCriteria[c].Weight;
                        normalizedDecisionMatrix[a, c] = normalizedValue;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return normalizedDecisionMatrix;
        }

        /// <summary>
        /// Find the ideal and worst solution based on rating provided to all candidates
        /// </summary>
        /// <param name="argCriteriaCount"></param>
        /// <param name="argAlternativeCount"></param>
        /// <param name="argNormalizedScores"></param>
        /// <param name="argCriteria"></param>
        void findIdealBestAndWorst(int argCriteriaCount, int argAlternativeCount, double[,] argNormalizedScores, List<Criteria> argCriteria)
        {
            l_IdealBest = null;
            l_IdealWorst = null;

            l_IdealBest = new double[argCriteriaCount];
            l_IdealWorst = new double[argCriteriaCount];

            try
            {
                for (int c = 0; c < argCriteriaCount; c++)
                {
                    double l_minValue = Double.MaxValue;
                    double l_maxValue = 0;

                    for (int a = 0; a < argAlternativeCount; a++)
                    {

                        if (argNormalizedScores[a, c] > l_maxValue)
                        {
                            l_maxValue = argNormalizedScores[a, c];
                        }
                        if (argNormalizedScores[a, c] < l_minValue)
                        {
                            l_minValue = argNormalizedScores[a, c];
                        }
                    }

                    if (argCriteria[c].IsNegative)
                    {
                        l_IdealBest[c] = l_minValue;
                        l_IdealWorst[c] = l_maxValue;
                    }
                    else
                    {
                        l_IdealBest[c] = l_maxValue;
                        l_IdealWorst[c] = l_minValue;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Find out the Euclidean distance for each score
        /// </summary>
        /// <param name="argNormalizedScores"></param>
        /// <param name="argNumberOfAlternatives"></param>
        /// <param name="argNumberOfCriteria"></param>
        private void computeEuclidianDistancesFromIdealBestAndWorst(double[,] argNormalizedScores, int argNumberOfAlternatives, int argNumberOfCriteria)
        {

            l_distancesFromlIdealBest = null;
            l_distancesFromIdealWorst = null;

            l_distancesFromlIdealBest = new double[argNumberOfAlternatives];
            l_distancesFromIdealWorst = new double[argNumberOfAlternatives];

            try
            {
                for (int a = 0; a < argNumberOfAlternatives; a++)
                {
                    double l_distanceFromBest = 0;
                    double l_distanceFromWorst = 0;

                    for (int c = 0; c < argNumberOfCriteria; c++)
                    {

                        double squareOfDifferenceFromBest = argNormalizedScores[a, c] - l_IdealBest[c];
                        squareOfDifferenceFromBest = squareOfDifferenceFromBest * squareOfDifferenceFromBest;
                        l_distanceFromBest += squareOfDifferenceFromBest;


                        double squareOfDifferenceFromWorst = argNormalizedScores[a, c] - l_IdealWorst[c];
                        squareOfDifferenceFromWorst = squareOfDifferenceFromWorst * squareOfDifferenceFromWorst;
                        l_distanceFromWorst += squareOfDifferenceFromWorst;
                    }

                    l_distancesFromlIdealBest[a] = Math.Pow(l_distanceFromBest, 0.5);
                    l_distancesFromIdealWorst[a] = Math.Pow(l_distanceFromWorst, 0.5);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Compute the final performace score
        /// </summary>
        /// <param name="argalternatives"></param>
        /// <returns></returns>
        private List<Alternative> computePerformanceScore(List<Alternative> argalternatives)
        {
            int l_numberOflternatives = argalternatives.Count;
            try
            {
                for (int a = 0; a < l_numberOflternatives; a++)
                {
                    double performanceScore = l_distancesFromIdealWorst[a] / (l_distancesFromlIdealBest[a] + l_distancesFromIdealWorst[a]);
                    argalternatives[a].calculatedPerformance = performanceScore;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return argalternatives;
        }


    }
}
