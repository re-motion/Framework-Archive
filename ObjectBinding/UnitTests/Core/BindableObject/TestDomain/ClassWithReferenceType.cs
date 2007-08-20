using System;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  [BindableObject]
  public class ClassWithReferenceType<T>
      where T: class
  {
    private T _scalar;
    private T _readOnlyScalar;
    private T _readOnlyNonPublicSetterScalar;
    private T _notVisibleAttributeScalar;
    private T _notVisibleNonPublicGetterScalar;
    private T _readOnlyAttributeScalar;
    private T[] _array;

    public ClassWithReferenceType ()
    {
    }

    public T Scalar
    {
      get { return _scalar; }
      set { _scalar = value; }
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
    public T NotVisibleAttributeScalar
    {
      get { return _notVisibleAttributeScalar; }
      set { _notVisibleAttributeScalar = value; }
    }

    public T NotVisibleNonPublicGetterScalar
    {
      protected get { return _notVisibleNonPublicGetterScalar; }
      set { _notVisibleNonPublicGetterScalar = value; }
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
  }
}