using System;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Reflection;

namespace Rubicon.ObjectBinding.Web.UnitTests.Domain
{

public class TypeWithReference: ReflectionBusinessObject
{
  private TypeWithReference _referenceValue;
  private TypeWithReference[] _referenceList;

  public TypeWithReference ReferenceValue
  {
    get { return _referenceValue; }
    set { _referenceValue = value; }
  }

  public TypeWithReference[] ReferenceList
  {
    get { return _referenceList; }
    set { _referenceList = value; }
  }
}

}
