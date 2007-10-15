using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.Configuration.ValidationSampleTypes
{
  public class ClassOverridingMixinEvent
  {
    [OverrideMixinMember]
    public virtual event EventHandler Event;
  }
}
