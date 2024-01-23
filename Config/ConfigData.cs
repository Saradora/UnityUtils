using BepInEx.Configuration;

namespace UnityMDK.Config;

public abstract class ConfigData
{
    internal abstract void Bind(ConfigFile cfg, string name, string section, string description);
}

public class ConfigData<T> : ConfigData
{
    private ConfigEntry<T> _configEntry;

    public T Value => _configEntry.Value;

    private readonly T _defaultValue;

    public static implicit operator T(ConfigData<T> config) => config.Value;

    public ConfigData(T defaultValue = default)
    {
        _defaultValue = defaultValue;
    }

    internal override void Bind(ConfigFile cfg, string name, string section, string description)
    {
        if (description is null)
            _configEntry = cfg.Bind(section, name, _defaultValue);
        else
            _configEntry = cfg.Bind(section, name, _defaultValue, description);
    }
}