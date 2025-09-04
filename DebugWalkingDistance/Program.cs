using System;
using SlidingTiles;
using Microsoft.Extensions.Logging;

namespace DebugWalkingDistance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Testing Walking Distance Heuristic...");
            
            // Create a logger factory
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<WalkingDistanceHeuristic>();

            var heuristic = new WalkingDistanceHeuristic(logger, 3, 3);

            var testStates = new[]
            {
                new PuzzleState(3, 3, new int[] { 1, 2, 3, 4, 0, 6, 7, 5, 8 }),
                new PuzzleState(3, 3, new int[] { 2, 1, 3, 4, 5, 6, 7, 8, 0 }),
                new PuzzleState(3, 3, new int[] { 8, 7, 6, 5, 4, 3, 2, 1, 0 })
            };

            for (int i = 0; i < testStates.Length; i++)
            {
                var state = testStates[i];
                var result = heuristic.Calculate(state);
                Console.WriteLine($"State {i + 1}: {result}");
            }
        }
    }
}
