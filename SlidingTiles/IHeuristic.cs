namespace SlidingTiles
{
    public interface IHeuristic
    {
        string Name { get; }
        string Abbreviation { get; }
        int Calculate(PuzzleState state);
    }
}
