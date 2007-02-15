using System;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.UnitTests.Domain
{

public class TypeWithReference: ReflectionBusinessObject
{
  private TypeWithReference _referenceValue;
  private TypeWithReference[] _referenceList;
  private TypeWithReference _firstValue;
  private TypeWithReference _secondValue;
  private string _displayName;

  public TypeWithReference()
  {
  }

  public TypeWithReference (string displayName)
  {
    _displayName = displayName;
  }

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

  public TypeWithReference FirstValue
  {
    get { return _firstValue; }
    set { _firstValue = value; }
  }

  public TypeWithReference SecondValue
  {
    get { return _secondValue; }
    set { _secondValue = value; }
  }

  public override string DisplayName 
  { 
    get 
    {
      if (StringUtility.IsNullOrEmpty (_displayName))
        return ((IBusinessObjectWithIdentity) this).UniqueIdentifier;
      else
        return _displayName; 
    }
  }

  public override string ToString()
  {
    return DisplayName;
  }

}

}
