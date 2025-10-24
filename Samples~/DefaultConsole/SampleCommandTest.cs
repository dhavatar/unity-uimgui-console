using System;
using UImGuiConsole;
using UnityEngine;

public class SampleCommandTest : MonoBehaviour
{
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
}
