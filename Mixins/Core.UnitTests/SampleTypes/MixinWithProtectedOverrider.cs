using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public class MixinWithProtectedOverrider : Mixin<MixinWithProtectedOverrider.IRequirements, MixinWithProtectedOverrider.IRequirements>
  {
    public interface IRequirements
    {
      string VirtualMethod ();
      string VirtualProperty { get; }
      event EventHandler VirtualEvent;
    }

    [Override]
    protected string VirtualMethod ()
    {
      return "MixinWithProtectedOverrider.VirtualMethod-" + Base.VirtualMethod ();
    }

    [Override]
    protected string VirtualProperty
    {
      get { return "MixinWithProtectedOverrider.VirtualProperty-" + Base.VirtualProperty; }
    }

    [Override]
    protected event EventHandler VirtualEvent
    {
      add
      {
        Base.VirtualEvent += value;
        Base.VirtualEvent += ThisHandler;
      }
      remove
      {
        Base.VirtualEvent -= value;
        Base.VirtualEvent -= ThisHandler;
      }
    }

    private void ThisHandler (object sender, EventArgs e)
    {
      throw new NotImplementedException();
    }
  }
}
