using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Uses(typeof(MixinWithAbstractMembers))]
  [Serializable]
  public class ClassOverridingMixinMembers
  {
    [OverrideMixinMember]
    public string AbstractMethod(int i)
    {
      return "ClassOverridingMixinMembers.AbstractMethod-" + i;
    }

    [OverrideMixinMember]
    public string AbstractProperty
    {
      get { return "ClassOverridingMixinMembers.AbstractProperty"; }
    }

    [OverrideMixinMember]
    public string RaiseEvent ()
    {
      return _abstractEvent ();
    }

    private Func<string> _abstractEvent;

    [OverrideMixinMember]
    public event Func<string> AbstractEvent
    {
      add { _abstractEvent += value; }
      remove { _abstractEvent -= value; }
    }
  }
}
