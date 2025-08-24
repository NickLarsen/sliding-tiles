using System;

namespace SlidingTiles
{
    public class WalkingDistanceHeuristic : IHeuristic
    {
        public string Name => "Walking Distance";
        public string Abbreviation => "wd";
        public string Description => "Walking distance heuristic for sliding puzzles";

        public int Calculate(PuzzleState state)
        {
            int walkingDistance = 0;
            
            // Calculate walking distance for rows
            walkingDistance += CalculateRowWalkingDistance(state);
            
            // Calculate walking distance for columns
            walkingDistance += CalculateColumnWalkingDistance(state);
            
            return walkingDistance;
        }

        private int CalculateRowWalkingDistance(PuzzleState state)
        {
            int totalDistance = 0;
            
            for (int row = 0; row < state.Height; row++)
            {
                int[] rowValues = new int[state.Width];
                int[] targetRowValues = new int[state.Width];
                
                // Get current row values
                for (int col = 0; col < state.Width; col++)
                {
                    int index = row * state.Width + col;
                    rowValues[col] = state.Cells[index];
                }
                
                // Get target row values
                for (int col = 0; col < state.Width; col++)
                {
                    int targetValue = row * state.Width + col + 1;
                    if (targetValue <= state.Width * state.Height - 1)
                    {
                        targetRowValues[col] = targetValue;
                    }
                    else
                    {
                        targetRowValues[col] = 0; // Empty tile
                    }
                }
                
                totalDistance += CalculateRowDistance(rowValues, targetRowValues);
            }
            
            return totalDistance;
        }

        private int CalculateColumnWalkingDistance(PuzzleState state)
        {
            int totalDistance = 0;
            
            for (int col = 0; col < state.Width; col++)
            {
                int[] colValues = new int[state.Height];
                int[] targetColValues = new int[state.Height];
                
                // Get current column values
                for (int row = 0; row < state.Height; row++)
                {
                    int index = row * state.Width + col;
                    colValues[row] = state.Cells[index];
                }
                
                // Get target column values
                for (int row = 0; row < state.Height; row++)
                {
                    int targetValue = row * state.Width + col + 1;
                    if (targetValue <= state.Width * state.Height - 1)
                    {
                        targetColValues[row] = targetValue;
                    }
                    else
                    {
                        targetColValues[row] = 0; // Empty tile
                    }
                }
                
                totalDistance += CalculateColumnDistance(colValues, targetColValues);
            }
            
            return totalDistance;
        }

        private int CalculateRowDistance(int[] current, int[] target)
        {
            int distance = 0;
            int[] currentCopy = (int[])current.Clone();
            int[] targetCopy = (int[])target.Clone();
            
            // Remove empty tiles (0) from consideration
            currentCopy = Array.FindAll(currentCopy, x => x != 0);
            targetCopy = Array.FindAll(targetCopy, x => x != 0);
            
            // Calculate minimum moves needed to transform current row to target row
            for (int i = 0; i < currentCopy.Length; i++)
            {
                if (currentCopy[i] != targetCopy[i])
                {
                    // Find the target position of current value
                    int targetPos = Array.IndexOf(targetCopy, currentCopy[i]);
                    if (targetPos != -1)
                    {
                        distance += Math.Abs(i - targetPos);
                    }
                }
            }
            
            return distance / 2; // Divide by 2 because each move affects two positions
        }

        private int CalculateColumnDistance(int[] current, int[] target)
        {
            int distance = 0;
            int[] currentCopy = (int[])current.Clone();
            int[] targetCopy = (int[])target.Clone();
            
            // Remove empty tiles (0) from consideration
            currentCopy = Array.FindAll(currentCopy, x => x != 0);
            targetCopy = Array.FindAll(targetCopy, x => x != 0);
            
            // Calculate minimum moves needed to transform current column to target column
            for (int i = 0; i < currentCopy.Length; i++)
            {
                if (currentCopy[i] != targetCopy[i])
                {
                    // Find the target position of current value
                    int targetPos = Array.IndexOf(targetCopy, currentCopy[i]);
                    if (targetPos != -1)
                    {
                        distance += Math.Abs(i - targetPos);
                    }
                }
            }
            
            return distance / 2; // Divide by 2 because each move affects two positions
        }
    }
}
