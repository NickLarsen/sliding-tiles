namespace SlidingTiles
{
    public class BlockEvaluationResult
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string? Source { get; set; }
        public List<InstanceEvaluationResult> InstanceResults { get; set; } = new List<InstanceEvaluationResult>();
    }
}
