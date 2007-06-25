using System;

namespace Mixins.UnitTests.SampleTypes
{
  public class TargetForOverridesAndShadowing
  {
    public virtual void Method (int i) { }

    public virtual int Property
    {
      get { return 0; }
      set { }
    }

    public virtual event EventHandler Event;
  }
  
  public class BaseWithOverrideAttributes
  {
    [Override]
    public virtual void Method(int i)
    {
    }

    [Override]
    public virtual int Property
    {
      get { return 0; }
      set { }
    }

    [Override]
    public virtual event EventHandler Event;
  }

  public class DerivedWithoutOverrideAttributes : BaseWithOverrideAttributes
  {
    public override void Method (int i)
    {
    }

    public override int Property
    {
      get { return 0; }
      set { }
    }

    public override event EventHandler Event;
  }

  public class DerivedNewWithAdditionalOverrideAttributes : BaseWithOverrideAttributes
  {
    [Override]
    public new void Method (int i)
    {
    }

    [Override]
    public new int Property
    {
      get { return 0; }
      set { }
    }

    [Override]
    public new event EventHandler Event;
  }

  public class BaseWithoutOverrideAttributes
  {
    public virtual void Method (int i)
    {
    }

    public virtual int Property
    {
      get { return 0; }
      set { }
    }

    public virtual event EventHandler Event;
  }

  public class DerivedNewWithOverrideAttributes : BaseWithoutOverrideAttributes
  {
    [Override]
    public new void Method (int i)
    {
    }

    [Override]
    public new int Property
    {
      get { return 0; }
      set { }
    }

    [Override]
    public new event EventHandler Event;
  }

  
  public class DerivedNewWithoutOverrideAttributes : BaseWithoutOverrideAttributes
  {
    public new void Method (int i)
    {
    }

    public new int Property
    {
      get { return 0; }
      set { }
    }

    public new event EventHandler Event;
  }
}
