using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;

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
                "The puzzle file to validate")
            {
                IsRequired = true
            };
            validateCommand.AddOption(fileOption);
            validateCommand.SetHandler((file) => ValidateCommand(file), fileOption);
            rootCommand.AddCommand(validateCommand);

            var evalCommand = new Command("eval", "Evaluate heuristics on a puzzle file");
            var evalFileOption = new Option<FileInfo>(
                "--file",
                "The puzzle file to evaluate")
            {
                IsRequired = true
            };
            var heuristicsOption = new Option<string>(
                "--heuristics",
                "Comma-separated list of heuristic abbreviations (hd, md, mc)")
            {
                IsRequired = true
            };
            evalCommand.AddOption(evalFileOption);
            evalCommand.AddOption(heuristicsOption);
            evalCommand.SetHandler((file, heuristics) => EvalCommand(file, heuristics), evalFileOption, heuristicsOption);
            rootCommand.AddCommand(evalCommand);

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

        static int EvalCommand(FileInfo file, string heuristics)
        {
            if (!file.Exists)
            {
                Console.WriteLine($"Error: File '{file.FullName}' not found");
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
    }
}
