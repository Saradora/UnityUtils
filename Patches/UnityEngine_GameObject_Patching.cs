using HarmonyLib;
using UnityEngine;
using UnityMDK.Injection;

namespace UnityMDK.Patches;

[HarmonyPatch(typeof(GameObject))]
internal static class UnityEngine_GameObject_Patching
{
    [HarmonyPatch("AddComponent", typeof(Type))]
    [HarmonyPostfix]
    private static void AddComponent_PostFix(ref Component __result)
    {
        if (__result is null) return;
        
        SceneInjection.InjectComponent(__result);
    }
}