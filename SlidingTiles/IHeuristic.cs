namespace SlidingTiles
{
    public interface IHeuristic
    {
        string Name { get; }
        string Abbreviation { get; }
        string Description { get; }
        int Calculate(PuzzleState state);
    }
}
