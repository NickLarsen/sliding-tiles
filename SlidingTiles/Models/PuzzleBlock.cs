namespace SlidingTiles
{
    public class PuzzleBlock
    {
        public BlockMetadata? Metadata { get; set; }
        public List<ProblemInstance> Instances { get; set; } = new List<ProblemInstance>();
    }
}
