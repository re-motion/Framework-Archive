using System;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Reflection;

namespace Rubicon.ObjectBinding.Web.UnitTests.Domain
{

public class TypeWithBoolean: ReflectionBusinessObject
{
  private bool _booleanValue;
  private NaBoolean _naBooleanValue;

  public bool BooleanValue
  {
    get { return _booleanValue; }
    set { _booleanValue = value; }
  }

  public NaBoolean NaBooleanValue
  {
    get { return _naBooleanValue; }
    set { _naBooleanValue = value; }
  }
}

}
