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
///     See <see cref="IBusinessObjectBoundModifiableControl.SaveValue"/> for a description of the data binding 
///     process.
///   </para><para>
///     See <see cref="BusinessObjectBoundModifiableWebControl"/> for the <see langword="abstract"/> default 
///     implementation.
///   </para>
/// </remarks>
/// <seealso cref="IBusinessObjectBoundWebControl"/>
/// <seealso cref="IBusinessObjectBoundModifiableControl"/>
/// <seealso cref="IValidatableControl"/>
/// <seealso cref="IBusinessObjectDataSourceControl"/>
public interface IBusinessObjectBoundModifiableWebControl:
  IBusinessObjectBoundWebControl, 
  IBusinessObjectBoundModifiableControl, 
  IValidatableControl, 
  IModifiableControl
{
}

/// <summary>
///   <b>BusinessObjectBoundModifiableWebControl</b> is the <see langword="abstract"/> default implementation of 
///   <see cref="IBusinessObjectBoundModifiableWebControl"/>.
/// </summary>
/// <seealso cref="IBusinessObjectBoundModifiableWebControl"/>
public abstract class BusinessObjectBoundModifiableWebControl:
    BusinessObjectBoundWebControl, IBusinessObjectBoundModifiableWebControl
{
  private NaBooleanEnum _required = NaBooleanEnum.Undefined;
  private NaBooleanEnum _readOnly = NaBooleanEnum.Undefined;
  private TypedArrayList _validators;
  private bool _isDirty = false;

  protected override void OnInit (EventArgs e)
  {
    base.OnInit (e);
    if (Page is ISmartPage)
      ((ISmartPage) Page).RegisterForDirtyStateTracking (this);
  }

  /// <summary> Gets or sets a flag that specifies whether the value of the control is required. </summary>
  /// <remarks>
  ///   Set this property to <see cref="NaBooleanEnum.Undefined"/> in order to use the default value 
  ///   (see <see cref="IsRequired"/>).
  /// </remarks>
  [Description("Explicitly specifies whether the control is required.")]
  [Category ("Data")]
  [DefaultValue (NaBooleanEnum.Undefined)]
  public NaBooleanEnum Required
  {
    get { return _required; }
    set { _required = value; }
  }

  /// <summary> Gets or sets a flag that specifies whether the control should be displayed in read-only mode. </summary>
  /// <remarks>
  ///   Set this property to <see cref="NaBooleanEnum.Undefined"/> in order to use the default value 
  ///   (see <see cref="IsReadOnly"/>). Note that if the data source is in read-only mode, the
  ///   control is read-only too, even if this property is set to <c>false</c>.
  /// </remarks>
  [Description("Explicitly specifies whether the control should be displayed in read-only mode.")]
  [Category ("Data")]
  [DefaultValue (NaBooleanEnum.Undefined)]
  public NaBooleanEnum ReadOnly
  {
    get { return _readOnly; }
    set { _readOnly = value; }
  }

  /// <summary>
  ///   Gets or sets the dirty flag.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     Initially, the <see cref="IsDirty"/> flag is <see langword="false"/>. It is reset to <see langword="false"/>
  ///     when the <see cref="BusinessObjectBoundWebControl.Value"/> is set. The dirty flag is also cleared when 
  ///     using <see cref="BusinessObjectBoundWebControl.LoadValue"/> to read the value from, or using
  ///     <see cref="SaveValue"/> to write the value back into the <see cref="IBusinessObject"/> bound to the 
  ///     <see cref="BusinessObjectBoundWebControl.DataSource"/>
  ///   </para><para>
  ///     It is set to <see langword="true"/> when the control's <see cref="BusinessObjectBoundWebControl.Value"/> 
  ///     is changed by the control after it has been set. This can happen either when the user submits new data in
  ///     the user interface or through the application using the control to modify the contents of the 
  ///     <see cref="Value"/>. (E.g. a row is added to the list of values by invoking a method on the 
  ///     <see cref="BocList"/>.)
  ///   </para><para>
  ///     If the control is used unbound, it is the application developers responsibility to reset the dirty flag 
  ///     after reading the value from the control.
  ///   </para>
  /// </remarks>
  [Browsable (false)]
  public virtual bool IsDirty
  {
    get { return _isDirty; }
    set { _isDirty = value; }
  }

  /// <summary> Returns the <see cref="ClientID"/> values of all controls containing modifiable data. </summary>
  /// <returns> A string arry containing zero or more client ids. </returns>
  public abstract string[] GetTrackedClientIDs();

  /// <summary>
  ///   Saves the <see cref="IBusinessObjectBoundControl.Value"/> back into the bound <see cref="IBusinessObject"/>.
  /// </summary>
  /// <param name="interim"> Specifies whether this is the final saving, or an interim saving. </param>
  public abstract void SaveValue (bool interim);

  /// <summary>
  ///   Gets a flag that determines whether the control is to be displayed in read-only mode.
  /// </summary>
  /// <remarks>
  ///     In read-only mode, a <see cref="System.Web.UI.WebControls.Label"/> control is used to display the value.
  ///     Otherwise, a <see cref="System.Web.UI.WebControls.TextBox"/> control is used to display and edit the value.
  /// </remarks>
  /// <value>
  ///   <list type="bullet">
  ///     <item>
  ///       Whether the control is bound or unbound, if the value of the <see cref="ReadOnly"/> property is 
  ///       <see cref="NaBooleanEnum.True"/>, <see langword="true"/> is returned.
  ///     </item>
  ///     <item>
  ///       If the control is bound to an <see cref="IBusinessObjectDataSourceControl"/> and 
  ///       <see cref="IBusinessObjectDataSource.Mode">DataSource.Mode</see> is set to 
  ///       <see cref="DataSourceMode.Search"/>, <see langword="false"/> is returned.
  ///     </item>
  ///     <item>
  ///       If the control is unbound (<see cref="BusinessObjectBoundWebControl.DataSource"/> or 
  ///       <see cref="BusinessObjectBoundWebControl.Property"/> is <see langword="null"/>) and the
  ///       <see cref="ReadOnly"/> property is not <see cref="NaBooleanEnum.True"/>, 
  ///       <see langword="false"/> is returned.
  ///     </item>
  ///     <item>
  ///       If the control is bound (<see cref="BusinessObjectBoundWebControl.DataSource"/> and  
  ///       <see cref="BusinessObjectBoundWebControl.Property"/> are not <see langword="null"/>), 
  ///       the following rules are used to determine the value of this property:
  ///       <list type="bullet">
  ///         <item>
  ///           If the <see cref="IBusinessObjectDataSource.Mode">DataSource.Mode</see> of the control's
  ///           <see cref="BusinessObjectBoundWebControl.DataSource"/> is set to <see cref="DataSourceMode.Read"/>, 
  ///           <see langword="true"/> is returned.
  ///         </item>
  ///         <item>
  ///           If the <see cref="IBusinessObjectDataSource.BusinessObject">DataSource.BusinessObject</see> is 
  ///           <see langword="null"/> and the control is not in <b>Design Mode</b>, 
  ///           <see langword="true"/> is returned.
  ///         </item>
  ///         <item>
  ///           If the control's <see cref="ReadOnly"/> property is <see cref="NaBooleanEnum.False"/>, 
  ///           <see langword="false"/> is returned.
  ///         </item>
  ///         <item>
  ///           Otherwise, <see langword="Property.IsReadOnly"/> is evaluated and returned.
  ///         </item>
  ///       </list>
  ///     </item>
  ///   </list>
  /// </value>
  [Browsable(false)]
  public virtual bool IsReadOnly
  {
    get
    {
      if (_readOnly == NaBooleanEnum.True) // (Bound Control || Unbound Control) && ReadOnly==true
        return true;
      if (DataSource != null && DataSource.Mode == DataSourceMode.Search) // Search DataSource 
        return false;
      if (Property == null || DataSource == null) // Unbound Control && (ReadOnly==false || ReadOnly==undefined)
        return false;
      if (DataSource.Mode == DataSourceMode.Read) // Bound Control && Reader DataSource
        return true;
      if (! IsDesignMode && DataSource.BusinessObject == null) // Bound Control but no BusinessObject
        return true;
      if (_readOnly == NaBooleanEnum.False) // Bound Control && ReadOnly==false
        return false;
      return Property.IsReadOnly (DataSource.BusinessObject); // ReadOnly==undefined: ObjectModel pulls
    }
  }

  /// <summary>
  ///   Gets a flag that determines whether the control is to be treated as a required value.
  /// </summary>
  /// <remarks>
  ///     The value of this property is used to decide whether <see cref="CreateValidators"/> should 
  ///     include a <see cref="RequiredFieldValidator"/> for this control.
  /// </remarks>
  /// <value>
  ///   The following rules are used to determine the value of this property:
  ///   <list type="bullet">
  ///     <item>If the control is read-only, <see langword="false"/> is returned.</item>
  ///     <item>
  ///       If the <see cref="Required"/> property is not <see cref="NaBooleanEnum.Undefined"/>, 
  ///       the value of <see cref="Required"/> is returned.
  ///     </item>
  ///     <item>
  ///       If the <see cref="BusinessObjectBoundWebControl.Property"/> contains a property definition with the
  ///       <see cref="IBusinessObjectProperty.IsRequired"/> flag set, <see langword="true"/> is returned. 
  ///     </item>
  ///     <item>Otherwise, <see langword="false"/> is returned.</item>
  ///   </list>
  /// </value>
  [Browsable(false)]
  public virtual bool IsRequired 
  {
    get 
    {
      if (IsReadOnly)
        return false;
      if (_required != NaBooleanEnum.Undefined)
        return _required == NaBooleanEnum.True;
      if (Property != null)
        return (bool) Property.IsRequired;
      return false;
    }
  }
 
  /// <summary> Creates the list of validators required for the current binding and property settings. </summary>
  /// <returns> An (empty) array of <see cref="BaseValidator"/> controls. </returns>
  public virtual BaseValidator[] CreateValidators()
  {
    return new BaseValidator[0];
  }

  /// <summary> Registers a validator that references this control. </summary>
  /// <remarks> 
  ///   <para>
  ///     The control may choose to ignore this call. 
  ///   </para><para>
  ///     The registered validators are evaluated when <see cref="Validate"/> is called.
  ///   </para>
  /// </remarks>
  public virtual void RegisterValidator (BaseValidator validator)
  {
    if (_validators == null)
      _validators = new TypedArrayList (typeof (BaseValidator));

    _validators.Add (validator);
  }

  /// <summary> Calls <see cref="BaseValidator.Validate"/> on all registered validators. </summary>
  /// <returns> <see langword="true"/>, if all validators validated. </returns>
  public virtual bool Validate()
  {
    if (_validators == null)
      return true;

    bool isValid = true;
    for (int i = 0; i < _validators.Count; i++)
    {
      BaseValidator validator = (BaseValidator) _validators[i];
      validator.Validate();
      isValid &= validator.IsValid;
    }
    return isValid;
  }

  /// <summary> Overrides the <see cref="Control.LoadViewState"/> method. </summary>
  protected override void LoadViewState(object savedState)
  {
    object[] values = (object[]) savedState;
    base.LoadViewState (values[0]);
    _isDirty = (bool)  values[1];
  }

  /// <summary> Overrides the <see cref="Control.SaveViewState"/> method. </summary>
  protected override object SaveViewState()
  {
    object[] values = new object[2];
    values[0] = base.SaveViewState();
    values[1] = _isDirty;
    return values;
  }
}

}