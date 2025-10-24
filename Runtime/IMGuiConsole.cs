using ImGuiNET;
using System;
using System.Collections.Generic;
using UImGui;
using UnityEngine;

namespace UImGuiConsole
{
    /// <summary>
    /// Generates the console window in IMGui.
    /// </summary>
    public class ImGuiConsole : MonoBehaviour
    {
        private enum ColorPalette : int
        {
            // This four have to match the csys item type enum.

            COMMAND = 0,    //!< Color for command logs
            LOG,            //!< Color for in-command logs
            WARNING,        //!< Color for warnings logs
            ERROR,          //!< Color for error logs
            INFO,            //!< Color for info logs

            TIMESTAMP,      //!< Color for timestamps

            COUNT            //!< For bookkeeping purposes
        };

        [SerializeField] private string consoleName = "ImGui Console";
        [SerializeField] private int inputBufferSize = 256;
        [SerializeField] private float windowAlpha = 0.75f;

        private string inputBuffer;
        private int historyIndex;
        private bool isOpen;
        private HashSet<string> textFilter;
        private bool wasPrevFrameTabCompletion = false; // Flag to determine if previous input was a tab completion
        private List<string> commandSuggestions;
        private ConsoleSystem consoleSystem;

        // Color palettes
        private Vector4[] colorPalettes;

        // Console settings
        private bool autoScroll;
        private bool coloredOutput;
        private bool scrollToBottom;
        private bool filterBar;
        private bool timeStamps;

        private void Awake()
        {
            consoleSystem = new ConsoleSystem();
            commandSuggestions = new List<string>();
            historyIndex = 0;
            colorPalettes = new Vector4[6];
            textFilter = new HashSet<string>();
            inputBuffer = string.Empty;

            RegisterConsoleCommands();
            DefaultSettings();
        }

        private void OnLayout(UImGui.UImGui obj)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, windowAlpha);
            if (!ImGui.Begin(consoleName, ref isOpen, ImGuiWindowFlags.MenuBar))
            {
                ImGui.PopStyleVar();
                ImGui.End();
                return;
            }
            ImGui.PopStyleVar();

            DrawMenuBar();

            if (filterBar)
            {

                ImGui.Separator();
            }

            DrawLogWindow();

            ImGui.Separator();

            // Command line
            DrawInputBar();

