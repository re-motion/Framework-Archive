using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
{
  public class BT1Attribute : Attribute { }

  [Serializable]
  [BT1Attribute]
  public class BaseType1
  {
    public int I;

    public virtual string VirtualMethod ()
    {
      return "BaseType1.VirtualMethod";
    }

    public virtual string VirtualMethod (string text)
    {
      return "BaseType1.VirtualMethod(" + text + ")";
    }

    private string _backingField = "BaseType1.BackingField";

    public virtual string VirtualProperty
    {
      get { return _backingField; }
      set { _backingField = value; }
    }

    public object this [int index]
    {
      get { return null; }
    }

    public object this[string index]
    {
      set { }
    }

    public virtual event EventHandler VirtualEvent;

    public event EventHandler ExplicitEvent
    {
      add { VirtualEvent += value; }
      remove { VirtualEvent -= value; }
    }
  }
}
