using System.Reflection;
using System.Text.RegularExpressions;
using BepInEx.Configuration;
using UnityMDK.Logging;

namespace UnityMDK.Config;

public static class ConfigBinder
{
    private static readonly Type ConfigDataType = typeof(ConfigData);

    private static ConfigFile _currentFile;
    
    public static void BindAll(ConfigFile cfg)
    {
        _currentFile = cfg;
        foreach (var type in Assembly.GetCallingAssembly().GetTypes())
        {
            BindType(type);
        }
        _currentFile = null;
    }

    private static void BindType(Type type)
    {
        string currentSection = "General";

        foreach (var memberInfo in type.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (!memberInfo.MemberType.HasFlag(MemberTypes.Field & MemberTypes.Property))
                continue;

            ConfigSectionAttribute configSectionAttribute = memberInfo.GetCustomAttribute<ConfigSectionAttribute>();
            if (configSectionAttribute is not null)
                currentSection = configSectionAttribute.Section;
            
            ConfigDescriptionAttribute configDescriptionAttribute = memberInfo.GetCustomAttribute<ConfigDescriptionAttribute>();
            string description = configDescriptionAttribute?.Description;

            string name = memberInfo.Name.Replace("_", "");
            name = currentSection + Regex.Replace(name, @"\b\p{Ll}", match => match.Value.ToUpper());

            FieldInfo field = memberInfo switch
            {
                PropertyInfo => type.GetField($"<{memberInfo.Name}>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic),
                FieldInfo fieldInfo => fieldInfo.Name.Contains("k__BackingField") ? null : fieldInfo,
                _ => null
            };
            
            if (field is null) continue;
            
            Type fieldType = field.FieldType;
            if (!ConfigDataType.IsAssignableFrom(fieldType)) continue;
            
            BindField(field, currentSection, name, description);
        }
    }

    private static void BindField(FieldInfo fieldInfo, string section, string name, string description)
    {
        Type genericType = fieldInfo.FieldType.GetGenericArguments()[0];
        if (!TomlTypeConverter.CanConvert(genericType))
        {
            Log.Error($"Type {genericType.Name} is not supported by the config system. Supported types: {string.Join(", ", TomlTypeConverter.GetSupportedTypes().Select(x => x.Name).ToArray())}");
            return;
        }
        
        ConfigData value = (ConfigData)fieldInfo.GetValue(null);
        if (value is null)
        {
            value = (ConfigData)Activator.CreateInstance(typeof(ConfigData<>).MakeGenericType(genericType));
            fieldInfo.SetValue(null, value);
        }
        
        value.Bind(_currentFile, name, section, description);
    }
}