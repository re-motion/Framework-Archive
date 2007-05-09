using System;
using System.Collections.Generic;
using System.Text;
using Mixins.UnitTests.SampleTypes;

namespace Mixins.UnitTests.Mixins.CodeGenSampleTypes
{
  public interface IVeryGenericMixin
  {
    string GetMessage<T>(T t);
  }

  public interface IVeryGenericMixin<T1, T2>
  {
    string GenericIfcMethod<T3> (T1 t1, T2 t2, T3 t3);
  }

  public class VeryGenericMixin<TThis, TBase> : Mixin<TThis, TBase>, IVeryGenericMixin<TThis, TBase>, IVeryGenericMixin
    where TThis : class, IBaseType31, IBaseType32, IBT3Mixin4
    where TBase : class, IBaseType31, IBaseType32, IBT3Mixin4
  {
    public string GenericIfcMethod<T3> (TThis t1, TBase t2, T3 t3)
    {
      return "IVeryGenericMixin.GenericIfcMethod-" + t3;
    }

    public string GetMessage<T> (T t)
    {
      return GenericIfcMethod (This, Base, t);
    }
  }

  public interface IUltraGenericMixin
  {
    string GetMessage<T> (T t);
  }

  public abstract class AbstractDerivedUltraGenericMixin<TThis, TBase> : VeryGenericMixin<TThis, TBase>, IUltraGenericMixin
    where TThis : class, IBaseType31, IBaseType32, IBT3Mixin4
    where TBase : class, IBaseType31, IBaseType32, IBT3Mixin4, IVeryGenericMixin<TThis, TBase>
  {
    protected abstract string AbstractGenericMethod<T>();

    public new string GetMessage<T> (T t)
    {
      return AbstractGenericMethod<T>() + "-" + Base.GenericIfcMethod (This, Base, t);
    }
  }

  [Uses(typeof (AbstractDerivedUltraGenericMixin<,>))]
  [Uses (typeof(BT3Mixin4))]
  public class ClassOverridingUltraGenericStuff : BaseType3
  {
    [Override]
    public string AbstractGenericMethod<T>()
    {
      return typeof (T).Name;
    }
  }
}
