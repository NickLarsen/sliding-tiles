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

            // Test 5x5 database construction time
            var stopwatch = Stopwatch.StartNew();
            var heuristic = new WalkingDistanceHeuristic(logger, 5, 5);
            stopwatch.Stop();

            Console.WriteLine($"5x5 database built in {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Database size: {heuristic.DatabaseSize} entries");
            Console.WriteLine($"Max heuristic value: {heuristic.MaxHeuristicValue}");

            // Test some valid states
            var testStates = new[]
            {
                new PuzzleState(5, 5, new int[] { 
                    1, 2, 3, 4, 5,
                    6, 7, 8, 9, 10,
                    11, 12, 13, 14, 15,
                    16, 17, 18, 19, 20,
                    21, 22, 23, 24, 0
                }), // Goal state
                new PuzzleState(5, 5, new int[] { 
                    1, 2, 3, 4, 5,
                    6, 7, 8, 9, 10,
                    11, 12, 13, 14, 15,
                    16, 17, 18, 19, 20,
                    21, 22, 23, 0, 24
                }), // One move away
                new PuzzleState(5, 5, new int[] { 
                    25, 24, 23, 22, 21,
                    20, 19, 18, 17, 16,
                    15, 14, 13, 12, 11,
                    10, 9, 8, 7, 6,
                    5, 4, 3, 2, 0
                }), // Completely reversed
            };

            for (int i = 0; i < testStates.Length; i++)
            {
                var state = testStates[i];
                try
                {
                    var result = heuristic.Calculate(state);
                    Console.WriteLine($"State {i + 1}: {result}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"State {i + 1}: Error - {ex.Message}");
                }
            }
            
            // Let's also test a 4x4 to see if the issue is specific to 5x5
            Console.WriteLine("\nTesting 4x4:");
            var heuristic4x4 = new WalkingDistanceHeuristic(logger, 4, 4);
            Console.WriteLine($"4x4 database size: {heuristic4x4.DatabaseSize} entries");
            Console.WriteLine($"4x4 max heuristic value: {heuristic4x4.MaxHeuristicValue}");
        }
    }
}
