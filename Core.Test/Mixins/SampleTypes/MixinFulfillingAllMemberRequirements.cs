using System;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
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