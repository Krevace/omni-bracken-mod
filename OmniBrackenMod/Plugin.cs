using System.IO;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Video;

namespace OmniBrackenMod
{
    public static class ConfigSettings
    {
        public static ConfigEntry<float> AudioDistance;
        public static ConfigEntry<float> Speed;
        public static ConfigEntry<float> StandupTime;
        public static ConfigEntry<float> StalkAudioIntervalMin;
        public static ConfigEntry<float> StalkAudioIntervalMax;

        public static void BindConfigSettings()
        {
            AudioDistance = Plugin.Instance.Config.Bind("OmniBrackenMod", "Custom Audio Distance", 30.0f, "Distance Omni-Man's audio travels. (Vanilla is ~20)");
            Speed = Plugin.Instance.Config.Bind("OmniBrackenMod", "Chase Speed", 22.0f, "The speed at which Omni-Man chases you when provoked. (Vanilla is 9)");
            StandupTime = Plugin.Instance.Config.Bind("OmniBrackenMod", "Standup Time", 0.5f, "The time it takes Omni-Man to stand back up when you catch him stalking.");
            StalkAudioIntervalMin = Plugin.Instance.Config.Bind("OmniBrackenMod", "Stalk Audio Interval Min", 5.0f, "The minimum time Omni-Man will wait to let you know he's stalking you.");
            StalkAudioIntervalMax = Plugin.Instance.Config.Bind("OmniBrackenMod", "Stalk Audio Interval Max", 13.0f, "The maximum time Omni-Man will wait to let you know he's stalking you.");
        }
    }
    
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static AudioClip AngerVoice;
        public static AudioClip CaughtVoice;
        public static AudioClip CrackNeckAudio;
        public static AudioClip StunSFX;
        public static GameObject StalkAudio;
        public static GameObject StandPrefab;
        public static GameObject MadPrefab;
        public static VideoClip TerminalVideo;
        public static TextAsset TerminalText;

        public static Plugin Instance;
        private Harmony _harmonyMain;

        private void Awake()
        {
            var bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Info.Location), "omnibracken"));
            AngerVoice = bundle.LoadAsset<AudioClip>("assets/Anger Voice.mp3");
            MadPrefab = bundle.LoadAsset<GameObject>("assets/OmniMadBracken.prefab");
            StandPrefab = bundle.LoadAsset<GameObject>("assets/OmniNormalBracken.prefab");
            TerminalText = bundle.LoadAsset<TextAsset>("assets/OmniManDescription.txt");
            TerminalVideo = bundle.LoadAsset<VideoClip>("assets/Terminal Video.mp4");
            StalkAudio = bundle.LoadAsset<GameObject>("assets/StalkSource.prefab");
            CaughtVoice = bundle.LoadAsset<AudioClip>("assets/VineBoom.mp3");
            StunSFX = bundle.LoadAsset<AudioClip>("assets/Stun.mp3");
            CrackNeckAudio = bundle.LoadAsset<AudioClip>("assets/Jumpscare.mp3");

            Instance = this;
            ConfigSettings.BindConfigSettings();
            _harmonyMain = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmonyMain.PatchAll();
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}