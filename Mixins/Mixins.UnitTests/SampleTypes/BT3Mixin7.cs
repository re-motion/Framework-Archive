using System;

namespace Mixins.UnitTests.SampleTypes
{
  public interface ICBaseType3 : IBaseType31, IBaseType32, IBaseType33, IBaseType34, IBaseType35
  { }

  public interface ICBaseType3BT3Mixin4 : ICBaseType3, IBT3Mixin4
  { }

  public class BT3Mixin7 : Mixin<ICBaseType3BT3Mixin4, ICBaseType3BT3Mixin4>
  {
  }
}
