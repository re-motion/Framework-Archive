using System;

namespace Rubicon.ObjectBinding
{

public interface IBusinessObjectClass
{
  IBusinessObjectProperty GetPropertyDefinition (string propertyIdentifier);
  IBusinessObjectProperty[] GetPropertyDefinitions ();

  IBusinessObjectProvider BusinessObjectProvider { get; }

  /// <summary>
  ///   Specifies wheter a referenced object of this business object class needs to be written back to its container if some of its values have changed.
  /// </summary>
  /// <remarks>
  ///   Address address = person.Address;
  ///   address.City = "Vienna";
  ///   // the RequiresWriteBack property of the 'Address' business object class specifies whether the following statement is required:
  ///   person.Address = address;
  /// </remarks>
  bool RequiresWriteBack { get; }
}

public interface IBusinessObjectClassWithIdentity: IBusinessObjectClass
{
  IBusinessObjectWithIdentity GetObject (string uniqueIdentifier);
}

}
