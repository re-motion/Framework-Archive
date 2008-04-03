using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  public class MixinRequiringAllMembersBase
      : Mixin<object, IMixinRequiringAllMembersRequirements>
  {
    public int PropertyViaBase
    {
      get { return Base.Property; }
    }
  }
}
