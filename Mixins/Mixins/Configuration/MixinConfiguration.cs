using System;
using System.Collections.Generic;

namespace Mixins.Configuration
{
  public class MixinConfiguration : ClassConfiguration
  {
    private List<Type> _introducedInterfaces = new List<Type> ();
    private List<OverrideConfiguration> _overrides = new List<OverrideConfiguration> ();

    public MixinConfiguration (Type type)
        : base (type)
    {
    }

    public IEnumerable<Type> IntroducedInterfaces
    {
      get { return _introducedInterfaces; }
    }

    public void AddIntroducedInterface (Type newInterface)
    {
      _introducedInterfaces.Add (newInterface);
    }

    public IEnumerable<OverrideConfiguration> Overrides
    {
      get { return _overrides; }
    }

    public void AddOverride (OverrideConfiguration newOverride)
    {
      _overrides.Add (newOverride);
    }
  }
}
