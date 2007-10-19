using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.Configuration.ValidationSampleTypes
{
  public class ClassOverridingMixinEvent
  {
    [OverrideMixin]
    public virtual event EventHandler Event;
  }
}
