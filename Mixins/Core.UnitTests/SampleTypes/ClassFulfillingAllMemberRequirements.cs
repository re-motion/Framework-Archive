using System;
using System.Collections.Generic;
using System.Text;
using Rubicon;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  // TODO: [Uses (typeof (MixinRequiringAllMembers))]
  public class ClassFulfillingAllMemberRequirements : IMixinRequiringAllMembersRequirements
  {
    public void Method ()
    {
      throw new NotImplementedException();
    }

    public int Property
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public event Func<string> Event;
  }
}
