using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.Configuration.ValidationTests.ValidationSampleTypes
{
  public class ClassOverridingMixinEvent
  {
    [OverrideMixin]
    public virtual event EventHandler Event;
  }
}
