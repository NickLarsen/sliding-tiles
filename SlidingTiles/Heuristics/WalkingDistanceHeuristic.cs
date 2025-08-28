using System;
using System.Collections.Generic;
using System.Linq;

namespace SlidingTiles
{
    public class WalkingDistanceHeuristic : IHeuristic
    {
        public string Name => "Walking Distance";
        public string Abbreviation => "wd";
        public string Description => "Walking distance heuristic for sliding puzzles using precomputed walking distance database";

        private readonly Dictionary<string, int> _walkingDistanceDatabase;
        private readonly int _width;
        private readonly int _height;

        public WalkingDistanceHeuristic(int width = 3, int height = 3)
        {
            _width = width;
            _height = height;
            _walkingDistanceDatabase = new Dictionary<string, int>();
            BuildWalkingDistanceDatabase();
        }

        private void BuildWalkingDistanceDatabase()
        {
            // Create goal walking distance state
            var goalWDState = CreateGoalWalkingDistanceState();
            var goalWDString = WalkingDistanceStateToString(goalWDState);

            // Initialize BFS
            var queue = new Queue<(WalkingDistanceState state, int cost)>();
            var visited = new HashSet<string>();

            queue.Enqueue((goalWDState, 0));
            visited.Add(goalWDString);
            _walkingDistanceDatabase[goalWDString] = 0;

            while (queue.Count > 0)
            {
                var (currentWDState, currentCost) = queue.Dequeue();

                // Generate all possible moves from current walking distance state
                var neighbors = GetNeighborWalkingDistanceStates(currentWDState);

                foreach (var neighborWDState in neighbors)
                {
                    var neighborWDString = WalkingDistanceStateToString(neighborWDState);

                    if (!visited.Contains(neighborWDString))
                    {
                        visited.Add(neighborWDString);
                        var newCost = currentCost + 1;
                        _walkingDistanceDatabase[neighborWDString] = newCost;
                        queue.Enqueue((neighborWDState, newCost));
                    }
                }
            }
        }

        private WalkingDistanceState CreateGoalWalkingDistanceState()
        {
            var rowPatterns = new int[_height];
            var colPatterns = new int[_width];

            // In goal state, each row and column has tiles in correct order
            for (int i = 0; i < _height; i++)
            {
                rowPatterns[i] = i; // Row i contains tiles from row i
            }
            for (int j = 0; j < _width; j++)
            {
                colPatterns[j] = j; // Column j contains tiles from column j
            }

            return new WalkingDistanceState { RowPatterns = rowPatterns, ColPatterns = colPatterns };
        }

        private List<WalkingDistanceState> GetNeighborWalkingDistanceStates(WalkingDistanceState currentState)
        {
            var neighbors = new List<WalkingDistanceState>();

            // Try all possible row swaps
            for (int i = 0; i < _height; i++)
            {
                for (int j = i + 1; j < _height; j++)
                {
                    var newState = new WalkingDistanceState
                    {
                        RowPatterns = (int[])currentState.RowPatterns.Clone(),
                        ColPatterns = (int[])currentState.ColPatterns.Clone()
                    };

                    // Swap row patterns
                    var temp = newState.RowPatterns[i];
                    newState.RowPatterns[i] = newState.RowPatterns[j];
                    newState.RowPatterns[j] = temp;

                    neighbors.Add(newState);
                }
            }

            // Try all possible column swaps
            for (int i = 0; i < _width; i++)
            {
                for (int j = i + 1; j < _width; j++)
                {
                    var newState = new WalkingDistanceState
                    {
                        RowPatterns = (int[])currentState.RowPatterns.Clone(),
                        ColPatterns = (int[])currentState.ColPatterns.Clone()
                    };

                    // Swap column patterns
                    var temp = newState.ColPatterns[i];
                    newState.ColPatterns[i] = newState.ColPatterns[j];
                    newState.ColPatterns[j] = temp;

                    neighbors.Add(newState);
                }
            }

            return neighbors;
        }

