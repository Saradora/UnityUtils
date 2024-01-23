using UnityEngine;

namespace UnityMDK.Injection;

[AttributeUsage(AttributeTargets.Class)]
public class InjectToComponentAttribute : Attribute
{
    internal readonly Type ComponentType;
    internal readonly bool PostAwake;
    
    public InjectToComponentAttribute(Type type, bool postAwake = false)
    {
        if (!typeof(Component).IsAssignableFrom(type))
            throw new ArgumentException($"{type.Name} isn't a Component.");

        PostAwake = postAwake;
        ComponentType = type;
    }
}