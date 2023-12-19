using HarmonyLib;

namespace OmniBrackenMod.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]
    public class EnemyAIPatches
    {
        [HarmonyPatch(nameof(EnemyAI.SwitchToBehaviourStateOnLocalClient))]
        [HarmonyPostfix]
        public static void SwitchToBehaviorStateOnLocalClientPatch(EnemyAI __instance, int stateIndex)
        {
            if (__instance is not FlowermanAI bracken) return;
            
            switch (stateIndex)
            {
                case 0: //Stealth
                    bracken.StopAllCoroutines();
                    bracken.StartCoroutine(FlowermanAIPatches.Stalk(bracken));
                    FlowermanAIPatches.Anger(bracken);
                    break;
                case 1: //Avoid
                    FlowermanAIPatches.Mute(bracken);
                    bracken.StopAllCoroutines();
                    bracken.StartCoroutine(FlowermanAIPatches.Stand(bracken));
                    break;
                case 2: //Anger
                    FlowermanAIPatches.Mute(bracken);
                    bracken.StopAllCoroutines();
                    FlowermanAIPatches.Anger(bracken);
                    break;
            }
        }
    }
}
