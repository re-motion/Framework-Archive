using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.Configuration.ValidationSampleTypes
{
  public class MixinWithGenericIntialiationMethod
  {
    [MixinInitializationMethod]
    public void Initialize<T> ()
    {
    }
  }
}
