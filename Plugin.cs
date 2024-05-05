using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityMDK.Injection;
using UnityMDK.Patches;

namespace UnityMDK;

[BepInPlugin(UnityMDK.ModGuid, UnityMDK.ModName, UnityMDK.ModVersion)]
public class PluginInitializer : BaseUnityPlugin
{
    internal static readonly Harmony HarmonyInstance = new(UnityMDK.ModGuid);
        
    private void Awake()
    {
        HarmonyInstance.PatchAll();
        
        UnityEngine_Object_Patching.PatchGenericInstantiate();
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameObject parent = new("Unity Injector (instance)");
        parent.AddComponent<UnityMDK>();
    }
}