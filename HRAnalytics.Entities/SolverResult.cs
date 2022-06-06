namespace HRAnalyticsPrescriptiveAPI.Entities
{
    public class SolverResult
    {
        public double ObjectiveValue { get; set; }
        public bool IsOptimal { get; set; }

        public long WallTime { get; set; }

        public long Iterations { get; set; }

        public long Nodes { get; set; }

        public Dictionary<string, double>? DecisionVariableResults { get; set; }

        public string Status { get; set; }

        public int ConstraintCount { get; set; }
        public int DecisionVariableCount { get; set; }
    }
}
