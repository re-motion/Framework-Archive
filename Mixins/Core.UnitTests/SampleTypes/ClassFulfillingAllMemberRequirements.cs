using System;
using System.Collections.Generic;
using System.Text;
using Remotion;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  [Uses (typeof (MixinRequiringAllMembersFace))]
  [Uses (typeof (MixinRequiringAllMembersBase))]
  public class ClassFulfillingAllMemberRequirements : IMixinRequiringAllMembersRequirements
  {
    public void Method ()
    {
      throw new NotImplementedException();
    }

    public int Property
    {
      get { return 11; }
      set { throw new NotImplementedException(); }
    }

    public event Func<string> Event;
  }
}
