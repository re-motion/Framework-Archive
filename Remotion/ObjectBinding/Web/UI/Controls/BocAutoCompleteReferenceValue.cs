// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using Microsoft.Practices.ServiceLocation;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocAutoCompleteReferenceValue;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocAutoCompleteReferenceValue.StandardMode;
using Remotion.ObjectBinding.Web.UI.Design;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering.DropDownMenu;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  [ValidationProperty ("BusinessObjectUniqueIdentifier")]
  [DefaultEvent ("SelectionChanged")]
  [ToolboxItemFilter ("System.Web.UI")]
  [Designer (typeof (BocDesigner))]
  public class BocAutoCompleteReferenceValue
      :
          BusinessObjectBoundEditableWebControl,
          IBocAutoCompleteReferenceValue,
          IPostBackDataHandler,
          IFocusableControl
  {
    // constants

    /// <summary> The text displayed when control is displayed in desinger, is read-only, and has no contents. </summary>
    private const string c_designModeEmptyLabelContents = "##";

    // types

    /// <summary> A list of control specific resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources ("Remotion.ObjectBinding.Web.Globalization.BocAutoCompleteReferenceValue")]
    protected enum ResourceIdentifier
    {
      /// <summary> The validation error message displayed when the null item is selected. </summary>
      NullItemValidationMessage
    }

    // static members

    private static readonly Type[] s_supportedPropertyInterfaces = new[] { typeof (IBusinessObjectReferenceProperty) };

    private static readonly object s_selectionChangedEvent = new object();

    // member fields

    private readonly TextBox _textBox;
    private readonly HiddenField _hiddenField;
    private readonly Label _label;
    private readonly DropDownMenu _optionsMenu;

    private readonly Style _commonStyle;
    private readonly SingleRowTextBoxStyle _textBoxStyle;
    private readonly Style _labelStyle;

    /// <summary> 
    ///   The object returned by <see cref="BocReferenceValue"/>. 
    ///   Does not require <see cref="System.Runtime.Serialization.ISerializable"/>. 
    /// </summary>
    private IBusinessObjectWithIdentity _value;

    /// <summary> The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> of the current object. </summary>
    private string _internalValue;

    private string _displayName;

    private string _errorMessage;
    private readonly ArrayList _validators;

    private string _serviceMethod = string.Empty;
    private string _servicePath = string.Empty;
    private string _args = string.Empty;
    private int? _completionSetCount = 10;
    private int _completionInterval = 1000;
    private int _suggestionInterval = 200;
    private readonly SingleControlItemCollection _command;

    // construction and disposing

    public BocAutoCompleteReferenceValue ()
    {
      _commonStyle = new Style();
      _textBoxStyle = new SingleRowTextBoxStyle();
      _labelStyle = new Style();
      _textBox = new TextBox();
      _hiddenField = new HiddenField();
      _label = new Label();
      _optionsMenu = new DropDownMenu();
      _validators = new ArrayList();

      _command = new SingleControlItemCollection (new BocCommand (), new[] { typeof (BocCommand) });

      EnableIcon = true;
      ShowOptionsMenu = true;
    }

    // methods and properties

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      if (!IsDesignMode)
        Page.RegisterRequiresPostBack (this);
    }

    public override void RegisterHtmlHeadContents (IHttpContext httpContext, HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("httpContext", httpContext);
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      base.RegisterHtmlHeadContents (httpContext, htmlHeadAppender);

      var factory = ServiceLocator.Current.GetInstance<IBocAutoCompleteReferenceValueRendererFactory>();
      var preRenderer = factory.CreatePreRenderer (new HttpContextWrapper (Context), this);
      preRenderer.RegisterHtmlHeadContents (htmlHeadAppender);
    }

    /// <remarks>
    ///   If the <see cref="DropDownList"/> could not be created from <see cref="DropDownListStyle"/>,
    ///   the control is set to read-only.
    /// </remarks>
    protected override void CreateChildControls ()
    {
      _hiddenField.ID = ID + "_Boc_HiddenField";
      _hiddenField.EnableViewState = false;
      Controls.Add (_hiddenField);

      _textBox.ID = ID + "_Boc_TextBox";
      _textBox.EnableViewState = true;
      Controls.Add (_textBox);

      _label.ID = ID + "_Boc_Label";
      _label.EnableViewState = false;
      Controls.Add (_label);
    }

    /// <summary> Invokes the <see cref="LoadPostData"/> method. </summary>
    bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      if (RequiresLoadPostData)
        return LoadPostData (postDataKey, postCollection);
      else
        return false;
    }

    /// <summary> Invokes the <see cref="RaisePostDataChangedEvent"/> method. </summary>
    void IPostBackDataHandler.RaisePostDataChangedEvent ()
    {
      RaisePostDataChangedEvent();
    }

    /// <summary>
    ///   Uses the <paramref name="postCollection"/> to determine whether the value of this control has been changed
    ///   between postbacks.
    /// </summary>
    /// <include file='..\Web\doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/LoadPostData/*' />
    protected virtual bool LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      string newValue = PageUtility.GetPostBackCollectionItem (Page, _hiddenField.UniqueID);
      bool isDataChanged = false;
      if (newValue != null)
      {
        if (_internalValue == null && newValue.Length > 0)
          isDataChanged = true;
        else if (_internalValue != null && newValue != _internalValue)
          isDataChanged = true;
      }

      if (isDataChanged)
      {
        if (newValue.Length == 0)
          _internalValue = null;
        else
          _internalValue = newValue;

        _displayName = PageUtility.GetPostBackCollectionItem (Page, _textBox.UniqueID);
        IsDirty = true;
      }
      return isDataChanged;
    }

    /// <summary> Called when the state of the control has changed between postbacks. </summary>
    protected virtual void RaisePostDataChangedEvent ()
    {
      if (!IsReadOnly && Enabled)
        OnSelectionChanged();
    }

    /// <summary> Fires the <see cref="SelectionChanged"/> event. </summary>
    protected virtual void OnSelectionChanged ()
    {
      EventHandler eventHandler = (EventHandler) Events[s_selectionChangedEvent];
      if (eventHandler != null)
        eventHandler (this, EventArgs.Empty);
    }

    /// <summary> Loads the resources into the control's properties. </summary>
    protected override void LoadResources (IResourceManager resourceManager)
    {
      if (resourceManager == null)
        return;
      if (IsDesignMode)
        return;
      base.LoadResources (resourceManager);

      string key = ResourceManagerUtility.GetGlobalResourceKey (ErrorMessage);
      if (!StringUtility.IsNullOrEmpty (key))
        ErrorMessage = resourceManager.GetString (key);
    }

    /// <summary> Checks whether the control conforms to the required WAI level. </summary>
    /// <exception cref="WcagException"> Thrown if the control does not conform to the required WAI level. </exception>
    protected virtual void EvaluateWaiConformity ()
    {
      if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
        WcagHelper.Instance.HandleError (1, this);
    }

    public override void PrepareValidation ()
    {
      base.PrepareValidation();

      if (!IsReadOnly)
        SetEditModeValue();
    }

    private void SetEditModeValue ()
    {
      _hiddenField.Value = _internalValue;
      IBusinessObjectWithIdentity obj = Value;
      if (obj != null)
        _displayName = GetDisplayName (obj);
      _textBox.Text = _displayName;
    }

    protected override void OnPreRender (EventArgs e)
    {
      EnsureChildControls();
      base.OnPreRender (e);

      LoadResources (GetResourceManager());

      if (IsReadOnly)
        PreRenderReadOnlyValue();
      else
        PreRenderEditModeValue();

      var factory = ServiceLocator.Current.GetInstance<IBocAutoCompleteReferenceValueRendererFactory> ();
      var preRenderer = factory.CreatePreRenderer (new HttpContextWrapper (Context), this);
      preRenderer.PreRender();
    }

    public override void RenderControl (HtmlTextWriter writer)
    {
      EvaluateWaiConformity();

      var renderer = new BocAutoCompleteReferenceValueRenderer (new HttpContextWrapper (Context), writer, this);
      renderer.Render();
    }

    protected override void LoadControlState (object savedState)
    {
      object[] values = (object[]) savedState;

      base.LoadControlState (values[0]);
      _internalValue = (string) values[1];
      _displayName = (string) values[2];

      _hiddenField.Value = _internalValue;
      _textBox.Text = _displayName;
    }

    protected override object SaveControlState ()
    {
      object[] values = new object[3];

      values[0] = base.SaveControlState();
      values[1] = _internalValue;
      values[2] = _displayName;

      return values;
    }


    /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='..\Web\doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/LoadValue/*' />
    public override void LoadValue (bool interim)
    {
      if (!interim)
      {
        if (Property != null && DataSource != null && DataSource.BusinessObject != null)
        {
          IBusinessObjectWithIdentity value =
              (IBusinessObjectWithIdentity) DataSource.BusinessObject.GetProperty (Property);
          LoadValueInternal (value, interim);
        }
      }
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value"> 
    ///   The object implementing <see cref="IBusinessObjectWithIdentity"/> to load, or <see langword="null"/>. 
    /// </param>
    /// <include file='..\Web\doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/LoadUnboundValue/*' />
    public void LoadUnboundValue (IBusinessObjectWithIdentity value, bool interim)
    {
      LoadValueInternal (value, interim);
    }

    /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
    protected virtual void LoadValueInternal (IBusinessObjectWithIdentity value, bool interim)
    {
      if (!interim)
      {
        Value = value;
        IsDirty = false;
      }
    }

    /// <summary> Saves the <see cref="Value"/> into the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='..\Web\doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/SaveValue/*' />
    public override void SaveValue (bool interim)
    {
      if (!interim && IsDirty)
      {
        if (Property != null && DataSource != null && DataSource.BusinessObject != null && !IsReadOnly)
        {
          DataSource.BusinessObject.SetProperty (Property, Value);
          IsDirty = false;
        }
      }
    }

    /// <summary> Returns the <see cref="IResourceManager"/> used to access the resources for this control. </summary>
    protected virtual IResourceManager GetResourceManager ()
    {
      return GetResourceManager (typeof (ResourceIdentifier));
    }

    /// <summary> Creates the list of validators required for the current binding and property settings. </summary>
    /// <include file='..\Web\doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/CreateValidators/*' />
    public override BaseValidator[] CreateValidators ()
    {
      if (IsReadOnly || !IsRequired)
        return new BaseValidator[0];

      BaseValidator[] validators = new BaseValidator[1];

      RequiredFieldValidator notNullItemValidator = new RequiredFieldValidator();
      notNullItemValidator.ID = ID + "_ValidatorNotNullItem";
      notNullItemValidator.ControlToValidate = ID;
      if (StringUtility.IsNullOrEmpty (_errorMessage))
        notNullItemValidator.ErrorMessage = GetResourceManager().GetString (ResourceIdentifier.NullItemValidationMessage);
      else
        notNullItemValidator.ErrorMessage = _errorMessage;
      validators[0] = notNullItemValidator;

      _validators.AddRange (validators);
      return validators;
    }

    private void PreRenderReadOnlyValue ()
    {
      string text;
      if (_internalValue != null)
        text = HttpUtility.HtmlEncode (_displayName);
      else
        text = String.Empty;
      if (StringUtility.IsNullOrEmpty (text))
      {
        if (IsDesignMode)
        {
          text = c_designModeEmptyLabelContents;
          //  Too long, can't resize in designer to less than the content's width
          //  _label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
        }
        else
          text = "&nbsp;";
      }
      _label.Text = text;

      _label.Height = Unit.Empty;
      _label.Width = Unit.Empty;
      _label.ApplyStyle (_commonStyle);
      _label.ApplyStyle (_labelStyle);
    }

    private void PreRenderEditModeValue ()
    {
      SetEditModeValue();

      _textBox.Enabled = Enabled;
      _textBox.Height = Unit.Empty;
      _textBox.Width = Unit.Empty;
      _textBox.ApplyStyle (_commonStyle);
      _textBoxStyle.ApplyStyle (_textBox);
    }

    protected bool IsCommandEnabled (bool isReadOnly)
    {
      if (WcagHelper.Instance.IsWaiConformanceLevelARequired ())
        return false;

      bool isCommandEnabled = false;
      if (Command != null)
      {
        bool isActive = Command.Show == CommandShow.Always
                        || isReadOnly && Command.Show == CommandShow.ReadOnly
                        || !isReadOnly && Command.Show == CommandShow.EditMode;
        bool isCommandLinkPossible = (IsReadOnly || ShowIcon) && Value != null;
        if (isActive
            && Command.Type != CommandType.None
            && isCommandLinkPossible)
          isCommandEnabled = Enabled;
      }
      return isCommandEnabled;
    }

    private bool ShowIcon
    {
      get
      {
        if (!EnableIcon)
          return false;
        if (Property == null)
          return false;
        if (GetIcon (Value, Property.ReferenceClass.BusinessObjectProvider) == null)
          return false;

        return true;
      }
    }

    protected bool EnableIcon { get; set; }

    /// <summary> Gets or sets the <see cref="IBusinessObjectReferenceProperty"/> object this control is bound to. </summary>
    /// <value> An <see cref="IBusinessObjectReferenceProperty"/> object. </value>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public new IBusinessObjectReferenceProperty Property
    {
      get { return (IBusinessObjectReferenceProperty) base.Property; }
      set { base.Property = value; }
    }

    /// <summary> Gets or sets the current value. </summary>
    [Browsable (false)]
    public new IBusinessObjectWithIdentity Value
    {
      get
      {
        if (_internalValue == null)
          _value = null;
            //  Only reload if value is outdated
        else if (_value == null || _value.UniqueIdentifier != _internalValue)
        {
          if (Property != null)
            _value = ((IBusinessObjectClassWithIdentity) Property.ReferenceClass).GetObject (_internalValue);
          else if (DataSource != null)
            _value = ((IBusinessObjectClassWithIdentity) DataSource.BusinessObjectClass).GetObject (_internalValue);
        }
        return _value;
      }
      set
      {
        IsDirty = true;

        _value = value;

        if (value != null)
        {
          _internalValue = value.UniqueIdentifier;
          _displayName = GetDisplayName (value);
        }
        else
        {
          _internalValue = null;
          _displayName = null;
        }
      }
    }

    /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
    /// <value> The value must be of type <see cref="IBusinessObjectWithIdentity"/>. </value>
    protected override object ValueImplementation
    {
      get { return Value; }
      set { Value = (IBusinessObjectWithIdentity) value; }
    }

    /// <summary> Gets or sets the <see cref="BocCommand"/> for this control's <see cref="Value"/>. </summary>
    /// <value> A <see cref="BocCommand"/>. </value>
    /// <remarks> This property is used for accessing the <see cref="BocCommand"/> at run time and for Designer support. </remarks>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Category ("Menu")]
    [Description ("The command rendered for this control's Value.")]
    [NotifyParentProperty (true)]
    public BocCommand Command
    {
      get { return (BocCommand) _command.ControlItem; }
      set
      {
        _command.ControlItem = value;
        if (value != null)
          _command.ControlItem.OwnerControl = this;
      }
    }

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> of the selected 
    ///   <see cref="IBusinessObjectWithIdentity"/>.
    /// </summary>
    /// <value> A string or <see langword="null"/> if no  <see cref="IBusinessObjectWithIdentity"/> is selected. </value>
    [Browsable (false)]
    public string BusinessObjectUniqueIdentifier
    {
      get { return _internalValue; }
    }

    /// <summary>
    ///   Gets the <see cref="IBusinessObject.DisplayName"/> of the selected 
    ///   <see cref="IBusinessObjectWithIdentity"/>.
    /// </summary>
    /// <value> A string or <see langword="null"/> if no  <see cref="IBusinessObjectWithIdentity"/> is selected. </value>
    [Browsable (false)]
    public string BusinessObjectDisplayName
    {
      get { return _displayName; }
    }

    /// <summary>
    ///   Returns the string to be used in the drop down list for the specified <see cref="IBusinessObjectWithIdentity"/>.
    /// </summary>
    /// <param name="businessObject"> The <see cref="IBusinessObjectWithIdentity"/> to get the display name for. </param>
    /// <returns> The display name for the specified <see cref="IBusinessObjectWithIdentity"/>. </returns>
    /// <remarks> 
    ///   <para>
    ///     Override this method to change the way the display name is composed. 
    ///   </para><para>
    ///     The default implementation used the <see cref="IBusinessObject.DisplayName"/> property to get the display name.
    ///   </para>
    /// </remarks>
    protected virtual string GetDisplayName (IBusinessObjectWithIdentity businessObject)
    {
      return businessObject.DisplayNameSafe;
    }

    /// <summary> 
    ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user interface.
    /// </summary>
    /// <returns> 
    ///   A <see cref="String"/> <see cref="Array"/> containing the <see cref="Control.ClientID"/> of the
    ///   <see cref="DropDownList"/> if the control is in edit mode, or an empty array if the control is read-only.
    /// </returns>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.GetTrackedClientIDs">BusinessObjectBoundEditableWebControl.GetTrackedClientIDs</seealso>
    public override string[] GetTrackedClientIDs ()
    {
      return IsReadOnly ? new string[0] : new[] { _textBox.ClientID };
    }

    /// <summary> The <see cref="BocReferenceValue"/> supports only scalar properties. </summary>
    /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="false"/>. </returns>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
    protected override bool SupportsPropertyMultiplicity (bool isList)
    {
      return !isList;
    }

    /// <summary>
    ///   The <see cref="BocReferenceValue"/> supports properties of types <see cref="IBusinessObjectReferenceProperty"/>.
    /// </summary>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
    protected override Type[] SupportedPropertyInterfaces
    {
      get { return s_supportedPropertyInterfaces; }
    }

    /// <summary>
    ///   Gets a flag that determines whether it is valid to generate HTML &lt;label&gt; tags referencing the
    ///   <see cref="TargetControl"/>.
    /// </summary>
    /// <value> Always <see langword="false"/>. </value>
    public override bool UseLabel
    {
      get { return false; }
    }

    /// <summary>
    ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its 
    ///   <see cref="Control.ClientID"/>.
    /// </summary>
    /// <value> The <see cref="DropDownList"/> if the control is in edit mode, otherwise the control itself. </value>
    public override Control TargetControl
    {
      get { return IsReadOnly ? (Control) this : _textBox; }
    }

    /// <summary> Gets the ID of the element to receive the focus when the page is loaded. </summary>
    /// <value>
    ///   Returns the <see cref="Control.ClientID"/> of the <see cref="DropDownList"/> if the control is in edit mode, 
    ///   otherwise <see langword="null"/>. 
    /// </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public string FocusID
    {
      get { return IsReadOnly ? null : _textBox.ClientID; }
    }

    /// <summary> This event is fired when the selection is changed between postbacks. </summary>
    [Category ("Action")]
    [Description ("Fires when the value of the control has changed.")]
    public event EventHandler SelectionChanged
    {
      add { Events.AddHandler (s_selectionChangedEvent, value); }
      remove { Events.RemoveHandler (s_selectionChangedEvent, value); }
    }

    /// <summary> Gets or sets a flag that determines whether to display the <see cref="OptionsMenu"/>. </summary>
    /// <value> <see langword="true"/> to show the <see cref="OptionsMenu"/>. The default value is <see langword="true"/>. </value>
    [Category ("Menu")]
    [Description ("Enables the options menu.")]
    [DefaultValue (true)]
    public bool ShowOptionsMenu { get; set; }

    /// <summary>
    ///   Gets the style that you want to apply to the <see cref="TextBox"/> (edit mode) 
    ///   and the <see cref="Label"/> (read-only mode).
    /// </summary>
    /// <remarks>
    ///   Use the <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> to assign individual 
    ///   style settings for the respective modes. Note that if you set one of the <b>Font</b> 
    ///   attributes (Bold, Italic etc.) to <see langword="true"/>, this cannot be overridden using 
    ///   <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/>  properties.
    /// </remarks>
    [Category ("Style")]
    [Description ("The style that you want to apply to the TextBox (edit mode) and the Label (read-only mode).")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public Style CommonStyle
    {
      get { return _commonStyle; }
    }

    /// <summary> Gets the style that you want to apply to the <see cref="TextBox"/> (edit mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
    [Category ("Style")]
    [Description ("The style that you want to apply to the TextBox (edit mode) only.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public SingleRowTextBoxStyle TextBoxStyle
    {
      get { return _textBoxStyle; }
    }

    /// <summary> Gets the style that you want to apply to the <see cref="Label"/> (read-only mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
    [Category ("Style")]
    [Description ("The style that you want to apply to the Label (read-only mode) only.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public Style LabelStyle
    {
      get { return _labelStyle; }
    }

    /// <summary> Gets the <see cref="TextBox"/> used in edit mode. </summary>
    [Browsable (false)]
    public TextBox TextBox
    {
      get { return _textBox; }
    }

    /// <summary> Gets the <see cref="HiddenField"/> used for posting the value back to the server.  </summary>
    [Browsable (false)]
    public HiddenField HiddenField
    {
      get { return _hiddenField; }
    }

    /// <summary> Gets the <see cref="Label"/> used in read-only mode. </summary>
    [Browsable (false)]
    public Label Label
    {
      get { return _label; }
    }

    /// <summary> Gets the <see cref="DropDownMenu"/> offering additional commands for the current <see cref="Value"/>. </summary>
    protected DropDownMenu OptionsMenu
    {
      get { return _optionsMenu; }
    }

    /// <summary> Gets or sets the validation error message. </summary>
    /// <value> 
    ///   The error message displayed when validation fails. The default value is an empty <see cref="String"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description ("Validation message displayed if there is an error.")]
    [Category ("Validator")]
    [DefaultValue ("")]
    public string ErrorMessage
    {
      get { return _errorMessage; }
      set
      {
        _errorMessage = value;
        for (int i = 0; i < _validators.Count; i++)
        {
          BaseValidator validator = (BaseValidator) _validators[i];
          validator.ErrorMessage = _errorMessage;
        }
      }
    }

    [Category ("AutoCompleteExtender")]
    [DefaultValue ("")]
    public string ServiceMethod
    {
      get { return _serviceMethod; }
      set { _serviceMethod = StringUtility.NullToEmpty (value); }
    }

    [Editor (typeof (UrlEditor), typeof (UITypeEditor))]
    [Category ("AutoCompleteExtender")]
    [DefaultValue ("")]
    public string ServicePath
    {
      get { return _servicePath; }
      set { _servicePath = StringUtility.NullToEmpty (value); }
    }

    [Category ("AutoCompleteExtender")]
    [DefaultValue (null)]
    public int? CompletionSetCount
    {
      get { return _completionSetCount; }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException ("value", "The CompletionSetCount must be greater than or equal to 0.");
        _completionSetCount = value;
      }
    }

    [Category ("AutoCompleteExtender")]
    [DefaultValue (1000)]
    public int CompletionInterval
    {
      get { return _completionInterval; }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException ("value", "The CompletionInterval must be greater than or equal to 0.");
        _completionInterval = value;
      }
    }

    [Category ("AutoCompleteExtender")]
    [DefaultValue (200)]
    public int SuggestionInterval
    {
      get { return _suggestionInterval; }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException ("value", "The SuggestionInterval must be greater than or equal to 0.");
        _suggestionInterval = value;
      }
    }

    [Category ("AutoCompleteExtender")]
    [DefaultValue ("")]
    public string Args
    {
      get { return _args; }
      set { _args = value; }
    }

    #region protected virtual string CssClass...

    /// <summary> Gets the CSS-Class applied to the <see cref="BocAutoCompleteReferenceValue"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>bocAutoCompleteReferenceValue</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    protected virtual string CssClassBase
    {
      get { return "bocAutoCompleteReferenceValue"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocAutoCompleteReferenceValue"/>'s value. </summary>
    /// <remarks> Class: <c>bocAutoCompleteReferenceValue</c> </remarks>
    protected virtual string CssClassContent
    {
      get { return "bocReferenceValueContent"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocAutoCompleteReferenceValue"/> when it is displayed in read-only mode. </summary>
    /// <remarks> 
    ///   <para> Class: <c>readOnly</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocAutoCompleteReferenceValue.readOnly</c> as a selector. </para>
    /// </remarks>
    protected virtual string CssClassReadOnly
    {
      get { return "readOnly"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocAutoCompleteReferenceValue"/> when it is displayed in read-only mode. </summary>
    /// <remarks> 
    ///   <para> Class: <c>disabled</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocAutoCompleteReferenceValue.disabled</c> as a selector. </para>
    /// </remarks>
    protected virtual string CssClassDisabled
    {
      get { return "disabled"; }
    }

    /// <summary> Gets the CSS-Class applied to the drop down panel of the <see cref="BocAutoCompleteReferenceValue"/>. </summary>
    /// <remarks> 
    ///   <para> Class: <c>bocAutoCompleteReferenceValueDropDownPanel</c>. </para>
    /// </remarks>
    protected virtual string CssClassDropDownPanel
    {
      get { return "bocAutoCompleteReferenceValueDropDownPanel"; }
    }

    #endregion

    string IBocAutoCompleteReferenceValue.TextBoxUniqueID
    {
      get { return _textBox.UniqueID; }
    }

    public string TextBoxClientID
    {
      get { return _textBox.ClientID; }
    }

    string IBocAutoCompleteReferenceValue.DropDownButtonClientID
    {
      get { return ClientID + "_Boc_DropDownButton"; }
    }

    public string HiddenFieldClientID
    {
      get { return _hiddenField.ClientID; }
    }

    string IBocAutoCompleteReferenceValue.HiddenFieldUniqueID
    {
      get { return _hiddenField.UniqueID; }
    }

    bool IBocRenderableControl.IsDesignMode
    {
      get { return base.IsDesignMode; }
    }

    IDropDownMenu IBocAutoCompleteReferenceValue.OptionsMenu
    {
      get { return _optionsMenu; }
    }
  }
}