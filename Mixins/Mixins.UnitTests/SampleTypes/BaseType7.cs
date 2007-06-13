using System;

namespace Mixins.UnitTests.SampleTypes
{
  public interface IBaseType7
  {
    string One ();
    string Two ();
    string Three ();
    string Four ();
    string Five ();
  }

  public abstract class BaseType7 : IBaseType7
  {
    public virtual string One()
    {
      return "BaseType7.One";
    }

    public virtual string Two ()
    {
      return "BaseType7.Two";
    }

    public abstract string Three ();

    public abstract string Four ();

    public virtual string Five ()
    {
      return "BaseType7.Five";
    }
  }
}
