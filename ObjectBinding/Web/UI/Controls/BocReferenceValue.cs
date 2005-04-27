using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.Design;
using System.Web.UI.HtmlControls;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;
using Rubicon.Web;
using Rubicon.Web.UI;
using Rubicon.Web.Utilities;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Globalization;
using Rubicon.ObjectBinding.Web.Design;

namespace Rubicon.ObjectBinding.Web.Controls
{
/// <summary> This control can be used to display or edit reference values. </summary>
/// <include file='doc\include\Controls\BocReferenceValue.xml' path='BocReferenceValue/Class/*' />
// TODO: see "Doc\Bugs and ToDos.txt"
[ValidationProperty ("Value")]
[DefaultEvent ("SelectionChanged")]
[ToolboxItemFilter("System.Web.UI")]
[Designer (typeof (BocReferenceValueDesigner))]
public class BocReferenceValue: BusinessObjectBoundModifiableWebControl, IPostBackEventHandler, IPostBackDataHandler
{
  // constants
	
  private const string c_nullIdentifier = "--null--";

  /// <summary> The text displayed when control is displayed in desinger, is read-only, and has no contents. </summary>
  private const string c_designModeEmptyLabelContents = "##";
  private const string c_defaultControlWidth = "150pt";

  private const string c_bocReferenceValueScriptUrl = "BocReferenceValue.js";

  // types

  /// <summary> A list of control specific resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:Rubicon.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
  ///   See the documentation of <b>GetString</b> for further details.
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Rubicon.ObjectBinding.Web.Globalization.BocReferenceValue")]
  protected enum ResourceIdentifier
  {
    /// <summary> Label displayed in the OptionsMenu. </summary>
    OptionsTitle,
    /// <summary> The validation error message displayed when the null item is selected. </summary>
    NullItemValidationMessage
  }

