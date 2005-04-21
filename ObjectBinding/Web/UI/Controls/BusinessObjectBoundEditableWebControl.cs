using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing.Design;
using Rubicon.NullableValueTypes;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Design;
using Rubicon.ObjectBinding.Web.Design;
using Rubicon.Globalization;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary>
///   Extends an <see cref="IBusinessObjectBoundWebControl"/> with functionality for validating the control's 
///   <see cref="IBusinessObjectBoundControl.Value"/> and writing it back into the bound <see cref="IBusinessObject"/>.
/// </summary>
/// <remarks>
///   <para>
///     See <see cref="IBusinessObjectBoundControl.SaveValue"/> for a description of the data binding process.
///   </para><para>
///     See <see cref="BusinessObjectBoundModifiableWebControl"/> for the abstract default implementation.
///   </para>
/// </remarks>
/// <seealso cref="IBusinessObjectBoundWebControl"/>
/// <seealso cref="IBusinessObjectBoundModifiableControl"/>
/// <seealso cref="IValidatableControl"/>
/// <seealso cref="IBusinessObjectDataSourceControl"/>
public interface IBusinessObjectBoundModifiableWebControl:
  IBusinessObjectBoundWebControl, 
  IBusinessObjectBoundModifiableControl, 
  IValidatableControl
{
  /// <summary>
  ///   <preliminary/>
  ///   Specifies whether the value of the control has been changed on the Client since the last load/save operation.
  /// </summary>
  /// <remarks>
  ///   Initially, the value of <c>IsDirty</c> is <c>true</c>. The value is set to <c>false</c> during loading
  ///   and saving values. Resetting <c>IsDirty</c> during saving is not implemented by all controls.
  /// </remarks>
  // TODO: redesign IsDirty semantics!
  bool IsDirty { get; set; }
}

