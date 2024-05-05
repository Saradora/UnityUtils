using HarmonyLib;
using UnityEngine.SceneManagement;
using UnityMDK.Logging;

namespace UnityMDK.Patches;

public static class MDKSceneManager
{
    public static event Action<int> LoadingScene;

    internal static void InvokeLoadingScene(int scene)
    {
        LoadingScene?.Invoke(scene);  
    } 
}

[HarmonyPatch(typeof(SceneManager))]
public static class SceneManager_Patching
{
    [HarmonyPrefix]
    //[HarmonyPatch(nameof(SceneManager.LoadScene), typeof(string))]
    [HarmonyPatch(nameof(SceneManager.LoadScene), typeof(string), typeof(LoadSceneMode))]
    [HarmonyPatch(nameof(SceneManager.LoadScene), typeof(string), typeof(LoadSceneParameters))]
    //[HarmonyPatch(nameof(SceneManager.LoadSceneAsync), typeof(string))]
    [HarmonyPatch(nameof(SceneManager.LoadSceneAsync), typeof(string), typeof(LoadSceneMode))]
    [HarmonyPatch(nameof(SceneManager.LoadSceneAsync), typeof(string), typeof(LoadSceneParameters))]
    private static void LoadScene_String_Prefix(string sceneName)
    {
        int buildIndex = SceneUtility.GetBuildIndexByScenePath(sceneName);
        LoadScene_Int_Prefix(buildIndex);
    }
    
    [HarmonyPrefix]
    //[HarmonyPatch(nameof(SceneManager.LoadScene), typeof(int))]
    [HarmonyPatch(nameof(SceneManager.LoadScene), typeof(int), typeof(LoadSceneMode))]
    [HarmonyPatch(nameof(SceneManager.LoadScene), typeof(int), typeof(LoadSceneParameters))]
    //[HarmonyPatch(nameof(SceneManager.LoadSceneAsync), typeof(int))]
    [HarmonyPatch(nameof(SceneManager.LoadSceneAsync), typeof(int), typeof(LoadSceneMode))]
    [HarmonyPatch(nameof(SceneManager.LoadSceneAsync), typeof(int), typeof(LoadSceneParameters))]
    private static void LoadScene_Int_Prefix(int sceneBuildIndex)
    {
        if (sceneBuildIndex < 0 || sceneBuildIndex >= SceneManager.sceneCountInBuildSettings)
            return;
        MDKSceneManager.InvokeLoadingScene(sceneBuildIndex);
    }
}