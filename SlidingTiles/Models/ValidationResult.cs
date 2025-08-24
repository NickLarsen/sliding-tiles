namespace SlidingTiles
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public int BlockCount { get; set; }
        public int InstanceCount { get; set; }
    }
}
