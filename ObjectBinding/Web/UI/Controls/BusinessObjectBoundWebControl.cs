using System;
using System.CodeDom;
using System.Collections;
using System.Globalization;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Drawing.Design;
using Rubicon.NullableValueTypes;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Design;
using Rubicon.ObjectBinding.Web.Design;

namespace Rubicon.ObjectBinding.Web.Controls
{

public interface IBusinessObjectBoundWebControl: 
  IBusinessObjectBoundControl, 
  ISmartControl,
  IControl
{
  string DataSourceControl { get; set; }
}

public interface IBusinessObjectBoundModifiableWebControl: IBusinessObjectBoundWebControl, IBusinessObjectBoundModifiableControl
{
}

/// <summary>
/// Provides a GUI designer for BusinessObjectBoundControl
/// </summary>
[Designer (typeof (BocDesigner))]
public abstract class BusinessObjectBoundWebControl: WebControl, IBusinessObjectBoundWebControl
{
  #region IBusinessObjectBoundControl implementation

  [Browsable(false)]
  public BusinessObjectBinding Binding
  {
    get 
    { 
      return _binding;
    }
  }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObjectDataSource"/> this 
  ///   <see cref="IBusinessObjectBoundWebControl"/> is bound to.
  /// </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public IBusinessObjectDataSource DataSource
  {
    get { return _binding.DataSource; }
    set { _binding.DataSource = value; }
  }

  [Category ("Data")]
  [Editor (typeof (PropertyPickerEditor), typeof (UITypeEditor))]
  public string PropertyIdentifier
  {
    get { return _binding.PropertyIdentifier; }
    set { _binding.PropertyIdentifier = value; }
  }

  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObjectProperty Property
  {
    get { return _binding.Property; }
    set { _binding.Property = value; }
  }

  /// <summary>
  ///   Gets or sets the ID of the <see cref="IBusinessObjectDataSourceControl"/> 
  ///   encapsulating the <see cref="IBusinessObjectDataSource"/> this 
  ///   <see cref="IBusinessObjectBoundWebControl"/> is bound to.
  /// </summary>
  [TypeConverter (typeof (BusinessObjectDataSourceControlConverter))]
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Data")]
  [Description ("The ID of the BusinessObjectDataSourceControl control used as data source.")]
  [DefaultValue ("")]
  public string DataSourceControl
  {
    get { return _binding.DataSourceControl; }
    set { _binding.DataSourceControl = value; }
  }
  #endregion

  private BusinessObjectBinding _binding;

  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
    _binding.EnsureDataSource();
  }


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

  public BusinessObjectBoundWebControl ()
  {
    _binding = new BusinessObjectBinding (this);
  }

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
  ///       <item>If the control is read-only, false is returned.</item>
  ///       <item>If the value of the property <c>Required</c> is not <see cref="NaBooleanEnum.Undefined"/>, this value is returned.</item>
  ///       <item>If the property <see cref="Property"/> contains a property definition with the <c>Required</c> 
  ///       flag set, <c>true</c> is returned. </item>
  ///       <item>Otherwise, <c>false is returned.</c></item>
  ///     </list>..
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
  ///       <item>If the value of the property <c>ReadOnly</c> is not <c>Undefined</c>, this value is returned.</item>
  ///       <item>If the control is bound to a <c>FscObject</c> component and a <c>PropertyPath</c>, and the bound
  ///         <c>FscObject</c> component's <c>EditMode</c> property is <c>false</c>, <c>false</c> 
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

  public abstract void LoadValue (bool interim);
  
  /// <remarks>
  ///   Override <see cref="ValueImplementation"/> to define the behaviour of <c>Value</c>. 
  ///   Redefine <c>Value</c> using the keyword <c>new</c> (<c>Shadows</c> in Visual Basic) 
  ///   to provide a typesafe implementation in derived classes.
  /// </remarks>
  [Browsable (false)]
  public object Value 
  { 
    get { return ValueImplementation; }
    set { ValueImplementation = value; }
  }

