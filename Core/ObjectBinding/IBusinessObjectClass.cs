using System;

namespace Rubicon.ObjectBinding
{

public interface IBusinessObjectClass
{
  IBusinessObjectProperty GetPropertyDefinition (string propertyIdentifier);

  IBusinessObjectProperty[] GetPropertyDefinitions ();
}

}
