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
///   </para><para>
///     See <see cref="BusinessObjectBoundWebControl"/> for the abstract default implementation.
///   </para>
/// </remarks>
/// <seealso cref="IBusinessObjectBoundControl"/>
/// <seealso cref="ISmartControl"/>
/// <seealso cref="IBusinessObjectBoundModifiableWebControl"/>
/// <seealso cref="IBusinessObjectDataSourceControl"/>
public interface IBusinessObjectBoundWebControl: IBusinessObjectBoundControl, ISmartControl
{
  /// <summary>
  ///   Gets or sets the <b>ID</b> of the <see cref="IBusinessObjectDataSourceControl"/> encapsulating the 
  ///   <see cref="IBusinessObjectDataSource"/> this  <see cref="IBusinessObjectBoundWebControl"/> is bound to.
  /// </summary>
  /// <value> 
  ///   A string set to the <b>ID</b> of an <see cref="IBusinessObjectDataSourceControl"/> inside the current
  ///   naming container.
  /// </value>
  /// <remarks>
  ///   The value of this property is used to find the <see cref="IBusinessObjectDataSourceControl"/> in the controls
  ///   collection.
  ///   <note type="inheritinfo">
  ///     Apply an <see cref="BusinessObjectDataSourceControlConverter"/> when implementing the property. 
  ///   </note>
  /// </remarks>
  string DataSourceControl { get; set; }
}

/// <summary>
///   <b>BusinessObjectBoundWebControl</b> is the abstract default implementation of 
///   <see cref="IBusinessObjectBoundWebControl"/>.
/// </summary>
/// <remarks>
///   In order for the control to be visible, it requires a valid binding before <see cref="OnLoad"/> is called.
/// </remarks>
/// <seealso cref="IBusinessObjectBoundWebControl"/>
[Designer (typeof (BocDesigner))]
public abstract class BusinessObjectBoundWebControl: WebControl, IBusinessObjectBoundWebControl
{
  #region BusinessObjectBinding implementation

  /// <summary>
  ///   Gets the <see cref="BusinessObjectBinding"/> object used to manage the binding for this
  ///   <see cref="BusinessObjectBoundWebControl"/>.
  /// </summary>
  /// <value> The <see cref="BusinessObjectBinding"/> instance used to manage this control's binding. </value>
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
  [DefaultValue ("")]
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

  /// <summary>
  ///   Gets or sets the <b>ID</b> of the <see cref="IBusinessObjectDataSourceControl"/> encapsulating the 
  ///   <see cref="IBusinessObjectDataSource"/> this  <see cref="IBusinessObjectBoundWebControl"/> is bound to.
  /// </summary>
  /// <value> 
  ///   A string set to the <b>ID</b> of an <see cref="IBusinessObjectDataSourceControl"/> inside the current
  ///   naming container.
  /// </value>
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

  private BusinessObjectBinding _binding;
  /// <summary> Set or cleared depending on <see cref="HasValidBinding"/> during <see cref="OnLoad"/>. </summary>
  bool _hasVisibleBinding = true;
  /// <summary> Caches the <see cref="ResourceManagerSet"/> for this control. </summary>
  private ResourceManagerSet _cachedResourceManager;
  private bool _controlExistedInPreviousRequest = false; 

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

  /// <summary>
  ///   Gets a flag specifying whether the <see cref="IBusinessObjectBoundControl"/> has a valid binding configuration.
  /// </summary>
  /// <remarks>
  ///   The configuration is considered invalid if data binding is configured for a property 
  ///   that is not available for the bound class or object.
  /// </remarks>
  /// <value> 
  ///   <list type="bullet">
  ///     <item>
  ///       <see langword="true"/> if the <see cref="DataSource"/> or the <see cref="Property"/> is 
  ///       <see langword="null"/>. 
  ///     </item>
  ///     <item>
  ///       The result of the 
  ///       <see cref="IBusinessObjectProperty.IsAccessible">IBusinessObjectProperty.IsAccessible</see> method.
  ///     </item>
  ///     <item>Otherwise, <see langword="false"/> is returned.</item>
  ///   </list>
  /// </value>
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

