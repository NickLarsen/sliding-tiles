using System;
using System.IO;
using System.Linq;
using SlidingTiles;

namespace SlidingTiles.Commands
{
    public class EvalCommand : ICommand
    {
        private readonly FileInfo? _file;
        private readonly string? _heuristics;
        private readonly FileInfo? _outputFile;

        public EvalCommand(FileInfo? file, string? heuristics, FileInfo? outputFile)
        {
            _file = file;
            _heuristics = heuristics;
            _outputFile = outputFile;
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

            var heuristicList = _heuristics.Split(',').Select(h => h.Trim().ToLower()).ToList();
            
            // Handle special "all" value - expand to all available heuristics
            if (heuristicList.Contains("all"))
            {
                var availableHeuristics = HeuristicFactory.GetAvailableHeuristics();
                heuristicList = availableHeuristics.Select(h => h.Code).ToList();
            }
            
            // Process file once and write results directly
            ProcessFileAndWriteResults(_file.FullName, heuristicList);
            
            return 0;
        }
        
        private void ProcessFileAndWriteResults(string filename, List<string> heuristicAbbreviations)
        {
            var parser = new PuzzleFileParser();
            var heuristics = HeuristicFactory.GetHeuristics(heuristicAbbreviations);
            var heuristicNames = heuristics.Select(h => h.Name).ToList();
            
            // Initialize CSV writer if output file is specified
            StreamWriter? csvWriter = null;
            if (_outputFile != null)
            {
                try
                {
                    csvWriter = new StreamWriter(_outputFile.FullName);
                    
                    // Write CSV header
                    var headerColumns = new List<string> { "Puzzle", "Optimal" };
                    headerColumns.AddRange(heuristicNames);
                    csvWriter.WriteLine(string.Join(",", headerColumns));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating CSV file: {ex.Message}");
                    csvWriter = null;
                }
            }
            
            try
            {
                var blocks = parser.ParseFile(filename);
                
                // Display results to console
                Console.WriteLine($"Evaluation Results for '{filename}':");
                Console.WriteLine();
                
                foreach (var block in blocks)
                {
                    if (block.Metadata == null)
                    {
                        throw new InvalidOperationException($"Block at index {blocks.IndexOf(block)} has null metadata");
                    }
                    
                    Console.WriteLine($"Block: {block.Metadata.Width}x{block.Metadata.Height}");
                    if (!string.IsNullOrEmpty(block.Metadata.Source))
                    {
                        Console.WriteLine($"Source: {block.Metadata.Source}");
                    }
                    Console.WriteLine();
                    
                    foreach (var instance in block.Instances)
                    {
                        var puzzleState = new PuzzleState(block.Metadata.Width, block.Metadata.Height, instance.Cells);
                        
                        // Calculate all heuristics once
                        var heuristicResults = new List<HeuristicResult>();
                        foreach (var heuristic in heuristics)
                        {
                            var heuristicResult = new HeuristicResult
                            {
                                HeuristicName = heuristic.Name,
                                Value = heuristic.Calculate(puzzleState)
                            };
                            heuristicResults.Add(heuristicResult);
                        }
                        
                        // Display in console
                        Console.WriteLine($"Instance: [{string.Join(",", instance.Cells)}]");
                        Console.WriteLine($"  Optimal: {instance.OptimalValue}");
                        
                        foreach (var heuristicResult in heuristicResults)
                        {
                            Console.WriteLine($"  {heuristicResult.HeuristicName}: {heuristicResult.Value}");
                        }
                        Console.WriteLine();
                        
                        // Write to CSV if output file is specified
                        if (csvWriter != null)
                        {
                            var puzzleRepresentation = $"[{string.Join(",", instance.Cells)}]";
                            var row = new List<string> { QuoteCsvField(puzzleRepresentation), instance.OptimalValue };
                            
                            // Add heuristic values in the same order as the header
                            foreach (var heuristicResult in heuristicResults)
                            {
                                row.Add(heuristicResult.Value.ToString());
                            }
                            
                            csvWriter.WriteLine(string.Join(",", row));
                        }
                    }
                }
                
                if (csvWriter != null && _outputFile != null)
                {
                    Console.WriteLine($"Results written to CSV file: {_outputFile.FullName}");
                }
            }
            finally
            {
                csvWriter?.Dispose();
            }
        }
        
        private static string QuoteCsvField(string field)
        {
            // Quote the field if it contains comma, quote, or newline
            if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            {
                // Escape quotes by doubling them and wrap in quotes
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }
    }
}
