using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.Globalization;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.Utilities;
using Rubicon.ObjectBinding.Web.UI.Design;
using Rubicon.ObjectBinding.Design;
using System.Drawing.Design;

namespace Rubicon.ObjectBinding.Web.UI.Controls
{

  /// <summary> This control can be used to render text without any escaping applied. </summary>
  [ToolboxItemFilter ("System.Web.UI")]
  [Designer (typeof (BocDesigner))]
  public class BocLiteral : Control, IExtendedBusinessObjectBoundWebControl
  {
    #region BusinessObjectBinding implementation

    /// <summary>Gets the <see cref="BusinessObjectBinding"/> object used to manage the binding for this <see cref="BusinessObjectBoundWebControl"/>.</summary>
    /// <value> The <see cref="BusinessObjectBinding"/> instance used to manage this control's binding. </value>
    [Browsable (false)]
    public BusinessObjectBinding Binding
    {
      get { return _binding; }
    }

    /// <summary>Gets or sets the <see cref="IBusinessObjectDataSource"/> this <see cref="IBusinessObjectBoundWebControl"/> is bound to.</summary>
    /// <value> An <see cref="IBusinessObjectDataSource"/> providing the current <see cref="IBusinessObject"/>. </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public IBusinessObjectDataSource DataSource
    {
      get { return _binding.DataSource; }
      set { _binding.DataSource = value; }
    }

    /// <summary>Gets or sets the string representation of the <see cref="Property"/>.</summary>
    /// <value> 
    ///   A string that can be used to query the <see cref="IBusinessObjectClass.GetPropertyDefinition"/> method for the 
    ///   <see cref="IBusinessObjectProperty"/>. 
    /// </value>
    [Category ("Data")]
    [Description ("The string representation of the Property.")]
    [Editor (typeof (PropertyPickerEditor), typeof (UITypeEditor))]
    [DefaultValue ("")]
    [MergableProperty (false)]
    public string PropertyIdentifier
    {
      get { return _binding.PropertyIdentifier; }
      set { _binding.PropertyIdentifier = value; }
    }

    /// <summary>Gets or sets the <see cref="IBusinessObjectProperty"/> used for accessing the data to be loaded into <see cref="Value"/>.</summary>
    /// <value>An <see cref="IBusinessObjectProperty"/> that is part of the bound <see cref="IBusinessObject"/>'s <see cref="IBusinessObjectClass"/>.</value>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public IBusinessObjectProperty Property
    {
      get { return _binding.Property; }
      set { _binding.Property = value; }
    }

    /// <summary>
    ///   Gets or sets the <b>ID</b> of the <see cref="IBusinessObjectDataSourceControl"/> encapsulating the <see cref="IBusinessObjectDataSource"/> 
    ///   this  <see cref="IBusinessObjectBoundWebControl"/> is bound to.
    /// </summary>
    /// <value>A string set to the <b>ID</b> of an <see cref="IBusinessObjectDataSourceControl"/> inside the current naming container.</value>
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

    /// <summary>Tests whether this <see cref="BusinessObjectBoundWebControl"/> can be bound to the <paramref name="property"/>.</summary>
    /// <param name="property">The <see cref="IBusinessObjectProperty"/> to be tested. Must not be <see langword="null"/>.</param>
    /// <returns>
    ///   <list type="bullet">
    ///     <item><see langword="true"/> is <see cref="SupportedPropertyInterfaces"/> is null.</item>
    ///     <item><see langword="false"/> if the <see cref="DataSource"/> is in <see cref="DataSourceMode.Search"/> mode.</item>
    ///     <item>Otherwise, <see langword="IsPropertyInterfaceSupported"/> is evaluated and returned as result.</item>
    ///   </list>
    /// </returns>
    public virtual bool SupportsProperty (IBusinessObjectProperty property)
    {
      return _binding.SupportsProperty (property);
    }

    /// <summary>Gets a flag specifying whether the <see cref="IBusinessObjectBoundControl"/> has a valid binding configuration.</summary>
    /// <remarks>
    ///   The configuration is considered invalid if data binding is configured for a property that is not available for the bound class or object.
    /// </remarks>
    /// <value> 
    ///   <list type="bullet">
    ///     <item><see langword="true"/> if the <see cref="DataSource"/> or the <see cref="Property"/> is <see langword="null"/>.</item>
    ///     <item>The result of the <see cref="IBusinessObjectProperty.IsAccessible">IBusinessObjectProperty.IsAccessible</see> method.</item>
    ///     <item>Otherwise, <see langword="false"/> is returned.</item>
    ///   </list>
    /// </value>
    [Browsable (false)]
    public bool HasValidBinding
    {
      get { return _binding.HasValidBinding; }
    }

    #endregion

    //  constants

    //  statics

    private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { typeof (IBusinessObjectStringProperty) };

    // types

    // fields

    private BusinessObjectBinding _binding;
    private string _text = string.Empty;
    private LiteralMode _mode = LiteralMode.Transform;

    public BocLiteral ()
    {
      _binding = new BusinessObjectBinding (this);
    }

    /// <remarks>Calls <see cref="Control.EnsureChildControls"/> and the <see cref="BusinessObjectBinding.EnsureDataSource"/> method.</remarks>
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      EnsureChildControls ();
      _binding.EnsureDataSource ();
    }

