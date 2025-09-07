using System;
using System.Diagnostics;
using SlidingTiles;
using Microsoft.Extensions.Logging;

namespace QuickTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<WalkingDistanceHeuristic>();

            Console.WriteLine("Testing 4x4 Walking Distance Heuristic...");

            var stopwatch = Stopwatch.StartNew();
            var heuristic4x4 = new WalkingDistanceHeuristic(logger, 4, 4);
            stopwatch.Stop();

            Console.WriteLine($"4x4 database built in {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"4x4 database size: {heuristic4x4.DatabaseSize} entries");
            Console.WriteLine($"4x4 max heuristic value: {heuristic4x4.MaxHeuristicValue}");
            
            // Test a simple 4x4 state
            var state4x4 = new PuzzleState(4, 4, new int[] { 
                1, 2, 3, 4,
                5, 6, 7, 8,
                9, 10, 11, 12,
                13, 14, 15, 0
            });
            
            try
            {
                var result4x4 = heuristic4x4.Calculate(state4x4);
                Console.WriteLine($"4x4 goal state result: {result4x4}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"4x4 error: {ex.Message}");
            }
        }
    }
}