        private string WalkingDistanceStateToString(WalkingDistanceState wdState)
        {
            var rowStr = string.Join(",", wdState.RowPatterns);
            var colStr = string.Join(",", wdState.ColPatterns);
            return $"R:{rowStr}|C:{colStr}";
        }

        public int Calculate(PuzzleState state)
        {
            // Convert PuzzleState to WalkingDistanceState
            var wdState = ConvertToWalkingDistanceState(state);
            var wdString = WalkingDistanceStateToString(wdState);

            if (_walkingDistanceDatabase.TryGetValue(wdString, out int cost))
            {
                return cost;
            }

            // If walking distance state not found, fall back to Manhattan distance
            return CalculateFallbackHeuristic(state);
        }

        private WalkingDistanceState ConvertToWalkingDistanceState(PuzzleState state)
        {
            var rowPatterns = new int[state.Height];
            var colPatterns = new int[state.Width];

            // Calculate row patterns
            for (int row = 0; row < state.Height; row++)
            {
                var rowTiles = new List<int>();
                for (int col = 0; col < state.Width; col++)
                {
                    int index = row * state.Width + col;
                    if (state.Cells[index] != 0) // Skip empty tile
                    {
                        rowTiles.Add(state.Cells[index]);
                    }
                }
                rowPatterns[row] = GetRowPattern(rowTiles);
            }

            // Calculate column patterns
            for (int col = 0; col < state.Width; col++)
            {
                var colTiles = new List<int>();
                for (int row = 0; row < state.Height; row++)
                {
                    int index = row * state.Width + col;
                    if (state.Cells[index] != 0) // Skip empty tile
                    {
                        colTiles.Add(state.Cells[index]);
                    }
                }
                colPatterns[col] = GetColumnPattern(colTiles);
            }

            return new WalkingDistanceState { RowPatterns = rowPatterns, ColPatterns = colPatterns };
        }

        private int GetRowPattern(List<int> tiles)
        {
            // Create a pattern representing the relative order of tiles in this row
            // This is a simplified approach - in practice, you'd want a more sophisticated pattern representation
            var sortedTiles = tiles.OrderBy(x => x).ToList();
            var pattern = 0;
            
            for (int i = 0; i < tiles.Count; i++)
            {
                var targetIndex = sortedTiles.IndexOf(tiles[i]);
                pattern = pattern * 10 + targetIndex;
            }
            
            return pattern;
        }

        private int GetColumnPattern(List<int> tiles)
        {
            // Similar to row pattern but for columns
            var sortedTiles = tiles.OrderBy(x => x).ToList();
            var pattern = 0;
            
            for (int i = 0; i < tiles.Count; i++)
            {
                var targetIndex = sortedTiles.IndexOf(tiles[i]);
                pattern = pattern * 10 + targetIndex;
            }
            
            return pattern;
        }

        private int CalculateFallbackHeuristic(PuzzleState state)
        {
            // Simple Manhattan distance as fallback
            int totalDistance = 0;
            for (int i = 0; i < state.Cells.Length; i++)
            {
                if (state.Cells[i] != 0) // Skip empty tile
                {
                    var currentRow = i / state.Width;
                    var currentCol = i % state.Width;
                    
                    var targetValue = state.Cells[i];
                    var targetIndex = targetValue - 1;
                    if (targetIndex >= 0 && targetIndex < state.Cells.Length - 1)
                    {
                        var targetRow = targetIndex / state.Width;
                        var targetCol = targetIndex % state.Width;
                        
                        totalDistance += Math.Abs(currentRow - targetRow) + Math.Abs(currentCol - targetCol);
                    }
                }
            }
            return totalDistance;
        }
    }

    public class WalkingDistanceState
    {
        public int[] RowPatterns { get; set; } = new int[0];
        public int[] ColPatterns { get; set; } = new int[0];
    }
}
