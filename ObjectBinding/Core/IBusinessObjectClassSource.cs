using System;

namespace Rubicon.ObjectBinding
{
/// <summary>
/// Summary description for PropertyPathBinding.
/// </summary>
public interface IPropertyPathBinding
{
  /// <summary> 
  ///   The <see cref="IBusinessObjectDataSource"/> used to evaluate the 
  ///   <see cref="PropertyPathIdentifier"/>. 
  /// </summary>
  IBusinessObjectDataSource DataSource { get; set; }

  /// <summary> 
  ///   The <see cref="BusinessObjectPropertyPath"/> mananged by this 
  ///   <see cref="PropertyPathBinding"/>.
  /// </summary>
  /// <value>
  ///   A <see cref="BusinessObjectPropertyPath"/> or <see langword="null"/> if the 
  ///   <see cref="PropertyPathIdentifier"/> has not been evaluated.
  ///   Must not be assigned <see langword="null"/>.
  /// </value>
  BusinessObjectPropertyPath PropertyPath { get; set; }

  /// <summary> 
  ///   The <see cref="string"/> representing the <see cref="BusinessObjectPropertyPath"/> mananged 
  ///   by this <see cref="PropertyPathBinding"/>.
  /// </summary>
  /// <value> 
  ///   A <see cref="string"/> formatted as a valid property path. 
  ///   Must not be assigned <see langword="null"/> or emtpy.
  /// </value>
  string PropertyPathIdentifier { get; set; }
}
}
