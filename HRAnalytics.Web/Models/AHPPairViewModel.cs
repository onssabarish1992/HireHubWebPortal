namespace HRAnalytics.Web.Models
{
    public class AHPPairViewModel
    {
        public int PairId { get; set; }

        public string? Pair1Name { get; set; }

        public int Pair1 { get; set; }

        public string? Pair2Name { get; set; }

        public int Pair2 { get; set; }

        public int EntityId { get; set; }

        public int Weightage { get; set; }

        public int ParentEntityId { get; set; }
    }
}
