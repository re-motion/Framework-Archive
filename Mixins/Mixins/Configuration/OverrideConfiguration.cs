using System;

namespace Mixins.Configuration
{
  public class OverrideConfiguration
  {
    private MemberConfiguration _overrider;
    private MemberConfiguration _overridden;

    public OverrideConfiguration (MemberConfiguration overrider, MemberConfiguration overridden)
    {
      _overrider = overrider;
      _overridden = overridden;
    }

    public MemberConfiguration Overrider
    {
      get { return _overrider; }
    }

    public MemberConfiguration Overridden
    {
      get { return _overridden; }
    }
  }
}
