using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Uses (typeof (MixinRequiringAllMembers))]
  public class ClassFulfillingAllMemberRequirementsDuck
  {
    public void Method ()
    {
      throw new NotImplementedException ();
    }

    public int Property
    {
      get { return 42; }
      set { throw new NotImplementedException (); }
    }

    public event Func<string> Event;
  }
}