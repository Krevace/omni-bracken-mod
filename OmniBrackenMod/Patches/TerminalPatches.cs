using System.Linq;
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
            var keyword = __instance.terminalNodes.allKeywords
                .FirstOrDefault(k => k.name == "Bracken");
            if (keyword != null)
            {
                keyword.word = "omni-man";
            }

            var node = __instance.enemyFiles
                .FirstOrDefault(n => n.name == "BrackenFile");
            if (node != null)
            {
                node.creatureName = "Omni-Man";
                node.displayVideo = Plugin.TerminalVideo;
                node.displayText = Plugin.TerminalText.text;
            }
        }
    }
}