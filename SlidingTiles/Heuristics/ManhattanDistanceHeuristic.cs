using System;

namespace SlidingTiles
{
    public class ManhattanDistanceHeuristic : IHeuristic
    {
        public string Name => "Manhattan Distance";
        public string Abbreviation => "md";
        public string Description => "Sum of Manhattan distances to goal";

        public int Calculate(PuzzleState state)
        {
            int manhattanDistance = 0;
            for (int i = 0; i < state.Cells.Length; i++)
            {
                if (state.Cells[i] != 0)
                {
                    manhattanDistance += CalculateManhattanDistance(state, i, state.Cells[i]);
                }
            }
            return manhattanDistance;
        }

        private int CalculateManhattanDistance(PuzzleState state, int position, int value)
        {
            if (value == 0) return 0; // Empty tile doesn't contribute to distance
            
            int targetRow = (value - 1) / state.Width;
            int targetCol = (value - 1) % state.Width;
            int currentRow = position / state.Width;
            int currentCol = position % state.Width;
            
            return Math.Abs(targetRow - currentRow) + Math.Abs(targetCol - currentCol);
        }
    }
}
