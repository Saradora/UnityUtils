using UnityEngine;
using UnityEngine.SceneManagement;
using UnityMDK.Injection;

namespace UnityMDK;

public class UnityMDK : MonoBehaviour
{
    public const string ModGuid = "Saradora.UnityMDK";
    public const string ModVersion = "1.2.0";
    public const string ModName = "Unity MDK";
    
    public static UnityMDK Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        CreateSceneConstructor(gameObject.scene);
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        CreateSceneConstructor(scene);
    }

    private static void CreateSceneConstructor(Scene scene)
    {
        GameObject gameObject = new();
        SceneManager.MoveGameObjectToScene(gameObject, scene);
        gameObject.AddComponent<SceneConstructor>();
    }
}