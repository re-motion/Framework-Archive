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
///   Provides functionality for binding an <see cref="ISmartControl"/> to an <see cref="IBusinessObject"/> using
///   an <see cref="IBusinessObjectDataSourceControl"/>. 
/// </summary>
/// <remarks>
///   <para>
///     See the <see href="Rubicon.ObjectBinding.html">Rubicon.ObjectBinding</see> namespace documentation for general 
///     information on the data binding process.
///   </para>
/// </remarks>
/// <seealso cref="IBusinessObjectBoundControl"/>
/// <seealso cref="ISmartControl"/>
/// <seealso cref="IBusinessObjectBoundModifiableWebControl"/>
/// <seealso cref="IBusinessObjectDataSourceControl"/>
public interface IBusinessObjectBoundWebControl: IBusinessObjectBoundControl, ISmartControl
{
  string DataSourceControl { get; set; }
}

/// <summary>
///   Extends an <see cref="IBusinessObjectBoundWebControl"/> with functionality for validating the control's 
///   <see cref="IBusinessObjectBoundControl.Value"/> and writing it back into the bound <see cref="IBusinessObject"/>.
/// </summary>
/// <remarks>
///   See <see cref="IBusinessObjectBoundControl.SaveValue"/> for a description of the data binding process.
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

/// <summary>
///   <b>BusinessObjectBoundWebControl</b> is the abstract default implementation of 
///   <see cref="IBusinessObjectBoundWebControl"/>.
/// </summary>
/// <seealso cref="IBusinessObjectBoundWebControl"/>
[Designer (typeof (BocDesigner))]
public abstract class BusinessObjectBoundWebControl: WebControl, IBusinessObjectBoundWebControl
{
  private BusinessObjectBinding _binding;
  private bool _childControlsPreRendered = false;
  bool _hasVisibleBinding = true;
  /// <summary> Caches the <see cref="ResourceManagerSet"/> for this control. </summary>
  private ResourceManagerSet _cachedResourceManager;

  #region BusinessObjectBinding implementation

  /// <summary>
  ///   Gets the <see cref="BusinessObjectBinding"/> object used to manage the binding for this
  ///   <see cref="BusinessObjectBoundWebControl"/>.
  /// </summary>
  [Browsable(false)]
  public BusinessObjectBinding Binding
  {
    get { return _binding; }
  }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObjectDataSource"/> this <see cref="IBusinessObjectBoundWebControl"/> 
  ///   is bound to.
  /// </summary>
  /// <value> An <see cref="IBusinessObjectDataSource"/> providing the current <see cref="IBusinessObject"/>. </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public IBusinessObjectDataSource DataSource
  {
    get { return _binding.DataSource; }
    set { _binding.DataSource = value; }
  }

  /// <summary>
  ///   Gets or sets the string representation of the <see cref="Property"/>.
  /// </summary>
  /// <value> 
  ///   A string that can be used to query the <see cref="IBusinessObjectClass.GetPropertyDefinition"/> method for
  ///   the <see cref="IBusinessObjectProperty"/>. 
  /// </value>
  [Category ("Data")]
  [Description ("The string representation of the Property.")]
  [Editor (typeof (PropertyPickerEditor), typeof (UITypeEditor))]
  public string PropertyIdentifier
  {
    get { return _binding.PropertyIdentifier; }
    set { _binding.PropertyIdentifier = value; }
  }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObjectProperty"/> used for accessing the data to be loaded into 
  ///   <see cref="Value"/>.
  /// </summary>
  /// <value> 
  ///   An <see cref="IBusinessObjectProperty"/> that is part of the bound <see cref="IBusinessObject"/>'s
  ///   <see cref="IBusinessObjectClass"/>
  /// </value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObjectProperty Property
  {
    get { return _binding.Property; }
    set { _binding.Property = value; }
  }
  #endregion

