using System;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.UnitTests.ValidationTests.ValidationSampleTypes
{
  public class MixinImplementingIMixinTarget : IMixinTarget
  {
    public TargetClassDefinition Configuration
    {
      get { throw new NotImplementedException (); }
    }

    public object[] Mixins
    {
      get { throw new NotImplementedException (); }
    }

    public object FirstBaseCallProxy
    {
      get { throw new NotImplementedException(); }
    }
  }
}