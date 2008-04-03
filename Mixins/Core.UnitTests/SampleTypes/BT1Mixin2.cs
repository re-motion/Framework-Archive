using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  [Extends (typeof (BaseType1))]
  [Serializable]
  [AcceptsAlphabeticOrdering]
  public class BT1Mixin2
  {
    [OverrideTarget]
    public string VirtualMethod ()
    {
      return "Mixin2ForBT1.VirtualMethod";
    }

    [OverrideTarget]
    public string VirtualProperty
    {
      get { return "Mixin2ForBT1.VirtualProperty"; }
      // no setter
    }

    public EventHandler BackingEventField;

    [OverrideTarget]
    public virtual event EventHandler VirtualEvent
    {
      add { BackingEventField += value; }
      remove { BackingEventField -= value; }
    }
  }
}
