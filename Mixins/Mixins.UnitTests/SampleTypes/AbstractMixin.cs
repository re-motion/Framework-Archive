using System;

namespace Mixins.UnitTests.SampleTypes
{
  public interface IAbstractMixin
  {
    string ImplementedMethod();
  }

  public abstract class AbstractMixin : Mixin<object>, IAbstractMixin
  {
    public string ImplementedMethod ()
    {
      return "AbstractMixin.ImplementedMethod-" + AbstractMethod(25);
    }

    protected abstract string AbstractMethod(int i);
  }
}
