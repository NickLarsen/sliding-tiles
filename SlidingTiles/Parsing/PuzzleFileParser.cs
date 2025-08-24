using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace SlidingTiles
{
    public class PuzzleFileParser
    {
        public List<PuzzleBlock> ParseFile(string filename)
        {
            string[] lines;
            
            // Check if file is gzipped
            if (filename.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
            {
                lines = ReadGzippedFile(filename);
            }
            else
            {
                lines = File.ReadAllLines(filename, Encoding.UTF8);
            }
            
            var blocks = new List<PuzzleBlock>();
            PuzzleBlock? currentBlock = null;
            BlockMetadata? currentMetadata = null;

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                
                // Skip empty lines
                if (string.IsNullOrEmpty(line))
                {
                    // End of current block if we have one with instances
                    if (currentBlock != null && currentBlock.Instances.Count > 0)
                    {
                        blocks.Add(currentBlock);
                        currentBlock = null;
                        currentMetadata = null;
                    }
                    continue;
                }

                // Check if this is a metadata section (block header)
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
                
                // Parse the cell list and inline metadata
                var instance = ParseProblemInstance(line, i + 1);
                currentBlock.Instances.Add(instance);
            }

            // Add the last block if it has instances
            if (currentBlock != null && currentBlock.Instances.Count > 0)
            {
                blocks.Add(currentBlock);
            }

            return blocks;
        }

        private string[] ReadGzippedFile(string filename)
        {
            using var fileStream = File.OpenRead(filename);
            using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            using var reader = new StreamReader(gzipStream, Encoding.UTF8);
            
            var content = reader.ReadToEnd();
            return content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        }

        private ProblemInstance ParseProblemInstance(string line, int lineNumber)
        {
            // Split on # to separate cells from metadata
            var parts = line.Split('#', 2);
            var cellString = parts[0];
            var metadataString = parts.Length > 1 ? parts[1] : string.Empty;
            
            // Parse the cell list
            var cellStrings = cellString.Split(',');
            var cells = new int[cellStrings.Length];
            
            for (int j = 0; j < cellStrings.Length; j++)
            {
                if (!int.TryParse(cellStrings[j].Trim(), out cells[j]))
                {
                    throw new FormatException($"Invalid cell value at line {lineNumber}: {cellStrings[j]}");
                }
            }

            var instance = new ProblemInstance { Cells = cells };
            
            // Parse metadata if present
            if (!string.IsNullOrEmpty(metadataString))
            {
                var metadataPairs = metadataString.Split('|');
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
            }
            
            return instance;
        }
    }
}
