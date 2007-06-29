using System;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain
{
  public class ClassWithReferenceType<T>
      where T: class
  {
    private T _scalar;
    private T[] _array;

    public ClassWithReferenceType ()
    {
    }

    public T Scalar
    {
      get { return _scalar; }
      set { _scalar = value; }
    }

    public T[] Array
    {
      get { return _array; }
      set { _array = value; }
    }
  }
}