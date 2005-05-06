using System;

namespace Rubicon.ObjectBinding
{

/// <summary> 
///   The <b>IBusinessObject</b> interface provides functionality to get and set the state of a business object.
/// </summary>
/// <remarks>
///   <para>
///     An <b>IBusinessObject</b> knows its <see cref="IBusinessObjectClass"/> through the 
///     <see cref="BusinessObjectClass"/> property.
///   </para><para>
///     Its state can be accessed through a number of get and set methods as well as indexers.
///   </para>
/// </remarks>
public interface IBusinessObject
{
  /// <overloads> Gets the value accessed through the specified property. </overloads>
  /// <summary> Gets the value accessed through the specified <see cref="IBusinessObjectProperty"/>. </summary>
  /// <param name="property"> The <see cref="IBusinessObjectProperty"/> used to access the value. </param>
  /// <returns> An <see cref="Object"/> containing the value. </returns>
  /// <exception type="Exception">
  ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
  /// </exception>
  object GetProperty (IBusinessObjectProperty property);

  /// <summary>
  ///   Gets the value accessed through the <see cref="IBusinessObjectProperty"/> identified by the passed 
  ///   <paramref name="propertyIdentifier"/>. 
  /// </summary>
  /// <param name="propertyIdentifier"> 
  ///   A <see cref="String"/> identifing the <see cref="IBusinessObjectProperty"/> used to access the value. 
  /// </param>
  /// <returns> An <see cref="Object"/> containing the value. </returns>
  /// <exception type="Exception"> 
  ///   Thrown if the <see cref="IBusinessObjectProperty"/> identified through the <paramref name="propertyIdentifier"/>
  ///   is not part of this business object's class. 
  /// </exception>
  object GetProperty (string propertyIdentifier);

  /// <overloads> Sets the value accessed through the specified property. </overloads>
  /// <summary> Sets the value accessed through the specified <see cref="IBusinessObjectProperty"/>. </summary>
  /// <param name="property"> 
  ///   The <see cref="IBusinessObjectProperty"/> used to access the value. Must not be <see langword="null"/>.
  /// </param>
  /// <param name="value"> The <see cref="Object"/> containing the value to be set. </param>
  /// <exception type="Exception"> 
  ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
  /// </exception>
  void SetProperty (IBusinessObjectProperty property, object value);

  /// <summary>
  ///   Sets the value accessed through the <see cref="IBusinessObjectProperty"/> identified by the passed 
  ///   <paramref name="propertyIdentifier"/>. 
  /// </summary>
  /// <param name="propertyIdentifier"> 
  ///   A <see cref="String"/> identifing the <see cref="IBusinessObjectProperty"/> used to access the value. 
  /// </param>
  /// <param name="value"> The <see cref="Object"/> containing the value to be set. </param>
  /// <exception type="Exception"> 
  ///   Thrown if the <see cref="IBusinessObjectProperty"/> identified by the <paramref name="propertyIdentifier"/>
  ///   is not part of this business object's class. 
  /// </exception>
  void SetProperty (string propertyIdentifier, object value);

  /// <summary> Gets or sets the value accessed through the specified <see cref="IBusinessObjectProperty"/>. </summary>
  /// <param name="property"> 
  ///   The <see cref="IBusinessObjectProperty"/> used to access the value. Must not be <see langword="null"/>.
  /// </param>
  /// <value> The <see cref="Object"/> containing the value to be accessed. </value>
  /// <exception type="Exception"> 
  ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
  /// </exception>
  object this [IBusinessObjectProperty property] { get; set; }

  /// <summary> 
  ///   Gets or Sets the value accessed through the <see cref="IBusinessObjectProperty"/> identified by the passed 
  ///   <paramref name="propertyIdentifier"/>. 
  /// </summary>
  /// <param name="propertyIdentifier"> 
  ///   A <see cref="String"/> identifing the <see cref="IBusinessObjectProperty"/> used to access the value. 
  /// </param>
  /// <value> The <see cref="Object"/> containing the value to be accessed. </value>
  /// <exception type="Exception"> 
  ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
  /// </exception>
  object this [string propertyIdentifier] { get; set; }

  /// <overloads> Gets the string representation of the value accessed through the specified property.  </overloads>
  /// <summary> 
  ///   Gets the string representation of the value accessed through the specified 
  ///   <see cref="IBusinessObjectProperty"/>.
  /// </summary>
  /// <param name="property"> 
  ///   The <see cref="IBusinessObjectProperty"/> used to access the value. Must not be <see langword="null"/>.
  /// </param>
  /// <returns> The string representation of the value. </returns>
  /// <exception type="Exception"> 
  ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
  /// </exception>
  string GetPropertyString (IBusinessObjectProperty property);

  /// <summary> 
  ///   Gets the formatted string representation of the value accessed through the specified 
  ///   <see cref="IBusinessObjectProperty"/>.
  /// </summary>
  /// <param name="property"> 
  ///   The <see cref="IBusinessObjectProperty"/> used to access the value. Must not be <see langword="null"/>.
  /// </param>
  /// <param name="format"> The format string applyed by the <b>ToString</b> method. </param>
  /// <returns> The string representation of the value. </returns>
  /// <exception type="Exception"> 
  ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
  /// </exception>
  string GetPropertyString (IBusinessObjectProperty property, string format);

  /// <summary> 
  ///   Gets the string representation of the value accessed through the <see cref="IBusinessObjectProperty"/> 
  ///   identified by the passed <paramref name="propertyIdentifier"/>.
  /// </summary>
  /// <param name="property"> 
  ///   A <see cref="String"/> identifing the <see cref="IBusinessObjectProperty"/> used to access the value. 
  /// </param>
  /// <returns> The string representation of the value. </returns>
  /// <exception type="Exception"> 
  ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
  /// </exception>
  string GetPropertyString (string propertyIdentifier);

  /// <summary> Gets the <see cref="IBusinessObjectClass"/> of this <see cref="IBusinessObject"/>. </summary>
  /// <value>
  ///   An <see cref="IBusinessObjectClass"/> instance acting as the <see cref="IBusinessObject"/> instance's type.
  /// </value>
  IBusinessObjectClass BusinessObjectClass { get; }
}

/// <summary>
///   The <b>IBusinessObjectWithIdentity</b> interface provides functionality to uniquely identify a business object 
///   within its business object domain.
/// </summary>
public interface IBusinessObjectWithIdentity : IBusinessObject
{
  /// <summary> Gets the human readable <b>ID</b> of this <see cref="IBusinessObjectWithIdentity"/>. </summary>
  /// <value> A <see cref="String"/> identifying this object to the user. </value>
  /// <remarks> This value does not have to be unqiue within its business object domain. </remarks>
  string DisplayName { get; }

  /// <summary> Gets the programmatic <b>ID</b> of this <see cref="IBusinessObjectWithIdentity"/> </summary>
  /// <value> A <see cref="String"/> uniquely identifying this object. </value>
  /// <remarks> This value must be be unqiue within its business object domain. </remarks>
  string UniqueIdentifier { get; }
}

}
