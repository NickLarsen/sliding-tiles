namespace SlidingTiles
{
    public class ManhattanDistanceHeuristic : IHeuristic
    {
        public string Name => "Manhattan Distance";
        public string Abbreviation => "md";

        public int Calculate(PuzzleState state)
        {
            int manhattanDistance = 0;
            for (int i = 0; i < state.Cells.Length; i++)
            {
                if (state.Cells[i] != 0)
                {
                    manhattanDistance += state.GetManhattanDistance(i, state.Cells[i]);
                }
            }
            return manhattanDistance;
        }
    }
}
