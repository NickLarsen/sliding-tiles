using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Collections.Generic; // Added for Dictionary
using SlidingTiles.Commands;
using Microsoft.Extensions.Logging;

namespace SlidingTiles
{
    class Program
    {
        static int Main(string[] args)
        {
            // Set up logging
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .SetMinimumLevel(LogLevel.Debug)
                    .AddConsole();
            });

            // Initialize the heuristic factory with logging
            HeuristicFactory.Initialize(loggerFactory);

            var rootCommand = new RootCommand("Sliding Tiles Puzzle Tool");

            // Create and add all commands using the factory
            var validateCommand = CommandFactory.CreateValidateCommand();
            rootCommand.AddCommand(validateCommand);

            var evalCommand = CommandFactory.CreateEvalCommand();
            rootCommand.AddCommand(evalCommand);

            var listHeuristicsCommand = CommandFactory.CreateListHeuristicsCommand();
            evalCommand.AddCommand(listHeuristicsCommand);

            var generateCommand = CommandFactory.CreateGenerateCommand();
            rootCommand.AddCommand(generateCommand);

            var buildWalkingDistanceCommand = CommandFactory.CreateBuildWalkingDistanceCommand();
            rootCommand.AddCommand(buildWalkingDistanceCommand);



            return rootCommand.Invoke(args);
        }
    }
}
