using System;

namespace SlidingTiles
{
    public class PuzzleState
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int[] Cells { get; set; }
        public int EmptyPosition { get; set; }

        public PuzzleState(int width, int height, int[] cells)
        {
            Width = width;
            Height = height;
            Cells = cells;
            EmptyPosition = Array.IndexOf(cells, 0);
        }

        public bool IsGoal()
        {
            for (int i = 0; i < Cells.Length - 1; i++)
            {
                if (Cells[i] != i + 1) return false;
            }
            return Cells[Cells.Length - 1] == 0;
        }

        public bool IsSolvable()
        {
            // For a puzzle to be solvable, the number of inversions plus the row number of the empty tile
            // must be even for odd-sized puzzles, or just the number of inversions must be even for even-sized puzzles
            int inversions = 0;
            for (int i = 0; i < Cells.Length; i++)
            {
                if (Cells[i] == 0) continue;
                for (int j = i + 1; j < Cells.Length; j++)
                {
                    if (Cells[j] == 0) continue;
                    if (Cells[i] > Cells[j]) inversions++;
                }
            }

            if (Width == 2)
            {
                // 2x2 puzzles: inversions must be even
                return inversions % 2 == 0;
            }
            else if (Width % 2 == 1)
            {
                // Odd width: inversions must be even
                return inversions % 2 == 0;
            }
            else
            {
                // Even width ≥ 4: inversions + row of empty tile from bottom must be even
                int emptyRowFromTop = EmptyPosition / Width;
                int emptyRowFromBottom = Height - 1 - emptyRowFromTop;
                return (inversions + emptyRowFromBottom) % 2 == 0;
            }
        }

        public int GetRow(int position)
        {
            return position / Width;
        }

        public int GetCol(int position)
        {
            return position % Width;
        }

        public int GetManhattanDistance(int position, int value)
        {
            if (value == 0) return 0; // Empty tile doesn't contribute to distance
            
            int targetRow = (value - 1) / Width;
            int targetCol = (value - 1) % Width;
            int currentRow = GetRow(position);
            int currentCol = GetCol(position);
            
            return Math.Abs(targetRow - currentRow) + Math.Abs(targetCol - currentCol);
        }
    }
}
