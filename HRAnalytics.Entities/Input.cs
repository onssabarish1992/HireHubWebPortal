namespace HRAnalyticsPrescriptiveAPI.Entities
{
    public class Input
    {
        public string? ProjectName { get; set; }
        public string? Type { get; set; }
        public List<InputVariable>? Variables { get; set; }
        public List<InputConstraints>? InputConstraints { get; set; }
        public ObjectiveFunction? ObjectiveFunction { get; set; }
    }
}
