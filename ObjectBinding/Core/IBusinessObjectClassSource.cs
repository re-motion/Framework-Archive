using System;

namespace Rubicon.ObjectBinding
{

/// <summary>
///   An <see cref="IPropertyPathBinding"/> encapsulates the creation of a 
///   <see cref="BusinessObjectPropertyPath"/> from its string representation and an
///   <see cref="IBusinessObjectDataSource"/>
/// </summary>
public interface IPropertyPathBinding
{
  /// <summary> 
  ///   Gets or sets the <see cref="IBusinessObjectDataSource"/> used to evaluate the 
  ///   <see cref="PropertyPathIdentifier"/>. 
  /// </summary>
  IBusinessObjectDataSource DataSource { get; }

  // TODO: weg damit?
//  /// <summary>
//  ///   Gets the object class of the parent object.
//  /// </summary>
//  IBusinessObjectClass BusinessObjectClass { get; }

  /// <summary> 
  ///   Gets or sets the <see cref="BusinessObjectPropertyPath"/> mananged by this 
  ///   <see cref="IPropertyPathBinding"/>.
  /// </summary>
  /// <value>
  ///   A <see cref="BusinessObjectPropertyPath"/> or <see langword="null"/> if the 
  ///   <see cref="PropertyPathIdentifier"/> has not been evaluated.
  ///   Must not be assigned <see langword="null"/>.
  /// </value>
  BusinessObjectPropertyPath PropertyPath { get; set; }

  /// <summary> 
  ///   Gets or sets the <see cref="string"/> representing the 
  ///   <see cref="BusinessObjectPropertyPath"/> mananged by this <see cref="IPropertyPathBinding"/>.
  /// </summary>
  /// <value> 
  ///   A <see cref="string"/> formatted as a valid property path. 
  ///   Must not be assigned <see langword="null"/> or emtpy.
  /// </value>
  string PropertyPathIdentifier { get; set; }


  // TODO: anschaun ob ok
  IBusinessObjectBoundControl OwnerControl { get; }
}

}
