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

        public override void Build()
        {
            int commandsBefore = manager.GetCommands().Length;
            
            if(consoleSystem.Settings.builtInCommands.analytics)
                Analytics();
            if(consoleSystem.Settings.builtInCommands.performanceReporting)
                PerformanceReporting();
            if(consoleSystem.Settings.builtInCommands.androidInput)
                AndroidInput();
            if(consoleSystem.Settings.builtInCommands.animator)
                Animator();
            if(consoleSystem.Settings.builtInCommands.appleReplayKit)
                AppleReplayKit();
            if(consoleSystem.Settings.builtInCommands.appleTvRemote)
                AppleTvRemote();
            if(consoleSystem.Settings.builtInCommands.application)
                Application();
            if(consoleSystem.Settings.builtInCommands.audioListener)
                AudioListener();
            if(consoleSystem.Settings.builtInCommands.audioSettings)
                AudioSettings();
            if(consoleSystem.Settings.builtInCommands.audioSource)
                AudioSource();
            if(consoleSystem.Settings.builtInCommands.caching)
                Caching();
            if(consoleSystem.Settings.builtInCommands.camera)
                Camera();
            if(consoleSystem.Settings.builtInCommands.canvas)
                Canvas();
            if(consoleSystem.Settings.builtInCommands.color)
                Color();
            if(consoleSystem.Settings.builtInCommands.color32)
                Color32();
            if(consoleSystem.Settings.builtInCommands.colorUtility)
                ColorUtility();
            if(consoleSystem.Settings.builtInCommands.crashReport)
                CrashReport();
            if(consoleSystem.Settings.builtInCommands.crashReportHandler)
                CrashReportHandler();
            if(consoleSystem.Settings.builtInCommands.cursor)
                Cursor();
            if(consoleSystem.Settings.builtInCommands.debug)
                Debug();
            if(consoleSystem.Settings.builtInCommands.playerConnection)
                PlayerConnection();
            if(consoleSystem.Settings.builtInCommands.display)
                Display();
            if(consoleSystem.Settings.builtInCommands.dynamicGI)
                DynamicGI();
            if(consoleSystem.Settings.builtInCommands.font)
                Font();
            if(consoleSystem.Settings.builtInCommands.gameObject)
                GameObject();
            if(consoleSystem.Settings.builtInCommands.hash128)
                Hash128();
            if(consoleSystem.Settings.builtInCommands.handheld)
                Handheld();
            if(consoleSystem.Settings.builtInCommands.humanTrait)
                HumanTrait();
            if(consoleSystem.Settings.builtInCommands.input)
                Input();
            if(consoleSystem.Settings.builtInCommands.compass)
                Compass();
            if(consoleSystem.Settings.builtInCommands.gyroscope)
                Gyroscope();
            if(consoleSystem.Settings.builtInCommands.locationService)
                LocationService();
            if(consoleSystem.Settings.builtInCommands.iOSDevice)
                IOSDevice();
            if(consoleSystem.Settings.builtInCommands.iOSNotificationServices)
                IOSNotificationServices();
            if(consoleSystem.Settings.builtInCommands.iOSOnDemandResources)
                IOSOnDemandResources();
            if(consoleSystem.Settings.builtInCommands.layerMask)
                LayerMask();
            if(consoleSystem.Settings.builtInCommands.lightmapSettings)
                LightmapSettings();
            if(consoleSystem.Settings.builtInCommands.lightProbeProxyVolume)
                LightProbeProxyVolume();
            if(consoleSystem.Settings.builtInCommands.lODGroup)
                LODGroup();
            if(consoleSystem.Settings.builtInCommands.masterServer)
                MasterServer();
            if(consoleSystem.Settings.builtInCommands.mathf)
                Mathf();
            if(consoleSystem.Settings.builtInCommands.microphone)
                Microphone();
            if(consoleSystem.Settings.builtInCommands.physics)
                Physics();
            if(consoleSystem.Settings.builtInCommands.physics2D)
                Physics2D();
            if(consoleSystem.Settings.builtInCommands.playerPrefs)
                PlayerPrefs();
            if(consoleSystem.Settings.builtInCommands.proceduralMaterial)
                ProceduralMaterial();
            if(consoleSystem.Settings.builtInCommands.profiler)
                Profiler();
            if(consoleSystem.Settings.builtInCommands.qualitySettings)
                QualitySettings();
            if(consoleSystem.Settings.builtInCommands.quaternion)
                Quaternion();
            if(consoleSystem.Settings.builtInCommands.random)
                Random();
            if(consoleSystem.Settings.builtInCommands.rect)
                Rect();
            if(consoleSystem.Settings.builtInCommands.reflectionProbe)
                ReflectionProbe();
            if(consoleSystem.Settings.builtInCommands.remoteSettings)
                RemoteSettings();
            if(consoleSystem.Settings.builtInCommands.graphicsSettings)
                GraphicsSettings();
            if(consoleSystem.Settings.builtInCommands.renderSettings)
                RenderSettings();
            if(consoleSystem.Settings.builtInCommands.samsungTV)
                SamsungTV();
            if(consoleSystem.Settings.builtInCommands.sceneManager)
                SceneManager();
            if(consoleSystem.Settings.builtInCommands.sceneUtility)
                SceneUtility();
            if(consoleSystem.Settings.builtInCommands.screen)
                Screen();
            if(consoleSystem.Settings.builtInCommands.shader)
                Shader();
            if(consoleSystem.Settings.builtInCommands.sortingLayer)
                SortingLayer();
            if(consoleSystem.Settings.builtInCommands.systemInfo)
                SystemInfo();
            if(consoleSystem.Settings.builtInCommands.texture)
                Texture();
            if(consoleSystem.Settings.builtInCommands.time)
                Time();
            if(consoleSystem.Settings.builtInCommands.touchScreenKeyboard)
                TouchScreenKeyboard();
            if(consoleSystem.Settings.builtInCommands.vector2)
                Vector2();
            if(consoleSystem.Settings.builtInCommands.vector3)
                Vector3();
            if(consoleSystem.Settings.builtInCommands.vector4)
                Vector4();
            if(consoleSystem.Settings.builtInCommands.vRInputTracking)
                VRInputTracking();
            if(consoleSystem.Settings.builtInCommands.vRDevice)
                VRDevice();
            if(consoleSystem.Settings.builtInCommands.vRSettings)
                VRSettings();
            
            int commandsAfter = manager.GetCommands().Length;
            consoleSystem.Log(msg: $"Loaded {commandsAfter - commandsBefore} built-in commands");
        }
    }
}