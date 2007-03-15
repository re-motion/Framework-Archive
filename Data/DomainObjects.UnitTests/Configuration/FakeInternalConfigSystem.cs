using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Internal;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration
{
  public class FakeInternalConfigSystem : IInternalConfigSystem
  {
    private Dictionary<string, ConfigurationSection> _sections = new Dictionary<string, ConfigurationSection>();
    
    public FakeInternalConfigSystem()
    {
    }

    public object GetSection (string configKey)
    {
      ConfigurationSection value;
      if  (_sections.TryGetValue (configKey, out value))
        return value;
      return null;
    }

    public void AddSection (string configKey, ConfigurationSection section)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("configKey", configKey);
      ArgumentUtility.CheckNotNull ("section", section);

      _sections.Add (configKey, section);
    }

    public void RefreshConfig (string sectionName)
    {
      throw new NotSupportedException();
    }

    public bool SupportsUserConfig
    {
      get { return false; }
    }
  }
}