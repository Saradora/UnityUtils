using System.Collections;
using UnityEngine;

namespace UnityMDK.Injection;

public class SceneConstructor : MonoBehaviour
{
    private int _awakeFrame;
    
    private void Awake()
    {
        _awakeFrame = Time.frameCount;
        
        SceneInjection.Initialize();
        SceneInjection.InjectScene(gameObject.scene);
        SceneInjection.ConstructScene(gameObject.scene, EConstructorEvent.AfterAwake);
    }

    private IEnumerator Start()
    {
        if (_awakeFrame == Time.frameCount) yield return null;
        
        SceneInjection.ConstructScene(gameObject.scene, EConstructorEvent.AfterStart);
        yield return null;
        SceneInjection.ConstructScene(gameObject.scene, EConstructorEvent.AfterFirstUpdate);
    }
}