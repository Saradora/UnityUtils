namespace UnityMDK.Injection;

[AttributeUsage(AttributeTargets.Class)]
public class InjectToPrefabAttribute : Attribute
{
    internal string PrefabName { get; }
    
    public InjectToPrefabAttribute(string prefabName)
    {
        PrefabName = prefabName;
    }
}