    /// <value> 
    ///   <para>
    ///     The <b>set accessor</b> passes the value to the base class's <b>Visible</b> property.
    ///   </para><para>
    ///     The <b>get accessor</b> ANDs the base class's <b>Visible</b> setting with the value of the <see cref="HasValidBinding"/> property.
    ///   </para>
    /// </value>
    /// <remarks>
    ///   The control only saves the set value of <b>Visible</b> into the view state. Therefor the control can change its visibilty during during 
    ///   subsequent postbacks.
    /// </remarks>
    public override bool Visible
    {
      get
      {
        if (!base.Visible)
          return false;

        if (IsDesignMode)
          return true;

        return HasValidBinding;
      }
      set { base.Visible = value; }
    }

    protected override void Render (HtmlTextWriter writer)
    {
      if (!string.IsNullOrEmpty (_text))
      {
        if (_mode != LiteralMode.Encode)
          writer.Write (_text);
        else
          HttpUtility.HtmlEncode (_text, writer);
      }
      else if (IsDesignMode)
      {
        writer.Write ("##");
      }
    }

    protected override void LoadViewState (object savedState)
    {
      object[] values = (object[]) savedState;
      base.LoadViewState (values[0]);
      _mode = (LiteralMode) values[1];
    }

    protected override object SaveViewState ()
    {
      object[] values = new object[4];
      values[0] = base.SaveViewState ();
      values[1] = _mode;
      return values;
    }


    /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
    public virtual void LoadValue (bool interim)
    {
      if (Property != null && DataSource != null && DataSource.BusinessObject != null)
      {
        object value = DataSource.BusinessObject.GetProperty (Property);
        LoadValueInternal (value, interim);
      }
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value"> A <see cref="String"/> to load or <see langword="null"/>. </param>
    public void LoadUnboundValue (object value, bool interim)
    {
      LoadValueInternal (value, interim);
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value"> A <see cref="String"/> to load or <see langword="null"/>. </param>
    public void LoadUnboundValue (string value, bool interim)
    {
      LoadValueInternal (value, interim);
    }

    /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
    protected virtual void LoadValueInternal (object value, bool interim)
    {
      Value = (string) value;
    }

    /// <summary> Gets or sets the current value. </summary>
    [Description ("Gets or sets the current value.")]
    [Browsable (false)]
    public string Value
    {
      get { return _text; }
      set { _text = value; }
    }

    object IBusinessObjectBoundControl.Value
    {
      get { return ValueImplementation; }
      set { ValueImplementation = value; }
    }

    /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
    protected virtual object ValueImplementation
    {
      get { return Value; }
      set { Value = (string) value; }
    }

    /// <summary> Gets or sets the string representation of the current value. </summary>
    /// <value> 
    ///   An empty <see cref="String"/> if the control's value is <see langword="null"/> or empty. 
    ///   The default value is an empty <see cref="String"/>. 
    /// </value>
    [Description ("Gets or sets the string representation of the current value.")]
    [Category ("Data")]
    [DefaultValue ("")]
    public string Text
    {
      get { return StringUtility.NullToEmpty (_text); }
      set { _text = value; }
    }

    /// <summary> Calls <see cref="Control.OnPreRender"/> on every invocation. </summary>
    /// <remarks> Used by the <see cref="BocDesigner"/>. </remarks>
    void IControlWithDesignTimeSupport.PreRenderForDesignMode ()
    {
      if (!IsDesignMode)
        throw new InvalidOperationException ("PreRenderChildControlsForDesignMode may only be called during design time.");
      EnsureChildControls ();
      OnPreRender (EventArgs.Empty);
    }

    bool IExtendedBusinessObjectBoundWebControl.SupportsPropertyMultiplicity (bool isList)
    {
      return SupportsPropertyMultiplicity (isList);
    }

    /// <summary> The <see cref="BocLiteral"/> supports only scalar properties. </summary>
    /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="false"/>. </returns>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
    protected virtual bool SupportsPropertyMultiplicity (bool isList)
    {
      return !isList;
    }

    Type[] IExtendedBusinessObjectBoundWebControl.SupportedPropertyInterfaces
    {
      get { return SupportedPropertyInterfaces; }
    }

    /// <summary>
    ///   The <see cref="BocLiteral"/> supports properties of type <see cref="IBusinessObjectStringProperty"/>.
    /// </summary>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
    protected virtual Type[] SupportedPropertyInterfaces
    {
      get { return s_supportedPropertyInterfaces; }
    }

    /// <summary> Gets the text to be written into the label for this control. </summary>
    /// <value> <see langword="null"/> for the default implementation. </value>
    [Browsable (false)]
    public virtual string DisplayName
    {
      get { return (Property != null) ? Property.DisplayName : null; }
    }

    /// <summary> Specifies the relative URL to the help text for this control. </summary>
    [Browsable (false)]
    public virtual string HelpUrl
    {
      get { return null; }
    }

    bool ISmartControl.UseLabel
    {
      get { return false; }
    }

    Control ISmartControl.TargetControl
    {
      get { return (Control) this; }
    }

    bool ISmartControl.IsRequired
    {
      get { return false; }
    }

    BaseValidator[] ISmartControl.CreateValidators ()
    {
      return new BaseValidator[0];
    }

    /// <summary> Evalutes whether this control is in <b>Design Mode</b>. </summary>
    [Browsable (false)]
    protected bool IsDesignMode
    {
      get { return ControlHelper.IsDesignMode (this, Context); }
    }

    public LiteralMode Mode
    {
      get { return _mode; }
      set { _mode = value; }
    }
  }
}
