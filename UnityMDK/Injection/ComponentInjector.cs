using UnityEngine;

namespace UnityMDK.Injection;

public abstract class ComponentInjector : IInjectable
{
    public abstract bool CanBeInjected(Component component);

    public abstract void Inject(Component component);
}

public abstract class ComponentInjector<TComponent> : IInjectable where TComponent : Component
{
    public bool CanBeInjected(Component component)
    {
        return CanBeInjected((TComponent)component);
    }

    public void Inject(Component component)
    {
        Inject((TComponent)component);
    }

    protected abstract bool CanBeInjected(TComponent component);

    protected abstract void Inject(TComponent component);
}