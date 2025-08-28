using System.CommandLine;
using System.IO;

namespace SlidingTiles.Commands
{
    public static class CommandFactory
    {
        public static Command CreateValidateCommand()
        {
            var validateCommand = new Command("validate", "Validate a puzzle file");
            var fileOption = new Option<FileInfo>(
                "--file",
                "The puzzle file to validate (.puz or .puz.gz)")
            {
                IsRequired = true
            };
            validateCommand.AddOption(fileOption);
            validateCommand.SetHandler((file) => new ValidateCommand(file).Execute(), fileOption);
            return validateCommand;
        }

        public static Command CreateEvalCommand()
        {
            var evalCommand = new Command("eval", "Evaluate heuristics on a puzzle file");
            var evalFileOption = new Option<FileInfo>(
                "--file",
                "The puzzle file to evaluate (.puz or .puz.gz)")
            {
                IsRequired = false
            };
            var heuristicsOption = new Option<string>(
                "--heuristics",
                "Comma-separated list of heuristic abbreviations or 'all' for all heuristics")
            {
                IsRequired = false
            };
            var outputOption = new Option<FileInfo?>(
                "--output",
                "Optional output CSV file to write results (if not provided, results are only displayed)")
            {
                IsRequired = false
            };
            evalCommand.AddOption(evalFileOption);
            evalCommand.AddOption(heuristicsOption);
            evalCommand.AddOption(outputOption);
            evalCommand.SetHandler((file, heuristics, output) => new EvalCommand(file, heuristics, output).Execute(), evalFileOption, heuristicsOption, outputOption);
            return evalCommand;
        }

        public static Command CreateListHeuristicsCommand()
        {
            var listHeuristicsCommand = new Command("list-heuristics", "List all available heuristics with their codes and descriptions");
            listHeuristicsCommand.SetHandler(() => new ListHeuristicsCommand().Execute());
            return listHeuristicsCommand;
        }

        public static Command CreateGenerateCommand()
        {
            var generateCommand = new Command("generate", "Generate all valid 3x3 puzzle instances using BFS and compress with gzip");
            var outputFileOption = new Option<FileInfo>(
                "--output",
                "The output gzipped puzzle file to generate (will use .puz.gz extension)")
            {
                IsRequired = true
            };
            var sourceOption = new Option<string>(
                "--source",
                "Source description for the generated puzzles")
            {
                IsRequired = false
            };
            generateCommand.AddOption(outputFileOption);
            generateCommand.AddOption(sourceOption);
            generateCommand.SetHandler((outputFile, source) => new GenerateCommand(outputFile, source).Execute(), outputFileOption, sourceOption);
            return generateCommand;
        }
    }
}
