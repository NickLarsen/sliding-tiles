using System;
using System.IO;
using System.Linq;

namespace SlidingTiles.Commands
{
    public class EvalCommand : ICommand
    {
        private readonly FileInfo? _file;
        private readonly string? _heuristics;

        public EvalCommand(FileInfo? file, string? heuristics)
        {
            _file = file;
            _heuristics = heuristics;
        }

        public string Name => "eval";
        public string Description => "Evaluate heuristics on a puzzle file";

        public int Execute()
        {
            if (_file == null)
            {
                Console.WriteLine("Error: File is required for evaluation. Use --file option or see 'eval list-heuristics' for available heuristics.");
                return 1;
            }

            if (!_file.Exists)
            {
                Console.WriteLine($"Error: File '{_file.FullName}' not found");
                return 1;
            }

            if (string.IsNullOrEmpty(_heuristics))
            {
                Console.WriteLine("Error: Heuristics are required. Use --heuristics option or see 'eval list-heuristics' for available heuristics.");
                return 1;
            }

            var evaluator = new PuzzleEvaluator();
            var heuristicList = _heuristics.Split(',').Select(h => h.Trim().ToLower()).ToList();
            
            var result = evaluator.EvaluateFile(_file.FullName, heuristicList);
            
            Console.WriteLine($"Evaluation Results for '{_file.FullName}':");
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