  // static members

  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectReferenceProperty) };

  private static readonly object s_selectionChangedEvent = new object();
  private static readonly object s_menuItemClickEvent = new object();
  private static readonly object s_commandClickEvent = new object();

	// member fields

  private bool _isDirty = true;

  private DropDownList _dropDownList;
  private Label _label;
  private Image _icon ;

  private Style _commonStyle;
  private DropDownListStyle _dropDownListStyle;
  private Style _labelStyle;

  /// <summary> 
  ///   The object returned by <see cref="BocReferenceValue"/>. 
  ///   Does not require <see cref="System.Runtime.Serialization.ISerializable"/>. 
  /// </summary>
  private IBusinessObjectWithIdentity _value = null;

  /// <summary> The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> of the current object. </summary>
  private string _internalValue = null;

  private bool _enableIcon = true;
  private string _select = String.Empty;

  private DropDownMenu _optionsMenu;
  private string _optionsTitle;
  private bool _showOptionsMenu = true;
  private Unit _optionsMenuWidth = Unit.Empty;
  private BocMenuItemCollection _optionsMenuItems;
  /// <summary> Contains the <see cref="BocMenuItem"/> objects during the handling of the post back events. </summary>
  private BocMenuItem[] _optionsMenuItemsPostBackEventHandlingPhase;
  /// <summary> Contains the <see cref="BocMenuItem"/> objects during the rendering phase. </summary>
  private BocMenuItem[] _optionsMenuItemsRenderPhase;
  
  /// <summary> The command rendered for this reference value. </summary>
  private SingleControlItemCollection _command = null;

  private string _errorMessage;
  private ArrayList _validators;

  // construction and disposing

  /// <summary> Initializes a new instance of the <b>BocReferenceValue</b> class. </summary>
	public BocReferenceValue()
	{
    _optionsMenuItems = new BocMenuItemCollection (this);
    _commonStyle = new Style();
    _dropDownListStyle = new DropDownListStyle();
    _labelStyle = new Style();
    _icon = new Image();
    _dropDownList = new DropDownList();
    _label = new Label();
    _optionsMenu = new DropDownMenu (this);
    _validators = new ArrayList();
    _command = new SingleControlItemCollection (new BocCommand(), new Type[] {typeof (BocCommand)});
  }

	// methods and properties

  /// <summary> Overrides the <see cref="Control.CreateChildControls"/> method. </summary>
  /// <remarks>
  ///   If the <see cref="ListControl"/> could not be created from <see cref="ListControlStyle"/>,
  ///   the control is set to read-only.
  /// </remarks>
  protected override void CreateChildControls()
  {
    _icon.ID = ID + "_Boc_Icon";
    _icon.EnableViewState = false;
    Controls.Add (_icon);

    _dropDownList.ID = ID + "_Boc_DropDownList";
    _dropDownList.EnableViewState = true;
    Controls.Add (_dropDownList);

    _label.ID = ID + "_Boc_Label";
    _label.EnableViewState = false;
    Controls.Add (_label);
    
    _optionsMenu.ID = ID + "_Boc_OptionsMenu";
    Controls.Add (_optionsMenu);
    _optionsMenu.EventCommandClick += new WebMenuItemClickEventHandler (OptionsMenu_EventCommandClick);
    _optionsMenu.WxeFunctionCommandClick += new WebMenuItemClickEventHandler (OptionsMenu_WxeFunctionCommandClick);
  }

  /// <summary> Overrides the <see cref="Control.OnLoad"/> method. </summary>
  /// <remarks> Populates the list. </remarks>
  protected override void OnLoad (EventArgs e)
  {
    base.OnLoad (e);

    if (! IsDesignMode && ! Page.IsPostBack)
      RefreshBusinessObjectList();

    _optionsMenu.MenuItems.Clear();
    _optionsMenu.MenuItems.AddRange (EnsureOptionsMenuItemsForPreviousLifeCycleGot());
  }

  /// <summary> Calls the <see cref="LoadPostData"/> method. </summary>
  bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
  {
    return LoadPostData (postDataKey, postCollection);
  }

  /// <summary> Calls the <see cref="RaisePostDataChangedEvent"/> method. </summary>
  void IPostBackDataHandler.RaisePostDataChangedEvent()
  {
    RaisePostDataChangedEvent();
  }

  /// <summary>
  ///   Uses the <paramref name="postCollection"/> to determine whether the value of this control has been changed
  ///   between postbacks.
  /// </summary>
  /// <include file='doc\include\Controls\BocReferenceValue.xml' path='BocReferenceValue/LoadPostData/*' />
  protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
  {
    string newValue = PageUtility.GetRequestCollectionItem (Page, _dropDownList.UniqueID);
    bool isDataChanged = false;
    if (newValue != null)
    {
      if (_internalValue == null && newValue != c_nullIdentifier)
        isDataChanged = true;
      else if (_internalValue != null && newValue != _internalValue)
        isDataChanged = true;
    }

    if (isDataChanged)
    {
      if (newValue == c_nullIdentifier)
        InternalValue = null;
      else
        InternalValue = newValue;
      _isDirty = true;
    }
    return isDataChanged;
  }

  /// <summary> Called when the state of the control has changed between postbacks. </summary>
  protected virtual void RaisePostDataChangedEvent()
  {
    OnSelectionChanged();
  }

  /// <summary> Fires the <see cref="SelectionChanged"/> event. </summary>
  protected virtual void OnSelectionChanged ()
  {
    EventHandler eventHandler = (EventHandler) Events[s_selectionChangedEvent];
    if (eventHandler != null)
      eventHandler (this, EventArgs.Empty);
  }

  /// <summary> Calls the <see cref="RaisePostBackEvent"/> method. </summary>
  void IPostBackEventHandler.RaisePostBackEvent (string eventArgument)
  {
    RaisePostBackEvent (eventArgument);
  }

  /// <summary> Called when the contorl caused a post back. </summary>
  /// <param name="eventArgument"> <see cref="String.Empty"/>. </param>
  protected virtual void RaisePostBackEvent (string eventArgument)
  {
    HandleCommand();
  }

  /// <summary> Handles post back events raised by the value's <see cref="Command"/>. </summary>
  private void HandleCommand()
  {
    switch (Command.Type)
    {
      case CommandType.Event:
      {
        OnCommandClick (Value);
        break;
      }
      case CommandType.WxeFunction:
      {
        Command.ExecuteWxeFunction ((IWxePage) Page, Value);
        break;
      }
      default:
      {
        break;
      }
    }
  }

  /// <summary> Fires the <see cref="CommandClick"/> event. </summary>
  /// <param name="businessObject"> 
  ///   The current <see cref="Value"/>, which corresponds to the clicked <see cref="IBusinessObjectWithIdentity"/>,
  ///   unless somebody changed the <see cref="Value"/> in the code behind before the event fired.
  /// </param>
  protected virtual void OnCommandClick (IBusinessObjectWithIdentity businessObject)
  {
    BocCommandClickEventHandler commandClickHandler = (BocCommandClickEventHandler) Events[s_commandClickEvent];
    if (Command != null)
      Command.OnClick (businessObject);
    if (commandClickHandler != null)
    {
      BocCommandClickEventArgs e = new BocCommandClickEventArgs (businessObject);
      commandClickHandler (this, e);
    }
  }

  /// <summary> Overrides the <see cref="Control.OnPreRender"/> method. </summary>
  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);

    if (! IsDesignMode && ! IsReadOnly && Enabled)
      Page.RegisterRequiresPostBack (this);
  }

  /// <summary> Overrides the <see cref="WebControl.AddAttributesToRender"/> method. </summary>
  protected override void AddAttributesToRender (HtmlTextWriter writer)
  {
    base.AddAttributesToRender (writer);
    string cssClass;
    if (StringUtility.IsNullOrEmpty (CssClass))
      cssClass = CssClassBase;
    else
      cssClass = CssClass;  
    //    cssClass += " " + CssClassOnMouseOut;
    writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
    
    //    string script = "BocReferenceValue_OnMouseOver (this, '" + CssClassOnMouseOver + "');";
    //    writer.AddAttribute ("onMouseOver", script);
    //    
    //    script = "BocReferenceValue_OnMouseOut (this, '" + CssClassOnMouseOut + "');";
    //    writer.AddAttribute ("onMouseOut", script);
  }

  /// <summary> Overrides the <see cref="Control.RenderContents"/> method. </summary>
  protected override void RenderContents (HtmlTextWriter writer)
  {
    bool isReadOnly = IsReadOnly;

    bool isControlHeightEmpty = Height.IsEmpty && StringUtility.IsNullOrEmpty (Style["height"]);
    bool isDropDownListHeightEmpty = StringUtility.IsNullOrEmpty (_dropDownList.Style["height"]);
    bool isControlWidthEmpty = Width.IsEmpty && StringUtility.IsNullOrEmpty (Style["width"]);
    bool isLabelWidthEmpty = StringUtility.IsNullOrEmpty (_label.Style["width"]);
    bool isDropDownListWidthEmpty = StringUtility.IsNullOrEmpty (_dropDownList.Style["width"]);
    if (isReadOnly)
    {
      if (isLabelWidthEmpty && ! isControlWidthEmpty)
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
    }
    else
    {
      if (! isControlHeightEmpty && isDropDownListHeightEmpty)
        writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
    
      if (isDropDownListWidthEmpty)
      {
        if (isControlWidthEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
        else
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      }
    }

    writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
    writer.AddStyleAttribute ("display", "inline");
    writer.RenderBeginTag (HtmlTextWriterTag.Table);  // Begin table
    writer.RenderBeginTag (HtmlTextWriterTag.Tr); //  Begin tr

    bool isCommandEnabled = false;
    if (Command != null)
    {
      bool isActive =    Command.Show == CommandShow.Always
                      || isReadOnly && Command.Show == CommandShow.ReadOnly
                      || ! isReadOnly && Command.Show == CommandShow.EditMode;
      bool isCommandLinkPossible = (IsReadOnly || _icon.Visible) && InternalValue != null;
      if (   isActive
          && Command.Type != CommandType.None
          && isCommandLinkPossible)
      {
          isCommandEnabled = Enabled;
      }
    }

    string argument = string.Empty;
    string postBackEvent = Page.GetPostBackClientEvent (this, argument);
    string objectID = string.Empty;
    if (InternalValue != c_nullIdentifier)
      objectID = InternalValue;


    if (_icon.Visible)
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      writer.AddStyleAttribute ("padding-right", "3pt");
      writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Span); //  Begin span

      if (isCommandEnabled)
        Command.RenderBegin (writer, postBackEvent, string.Empty, objectID);
      _icon.RenderControl (writer);
      if (isCommandEnabled)
        Command.RenderEnd (writer);

      writer.RenderEndTag();  //  End span
      writer.RenderEndTag();  //  End td
    }   

    if (isReadOnly)
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "auto");
      writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Span); //  Begin span

      if (isCommandEnabled)
        Command.RenderBegin (writer, postBackEvent, string.Empty, objectID);
      _label.RenderControl (writer);
      if (isCommandEnabled)
        Command.RenderEnd (writer);
      
      writer.RenderEndTag();  //  End span
      writer.RenderEndTag();  //  End td
    }
    else
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1%");
      writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Span); //  Begin span

      if (! isControlHeightEmpty && isDropDownListHeightEmpty)
        writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
      if (isDropDownListWidthEmpty)
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      _dropDownList.RenderControl (writer);
      
      writer.RenderEndTag();  //  End span
      writer.RenderEndTag();  //  End td
    }

    if (HasOptionsMenu)
    {
      writer.AddStyleAttribute ("padding-left", "3pt");
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      //writer.AddAttribute ("align", "right");
      writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
      _optionsMenu.Width = _optionsMenuWidth;
      _optionsMenu.RenderControl (writer);
      writer.RenderEndTag();  //  End td
    }

    writer.RenderEndTag();
    writer.RenderEndTag();
  }

  /// <summary> Overrides the <see cref="Control.LoadViewState"/> method. </summary>
  protected override void LoadViewState (object savedState)
  {
    object[] values = (object[]) savedState;

    base.LoadViewState (values[0]);
    if (values[1] != null)    
      InternalValue = (string) values[1];  
    _isDirty = (bool) values[2];

    //  Drop down list has enabled view state, selected value must not be restored
  }

  /// <summary> Overrides the <see cref="Control.SaveViewState"/> method. </summary>
  protected override object SaveViewState()
  {
    object[] values = new object[3];

    values[0] = base.SaveViewState();
    values[1] = _internalValue;
    values[2] = _isDirty;

    return values;
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.LoadValue"/> method. </summary>
  /// <include file='doc\include\Controls\BocReferenceValue.xml' path='BocReferenceValue/LoadValue/*' />
  public override void LoadValue (bool interim)
  {
    if (! interim)
    {
      if (Property != null && DataSource != null && DataSource.BusinessObject != null)
      {
        ValueImplementation = DataSource.BusinessObject.GetProperty (Property);
        _isDirty = false;
      }
    }
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundModifiableWebControl.SaveValue"/> method. </summary>
  /// <include file='doc\include\Controls\BocReferenceValue.xml' path='BocReferenceValue/SaveValue/*' />
  public override void SaveValue (bool interim)
  {
    if (! interim)
    {
      if (Property != null && DataSource != null && DataSource.BusinessObject != null && ! IsReadOnly)
        DataSource.BusinessObject.SetProperty (Property, Value);
    }
  }

  /// <summary> Returns the <see cref="IResourceManager"/> used to access the resources for this control. </summary>
  protected virtual IResourceManager GetResourceManager()
  {
    return GetResourceManager (typeof (ResourceIdentifier));
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundModifiableWebControl.CreateValidators"/> method. </summary>
  /// <include file='doc\include\Controls\BocReferenceValue.xml' path='BocReferenceValue/CreateValidators/*' />
  public override BaseValidator[] CreateValidators()
  {
    if (IsReadOnly || ! IsRequired)
      return new BaseValidator[0];

    BaseValidator[] validators = new BaseValidator[1];
    
    CompareValidator notNullItemValidator = new CompareValidator();
    notNullItemValidator.ID = ID + "_ValidatorNotNullItem";
    notNullItemValidator.ControlToValidate = TargetControl.ID;
    notNullItemValidator.ValueToCompare = c_nullIdentifier;
    notNullItemValidator.Operator = ValidationCompareOperator.NotEqual;
    if (StringUtility.IsNullOrEmpty (_errorMessage))
    {
      notNullItemValidator.ErrorMessage = 
          GetResourceManager().GetString (ResourceIdentifier.NullItemValidationMessage);
    }
    else
    {
      notNullItemValidator.ErrorMessage = _errorMessage;
    }      
    validators[0] = notNullItemValidator;

    _validators.AddRange (validators);
    return validators;
  }

  /// <summary> Sets the <see cref="IBusinessObjectWithIdentity"/> objects to be displayed in edit mode. </summary>
  /// <remarks>
  ///   Use this method to set the listed items, e.g. from the parent control, if no <see cref="Select"/>
  ///   statement was provided.
  /// </remarks>
  /// <param name="businessObjects">
  ///   An array of <see cref="IBusinessObjectWithIdentity"/> objects to be used as list of possible values.
  ///   Must not be <see langword="null"/>.
  /// </param>
  public void SetBusinessObjectList (IBusinessObjectWithIdentity[] businessObjects)
  {
    ArgumentUtility.CheckNotNull ("businessObjects", businessObjects);

    RefreshBusinessObjectList (businessObjects);
  }

  /// <summary>
  ///   Queries <see cref="IBusinessObjectReferenceProperty.SearchAvailableObjects"/> for the
  ///   <see cref="IBusinessObjectWithIdentity"/> objects to be displayed in edit mode.
  /// </summary>
  /// <remarks> 
  ///   Uses the <see cref="Select"/> statement to query the <see cref="Property"/>'s 
  ///   <see cref="IBusinessObjectReferenceProperty.SearchAvailableObjects"/> method for the list contents.
  /// </remarks>
  protected void RefreshBusinessObjectList()
  {
    if (Property == null)
      return;

    IBusinessObjectWithIdentity[] businessObjects = null;

    //  Get all matching business objects
    if (DataSource != null && DataSource.BusinessObject != null)
      businessObjects = Property.SearchAvailableObjects (DataSource.BusinessObject, _select);

    RefreshBusinessObjectList (businessObjects);
  }

  /// <summary>
  ///   Populates the <see cref="DropDownList"/> with the items passed in <paramref name="businessObjects"/>.
  /// </summary>
  /// <remarks>
  ///   This method controls the actual refilling of the <see cref="DropDownList"/>.
  /// </remarks>
  /// <param name="businessObjects">
  ///   The array of <see cref="IBusinessObjectWithIdentity"/> objects to populate the <see cref="DropDownList"/>.
  /// </param>
  protected virtual void RefreshBusinessObjectList (IBusinessObjectWithIdentity[] businessObjects)
  {
    if (! IsReadOnly)
    {
      _dropDownList.Items.Clear();

      //  Add Undefined item
      if (! IsRequired)
        _dropDownList.Items.Add (CreateNullItem());

      if (businessObjects != null)
      {
        //  Populate _dropDownList
        foreach (IBusinessObjectWithIdentity businessObject in businessObjects)
        {
          ListItem item = new ListItem (businessObject.DisplayName, businessObject.UniqueIdentifier);
          _dropDownList.Items.Add (item);
        }
      }
    }
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.PreRenderChildControls"/> method. </summary>
  protected override void PreRenderChildControls()
  {
    string key = typeof (BocReferenceValue).FullName + "_Script";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      string scriptUrl = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (BocReferenceValue), ResourceType.Html, c_bocReferenceValueScriptUrl);
      HtmlHeadAppender.Current.RegisterJavaScriptInclude (key, scriptUrl);
    }

    key = typeof (BocReferenceValue).FullName+ "_Startup";
    if (! Page.IsStartupScriptRegistered (key))
    {
      string script = string.Format ("BocReferenceValue_InitializeGlobals ('{0}');", c_nullIdentifier);
      PageUtility.RegisterStartupScriptBlock (Page, key, script);
    }

    key = typeof (BocReferenceValue).FullName + "_Style";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      string url = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (BocReferenceValue), ResourceType.Html, "BocReferenceValue.css");
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, url);
    }

    PreRenderIcon();

    if (HasOptionsMenu)
      PreRenderOptionsMenu();

    if (IsReadOnly)
      PreRenderReadOnlyValue();
    else
      PreRenderEditModeValue();
  }

  /// <summary> Prerenders the <see cref="Label"/>. </summary>
  private void PreRenderReadOnlyValue()
  {
    string text;
    if (Value != null)
      text = HttpUtility.HtmlEncode (Value.DisplayName);
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
      {
        text = "&nbsp;";
      }
    }
    _label.Text = text;

    _label.Enabled = Enabled;
    _label.Height = Unit.Empty;
    _label.Width = Unit.Empty;
    _label.ApplyStyle (_commonStyle);
    _label.ApplyStyle (_labelStyle);
  }

  /// <summary> Prerenders the <see cref="DropDownList"/>. </summary>
  private void PreRenderEditModeValue()
  {
    bool isNullItem = InternalValue == null;

    //  Check if null item is to be selected
    if (isNullItem)
    {
      //  No null item in the list
      if (_dropDownList.Items.FindByValue (c_nullIdentifier) == null)
        _dropDownList.Items.Insert (0, CreateNullItem());
      _dropDownList.SelectedValue = c_nullIdentifier;
    }
    else
    {
      if (_dropDownList.Items.FindByValue (InternalValue) != null)
      {
        _dropDownList.SelectedValue = InternalValue;
      }
      else if (Value != null)
      {
        //  Item not yet in the list but is a valid item.
        IBusinessObjectWithIdentity businessObject = Value;

        ListItem item = new ListItem (businessObject.DisplayName, businessObject.UniqueIdentifier);
        _dropDownList.Items.Add (item);

        _dropDownList.SelectedValue = InternalValue;
      }
    }

    _dropDownList.Enabled = Enabled;
    _dropDownList.Height = Unit.Empty;
    _dropDownList.Width = Unit.Empty;
    _dropDownList.ApplyStyle (_commonStyle);
    _dropDownListStyle.ApplyStyle (_dropDownList);
  }

  /// <summary> Prerenders the <see cref="Icon"/>. </summary>
  private void PreRenderIcon()
  {
    _icon.Visible = false;

    //  Get icon
    if (_enableIcon && Property != null)
    {
      IconInfo iconInfo = BusinessObjectBoundWebControl.GetIcon (Value, Property.ReferenceClass.BusinessObjectProvider);

      if (iconInfo != null)
      {
        _icon.ImageUrl = iconInfo.Url;
        _icon.Width = iconInfo.Width;
        _icon.Height = iconInfo.Height;

        _icon.Enabled = Enabled;
        _icon.Visible = _enableIcon;
      }
    }
  }

  /// <summary> Prerenders the <see cref="_optionsMenu"/>. </summary>
  private void PreRenderOptionsMenu()
  {
    _optionsMenu.Enabled = Enabled;
    _optionsMenu.MenuItems.Clear();
    _optionsMenu.MenuItems.AddRange (EnsureOptionsMenuItemsGot (true));
    if (StringUtility.IsNullOrEmpty (_optionsTitle))
      _optionsMenu.TitleText = GetResourceManager().GetString (ResourceIdentifier.OptionsTitle);
    else
      _optionsMenu.TitleText = _optionsTitle;

    string getSelectionCount;
    if (IsReadOnly)
    {
      if (InternalValue != null)
        getSelectionCount = "function() { return 1; }";
      else 
        getSelectionCount = "function() { return 0; }";
    }
    else
      getSelectionCount = "function() { return BocReferenceValue_GetSelectionCount ('" + _dropDownList.ClientID + "'); }";
    _optionsMenu.GetSelectionCount = getSelectionCount;
  }

  /// <summary> Gets a flag describing whether the <b>OptionsMenu</b> is visible. </summary>
  private bool HasOptionsMenu
  {
    get { return _showOptionsMenu && EnsureOptionsMenuItemsGot().Length > 0; }
  }

  /// <summary> Creates the <see cref="ListItem"/> symbolizing the undefined selection. </summary>
  /// <returns> A <see cref="ListItem"/>. </returns>
  private ListItem CreateNullItem()
  {
    ListItem emptyItem = new ListItem (string.Empty, c_nullIdentifier);
    return emptyItem;
  }

  /// <summary> 
  ///   Ensures that the menu items for the <b>OptionsMenu</b> from the previous page life cycle have been loaded.
  /// </summary>
  /// <param name="forceRefresh"> <see langword="true"/> to override the ensure pattern. </param>
  /// <returns> An array of <see cref="BocMenuItem"/> objects to be used by the <b>OptionsMenu</b>. </returns>
  private BocMenuItem[] EnsureOptionsMenuItemsForPreviousLifeCycleGot()
  {
    if (_optionsMenuItemsPostBackEventHandlingPhase == null)
    {
      _optionsMenuItemsPostBackEventHandlingPhase = 
          GetOptionsMenuItemsForPreviousLifeCycle (_optionsMenuItems.ToArray());
    }
    return _optionsMenuItemsPostBackEventHandlingPhase;
  }

  /// <summary> Ensures that the menu items for the <b>OptionsMenu</b> have been loaded. </summary>
  /// <param name="forceRefresh"> <see langword="true"/> to override the ensure pattern. </param>
  /// <returns> An array of <see cref="BocMenuItem"/> objects to be used by the <b>OptionsMenu</b>. </returns>
  private BocMenuItem[] EnsureOptionsMenuItemsGot (bool forceRefresh)
  {
    if (_optionsMenuItemsRenderPhase == null || forceRefresh)
      _optionsMenuItemsRenderPhase = GetOptionsMenuItems (_optionsMenuItems.ToArray());
    return _optionsMenuItemsRenderPhase;
  }

  /// <summary> Ensures that the menu items for the <b>OptionsMenu</b> have been loaded. </summary>
  /// <returns> An array of <see cref="BocMenuItem"/> objects to be used by the <b>OptionsMenu</b>. </returns>
  private BocMenuItem[] EnsureOptionsMenuItemsGot()
  {
    return EnsureOptionsMenuItemsGot (false);
  }

  /// <summary> Gets the <see cref="BocMenuItem"/> objects used during the previous life cycle. </summary>
  /// <include file='doc\include\Controls\BocReferenceValue.xml' path='BocReferenceValue/GetOptionsMenuItemsForPreviousLifeCycle/*' />
  private BocMenuItem[] GetOptionsMenuItemsForPreviousLifeCycle (BocMenuItem[] menuItems)
  {
    //  return menuItems;
    return EnsureOptionsMenuItemsGot (true);
  }

  /// <summary> Gets the <see cref="BocMenuItem"/> objects used during the current life cycle. </summary>
  /// <include file='doc\include\Controls\BocReferenceValue.xml' path='BocReferenceValue/GetOptionsMenuItems/*' />
  protected virtual BocMenuItem[] GetOptionsMenuItems (BocMenuItem[] menuItems)
  {
    return menuItems;
  }

  /// <summary> Handles the <see cref="OptionsMenu.EventCommandClick"/> event of the <b>OptionsMenu</b>. </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="WebMenuItemClickEventArgs"/> object that contains the event data. </param>
  private void OptionsMenu_EventCommandClick (object sender, WebMenuItemClickEventArgs e)
  {
    OnMenuItemEventCommandClick ((BocMenuItem) e.Item);
  }

  /// <summary> 
  ///   Calls the <see cref="BocMenuItemCommand.OnClick"/> method of the <paramref name="menuItem"/>'s 
  ///   <see cref="BocMenuItem.Command"/> and raises <see cref="MenuItemClick"/> event. 
  /// </summary>
  /// <param name="menuItem"> The <see cref="BocMenuItem"/> that has been clicked. </param>
  /// <remarks> Only called for commands of type <see cref="CommandType.Event"/>. </remarks>
  protected virtual void OnMenuItemEventCommandClick (BocMenuItem menuItem)
  {
    WebMenuItemClickEventHandler menuItemClickHandler = (WebMenuItemClickEventHandler) Events[s_menuItemClickEvent];
    if (menuItem != null && menuItem.Command != null)
      ((BocMenuItemCommand) menuItem.Command).OnClick (menuItem);
    if (menuItemClickHandler != null)
    {
      WebMenuItemClickEventArgs e = new WebMenuItemClickEventArgs (menuItem);
      menuItemClickHandler (this, e);
    }
  }

  /// <summary> Is raised when a menu item with a command of type <see cref="CommandType.Event"/> is clicked. </summary>
  [Category ("Action")]
  [Description ("Is raised when a menu item with a command of type Event is clicked.")]
  public event WebMenuItemClickEventHandler MenuItemClick
  {
    add { Events.AddHandler (s_menuItemClickEvent, value); }
    remove { Events.RemoveHandler (s_menuItemClickEvent, value); }
  }

  /// <summary> Handles the <see cref="OptionsMenu.WxeFunctionCommandClick"/> event of the <b>OptionsMenu</b>. </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="WebMenuItemClickEventArgs"/> object that contains the event data. </param>
  /// <remarks> Only called for commands of type <see cref="CommandType.Event"/>. </remarks>
  private void OptionsMenu_WxeFunctionCommandClick (object sender, WebMenuItemClickEventArgs e)
  {
    OnMenuItemWxeFunctionCommandClick ((BocMenuItem) e.Item);
  }

  /// <summary> 
  ///   Calls the <see cref="BocMenuItemCommand.ExecuteWxeFunction"/> method of the <paramref name="menuItem"/>'s 
  ///   <see cref="BocMenuItem.Command"/>.
  /// </summary>
  /// <param name="menuItem"> The <see cref="BocMenuItem"/> that has been clicked. </param>
  /// <remarks> Only called for commands of type <see cref="CommandType.WxeFunction"/>. </remarks>
  protected virtual void OnMenuItemWxeFunctionCommandClick (BocMenuItem menuItem)
  {
    if (menuItem != null && menuItem.Command != null)
    {
      int[] indices = new int[0];
      IBusinessObject[] businessObjects;
      if (Value != null)
        businessObjects = new IBusinessObject[] { Value };
      else
        businessObjects = new IBusinessObject[0];
 
      BocMenuItemCommand command = (BocMenuItemCommand) menuItem.Command;
      command.ExecuteWxeFunction ((IWxePage) Page, indices, businessObjects);
    }
  }

  /// <summary> The <see cref="IBusinessObjectReferenceProperty"/> object this control is bound to. </summary>
  /// <remarks> Explicit setting of <see cref="Property"/> is not offically supported. </remarks>
  /// <value>An <see cref="IBusinessObjectReferenceProperty"/> object.</value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public new IBusinessObjectReferenceProperty Property
  {
    get { return (IBusinessObjectReferenceProperty) base.Property; }
    set 
    {
      ArgumentUtility.CheckType ("value", value, typeof (IBusinessObjectReferenceProperty));

      base.Property = (IBusinessObjectReferenceProperty) value; 
    }
  }

  /// <summary> Gets or sets the current value. </summary>
  /// <value> 
  ///   The <see cref="IBusinessObjectWithIdentity"/> currently displayed 
  ///   or <see langword="null"/> if no item / the null item is selected.
  /// </value>
  [Browsable (false)]
  public new IBusinessObjectWithIdentity Value
  {
    get 
    {
      if (InternalValue == null)
      {
        _value = null;
      }
      //  Only reload if value is outdated
      else if (   Property != null
               && (   _value == null
                   || _value.UniqueIdentifier != InternalValue))
      {
        _value = ((IBusinessObjectClassWithIdentity) Property.ReferenceClass).GetObject (InternalValue);
      }

      return _value;
    }
    set 
    { 
      IBusinessObjectWithIdentity businessObjectWithIdentity = value;
      _value = businessObjectWithIdentity; 
      
      if (businessObjectWithIdentity != null)
        InternalValue = businessObjectWithIdentity.UniqueIdentifier;
      else
        InternalValue = null;
    }
  }

  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = (IBusinessObjectWithIdentity) value; }
  }

  /// <summary> Gets or sets the current value. </summary>
  /// <value> 
  ///   The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> for the current 
  ///   <see cref="IBusinessObjectWithIdentity"/> object 
  ///   or <see langword="null"/> if no item / the null item is selected.
  /// </value>
  protected string InternalValue
  {
    get { return _internalValue; }
    set 
    {
      if (_internalValue == value)
        return;

      bool isOldInternalValueNull = _internalValue == null;

      _internalValue = StringUtility.EmptyToNull (value);

      bool removeNullItem =    IsRequired 
                            && isOldInternalValueNull
                            && _internalValue != null;
      
      if (removeNullItem)
      {
        ListItem itemToRemove = _dropDownList.Items.FindByValue (c_nullIdentifier);
        _dropDownList.Items.Remove (itemToRemove);
      }
    }
  }

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its ClientID.
  /// </summary>
  public override Control TargetControl 
  {
    get { return (! IsReadOnly) ? _dropDownList : (Control) this; }
  }

  public override bool IsDirty
  {
    get { return _isDirty; }
    set { _isDirty = value; }
  }

  /// <summary>
  ///   The list of<see cref="Type"/> objects for the <see cref="IBusinessObjectProperty"/> 
  ///   implementations that can be bound to this control.
  /// </summary>
  protected override Type[] SupportedPropertyInterfaces
  {
    get { return s_supportedPropertyInterfaces; }
  }

  /// <summary> Overrides <see cref="Rubicon.Web.UI.ISmartControl.UseLabel"/>. </summary>
  public override bool UseLabel
  {
    get { return false; }
  }

  /// <summary> This event is fired when the selection is changed between postbacks. </summary>
  [Category ("Action")]
  [Description ("Fires when the value of the control has changed.")]
  public event EventHandler SelectionChanged
  {
    add { Events.AddHandler (s_selectionChangedEvent, value); }
    remove { Events.RemoveHandler (s_selectionChangedEvent, value); }
  }

  /// <summary> This event is fired when the value's command is clicked. </summary>
  [Category ("Action")]
  [Description ("Fires when the value's command is clicked.")]
  public event EventHandler CommandClick
  {
    add { Events.AddHandler (s_commandClickEvent, value); }
    remove { Events.RemoveHandler (s_commandClickEvent, value); }
  }

  /// <summary> The style that you want to apply to the TextBox (edit mode) and the Label (read-only mode). </summary>
  /// <remarks>
  ///   Use the <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> to assign individual 
  ///   style settings for the respective modes. Note that if you set one of the <c>Font</c> 
  ///   attributes (Bold, Italic etc.) to <c>true</c>, this cannot be overridden using 
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

  /// <summary> The style that you want to apply to the TextBox (edit mode) only. </summary>
  /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
  [Category ("Style")]
  [Description ("The style that you want to apply to the TextBox (edit mode) only.")]
  [NotifyParentProperty( true)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public DropDownListStyle DropDownListStyle
  {
    get { return _dropDownListStyle; }
  }

  /// <summary> The style that you want to apply to the Label (read-only mode) only. </summary>
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

  /// <summary> Gets the <see cref="DropDownList"/> used in edit mode. </summary>
  [Browsable (false)]
  public DropDownList DropDownList
  {
    get { return _dropDownList; }
  }

  /// <summary> Gets the <see cref="Label"/> used in read-only mode. </summary>
  [Browsable (false)]
  public Label Label
  {
    get { return _label; }
  }

  /// <summary> Gets the <see cref="Image"/> used as an icon. </summary>
  [Browsable (false)]
  public Image Icon
  {
    get { return _icon; }
  }

  /// <summary> Gets or sets the <see cref="BocCommand"/> rendered in this column. </summary>
  /// <value> A <see cref="BocCommand"/>. </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Category ("Menu")]
  [Description ("The command rendered in this column.")]
  [NotifyParentProperty (true)]
  public BocCommand Command
  {
    get { return (BocCommand) _command.Item; }
    set 
    { 
      _command.Item = value; 
      _command.Item.OwnerControl = this;
    }
  }

  private bool ShouldSerializeCommand()
  {
    if (Command == null)
      return false;

    if (Command.Type == CommandType.None)
      return false;
    else
      return true;
  }

  /// <summary> Sets the <see cref="Command"/> to its default value. </summary>
  /// <remarks> 
  ///   The default value is a <see cref="BocCommand"/> object with a <c>Command.Type</c> set to 
  ///   <see cref="CommandType.None"/>.
  /// </remarks>
  private void ResetCommand()
  {
    if (Command != null)
    {
      Command = (BocCommand) Activator.CreateInstance (Command.GetType());
      Command.Type = CommandType.None;
    }
  }

  [PersistenceMode (PersistenceMode.InnerProperty)]
  [Browsable (false)]
  [NotifyParentProperty (true)]
  public SingleControlItemCollection PersistedCommand
  {
    get { return _command; }
  }

  /// <summary> Controls the persisting of the <see cref="Command"/>. </summary>
  /// <remarks> 
  ///   Does not persist <see cref="BocCommand"/> objects with a <c>Command.Type</c> set to 
  ///   <see cref="CommandType.None"/>.
  /// </remarks>
  private bool ShouldSerializePersistedCommand()
  {
    return ShouldSerializeCommand();
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("Flag that determines whether to show the icon in front of the value.")]
  [DefaultValue (true)]
  public bool EnableIcon
  {
    get { return _enableIcon; }
    set { _enableIcon = value; }
  }

  /// <summary> The search expression used to populate the selection list in edit mode. </summary>
  /// <value> A <see cref="String"/> with a valid search expression. The default value is <see cref="String.Empty"/>. </value>
  [Category ("Data")]
  [Description ("Set the search expression for populating the selection list.")]
  [DefaultValue ("")]
  public string Select
  {
    get { return _select; }
    set { _select = value; }
  }

  /// <summary> Gets the <see cref="BocMenuItem"/> objects displayed in the <b>OptionsMenu</b>. </summary>
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Category ("Menu")]
  [Description ("The menu items displayed by options menu.")]
  [DefaultValue ((string) null)]
  public BocMenuItemCollection OptionsMenuItems
  {
    get { return _optionsMenuItems; }
  }

  /// <summary> Gets or sets the text that is rendered as a label for the <b>OptionsMenu</b>. </summary>
  /// <value> The text rendered as the <b>OptionsMenu</b>'s label. The default value is <see cref="String.Empty"/>. </value>
  [Category ("Menu")]
  [Description ("The text that is rendered as a label for the options menu.")]
  [DefaultValue ("")]
  public string OptionsTitle
  {
    get { return _optionsTitle; }
    set { _optionsTitle = value; }
  }

  /// <summary>
  ///   Gets or sets a flag that determines whether to display the options menu.
  /// </summary>
  /// <value> <see langword="true"/> to show the options menu. The default value is <see langword="true"/>. </value>
  [Category ("Menu")]
  [Description ("Enables the options menu.")]
  [DefaultValue (true)]
  public bool ShowOptionsMenu
  {
    get { return _showOptionsMenu; }
    set { _showOptionsMenu = value; }
  }

  /// <summary> Gets or sets the width of the options menu. </summary>
  /// <value> The <see cref="Unit"/> value used for the option menu's width. The default value is <b>undefined</b>. </value>
  [Category ("Menu")]
  [Description ("The width of the options menu.")]
  [DefaultValue (typeof (Unit), "")]
  public Unit OptionsMenuWidth
  {
    get { return _optionsMenuWidth; }
    set { _optionsMenuWidth = value; }
  }

  /// <summary> Gets or sets the validation error message. </summary>
  /// <value> 
  ///   The error message displayed when validation fails. The default value is <see cref="String.Empty"/>.
  ///   In case of the default value, the text is read from the resources for this control.
  /// </value>
  [Description("Validation message displayed if there is an error.")]
  [Category ("Validator")]
  [DefaultValue("")]
  public string ErrorMessage
  {
    get { return _errorMessage; }
    set 
    {
      _errorMessage = value; 
      foreach (BaseValidator validator in _validators)
        validator.ErrorMessage = _errorMessage;
    }
  }

  #region protected virtual string CssClass...
  /// <summary> Gets the CSS-Class applied to the <see cref="BocReferenceValue"/> itself. </summary>
  /// <remarks> 
  ///   <para> Class: <c>bocReferenceValue</c>. </para>
  ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassBase
{ get { return "bocReferenceValue"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocReferenceValue"/>'s value. </summary>
  /// <remarks> Class: <c>bocReferenceValueContent</c> </remarks>
  protected virtual string CssClassContent
  { get { return "bocReferenceValueContent"; } }

  // <summary> Gets the CSS-Class applied to the <see cref="BocReferenceValue"/>'s inner table on mouse over. </summary>
  // <remarks> Class: <c>bocReferenceValueOnMouseOver</c> </remarks>
  //protected virtual string CssClassOnMouseOver
  //{ get { return "bocReferenceValueOnMouseOver"; } }
  
  // <summary> Gets the CSS-Class applied to the <see cref="BocReferenceValue"/>'s inner table on mouse out. </summary>
  // <remarks> Class: <c>bocReferenceValueOnMouseOut</c> </remarks>
  //protected virtual string CssClassOnMouseOut
  //{ get { return "bocReferenceValueOnMouseOut"; } }  
  #endregion
}

}
