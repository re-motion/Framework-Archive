using System;
using System.CodeDom;
using System.Collections;
using System.Globalization;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Drawing.Design;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Design;
using Rubicon.Web.UI.Controls;

namespace Rubicon.ObjectBinding.Web.Controls
{

public interface IBusinessObjectBoundWebControl: 
  IBusinessObjectBoundControl, 
  ISmartControl,
  IDataBindingsAccessor, IParserAccessor // these interfaces are implemented by System.Web.UI.Control
{
}

public interface IBusinessObjectBoundModifiableWebControl: IBusinessObjectBoundWebControl, IBusinessObjectBoundModifiableControl
{
}

/// <summary>
/// Provides a GUI designer for BusinessObjectBoundControl
/// </summary>
[Designer (typeof (BusinessObjectBoundControlDesigner))]
public abstract class BusinessObjectBoundWebControl: WebControl, IBusinessObjectBoundWebControl
{
  #region IBusinessObjectBoundControl implementation
  private BusinessObjectBinding _binding;

  [Browsable(false)]
  public BusinessObjectBinding Binding
  {
    get { return _binding; }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Category ("Data")]
  //[TypeConverter (typeof (BusinessObjectDataSourceObjectConverter))]
  public IBusinessObjectDataSource DataSource
  {
    get { return _binding.DataSource; }
    set 
    { 
//      IBusinessObjectDataSource _previousDataSource = _binding.DataSource;
      _binding.DataSource = value; 
//      OnBindingChanged (Property, _previousDataSource);
    }
  }

  [Category ("Data")]
  [Editor (typeof (PropertyPathPickerEditor), typeof (UITypeEditor))]
  public string PropertyIdentifier
  {
    get { return _binding.PropertyIdentifier; }
    set 
    { 
//      IBusinessObjectProperty previousProperty = _binding.Property;
      _binding.PropertyIdentifier = value; 
//      OnBindingChanged (previousProperty, DataSource);
    }
  }

  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObjectProperty Property
  {
    get { return _binding.Property; }
    set 
    { 
//      IBusinessObjectProperty previousProperty = _binding.Property;
      _binding.Property = value; 
//      OnBindingChanged (previousProperty, DataSource);
    }
  }
  #endregion

//  /// <summary>
//  ///   Occurs after either the <see cref="Property"/> property or the <see cref="PropertyIdentifier"/> property is assigned a new value.
//  /// </summary>
//  /// <remarks>
//  ///   Note that this event does not occur if the property path is modified, only if a new one is assigned.
//  /// </remarks>
//  public event BindingChangedEventHandler BindingChanged;

  private bool _childControlsInitialized = false;
//  private bool _onLoadCalled = false;
//  private bool _propertyBindingChangedBeforeOnLoad = false;

  public BusinessObjectBoundWebControl ()
  {
    _binding = new BusinessObjectBinding (this);
  }

//  protected override void OnLoad (EventArgs e)
//  {
//    base.OnLoad (e);
//    _onLoadCalled = true;
//    if (_propertyBindingChangedBeforeOnLoad)
//      OnBindingChanged (null, null);
//  }

//  /// <summary>
//  /// Raises the <see cref="PropertyChanged"/> event.
//  /// </summary>
//  protected virtual void OnBindingChanged (IBusinessObjectProperty previousProperty, IBusinessObjectDataSource previousDataSource)
//  {
//    if (! _onLoadCalled)
//    {
//      _propertyBindingChangedBeforeOnLoad = true;
//      return;
//    }
//    if (BindingChanged != null)
//      BindingChanged (this, new BindingChangedEventArgs (previousProperty, previousDataSource));
//  }

  /// <summary>
  ///   Determines whether the control is to be treated as a required value.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     The value of this value is used to decide whether <see cref="BocTextValueValidator"/> controls should create a 
  ///     <see cref="RequiredFieldValidator"/> for this control.
  ///   </para><para>
  ///     The following rules are used to determine the value of this property:
  ///     <list type="bullets">
  ///       <item>If the value of the property <see cref="Required"/> is not <c>Undefined</c>, this value is returned.</item>
  ///       <item>If the property <see cref="PropertyPath"/> contains a property definition with the <c>Must be defined</c> 
  ///       flag set, <c>true</c> is returned. </item>
  ///       <item>Otherwise, <c>false is returned.</c></item>
  ///     </list>
  ///   </para>
  /// </remarks>
  [Browsable(false)]
  public virtual bool IsRequired 
  {
    get { return false; }
  }

  /// <summary>
  ///   Determines whether the control is to be displayed in read-only mode.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     In read-only mode, a <see cref="System.Web.UI.WebControls.Label"/> control is used to display the value. Otherwise,
  ///     a <see cref="System.Web.UI.WebControls.TextBox"/> control is used to display and edit the value.
  ///   </para><para>
  ///     The following rules are used to determine the value of this property:
  ///     <list type="bullets">
  ///       <item>If the value of the property <see cref="ReadOnly"/> is not <c>Undefined</c>, this value is returned.</item>
  ///       <item>If the control is bound to a <see cref="FscObject"/> component and a <see cref="PropertyPath"/>, and the bound
  ///         <see cref="FscObject"/> component's <c>EditMode</c> property is <c>false</c>, <c>false</c> 
  ///         is returned.</item>
  ///       <item>If the control is bound, the attributes of the property and the current object's ACL determine which
  ///         value is returned</item>
  ///       <item>Otherwise, <c>false</c> is returned.</item>
  ///     </list>
  ///   </para>
  /// </remarks>
  [Browsable(false)]
  public virtual bool IsReadOnly
  {
    get { return true; }
  }

