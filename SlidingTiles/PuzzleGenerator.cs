using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace SlidingTiles
{
    public class PuzzleGenerator
    {
        private readonly int _width;
        private readonly int _height;

        public PuzzleGenerator(int width = 3, int height = 3)
        {
            _width = width;
            _height = height;
        }

        public List<ProblemInstance> GenerateAllPuzzles()
        {
            var puzzles = new List<ProblemInstance>();
            var visited = new HashSet<string>();
            var queue = new Queue<(int[] state, int depth)>();

            // Start with the goal state
            var goalState = CreateGoalState();
            var goalString = StateToString(goalState);
            visited.Add(goalString);
            puzzles.Add(new ProblemInstance { Cells = goalState, OptimalValue = "0" });
            queue.Enqueue((goalState, 0));

            while (queue.Count > 0)
            {
                var (currentState, depth) = queue.Dequeue();

                // Generate all possible moves from current state
                var nextStates = GenerateNextStates(currentState);
                foreach (var nextState in nextStates)
                {
                    var nextStateString = StateToString(nextState);
                    if (!visited.Contains(nextStateString))
                    {
                        visited.Add(nextStateString);
                        puzzles.Add(new ProblemInstance { Cells = nextState, OptimalValue = (depth + 1).ToString() });
                        queue.Enqueue((nextState, depth + 1));
                    }
                }
            }

            return puzzles;
        }

        private int[] CreateGoalState()
        {
            var state = new int[_width * _height];
            for (int i = 0; i < state.Length - 1; i++)
            {
                state[i] = i + 1;
            }
            state[state.Length - 1] = 0; // Empty tile
            return state;
        }

        private List<int[]> GenerateNextStates(int[] currentState)
        {
            var nextStates = new List<int[]>();
            var emptyIndex = Array.IndexOf(currentState, 0);
            var (emptyRow, emptyCol) = IndexToPosition(emptyIndex);

            // Check all four directions: up, down, left, right
            var directions = new[] { (-1, 0), (1, 0), (0, -1), (0, 1) };

            foreach (var (dRow, dCol) in directions)
            {
                var newRow = emptyRow + dRow;
                var newCol = emptyCol + dCol;

                if (IsValidPosition(newRow, newCol))
                {
                    var newIndex = PositionToIndex(newRow, newCol);
                    var newState = (int[])currentState.Clone();
                    
                    // Swap empty tile with neighbor
                    newState[emptyIndex] = newState[newIndex];
                    newState[newIndex] = 0;
                    
                    nextStates.Add(newState);
                }
            }

            return nextStates;
        }

        private (int row, int col) IndexToPosition(int index)
        {
            return (index / _width, index % _width);
        }

        private int PositionToIndex(int row, int col)
        {
            return row * _width + col;
        }

        private bool IsValidPosition(int row, int col)
        {
            return row >= 0 && row < _height && col >= 0 && col < _width;
        }

        private string StateToString(int[] state)
        {
            return string.Join(",", state);
        }

        public void SaveToFile(string filename, string source = "Generated 3x3 Puzzles")
        {
            var puzzles = GenerateAllPuzzles();
            
            // Ensure filename has .puz.gz extension
            if (!filename.EndsWith(".puz.gz", StringComparison.OrdinalIgnoreCase))
            {
                filename = Path.ChangeExtension(filename, ".puz.gz");
            }
            
            using var fileStream = File.Create(filename);
            using var gzipStream = new GZipStream(fileStream, CompressionMode.Compress);
            using var writer = new StreamWriter(gzipStream, Encoding.UTF8);
            
            // Write metadata
            writer.WriteLine($"#width:{_width}|height:{_height}|source:{source}");
            
            // Write all puzzles
            foreach (var puzzle in puzzles)
            {
                var cellsString = string.Join(",", puzzle.Cells);
                writer.WriteLine($"{cellsString}#optimal:{puzzle.OptimalValue}");
            }
        }
    }
}
