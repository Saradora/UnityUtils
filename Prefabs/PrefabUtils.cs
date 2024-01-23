using UnityEngine;
using UnityMDK.Injection;

namespace UnityMDK;

public static class PrefabUtils
{
    private static GameObject _prefabRoot;

    private static GameObject PrefabRoot
    {
        get
        {
            if (_prefabRoot) return _prefabRoot;

            _prefabRoot = new GameObject("PrefabRoot");
            _prefabRoot.SetActive(false);
            _prefabRoot.hideFlags = HideFlags.HideAndDontSave;
            return _prefabRoot;
        }
    }

    public static GameObject GetEmptyPrefab(string name)
    {
        GameObject newPrefab = new(name)
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        newPrefab.transform.SetParent(PrefabRoot.transform);
        return newPrefab;
    }
}