  public abstract void LoadValue ();
  
  [Browsable (false)]
  public abstract object Value { get; set; }

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);
    // EnsureChildControlsInitialized();
  }

  /// <summary>
  ///   Calls <see cref="InitializeChildControls"/> on the first invocation.
  /// </summary>
  /// <remarks>
  ///   In some situations, this method gets invoked more than once in the VS.NET designer.
  /// </remarks>
  protected void EnsureChildControlsInitialized ()
  {
    if (! _childControlsInitialized || (this.Site != null && this.Site.DesignMode))
    {
      InitializeChildControls();
      _childControlsInitialized = true;
    }
  }

  /// <summary>
  ///   Override this method to initialize child controls.
  /// </summary>
  /// <remarks>
  ///   Child controls that do not need to be created before handling post data can be created in this method.
  ///   Use <see cref="EnsureChildControlsInitialized"/> to call this method.
  /// </remarks>
  protected virtual void InitializeChildControls ()
  {
  }

  [Browsable(false)]
  public abstract bool IsDirty { get; set; }

  [Browsable(false)]
  public abstract Type[] SupportedPropertyInterfaces { get; }

  public virtual BaseValidator[] CreateValidators()
  {
    return new BaseValidator[0];
  }

  [Browsable(false)]
  public virtual string DisplayName 
  {
    get 
    {
      Binding.EvaluateBinding();
      return (Property != null) ? Property.DisplayName : null;
    }
  }

  [Browsable(false)]
  public virtual string HelpUrl
  {
    get { return null; }
  }

  [Browsable(false)]
  public virtual Control TargetControl
  {
    get { return this; }
  }

  [Browsable(false)]
  public virtual bool UseLabel
  {
    get { return ! (TargetControl is DropDownList || TargetControl is HtmlSelect); }
  }
}

public abstract class BusinessObjectBoundModifiableWebControl: BusinessObjectBoundWebControl, IBusinessObjectBoundModifiableWebControl
{
  private NaBooleanEnum _required = NaBooleanEnum.Undefined;
  private NaBooleanEnum _readOnly = NaBooleanEnum.Undefined;

  /// <summary>
  ///   Explicitly specifies whether the control is required.
  /// </summary>
  /// <remarks>
  ///   Set this property to <c>Unspecified</c> in order to use the default value (see <see cref="IsRequired"/>).
  /// </remarks>
  [Description("Explicitly specifies whether the control is required.")]
  [Category("Data")]
  [DefaultValue (typeof(NaBooleanEnum), "Undefined")]
  public NaBooleanEnum Required
  {
    get { return _required; }
    set { _required = value; }
  }

  /// <summary>
  ///   Explicitly specifies whether the control should be displayed in read-only mode.
  /// </summary>
  /// <remarks>
  ///   Set this property to <c>Unspecified</c> in order to use the default value (see <see cref="IsReadOnly"/>).
  /// </remarks>
  [Description("Explicitly specifies whether the control should be displayed in read-only mode.")]
  [Category("Data")]
  [DefaultValue (typeof(NaBooleanEnum), "Undefined")]
  public NaBooleanEnum ReadOnly
  {
    get { return _readOnly; }
    set { _readOnly = value; }
  }

  public abstract void SaveValue ();

  public override bool IsReadOnly
  {
    get
    {
      if (_readOnly != NaBooleanEnum.Undefined)
        return _readOnly == NaBooleanEnum.True;
      Binding.EvaluateBinding();
      if (Property == null || DataSource == null)
        return false;
      if (! DataSource.IsWritable)
        return true;
      return Property.IsReadOnly (DataSource.BusinessObject);
    }
  }

  public override bool IsRequired
  {
    get 
    {
      if (_required != NaBooleanEnum.Undefined)
        return _required == NaBooleanEnum.True;
      Binding.EvaluateBinding();
      if (Property != null && ! Property.IsRequired.IsNull)
        return (bool) Property.IsRequired;
      return false;
    }
  }
}

///// <summary>
/////   Provides data for the <cBindingChanged</c> event.
/////   <seealso cref="BusinessObjectBoundControl.BindingChanged"/>
///// </summary>
//public class BindingChangedEventArgs: EventArgs
//{
//  /// <summary>
//  ///   The value of the <c>PropertyPath</c> property before the change took place.
//  /// </summary>
//  public readonly IBusinessObjectProperty PreviousProperty;
//  public readonly IBusinessObjectDataSource PreviuosDataSource;
//
//  public BindingChangedEventArgs (IBusinessObjectProperty previousProperty, IBusinessObjectDataSource previousDataSource)
//  {
//    PreviousProperty = previousProperty;
//    PreviuosDataSource = previousDataSource;
//  }
//}
//
//public delegate void BindingChangedEventHandler (object sender, BindingChangedEventArgs e);

}
