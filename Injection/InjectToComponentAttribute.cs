using UnityEngine;

namespace UnityMDK.Injection;

[AttributeUsage(AttributeTargets.Class)]
public class InjectToComponentAttribute : Attribute
{
    internal readonly Type ComponentType; 
    
    public InjectToComponentAttribute(Type type)
    {
        if (!typeof(Component).IsAssignableFrom(type))
            throw new ArgumentException($"{type.Name} isn't a Component.");
                
        ComponentType = type;
    }
}