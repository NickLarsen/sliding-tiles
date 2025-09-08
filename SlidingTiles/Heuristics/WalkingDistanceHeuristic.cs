using System.Text;
using Microsoft.Extensions.Logging;

namespace SlidingTiles
{
    public class WalkingDistanceHeuristic : IHeuristic
    {
        public string Name => "Walking Distance";
        public string Abbreviation => "wd";
        public string Description => "Walking distance heuristic for sliding puzzles using precomputed walking distance database";

        private readonly IDictionary<byte[,], byte> _walkingDistanceDatabase;
        private readonly int _width;
        private readonly int _height;
        private readonly ILogger<WalkingDistanceHeuristic> _logger;
        private readonly ByteArrayEqualityComparer _equalityComparer;

        public int MaxHeuristicValue => _walkingDistanceDatabase.Values.Max();
        public int DatabaseSize => _walkingDistanceDatabase.Count;

        public WalkingDistanceHeuristic(ILogger<WalkingDistanceHeuristic> logger, int width = 3, int height = 3)
        {
            if (width != height)
            {
                // for now just support square puzzles
                throw new ArgumentException("Width and height must be equal");
            }
            
            // Validate that the puzzle size won't exceed byte limits
            // For walking distance, the maximum value in any cell is 'width'
            // and the maximum distance should be reasonable for byte storage
            if (width > 255)
            {
                throw new ArgumentException("Width cannot exceed 255 due to byte storage limitations");
            }
            
            _width = width;
            _height = height;
            _logger = logger;
            _equalityComparer = new ByteArrayEqualityComparer();
            _walkingDistanceDatabase = BuildWalkingDistanceDatabase(width);
        }

        public int Calculate(PuzzleState state)
        {
            // row-wise walking distance
            var rowWdState = ConvertToWalkingDistanceState(state, Direction.Row);
            var rowWd = _walkingDistanceDatabase[rowWdState];

            // column-wise walking distance
            var colWdState = ConvertToWalkingDistanceState(state, Direction.Column);
            var colWd = _walkingDistanceDatabase[colWdState];

            // return the sum of the two
            return rowWd + colWd;
        }

        public byte[,] ConvertToWalkingDistanceState(PuzzleState state, Direction direction)
        {
            var result = new byte[_width, _width];
            for (int actualRow = 0; actualRow < state.Height; actualRow++)
            {
                for (int actualCol = 0; actualCol < state.Width; actualCol++)
                {
                    int actualLocation = actualRow * state.Width + actualCol;
                    // the real thing we want here is the number of the tile minus 1
                    // but the blank (0) needs to be at the end
                    // we ignore the blank because it's not tracked explicitly
                    int tileDesiredLocation = state.Cells[actualLocation] - 1;
                    if (tileDesiredLocation < 0)
                    {
                        continue;
                    }
                    int desiredRow = tileDesiredLocation / state.Width;
                    int desiredCol = tileDesiredLocation % state.Width;
                    if (direction == Direction.Row)
                    {
                        result[actualRow, desiredRow]++;
                    }
                    else if (direction == Direction.Column)
                    {
                        result[actualCol, desiredCol]++;
                    }
                }
            }
            return result;
        }

        private IDictionary<byte[,], byte> BuildWalkingDistanceDatabase(int width)
        {
            int initialCapacity = Convert.ToInt32(Math.Pow(10, width+2));
            var database = new Dictionary<byte[,], byte>(initialCapacity, _equalityComparer);

            var (state, initialBlankRow) = BuildGoalWalkingDistanceState(width);
            var queue = new Queue<(byte[,] state, byte distance, byte blankRow)>();
            
            // Add the initial state to database immediately
            database[state] = 0;
            queue.Enqueue((state, 0, initialBlankRow));
            
            int currentLevel = 0;
            int nodesAtCurrentLevel = 0;
            int totalNodesProcessed = 0;
            
            while (queue.Count > 0)
            {
                // process the state from the queue (already in database)
                var (currentState, distance, blankRow) = queue.Dequeue();
                
                // Check if we've moved to a new level
                if (distance > currentLevel)
                {
                    _logger.LogDebug("Level {Level}: {NodeCount} nodes evaluated", currentLevel, nodesAtCurrentLevel);
                    currentLevel = distance;
                    nodesAtCurrentLevel = 0;
                }
                
                nodesAtCurrentLevel++;
                totalNodesProcessed++;

                // try to move the blank up
                int upRow = blankRow + 1;
                if (upRow < _height)
                {
                    for (int col = 0; col < _width; col++)
                    {
                        if (currentState[upRow, col] > 0)
                        {
                            byte[,] newState = (byte[,])currentState.Clone();
                            newState[blankRow, col] += 1;
                            newState[upRow, col] -= 1;
                            if (!database.ContainsKey(newState))
                            {
                                // Add to database immediately when generated
                                database[newState] = (byte)(distance + 1);
                                queue.Enqueue((newState, (byte)(distance + 1), (byte)upRow));
                            }
                        }
                    }
                }

                // try to move the blank down
                int downRow = blankRow - 1;
                if (downRow >= 0)
                {
                    for (int col = 0; col < _width; col++)
                    {
                        if (currentState[downRow, col] > 0)
                        {
                            byte[,] newState = (byte[,])currentState.Clone();
                            newState[blankRow, col] += 1;
                            newState[downRow, col] -= 1;
                            if (!database.ContainsKey(newState))
                            {
                                // Add to database immediately when generated
                                database[newState] = (byte)(distance + 1);
                                queue.Enqueue((newState, (byte)(distance + 1), (byte)downRow));
                            }
                        }
                    }
                }
            }
            
            // Log the final level and total statistics
            _logger.LogDebug("Level {Level}: {NodeCount} nodes evaluated", currentLevel, nodesAtCurrentLevel);
            _logger.LogDebug("Total nodes processed: {TotalNodes}", totalNodesProcessed);
            
            return database;
        }

        private (byte[,] state, byte initialBlankRow) BuildGoalWalkingDistanceState(int width)
        {
            var state = new byte[width, width];
            for (int i = 0; i < width; i++)
            {
                state[i, i] = (byte)width;
            }
            state[width - 1, width - 1] = (byte)(width - 1);
            return (state, (byte)(width - 1));
        }

        public enum Direction
        {
            Row,
            Column,
        }
    }

    public class ByteArrayEqualityComparer : IEqualityComparer<byte[,]>
    {
        public bool Equals(byte[,]? x, byte[,]? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            if (x.GetLength(0) != y.GetLength(0) || x.GetLength(1) != y.GetLength(1)) return false;
            
            for (int i = 0; i < x.GetLength(0); i++)
            {
                for (int j = 0; j < x.GetLength(1); j++)
                {
                    if (x[i, j] != y[i, j]) return false;
                }
            }
            return true;
        }
        
        public int GetHashCode(byte[,]? obj)
        {
            if (obj == null) return 0;
            
            int hash = 17;
            for (int i = 0; i < obj.GetLength(0); i++)
            {
                for (int j = 0; j < obj.GetLength(1); j++)
                {
                    hash = hash * 31 + obj[i, j].GetHashCode();
                }
            }
            return hash;
        }
    }
}
