using SickDev.CommandSystem;

namespace UImGuiConsole
{
    /// <summary>
    /// Implementation of the <see cref="SickDev.CommandSystem.Unity.BuiltInCommandsBuilder"/> for adding built-in Unity
    /// functions to the console commands.
    /// </summary>
    public class UnityCommandsBuilder : SickDev.CommandSystem.Unity.BuiltInCommandsBuilder
    {
        private ConsoleSystem consoleSystem;

        public UnityCommandsBuilder(CommandsManager manager, ConsoleSystem consoleSystem) : base(manager)
        {
            this.consoleSystem = consoleSystem;
        }

        public override void Build() {
            int commandsBefore = manager.GetCommands().Length;

            if(ConsoleSystem.Settings.builtInCommands.analytics)
                Analytics();
            if(ConsoleSystem.Settings.builtInCommands.performanceReporting)
                PerformanceReporting();
            if(ConsoleSystem.Settings.builtInCommands.androidInput)
                AndroidInput();
            if(ConsoleSystem.Settings.builtInCommands.animator)
                Animator();
            if(ConsoleSystem.Settings.builtInCommands.appleReplayKit)
                AppleReplayKit();
            if(ConsoleSystem.Settings.builtInCommands.appleTvRemote)
                AppleTvRemote();
            if(ConsoleSystem.Settings.builtInCommands.application)
                Application();
            if(ConsoleSystem.Settings.builtInCommands.audioListener)
                AudioListener();
            if(ConsoleSystem.Settings.builtInCommands.audioSettings)
                AudioSettings();
            if(ConsoleSystem.Settings.builtInCommands.audioSource)
                AudioSource();
            if(ConsoleSystem.Settings.builtInCommands.caching)
                Caching();
            if(ConsoleSystem.Settings.builtInCommands.camera)
                Camera();
            if(ConsoleSystem.Settings.builtInCommands.canvas)
                Canvas();
            if(ConsoleSystem.Settings.builtInCommands.color)
                Color();
            if(ConsoleSystem.Settings.builtInCommands.color32)
                Color32();
            if(ConsoleSystem.Settings.builtInCommands.colorUtility)
                ColorUtility();
            if(ConsoleSystem.Settings.builtInCommands.crashReport)
                CrashReport();
            if(ConsoleSystem.Settings.builtInCommands.crashReportHandler)
                CrashReportHandler();
            if(ConsoleSystem.Settings.builtInCommands.cursor)
                Cursor();
            if(ConsoleSystem.Settings.builtInCommands.debug)
                Debug();
            if(ConsoleSystem.Settings.builtInCommands.playerConnection)
                PlayerConnection();
            if(ConsoleSystem.Settings.builtInCommands.display)
                Display();
            if(ConsoleSystem.Settings.builtInCommands.dynamicGI)
                DynamicGI();
            if(ConsoleSystem.Settings.builtInCommands.font)
                Font();
            if(ConsoleSystem.Settings.builtInCommands.gameObject)
                GameObject();
            if(ConsoleSystem.Settings.builtInCommands.hash128)
                Hash128();
            if(ConsoleSystem.Settings.builtInCommands.handheld)
                Handheld();
            if(ConsoleSystem.Settings.builtInCommands.humanTrait)
                HumanTrait();
            if(ConsoleSystem.Settings.builtInCommands.input)
                Input();
            if(ConsoleSystem.Settings.builtInCommands.compass)
                Compass();
            if(ConsoleSystem.Settings.builtInCommands.gyroscope)
                Gyroscope();
            if(ConsoleSystem.Settings.builtInCommands.locationService)
                LocationService();
            if(ConsoleSystem.Settings.builtInCommands.iOSDevice)
                IOSDevice();
            if(ConsoleSystem.Settings.builtInCommands.iOSNotificationServices)
                IOSNotificationServices();
            if(ConsoleSystem.Settings.builtInCommands.iOSOnDemandResources)
                IOSOnDemandResources();
            if(ConsoleSystem.Settings.builtInCommands.layerMask)
                LayerMask();
            if(ConsoleSystem.Settings.builtInCommands.lightmapSettings)
                LightmapSettings();
            if(ConsoleSystem.Settings.builtInCommands.lightProbeProxyVolume)
                LightProbeProxyVolume();
            if(ConsoleSystem.Settings.builtInCommands.lODGroup)
                LODGroup();
            if(ConsoleSystem.Settings.builtInCommands.masterServer)
                MasterServer();
            if(ConsoleSystem.Settings.builtInCommands.mathf)
                Mathf();
            if(ConsoleSystem.Settings.builtInCommands.microphone)
                Microphone();
            if(ConsoleSystem.Settings.builtInCommands.physics)
                Physics();
            if(ConsoleSystem.Settings.builtInCommands.physics2D)
                Physics2D();
            if(ConsoleSystem.Settings.builtInCommands.playerPrefs)
                PlayerPrefs();
            if(ConsoleSystem.Settings.builtInCommands.proceduralMaterial)
                ProceduralMaterial();
            if(ConsoleSystem.Settings.builtInCommands.profiler)
                Profiler();
            if(ConsoleSystem.Settings.builtInCommands.qualitySettings)
                QualitySettings();
            if(ConsoleSystem.Settings.builtInCommands.quaternion)
                Quaternion();
            if(ConsoleSystem.Settings.builtInCommands.random)
                Random();
            if(ConsoleSystem.Settings.builtInCommands.rect)
                Rect();
            if(ConsoleSystem.Settings.builtInCommands.reflectionProbe)
                ReflectionProbe();
            if(ConsoleSystem.Settings.builtInCommands.remoteSettings)
                RemoteSettings();
            if(ConsoleSystem.Settings.builtInCommands.graphicsSettings)
                GraphicsSettings();
            if(ConsoleSystem.Settings.builtInCommands.renderSettings)
                RenderSettings();
            if(ConsoleSystem.Settings.builtInCommands.samsungTV)
                SamsungTV();
            if(ConsoleSystem.Settings.builtInCommands.sceneManager)
                SceneManager();
            if(ConsoleSystem.Settings.builtInCommands.sceneUtility)
                SceneUtility();
            if(ConsoleSystem.Settings.builtInCommands.screen)
                Screen();
            if(ConsoleSystem.Settings.builtInCommands.shader)
                Shader();
            if(ConsoleSystem.Settings.builtInCommands.sortingLayer)
                SortingLayer();
            if(ConsoleSystem.Settings.builtInCommands.systemInfo)
                SystemInfo();
            if(ConsoleSystem.Settings.builtInCommands.texture)
                Texture();
            if(ConsoleSystem.Settings.builtInCommands.time)
                Time();
            if(ConsoleSystem.Settings.builtInCommands.touchScreenKeyboard)
                TouchScreenKeyboard();
            if(ConsoleSystem.Settings.builtInCommands.vector2)
                Vector2();
            if(ConsoleSystem.Settings.builtInCommands.vector3)
                Vector3();
            if(ConsoleSystem.Settings.builtInCommands.vector4)
                Vector4();
            if(ConsoleSystem.Settings.builtInCommands.vRInputTracking)
                VRInputTracking();
            if(ConsoleSystem.Settings.builtInCommands.vRDevice)
                VRDevice();
            if(ConsoleSystem.Settings.builtInCommands.vRSettings)
                VRSettings();

            int commandsAfter = manager.GetCommands().Length;
            consoleSystem.Log(msg: $"Loaded {commandsAfter - commandsBefore} built-in commands");
        }
    }
}