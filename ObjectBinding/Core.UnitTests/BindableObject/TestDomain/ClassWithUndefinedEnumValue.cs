using System;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain
{
  [BindableObject]
  public class ClassWithUndefinedEnumValue
  {
    private EnumWithUndefinedValue _scalar;
    private EnumWithUndefinedValue[] _array;

    public ClassWithUndefinedEnumValue ()
    {
    }

    public EnumWithUndefinedValue Scalar
    {
      get { return _scalar; }
      set { _scalar = value; }
    }

    public EnumWithUndefinedValue[] Array
    {
      get { return _array; }
      set { _array = value; }
    }
  }
}