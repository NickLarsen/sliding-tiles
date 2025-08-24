using System;
using System.Collections.Generic;
using System.Linq;

namespace SlidingTiles
{
    public class PuzzleEvaluator
    {
        private readonly PuzzleFileParser _parser = new PuzzleFileParser();

        public EvaluationResult EvaluateFile(string filename, List<string> heuristicAbbreviations)
        {
            var blocks = _parser.ParseFile(filename);
            var heuristics = HeuristicFactory.GetHeuristics(heuristicAbbreviations);
            var result = new EvaluationResult();

            foreach (var block in blocks)
            {
                if (block.Metadata == null)
                {
                    throw new InvalidOperationException($"Block at index {blocks.IndexOf(block)} has null metadata");
                }
                
                var blockResult = new BlockEvaluationResult
                {
                    Width = block.Metadata.Width,
                    Height = block.Metadata.Height,
                    Source = block.Metadata.Source
                };

                foreach (var instance in block.Instances)
                {
                    var instanceResult = new InstanceEvaluationResult
                    {
                        Cells = instance.Cells,
                        OptimalValue = instance.OptimalValue
                    };

                    var puzzleState = new PuzzleState(block.Metadata.Width, block.Metadata.Height, instance.Cells);

                    foreach (var heuristic in heuristics)
                    {
                        var heuristicResult = new HeuristicResult
                        {
                            HeuristicName = heuristic.Name,
                            Value = heuristic.Calculate(puzzleState)
                        };
                        instanceResult.HeuristicResults.Add(heuristicResult);
                    }

                    blockResult.InstanceResults.Add(instanceResult);
                }

                result.BlockResults.Add(blockResult);
            }

            return result;
        }
    }
}
