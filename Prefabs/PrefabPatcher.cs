using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityMDK.Injection;
using UnityMDK.Logging;
using UnityMDK.Patches;

namespace UnityMDK;

[Initializer]
public static class PrefabPatcher
{
    [Initializer]
    private static void Init()
    {
#if DEBUG
        MDKSceneManager.LoadingScene += OnSceneLoading;
        Application.quitting += OnApplicationQuitting;
#endif
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static readonly SortedDictionary<string, string> _prefabs = new();

    private static readonly List<GameObject> _patchedPrefabs = new();

    private static void OnSceneLoading(int scene)
    {
        PatchPrefabs();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        PatchPrefabs();
    }

    private static void PatchPrefabs()
    {
#if !DEBUG
        if (SceneInjection.PrefabsToPatch.Count <= 0)
            return;
#else
        int prefabsLogged = 0;
#endif
        
        var gameObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (var gameObject in gameObjects)
        {
            if (gameObject.transform.parent != null)
                continue;
            
            if (gameObject.scene.IsValid())
                continue;
            
            if (_patchedPrefabs.Contains(gameObject))
                continue;
            
            _patchedPrefabs.Add(gameObject);

            if (SceneInjection.PrefabsToPatch.TryGetValue(gameObject.name, out var list))
            {
                foreach (var injector in list)
                {
                    injector.OnInject(gameObject);
                }

                SceneInjection.PrefabsToPatch.Remove(gameObject.name);
            }

#if DEBUG
            LogPrefab(gameObject, ref prefabsLogged);
#endif
        }

#if DEBUG
        if (prefabsLogged > 0)
        {
            Log.Warning($"Logged {prefabsLogged} prefabs!");
        }
#endif
    }

    private static void LogPrefab(GameObject gameObject, ref int prefabsLogged)
    {
        var prefabName = gameObject.name;
        var pascalName = "P" + prefabName.ToPascalCase();
        if (_prefabs.TryGetValue(pascalName, out string prefab))
        {
            if (prefab != prefabName)
            {
                Log.Error($"Couldn't log prefab {prefabName} because its Pascal variant {prefab} already exists...");
            }
            return;
        }
            
        _prefabs.Add(pascalName, prefabName);
        prefabsLogged++;
    }

    private static void OnApplicationQuitting()
    {
        if (_prefabs.Count <= 0)
            return;
        
        string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (string.IsNullOrEmpty(folderPath))
        {
            Log.Error($"Couldn't find path for assembly");
            return;
        }

        string filePath = folderPath + "/PrefabList.cs";

        using FileStream stream = File.Open(filePath, FileMode.Create, FileAccess.Write);
        using StreamWriter writer = new(stream);

        StringBuilder builder = new();
        builder.AppendLine("// ----- AUTO-GENERATED CODE ----- //");
        builder.AppendLine("");
        builder.AppendLine("public class PrefabList");
        builder.AppendLine("{");
        foreach ((string pascalName, string prefabName) in _prefabs)
        {
            builder.AppendLine($"\tpublic const string {pascalName} = \"{prefabName}\";");
        }
        builder.AppendLine("}");

        writer.Write(builder.ToString());
    }
}