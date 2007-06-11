using System;

namespace Mixins.UnitTests.SampleTypes
{
  public interface IAbstractMixin
  {
    string ImplementedMethod();
  }

  [Serializable]
  public abstract class AbstractMixin : Mixin<object, object>, IAbstractMixin
  {
    public int I;

    public string ImplementedMethod ()
    {
      return "AbstractMixin.ImplementedMethod-" + AbstractMethod(25);
    }

    protected abstract string AbstractMethod (int i);
  }

  [Serializable]
  public abstract class AbstractMixin2 : Mixin<object, object>
  {
    protected abstract string AbstractMethod (int i);
  }
}
