using System;
using System.ComponentModel;

namespace Rubicon.ObjectBinding
{

/// <summary>
///   Provides functionality for binding an <see cref="IComponent"/> to an <see cref="IBusinessObject"/> using
///   an <see cref="IBusinessObjectDataSource"/>. 
/// </summary>
/// <remarks>
///   See <see cref="LoadValue"/> for a description of the data binding process.
/// </remarks>
/// <seealso cref="IBusinessObjectBoundModifiableControl"/>
/// <seealso cref="IBusinessObjectDataSource"/>
public interface IBusinessObjectBoundControl: IComponent
{
  /// <summary>
  ///   Gets or sets the <see cref="DataSource"/> providing the <see cref="IBusinessObject"/> to which this
  ///   <see cref="IBusinessObjectBoundControl"/> is bound.
  /// </summary>
  /// <value> An <see cref="IBusinessObjectDataSource"/> providing the current <see cref="IBusinessObject"/>. </value>
  IBusinessObjectDataSource DataSource { get; set; }

  /// <summary>
  ///   Gets or sets the string representation of the <see cref="Property"/>.
  /// </summary>
  /// <value> 
  ///   A string that can be used to query the <see cref="IBusinessObjectClass.GetPropertyDefinition"/> method for
  ///   the <see cref="IBusinessObjectProperty"/>. 
  /// </value>
  string PropertyIdentifier { get; set; }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObjectProperty"/> used for accessing the data to be loaded into 
  ///   <see cref="Value"/>.
  /// </summary>
  /// <value> 
  ///   An <see cref="IBusinessObjectProperty"/> that is part of the bound <see cref="IBusinessObject"/>'s
  ///   <see cref="IBusinessObjectClass"/>
  /// </value>
  IBusinessObjectProperty Property { get; set; }

  /// <summary>
  ///   Gets or sets the value provided by the <see cref="IBusinessObjectBoundControl"/>.
  /// </summary>
  /// <value> An object or boxed value. </value>
  object Value { get; set; }

  /// <summary> 
  ///   Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. 
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     The <see cref="IBusinessObjectBoundControl"/> is bound to an <see cref="IBusinessObjectDataSource"/>
  ///     and an <see cref="IBusinessObjectProperty"/>. When <see cref="LoadValue"/> is executed, the 
  ///     <see cref="Property"/> is used get the value from the <see cref="IBusinessObject"/> provided by the 
  ///     <see cref="DataSource"/>. This object is then used to populate <see cref="Value"/>.
  ///   </para><para>
  ///     This method is usually called by 
  ///     <see cref="IBusinessObjectDataSource.LoadValues">IBusinessObjectDataSource.LoadValues</see>.
  ///   </para>
  ///   <para>
  ///     For details on <b>LoadValue</b>, 
  ///     see <see cref="IBusinessObjectDataSource.LoadValues">IBusinessObjectDataSource.LoadValues</see>.
  ///   </para>
  /// </remarks>
  /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
  void LoadValue (bool interim);

  /// <summary>
  ///   Tests whether the <see cref="IBusinessObjectBoundControl"/> can be bound to the <paramref name="property"/>.
  /// </summary>
  /// <param name="property"> The <see cref="IBusinessObjectProperty"/> to be testet.</param>
  /// <returns>
  ///   <see langword="true"/> if the <see cref="IBusinessObjectBoundControl"/> can be bound to the
  ///   <paramref name="property"/>.
  /// </returns>
  bool SupportsProperty (IBusinessObjectProperty property);

  /// <summary>
  ///   Gets a flag specifying whether the <see cref="IBusinessObjectBoundControl"/> has a valid configuration.
  /// </summary>
  /// <remarks>
  ///   The configuration is considered invalid if data binding is configured for a property 
  ///   that is not available for the bound class or object.
  /// </remarks>
  /// <value> <see langword="true"/> if the configuration is valid. </value>
  bool IsValid { get; }
}

/// <summary>
///   Extends an <see cref="IBusinessObjectBoundControl"/> with the option of writing the 
///   <see cref="IBusinessObjectBoundControl.Value"/> back into the bound <see cref="IBusinessObject"/>.
/// </summary>
/// <remarks>
///   See <see cref="SaveValue"/> for a description of the data binding process.
/// </remarks>
/// <seealso cref="IBusinessObjectDataSource"/>
public interface IBusinessObjectBoundModifiableControl: IBusinessObjectBoundControl
{
  /// <summary>
  ///   Saves the <see cref="IBusinessObjectBoundControl.Value"/> back into the bound <see cref="IBusinessObject"/>.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     When <see cref="SaveValue"/> is executed, the object held by <see cref="IBusinessObjectBoundControl.Value"/>
  ///     is written back into the <see cref="IBusinessObject"/> provided by the 
  ///     <see cref="IBusinessObjectBoundControl.DataSource"/>.
  ///   </para><para>
  ///     This method is usually called by 
  ///     <see cref="IBusinessObjectDataSource.SaveValues">IBusinessObjectDataSource.SaveValues</see>.
  ///   </para>
  ///   <para>
  ///     For details on <b>SaveValue</b>, 
  ///     see <see cref="IBusinessObjectDataSource.SaveValues">IBusinessObjectDataSource.SaveValues</see>.
  ///   </para>
  /// </remarks>
  /// <param name="interim"> Specifies whether this is the final saving, or an interim saving. </param>
  void SaveValue (bool interim);
}

}
