using System;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.UnitTests.Domain
{
  //[BindableObject]
  public class TypeWithReference:ReflectionBusinessObject
  {
    public static TypeWithReference Create ()
    {
      return new TypeWithReference();
      //return ObjectFactory.Create<TypeWithReference> ().With ();
    }

    public static TypeWithReference Create (TypeWithReference firstValue, TypeWithReference secondValue)
    {
      return new TypeWithReference (firstValue, secondValue);
      //return ObjectFactory.Create<TypeWithReference> ().With (firstValue, secondValue);
    }

    public static TypeWithReference Create (string displayName)
    {
      return new TypeWithReference (displayName);
      //return ObjectFactory.Create<TypeWithReference> ().With (displayName);
    }

    private TypeWithReference _referenceValue;
    private TypeWithReference[] _referenceList;
    private TypeWithReference _firstValue;
    private TypeWithReference _secondValue;
    private string _displayName;

    protected TypeWithReference ()
    {
    }

    protected TypeWithReference (TypeWithReference firstValue, TypeWithReference secondValue)
    {
      _firstValue = firstValue;
      _secondValue = secondValue;
    }

    protected TypeWithReference (string displayName)
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

    //[Override]
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

    public override string ToString ()
    {
      return DisplayName;
    }
  }
}