using System;
using System.Collections.Generic;
using System.Linq;

namespace SlidingTiles
{
    public class PuzzleFileValidator
    {
        private readonly PuzzleFileParser _parser = new PuzzleFileParser();

        public ValidationResult ValidateFile(string filename)
        {
            var result = new ValidationResult();
            
            try
            {
                var blocks = _parser.ParseFile(filename);
                result.BlockCount = blocks.Count;
                result.InstanceCount = blocks.Sum(b => b.Instances.Count);

                // Validate each block
                for (int blockIndex = 0; blockIndex < blocks.Count; blockIndex++)
                {
                    var block = blocks[blockIndex];
                    ValidateBlock(block, blockIndex, result);
                }

                result.IsValid = result.Errors.Count == 0;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"File parsing error: {ex.Message}");
                result.IsValid = false;
            }

            return result;
        }

        private void ValidateBlock(PuzzleBlock block, int blockIndex, ValidationResult result)
        {
            // Validate block metadata
            if (block.Metadata == null)
            {
                result.Errors.Add($"Block {blockIndex + 1}: Missing metadata");
                return;
            }

            if (block.Metadata.Width <= 0)
            {
                result.Errors.Add($"Block {blockIndex + 1}: Invalid width: {block.Metadata.Width}");
            }

            if (block.Metadata.Height <= 0)
            {
                result.Errors.Add($"Block {blockIndex + 1}: Invalid height: {block.Metadata.Height}");
            }

            if (block.Instances.Count == 0)
            {
                result.Errors.Add($"Block {blockIndex + 1}: No problem instances found");
                return;
            }

            // Validate each instance
            for (int instanceIndex = 0; instanceIndex < block.Instances.Count; instanceIndex++)
            {
                var instance = block.Instances[instanceIndex];
                ValidateInstance(instance, blockIndex, instanceIndex, block.Metadata, result);
            }
        }

        private void ValidateInstance(ProblemInstance instance, int blockIndex, int instanceIndex, BlockMetadata metadata, ValidationResult result)
        {
            var instanceId = $"Block {blockIndex + 1}, Instance {instanceIndex + 1}";

            // Check if cells array is null
            if (instance.Cells == null)
            {
                result.Errors.Add($"{instanceId}: Cells array is null");
                return;
            }

            // Check if the number of cells matches the expected size
            int expectedSize = metadata.Width * metadata.Height;
            if (instance.Cells.Length != expectedSize)
            {
                result.Errors.Add($"{instanceId}: Expected {expectedSize} cells, got {instance.Cells.Length}");
                return;
            }

            // Check if all values are in the valid range [0, N-1]
            for (int i = 0; i < instance.Cells.Length; i++)
            {
                if (instance.Cells[i] < 0 || instance.Cells[i] >= expectedSize)
                {
                    result.Errors.Add($"{instanceId}: Cell value {instance.Cells[i]} at position {i} is out of range [0, {expectedSize - 1}]");
                }
            }

            // Check for duplicate values
            var uniqueValues = new HashSet<int>(instance.Cells);
            if (uniqueValues.Count != instance.Cells.Length)
            {
                result.Errors.Add($"{instanceId}: Duplicate cell values found");
            }

            // Check if the puzzle is solvable
            var puzzleState = new PuzzleState(metadata.Width, metadata.Height, instance.Cells);
            if (!puzzleState.IsSolvable())
            {
                result.Errors.Add($"{instanceId}: Puzzle is not solvable");
            }

            // Check if optimal value is present
            if (string.IsNullOrEmpty(instance.OptimalValue))
            {
                result.Errors.Add($"{instanceId}: Missing optimal value");
            }
            else if (instance.OptimalValue != "?")
            {
                // If optimal value is not "?", it should be a positive integer
                if (!int.TryParse(instance.OptimalValue, out int optimal) || optimal < 0)
                {
                    result.Errors.Add($"{instanceId}: Invalid optimal value: {instance.OptimalValue}");
                }
            }
        }
    }
}
