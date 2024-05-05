using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityMDK.Injection;

namespace UnityMDK;

public class UnityMDK : MonoBehaviour
{
    public const string ModGuid = "Saradora.UnityMDK";
    public const string ModVersion = "1.3.0";
    public const string ModName = "Unity MDK";
    
    public static UnityMDK Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        SceneInjection.Initialize();
        
        InjectScene(gameObject.scene);
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        InjectScene(scene);
    }

    private void InjectScene(Scene scene)
    {
        StartCoroutine(InjectionRoutine(scene));
    }

    private static IEnumerator InjectionRoutine(Scene scene)
    {
        SceneInjection.InjectScene(scene);
        SceneInjection.ConstructScene(scene, EConstructorEvent.AfterAwake);
        yield return null;
        SceneInjection.ConstructScene(scene, EConstructorEvent.AfterStart);
        yield return null;
        SceneInjection.ConstructScene(scene, EConstructorEvent.AfterFirstUpdate);
    }
}