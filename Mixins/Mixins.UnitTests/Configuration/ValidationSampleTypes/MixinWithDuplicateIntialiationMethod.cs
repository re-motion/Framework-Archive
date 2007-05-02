using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.Configuration.ValidationSampleTypes
{
  public class MixinWithDuplicateIntialiationMethod
  {
    [MixinInitializationMethod]
    public void Initialize ()
    {
    }

    [MixinInitializationMethod]
    public void Initialize ([This]object _this)
    {
    }
  }
}
