using System;

namespace Rubicon.ObjectBinding
{

/// <summary>
///   An <see cref="IPropertyPathBinding"/> encapsulates the creation of a 
///   <see cref="BusinessObjectPropertyPath"/> from it's string representation and an
///   <see cref="IBusinessObjectDataSource"/>
/// </summary>
public interface IPropertyPathBinding
{
  /// <summary> 
  ///   Gets or sets the <see cref="IBusinessObjectDataSource"/> used to evaluate the 
  ///   <see cref="PropertyPathIdentifier"/>. 
  /// </summary>
  IBusinessObjectDataSource DataSource { get; set; }

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
}

}
