using UnityEngine;
using UnityEditor;
using System.Linq;

namespace UImGuiConsole
{
    [CustomEditor(typeof(Settings))]
    public class SettingsEditor : Editor
    {
        GUIContent[] tabsContents = new GUIContent[]
        {
            new GUIContent("Behaviour and Look"),
            new GUIContent("Unity Commands"),
        };
        SerializedProperty selectedTab;

        void OnEnable()
        {
            selectedTab = serializedObject.FindProperty("selectedTab");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawScript();
            DrawTabs();
            serializedObject.ApplyModifiedProperties();
        }

        void DrawScript()
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            GUI.enabled = true;
        }

        void DrawTabs()
        {
            selectedTab.intValue = GUILayout.Toolbar(selectedTab.intValue, tabsContents);
            switch(selectedTab.intValue) {
            case 0:
                DrawBehaviorAndLook();
                break;
            case 1:
                GUI.enabled = !EditorApplication.isPlaying;
                DrawBuiltInCommands();
                GUI.enabled = true;
                break;
            }
        }

        void DrawBehaviorAndLook()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attachedLogLevel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("openKey"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("autoCompleteKey"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("historyKey"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("windowsAlpha"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("inputBufferSize"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("autoScroll"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("coloredOutput"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scrollToBottom"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("showTimeStamp"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("filterBar"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("commandColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("logColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("warningColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("errorColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("infoColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("timestampColor"));
        }

        void DrawBuiltInCommands()
        {
            SerializedProperty currentCommand = serializedObject.FindProperty("builtInCommands");
            SerializedProperty[] commands = new SerializedProperty[currentCommand.Copy().CountRemaining()];
            for(int i = 0; currentCommand.Next(true); i++)
                commands[i] = currentCommand.Copy();
            commands = commands.OrderBy(x => x.name).ToArray();

            int firstColumnCount = commands.Length/2;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            for(int i = 0; i < commands.Length; i++)
            {
                if (i == firstColumnCount+1)
                    EditorGUILayout.BeginVertical();
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(commands[i], true);         
                if (i == firstColumnCount)
                    EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select All"))
            {
                EnableBuiltInCommands(commands, true);
            }

            if (GUILayout.Button("Deselect All"))
            {
                EnableBuiltInCommands(commands, false);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void EnableBuiltInCommands(SerializedProperty[] commands, bool enabled)
        {
            for (int i = 0; i < commands.Length; i++)
            {
                commands[i].boolValue = enabled;
            }
        }
    }
}