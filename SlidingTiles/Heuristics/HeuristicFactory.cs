using System;
using System.Collections.Generic;
using System.Linq;

namespace SlidingTiles
{
    public static class HeuristicFactory
    {
        private static readonly Dictionary<string, IHeuristic> _heuristics = new Dictionary<string, IHeuristic>
        {
            { "hd", new HammingDistanceHeuristic() },
            { "md", new ManhattanDistanceHeuristic() },
            { "mc", new ManhattanDistanceWithLinearConflictsHeuristic() },
            { "wd", new WalkingDistanceHeuristic() }
        };

        public static IHeuristic GetHeuristic(string abbreviation)
        {
            if (_heuristics.TryGetValue(abbreviation.ToLower(), out var heuristic))
            {
                return heuristic;
            }
            throw new ArgumentException($"Unknown heuristic abbreviation: {abbreviation}");
        }

        public static List<IHeuristic> GetHeuristics(List<string> abbreviations)
        {
            return abbreviations.Select(GetHeuristic).ToList();
        }

        public static List<(string Code, string Name, string Description)> GetAvailableHeuristics()
        {
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
