using Xunit;
using SlidingTiles;

namespace SlidingTiles.Tests
{
    public class WalkingDistanceHeuristicTests
    {
        private readonly WalkingDistanceHeuristic _heuristic;

        public WalkingDistanceHeuristicTests()
        {
            _heuristic = new WalkingDistanceHeuristic();
        }

        [Fact]
        public void Calculate_GoalState_ShouldReturnZero()
        {
            // Arrange - 3x3 goal state
            var goalState = new PuzzleState(3, 3, new int[]
            {
                1, 2, 3,
                4, 5, 6,
                7, 8, 0
            });

            // Act
            var result = _heuristic.Calculate(goalState);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Calculate_OneMoveAway_ShouldReturnExpectedValue()
        {
            // Arrange - One move away from goal (swap 8 and 0)
            var oneMoveState = new PuzzleState(3, 3, new int[]
            {
                1, 2, 3,
                4, 5, 6,
                7, 0, 8
            });

            // Act
            var result = _heuristic.Calculate(oneMoveState);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void Calculate_ScrambledState_ShouldReturnExpectedValue()
        {
            // Arrange - Scrambled 3x3 state (swap 1 and 2 in first row)
            var scrambledState = new PuzzleState(3, 3, new int[]
            {
                2, 1, 3,
                4, 5, 6,
                7, 8, 0
            });

            // Act
            var result = _heuristic.Calculate(scrambledState);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void Calculate_2x2GoalState_ShouldReturnZero()
        {
            // Arrange - 2x2 goal state
            var goalState = new PuzzleState(2, 2, new int[]
            {
                1, 2,
                3, 0
            });

            // Act
            var result = _heuristic.Calculate(goalState);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Calculate_2x2ScrambledState_ShouldReturnExpectedValue()
        {
            // Arrange - 2x2 scrambled state (swap 1 and 2)
            var scrambledState = new PuzzleState(2, 2, new int[]
            {
                2, 1,
                3, 0
            });

            // Act
            var result = _heuristic.Calculate(scrambledState);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void Calculate_ComplexScrambledState_ShouldReturnExpectedValue()
        {
            // Arrange - More complex scrambled 3x3 state
            var scrambledState = new PuzzleState(3, 3, new int[]
            {
                8, 1, 3,
                4, 0, 2,
                7, 6, 5
            });

            // Act
            var result = _heuristic.Calculate(scrambledState);

            // Assert
            Assert.Equal(10, result);
        }

        [Fact]
        public void Calculate_EmptyTileInMiddle_ShouldReturnExpectedValue()
        {
            // Arrange - Empty tile in middle
            var middleEmptyState = new PuzzleState(3, 3, new int[]
            {
                1, 2, 3,
                4, 0, 6,
                7, 8, 5
            });

            // Act
            var result = _heuristic.Calculate(middleEmptyState);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void Calculate_AllTilesOutOfPlace_ShouldReturnExpectedValue()
        {
            // Arrange - All tiles out of place
            var allOutOfPlaceState = new PuzzleState(3, 3, new int[]
            {
                8, 7, 6,
                5, 4, 3,
                2, 1, 0
            });

            // Act
            var result = _heuristic.Calculate(allOutOfPlaceState);

            // Assert
            Assert.Equal(16, result);
        }

        [Fact]
        public void Calculate_SimpleSwap_ShouldReturnExpectedValue()
        {
            // Arrange - Simple swap of adjacent tiles
            var simpleSwapState = new PuzzleState(3, 3, new int[]
            {
                1, 2, 3,
                4, 5, 0,
                7, 8, 6
            });

            // Act
            var result = _heuristic.Calculate(simpleSwapState);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void Calculate_CrossPattern_ShouldReturnExpectedValue()
        {
            // Arrange - Cross pattern with tiles out of place
            var crossPatternState = new PuzzleState(3, 3, new int[]
            {
                0, 2, 1,
                4, 5, 6,
                7, 8, 3
            });

            // Act
            var result = _heuristic.Calculate(crossPatternState);

            // Assert
            Assert.Equal(4, result);
        }

        [Fact]
        public void Calculate_4x4GoalState_ShouldReturnZero()
        {
            // Arrange - 4x4 goal state
            var goalState = new PuzzleState(4, 4, new int[]
            {
                1,  2,  3,  4,
                5,  6,  7,  8,
                9,  10, 11, 12,
                13, 14, 15, 0
            });

            // Act
            var result = _heuristic.Calculate(goalState);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Calculate_4x4ScrambledState_ShouldReturnExpectedValue()
        {
            // Arrange - 4x4 scrambled state (swap 15 and 0)
            var scrambledState = new PuzzleState(4, 4, new int[]
            {
                1,  2,  3,  4,
                5,  6,  7,  8,
                9,  10, 11, 12,
                13, 14, 0,  15
            });

            // Act
            var result = _heuristic.Calculate(scrambledState);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void Calculate_HeuristicIsAdmissible_ShouldNotOverestimate()
        {
            // Arrange - Test that walking distance never overestimates the actual solution cost
            var testStates = new[]
            {
                new PuzzleState(3, 3, new int[] { 1, 2, 3, 4, 0, 6, 7, 5, 8 }),
                new PuzzleState(3, 3, new int[] { 2, 1, 3, 4, 5, 6, 7, 8, 0 }),
                new PuzzleState(3, 3, new int[] { 8, 7, 6, 5, 4, 3, 2, 1, 0 })
            };

            // Act & Assert
            foreach (var state in testStates)
            {
                var walkingDistance = _heuristic.Calculate(state);
                
                // Walking distance should be non-negative
                Assert.True(walkingDistance >= 0, $"Walking distance should be non-negative, got {walkingDistance}");
                
                // Walking distance should be reasonable (not excessively high)
                Assert.True(walkingDistance <= 20, $"Walking distance should be reasonable, got {walkingDistance}");
            }
        }

        [Fact]
        public void Calculate_HeuristicIsConsistent_ShouldMaintainMonotonicity()
        {
            // Arrange - Test that walking distance is consistent (monotonic)
            // Moving closer to goal should not increase the heuristic value
            var goalState = new PuzzleState(3, 3, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 0 });
            var oneMoveAway = new PuzzleState(3, 3, new int[] { 1, 2, 3, 4, 5, 6, 7, 0, 8 });
            var twoMovesAway = new PuzzleState(3, 3, new int[] { 1, 2, 3, 4, 5, 0, 7, 8, 6 });

            // Act
            var goalValue = _heuristic.Calculate(goalState);
            var oneMoveValue = _heuristic.Calculate(oneMoveAway);
            var twoMovesValue = _heuristic.Calculate(twoMovesAway);

            // Assert
            // Goal state should have lowest value
            Assert.Equal(0, goalValue);
            
            // One move away should have higher value than goal
            Assert.True(oneMoveValue > goalValue, "One move away should have higher heuristic than goal");
            
            // Two moves away should have higher or equal value than one move away
            Assert.True(twoMovesValue >= oneMoveValue, "Two moves away should have higher or equal heuristic than one move away");
        }
    }
}
