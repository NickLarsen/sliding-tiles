using System;
using System.IO;
using System.Linq;

namespace SlidingTiles.Commands
{
    public class GenerateCommand : ICommand
    {
        private readonly FileInfo _outputFile;
        private readonly string? _source;

        public GenerateCommand(FileInfo outputFile, string? source)
        {
            _outputFile = outputFile;
            _source = source;
        }

        public string Name => "generate";
        public string Description => "Generate all valid 3x3 puzzle instances using BFS and compress with gzip";

        public int Execute()
        {
            try
            {
                Console.WriteLine("Generating all valid 3x3 puzzle instances using BFS and compressing with gzip...");
                
                var generator = new PuzzleGenerator(3, 3);
                var sourceDescription = string.IsNullOrEmpty(_source) ? "Generated 3x3 Puzzles - All Valid Solvable Configurations" : _source;
                
                generator.SaveToFile(_outputFile.FullName, sourceDescription);
                
                // Count the puzzles to show how many were generated
                var puzzles = generator.GenerateAllPuzzles();
                
                Console.WriteLine($"Successfully generated {puzzles.Count} valid 3x3 puzzle instances and compressed with gzip!");
                Console.WriteLine($"Output file: {_outputFile.FullName}");
                Console.WriteLine($"Source: {sourceDescription}");
                
                // Show file size information
                var fileInfo = new FileInfo(_outputFile.FullName);
                if (fileInfo.Exists)
                {
                    var sizeKB = fileInfo.Length / 1024.0;
                    Console.WriteLine($"File size: {sizeKB:F1} KB");
                }
                
                // Show some statistics
                var maxDepth = puzzles.Max(p => int.Parse(p.OptimalValue));
                Console.WriteLine($"Maximum depth (optimal moves): {maxDepth}");
                
                // Group by depth and show counts
                var depthGroups = puzzles.GroupBy(p => int.Parse(p.OptimalValue))
                                       .OrderBy(g => g.Key)
                                       .Select(g => new { Depth = g.Key, Count = g.Count() });
                
                Console.WriteLine("\nPuzzle distribution by depth:");
                foreach (var group in depthGroups)
                {
                    Console.WriteLine($"  {group.Depth} moves: {group.Count} puzzles");
                }
                
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating puzzles: {ex.Message}");
                return 1;
            }
        }
    }
}
