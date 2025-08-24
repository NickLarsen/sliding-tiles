namespace SlidingTiles
{
    public class BlockMetadata
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string? Source { get; set; }
        public Dictionary<string, string> AdditionalProperties { get; set; } = new Dictionary<string, string>();
    }
}
