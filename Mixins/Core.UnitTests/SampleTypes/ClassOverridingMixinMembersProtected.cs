using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  [Uses(typeof(MixinWithAbstractMembers))]
  [Serializable]
  public class ClassOverridingMixinMembersProtected
  {
    [OverrideMixin]
    protected string AbstractMethod (int i)
    {
      return "ClassOverridingMixinMembersProtected.AbstractMethod-" + i;
    }

    [OverrideMixin]
    protected string AbstractProperty
    {
      get { return "ClassOverridingMixinMembersProtected.AbstractProperty"; }
    }

    [OverrideMixin]
    protected string RaiseEvent ()
    {
      return _abstractEvent ();
    }

    private Func<string> _abstractEvent;

    [OverrideMixin]
    protected event Func<string> AbstractEvent
    {
      add { _abstractEvent += value; }
      remove { _abstractEvent -= value; }
    }
  }
}
