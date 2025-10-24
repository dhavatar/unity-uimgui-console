using System.Reflection;
using UnityEngine;

namespace UImGuiConsole
{
    /// <summary>
    /// Console settings.
    /// </summary>
    public class Settings : ScriptableObject
    {
        [SerializeField] private int selectedTab;

        public LogLevel attachedLogLevel;
        public KeyCode openKey;
        public KeyCode autoCompleteKey;
        public KeyCode historyKey;

        public float windowsAlpha = 0.75f;
        public int inputBufferSize = 256;
        public bool autoScroll = true;
        public bool coloredOutput = true;
        public bool scrollToBottom = true;
        public bool showTimeStamp = true;
        public bool filterBar = true;

        public Color commandColor = new Vector4(1f, 1f, 1f, 1f);
        public Color logColor = new Vector4(1f, 1f, 1f, 0.5f);
        public Color warningColor = new Vector4(1.0f, 0.87f, 0.37f, 1f);
        public Color errorColor = new Vector4(1f, 0.365f, 0.365f, 1f);
        public Color infoColor = new Vector4(0.46f, 0.96f, 0.46f, 1f);
        public Color timestampColor = new Vector4(1f, 1f, 1f, 0.5f);

        public BuiltInCommandsPreferences builtInCommands = new();

        public void CopyFrom(Settings settings)
        {
            FieldInfo[] fields = GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
                fields[i].SetValue(this, fields[i].GetValue(settings));
        }

        [System.Serializable]
        public class BuiltInCommandsPreferences
        {
            public bool analytics = true;
            public bool performanceReporting = true;
            public bool androidInput = true;
            public bool animator = true;
            public bool appleReplayKit = true;
            public bool appleTvRemote = true;
            public bool application = true;
            public bool audioListener = true;
            public bool audioSettings = true;
            public bool audioSource = true;
            public bool caching = true;
            public bool camera = true;
            public bool canvas = true;
            public bool color = true;
            public bool color32 = true;
            public bool colorUtility = true;
            public bool crashReport = true;
            public bool crashReportHandler = true;
            public bool cursor = true;
            public bool debug = true;
            public bool playerConnection = true;
            public bool display = true;
            public bool dynamicGI = true;
            public bool font = true;
            public bool gameObject = true;
            public bool hash128 = true;
            public bool handheld = true;
            public bool humanTrait = true;
            public bool input = true;
            public bool compass = true;
            public bool gyroscope = true;
            public bool locationService = true;
            public bool iOSDevice = true;
            public bool iOSNotificationServices = true;
            public bool iOSOnDemandResources = true;
            public bool layerMask = true;
            public bool lightmapSettings = true;
            public bool lightProbeProxyVolume = true;
            public bool lODGroup = true;
            public bool masterServer = true;
            public bool mathf = true;
            public bool microphone = true;
            public bool physics = true;
            public bool physics2D = true;
            public bool playerPrefs = true;
            public bool proceduralMaterial = true;
            public bool profiler = true;
            public bool qualitySettings = true;
            public bool quaternion = true;
            public bool random = true;
            public bool rect = true;
            public bool reflectionProbe = true;
            public bool remoteSettings = true;
            public bool graphicsSettings = true;
            public bool renderSettings = true;
            public bool samsungTV = true;
            public bool sceneManager = true;
            public bool sceneUtility = true;
            public bool screen = true;
            public bool shader = true;
            public bool sortingLayer = true;
            public bool systemInfo = true;
            public bool texture = true;
            public bool time = true;
            public bool touchScreenKeyboard = true;
            public bool vector2 = true;
            public bool vector3 = true;
            public bool vector4 = true;
            public bool vRInputTracking = true;
            public bool vRDevice = true;
            public bool vRSettings = true;
        }
    }

    [System.Flags]
    public enum LogLevel : byte
    {
        None = 0,
        Log = 1,
        Warning = 2,
        Error = 4,
        Exception = 8,
        Assert = 16
    }
}