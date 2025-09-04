using System;
using SlidingTiles;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DebugWalkingDistance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<WalkingDistanceHeuristic>();

            Console.WriteLine("Testing Walking Distance Heuristic Performance...");

            // Test 4x4 database construction time
            var stopwatch = Stopwatch.StartNew();
            var heuristic = new WalkingDistanceHeuristic(logger, 4, 4);
            stopwatch.Stop();

            Console.WriteLine($"4x4 database built in {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Database size: {heuristic.DatabaseSize} entries");
            Console.WriteLine($"Max heuristic value: {heuristic.MaxHeuristicValue}");

            // Test some states
            var testStates = new[]
            {
                new PuzzleState(4, 4, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0 }), // Goal
                new PuzzleState(4, 4, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 0, 15 }), // One move away
                new PuzzleState(4, 4, new int[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 })  // Completely reversed
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
