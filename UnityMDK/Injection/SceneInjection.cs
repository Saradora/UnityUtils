using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityMDK.Logging;
using Object = UnityEngine.Object;

namespace UnityMDK.Injection;

public static class SceneInjection
{
    private static readonly SortedList<int, List<MethodInfo>> Initializers = new();
    private static readonly Dictionary<EConstructorEvent, List<MethodInfo>> SceneConstructors = new();
    private static readonly Dictionary<Type, List<IInjectable>> ObjectInjectors = new();
    private static readonly List<GameObject> ConstructedPrefabs = new();
    private static readonly List<int> ConstructedScenes = new();
    
    private static bool _initialized;
        
    private static readonly Type ComponentType = typeof(Component);
    private static readonly Type InjectAttributeType = typeof(InjectToComponentAttribute);

    private static Scene _dontDestroyOnLoadScene;

    public static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;

        // find DontDestroyOnLoadScene
        GameObject ddolObject = new("Dummy"); // Todo fix when patching externs is possible (by patching Object.DontDestroyOnLoad)
        Object.DontDestroyOnLoad(ddolObject);
        _dontDestroyOnLoadScene = ddolObject.scene;
        Object.Destroy(ddolObject);
        
        foreach (EConstructorEvent constructorEvent in Enum.GetValues(typeof(EConstructorEvent)))
        {
            SceneConstructors.Add(constructorEvent, new List<MethodInfo>());
        }

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                RegisterInjector(type);
                RegisterSceneConstructor(type);
                RegisterInitializer(type);
            }
        }
        
        RunInitializers();

        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    public static void AddComponentInjector<TComponent>(ComponentInjector injector) where TComponent : Component
    {
        Type type = typeof(TComponent);
        if (type == ComponentType)
        {
            Log.Error($"Specified component type is too vague");
            return;
        }
        
        if (!ObjectInjectors.ContainsKey(type)) ObjectInjectors.Add(type, new List<IInjectable>());
        ObjectInjectors[type].Add(injector);
    }

    public static void AddComponentInjector<TComponent>(ComponentInjector<TComponent> injector) where TComponent : Component
    {
        Type type = typeof(TComponent);
        if (type == ComponentType)
        {
            Log.Error($"Specified component type is too vague");
            return;
        }
        
        if (!ObjectInjectors.ContainsKey(type)) ObjectInjectors.Add(type, new List<IInjectable>());
        ObjectInjectors[type].Add(injector);
    }

    private static void RegisterInjector(Type type)
    {
        if (!type.IsDefined(InjectAttributeType, false)) return;
        if (!ComponentType.IsAssignableFrom(type)) return;

        InjectToComponentAttribute injectAttribute = (InjectToComponentAttribute)type.GetCustomAttribute(InjectAttributeType);
        if (injectAttribute.ComponentType is null) return;
                
        if (!ObjectInjectors.ContainsKey(injectAttribute.ComponentType)) ObjectInjectors.Add(injectAttribute.ComponentType, new List<IInjectable>());

        Log.Warning($"Constructable: Adding {type} to {injectAttribute.ComponentType}");
        ObjectInjectors[injectAttribute.ComponentType].Add(new AddComponentInjector(type));
    }

    private static void RegisterSceneConstructor(Type type)
    {
        SceneConstructorAttribute sceneConstructorAttribute = (SceneConstructorAttribute)type.GetCustomAttribute(typeof(SceneConstructorAttribute));
                
        if (sceneConstructorAttribute is null) return;

        MethodInfo method = type.GetMethod("SceneConstructor", BindingFlags.Static | BindingFlags.NonPublic);
        if (method is null) return;

        var parameters = method.GetParameters();
        if (parameters.Length != 1 || parameters[0].ParameterType != typeof(Scene)) return;
                
        Log.Warning($"Constructable: Adding {type} to scene constructors");
        SceneConstructors[sceneConstructorAttribute.Event].Add(method);
    }

    private static void RegisterInitializer(Type type)
    {
        InitializerAttribute initializerAttribute = type.GetCustomAttribute<InitializerAttribute>();
        if (initializerAttribute is null) return;

        SortedList<int, List<MethodInfo>> methods = new();

        foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
        {
            InitializerAttribute methodAttribute = method.GetCustomAttribute<InitializerAttribute>();
            if (methodAttribute is null) continue;
            if (method.GetParameters().Length > 0) continue;
            
            if (!methods.ContainsKey(methodAttribute.Order)) methods.Add(methodAttribute.Order, new List<MethodInfo>());
            methods[methodAttribute.Order].Add(method);
        }
        
        if (!Initializers.ContainsKey(initializerAttribute.Order)) Initializers.Add(initializerAttribute.Order, new List<MethodInfo>());

        foreach (var methodList in methods)
        {
            Initializers[initializerAttribute.Order].AddRange(methodList.Value);
        }
    }

    private static void RunInitializers()
    {
        object[] parameters = Array.Empty<object>();
        foreach (var initializer in Initializers)
        {
            foreach (var methodInfo in initializer.Value)
            {
                methodInfo.Invoke(null, parameters);
            }
        }
    }

    private static void OnSceneUnloaded(Scene scene)
    {
        ConstructedScenes.Remove(scene.handle);
    }
    
    internal static void InjectScene(Scene scene)
    {
        if (ConstructedScenes.Contains(scene.handle)) return;
        
        Log.Print($"Injecting scene: {scene.name}");

        foreach (var rootGameObject in scene.GetRootGameObjects())
        {
            InjectGameObject(rootGameObject);
        }
        
        ConstructedScenes.Add(scene.handle);
    }

    internal static void ConstructScene(Scene scene, EConstructorEvent eventType)
    {
        object[] constructorParam = { scene };

        Log.Print($"Constructing scene {scene.name} [{eventType.ToString()}]");

        if (!SceneConstructors.TryGetValue(eventType, out List<MethodInfo> methods)) return;
        foreach (var method in methods)
        {
            method.Invoke(null, constructorParam);
        }

        InjectDontDestroyOnLoadScene(); // todo fix when patching externs is possible (to patch Object.DontDestroyOnLoad)
    }

    private static void InjectDontDestroyOnLoadScene()
    {
        if (!_dontDestroyOnLoadScene.IsValid()) return;
        
        foreach (var rootGameObject in _dontDestroyOnLoadScene.GetRootGameObjects())
        {
            InjectGameObject(rootGameObject);
        }
    }

    internal static void InjectGameObject(GameObject gameObject)
    {
        if (gameObject.scene.IsValid())
        {
            if (ConstructedScenes.Contains(gameObject.scene.handle)) return;
            DoInjectGameObject(gameObject);
            return;
        }

        if (ConstructedPrefabs.Contains(gameObject)) return;
        DoInjectGameObject(gameObject);
        ConstructedPrefabs.Add(gameObject);
    }
    
    private static List<Component> _constructedComponents = new();

    private static void DoInjectGameObject(GameObject gameObject)
    {
        _constructedComponents.Clear();
        foreach (var page in ObjectInjectors)
        {
            var components = gameObject.GetComponentsInChildren(page.Key, true);

            foreach (var component in components)
            {
                if (_constructedComponents.Contains(component)) continue;
                foreach (var constructor in page.Value)
                {
                    if (!constructor.CanBeInjected(component)) continue;
                    constructor.Inject(component);
                }
                _constructedComponents.Add(component);
            }
        }
    }

    internal static void InjectComponent(Component component)
    {
        Type componentType = component.GetType();

        foreach (var page in ObjectInjectors)
        {
            if (!page.Key.IsAssignableFrom(componentType)) continue;

            foreach (var constructor in page.Value)
            {
                if (!constructor.CanBeInjected(component)) continue;
                constructor.Inject(component);
            }
        }
    }
}