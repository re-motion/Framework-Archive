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
  string DataSourceControl { get; set; }
}

/// <summary>
///   <b>BusinessObjectBoundWebControl</b> is the abstract default implementation of 
///   <see cref="IBusinessObjectBoundWebControl"/>.
/// </summary>
/// <seealso cref="IBusinessObjectBoundWebControl"/>
[Designer (typeof (BocDesigner))]
public abstract class BusinessObjectBoundWebControl: WebControl, IBusinessObjectBoundWebControl
{
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
  private bool _childControlsPreRendered = false;
  bool _hasVisibleBinding = true;
  /// <summary> Caches the <see cref="ResourceManagerSet"/> for this control. </summary>
  private ResourceManagerSet _cachedResourceManager;

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
  ///       <see langword="true"/> if <see cref="DataSource"/> or <see cref="Property"/> is <see langword="null"/>. 
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

  /// <summary> Overrides <see cref="Control.Visible"/>. </summary>
  /// <remarks>
  ///   <note type="inheritinfo">
  ///     Override this method and execute <see cref="EnsureChildControlsPreRendered"/>.
  ///   </note>
  /// </remarks>
  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);
    //  Best left to specializations
    // EnsureChildControlsPreRendered();
  }

  /// <summary> 
  ///   Calls <see cref="PreRenderChildControls"/> on the first invocation or
  ///   on every invocation while the control is in <b>DesignMode</b>.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     In <b>DesignMode</b>, <see cref="OnPreRender"/> is not executed. The <see cref="Control.Render"/> method
  ///     on the other hand is called every time the control must be redrawn. This includes changing a property,
  ///     switching from <b>HTML</b> view to <b>Design</b> view and of course, a complete <b>Refresh</b> of
  ///     the page being designed.
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

  /// <summary> Override this method to pre-render child controls. </summary>
  /// <remarks>
  ///   Child controls that do not need to be created before handling post data can be created
  ///   in this method. Use <see cref="EnsureChildControlsPreRendered"/> to call this method.
  /// </remarks>
  protected virtual void PreRenderChildControls ()
  {
  }

  public override ControlCollection Controls
  {
    get
    {
      EnsureChildControls();
      return base.Controls;
    }
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

  /// <exclude/>
  bool ISmartControl.IsRequired 
  {
    get { return false; }
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
