using System;
using Rubicon.Mixins;

namespace Rubicon.ObjectBinding.UnitTests.Web.Domain
{
  [BindableObject]
  public class TypeWithBoolean
  {
    public static TypeWithBoolean Create ()
    {
      return ObjectFactory.Create<TypeWithBoolean> (true).With ();
    }

    private bool _booleanValue;
    private bool? _nullableBooleanValue;

    protected TypeWithBoolean ()
    {
    }

    public bool BooleanValue
    {
      get { return _booleanValue; }
      set { _booleanValue = value; }
    }

    public bool? NullableBooleanValue
    {
      get { return _nullableBooleanValue; }
      set { _nullableBooleanValue = value; }
    }
  }
}