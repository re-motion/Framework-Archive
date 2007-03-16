using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using Rubicon.Utilities;

namespace Rubicon.Development.UnitTesting.Configuration
{
  public class FakeInternalConfigSystem: IInternalConfigSystem
  {
    private Dictionary<string, object> _sections = new Dictionary<string, object>();

    public FakeInternalConfigSystem()
    {
    }

    public object GetSection (string configKey)
    {
      object value;
      if (_sections.TryGetValue (configKey, out value))
        return value;
      return null;
    }

    public void AddSection (string configKey, object section)
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