            ImGui.End();
        }

        private void OnInitialize(UImGui.UImGui obj)
        {
            // runs after UImGui.OnEnable();
        }

        private void OnDeinitialize(UImGui.UImGui obj)
        {
            // runs after UImGui.OnDisable();
        }

        private void OnEnable()
        {
            UImGuiUtility.Layout += OnLayout;
            UImGuiUtility.OnInitialize += OnInitialize;
            UImGuiUtility.OnDeinitialize += OnDeinitialize;
        }

        private void OnDisable()
        {
            UImGuiUtility.Layout -= OnLayout;
            UImGuiUtility.OnInitialize -= OnInitialize;
            UImGuiUtility.OnDeinitialize -= OnDeinitialize;
        }

        private void DrawLogWindow()
        {
            float footerHeightToReserve = ImGui.GetStyle().ItemSpacing.y + ImGui.GetFrameHeightWithSpacing();
            if (ImGui.BeginChild("ScrollRegion##", new Vector2(0, -footerHeightToReserve)))
            {
                // Display colored command output.
                float timestamp_width = ImGui.CalcTextSize("00:00:00:0000").x;    // Timestamp.
                int count = 0;                                                    // Item count.

                // Wrap items.
                ImGui.PushTextWrapPos();

                // Display items.
                foreach (var item in consoleSystem.Items)
                {
                    // Exit if word is filtered.
                    if (textFilter.Count > 0 && !textFilter.Contains(item.Get()))
                        continue;

                    // Spacing between commands.
                    if (item.type == ItemType.Command)
                    {
                        if (timeStamps) ImGui.PushTextWrapPos(ImGui.GetColumnWidth() - timestamp_width);    // Wrap before timestamps start.
                        if (count++ != 0) ImGui.Dummy(new Vector2(-1, ImGui.GetFontSize()));                // No space for the first command.
                    }

                    // Items.
                    if (coloredOutput)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, colorPalettes[(int)item.type]);
                        ImGui.TextUnformatted(item.Get());
                        ImGui.PopStyleColor();
                    }
                    else
                    {
                        ImGui.TextUnformatted(item.Get());
                    }

                    // Time stamp.
                    if (item.type == ItemType.Command && timeStamps)
                    {
                        // No wrap for timestamps
                        ImGui.PopTextWrapPos();

                        // Right align.
                        ImGui.SameLine(ImGui.GetColumnWidth(-1) - timestamp_width);

                        // Draw time stamp.
                        ImGui.PushStyleColor(ImGuiCol.Text, colorPalettes[(int)ColorPalette.TIMESTAMP]);
                        ImGui.Text(item.timeStamp.ToString("HH:mm:ss:ffff"));
                        ImGui.PopStyleColor();
                    }
                }

                // Stop wrapping since we are done displaying console items.
                ImGui.PopTextWrapPos();

                // Auto-scroll logs.
                if (scrollToBottom && (ImGui.GetScrollY() >= ImGui.GetScrollMaxY() || autoScroll))
                {
                    ImGui.SetScrollHereY(1.0f);
                }
                scrollToBottom = false;

                // Loop through command string vector.
                ImGui.EndChild();
            }
        }

        private unsafe void DrawInputBar()
        {
            // Variables.
            ImGuiInputTextFlags inputTextFlags =
                    ImGuiInputTextFlags.CallbackHistory | ImGuiInputTextFlags.CallbackCharFilter | ImGuiInputTextFlags.CallbackCompletion |
                    ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.CallbackAlways;

            // Only reclaim after enter key is pressed!
            bool reclaimFocus = false;

            // Input widget. (Width an always fixed width)
            ImGui.PushItemWidth(-ImGui.GetStyle().ItemSpacing.x * 7);
            if (ImGui.InputText("Input", ref inputBuffer, (uint)inputBufferSize, inputTextFlags, InputCallback))
            {
                // Validate.
                if (!string.IsNullOrEmpty(inputBuffer))
                {
                    // Run command line input.
                    consoleSystem.RunCommand(inputBuffer);

                    // Scroll to bottom after its ran.
                    scrollToBottom = true;
                }

                // Keep focus.
                reclaimFocus = true;

                // Clear command line.
                inputBuffer = string.Empty;
            }
            ImGui.PopItemWidth();

            // Reset suggestions when client provides char input.
            if (ImGui.IsItemEdited() && !wasPrevFrameTabCompletion)
            {
                commandSuggestions.Clear();
            }
            wasPrevFrameTabCompletion = false;

            // Auto-focus on window apparition
            ImGui.SetItemDefaultFocus();
            if (reclaimFocus)
            {
                ImGui.SetKeyboardFocusHere(-1); // Focus on command line after clearing.
            }
        }

        private void DrawMenuBar()
        {
            if (ImGui.BeginMenuBar())
            {
                // Settings menu.
                if (ImGui.BeginMenu("Settings"))
                {
                    // Colored output
                    ImGui.Checkbox("Colored Output", ref coloredOutput);
                    ImGui.SameLine();
                    HelpMaker("Enable colored command output");

                    // Auto Scroll
                    ImGui.Checkbox("Auto Scroll", ref autoScroll);
                    ImGui.SameLine();
                    HelpMaker("Automatically scroll to bottom of console log");

                    // Filter bar
                    ImGui.Checkbox("Filter Bar", ref filterBar);
                    ImGui.SameLine();
                    HelpMaker("Enable console filter bar");

                    // Time stamp
                    ImGui.Checkbox("Time Stamps", ref timeStamps);
                    ImGui.SameLine();
                    HelpMaker("Display command execution timestamps");

                    // Reset to default settings
                    if (ImGui.Button("Reset settings", new Vector2(ImGui.GetColumnWidth(), 0)))
                        ImGui.OpenPopup("Reset Settings?");

                    // Confirmation
                    bool popupOpen = false;
                    if (ImGui.BeginPopupModal("Reset Settings?", ref popupOpen, ImGuiWindowFlags.AlwaysAutoResize))
                    {
                        ImGui.Text("All settings will be reset to default.\nThis operation cannot be undone!\n\n");
                        ImGui.Separator();

                        if (ImGui.Button("Reset", new Vector2(120, 0)))
                        {
                            DefaultSettings();
                            ImGui.CloseCurrentPopup();
                        }

                        ImGui.SetItemDefaultFocus();
                        ImGui.SameLine();
                        if (ImGui.Button("Cancel", new Vector2(120, 0)))
                        { ImGui.CloseCurrentPopup(); }
                        ImGui.EndPopup();
                    }

                    ImGui.EndMenu();
                }

                // View settings.
                if (ImGui.BeginMenu("Appearance"))
                {
                    // Logging Colors
                    ImGuiColorEditFlags flags =
                            ImGuiColorEditFlags.Float | ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.AlphaBar;

                    ImGui.TextUnformatted("Color Palette");
                    ImGui.Indent();
                    ImGui.ColorEdit4("Command##", ref colorPalettes[(int)ColorPalette.COMMAND], flags);
                    ImGui.ColorEdit4("Log##", ref colorPalettes[(int)ColorPalette.LOG], flags);
                    ImGui.ColorEdit4("Warning##", ref colorPalettes[(int)ColorPalette.WARNING], flags);
                    ImGui.ColorEdit4("Error##", ref colorPalettes[(int)ColorPalette.ERROR], flags);
                    ImGui.ColorEdit4("Info##", ref colorPalettes[(int)ColorPalette.INFO], flags);
                    ImGui.ColorEdit4("Time Stamp##", ref colorPalettes[(int)ColorPalette.TIMESTAMP], flags);
                    ImGui.Unindent();

                    ImGui.Separator();

                    // Window transparency.
                    ImGui.TextUnformatted("Background");
                    ImGui.SliderFloat("Transparency##", ref windowAlpha, 0.1f, 1.0f);

                    ImGui.EndMenu();
                }

                // All scripts.
                if (ImGui.BeginMenu("Scripts"))
                {
                    // Show registered scripts.
                    foreach (var scr_pair in consoleSystem.Scripts)
                    {
                        if (ImGui.MenuItem(scr_pair.Key))
                        {
                            consoleSystem.RunScript(scr_pair.Key);
                            scrollToBottom = true;
                        }
                    }

                    // Reload scripts.
                    ImGui.Separator();
                    if (ImGui.Button("Reload Scripts", new Vector2(ImGui.GetColumnWidth(), 0)))
                    {
                        foreach (var scr_pair in consoleSystem.Scripts)
                        {
                            // TODO: Script reloading
                            //scr_pair.Value->Reload();
                        }
                    }
                    ImGui.EndMenu();
                }

                ImGui.EndMenuBar();
            }
        }

        private void HelpMaker(string desc)
        {
            ImGui.TextDisabled("(?)");
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
                ImGui.TextUnformatted(desc);
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }

        private void DefaultSettings()
        {
            // Settings
            autoScroll = true;
            scrollToBottom = true;
            coloredOutput = true;
            filterBar = true;
            timeStamps = true;

            // Style
            windowAlpha = 1;
            colorPalettes[(int)ColorPalette.COMMAND] = new Vector4(1f, 1f, 1f, 1f);
            colorPalettes[(int)ColorPalette.LOG] = new Vector4(1f, 1f, 1f, 0.5f);
            colorPalettes[(int)ColorPalette.WARNING] = new Vector4(1.0f, 0.87f, 0.37f, 1f);
            colorPalettes[(int)ColorPalette.ERROR] = new Vector4(1f, 0.365f, 0.365f, 1f);
            colorPalettes[(int)ColorPalette.INFO] = new Vector4(0.46f, 0.96f, 0.46f, 1f);
            colorPalettes[(int)ColorPalette.TIMESTAMP] = new Vector4(1f, 1f, 1f, 0.5f);
        }

        private unsafe int InputCallback(ImGuiInputTextCallbackData* data)
        {
            // Exit if no buffer.
            if (data->BufTextLen == 0 && (data->EventFlag != ImGuiInputTextFlags.CallbackHistory))
                return 0;

            var dataPtr = new ImGuiInputTextCallbackDataPtr(data);
            string trim_str = inputBuffer.Trim();

            switch (dataPtr.EventFlag)
            {
                case ImGuiInputTextFlags.CallbackCompletion:
                    {
                        // Check if there's more than one word in the input
                        var trimSplit = trim_str.Split(' ');
                        int startSubtrPos = 0;
                        string inputString;
                        AutoComplete console_autocomplete;

                        // Command line is an entire word/string (No whitespace)
                        // Determine which autocomplete tree to use.
                        if (trimSplit.Length == 1)
                        {
                            inputString = trim_str;
                            console_autocomplete = consoleSystem.CmdAutocomplete;
                        }
                        else
                        {
                            // Store the last word in the input
                            inputString = trimSplit[^1];
                            for (int i = 0; i < trimSplit.Length - 1; i++)
                            {
                                startSubtrPos += trimSplit[i].Length + 1;
                            }

                            console_autocomplete = consoleSystem.VarAutocomplete;
                        }

                        // Validate str
                        if (!string.IsNullOrEmpty(trim_str))
                        {
                            // Display suggestions on console.
                            if (commandSuggestions.Count > 0)
                            {
                                consoleSystem.Log(ItemType.Command, "Suggestions: ");

                                foreach (var suggestion in commandSuggestions)
                                {
                                    consoleSystem.Log(msg: suggestion);
                                }

                                commandSuggestions.Clear();
                            }

                            // Get partial completion and suggestions.
                            string partial = console_autocomplete.PartialSuggestions(inputString, commandSuggestions);

                            // Autocomplete only when one work is available.
                            if (commandSuggestions.Count == 1)
                            {
                                dataPtr.DeleteChars(startSubtrPos, dataPtr.BufTextLen - startSubtrPos);
                                dataPtr.InsertChars(startSubtrPos, commandSuggestions[0]);
                                commandSuggestions.Clear();
                            }
                            else
                            {
                                // Partially complete word.
                                if (!string.IsNullOrEmpty(partial))
                                {
                                    dataPtr.DeleteChars(startSubtrPos, dataPtr.BufTextLen - startSubtrPos);
                                    dataPtr.InsertChars(startSubtrPos, partial);
                                }
                            }
                        }

                        // We have performed the completion event.
                        wasPrevFrameTabCompletion = true;
                    }
                    break;

                case ImGuiInputTextFlags.CallbackHistory:
                    {
                        // Clear buffer
                        dataPtr.DeleteChars(0, dataPtr.BufTextLen);

                        // Init history index
                        if (historyIndex == 0)
                            historyIndex = (int)consoleSystem.History.GetNewIndex();

                        // Traverse history.
                        if (dataPtr.EventKey == ImGuiKey.UpArrow)
                        {
                            if (historyIndex > 0) --historyIndex;
                        }
                        else
                        {
                            if (historyIndex < consoleSystem.History.Size) ++historyIndex;
                        }

                        // Get history.
                        string prevCommand = consoleSystem.History[historyIndex];

                        // Insert commands.
                        dataPtr.InsertChars(dataPtr.CursorPos, prevCommand != null ? prevCommand : string.Empty);
                    }
                    break;

                case ImGuiInputTextFlags.CallbackCharFilter:
                case ImGuiInputTextFlags.CallbackAlways:
                default:
                    break;
            }

            return 0;
        }

        private void RegisterConsoleCommands()
        {
            // TODO: Find custom console commands in all the scripts
        }
    }
}