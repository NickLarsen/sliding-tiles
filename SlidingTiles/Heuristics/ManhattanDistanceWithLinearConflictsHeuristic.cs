namespace SlidingTiles
{
    public class ManhattanDistanceWithLinearConflictsHeuristic : IHeuristic
    {
        public string Name => "Manhattan Distance with Linear Conflicts";
        public string Abbreviation => "mc";

        public int Calculate(PuzzleState state)
        {
            int manhattanDistance = 0;
            int linearConflicts = 0;

            // Calculate basic Manhattan distance
            for (int i = 0; i < state.Cells.Length; i++)
            {
                if (state.Cells[i] != 0)
                {
                    manhattanDistance += state.GetManhattanDistance(i, state.Cells[i]);
                }
            }

            // Add linear conflicts for rows
            for (int row = 0; row < state.Height; row++)
            {
                linearConflicts += CalculateRowLinearConflicts(state, row);
            }

            // Add linear conflicts for columns
            for (int col = 0; col < state.Width; col++)
            {
                linearConflicts += CalculateColumnLinearConflicts(state, col);
            }

            return manhattanDistance + 2 * linearConflicts;
        }

        private int CalculateRowLinearConflicts(PuzzleState state, int row)
        {
            var tilesInRow = new List<(int value, int col)>();
            
            // Collect tiles that belong in this row
            for (int col = 0; col < state.Width; col++)
            {
                int position = row * state.Width + col;
                int value = state.Cells[position];
                if (value != 0 && (value - 1) / state.Width == row)
                {
                    tilesInRow.Add((value, col));
                }
            }

            // Count conflicts (tiles that are out of order)
            int conflicts = 0;
            for (int i = 0; i < tilesInRow.Count; i++)
            {
                for (int j = i + 1; j < tilesInRow.Count; j++)
                {
                    if (tilesInRow[i].value > tilesInRow[j].value)
                    {
                        conflicts++;
                    }
                }
            }

            return conflicts;
        }

        private int CalculateColumnLinearConflicts(PuzzleState state, int col)
        {
            var tilesInCol = new List<(int value, int row)>();
            
            // Collect tiles that belong in this column
            for (int row = 0; row < state.Height; row++)
            {
                int position = row * state.Width + col;
                int value = state.Cells[position];
                if (value != 0 && (value - 1) % state.Width == col)
                {
                    tilesInCol.Add((value, row));
                }
            }

            // Count conflicts (tiles that are out of order)
            int conflicts = 0;
            for (int i = 0; i < tilesInCol.Count; i++)
            {
                for (int j = i + 1; j < tilesInCol.Count; j++)
                {
                    if (tilesInCol[i].value > tilesInCol[j].value)
                    {
                        conflicts++;
                    }
                }
            }

            return conflicts;
        }
    }
}
