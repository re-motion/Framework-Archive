using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  public interface IMixinRequiringAllMembersRequirements
  {
    void Method ();
    int Property { get; set; }
    event Func<string> Event;
  }

  public class MixinRequiringAllMembersFace
      : Mixin<IMixinRequiringAllMembersRequirements>
  {
    public int PropertyViaThis
    {
      get { return This.Property; }
    }
  }
}
