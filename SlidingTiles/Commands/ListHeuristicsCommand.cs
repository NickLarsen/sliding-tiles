using System;
using System.Linq;

namespace SlidingTiles.Commands
{
    public class ListHeuristicsCommand : ICommand
    {
        public string Name => "list-heuristics";
        public string Description => "List all available heuristics with their codes and descriptions";

        public int Execute()
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
    }
}
