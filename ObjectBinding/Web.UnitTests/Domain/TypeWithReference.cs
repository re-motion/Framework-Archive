using System;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Reflection;

namespace Rubicon.ObjectBinding.Web.UnitTests.Domain
{

public class TypeWithReference: ReflectionBusinessObject
{
  private TypeWithReference _referenceValue;

  public TypeWithReference ReferenceValue
  {
    get { return _referenceValue; }
    set { _referenceValue = value; }
  }
}

}
