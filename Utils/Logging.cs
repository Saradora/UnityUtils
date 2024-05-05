using System.Diagnostics;
using System.Reflection;
using BepInEx.Logging;
using Logger = BepInEx.Logging.Logger;

namespace UnityMDK.Logging;

public static class Log
{
    private static readonly Dictionary<Assembly, ManualLogSource> LOGSources = new();
    
    public static void Print(object message)
    {
        GetLogger(Assembly.GetCallingAssembly()).LogInfo(message ?? "Null");
    }

    public static void Warning(object message)
    {
        GetLogger(Assembly.GetCallingAssembly()).LogWarning(message ?? "Null");
    }

    public static void Error(object message)
    {
        GetLogger(Assembly.GetCallingAssembly()).LogError(message ?? "Null");
    }

    public static void Exception(Exception exception)
    {
        GetLogger(Assembly.GetCallingAssembly()).LogError(exception?.Message ?? "Null");
    }

    [Conditional("DEBUG")]
    public static void DebugPrint(object message)
    {
        Print(message);
    }

    [Conditional("DEBUG")]
    public static void DebugWarning(object message)
    {
        Warning(message);
    }

    [Conditional("DEBUG")]
    public static void DebugError(object message)
    {
        Error(message);
    }

    [Conditional("DEBUG")]
    public static void DebugException(Exception exception)
    {
        Exception(exception);
    }

    private static ManualLogSource GetLogger(Assembly assembly)
    {
        if (LOGSources.TryGetValue(assembly, out ManualLogSource logger)) return logger;
        
        LOGSources[assembly] = Logger.CreateLogSource(assembly.GetName().Name);
        return LOGSources[assembly];
    }
}