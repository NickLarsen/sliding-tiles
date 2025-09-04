using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Collections.Generic; // Added for Dictionary
using SlidingTiles.Commands;

namespace SlidingTiles
{
    class Program
    {
        static int Main(string[] args)
        {
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
