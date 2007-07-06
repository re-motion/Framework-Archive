using System;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain
{
  [BindableObject]
  public class ClassWithReferenceType<T>
      where T: class
  {
    private T _scalar;
    private T _readOnlyScalar;
    private T _notVisibleScalar;
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
  }
}