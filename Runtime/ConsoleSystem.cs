using SickDev.CommandSystem;
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
        private const string ConsoleSettingsReource = "DevConsoleSettings";
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
        public Dictionary<string, Command> Commands { get; private set; }

        /// <summary>
        /// Dictionary of loaded scripts referenced by a string script name key.
        /// </summary>
        public Dictionary<string, Script> Scripts { get; private set; }

        private static Settings settingsCopy;
        private static Settings _settings;
        public static Settings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = Resources.Load<Settings>(ConsoleSettingsReource);
                    if (_settings != null)
                    {
                        // Copy the original settings and use it to reset the file when play mode ends
                        settingsCopy = GameObject.Instantiate(_settings);
                    }
                }
                return _settings;
            }
        }

        private CommandsManager _commandsManager;
        public CommandsManager commandsManager
        {
            get
            {
                if (_commandsManager == null)
                    _commandsManager = CreateCommandsManager();
                return _commandsManager;
            }
        }

        public ConsoleSystem()
        {
            CmdAutocomplete = new AutoComplete();
            VarAutocomplete = new AutoComplete();
            History = new CommandHistory();
            Items = new List<ConsoleItem>();
            Commands = new Dictionary<string, Command>();
            Scripts = new Dictionary<string, Script>();

            _commandsManager = CreateCommandsManager();

            // Register all the loaded Unity functions into the autosuggest tree
            var commands = commandsManager.GetCommands();
            foreach (var c in commands)
            {
                RegisterUnityCommand(c.name);
            }
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
            new UnityCommandsBuilder(commandsManager, this).Build();
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
        /// <param name="cmd"><see cref="Command"/> to put into the map.</param>
        public void RegisterCommand(string command, Command cmd)
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

            CmdAutocomplete.Insert(command);
            VarAutocomplete.Insert(command);

            commandsManager.Add(cmd);
            Commands[command] = cmd;
        }

        /// <summary>
        /// Registers a command within the system to be invokable.
        /// </summary>
        /// <param name="command">Non-whitespace separating name of the command. Whitespace will be dropped.</param>
        /// <param name="function">Function to run when the command is called.</param>
        public void RegisterCommand(string command, Delegate function)
        {
            RegisterCommand(command, new Command(function));
        }

        /// <summary>
        /// Special version for registering Unity commands as it has many overloaded functions, but the command manager
        /// will handle all of it. No command will be stored in the map for the native functions.
        /// </summary>
        /// <remarks>
        /// This will break removing individual commands, but you're probably not removing the native Unity ones.
        /// </remarks>
        /// <param name="command">Non-whitespace separating name of the command. Whitespace will be dropped.</param>
        private void RegisterUnityCommand(string command)
        {
            if (Commands.ContainsKey(command))
            {
                // If another version of the same Unity function is in, continue to the next one
                // as auto complete will work for all overloaded function versions.
                return;
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

            CmdAutocomplete.Insert(command);
            Commands[command] = null;
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

            if (Commands.ContainsKey(command))
            {
                CmdAutocomplete.Remove(command);
                VarAutocomplete.Remove(command);
                commandsManager.Remove(Commands[command]);
                Commands.Remove(command);
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
                object result = commandsManager.Execute(line);

                // Log output.
                ConsoleItem cmd_out = new ConsoleItem(data: $"{result}");
                Items.Add(cmd_out);
            }
        }
    }
}