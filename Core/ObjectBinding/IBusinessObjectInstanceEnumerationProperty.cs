using System;

namespace Rubicon.ObjectBinding
{
  /// <summary> 
  ///   The <b>IBusinessObjectInstanceEnumerationProperty</b> interface is used for accessing the values of an 
  ///   enumeration whose values depend on the <see cref="IBusinessObject"/> instance. 
  /// </summary>
  /// <remarks> 
  ///   This property is not restrained to the enumerations derived from the <see cref="Enum"/> type. 
  ///   <note type="inotes">
  ///     The native value must be serializable if this property is to be bound to the 
  ///     <see cref="T:Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue"/> control.
  ///   </note>
  /// </remarks>
  public interface IBusinessObjectInstanceEnumerationProperty: IBusinessObjectEnumerationProperty
  {
    /// <summary> Returns a list of all the enumeration's values for the specified <paramref name="businessObject"/>. </summary>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> used to determine the enum values. </param>
    /// <returns> 
    ///   A list of <see cref="IEnumerationValueInfo"/> objects encapsulating the values defined in the enumeration. 
    /// </returns>
    IEnumerationValueInfo[] GetAllValues (IBusinessObject businessObject);

    /// <summary> 
    ///   Returns a list of the enumeration's values that can be used in the current context
    ///   for the specified <paramref name="businessObject"/>
    /// </summary>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> used to determine the enum values. </param>
    /// <returns> 
    ///   A list of <see cref="IEnumerationValueInfo"/> objects encapsulating the enabled values in the enumeration. 
    /// </returns>
    /// <remarks> CLS type enums do not inherently support the disabling of its values. </remarks>
    IEnumerationValueInfo[] GetEnabledValues (IBusinessObject businessObject);
  }
}