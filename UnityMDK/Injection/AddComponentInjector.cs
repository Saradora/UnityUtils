using UnityEngine;

namespace UnityMDK.Injection;

public readonly struct AddComponentInjector : IInjectable
{
    private readonly Type _type;

    public AddComponentInjector(Type type)
    {
        if (!typeof(Component).IsAssignableFrom(type))
            throw new ArgumentException($"{type} is not a component.");

        _type = type;
    }

    public bool CanBeInjected(Component component)
    {
        return !component.GetComponent(_type);
    }

    public void Inject(Component component)
    {
        component.gameObject.AddComponent(_type);
    }
}