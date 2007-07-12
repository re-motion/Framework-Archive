using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public interface IMixinRequiringAllMembersRequirements
  {
    void Method ();
    int Property { get; set; }
    event Func<string> Event;
  }

  public class MixinRequiringAllMembers
      : Mixin<IMixinRequiringAllMembersRequirements>
      //: Mixin<IMixinRequiringAllMembersRequirements, IMixinRequiringAllMembersRequirements>
  {
  }
}
