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
  /// <example>
  ///   The following pseudo code shows how this value affects the binding behaviour.
  ///   <code><![CDATA[
  ///   Address address = person.Address;
  ///   address.City = "Vienna";
  ///   // the RequiresWriteBack property of the 'Address' business object class specifies whether the following statement is required:
  ///   person.Address = address;
  ///   ]]></code>
  /// </example>
  bool RequiresWriteBack { get; }

  string Identifier { get; }
}

public interface IBusinessObjectClassWithIdentity: IBusinessObjectClass
{
  IBusinessObjectWithIdentity GetObject (string uniqueIdentifier);
}

}
