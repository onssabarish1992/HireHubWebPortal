using Google.OrTools.LinearSolver;
using HRAnalytics.BL.Interfaces;
using HRAnalyticsPrescriptiveAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.BL
{
    public class SolverBL: ISolverBL
    {
        private Solver _solver;

        /// <summary>
        /// This method will produce the results for prescriptive analytics
        /// </summary>
        /// <param name="argInput"></param>
        /// <returns></returns>
        public SolverResult SolveEquation(Input argInput)
        {
            SolverResult result = new();
            try
            {
                //Check for null
                if (argInput != null)
                {
                    _solver = Solver.CreateSolver("SCIP"); // To be made configurable

                    //Create the variables
                    List<Variable> requiredVariables = SetVariables(argInput);

                    //Create contraints
                    CreateConstraints(argInput, ref requiredVariables);

                    //Create objective function
                    Objective objective = CreateObjectiveFunction(argInput, ref requiredVariables);

                    //To be made configurable
                    objective.SetMaximization();

                    //Get the Results
                    Solver.ResultStatus resultStatus = _solver.Solve();

                    //Format the results
                    result = FormatResults(resultStatus, ref requiredVariables);

                }
                else
                {
                    result.Status = "ERROR";
                }
            }
            catch (Exception)
            {
                result.Status = "ERROR";
                throw;
            }

            return result;
        }

        private SolverResult FormatResults(Solver.ResultStatus resultStatus, ref List<Variable> requiredVariables)
        {
            SolverResult result = new();
            Dictionary<string, double> dict_Result = new();
            try
            {
                if (resultStatus == Solver.ResultStatus.OPTIMAL)
                {
                    result.IsOptimal = true;
                    result.ObjectiveValue = _solver.Objective().Value();

                    foreach (var vars in requiredVariables)
                    {
                        dict_Result.Add(vars.Name(), vars.SolutionValue());
                    }

                    result.DecisionVariableResults = dict_Result;
                    result.WallTime = _solver.WallTime();
                    result.Iterations = _solver.Iterations();
                    result.Nodes = _solver.Nodes();
                    result.DecisionVariableCount = _solver.NumVariables();
                    result.ConstraintCount = _solver.NumConstraints();
                }
                else
                {
                    result.IsOptimal = false;
                }
                result.Status = "SUCCESS";
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        /// <summary>
        /// This function is used to generate objective function
        /// </summary>
        /// <param name="argInput"></param>
        /// <param name="requiredVariables"></param>
        private Objective CreateObjectiveFunction(Input argInput, ref List<Variable> requiredVariables)
        {
            Objective objective = _solver.Objective();
            try
            {
                if (argInput.ObjectiveFunction != null && argInput.ObjectiveFunction.Coefficients != null)
                {
                    foreach (var coeffs in argInput.ObjectiveFunction.Coefficients)
                    {
                        var searchResult = requiredVariables.FirstOrDefault(x => x.Name().ToLower() == coeffs.VariableName.ToLower());

                        if (searchResult != null)
                        {
                            objective.SetCoefficient(searchResult, coeffs.Value);
                        }

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return objective;
        }

        /// <summary>
        /// This method is used to create constraints
        /// </summary>
        /// <param name="argInput"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void CreateConstraints(Input argInput, ref List<Variable> argRequiredVariables)
        {
            try
            {
                if (argInput.InputConstraints != null)
                {
                    foreach (var ct in argInput.InputConstraints)
                    {
                        Constraint cnts = _solver.MakeConstraint(ct.LowerBound.HasValue ? ct.LowerBound.Value : 0, ct.UpperBound.HasValue ? ct.UpperBound.Value : 0, ct.Name);
                        if (ct.Coefficients != null)
                        {
                            foreach (var coeff in ct.Coefficients)
                            {
                                var searchResult = argRequiredVariables.FirstOrDefault(x => x.Name().ToLower() == coeff.VariableName.ToLower());
                                if (searchResult != null)
                                {
                                    //Setting up coefficients of constraints
                                    cnts.SetCoefficient(searchResult, coeff.Value);
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// This method is used to set the required decision variables 
        /// </summary>
        /// <param name="argInput"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private List<Variable> SetVariables(Input argInput)
        {
            List<Variable> lst_variables = new();
            try
            {
                if (argInput.Variables != null)
                {
                    foreach (var decisionvars in argInput.Variables)
                    {
                        lst_variables.Add(_solver.MakeIntVar(decisionvars.LowerBound, decisionvars.UpperBound, decisionvars.Name));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return lst_variables;
        }
    }
}
