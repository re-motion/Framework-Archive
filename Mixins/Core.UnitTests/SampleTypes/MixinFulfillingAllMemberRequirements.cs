using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public class MixinFulfillingAllMemberRequirements : IMixinRequiringAllMembersRequirements
  {
    public void Method ()
    {
      throw new NotImplementedException ();
    }

    public int Property
    {
      get { throw new NotImplementedException (); }
      set { throw new NotImplementedException (); }
    }

    public event Func<string> Event;
  }
}