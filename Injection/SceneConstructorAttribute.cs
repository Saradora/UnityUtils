namespace UnityMDK.Injection;

/// <summary>
/// Calls the method 'private static void SceneConstructor(Scene scene)'
/// in the class each time a scene is load if available.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SceneConstructorAttribute : Attribute
{
    public EConstructorEvent Event { get; }
    
    public SceneConstructorAttribute()
    {
        Event = EConstructorEvent.AfterAwake;
    }

    public SceneConstructorAttribute(EConstructorEvent @event)
    {
        Event = @event;
    }
}