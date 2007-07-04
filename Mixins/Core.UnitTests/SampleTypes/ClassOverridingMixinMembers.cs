using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Uses(typeof(MixinWithAbstractMembers))]
  [Serializable]
  public class ClassOverridingMixinMembers
  {
    [Override]
    public string AbstractMethod(int i)
    {
      return "ClassOverridingMixinMembers.AbstractMethod-" + i;
    }

    [Override]
    public string AbstractProperty
    {
      get { return "ClassOverridingMixinMembers.AbstractProperty"; }
    }

    [Override]
    public string RaiseEvent ()
    {
      return _abstractEvent ();
    }

    private Func<string> _abstractEvent;

    [Override]
    public event Func<string> AbstractEvent
    {
      add { _abstractEvent += value; }
      remove { _abstractEvent -= value; }
    }
  }
}
