using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Collections.Generic; // Added for Dictionary

namespace SlidingTiles
{
    class Program
    {
        static int Main(string[] args)
        {
            var rootCommand = new RootCommand("Sliding Tiles Puzzle Tool");

            var validateCommand = new Command("validate", "Validate a puzzle file");
            var fileOption = new Option<FileInfo>(
                "--file",
                "The puzzle file to validate (.puz or .puz.gz)")
            {
                IsRequired = true
            };
            validateCommand.AddOption(fileOption);
            validateCommand.SetHandler((file) => ValidateCommand(file), fileOption);
            rootCommand.AddCommand(validateCommand);

            var evalCommand = new Command("eval", "Evaluate heuristics on a puzzle file");
            var evalFileOption = new Option<FileInfo>(
                "--file",
                "The puzzle file to evaluate (.puz or .puz.gz)")
            {
                IsRequired = false
            };
            var heuristicsOption = new Option<string>(
                "--heuristics",
                "Comma-separated list of heuristic abbreviations (hd, md, mc)")
            {
                IsRequired = false
            };
            evalCommand.AddOption(evalFileOption);
            evalCommand.AddOption(heuristicsOption);
            evalCommand.SetHandler((file, heuristics) => EvalCommand(file, heuristics), evalFileOption, heuristicsOption);
            rootCommand.AddCommand(evalCommand);

            var listHeuristicsCommand = new Command("list-heuristics", "List all available heuristics with their codes and descriptions");
            evalCommand.AddCommand(listHeuristicsCommand);
            listHeuristicsCommand.SetHandler(() => ListHeuristicsCommand());

            var generateCommand = new Command("generate", "Generate all valid 3x3 puzzle instances using BFS and compress with gzip");
            var outputFileOption = new Option<FileInfo>(
                "--output",
                "The output gzipped puzzle file to generate (will use .puz.gz extension)")
            {
                IsRequired = true
            };
            var sourceOption = new Option<string>(
                "--source",
                "Source description for the generated puzzles")
            {
                IsRequired = false
            };
            generateCommand.AddOption(outputFileOption);
            generateCommand.AddOption(sourceOption);
            generateCommand.SetHandler((outputFile, source) => GenerateCommand(outputFile, source), outputFileOption, sourceOption);
            rootCommand.AddCommand(generateCommand);

            return rootCommand.Invoke(args);
        }

        static int ValidateCommand(FileInfo file)
        {
            if (!file.Exists)
            {
                Console.WriteLine($"Error: File '{file.FullName}' not found");
                return 1;
            }

            var validator = new PuzzleFileValidator();
            var result = validator.ValidateFile(file.FullName);
            
            if (result.IsValid)
            {
                Console.WriteLine($"File '{file.FullName}' is valid!");
                Console.WriteLine($"Total blocks: {result.BlockCount}");
                Console.WriteLine($"Total instances: {result.InstanceCount}");
                return 0;
            }
            else
            {
                Console.WriteLine($"File '{file.FullName}' is invalid:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"  - {error}");
                }
                return 1;
            }
        }

        static int EvalCommand(FileInfo? file, string? heuristics)
        {
            if (file == null)
            {
                Console.WriteLine("Error: File is required for evaluation. Use --file option or see 'eval list-heuristics' for available heuristics.");
                return 1;
            }

            if (!file.Exists)
            {
                Console.WriteLine($"Error: File '{file.FullName}' not found");
                return 1;
            }

            if (string.IsNullOrEmpty(heuristics))
            {
                Console.WriteLine("Error: Heuristics are required. Use --heuristics option or see 'eval list-heuristics' for available heuristics.");
                return 1;
            }

            var evaluator = new PuzzleEvaluator();
            var heuristicList = heuristics.Split(',').Select(h => h.Trim().ToLower()).ToList();
            
            var result = evaluator.EvaluateFile(file.FullName, heuristicList);
            
            Console.WriteLine($"Evaluation Results for '{file.FullName}':");
            Console.WriteLine();
            
            foreach (var blockResult in result.BlockResults)
            {
                Console.WriteLine($"Block: {blockResult.Width}x{blockResult.Height}");
                if (!string.IsNullOrEmpty(blockResult.Source))
                {
                    Console.WriteLine($"Source: {blockResult.Source}");
                }
                Console.WriteLine();
                
                foreach (var instanceResult in blockResult.InstanceResults)
                {
                    Console.WriteLine($"Instance: [{string.Join(",", instanceResult.Cells)}]");
                    Console.WriteLine($"  Optimal: {instanceResult.OptimalValue}");
                    
                    foreach (var heuristicResult in instanceResult.HeuristicResults)
                    {
                        Console.WriteLine($"  {heuristicResult.HeuristicName}: {heuristicResult.Value}");
                    }
                    Console.WriteLine();
                }
            }
            
            return 0;
        }

        static int ListHeuristicsCommand()
        {
            Console.WriteLine("Available Heuristics:");
            Console.WriteLine();
            Console.WriteLine("Code  | Name                                   | Description");
            Console.WriteLine("------|----------------------------------------|----------------------------------------");
            
            // Get all available heuristics from the factory
            var availableHeuristics = HeuristicFactory.GetAvailableHeuristics();
            
            foreach (var (code, name, description) in availableHeuristics)
            {
                // Truncate name and description to fit table columns
                var truncatedName = TruncateString(name, 38);
                var truncatedDescription = TruncateString(description, 38);
                
                Console.WriteLine($"{code,-5} | {truncatedName,-38} | {truncatedDescription}");
            }
            
            if (availableHeuristics.Count == 0)
            {
                Console.WriteLine("No working heuristics found.");
                return 1;
            }
            
            Console.WriteLine();
            Console.WriteLine("Usage Examples:");
            var codes = string.Join(",", availableHeuristics.Select(h => h.Code));
            Console.WriteLine($"  dotnet run -- eval --file puzzles.puz --heuristics {codes}");
            Console.WriteLine();
            Console.WriteLine("Note: Use comma-separated list without spaces for multiple heuristics.");
            return 0;
        }

        private static string TruncateString(string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input) || input.Length <= maxLength)
                return input;
            
            // Truncate and add ellipsis
            return input.Substring(0, maxLength - 3) + "...";
        }

        static int GenerateCommand(FileInfo outputFile, string? source)
        {
            try
            {
                Console.WriteLine("Generating all valid 3x3 puzzle instances using BFS and compressing with gzip...");
                
                var generator = new PuzzleGenerator(3, 3);
                var sourceDescription = string.IsNullOrEmpty(source) ? "Generated 3x3 Puzzles - All Valid Solvable Configurations" : source;
                
                generator.SaveToFile(outputFile.FullName, sourceDescription);
                
                // Count the puzzles to show how many were generated
                var puzzles = generator.GenerateAllPuzzles();
                
                Console.WriteLine($"Successfully generated {puzzles.Count} valid 3x3 puzzle instances and compressed with gzip!");
                Console.WriteLine($"Output file: {outputFile.FullName}");
                Console.WriteLine($"Source: {sourceDescription}");
                
                // Show file size information
                var fileInfo = new FileInfo(outputFile.FullName);
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
