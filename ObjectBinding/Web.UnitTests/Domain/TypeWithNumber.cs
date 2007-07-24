using System;
using Rubicon.Mixins;

namespace Rubicon.ObjectBinding.Web.UnitTests.Domain
{
  [BindableObject]
  public class TypeWithNumber
  {
    public static TypeWithNumber Create ()
    {
      return ObjectFactory.Create<TypeWithNumber> ().With ();
    }

    private int _int32Value;
    private int? _nullableInt32Value;

    protected TypeWithNumber ()
    {
    }

    public int Int32Value
    {
      get { return _int32Value; }
      set { _int32Value = value; }
    }

    public int? NullableInt32Value
    {
      get { return _nullableInt32Value; }
      set { _nullableInt32Value = value; }
    }
  }
}