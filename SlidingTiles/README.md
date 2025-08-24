# Sliding Tiles Puzzle Tool

A comprehensive tool for working with sliding tile puzzles, featuring validation, heuristic evaluation, and puzzle generation capabilities.

## Features

- **File Validation**: Validates puzzle file format and checks puzzle solvability
- **Heuristic Evaluation**: Applies multiple heuristics to puzzle instances
- **Multiple Puzzle Sizes**: Supports puzzles of any width and height
- **Puzzle Generation**: Generates all valid 3x3 puzzle instances using BFS
- **Compression Support**: Supports both plain text (.puz) and gzipped (.puz.gz) files
- **Three Built-in Heuristics**:
  - **hd**: Hamming Distance - counts misplaced tiles
  - **md**: Manhattan Distance - sum of Manhattan distances to goal positions
  - **mc**: Manhattan Distance with Linear Conflicts - adds penalty for linear conflicts

## Usage

### Validation
```bash
dotnet run -- validate --file <filename>
```

### Evaluation
```bash
dotnet run -- eval --file <filename> --heuristics <heuristic_list>
```

### Generation
```bash
# Generate plain text .puz file
dotnet run -- generate --output <filename>

# Generate gzipped .puz.gz file
dotnet run -- gzip --output <filename>
```

Where `<heuristic_list>` is a comma-separated list of 2-letter heuristic abbreviations.

## File Format

The tool uses a human-readable text format for puzzle files with `.puz` extension. Each file contains one or more blocks, where each block defines puzzles of the same dimensions. The tool also supports gzipped `.puz.gz` files for space efficiency.

### Block Structure

1. **Metadata Section**: Starts with `#` and contains pipe-separated key-value pairs
   - Required: `width` and `height`
   - Optional: `source` and any other custom properties
2. **Problem Instances**: Each instance is a comma-separated list of cell values followed by optional metadata
3. **Instance Metadata**: Optional metadata for each instance (e.g., optimal solution cost) specified on the same line after a `#` character

### Example File

```
#width:3|height:3|source:Example 3x3 Puzzles
1,2,3,4,0,6,7,5,8#optimal:1
1,2,3,4,5,6,7,8,0#optimal:0
8,7,6,5,4,3,2,1,0#optimal:?

#width:4|height:4|source:Example 4x4 Puzzles
1,2,3,4,5,6,7,8,9,10,11,0,13,14,15,12#optimal:1
1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,0#optimal:0
1,2,3,4,5,6,7,8,9,10,11,12,13,14,0,15#optimal:1
```

### Format Rules

- Cell values must be integers from 0 to (width × height - 1)
- Value 0 represents the empty tile
- Each value must appear exactly once
- Problem instance metadata is specified on the same line after a `#` character
- Blocks are separated by blank lines
- No blank lines are needed between problem instances within a block
- The file must end with a blank line
- Instance metadata is optional but recommended

## Solvability Check

The tool automatically checks if each puzzle is solvable using the following rules:

- **2×2 puzzles**: Number of inversions must be even
- **Odd-width puzzles (≥3)**: Number of inversions must be even
- **Even-width puzzles (≥4)**: (Number of inversions + row of empty tile from bottom) must be even

## Building and Running

```bash
# Build the project
dotnet build

# Run validation
dotnet run -- validate --file example_puzzles.puz

# Run evaluation with all heuristics
dotnet run -- eval --file example_puzzles.puz --heuristics hd,md,mc

# Generate all valid 3x3 puzzles
dotnet run -- generate --output all_3x3_puzzles.puz

# Generate compressed 3x3 puzzles
dotnet run -- gzip --output all_3x3_puzzles.puz.gz
```

## Heuristic Details

### Hamming Distance (hd)
Counts the number of tiles that are not in their goal position. This is a simple but effective heuristic that provides a lower bound on the number of moves needed.

### Manhattan Distance (md)
Calculates the sum of Manhattan distances from each tile's current position to its goal position. This is admissible and often more informative than Hamming distance.

### Manhattan Distance with Linear Conflicts (mc)
Extends Manhattan distance by adding penalties for linear conflicts. A linear conflict occurs when two tiles that belong in the same row or column are in that row or column but in the wrong order. This heuristic is still admissible and often provides better estimates than basic Manhattan distance.

## Performance Considerations

The tool is designed for performance-critical evaluation scenarios:
- Uses efficient in-memory data structures
- Minimizes allocations during heuristic calculations
- Provides fine control over memory representation
- Built with .NET for cross-platform performance
- Supports gzipped files for efficient storage and transfer
