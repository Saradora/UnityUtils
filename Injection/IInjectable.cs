using UnityEngine;

namespace UnityMDK.Injection;

internal interface IInjectable
{
    public bool CanBeInjected(Component component);
    
    public void Inject(Component component);
}