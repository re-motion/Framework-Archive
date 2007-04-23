using System;

namespace Mixins.UnitTests.SampleTypes
{
  public interface IBT1Mixin1
  {
    string IntroducedMethod ();
    string IntroducedProperty { get; }
    event EventHandler IntroducedEvent;
  }

  [MixinFor (typeof (BaseType1))]
  [Serializable]
  public class BT1Mixin1 : IBT1Mixin1
  {
    [Override]
    public string VirtualMethod ()
    {
      return "BT1Mixin1.VirtualMethod";
    }

    public string BackingField = "BT1Mixin1.BackingField";

    [Override]
    public virtual string VirtualProperty
    {
      set { BackingField = value; } // no getter
    }

    [Override]
    public virtual event EventHandler VirtualEvent;


    public string IntroducedMethod ()
    {
      return "BT1Mixin1.IntroducedMethod";
    }

    public string IntroducedProperty
    {
      get { return "BT1Mixin1.IntroducedProperty"; }
    }

    public event EventHandler IntroducedEvent;
  }
}
