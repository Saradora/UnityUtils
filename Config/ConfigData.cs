using BepInEx.Configuration;
using UnityMDK.Logging;

namespace UnityMDK.Config;

internal interface IConfigData
{
    internal void Bind(ConfigFile cfg, string name, string section, string description);
}

public class ConfigData<T> : IConfigData
{
    private ConfigEntry<T> _configEntry;

    public T Value => _configEntry.Value;

    private readonly T _defaultValue;

    public event Action<T> ConfigChanged;

    public static implicit operator T(ConfigData<T> config) => config.Value;

    public ConfigData(T defaultValue = default)
    {
        _defaultValue = defaultValue;
    }

    void IConfigData.Bind(ConfigFile cfg, string name, string section, string description)
    {
        if (_configEntry is not null)
        {
            Log.Warning($"Config {name} is already bound");
            return;
        }
        
        if (description is null)
            _configEntry = cfg.Bind(section, name, _defaultValue);
        else
            _configEntry = cfg.Bind(section, name, _defaultValue, description);

        _configEntry.SettingChanged += OnSettingChanged;
    }

    private void OnSettingChanged(object sender, EventArgs e)
    {
        ConfigChanged?.Invoke(Value);
    }
}