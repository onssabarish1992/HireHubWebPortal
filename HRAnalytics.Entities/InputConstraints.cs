namespace HRAnalyticsPrescriptiveAPI.Entities
{
    public class InputConstraints
    {
        public string? Name { get; set; }
        public double? UpperBound { get; set; }
        public double? LowerBound { get; set; }
        public List<Coefficient>? Coefficients { get; set; }
    }
}