  /// <summary> Overrides <see cref="Control.Visible"/>. </summary>
  /// <value> 
  ///   <para>
  ///     The <b>set accessor</b> passes the value to the base class's <b>Visible</b> property.
  ///   </para><para>
  ///     The <b>get accessor</b> ANDs the base class's <b>Visible</b> setting with the value of the 
  ///     <see cref="HasValidBinding"/> property cached during <see cref="OnLoad"/>.
  ///   </para>
  /// </value>
  /// <remarks>
  ///   The control only saves the set value of <b>Visible</b> into the view state. Therefor the control can change
  ///   its visibilty during during subsequent postbacks.
  /// </remarks>
  public override bool Visible
  {
    get { return _hasVisibleBinding && base.Visible; }
    set { base.Visible = value; }
  }

  /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
  /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
  public abstract void LoadValue (bool interim);
  
  /// <summary> Gets or sets the value provided by the <see cref="IBusinessObjectBoundControl"/>. </summary>
  /// <value> An object or boxed value. </value>
  /// <remarks>
  ///   <para>
  ///     Override <see cref="ValueImplementation"/> to define the behaviour of <c>Value</c>. 
  ///   </para><para>
  ///     Redefine <c>Value</c> using the keyword <c>new</c> (<c>Shadows</c> in Visual Basic) 
  ///     to provide a typesafe implementation in derived classes.
  ///   </para>
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

  /// <summary> Calls <see cref="Control.OnPreRender"/> on every invocation. </summary>
  /// <remarks> Used by the <see cref="BocDesigner"/>. </remarks>
  internal void PreRenderForDesignMode()
  {
    if (! IsDesignMode)
      throw new InvalidOperationException ("PreRenderChildControlsForDesignMode may only be called during design time.");
    EnsureChildControls();
    OnPreRender (EventArgs.Empty);
  }

  /// <summary> Overrides <see cref="Control.Controls"/> and calls <see cref="Control.EnsureChildControls"/>. </summary>
  public override ControlCollection Controls
  {
    get
    {
      EnsureChildControls();
      return base.Controls;
    }
  }

  /// <summary>
  ///   Tests whether this <see cref="BusinessObjectBoundWebControl"/> can be bound to the <paramref name="property"/>.
  /// </summary>
  /// <param name="property"> The <see cref="IBusinessObjectProperty"/> to be tested. </param>
  /// <returns>
  ///   <list type="bullet">
  ///     <item>
  ///       <see langword="true"/> is <see cref="SupportedPropertyInterfaces"/> is null.
  ///     </item>
  ///     <item>
  ///       <see langword="false"/> if the <see cref="DataSource"/> is in <see cref="DataSourceMode.Search"/> mode.
  ///     </item>
  ///     <item>Otherwise, <see langword="IsPropertyInterfaceSupported"/> is evaluated and returned as result.</item>
  ///   </list>
  /// </returns>
  public virtual bool SupportsProperty (IBusinessObjectProperty property)
  {
    if (SupportedPropertyInterfaces == null)
      return true;

    bool isSearchMode = DataSource != null && DataSource.Mode == DataSourceMode.Search;
    if (! isSearchMode && ! SupportsPropertyMultiplicity (property.IsList))
    {
      return false;
    }

    return BusinessObjectBoundWebControl.IsPropertyInterfaceSupported (property, SupportedPropertyInterfaces);
  }

  /// <summary>
  ///   Tests whether the <paramref name="property"/>'s type is part of the 
  ///   <paramref name="supportedPropertyInterfaces"/> array.
  /// </summary>
  /// <param name="property"> The <see cref="IBusinessObjectProperty"/> to be tested. </param>
  /// <param name="supportedPropertyInterfaces"> 
  ///   The list of interfaces to test the <paramref name="property"/> against. 
  /// </param>
  /// <returns> 
  ///   <see langword="true"/> if the <paramref name="property"/>'s type is found in the 
  ///   <paramref name="supportedPropertyInterfaces"/> array. 
  /// </returns>
  public static bool IsPropertyInterfaceSupported (IBusinessObjectProperty property, Type[] supportedPropertyInterfaces)
  {
    bool isSupportedPropertyInterface = false;
    for (int i = 0; i < supportedPropertyInterfaces.Length; i++)
    {
      Type supportedInterface = supportedPropertyInterfaces[i];
      if (supportedInterface.IsAssignableFrom (property.GetType()))
      {
        isSupportedPropertyInterface = true;
        break;
      }
    }
    return isSupportedPropertyInterface;
  }

