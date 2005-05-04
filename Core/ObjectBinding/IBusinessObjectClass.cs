using System;

namespace Rubicon.ObjectBinding
{

/// <summary>
///   The <b>IBusinessObjectClassWithIdentity</b> interface provides functionality for defining the <b>Class</b> of an 
///   <see cref="IBusinessObject"/>. 
/// </summary>
/// <remarks>
///   <para>
///     The <b>IBusinessObjectClass</b> interface provides the list of <see cref="IBusinessObjectProperty"/> instances
///     available by an <see cref="IBusinessObject"/> of this <b>Class</b>'s type. 
///   </para><para>
///     It also provides services for accessing class specific meta data.
///   </para>
/// </remarks>
public interface IBusinessObjectClass
{
  /// <summary> Returns the <see cref="IBusinessObjectProperty"/> for the passed <paramref name="propertyIdentifier"/>. </summary>
  /// <param name="propertyIdentifier"> 
  ///   A <see cref="String"/> uniquely identifying an <see cref="IBusinessObjectProperty"/> in this
  ///   <see cref="IBusinessObjectClass"/>.
  /// </param>
  /// <returns> Returns the <see cref="IBusinessObjectProperty"/> or <see langword="null"/>. </returns>
  /// <remarks> 
  ///   It is not specified wheter an exception is thrown or <see langword="null"/> is returned if the 
  ///   <see cref="IBusinessObjectProperty"/> could not be found.
  /// </remarks>
  IBusinessObjectProperty GetPropertyDefinition (string propertyIdentifier);

  /// <summary> 
  ///   Returns the <see cref="IBusinessObjectProperty"/> instances defined for this <see cref="IBusinessObjectClass"/>.
  /// </summary>
  /// <returns> An array of <see cref="IBusinessObjectProperty"/> instances. Must not be <see langword="null"/>. </returns>
  IBusinessObjectProperty[] GetPropertyDefinitions ();

  /// <summary> Gets the <see cref="IBusinessObjectProvider"/> for this <see cref="IBusinessObjectClass"/>. </summary>
  /// <value> An instance of the <see cref="IBusinessObjectProvider"/> type.
  ///   <note type="inheritinfo">
  ///     Must not return <see langword="null"/>.
  ///   </note>
  /// </value>
  IBusinessObjectProvider BusinessObjectProvider { get; }

  /// <summary>
  ///   Gets a flag that specifies whether a referenced object of this <see cref="IBusinessObjectClass"/> needs to be 
  ///   written back to its container if some of its values have changed.
  /// </summary>
  /// <value> <see langword="true"/> if the <see cref="IBusinessObject"/> must be reassigned to its container. </value>
  /// <example>
  ///   The following pseudo code shows how this value affects the binding behaviour.
  ///   <code><![CDATA[
  ///   Address address = person.Address;
  ///   address.City = "Vienna";
  ///   // the RequiresWriteBack property of the 'Address' business object class specifies 
  ///   // whether the following statement is required:
  ///   person.Address = address;
  ///   ]]></code>
  /// </example>
  bool RequiresWriteBack { get; }

  /// <summary> Gets the identifier (i.e. the type name) for this <see cref="IBusinessObjectClass"/> instance. </summary>
  /// <value> 
  ///   A string that uniquely identifies the <see cref="IBusinessObjectClass"/> instance within the business object 
  ///   model. 
  /// </value>
  string Identifier { get; }
}

/// <summary>
///   The <b>IBusinessObjectClassWithIdentity</b> interface provides functionality for defining the <b>Class</b> of an 
///   <see cref="IBusinessObjectWithIdentity"/>. 
/// </summary>
/// <remarks>
///   The <b>IBusinessObjectClassWithIdentity</b> interface provides additional funcitonality utilizing the
///  <see cref="IBusinessObjectWithIdentity"/>' <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/>.
/// </remarks>
public interface IBusinessObjectClassWithIdentity: IBusinessObjectClass
{
  /// <summary> 
  ///   Looks up and returns the <see cref="IBusinessObjectWithIdentity"/> identified by the 
  ///   <paramref name="uniqueIdentifier"/>.
  /// </summary>
  /// <param name="uniqueIdentifier"> 
  ///   A <see cref="String"/> representing the <b>ID</b> of an <see cref="IBusinessObjectWithIdentity"/>.
  /// </param>
  /// <returns> 
  ///   An <see cref="IBusinessObjectWithIdentity"/> or <see langword="null"/> if the specified object was not found. 
  /// </returns>
  IBusinessObjectWithIdentity GetObject (string uniqueIdentifier);
}

}
