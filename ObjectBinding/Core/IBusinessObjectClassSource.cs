using System;
using System.ComponentModel;

namespace Rubicon.ObjectBinding
{

/// <summary>
///   This interface is used for the <see cref="ITypeDescriptorContext.Instance"/> argument of the 
///   Visual Studio .NET designer editor. 
/// </summary>
/// <remarks>
///   <para>
///     The <see cref="T:Rubicon.ObjectBinding.Design.PropertyPathPicker"/> control uses this interface 
///     to query the <see cref="IBusinessObjectClass"/> of an <see cref="IBusinessObjectReferenceProperty"/>
///     or an <see cref="IBusinessObjectDataSource"/>, respectively.
///   </para><para>
///     Implemented by <see cref="T:Rubicon.ObjectBinding.Web.Controls.BocSimpleColumnDefinition"/> 
///     and <see cref="T:Rubicon.ObjectBinding.Web.Controls.PropertyPathBinding"/>.
///   </para>
/// </remarks>
public interface IBusinessObjectClassSource
{
  /// <summary>
  ///   Gets the <see cref="IBusinessObjectClass"/> of an <see cref="IBusinessObjectReferenceProperty"/>
  ///   or an <see cref="IBusinessObjectDataSource"/>, respectively.
  /// </summary>
  /// <value> 
  ///   The <see cref="IBusinessObjectClass"/> to be queried for the properties offered by the 
  ///   <see cref="T:Rubicon.ObjectBinding.Design.PropertyPathPicker"/> control.
  /// </value>
  IBusinessObjectClass BusinessObjectClass { get; }
}

}
