# Commands Directory

This directory contains the refactored command classes that were previously part of the `Program.cs` file.

## Structure

- **`ICommand.cs`** - Base interface for all command classes
- **`CommandFactory.cs`** - Factory class for creating and configuring commands
- **`ValidateCommand.cs`** - Handles puzzle file validation
- **`EvalCommand.cs`** - Handles heuristic evaluation on puzzle files
- **`ListHeuristicsCommand.cs`** - Lists available heuristics
- **`GenerateCommand.cs`** - Handles puzzle generation

## Benefits of Refactoring

1. **Separation of Concerns** - Each command is now in its own class
2. **Maintainability** - Easier to modify individual commands without affecting others
3. **Testability** - Each command can be unit tested independently
4. **Readability** - Program.cs is now much cleaner and easier to follow
5. **Extensibility** - New commands can be easily added by creating new classes

## Usage

The `CommandFactory` class provides static methods to create each command with proper configuration. The main `Program.cs` file now simply creates commands using the factory and adds them to the root command.

## Adding New Commands

To add a new command:

1. Create a new class implementing `ICommand`
2. Add a factory method in `CommandFactory`
3. Register the command in `Program.cs`
