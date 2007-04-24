using System;
using Mixins.Definitions;

namespace Mixins.UnitTests.Configuration.ValidationSampleTypes
{
  public class MixinImplementingIMixinTarget : IMixinTarget
  {
    public BaseClassDefinition Configuration
    {
      get { throw new Exception ("The method or operation is not implemented."); }
    }

    public object[] Mixins
    {
      get { throw new Exception ("The method or operation is not implemented."); }
    }
  }
}