  [Browsable (false)]
  protected abstract object ValueImplementation { get; set; }

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);
    //  Best left to specializations
    // EnsureChildControlsInitialized();
  }

  /// <summary>
  ///   Calls <see cref="InitializeChildControls"/> on the first invocation.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     In some situations, this method gets invoked more than once in the VS.NET designer.
  ///   </para><para>
  ///     Best practice: call once in <c>OnPreRender</c> method and once in <c>Render</c> method.
  ///   </para>
  /// </remarks>
  protected void EnsureChildControlsInitialized()
  {
    if (! _childControlsInitialized || IsDesignMode)
    {
      InitializeChildControls();
      _childControlsInitialized = true;
    }
  }

  /// <summary>
  ///   Override this method to initialize child controls.
  /// </summary>
  /// <remarks>
  ///   Child controls that do not need to be created before handling post data can be created
  ///   in this method. Use <see cref="EnsureChildControlsInitialized"/> to call this method.
  /// </remarks>
  protected virtual void InitializeChildControls ()
  {
  }

  [Browsable(false)]
  public abstract bool IsDirty { get; set; }


  public virtual bool SupportsProperty (IBusinessObjectProperty property)
  {
    if (SupportedPropertyInterfaces == null)
      return true;
    if (! SupportsPropertyMultiplicity (property.IsList))
      return false;

    bool isSupportedPropertyType = false;
    foreach (Type supportedInterface in SupportedPropertyInterfaces)
    {
      if (supportedInterface.IsAssignableFrom (property.GetType()))
      {
        isSupportedPropertyType = true;
        break;
      }
    }
    return isSupportedPropertyType;
  }

  /// <summary>
  ///   Gets the interfaces derived from IBusinessObjectProperty that this control supports, 
  ///   or <see langword="null"/> if no restrictions are made.
  /// </summary>
  /// <remarks>
  ///   Used by <see cref="SupportsProperty"/>.
  /// </remarks>
  [Browsable(false)]
  protected virtual Type[] SupportedPropertyInterfaces 
  { 
    get { return null; }
  }

  /// <summary>
  ///   Indicates whether properties with the specified multiplicity are supported.
  /// </summary>
  /// <remarks>
  ///   Used by <see cref="SupportsProperty"/>.
  /// </remarks>
  /// <returns>
  ///   <see langword="true"/> if the multiplicity specified by <paramref name="isList"/> is 
  ///   supported.
  /// </returns>
  protected virtual bool SupportsPropertyMultiplicity (bool isList)
  {
    return ! isList;
  }

  public virtual BaseValidator[] CreateValidators()
  {
    return new BaseValidator[0];
  }

  [Browsable(false)]
  public virtual string DisplayName 
  {
    get 
    {
      //Binding.EvaluateBinding();
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

  [Browsable (false)]
  protected bool IsDesignMode
  {
    get
    { 
      return ControlHelper.IsDesignMode (this, Context);
    }
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
  [Category ("Data")]
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
  [Category ("Data")]
  [DefaultValue (typeof(NaBooleanEnum), "Undefined")]
  public NaBooleanEnum ReadOnly
  {
    get { return _readOnly; }
    set { _readOnly = value; }
  }

  public abstract void SaveValue (bool interim);

  public override bool IsReadOnly
  {
    get
    {
      if (_readOnly != NaBooleanEnum.Undefined)
        return _readOnly == NaBooleanEnum.True;
      //Binding.EvaluateBinding();
      if (Property == null || DataSource == null)
        return false;
      if (! IsDesignMode && DataSource.BusinessObject == null)
        return true;
      if (! DataSource.EditMode)
        return true;
      return Property.IsReadOnly (DataSource.BusinessObject);
    }
  }

  public override bool IsRequired
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
