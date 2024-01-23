namespace UnityMDK.Config;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ConfigSectionAttribute : Attribute
{
    internal string Section;
    
    public ConfigSectionAttribute(string section)
    {
        Section = section;
    }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ConfigDescriptionAttribute : Attribute
{
    internal string Description;
    
    public ConfigDescriptionAttribute(string description)
    {
        Description = description;
    }
}