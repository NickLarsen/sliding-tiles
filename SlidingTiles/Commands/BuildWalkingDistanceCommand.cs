using System;
using System.Diagnostics;

namespace SlidingTiles.Commands
{
    public class BuildWalkingDistanceCommand : ICommand
    {
        private readonly int _size;

        public string Name => "build-walking-distance";
        public string Description => $"Build WalkingDistanceHeuristic database for {_size}x{_size} puzzles";

        public BuildWalkingDistanceCommand(int size)
        {
            _size = size;
        }

        public int Execute()
        {
            if (_size <= 0)
            {
                Console.WriteLine($"❌ Error: Size must be a positive integer (e.g., 3 for 3x3, 4 for 4x4). Got: {_size}");
                return 1;
            }

            Console.WriteLine($"Building WalkingDistanceHeuristic database for {_size}x{_size} puzzles...");
            Console.WriteLine();

            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var heuristic = HeuristicFactory.GetHeuristic("wd", _size, _size);
                stopwatch.Stop();

                // Cast to WalkingDistanceHeuristic to access database properties
                var walkingDistanceHeuristic = heuristic as WalkingDistanceHeuristic;
                if (walkingDistanceHeuristic == null)
                {
                    Console.WriteLine("❌ Error: Failed to create WalkingDistanceHeuristic");
                    return 1;
                }

                Console.WriteLine($"✅ Successfully built WalkingDistanceHeuristic for {_size}x{_size} puzzles");
                Console.WriteLine();
                Console.WriteLine("Database Statistics:");
                Console.WriteLine($"  Size: {_size}x{_size}");
                Console.WriteLine($"  Database entries: {walkingDistanceHeuristic.DatabaseSize + 1}");
                Console.WriteLine($"  Max heuristic value: {walkingDistanceHeuristic.MaxHeuristicValue}");
                Console.WriteLine($"  Build time: {stopwatch.ElapsedMilliseconds}ms");
                Console.WriteLine();
                Console.WriteLine("Heuristic Information:");
                Console.WriteLine($"  Name: {heuristic.Name}");
                Console.WriteLine($"  Abbreviation: {heuristic.Abbreviation}");
                Console.WriteLine($"  Description: {heuristic.Description}");
                
                return 0;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected error: {ex.Message}");
                return 1;
            }
        }
    }
}
