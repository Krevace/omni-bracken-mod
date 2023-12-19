using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace OmniBrackenMod.Patches
{
    [HarmonyPatch(typeof(FlowermanAI))]
    public class FlowermanAIPatches
    {
        private class FlowermanExtras
        {
            public GameObject Model;
            public AudioSource StalkAudioSource;

            public FlowermanExtras(GameObject newModel, AudioSource newStalkAudioSource)
            {
                Model = newModel;
                StalkAudioSource = newStalkAudioSource;
            }
        }
        
        private static Dictionary<FlowermanAI, FlowermanExtras> _omniManMap = new();

        [HarmonyPatch(nameof(FlowermanAI.Start))]
        [HarmonyPostfix]
        public static void StartPatch(FlowermanAI __instance)
        {
            _omniManMap.Add(__instance,
                new FlowermanExtras(null,
                    Object.Instantiate(Plugin.StalkAudio, __instance.gameObject.transform)
                        .GetComponent<AudioSource>()));
            
            __instance.creatureAngerVoice.clip = Plugin.AngerVoice;
            __instance.crackNeckSFX = Plugin.CrackNeckAudio;
            __instance.crackNeckAudio.clip = Plugin.CrackNeckAudio;
            __instance.enemyType.stunSFX = Plugin.StunSFX;
            __instance.enemyBehaviourStates[1].VoiceClip = Plugin.CaughtVoice;
            
            __instance.creatureAngerVoice.maxDistance = ConfigSettings.AudioDistance.Value;
            __instance.crackNeckAudio.maxDistance = ConfigSettings.AudioDistance.Value;
            __instance.creatureSFX.maxDistance = ConfigSettings.AudioDistance.Value;
            __instance.creatureVoice.maxDistance = ConfigSettings.AudioDistance.Value;
            
            __instance.gameObject.transform.Find("ScanNode").GetComponent<ScanNodeProperties>().headerText = "Omni-Man";
            
            Object.Destroy(__instance.gameObject.transform.Find("FlowermanModel").Find("LOD1").gameObject
                .GetComponent<SkinnedMeshRenderer>());
            Object.Destroy(__instance.gameObject.transform.Find("FlowermanModel").Find("AnimContainer").Find("metarig")
                .gameObject);
            
            __instance.EnterAngerModeServerRpc(0.0f); //Gets rid of lag spike early on    
            __instance.EnterAngerModeClientRpc(0.0f);
            
            __instance.StartCoroutine(Stand(__instance));
        }
        
        [HarmonyPatch(nameof(FlowermanAI.EnterAngerModeClientRpc))]
        [HarmonyPostfix]
        public static void EnterAngerModeClientRPCPatch(FlowermanAI __instance)
        {
            __instance.agent.speed = ConfigSettings.Speed.Value;
        }
        
        [HarmonyPatch(nameof(FlowermanAI.KillEnemy))]
        [HarmonyPostfix]
        public static void KillEnemyPatch(FlowermanAI __instance)
        {
            Mute(__instance);
            __instance.StopAllCoroutines();
            
            _omniManMap[__instance].Model.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
            _omniManMap[__instance].Model.transform.localPosition += new Vector3(0.0f, -1.45f, 0.0f);
        }
        
        [HarmonyPatch(nameof(FlowermanAI.KillPlayerAnimationClientRpc))]
        [HarmonyPostfix]
        public static void KillPlayerAnimationClientRpcPatch(FlowermanAI __instance, int playerObjectId)
        {
            StartOfRound.Instance.allPlayerScripts[playerObjectId].gameplayCamera.transform
                .LookAt(__instance.transform);
        }

        public static void Anger(FlowermanAI __instance)
        {
            var newOmniMan = Object.Instantiate(Plugin.MadPrefab, __instance.gameObject.transform);
            newOmniMan.transform.localPosition = new Vector3(0f, 1.5f, 0f);

            if (_omniManMap[__instance].Model)
            {
                Object.Destroy(_omniManMap[__instance].Model);
            }

            _omniManMap[__instance].Model = newOmniMan;
        }
        
        public static IEnumerator Stand(FlowermanAI __instance)
        {
            yield return new WaitForSeconds(ConfigSettings.StandupTime.Value);
            
            var newOmniMan = Object.Instantiate(Plugin.StandPrefab, __instance.gameObject.transform);
            newOmniMan.transform.localPosition = new Vector3(0f, 1.5f, 0f);
            
            if (_omniManMap[__instance].Model)
            {
                Object.Destroy(_omniManMap[__instance].Model);
            }

            _omniManMap[__instance].Model = newOmniMan;
        }

        public static IEnumerator Stalk(FlowermanAI __instance)
        {
            var stalkAudio = _omniManMap[__instance].StalkAudioSource;
            
            while (true)
            {
                stalkAudio.Play();
                yield return new WaitForSeconds(stalkAudio.clip.length + (float) new System.Random().NextDouble() *
                    (ConfigSettings.StalkAudioIntervalMax.Value - ConfigSettings.StalkAudioIntervalMin.Value) +
                    ConfigSettings.StalkAudioIntervalMin.Value);
            }
        }

        public static void Mute(FlowermanAI __instance)
        {
            _omniManMap[__instance].StalkAudioSource.Stop();
        }
    }
}
