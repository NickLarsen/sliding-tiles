using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SlidingTiles
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintUsage();
                return 1;
            }

            try
            {
                switch (args[0].ToLower())
                {
                    case "validate":
                        return HandleValidate(args.Skip(1).ToArray());
                    case "eval":
                        return HandleEval(args.Skip(1).ToArray());
                    default:
                        Console.WriteLine($"Unknown command: {args[0]}");
                        PrintUsage();
                        return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("Sliding Tiles Puzzle Tool");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  slider validate --file <filename>");
            Console.WriteLine("  slider eval --file <filename> --heuristics <heuristic_list>");
            Console.WriteLine();
            Console.WriteLine("Heuristics (comma-separated, 2-letter abbreviations):");
            Console.WriteLine("  hd - Hamming Distance");
            Console.WriteLine("  md - Manhattan Distance");
            Console.WriteLine("  mc - Manhattan Distance with Linear Conflicts");
        }

        static int HandleValidate(string[] args)
        {
            string? filename = null;
            
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--file" && i + 1 < args.Length)
                {
                    filename = args[i + 1];
                    break;
                }
            }

            if (string.IsNullOrEmpty(filename))
            {
                Console.WriteLine("Error: --file parameter is required for validate command");
                return 1;
            }

            if (!File.Exists(filename))
            {
                Console.WriteLine($"Error: File '{filename}' not found");
                return 1;
            }

            var validator = new PuzzleFileValidator();
            var result = validator.ValidateFile(filename);
            
            if (result.IsValid)
            {
                Console.WriteLine($"File '{filename}' is valid!");
                Console.WriteLine($"Total blocks: {result.BlockCount}");
                Console.WriteLine($"Total instances: {result.InstanceCount}");
                return 0;
            }
            else
            {
                Console.WriteLine($"File '{filename}' is invalid:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"  - {error}");
                }
                return 1;
            }
        }

        static int HandleEval(string[] args)
        {
            string? filename = null;
            string? heuristics = null;
            
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--file" && i + 1 < args.Length)
                {
                    filename = args[i + 1];
                }
                else if (args[i] == "--heuristics" && i + 1 < args.Length)
                {
                    heuristics = args[i + 1];
                }
            }

            if (string.IsNullOrEmpty(filename))
            {
                Console.WriteLine("Error: --file parameter is required for eval command");
                return 1;
            }

            if (string.IsNullOrEmpty(heuristics))
            {
                Console.WriteLine("Error: --heuristics parameter is required for eval command");
                return 1;
            }

            if (!File.Exists(filename))
            {
                Console.WriteLine($"Error: File '{filename}' not found");
                return 1;
            }

            var evaluator = new PuzzleEvaluator();
            var heuristicList = heuristics.Split(',').Select(h => h.Trim().ToLower()).ToList();
            
            var result = evaluator.EvaluateFile(filename, heuristicList);
            
            Console.WriteLine($"Evaluation Results for '{filename}':");
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
