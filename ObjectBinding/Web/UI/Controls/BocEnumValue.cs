using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.Design;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary>
///   This control can be used to display or edit enumeration values.
/// </summary>
[ValidationProperty ("Value")]
[DefaultEvent ("SelectionChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocEnumValue: BusinessObjectBoundModifiableWebControl //, IPostBackDataHandler
{
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectEnumerationProperty) };

  /// <summary>
  ///   This event is fired when the selection is changed in the UI.
  /// </summary>
  /// <remarks>
  ///   The event is fired only if the selection change is caused by the user.
  /// </remarks>
  public event EventHandler SelectionChanged;

  private const string c_nullIdentifier = "--null--";
  private const string c_nullDisplayName = "Undefined";

  private bool _isDirty = true;
  private ListControl _listControl;
  private Label _label = null;

  private object _value = null;
  private object _newValue = null;

  [Browsable (false)]
  public ListControl ListControl
  {
    get { return _listControl; }
  }

  [Browsable (false)]
  public Label Label
  {
    get { return _label; }
  }

	public BocEnumValue()
	{
	}

  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
    Binding.BindingChanged += new EventHandler (Binding_BindingChanged);

    _listControl = _listControlStyle.Create (false);
    _label = new Label ();

    _listControl.ID = this.ID + "_ListControl";
    _listControl.EnableViewState = true;
    Controls.Add (_listControl);

    _label.ID = this.ID + "_Label";
    _label.EnableViewState = false;
    Controls.Add (_label);

    _listControl.SelectedIndexChanged += new EventHandler(ListControl_SelectedIndexChanged);
  }

  protected override void OnLoad (EventArgs e)
  {
    base.OnLoad (e);

    Binding.EvaluateBinding();

    if (! (this.Site != null && this.Site.DesignMode))
    {
      string newValue = this.Page.Request.Form[_listControl.UniqueID];
      if (newValue == c_nullIdentifier)
        _newValue = null;
      else if (newValue != null)
        _newValue = Property.GetValueInfo (newValue).Value;
    }

    if (_newValue != null && _newValue != _value)
      _isDirty = true;
  }

  private void ListControl_SelectedIndexChanged (object sender, EventArgs e)
  {
    if (_newValue != null && _newValue != _value)
    {
      Value = _newValue;
      OnSelectionChanged (EventArgs.Empty);
    }
  }

  /// <summary>
  /// Fires the <see cref="SelectionChanged"/> event.
  /// </summary>
  /// <param name="e"> Empty. </param>
  protected virtual void OnSelectionChanged (EventArgs e)
  {
    if (SelectionChanged != null)
      SelectionChanged (this, e);
  }

  private void Binding_BindingChanged (object sender, EventArgs e)
  {
    _listControl.Items.Clear();
    if (Property != null && _listControl != null)
    {
      if (_value == null || !IsRequired)
      {
        string nullDisplayName = (_listControl is DropDownList) ? string.Empty : c_nullDisplayName;
        ListItem emptyItem = new ListItem (nullDisplayName, c_nullIdentifier);
        _listControl.Items.Add (emptyItem);
      }

      foreach (IEnumerationValueInfo valueInfo in Property.GetAllValues())
      {
        ListItem item = new ListItem (valueInfo.DisplayName, valueInfo.Value.ToString());
        _listControl.Items.Add (item);
      }
    }
    InternalLoadValue();
  }

  private void InternalLoadValue ()
  {
    if (Property != null)
    {
      IEnumerationValueInfo valueInfo = Property.GetValueInfo (_value);
      if (valueInfo == null)
      {
        _listControl.SelectedIndex = 0;
      }
      else
      {
        for (int i = 0; i < _listControl.Items.Count; ++i)
        {
          if (_listControl.Items[i].Value == valueInfo.Identifier)
          {
            _listControl.SelectedIndex = i;
            break;
          }
        }
      }
    }
  }

  public override void LoadValue()
  {
    Binding.EvaluateBinding();
    if (Property != null && DataSource != null && DataSource.BusinessObject != null)
    {
      Value = DataSource.BusinessObject.GetProperty (Property);
      _isDirty = false;
    }
  }

  public override void SaveValue()
  {
    Binding.EvaluateBinding();
    if (Property != null && DataSource != null &&  DataSource.BusinessObject != null && ! IsReadOnly)
      DataSource.BusinessObject.SetProperty (Property, Value);
  }

  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);
    EnsureChildControlsInitialized ();
  }

  protected override void Render (HtmlTextWriter writer)
  {
    EnsureChildControlsInitialized ();
    base.Render (writer);
  }

  public new IBusinessObjectEnumerationProperty Property
  {
    get { return (IBusinessObjectEnumerationProperty) base.Property; }
    set { base.Property = (IBusinessObjectEnumerationProperty) value; }
  }

  protected override void InitializeChildControls()
  {
    bool isReadOnly = IsReadOnly;
    _listControl.Visible = ! IsReadOnly;
    _label.Visible = isReadOnly;
    if (isReadOnly)
    {
      _label.Text = Property.GetValueInfo (Value).DisplayName;

      _label.Width = this.Width;
      _label.Height = this.Height;
      _label.ApplyStyle (_commonStyle);
      _label.ApplyStyle (_labelStyle);
    }
    else
    {
      _listControl.Width = this.Width;
      _listControl.Height = this.Height;
      _listControl.ApplyStyle (_commonStyle);
      _listControlStyle.ApplyStyle (_listControl);
    }
  }

  private Style _commonStyle = new Style ();
  private ListControlStyle _listControlStyle = new ListControlStyle ();
  private Style _labelStyle = new Style ();

  /// <summary>
  ///   The style that you want to apply to the TextBox (edit mode) and the Label (read-only mode).
  /// </summary>
  /// <remarks>
  ///   Use the <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> to assign individual style settings for
  ///   the respective modes. Note that if you set one of the <c>Font</c> attributes (Bold, Italic etc.) to 
  ///   <c>true</c>, this cannot be overridden using <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> 
  ///   properties.
  /// </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to the TextBox (edit mode) and the Label (read-only mode).")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style CommonStyle
  {
    get { return _commonStyle; }
  }

  /// <summary>
  ///   The style that you want to apply to the TextBox (edit mode) only.
  /// </summary>
  /// <remarks>
  ///   These style settings override the styles defined in <see cref="CommonStyle"/>.
  /// </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to the TextBox (edit mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public ListControlStyle ListControlStyle
  {
    get { return _listControlStyle; }
  }

  /// <summary>
  ///   The style that you want to apply to the Label (read-only mode) only.
  /// </summary>
  /// <remarks>
  ///   These style settings override the styles defined in <see cref="CommonStyle"/>.
  /// </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to the Label (read-only mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style LabelStyle
  {
    get { return _labelStyle; }
  }

  protected override void LoadViewState(object savedState)
  {
    object[] values = (object[]) savedState;
    base.LoadViewState (values[0]);
    Value = values[1];
    _isDirty = (bool)  values[2];
  }

  protected override object SaveViewState()
  {
    object[] values = new object[3];
    values[0] = base.SaveViewState();
    values[1] = Value;
    values[2] = _isDirty;
    return values;
  }

  /// <summary>
  ///   Gets or sets the current value.
  /// </summary>
  [Description("Gets or sets the current value.")]
  [Browsable(false)]
  public override object Value
  {
    get { return _value; }
    set 
    { 
      _value = value; 
      InternalLoadValue();
    }
  }

  public override Control TargetControl 
  {
    get { return (_listControl != null) ? _listControl : (Control) this; }
  }

  /// <summary>
  ///   Specifies whether the text within the control has been changed since the last load/save operation.
  /// </summary>
  /// <remarks>
  ///   Initially, the value of <c>IsDirty</c> is <c>true</c>. The value is set to <c>false</c> during loading
  ///   and saving values. Text changes by the user cause <c>IsDirty</c> to be reset to <c>false</c> during the
  ///   loading phase of the request (i.e., before the page's <c>Load</c> event is raised).
  /// </remarks>
  public override bool IsDirty
  {
    get { return _isDirty; }
    set { _isDirty = value; }
  }

  protected override Type[] SupportedPropertyInterfaces
  {
    get { return s_supportedPropertyInterfaces; }
  }

  // TODO: works for list box??
  public override bool UseLabel
  {
    get { return _listControlStyle.ControlType != ListControlType.DropDownList; }
  }
}

}
