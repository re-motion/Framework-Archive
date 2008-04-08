using System;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;

namespace Remotion.Core.UnitTests.Mixins.ValidationTests.ValidationSampleTypes
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