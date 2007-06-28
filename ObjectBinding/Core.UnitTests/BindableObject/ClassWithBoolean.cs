using System;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  public class ClassWithBoolean
  {
    private bool _boolean;
    private bool? _nullableBoolean;
    private bool[] _booleanArray;
    private bool?[] _nullableBooleanArray;

    public ClassWithBoolean ()
    {
    }

    public bool Booelan
    {
      get { return _boolean; }
      set { _boolean = value; }
    }

    public bool? NullableBoolean
    {
      get { return _nullableBoolean; }
      set { _nullableBoolean = value; }
    }

    public bool[] BooleanArray
    {
      get { return _booleanArray; }
      set { _booleanArray = value; }
    }

    public bool?[] NullableBooleanArray
    {
      get { return _nullableBooleanArray; }
      set { _nullableBooleanArray = value; }
    }
  }
}