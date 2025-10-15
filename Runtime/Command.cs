using System;

namespace UImGuiConsole
{
    /// <summary>
    /// Non-templated class that allows for the storage of commands as well as accessing certain functionality
    /// of said commands.
    /// </summary>
    public abstract class CommandBase
    {
        /// <summary>
        /// Gets info about the command and usage.
        /// </summary>
        /// <returns>String containing info about the command.</returns>
        public abstract string Help();

        /// <summary>
        /// Getter for the number of arguments the command takes.
        /// </summary>
        /// <returns>Returns the number of arguments taken by the command.</returns>
        public abstract int ArgumentCount { get; }

        /// <summary>
        /// Parses and runs the function held within the child class.
        /// </summary>
        /// <param name="input">String of arguments for the command to parse and pass to the function.</param>
        /// <returns><see cref="ConsoleItem"/> error if the parsing in someway was messed up, and none if there was no issue.</returns>
        public abstract ConsoleItem Invoke(string input);
    }

    public class Command : CommandBase
    {
        private string name;
        private string description;
        private Action<string> function;
        private string[] arguments; // TODO: This should be a generic object with type and value information

        public override int ArgumentCount => arguments.Length;

        public Command(string name, string description, Action<string> function, params string[] args)
        {
            this.name = name;
            this.description = description;
            this.function = function;
            arguments = args;
        }

        public override ConsoleItem Invoke(string input)
        {
            try
            {
                // Try to parse and call the function
                Call(input, arguments);
            }
            catch (Exception ae)
            {
                // Error happened with parsing
                return new ConsoleItem(ItemType.Error, $"{name}: {ae.Message}");
            }

            return ConsoleItem.NONE;
        }

        public override string Help()
        {
            return $"{name} {DisplayArguments()}\n\t\t- {description}\n\n";
        }

        private void Call(string input, params string[] args)
        {
            // TODO: Should this parse or should the invoked function parse?
            // Parse the arguments

            // Call function within unpacked tuple
            function.Invoke(input);
        }

        private string DisplayArguments()
        {
            // TODO
            return string.Empty;
        }
    }
}