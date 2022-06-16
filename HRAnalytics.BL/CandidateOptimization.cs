using HRAnalytics.BL.Interfaces;
using HRAnalytics.Entities;
using HRAnalyticsPrescriptiveAPI.Entities;

namespace HRAnalytics.BL
{
    public class CandidateOptimization: ICandidateOptimization
    {
        private IJobBL _IJobBL;
        private ICandidateResult _ICandidateBL;
        private ISolverBL _ISolverBL;
        private ICandidateScore _ICandidateScore;

        public CandidateOptimization(IJobBL argJobBL, 
                                     ICandidateResult argCandidateBL, 
                                     ISolverBL argSolverBL,
                                     ICandidateScore argCandidateScore)
        {
            _IJobBL = argJobBL;
            _ICandidateBL = argCandidateBL;
            _ISolverBL = argSolverBL;
            _ICandidateScore = argCandidateScore;
        }

        public List<Candidate> GetOptimumResult()
        {
            #region Declaration
            List<Candidate> l_result = new List<Candidate>();
            Input l_input = new Input();
            #endregion

            try
            {
                var l_CandidateScore = _ICandidateBL.GetCandidateResult();

                l_input.ObjectiveFunction = CreateObjectiveFunction(l_CandidateScore);

                l_input.Variables = SetInputVariable(l_CandidateScore);

                l_input.InputConstraints = GetConstraints(l_CandidateScore);

                l_input.ProjectName = "HRA";

                l_input.Type = "MIP";

                //var jsonInput = JsonConvert.SerializeObject(l_input);

                var SolverResult = _ISolverBL.SolveEquation(l_input);

                l_result = GenerateCandidateResult(SolverResult, l_CandidateScore);

            }
            catch (Exception)
            {
                throw;
            }

            return l_result;

        }

        /// <summary>
        /// Form the final object for candidate object
        /// </summary>
        /// <param name="solverResult"></param>
        /// <param name="l_CandidateScore"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private List<Candidate> GenerateCandidateResult(SolverResult solverResult, List<Candidate> argCandidateScore)
        {
            try
            {
                if(solverResult!=null && 
                   solverResult.DecisionVariableResults!=null && 
                   solverResult.DecisionVariableResults.Count > 0)
                {
                    foreach (var candidate in argCandidateScore)
                    {
                        var l_decision = solverResult.DecisionVariableResults
                                        .Where(x => Convert.ToInt32(x.Key) == candidate.CandidateID)
                                        .Select(y => y.Value).FirstOrDefault();

                        candidate.IsRecommended = l_decision == 1.0 ? true : false;

                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

            return argCandidateScore;
        }

        private List<InputConstraints> GetConstraints(List<Candidate> argCandidateScore)
        {
            #region Declarations
            List<InputConstraints> l_constraints = new List<InputConstraints>();
            List<InputConstraints> l_stragegicConstraints = new List<InputConstraints>();
            #endregion
            try
            {
                //Get roles
                var l_roles = _IJobBL.GetRoles();

                if(l_roles!=null)
                {
                    //Add compensation constraint
                    var l_CompensationConstraint = CreateCompensationConstraint(l_roles, argCandidateScore);

                    l_constraints.Add(l_CompensationConstraint);

                    l_stragegicConstraints = GetStrategicConstraints(l_roles, argCandidateScore);

                    l_constraints.AddRange(l_stragegicConstraints);

                }

            }
            catch (Exception)
            {

                throw;
            }

            return l_constraints;
        }

        private List<InputConstraints> GetStrategicConstraints(JobCollection argRoles, List<Candidate> argCandidateScore)
        {
            List<InputConstraints> l_strategicConstraints = new List<InputConstraints>();
            InputConstraints l_constraint;
            List<Coefficient> l_coefficients = new List<Coefficient>();
            Coefficient l_coefficient;

            try
            {
                foreach (var role in argRoles)
                {
                    l_constraint = new InputConstraints();
                    l_coefficients = new List<Coefficient>();
                    l_constraint.Name = "CONSTRAINT" + " " + role.JobName;
                    l_constraint.UpperBound = role.Position;
                    l_constraint.LowerBound = 0;

                    var l_requiredCandidates = argCandidateScore.Where(x => x.JobId == role.JobId);

                    if(l_requiredCandidates.Any())
                    {
                        foreach (var candidate in l_requiredCandidates)
                        {
                            l_coefficient = new Coefficient();
                            l_coefficient.VariableName = Convert.ToString(candidate.CandidateID);
                            l_coefficient.Value = 1;
                            l_coefficients.Add(l_coefficient);
                        }

                        l_constraint.Coefficients = l_coefficients;
                    }

                    l_strategicConstraints.Add(l_constraint);

                }
            }
            catch (Exception)
            {
                throw;
            }

            return l_strategicConstraints;


        }

        private InputConstraints CreateCompensationConstraint(JobCollection argRoles, List<Candidate> argCandidates)
        {
            #region Declaration
            InputConstraints l_inputConstraints = new InputConstraints();
            List<Coefficient> l_coefficients = new List<Coefficient>();
            Coefficient l_coefficient;
            #endregion
            try
            {
                l_inputConstraints.Name = "COMPENSATION";
                l_inputConstraints.LowerBound = 0;
                l_inputConstraints.UpperBound = argRoles.Sum(x => x.Compensation);

                foreach (var item in argRoles)
                {
                    var l_candidatesForRole = argCandidates.Where(x => x.JobId == item.JobId);
                    l_coefficients = new List<Coefficient>();

                    if (l_candidatesForRole.Any())
                    {
                        foreach (var candidate in l_candidatesForRole)
                        {
                            l_coefficient = new Coefficient();
                            l_coefficient.VariableName = Convert.ToString(candidate.CandidateID);
                            l_coefficient.Value = item.Compensation;

                            l_coefficients.Add(l_coefficient);
                        }
                    }
                }

                l_inputConstraints.Coefficients = l_coefficients;

            }
            catch (Exception)
            {
                throw;
            }

            return l_inputConstraints;

        }

        /// <summary>
        /// Create Input Variables array
        /// </summary>
        /// <returns></returns>
        private List<InputVariable> SetInputVariable(List<Candidate> argCandidateScore)
        {
            List<InputVariable> l_InputVars = new List<InputVariable>();
            InputVariable l_inputVariable;

            try
            {
                foreach (var item in argCandidateScore)
                {
                    l_inputVariable = new InputVariable();
                    l_inputVariable.Name = Convert.ToString(item.CandidateID);
                    l_inputVariable.LowerBound = 0;
                    l_inputVariable.UpperBound = 1;

                    l_InputVars.Add(l_inputVariable);

                }
            }
            catch (Exception)
            {

                throw;
            }

            return l_InputVars;
        }

        /// <summary>
        /// Create objective function
        /// </summary>
        /// <param name="argCandidateScore"></param>
        /// <returns></returns>
        private ObjectiveFunction CreateObjectiveFunction(List<Candidate> argCandidateScore)
        {
            ObjectiveFunction objFunc = new ObjectiveFunction();
            List<Coefficient> l_coefficients = new List<Coefficient>();
            Coefficient l_coefficient;

            try
            {
                objFunc.Type = "MAX";
                foreach (var item in argCandidateScore)
                {
                    l_coefficient = new Coefficient();
                    l_coefficient.VariableName = Convert.ToString(item.CandidateID);
                    l_coefficient.Value = item.GlobalScore;

                    l_coefficients.Add(l_coefficient);
                }
                objFunc.Coefficients = l_coefficients;
            }
            catch (Exception)
            {

                throw;
            }

            return objFunc;
        }
    }
}
