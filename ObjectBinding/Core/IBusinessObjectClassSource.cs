using System;
using System.ComponentModel;

namespace Rubicon.ObjectBinding
{

/// <summary>
///   This interface is used for the <see cref="ITypeDescriptorContext.Instance"/> argument of the VS.NET designer editor. 
/// </summary>
/// <remarks>
///   The PropertyPathPicker control uses this interface to query the business object class of the reference property.
///   Implemented by Rubicon.ObjectBinding.Web.Controls.BocSimpleColumnDefinition and Rubicon.ObjectBinding.Web.Controls.PropertyPathBinding.
/// </remarks>
public interface IReferencePropertySource
{
  IBusinessObjectReferenceProperty ReferenceProperty { get; }
}

}
