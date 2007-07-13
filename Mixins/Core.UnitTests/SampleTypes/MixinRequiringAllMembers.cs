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
      : Mixin<IMixinRequiringAllMembersRequirements/*, IMixinRequiringAllMembersRequirements*/>
  {
    public int PropertyViaThis
    {
      get { return This.Property; }
    }

    /*public int PropertyViaBase
    {
      get { return Base.Property; }
    }*/
  }
}
