using System;

namespace Rubicon.ObjectBinding
{

public interface IBusinessObjectClass
{
  IBusinessObjectProperty GetPropertyDefinition (string propertyIdentifier);
  IBusinessObjectProperty[] GetPropertyDefinitions ();

  IBusinessObjectProvider BusinessObjectProvider { get; }
}

public interface IBusinessObjectClassWithIdentity: IBusinessObjectClass
{
  IBusinessObjectWithIdentity GetObject (string uniqueIdentifier);
}

}
