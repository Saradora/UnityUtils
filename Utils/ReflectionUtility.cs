using System.Reflection;
using UnityMDK.Logging;

namespace UnityMDK.Reflection;

/// <summary>
/// Enter Summary here
/// </summary>
public static class ReflectionUtility
{
    private static readonly BindingFlags DefaultBindingFlags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
    public static void InvokeMethod(this object target, string methodName)
    {
        var methodInfo = target.GetType().GetMethod(methodName, DefaultBindingFlags);

        if (methodInfo is null)
        {
            Log.Error($"Method {methodName} not found");
        }
        else
        {
            methodInfo.Invoke(target, null);
        }
    }

    public static TReturn InvokeMethod<TReturn>(this object target, string methodName)
    {
        var methodInfo = target.GetType().GetMethod(methodName, DefaultBindingFlags);

        if (methodInfo is null)
        {
            Log.Error($"Method {methodName} not found");
            return default;
        }

        object returnValue = methodInfo.Invoke(target, null);
        if (returnValue == default) return default;

        if (returnValue is TReturn castValue)
        {
            return castValue;
        }

        throw new ArgumentException($"Method {methodName} isn't of type {typeof(TReturn).Name}");
    }

    /// <summary>
    /// Get any field, public or private, from an object
    /// </summary>
    public static bool TryGetField<TObject>(this object target, string fieldName, out TObject outObject)
    {
        if (target.TryGetField(fieldName, out object outValue) && outValue is TObject returnObject)
        {
            outObject = returnObject;
            return true;
        }

        outObject = default;
        return false;
    }

    public static bool TryGetField(this object target, string fieldName, out object outObject)
    {
        var fieldInfo = target.GetType().GetField(fieldName, DefaultBindingFlags);

        if (fieldInfo is null)
        {
            outObject = default;
            return false;
        }

        outObject = fieldInfo.GetValue(target);
        return true;
    }
        
    public static bool TryGetProperty<TObject>(this object target, string fieldName, out TObject outObject)
    {
        var propertyInfo = target.GetType().GetProperty(fieldName, DefaultBindingFlags);

        if (propertyInfo is null)
        {
            outObject = default;
            return false;
        }

        object returnValue = propertyInfo.GetValue(target);

        if (returnValue is TObject returnObject)
        {
            outObject = returnObject;
            return true;
        }

        outObject = default;
        return true;
    }

    public static void SetProperty<TObject>(this object target, string propertyName, TObject value)
    {
        var propInfo = target.GetType().GetProperty(propertyName, DefaultBindingFlags);

        if (propInfo is null)
        {
            Log.Error($"Field {propertyName} not found");
            return;
        }

        if (propInfo.PropertyType != typeof(TObject))
        {
            Log.Error($"Property {propertyName} isn't of type {typeof(TObject)}");
            return;
        }
            
        propInfo.SetValue(target, value);
    }

    public static TObject GetField<TObject>(this object target, string fieldName)
    {
        var fieldInfo = target.GetType().GetField(fieldName, DefaultBindingFlags);

        object returnValue = fieldInfo?.GetValue(target);

        if (returnValue is TObject returnObject)
        {
            return returnObject;
        }

        return default;
    }

    public static void SetField<TObject>(this object target, string fieldName, TObject value)
    {
        var fieldInfo = target.GetType().GetField(fieldName, DefaultBindingFlags);

        if (fieldInfo is null)
        {
            Log.Error($"Field {fieldName} not found");
            return;
        }

        object returnValue = fieldInfo.GetValue(target);

        if (returnValue is TObject)
        {
            fieldInfo.SetValue(target, value);
        }
    }
}