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

        public int GetRow(int position)
        {
            return position / Width;
        }

        public int GetCol(int position)
        {
            return position % Width;
        }
    }
}
