using System;

namespace Rubicon.ObjectBinding
{

public interface IBusinessObjectClass
{
  IBusinessObjectProperty GetProperty (string propertyIdentifier);

  IBusinessObjectProperty[] GetProperties ();
}

}
