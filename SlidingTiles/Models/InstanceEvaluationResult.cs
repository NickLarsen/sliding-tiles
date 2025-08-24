namespace SlidingTiles
{
    public class InstanceEvaluationResult
    {
        public int[] Cells { get; set; } = Array.Empty<int>();
        public string OptimalValue { get; set; } = string.Empty;
        public List<HeuristicResult> HeuristicResults { get; set; } = new List<HeuristicResult>();
    }
}
