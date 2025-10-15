using SickDev.CommandSystem;
using SickDev.CommandSystem.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UImGuiConsole
{
    /// <summary>
    /// Main console driver for registering and executing commands from the console.
    /// </summary>
    public class ConsoleSystem
    {
        private const string HelpCommand = "help";
        private const string SetCommand = "set";
        private const string GetCommand = "get";
        private const string ErrorNoVar = "No variable provided";
        private const string ErrorSetGetNotFound = "Command doesn't exist and/or variable is not registered";

        /// <summary>
        /// Autocomplete tree for commands.
        /// </summary>
        public AutoComplete CmdAutocomplete { get; private set; }

        /// <summary>
        /// Autocomplete tree for variables.
        /// </summary>
        public AutoComplete VarAutocomplete { get; private set; }

        /// <summary>
        /// Command history from the typed input.
        /// </summary>
        public CommandHistory History { get; private set; }

        /// <summary>
        /// Console items to display in the window.
        /// </summary>
        public List<ConsoleItem> Items { get; private set; }

        /// <summary>
        /// Dictionary of loaded commands referenced by a string command key.
        /// </summary>
        public Dictionary<string, CommandBase> Commands { get; private set; }

        /// <summary>
        /// Dictionary of loaded scripts referenced by a string script name key.
        /// </summary>
        public Dictionary<string, Script> Scripts { get; private set; }

        private CommandsManager _commandsManager;
        public CommandsManager CommandsManager
        {
            get
            {
                if (_commandsManager == null)
                    _commandsManager = CreateCommandsManager();
                return _commandsManager;
            }
        }

        // Flag that determines if commands will be registered for autocomplete.
        private bool registerCommandSuggestion = true;

        public ConsoleSystem()
        {
            CmdAutocomplete = new AutoComplete();
            VarAutocomplete = new AutoComplete();
            History = new CommandHistory();
            Items = new List<ConsoleItem>();
            Commands = new Dictionary<string, CommandBase>();
            Scripts = new Dictionary<string, Script>();

            RegisterCommand(HelpCommand, "Display commands information", (string _) =>
            {
                // Custom command information display
                Log(msg: $"{HelpCommand} [command_name:String] (Optional)\n\t\t- Display command(s) information\n");
                Log(msg: $"{SetCommand} [variable_name:String] [data]\n\t\t- Assign data to given variable\n");
                Log(msg: $"{GetCommand} [variable_name:String]\n\t\t- Display data of given variable\n");

                // Print the other commands
                foreach (var c in Commands)
                {
                // Filter set and get
                if (c.Key[..3] == SetCommand || c.Key[..3] == GetCommand)
                        continue;

                // Skip help command
                if (c.Key[..4] == HelpCommand)
                        continue;

                    Log(msg: c.Value.Help());
                }
            });

            // Register pre-defined get/set commands
            CmdAutocomplete.Insert(SetCommand);
            CmdAutocomplete.Insert(GetCommand);
        }

        public void Log(ItemType type = ItemType.Log, string msg = "")
        {
            Items.Add(new ConsoleItem(type, msg));
        }

        CommandsManager CreateCommandsManager()
        {
            //CommandsManager.onExceptionThrown += OnCommandSystemExceptionThrown;
            //CommandsManager.onMessage += OnCommandSystemMessage;
            Configuration configuration = new Configuration(
                Application.platform != RuntimePlatform.WebGLPlayer,
                "Assembly-CSharp-firstpass",
                "Assembly-CSharp"
            );
            CommandsManager commandsManager = new CommandsManager(configuration);
            commandsManager.LoadCommands();
            //new SickDev.DevConsole.BuiltInCommandsBuilder(commandsManager).Build();
            return commandsManager;
        }

        public void RunCommand(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return;
            }

            Log(msg: line);
            ParseCommandLine(line);
        }

        public void RunScript(string scriptName)
        {
            // Attempt to find script
            if (!Scripts.TryGetValue(scriptName, out Script value))
            {
                Log(ItemType.Error, $"Script \"{scriptName}\" not found");
                return;
            }

            // About to run script
            Log(ItemType.Info, $"Run \"{scriptName}\"");

            // Load if script is empty
            if (value.Data.Count == 0)
            {
                try
                {
                    value.Load();
                }
                catch (Exception e)
                {
                    Log(ItemType.Error, e.Message);
                }
            }

            // Run script
            foreach (var cmd in value.Data)
            {
                RunCommand(cmd);
            }
        }

        /// <summary>
        /// Registers a command within the system to be invokable.
        /// </summary>
        /// <param name="command">Non-whitespace separating name of the command. Whitespace will be dropped.</param>
        /// <param name="description">Describes what the command does.</param>
        /// <param name="function">Function to run when the command is called.</param>
        /// <param name="args"></param>
        public void RegisterCommand(string command, string description, Action<string> function, params string[] args)
        {
            if (Commands.ContainsKey(command))
            {
                throw new Exception("ERROR: Command already exists");
            }
            else if (string.IsNullOrEmpty(command))
            {
                Log(ItemType.Error, "Empty command name given.");
                return;
            }

            var splitCommand = command.Split(' ');
            if (splitCommand.Length > 1)
            {
                throw new Exception("ERROR: Whitespace separated command names are forbidden.");
            }

            if (registerCommandSuggestion)
            {
                CmdAutocomplete.Insert(command);
                VarAutocomplete.Insert(command);
            }

            Commands[command] = new Command(command, description, function, args);

            // Make help command for command just added
            Action<string> help = (string _) =>
            {
                Log(msg: Commands[command].Help());
            };

            Commands[$"{HelpCommand} {command}"] = new Command($"{HelpCommand} {command}", $"Displays help info about command {command}", help);
        }

        public void RegisterVariable<T, U>(string name, T variable, params U[] args)
        {
            var varName = RegisterVariableAux<T>(name, variable);
            // TODO
        }

        public void RegisterVariable<T, U>(string name, T variable, Action<T, U> setter)
        {
            // TODO
        }

        /// <summary>
        /// Registers script into the console system.
        /// </summary>
        /// <param name="name">Script name to use as the key.</param>
        /// <param name="path">File path to the script to load.</param>
        public void RegisterScript(string name, string path)
        {
            if (!Scripts.ContainsKey(name))
            {
                Scripts[name] = new Script(path, true);
                VarAutocomplete.Insert(name);
            }
            else
            {
                throw new Exception($"ERROR: Script {name} already registered");
            }
        }

        /// <summary>
        /// Removes a command from the console system.
        /// </summary>
        /// <param name="command">Command to remove.</param>
        public void UnregisterCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                return;
            }

            string helpCommand = $"{HelpCommand} {command}";

            if (Commands.ContainsKey(command) && Commands.ContainsKey(helpCommand))
            {
                CmdAutocomplete.Remove(command);
                VarAutocomplete.Remove(command);

                Commands.Remove(command);
                Commands.Remove(helpCommand);
            }
        }

        public void UnregisterVariable(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            string setCommand = $"{SetCommand} {name}";
            string getCommand = $"{GetCommand} {name}";

            if (Commands.ContainsKey(setCommand) && Commands.ContainsKey(getCommand))
            {
                VarAutocomplete.Remove(name);
                Commands.Remove(setCommand);
                Commands.Remove(getCommand);
            }
        }

        /// <summary>
        /// Removes a script from the console system.
        /// </summary>
        /// <param name="name">Key name script to unregister.</param>
        public void UnregisterScript(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (Scripts.ContainsKey(name))
            {
                VarAutocomplete.Remove(name);
                Scripts.Remove(name);
            }
        }

        private string RegisterVariableAux<T>(string name, T variable)
        {
            registerCommandSuggestion = false;

            var splitName = name.Split(' ');
            if (splitName.Length > 1)
            {
                throw new Exception("ERROR: Whitespace separated variable names are forbidden.");
            }

            string trimmedName = name.Trim();

            // TODO
            // Register get command
            Commands[$"{GetCommand} {trimmedName}"] = new Command($"{GetCommand} {trimmedName}", $"Gets the variable {trimmedName}", null);

            registerCommandSuggestion = true;

            VarAutocomplete.Insert(trimmedName);

            return trimmedName;
        }

        private void ParseCommandLine(string line)
        {
            // Just whitespace was passed in. Don't log as command.
            if (string.IsNullOrWhiteSpace(line))
                return;

            // Split the command based on the whitespace
            var lineSplit = line.Split(' ');

            // Push to history.
            History.PushBack(line);

            // Get name of command.
            string command_name = lineSplit[0];

            // Set or get
            bool is_cmd_set = command_name == SetCommand;
            bool is_cmd_get = command_name == GetCommand;
            bool is_cmd_help = !(is_cmd_set || is_cmd_get) ? command_name == HelpCommand : false;

            // Edge case for if user is just runs "help" command
            if (is_cmd_help)
            {
                command_name += " " + string.Join(' ', lineSplit[1..]);
            }

            // Its a set or get command
            else if (is_cmd_set || is_cmd_get)
            {
                // Try to get variable name
                if (lineSplit.Length == 1)
                {
                    Log(ItemType.Error, ErrorNoVar);
                    return;
                }
                else
                {
                    // Append variable name.
                    command_name += " " + string.Join(' ', lineSplit[1..]);
                }
            }

            command_name = command_name.Trim();

            // Get runnable command
            if (!Commands.ContainsKey(command_name))
            {
                Log(ItemType.Error, ErrorSetGetNotFound);
            }
            // Run the command
            else
            {
                // Get the arguments.
                string arguments = string.Join(' ', lineSplit[1..]);

                // Execute command.
                ConsoleItem cmd_out = Commands[command_name].Invoke(arguments);

                // Log output.
                if (cmd_out.type != ItemType.None)
                {
                    Items.Add(cmd_out);
                }
            }
        }
    }
}