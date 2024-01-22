using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityMDK.Injection;
using UnityMDK.Logging;
using Object = UnityEngine.Object;

namespace UnityMDK.Patches;

[HarmonyPatch(typeof(Object))]
internal static class UnityEngine_Object_Patching
{
    [HarmonyPatch("Instantiate", typeof(Object))]
    [HarmonyPrefix]
    private static void Instantiate_Prefix(ref Object original)
    {
        InjectInstance(original);
    }
    
    [HarmonyPatch("Instantiate", typeof(Object), typeof(Transform), typeof(bool))]
    [HarmonyPrefix]
    private static void InstantiateParent_Prefix(ref Object original)
    {
        InjectInstance(original);
    }
    
    [HarmonyPatch("Internal_InstantiateSingle", typeof(Object), typeof(Vector3), typeof(Quaternion))]
    [HarmonyPrefix]
    private static void Internal_InstantiateSingle_Prefix(ref Object data)
    {
        InjectInstance(data);
    }
    
    [HarmonyPatch("Internal_InstantiateSingleWithParent", typeof(Object), typeof(Transform), typeof(Vector3), typeof(Quaternion))]
    [HarmonyPrefix]
    private static void Internal_InstantiateSingleWithParent_Prefix(ref Object data)
    {
        InjectInstance(data);
    }

    // might want to patch the extern method instead when extern are patcheable
    internal static void PatchGenericInstantiate()
    {
        var methods = typeof(Object).GetMethods(BindingFlags.Public | BindingFlags.Static);
        MethodInfo originalMethod = null;

        foreach (var methodInfo in methods)
        {
            Type[] genericArguments = methodInfo.GetGenericArguments();
            if (genericArguments.Length != 1) continue;
            if (methodInfo.Name != "Instantiate") continue;
            if (methodInfo.GetParameters().Length != 1) continue;
            originalMethod = methodInfo;
            break;
        }

        originalMethod = originalMethod.MakeGenericMethod(typeof(Object));
        MethodInfo instantiatePrefix = typeof(UnityEngine_Object_Patching).GetMethod(nameof(GenericInstantiate),
            BindingFlags.Static | BindingFlags.NonPublic);

        Log.Print(originalMethod);
        Log.Print(instantiatePrefix);

        PluginInitializer.HarmonyInstance.Patch(originalMethod, prefix: new HarmonyMethod(instantiatePrefix));
    }

    private static void GenericInstantiate(Object original)
    {
        InjectInstance(original);
    }

    private static void InjectInstance(Object data)
    {
        switch (data)
        {
            case GameObject gameObject:
                SceneInjection.InjectGameObject(gameObject);
                break;
            case Component component:
                SceneInjection.InjectGameObject(component.gameObject);
                break;
        }
    }
}