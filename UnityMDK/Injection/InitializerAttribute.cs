namespace UnityMDK.Injection;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class InitializerAttribute : Attribute
{
    public readonly int Order;
    
    public InitializerAttribute(int order = 0)
    {
        Order = order;
    }
}