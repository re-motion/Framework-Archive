using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Uses(typeof(MixinWithAbstractMembers))]
  [Serializable]
  public class ClassOverridingMixinMembersProtected
  {
    [Override]
    protected string AbstractMethod (int i)
    {
      return "ClassOverridingMixinMembersProtected.AbstractMethod-" + i;
    }

    [Override]
    protected string AbstractProperty
    {
      get { return "ClassOverridingMixinMembersProtected.AbstractProperty"; }
    }

    [Override]
    protected string RaiseEvent ()
    {
      return _abstractEvent ();
    }

    private Func<string> _abstractEvent;

    [Override]
    protected event Func<string> AbstractEvent
    {
      add { _abstractEvent += value; }
      remove { _abstractEvent -= value; }
    }
  }
}
