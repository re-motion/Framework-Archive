using System;
using Mixins.Definitions;

namespace Mixins.UnitTests.Configuration.ValidationSampleTypes
{
  public class MixinImplementingIMixinTarget : IMixinTarget
  {
    public BaseClassDefinition Configuration
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