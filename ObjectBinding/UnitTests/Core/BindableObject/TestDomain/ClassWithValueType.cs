using System;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  [BindableObject]
  public class ClassWithValueType<T>
      where T: struct
  {
    private T _scalar;
    private T? _nullableScalar;
    private T _readOnlyScalar;
    private T _readOnlyNonPublicSetterScalar;
    private T _notVisibleScalar;
    private T _readOnlyAttributeScalar;
    private T[] _array;
    private T?[] _nullableArray;

    public ClassWithValueType ()
    {
    }

    public T Scalar
    {
      get { return _scalar; }
      set { _scalar = value; }
    }

    public T? NullableScalar
    {
      get { return _nullableScalar; }
      set { _nullableScalar = value; }
    }

    public T ReadOnlyScalar
    {
      get { return _readOnlyScalar; }
    }

    public T ReadOnlyNonPublicSetterScalar
    {
      get { return _readOnlyNonPublicSetterScalar; }
      protected set { _readOnlyNonPublicSetterScalar = value; }
    }

    [ObjectBinding (Visible = false)]
    public T NotVisibleScalar
    {
      get { return _notVisibleScalar; }
      set { _notVisibleScalar = value; }
    }

    [ObjectBinding (ReadOnly = true)]
    public T ReadOnlyAttributeScalar
    {
      get { return _readOnlyAttributeScalar; }
      set { _readOnlyAttributeScalar = value; }
    }

    public T[] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public T?[] NullableArray
    {
      get { return _nullableArray; }
      set { _nullableArray = value; }
    }
  }
}