using UnityEngine;

namespace UnityMDK.Injection;

public interface IPrefabInjector
{
    protected internal void OnInject(GameObject obj);
}