  /// <summary>
  ///   Gets the interfaces derived from <see cref="IBusinessObjectProperty"/> supported by this control, 
  ///   or <see langword="null"/> if no restrictions are made.
  /// </summary>
  /// <value> <see langword="null"/> in the default implementation. </value>
  /// <remarks> Used by <see cref="SupportsProperty"/>. </remarks>
  [Browsable(false)]
  protected virtual Type[] SupportedPropertyInterfaces 
  { 
    get { return null; }
  }

  /// <summary> Indicates whether properties with the specified multiplicity are supported. </summary>
  /// <remarks> Used by <see cref="SupportsProperty"/>. </remarks>
  /// <param name="isList"> True if the property is a list property. </param>
  /// <returns> <see langword="true"/> if the multiplicity specified by <paramref name="isList"/> is supported. </returns>
  protected virtual bool SupportsPropertyMultiplicity (bool isList)
  {
    return ! isList;
  }

  /// <summary> Find the <see cref="IResourceManager"/> for this control. </summary>
  /// <param name="localResourcesType"> 
  ///   A type with the <see cref="MultiLingualResourcesAttribute"/> applied to it.
  ///   Typically an <b>enum</b> or the derived class itself.
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

  /// <summary> Gets the text to be written into the label for this control. </summary>
  /// <value> <see langword="null"/> for the default implementation. </value>
  [Browsable(false)]
  public virtual string DisplayName 
  {
    get { return (Property != null) ? Property.DisplayName : null; }
  }

  /// <summary> Specifies the relative URL to the help text for this control. </summary>
  [Browsable(false)]
  public virtual string HelpUrl
  {
    get { return null; }
  }

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its ClientID.
  /// </summary>
  /// <value> This instance for the default implementation. </value>
  [Browsable(false)]
  public virtual Control TargetControl
  {
    get { return this; }
  }

  /// <summary>
  ///   If <b>UseLabel</b> is <see langword="true"/>, it is valid to generate HTML &lt;label&gt; tags referencing 
  ///   <see cref="TargetControl"/>.
  /// </summary>
  /// <value>
  ///   <see langword="true"/> unsless the <see cref="TargetControl"/> is a <see cref="DropDownList"/> or an 
  ///   <see cref="System.Web.UI.HtmlControls.HtmlSelect"/> control.
  /// </value>
  [Browsable(false)]
  public virtual bool UseLabel
  {
    get { return ! (TargetControl is DropDownList || TargetControl is System.Web.UI.HtmlControls.HtmlSelect); }
  }

  /// <summary> Evalutes whether this control is in <b>Design Mode</b>. </summary>
  [Browsable (false)]
  protected bool IsDesignMode
  {
    get { return ControlHelper.IsDesignMode (this, Context); }
  }

  /// <exclude/>
  bool ISmartControl.IsRequired 
  {
    get { return false; }
  }

  /// <exclude/>
  BaseValidator[] ISmartControl.CreateValidators()
  {
    return new BaseValidator[0];
  }

  /// <summary> Gets a flag whether the control already existed in the previous page life cycle. </summary>
  /// <remarks> 
  ///   This property utilizes the <see cref="LoadViewState"/> method for determining a post back. It therefor 
  ///   requires the control to use the view state. In addition, the property is only useful after the load view state
  ///   phase of the page life cycle.
  /// </remarks>
  /// <value> <see langword="true"/> if the control has been on the page in the previous life cycle. </value>
  protected bool ControlExistedInPreviousRequest 
  { 
    get { return _controlExistedInPreviousRequest; }
  }

  protected override void LoadViewState(object savedState)
  {
    base.LoadViewState (savedState);
    _controlExistedInPreviousRequest = true;
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
