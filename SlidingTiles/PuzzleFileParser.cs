using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SlidingTiles
{
    public class PuzzleFileParser
    {
        public List<PuzzleBlock> ParseFile(string filename)
        {
            var lines = File.ReadAllLines(filename);
            var blocks = new List<PuzzleBlock>();
            PuzzleBlock? currentBlock = null;
            BlockMetadata? currentMetadata = null;

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                
                // Skip empty lines
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                // Check if this is a metadata section
                if (line.StartsWith("#"))
                {
                    // If we have a current block with instances, save it
                    if (currentBlock != null && currentBlock.Instances.Count > 0)
                    {
                        blocks.Add(currentBlock);
                    }
                    
                    // Start a new block
                    currentBlock = new PuzzleBlock();
                    currentMetadata = new BlockMetadata();
                    
                    // Parse metadata key-value pairs
                    var metadataPairs = line.Substring(1).Split('|');
                    foreach (var pair in metadataPairs)
                    {
                        var colonIndex = pair.IndexOf(':');
                        if (colonIndex > 0)
                        {
                            var key = pair.Substring(0, colonIndex).Trim();
                            var value = pair.Substring(colonIndex + 1).Trim();
                            
                            switch (key.ToLower())
                            {
                                case "width":
                                    if (int.TryParse(value, out int width))
                                        currentMetadata.Width = width;
                                    break;
                                case "height":
                                    if (int.TryParse(value, out int height))
                                        currentMetadata.Height = height;
                                    break;
                                case "source":
                                    currentMetadata.Source = value;
                                    break;
                                default:
                                    currentMetadata.AdditionalProperties[key] = value;
                                    break;
                            }
                        }
                    }
                    
                    // Set metadata for current block
                    currentBlock.Metadata = currentMetadata;
                    continue;
                }

                // If we're here, this must be a problem instance
                if (currentBlock == null || currentMetadata == null)
                {
                    throw new FormatException($"Problem instance found before metadata at line {i + 1}");
                }
                
                // Parse the cell list
                var cellStrings = line.Split(',');
                var cells = new int[cellStrings.Length];
                
                for (int j = 0; j < cellStrings.Length; j++)
                {
                    if (!int.TryParse(cellStrings[j].Trim(), out cells[j]))
                    {
                        throw new FormatException($"Invalid cell value at line {i + 1}: {cellStrings[j]}");
                    }
                }

                // Parse inline metadata (next line)
                var instance = new ProblemInstance { Cells = cells };
                
                if (i + 1 < lines.Length)
                {
                    var nextLine = lines[i + 1].Trim();
                    if (nextLine.StartsWith("#"))
                    {
                        // Parse instance metadata
                        var metadataPairs = nextLine.Substring(1).Split('|');
                        foreach (var pair in metadataPairs)
                        {
                            var colonIndex = pair.IndexOf(':');
                            if (colonIndex > 0)
                            {
                                var key = pair.Substring(0, colonIndex).Trim();
                                var value = pair.Substring(colonIndex + 1).Trim();
                                
                                switch (key.ToLower())
                                {
                                    case "optimal":
                                        instance.OptimalValue = value;
                                        break;
                                    default:
                                        instance.AdditionalProperties[key] = value;
                                        break;
                                }
                            }
                        }
                        i++; // Skip the metadata line in next iteration
                    }
                }
                
                currentBlock.Instances.Add(instance);
            }

            // Add the last block if it has instances
            if (currentBlock != null && currentBlock.Instances.Count > 0)
            {
                blocks.Add(currentBlock);
            }

            return blocks;
        }
    }
}
