using System;
using System.IO;

namespace SlidingTiles.Commands
{
    public class ValidateCommand : ICommand
    {
        private readonly FileInfo _file;

        public ValidateCommand(FileInfo file)
        {
            _file = file;
        }

        public string Name => "validate";
        public string Description => "Validate a puzzle file";

        public int Execute()
        {
            if (!_file.Exists)
            {
                Console.WriteLine($"Error: File '{_file.FullName}' not found");
                return 1;
            }

            var validator = new PuzzleFileValidator();
            var result = validator.ValidateFile(_file.FullName);
            
            if (result.IsValid)
            {
                Console.WriteLine($"File '{_file.FullName}' is valid!");
                Console.WriteLine($"Total blocks: {result.BlockCount}");
                Console.WriteLine($"Total instances: {result.InstanceCount}");
                return 0;
            }
            else
            {
                Console.WriteLine($"File '{_file.FullName}' is invalid:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"  - {error}");
                }
                return 1;
            }
        }
    }
}
