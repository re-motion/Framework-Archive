using System;
using Rubicon.ObjectBinding.Reflection;

namespace Rubicon.ObjectBinding.Web.UnitTests.Domain
{

public class TypeWithNumber: ReflectionBusinessObject
{
  private int _int32Value;
  private int? _nullableInt32Value;

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
