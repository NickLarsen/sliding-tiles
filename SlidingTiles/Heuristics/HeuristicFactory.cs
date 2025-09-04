using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace SlidingTiles
{
    public static class HeuristicFactory
    {
        private static Dictionary<string, IHeuristic>? _heuristics;
        private static ILoggerFactory? _loggerFactory;

        public static void Initialize(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _heuristics = new Dictionary<string, IHeuristic>
            {
                { "hd", new HammingDistanceHeuristic() },
                { "md", new ManhattanDistanceHeuristic() },
                { "mc", new ManhattanDistanceWithLinearConflictsHeuristic() },
                { "wd", new WalkingDistanceHeuristic(loggerFactory.CreateLogger<WalkingDistanceHeuristic>()) }
            };
        }

        public static IHeuristic GetHeuristic(string abbreviation, int width = 3, int height = 3)
        {
            if (_heuristics == null || _loggerFactory == null)
            {
                throw new InvalidOperationException("HeuristicFactory must be initialized with a logger factory before use.");
            }
            
            if (abbreviation.ToLower() == "wd")
            {
                // For walking distance, create a new instance with the specified size
                return new WalkingDistanceHeuristic(_loggerFactory.CreateLogger<WalkingDistanceHeuristic>(), width, height);
            }
            
            if (_heuristics.TryGetValue(abbreviation.ToLower(), out var heuristic))
            {
                return heuristic;
            }
            throw new ArgumentException($"Unknown heuristic abbreviation: {abbreviation}");
        }

        public static List<IHeuristic> GetHeuristics(List<string> abbreviations)
        {
            return abbreviations.Select(abbr => GetHeuristic(abbr)).ToList();
        }

        public static List<(string Code, string Name, string Description)> GetAvailableHeuristics()
        {
            if (_heuristics == null)
            {
                throw new InvalidOperationException("HeuristicFactory must be initialized with a logger factory before use.");
            }
            
            var availableHeuristics = new List<(string Code, string Name, string Description)>();
            
            foreach (var kvp in _heuristics)
            {
                var code = kvp.Key;
                var heuristic = kvp.Value;
                availableHeuristics.Add((code, heuristic.Name, heuristic.Description));
            }
            
            return availableHeuristics;
        }
    }
}
