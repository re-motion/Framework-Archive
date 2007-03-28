using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.Configuration
{
  /// <summary>
  /// Set of base class configurations for configuríng a CodeBuilder.
  /// </summary>
  public class ApplicationConfiguration
  {
    private Dictionary<Type, BaseClassConfiguration> _baseClassConfigurations = new Dictionary<Type, BaseClassConfiguration>();

    public IEnumerable<BaseClassConfiguration> BaseClassConfigurations
    {
      get { return _baseClassConfigurations.Values; }
    }

    public void AddBaseClassConfiguration (BaseClassConfiguration configuration)
    {
      if (HasBaseClassConfiguration(configuration.Type))
      {
        string message = string.Format("Configuration for type {0} already exists:", configuration.Type.FullName);
        throw new InvalidOperationException(message);
      }
      _baseClassConfigurations.Add (configuration.Type, configuration);
    }

    public BaseClassConfiguration GetBaseClassConfiguration (Type type)
    {
      if (HasBaseClassConfiguration (type))
      {
        return _baseClassConfigurations[type];
      }
      else
      {
        return null;
      }
    }

    public bool HasBaseClassConfiguration (Type type)
    {
      return _baseClassConfigurations.ContainsKey (type);
    }
  }
}