/// <seealso cref="IBusinessObjectBoundModifiableWebControl"/>
public abstract class BusinessObjectBoundModifiableWebControl:
    BusinessObjectBoundWebControl, IBusinessObjectBoundModifiableWebControl
{
  private NaBooleanEnum _required = NaBooleanEnum.Undefined;
  private NaBooleanEnum _readOnly = NaBooleanEnum.Undefined;
  private TypedArrayList _validators;

  /// <summary> Explicitly specifies whether the value of the control is required. </summary>
  /// <remarks>
  ///   Set this property to <see cref="NaBooleanEnum.Undefined"/> in order to use the default value 
  ///   (see <see cref="IsRequired"/>).
  /// </remarks>
  [Description("Explicitly specifies whether the control is required.")]
  [Category ("Data")]
  [DefaultValue (typeof(NaBooleanEnum), "Undefined")]
  public NaBooleanEnum Required
  {
    get { return _required; }
    set { _required = value; }
  }

  /// <summary> Explicitly specifies whether the control should be displayed in read-only mode. </summary>
  /// <remarks>
  ///   Set this property to <see cref="NaBooleanEnum.Undefined"/> in order to use the default value 
  ///   (see <see cref="IsReadOnly"/>).
  /// </remarks>
  [Description("Explicitly specifies whether the control should be displayed in read-only mode.")]
  [Category ("Data")]
  [DefaultValue (typeof(NaBooleanEnum), "Undefined")]
  public NaBooleanEnum ReadOnly
  {
    get { return _readOnly; }
    set { _readOnly = value; }
  }

  /// <summary>
  ///   <preliminary/>
  ///   Specifies whether the value of the control has been changed on the Client since the last load/save operation.
  /// </summary>
  /// <remarks>
  ///   Initially, the value of <c>IsDirty</c> is <c>true</c>. The value is set to <c>false</c> during loading
  ///   and saving values. Resetting <c>IsDirty</c> during saving is not implemented by all controls.
  /// </remarks>
  // TODO: redesign IsDirty semantics!
  [Browsable(false)]
  public abstract bool IsDirty { get; set; }

  /// <summary>
  ///   Saves the <see cref="IBusinessObjectBoundControl.Value"/> back into the bound <see cref="IBusinessObject"/>.
  /// </summary>
  /// <param name="interim"> Specifies whether this is the final saving, or an interim saving. </param>
  public abstract void SaveValue (bool interim);

  /// <summary>
  ///   Determines whether the control is to be displayed in read-only mode.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     In read-only mode, a <see cref="System.Web.UI.WebControls.Label"/> control is used to display the value.
  ///     Otherwise, a <see cref="System.Web.UI.WebControls.TextBox"/> control is used to display and edit the value.
  ///   </para><para>
  ///     The following rules are used to determine the value of this property:
  ///     <list type="bullet">
  ///       <item>
  ///         If the value of the <see cref="ReadOnly"/> property is not <see cref="NaBooleanEnum.Undefined"/>,
  ///         the value of <see cref="ReadOnly"/> is returned.
  ///       </item>
  ///       <item>
  ///         If the control is bound to an <see cref="IBusinessObjectDataSourceControl"/> and <see cref="Mode"/>
  ///         is set to <see cref="DataSourceMode.Search"/>
  ///       </item>
  ///       <item>
  ///         If the control is bound to an <c>FscObject</c> component and a <see cref="BusinessObjectPropertyPath"/>, 
  ///         and the bound <c>FscObject</c> component's <c>EditMode</c> property is <see langword="false"/>, 
  ///         <see langword="false"/> is returned.
  ///       </item>
  ///       <item>
  ///         If the control is bound, the attributes of the property and the current object's ACL determine which
  ///         value is returned
  ///       </item>
  ///       <item>Otherwise, <see langword="false"/> is returned.</item>
  ///     </list>
  ///   </para>
  /// </remarks>
  [Browsable(false)]
  public virtual bool IsReadOnly
  {
    get
    {
      if (_readOnly != NaBooleanEnum.Undefined)
        return _readOnly == NaBooleanEnum.True;
      //Binding.EvaluateBinding();
      if (DataSource != null && DataSource.Mode == DataSourceMode.Search)
        return false;
      if (Property == null || DataSource == null)
        return false;
      if (! IsDesignMode && DataSource.BusinessObject == null)
        return true;
      if (DataSource.Mode == DataSourceMode.Read)
        return true;
      return Property.IsReadOnly (DataSource.BusinessObject);
    }
  }

  /// <summary>
  ///   Determines whether the control is to be treated as a required value.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     The value of this property is used to decide whether <see cref="BocTextValueValidator"/> controls should 
  ///     create a <see cref="RequiredFieldValidator"/> for this control.
  ///   </para><para>
  ///     The following rules are used to determine the value of this property:
  ///     <list type="bullet">
  ///       <item>If the control is read-only, <see langword="false"/> is returned.</item>
  ///       <item>
  ///         If the <see cref="Required"/> property is not <see cref="NaBooleanEnum.Undefined"/>, 
  ///         the value of <see cref="Required"/> is returned.
  ///       </item>
  ///       <item>
  ///         If the <see cref="Property"/> contains a property defintion with the
  ///         <see cref="IBusinessObjectProperty.Required"/> flag set, <see langword="true"/> is returned. 
  ///       </item>
  ///       <item>Otherwise, <see langword="false"/> is returned.</item>
  ///     </list>
  ///   </para>
  /// </remarks>
  [Browsable(false)]
  public virtual bool IsRequired 
  {
    get 
    {
      if (IsReadOnly)
        return false;
      if (_required != NaBooleanEnum.Undefined)
        return _required == NaBooleanEnum.True;
      //Binding.EvaluateBinding();
      if (Property != null)
        return (bool) Property.IsRequired;
      return false;
    }
  }
 
  public virtual void RegisterValidator (BaseValidator validator)
  {
    if (_validators == null)
      _validators = new TypedArrayList (typeof (BaseValidator));

    _validators.Add (validator);
  }

  public virtual bool Validate ()
  {
    if (_validators == null)
      return true;

    bool isValid = true;
    foreach (BaseValidator validator in _validators)
    {
      validator.Validate();
      isValid &= validator.IsValid;
    }
    return isValid;
  }
}

}