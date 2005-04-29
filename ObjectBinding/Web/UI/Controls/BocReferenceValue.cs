using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Globalization;
using log4net;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;
using Rubicon.ObjectBinding.Web.Design;
using Rubicon.Web;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.Utilities;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Globalization;

namespace Rubicon.ObjectBinding.Web.Controls
{
/// <summary> This control can be used to display or edit reference values. </summary>
/// <include file='doc\include\Controls\BocReferenceValue.xml' path='BocReferenceValue/Class/*' />
// TODO: see "Doc\Bugs and ToDos.txt"
[ValidationProperty ("Value")]
[DefaultEvent ("SelectionChanged")]
[ToolboxItemFilter("System.Web.UI")]
[Designer (typeof (BocReferenceValueDesigner))]
public class BocReferenceValue: 
    BusinessObjectBoundModifiableWebControl, 
    IPostBackEventHandler, 
    IPostBackDataHandler,
    IResourceDispatchTarget
{
  // constants
	
  private const string c_nullIdentifier = "--null--";

  /// <summary> The text displayed when control is displayed in desinger, is read-only, and has no contents. </summary>
  private const string c_designModeEmptyLabelContents = "##";
  private const string c_defaultControlWidth = "150pt";

  private const string c_scriptFileUrl = "BocReferenceValue.js";
  private const string c_styleFileUrl = "BocReferenceValue.css";

  /// <summary> The key identifying a options menu item resource entry. </summary>
  private const string c_resourceKeyOptionsMenuItems = "OptionsMenuItems";
  /// <summary> The key identifying the command resource entry. </summary>
  private const string c_resourceKeyCommand = "Command";

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
  
  /// <summary> The log4net logger. </summary>
  private static readonly log4net.ILog s_log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

  private static readonly object s_selectionChangedEvent = new object();
  private static readonly object s_menuItemClickEvent = new object();
  private static readonly object s_commandClickEvent = new object();

  private static readonly string s_scriptFileKey = typeof (BocReferenceValue).FullName + "_Script";
  private static readonly string s_startUpScriptKey = typeof (BocReferenceValue).FullName+ "_Startup";
  private static readonly string s_styleFileKey = typeof (BocReferenceValue).FullName + "_Style";

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
  ///   If the <see cref="DropDownList"/> could not be created from <see cref="DropDownListStyle"/>,
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

  /// <summary> Dispatches the resources passed in <paramref name="values"/> to the <see cref="BocReferenceValue"/>'s properties. </summary>
  /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
  public void Dispatch (IDictionary values)
  {
    HybridDictionary optionsMenuItemValues = new HybridDictionary();
    HybridDictionary propertyValues = new HybridDictionary();
    HybridDictionary commandValues = new HybridDictionary();

    //  Parse the values

    foreach (DictionaryEntry entry in values)
    {
      string key = (string) entry.Key;
      string[] keyParts = key.Split (new Char[] {':'}, 3);

      //  Is a property/value entry?
      if (keyParts.Length == 1)
      {
        string property = keyParts[0];
        propertyValues.Add (property, entry.Value);
      }
        //  Is compound element entry
      else if (keyParts.Length == 2)
      {
        //  Compound key: "elementID:property"
        string elementID = keyParts[0];
        string property = keyParts[1];

         //  Switch to the right collection
        switch (elementID)
        {
          case c_resourceKeyCommand:
          {
            commandValues.Add (property, entry.Value);
            break;
          }
          default:
          {
            //  Invalid collection property
            s_log.Debug ("BocReferenceValue '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' does not contain an element named '" + elementID + "'.");
            break;
          }
        }       
      }
        //  Is collection entry?
      else if (keyParts.Length == 3)
      {    
        //  Compound key: "collectionID:elementID:property"
        string collectionID = keyParts[0];
        string elementID = keyParts[1];
        string property = keyParts[2];

        IDictionary currentCollection = null;

        //  Switch to the right collection
        switch (collectionID)
        {
          case c_resourceKeyOptionsMenuItems:
          {
            currentCollection = optionsMenuItemValues;
            break;
          }
          default:
          {
            //  Invalid collection property
            s_log.Debug ("BocReferenceValue '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' does not contain a collection property named '" + collectionID + "'.");
            break;
          }
        }       

        //  Add the property/value pair to the collection
        if (currentCollection != null)
        {
          //  Get the dictonary for the current element
          IDictionary elementValues = (IDictionary) currentCollection[elementID];

          //  If no dictonary exists, create it and insert it into the elements hashtable.
          if (elementValues == null)
          {
            elementValues = new HybridDictionary();
            currentCollection[elementID] = elementValues;
          }

          //  Insert the argument and resource's value into the dictonary for the specified element.
          elementValues.Add (property, entry.Value);
        }
      }
      else
      {
        //  Not supported format or invalid property
        s_log.Debug ("BocReferenceValue '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' received a resource with an invalid or unknown key '" + key + "'. Required format: 'property' or 'collectionID:elementID:property'.");
      }
    }

    //  Dispatch simple properties
    ResourceDispatcher.DispatchGeneric (this, propertyValues);

    //  Dispatch compound element properties
    ResourceDispatcher.DispatchGeneric (Command, commandValues);

    //  Dispatch to collections
    DispatchToMenuItems (_optionsMenuItems, optionsMenuItemValues, "OptionsMenuItems");
  }

  /// <summary>
  ///   Dispatches the resources passed in <paramref name="values"/> to the properties of the 
  ///   <see cref="BocMenuItem"/> objects in the collection <paramref name="menuItems"/>.
  /// </summary>
  private void DispatchToMenuItems (BocMenuItemCollection menuItems, IDictionary values, string collectionName)
  {
    foreach (DictionaryEntry entry in values)
    {
      string itemID = (string) entry.Key;
      
      bool isValidID = false;
      foreach (BocMenuItem menuItem in menuItems)
      {
        if (menuItem.ItemID == itemID)
        {
          ResourceDispatcher.DispatchGeneric (menuItem, (IDictionary) entry.Value);
          isValidID = true;
          break;
        }
      }

      if (! isValidID)
      {
        //  Invalid collection element
        s_log.Debug ("BocReferenceValue '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' does not contain an item with an ID of '" + itemID + "' inside the collection '" + collectionName + "'.");
      }
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

  /// <summary> Overrides the <see cref="WebControl.RenderContents"/> method. </summary>
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
      {
        Command.RenderBegin (writer, postBackEvent, string.Empty, objectID);
        if (! StringUtility.IsNullOrEmpty (Command.ToolTip))
          _icon.ToolTip = Command.ToolTip;
      }
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

  /// <summary> Populates the <see cref="DropDownList"/> with the items passed in <paramref name="businessObjects"/>. </summary>
  /// <param name="businessObjects">
  ///   The array of <see cref="IBusinessObjectWithIdentity"/> objects to populate the <see cref="DropDownList"/>.
  /// </param>
  /// <remarks> This method controls the actual refilling of the <see cref="DropDownList"/>. </remarks>
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
    if (! HtmlHeadAppender.Current.IsRegistered (s_scriptFileKey))
    {
      string scriptUrl = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (BocReferenceValue), ResourceType.Html, c_scriptFileUrl);
      HtmlHeadAppender.Current.RegisterJavaScriptInclude (s_scriptFileKey, scriptUrl);
    }

    if (! Page.IsStartupScriptRegistered (s_startUpScriptKey))
    {
      const string script = "BocReferenceValue_InitializeGlobals ('" + c_nullIdentifier + "');";
      PageUtility.RegisterStartupScriptBlock (Page, s_startUpScriptKey, script);
    }

    if (! HtmlHeadAppender.Current.IsRegistered (s_styleFileKey))
    {
      string url = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (BocReferenceValue), ResourceType.Html, c_styleFileUrl);
      HtmlHeadAppender.Current.RegisterStylesheetLink (s_styleFileKey, url);
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

//        if (_enableIcon && ! IsReadOnly)
//        {
//          if (Value != null)
//            _icon.AlternateText = HttpUtility.HtmlEncode (Value.DisplayName);
//          else
//            _icon.AlternateText = String.Empty;
//        }
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

  /// <summary> Gets a flag describing whether the <see cref="OptionsMenu"/> is visible. </summary>
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
  ///   Ensures that the menu items for the <see cref="OptionsMenu"/> from the previous page life cycle have been loaded.
  /// </summary>
  /// <returns> An array of <see cref="BocMenuItem"/> objects to be used by the <see cref="OptionsMenu"/>. </returns>
  private BocMenuItem[] EnsureOptionsMenuItemsForPreviousLifeCycleGot()
  {
    if (_optionsMenuItemsPostBackEventHandlingPhase == null)
    {
      _optionsMenuItemsPostBackEventHandlingPhase = 
          GetOptionsMenuItemsForPreviousLifeCycle (_optionsMenuItems.ToArray());
    }
    return _optionsMenuItemsPostBackEventHandlingPhase;
  }

  /// <summary> Ensures that the menu items for the <see cref="OptionsMenu"/> have been loaded. </summary>
  /// <param name="forceRefresh"> <see langword="true"/> to override the ensure pattern. </param>
  /// <returns> An array of <see cref="BocMenuItem"/> objects to be used by the <see cref="OptionsMenu"/>. </returns>
  private BocMenuItem[] EnsureOptionsMenuItemsGot (bool forceRefresh)
  {
    if (_optionsMenuItemsRenderPhase == null || forceRefresh)
      _optionsMenuItemsRenderPhase = GetOptionsMenuItems (_optionsMenuItems.ToArray());
    return _optionsMenuItemsRenderPhase;
  }

  /// <summary> Ensures that the menu items for the <see cref="OptionsMenu"/> have been loaded. </summary>
  /// <returns> An array of <see cref="BocMenuItem"/> objects to be used by the <see cref="OptionsMenu"/>. </returns>
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

  /// <summary> 
  ///   Handles the <see cref="DropDownMenu.EventCommandClick"/> event of the <see cref="OptionsMenu"/>.
  /// </summary>
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

  /// <summary> Handles the <see cref="DropDownMenu.WxeFunctionCommandClick"/> event of the <see cref="OptionsMenu"/>. </summary>
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
  /// <include file='doc\include\Controls\BocReferenceValue.xml' path='BocReferenceValue/Value/*' />
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

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.ValueImplementation"/> property. </summary>
  /// <value> The value must be of type <see cref="IBusinessObjectWithIdentity"/>. </value>
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

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.TargetControl"/> property. </summary>
  /// <value> The <see cref="DropDownList"/> if the control is in edit mode, otherwise the control itself. </value>
  public override Control TargetControl 
  {
    get { return (! IsReadOnly) ? _dropDownList : (Control) this; }
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundModifiableWebControl.IsDirty"/> property. </summary>
  public override bool IsDirty
  {
    get { return _isDirty; }
    set { _isDirty = value; }
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/> property. </summary>
  protected override Type[] SupportedPropertyInterfaces
  {
    get { return s_supportedPropertyInterfaces; }
  }

  /// <summary> Overrides <see cref="Rubicon.Web.UI.ISmartControl.UseLabel"/>. </summary>
  /// <value> Always <see langword="false"/>. </value>
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

  /// <summary>
  ///   Gets the style that you want to apply to the <see cref="DropDownList"/> (edit mode) 
  ///   and the <see cref="Label"/> (read-only mode).
  /// </summary>
  /// <remarks>
  ///   Use the <see cref="DropDownListStyle"/> and <see cref="LabelStyle"/> to assign individual 
  ///   style settings for the respective modes. Note that if you set one of the <b>Font</b> 
  ///   attributes (Bold, Italic etc.) to <see langword="true"/>, this cannot be overridden using 
  ///   <see cref="DropDownListStyle"/> and <see cref="LabelStyle"/>  properties.
  /// </remarks>
  [Category ("Style")]
  [Description ("The style that you want to apply to the DropDownList (edit mode) and the Label (read-only mode).")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style CommonStyle
  {
    get { return _commonStyle; }
  }

  /// <summary> Gets the style that you want to apply to the <see cref="DropDownList"/> (edit mode) only. </summary>
  /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
  [Category ("Style")]
  [Description ("The style that you want to apply to the DropDownList (edit mode) only.")]
  [NotifyParentProperty( true)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public DropDownListStyle DropDownListStyle
  {
    get { return _dropDownListStyle; }
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

  /// <summary> Gets the <see cref="DropDownMenu"/> offering additional commands for the current <see cref="Value"/>. </summary>
  /// <remarks> 
  ///   <note type="caution"> Use the <see cref="OptionsMenuItems"/> property for setting the available menu items. </note>
  /// </remarks>
  [Browsable (false)]
  public DropDownMenu OptionsMenu
  {
    get { return _optionsMenu; }
  }

  /// <summary> Gets or sets the <see cref="BocCommand"/> for this control's <see cref="Value"/>. </summary>
  /// <value> A <see cref="BocCommand"/>. </value>
  /// <remarks> This property is used for Designer support. </remarks>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Category ("Menu")]
  [Description ("The command rendered for this control's Value.")]
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

  /// <summary> Controls the persisting of the <see cref="Command"/>. </summary>
  /// <remarks> 
  ///   <para>
  ///     Does not persist <see cref="BocCommand"/> objects with a <c>Command.Type</c> set to 
  ///     <see cref="CommandType.None"/>.
  ///   </para><para>
  ///     Used by <see cref="ShouldSerializePersistedCommand"/>.
  ///   </para>
  /// </remarks>
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

  /// <summary> Gets or sets the encapsulated <see cref="BocCommand"/> for this control's <see cref="Value"/>. </summary>
  /// <value> 
  ///   A <see cref="SingleControlItemCollection"/> containing a <see cref="BocCommand"/> in it's 
  ///   <see cref="SingleControlItemCollection.Item"/> property.
  /// </value>
  /// <remarks> This property is used for persisting the <see cref="Command"/> into the <b>ASPX</b> source code. </remarks>
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [Browsable (false)]
  [NotifyParentProperty (true)]
  public SingleControlItemCollection PersistedCommand
  {
    get { return _command; }
  }

  /// <summary> Controls the persisting of the <see cref="PersistedCommand"/>. </summary>
  /// <remarks> Returns <see cref="ShouldSerializeCommand"/>. </remarks>
  private bool ShouldSerializePersistedCommand()
  {
    return ShouldSerializeCommand();
  }

  /// <summary> 
  ///   Gets or sets a flag that determines whether the <see cref="Icon"/> is shown in front of the <see cref="Value"/>.
  /// </summary>
  /// <value> <see langword="true"/> to show the <see cref="Icon"/>. The default value is <see langword="true"/>. </value>
  /// <remarks> 
  ///   An icon is only shown if the <see cref="Property"/>'s 
  ///   <see cref="IBusinessObjectClass.BusinessObjectProvider">ReferenceClass.BusinessObjectProvider</see>
  ///   provides an isntance of type <see cref="IBusinessObjectWebUIService"/> and 
  ///   <see cref="IBusinessObjectWebUIService.GetIcon"/> returns an icon.
  /// </remarks>
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
  /// <remarks> A valid <see cref="Property"/> is required in order to populate the list using the search statement. </remarks>
  [Category ("Data")]
  [Description ("Set the search expression for populating the selection list.")]
  [DefaultValue ("")]
  public string Select
  {
    get { return _select; }
    set { _select = value; }
  }

  /// <summary> Gets the <see cref="BocMenuItem"/> objects displayed in the <see cref="OptionsMenu"/>. </summary>
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Category ("Menu")]
  [Description ("The menu items displayed by options menu.")]
  [DefaultValue ((string) null)]
  public BocMenuItemCollection OptionsMenuItems
  {
    get { return _optionsMenuItems; }
  }

  /// <summary> Gets or sets the text that is rendered as a label for the <see cref="OptionsMenu"/>. </summary>
  /// <value> 
  ///   The text rendered as the <see cref="OptionsMenu"/>'s label. The default value is <see cref="String.Empty"/>. 
  /// </value>
  [Category ("Menu")]
  [Description ("The text that is rendered as a label for the options menu.")]
  [DefaultValue ("")]
  public string OptionsTitle
  {
    get { return _optionsTitle; }
    set { _optionsTitle = value; }
  }

  /// <summary> Gets or sets a flag that determines whether to display the <see cref="OptionsMenu"/>. </summary>
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