  /// <summary>
  ///   Gets or sets the <b>ID</b> of the <see cref="IBusinessObjectDataSourceControl"/> encapsulating the 
  ///   <see cref="IBusinessObjectDataSource"/> this  <see cref="IBusinessObjectBoundWebControl"/> is bound to.
  /// </summary>
  /// <value> 
  ///   A string set to the <b>ID</b> of an <see cref="IBusinessObjectDataSourceControl"/> inside the current
  ///   naming container.
  /// </value>
  /// <remarks>
  ///   In order for the control to be visible, it requires a valid binding before <see cref="OnLoad"/> is called.
  /// </remarks>
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

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectService"/> from the <paramref name="businessObjectProvider"/> 
  ///   and queries it for an <see cref="IconInfo"/> object.
  /// </summary>
  /// <param name="businessObject"> 
  ///   The <see cref="IBusinessObject"/> to be passed to the <see cref="IBusinessObjectWebUIService"/>'s
  ///   <see cref="IBusinessObjectWebUIService.GetIcon"/> method.
  /// </param>
  /// <param name="businessObjectProvider"> 
  ///   The <see cref="IBusinessObjectProvider"/> to be used to get the <see cref="IconInfo"/> object.
  ///   Must not be <see langowrd="null"/>. 
  /// </param>
  public static IconInfo GetIcon (IBusinessObject businessObject, IBusinessObjectProvider businessObjectProvider)
  {
    ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

    IconInfo iconInfo = null;

    IBusinessObjectWebUIService webUIService = 
        (IBusinessObjectWebUIService) businessObjectProvider.GetService (typeof (IBusinessObjectWebUIService));

    if (webUIService != null)
      iconInfo = webUIService.GetIcon (businessObject);

    return iconInfo;
  }

  /// <summary> Creates a new instance of the BusinessObjectBoundWebControl type. </summary>
  public BusinessObjectBoundWebControl ()
  {
    _binding = new BusinessObjectBinding (this);
  }

