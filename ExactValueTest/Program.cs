using System;
using SlidingTiles;
using Microsoft.Extensions.Logging;

namespace ExactValueTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Testing exact walking distance values...");
            
            // Create a logger factory
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
            });
            
            var heuristic = new WalkingDistanceHeuristic(loggerFactory.CreateLogger<WalkingDistanceHeuristic>());
            
            // Test 2x2 scrambled state
            var scrambled2x2 = new PuzzleState(2, 2, new int[]
            {
                2, 1,
                3, 0
            });
            
            var result2x2 = heuristic.Calculate(scrambled2x2);
            Console.WriteLine($"2x2 scrambled state [2,1,3,0]: {result2x2}");
            
            // Test empty tile in middle
            var middleEmpty = new PuzzleState(3, 3, new int[]
            {
                1, 2, 3,
                4, 0, 6,
                7, 8, 5
            });
            
            var resultMiddle = heuristic.Calculate(middleEmpty);
            Console.WriteLine($"Empty tile in middle: {resultMiddle}");
            
            // Test one move away
            var oneMove = new PuzzleState(3, 3, new int[]
            {
                1, 2, 3,
                4, 5, 6,
                7, 0, 8
            });
            
            var resultOneMove = heuristic.Calculate(oneMove);
            Console.WriteLine($"One move away: {resultOneMove}");
            
            // Test scrambled state
            var scrambled = new PuzzleState(3, 3, new int[]
            {
                2, 1, 3,
                4, 5, 6,
                7, 8, 0
            });
            
            var resultScrambled = heuristic.Calculate(scrambled);
            Console.WriteLine($"Scrambled state [2,1,3,4,5,6,7,8,0]: {resultScrambled}");
        }
    }
}
