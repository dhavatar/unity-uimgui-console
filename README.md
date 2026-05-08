# Unity ImGui Console

**Requires [Dear ImGui for Unity](https://github.com/dhavatar/uimgui) for this to work, either with my fork or the original [psydack ImGui](https://github.com/psydack/uimgui)**

This is an adaptation of [rmxbalanque's imgui-console](https://github.com/rmxbalanque/imgui-console") to work with Dear ImGui for Unity and integrating [CommandSystem](https://github.com/Cobo3/CommandSystem) from Cobo3 to get native Unity functions to work in the console as well as easier use to add custom debug functions to the console.

## Usage

Add the **IMGuiConsole** script to a GameObject as well as **UImGui** from the Dear ImGui for Unity. The **IMGuiConsole** component requires a console key input that will be used to show and hide the console.

Sample code to demonstrate how to add custom function calls to the console.
```C#
private void Start()
{
    // These use a basic System Action/Func declarations and converts it internally
    ImGuiConsole.RegisterCommand("new-action", new Action(DebugSimpleAction));
    ImGuiConsole.RegisterCommand("number-test", new Func<float, float, string>(DebugNumberTest));
}

private void DebugSimpleAction()
{
    Debug.Log("This doesn't do anything but print.");
}

private string DebugNumberTest(float a, float b)
{
    return $"{a} + {b} is {a + b}";
}
```

If the function needs to output to the console, it must return a string that the console will display. Otherwise, the function will execute with no message.