  /// <summary> Overrides <see cref="Control.OnInit"/>. </summary>
  /// <remarks>
  ///   Calls <see cref="Control.EnsureChildControls"/> and the <see cref="BusinessObjectBinding.EnsureDataSource"/>
  ///   method.
  /// </remarks>
  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
    EnsureChildControls();
    _binding.EnsureDataSource();
  }

  /// <summary> Overrides <see cref="Control.OnLoad"/>. </summary>
  /// <remarks> Evaluates <see cref="HasValidBinding"/> to determine the control's visibility. </remarks>
  protected override void OnLoad (EventArgs e)
  {
    base.OnLoad (e);
    if (! ControlHelper.IsDesignMode (this, Context))
      _hasVisibleBinding = HasValidBinding;
  }

  /// <summary> Overrides <see cref="WebControl.Visible"/>. </summary>
  /// <value> 
  ///   <para>
  ///     The <b>set accessor</b> passes the value to the base class's <b>Visible</b> property.
  ///   </para><para>
  ///     The <b>get accessor</b> ANDs the base class's <b>Visible</b> setting with the value of the 
  ///     <see cref="HasValidBinding"/> property during <see cref="OnLoad"/>.
  ///   </para>
  /// </value>
  /// <remarks>
  ///   The control only saves the set value of <b>Visible</b> into the view state. Therefor the control can change
  ///   it's visibilty during during subsequent post backs.
  /// </remarks>
  public override bool Visible
  {
    get { return _hasVisibleBinding && base.Visible; }
    set { base.Visible = value; }
  }

  /// <exclude/>
  bool ISmartControl.IsRequired 
  {
    get { return false; }
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

  /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
  [Browsable (false)]
  protected abstract object ValueImplementation { get; set; }

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);
    //  Best left to specializations
    // EnsureChildControlsPreRendered();
  }

  public override ControlCollection Controls
  {
    get
    {
      EnsureChildControls();
      return base.Controls;
    }
  }

  /// <summary>
  ///   Calls <see cref="PreRenderChildControls"/> on the first invocation.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     In some situations, this method gets invoked more than once in the VS.NET designer.
  ///   </para><para>
  ///     Best practice: call once in <c>OnPreRender</c> method and once in <c>Render</c> method.
  ///   </para>
  /// </remarks>
  protected void EnsureChildControlsPreRendered()
  {
    EnsureChildControls();
    if (! _childControlsPreRendered || IsDesignMode)
    {
      PreRenderChildControls();
      _childControlsPreRendered = true;
    }
  }

  /// <summary>
  ///   Override this method to Prerenders child controls.
  /// </summary>
  /// <remarks>
  ///   Child controls that do not need to be created before handling post data can be created
  ///   in this method. Use <see cref="EnsureChildControlsPreRendered"/> to call this method.
  /// </remarks>
  protected virtual void PreRenderChildControls ()
  {
  }

  public virtual bool SupportsProperty (IBusinessObjectProperty property)
  {
    if (SupportedPropertyInterfaces == null)
      return true;

    bool searchMode = DataSource != null && DataSource.Mode == DataSourceMode.Search;
    if (! searchMode && ! SupportsPropertyMultiplicity (property.IsList))
    {
      return false;
    }

    return BusinessObjectBoundWebControl.IsPropertyInterfaceSupported (property, SupportedPropertyInterfaces);
  }

  public static bool IsPropertyInterfaceSupported (IBusinessObjectProperty property, Type[] supportedPropertyInterfaces)
  {
    bool isSupportedPropertyInterface = false;
    foreach (Type supportedInterface in supportedPropertyInterfaces)
    {
      if (supportedInterface.IsAssignableFrom (property.GetType()))
      {
        isSupportedPropertyInterface = true;
        break;
      }
    }
    return isSupportedPropertyInterface;
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
  /// <param name="isList"> True if the property is a list property. </param>
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

  /// <summary> Find the <see cref="IResourceManager"/> for this control. </summary>
  /// <param name="localResourcesType"> 
  ///   A type with the <see cref="MultiLingualResourcesAttribute"/> applied to it.
  ///   Typically the an enum or the derived class it self.
  /// </param>
  protected IResourceManager GetResourceManager (Type localResourcesType)
  {
    Rubicon.Utilities.ArgumentUtility.CheckNotNull ("localResourcesType", localResourcesType);

    //  Provider has already been identified.
    if (_cachedResourceManager != null)
        return _cachedResourceManager;

    //  Get the resource managers

    IResourceManager localResourceManager = 
        MultiLingualResourcesAttribute.GetResourceManager (localResourcesType, true);
    IResourceManager namingContainerResourceManager = 
        ResourceManagerUtility.GetResourceManager (NamingContainer, true);

    if (namingContainerResourceManager == null)
      _cachedResourceManager = new ResourceManagerSet (localResourceManager);
    else
      _cachedResourceManager = new ResourceManagerSet (localResourceManager, namingContainerResourceManager);

    return _cachedResourceManager;
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
    get { return ! (TargetControl is DropDownList || TargetControl is System.Web.UI.HtmlControls.HtmlSelect); }
  }

  [Browsable (false)]
  protected bool IsDesignMode
  {
    get
    { 
      return ControlHelper.IsDesignMode (this, Context);
    }
  }

  [Browsable (false)]
  public bool HasValidBinding
  {
    get 
    { 
      IBusinessObjectDataSource dataSource = _binding.DataSource;
      IBusinessObjectProperty property = _binding.Property;
      if (dataSource == null || property == null)
        return true;
      return property.IsAccessible (dataSource.BusinessObjectClass, dataSource.BusinessObject);
    } 
  }

//  /// <summary>
//  ///   Occurs after either the <see cref="Property"/> property or the <see cref="PropertyIdentifier"/> property is assigned a new value.
//  /// </summary>
//  /// <remarks>
//  ///   Note that this event does not occur if the property path is modified, only if a new one is assigned.
//  /// </remarks>
//  public event BindingChangedEventHandler BindingChanged;

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
}

/// <seealso cref="IBusinessObjectBoundModifiableWebControl"/>
public abstract class BusinessObjectBoundModifiableWebControl:
    BusinessObjectBoundWebControl, IBusinessObjectBoundModifiableWebControl
{
  private NaBooleanEnum _required = NaBooleanEnum.Undefined;
  private NaBooleanEnum _readOnly = NaBooleanEnum.Undefined;
  private TypedArrayList _validators;

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
  ///     <list type="bullets">
  ///       <item>
  ///         If the value of the <see cref="ReadOnly"/> property is not <see cref="NaBooleanEnum.Undefined"/>,
  ///         the value of <see cref="ReadOnly"/> is returned.
  ///       </item>
  ///       <item>
  ///         If the control is bound to an <c>FscObject</c> component and a <see cref="BusinessObjectPropertyPath"/>, 
  ///         and the bound <c>FscObject</c> component's <c>EditMode</c> property is <see langword="false"/>, 
  ///         <see langword="false"/> is returned.
  ///         </item>
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
  ///     <list type="bullets">
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
