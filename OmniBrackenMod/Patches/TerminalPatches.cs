using HarmonyLib;

namespace OmniBrackenMod.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    public class TerminalPatches
    {
        [HarmonyPatch(nameof(Terminal.BeginUsingTerminal))]
        [HarmonyPostfix]
        public static void BeginUsingTerminalPatch(Terminal __instance)
        {
            __instance.terminalNodes.allKeywords[36].word = "omni-man";
            __instance.enemyFiles[1].creatureName = "Omni-Man";
            __instance.enemyFiles[1].displayVideo = Plugin.TerminalVideo;
            __instance.enemyFiles[1].displayText = Plugin.TerminalText.text;
        }
    }
}