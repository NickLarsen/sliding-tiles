using System.Text;

namespace SlidingTiles
{
    public class WalkingDistanceHeuristic : IHeuristic
    {
        public string Name => "Walking Distance";
        public string Abbreviation => "wd";
        public string Description => "Walking distance heuristic for sliding puzzles using precomputed walking distance database";

        private readonly IDictionary<string, int> _walkingDistanceDatabase;
        private readonly int _width;
        private readonly int _height;

        public int MaxHeuristicValue => _walkingDistanceDatabase.Values.Max();

        public WalkingDistanceHeuristic(int width = 3, int height = 3)
        {
            if (width != height)
            {
                // for now just support square puzzles
                throw new ArgumentException("Width and height must be equal");
            }
            _width = width;
            _height = height;
            _walkingDistanceDatabase = BuildWalkingDistanceDatabase(width);
        }

        public int Calculate(PuzzleState state)
        {
            // row-wise walking distance
            var rowWdState = ConvertToWalkingDistanceState(state, Direction.Row);
            var rowWdString = WalkingDistanceStateToString(rowWdState);
            var rowWd = _walkingDistanceDatabase[rowWdString];

            // column-wise walking distance
            var colWdState = ConvertToWalkingDistanceState(state, Direction.Column);
            var colWdString = WalkingDistanceStateToString(colWdState);
            var colWd = _walkingDistanceDatabase[colWdString];

            // return the sum of the two
            return rowWd + colWd;
        }

        private int[,] ConvertToWalkingDistanceState(PuzzleState state, Direction direction)
        {
            var result = new int[_width, _width];
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

        private string WalkingDistanceStateToString(int[,] state)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < state.GetLength(0); i++)
            {
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    sb.Append(state[i, j]);
                }
            }
            return sb.ToString();
        }

        private IDictionary<string, int> BuildWalkingDistanceDatabase(int width)
        {
            var database = new Dictionary<string, int>();

            var state = BuildGoalWalkingDistanceState(width);
            var queue = new Queue<(int[,] state, int distance)>();
            queue.Enqueue((state, 0));
            while (queue.Count > 0)
            {
                // add the state to the database
                var (currentState, distance) = queue.Dequeue();
                var stateString = WalkingDistanceStateToString(currentState);
                if (database.ContainsKey(stateString))
                {
                    // state already visited so bail because we have already
                    // found the shortest step path to it
                    continue;
                }
                database[stateString] = distance;

                // find which row the blank is on
                var blankRow = -1;
                for (int row = 0; row < _height; row++)
                {
                    int tileCount = 0;
                    for (int col = 0; col < width; col++)
                    {
                        tileCount += currentState[row, col];
                    }
                    if (tileCount == (_width - 1))
                    {
                        blankRow = row;
                        break;
                    }
                }
                if (blankRow == -1)
                {
                    var e = new Exception("blank row was -1, should not happen");
                    e.Data.Add("state", stateString);
                    throw e;
                }

                // try to move the blank up
                int upRow = blankRow + 1;
                if (upRow < _height)
                {
                    for (int col = 0; col < _width; col++)
                    {
                        if (currentState[upRow, col] > 0)
                        {
                            int[,] newState = (int[,])currentState.Clone();
                            newState[blankRow, col] += 1;
                            newState[upRow, col] -= 1;
                            queue.Enqueue((newState, distance + 1));
                        }
                    }
                }

                // try to move the bank down
                int downRow = blankRow - 1;
                if (downRow >= 0)
                {
                    for (int col = 0; col < _width; col++)
                    {
                        if (currentState[downRow, col] > 0)
                        {
                            int[,] newState = (int[,])currentState.Clone();
                            newState[blankRow, col] += 1;
                            newState[downRow, col] -= 1;
                            queue.Enqueue((newState, distance + 1));
                        }
                    }
                }
            }
            return database;
        }

        private int[,] BuildGoalWalkingDistanceState(int width)
        {
            var state = new int[width, width];
            for (int i = 0; i < width; i++)
            {
                state[i, i] = width;
            }
            state[width - 1, width - 1] = width - 1;
            return state;
        }

        enum Direction
        {
            Row,
            Column,
        }
    }
}
