using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.Globalization;
using Rubicon.Logging;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Rubicon.ObjectBinding.Web.UI.Design;
using Rubicon.Security;
using Rubicon.Utilities;
using Rubicon.Web;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.Utilities;

namespace Rubicon.ObjectBinding.Web.UI.Controls
{

/// <summary> 
///   This control can be used to display and edit a list of <see cref="IBusinessObject"/> instances.
///   The properties of the business objects are displayed in individual columns. 
/// </summary>
/// <include file='doc\include\UI\Controls\BocList.xml' path='BocList/Class/*' />
// TODO: see "Doc\Bugs and ToDos.txt"
[Designer (typeof (BocListDesigner))]
[DefaultEvent ("CommandClick")]
[ToolboxItemFilter("System.Web.UI")]
public class BocList:
    BusinessObjectBoundEditableWebControl, 
    IPostBackEventHandler, 
    IPostBackDataHandler, 
    IBocMenuItemContainer,
    IBocListSortingOrderProvider,
    IResourceDispatchTarget
{
  #region Obsolete Stuff

  [Browsable (false)]
  [Obsolete ("Use EditableRowIndex instead.")]
  [EditorBrowsable (EditorBrowsableState.Never)]
  public NaInt32 ModifiableRowIndex
  {
    get { return EditableRowIndex; }
  }

  [Browsable (false)]
  [Obsolete ("Use EndEditDetailsMode instead.")]
  [EditorBrowsable (EditorBrowsableState.Never)]
  public void EndEditDetailsMode (bool saveChanges)
  {
    EndRowEditMode (saveChanges);
  }

  [Browsable (false)]
  [Obsolete ("Use IsEditDetailsModeActive instead.")]
  [EditorBrowsable (EditorBrowsableState.Never)]
  public bool IsEditDetailsModeActive
  {
    get { return IsRowEditModeActive; } 
  }

  [Obsolete ("Use EditableRowChangesSaving event instead.", true)]
  [EditorBrowsable (EditorBrowsableState.Never)]
  protected virtual void OnRowEditModeSaving (
    int index,
    IBusinessObject businessObject,
    IBusinessObjectDataSource dataSource,
    IBusinessObjectBoundEditableWebControl[] controls)
  {
    throw new NotImplementedException ();
  }

  [Obsolete ("Use EditableRowChangesSaved event instead.", true)]
  [EditorBrowsable (EditorBrowsableState.Never)]
  protected virtual void OnRowEditModeSaved (int index, IBusinessObject businessObject)
  {
    throw new NotImplementedException ();
  }

  [Obsolete ("Use EditableRowChangesCanceling event instead.", true)]
  [EditorBrowsable (EditorBrowsableState.Never)]
  protected virtual void OnRowEditModeCanceling (
    int index,
    IBusinessObject businessObject,
    IBusinessObjectDataSource dataSource,
    IBusinessObjectBoundEditableWebControl[] controls)
  {
    throw new NotImplementedException ();
  }

  [Obsolete ("Use EditableRowChangesCanceled event instead.", true)]
  [EditorBrowsable (EditorBrowsableState.Never)]
  protected virtual void OnRowEditModeCanceled (int index, IBusinessObject businessObject)
  {
    throw new NotImplementedException ();
  }

  [Browsable (false)]
  [EditorBrowsable (EditorBrowsableState.Never)]
  [Obsolete ("Use EditableRowChangesSaving instead.", true)]
  public event BocListRowEditModeEventHandler SavingEditedRow
  {
    add { throw new NotImplementedException ("Obsolete"); }
    remove { throw new NotImplementedException ("Obsolete"); }
  }

  [Browsable (false)]
  [EditorBrowsable (EditorBrowsableState.Never)]
  [Obsolete ("Use EditableRowChangesSaved instead.", true)]
  public event BocListItemEventHandler EditedRowSaved
  {
    add { throw new NotImplementedException ("Obsolete"); }
    remove { throw new NotImplementedException ("Obsolete"); }
  }

  [Browsable (false)]
  [EditorBrowsable (EditorBrowsableState.Never)]
  [Obsolete ("Use EditableRowChangesCanceling instead.", true)]
  public event BocListRowEditModeEventHandler CancelingEditDetailsMode
  {
    add { throw new NotImplementedException ("Obsolete"); }
    remove { throw new NotImplementedException ("Obsolete"); }
  }

  [Browsable (false)]
  [EditorBrowsable (EditorBrowsableState.Never)]
  [Obsolete ("Use EditableRowChangesCanceled instead.", true)]
  public event BocListItemEventHandler EditDetailsModeCanceled
  {
    add { throw new NotImplementedException ("Obsolete"); }
    remove { throw new NotImplementedException ("Obsolete"); }
  }

  [Obsolete ("Use ValidateEditableRows instead.")]
  [EditorBrowsable (EditorBrowsableState.Never)]
  public bool ValiadateModifiableRow()
  {
    return ValidateEditableRows();
  }

  [Obsolete ("Use ValidateEditableRows instead.")]
  [EditorBrowsable (EditorBrowsableState.Never)]
  public bool ValidateModifiableRow()
  {
    return ValidateEditableRows();
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  [EditorBrowsable (EditorBrowsableState.Never)]
  [Obsolete ("Use ShowEditModeRequiredMarkers instead.", true)]
  public bool ShowEditDetailsRequiredMarkers
  {
    get { throw new NotImplementedException(); }
    set { throw new NotImplementedException(); }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  [EditorBrowsable (EditorBrowsableState.Never)]
  [Obsolete ("Use ShowEditModeValidationMarkers instead.", true)]
  public bool ShowEditDetailsValidationMarkers
  {
    get { throw new NotImplementedException(); }
    set { throw new NotImplementedException(); }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  [EditorBrowsable (EditorBrowsableState.Never)]
  [Obsolete ("Use DisableEditModeValidationMessages instead.", true)]
  public bool DisableEditDetailsValidationMessages
  {
    get { throw new NotImplementedException(); }
    set { throw new NotImplementedException(); }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  [EditorBrowsable (EditorBrowsableState.Never)]
  [Obsolete ("Use EnableEditModeValidator instead.", true)]
  public bool EnableEditDetailsValidator
  {
    get { throw new NotImplementedException(); }
    set { throw new NotImplementedException(); }
  }

  #endregion

  //  constants
  private const string c_dataRowHiddenFieldIDSuffix = "_Boc_HiddenField_";
  private const string c_dataRowSelectorControlIDSuffix = "_Boc_SelectorControl_";
  private const string c_titleRowSelectorControlIDSuffix = "_Boc_SelectorControl_SelectAll";
  private const string c_availableViewsListIDSuffix = "_Boc_AvailableViewsList";
  private const string c_optionsMenuIDSuffix = "_Boc_OptionsMenu";

  private const int c_titleRowIndex = -1;

  /// <summary> Prefix applied to the post back argument of the event type column commands. </summary>
  private const string c_eventListItemCommandPrefix = "ListCommand=";
  /// <summary> Prefix applied to the post back argument of the event type menu commands. </summary>
  private const string c_eventMenuItemPrefix = "MenuItem=";
  /// <summary> Prefix applied to the post back argument of the custom columns. </summary>
  private const string c_customCellEventPrefix = "CustomCell=";

  private const string c_eventRowEditModePrefix = "RowEditMode=";
  private const string c_rowEditModeRequiredFieldIcon = "RequiredField.gif";
  private const string c_rowEditModeValidationErrorIcon = "ValidationError.gif";

  private const string c_sortAscendingIcon = "SortAscending.gif";
  private const string c_sortDescendingIcon = "SortDescending.gif";
  /// <summary> Prefix applied to the post back argument of the sort buttons. </summary>
  private const string c_sortCommandPrefix = "Sort=";

  #region private const string c_goTo...Icon
  private const string c_goToFirstIcon = "MoveFirst.gif";
  private const string c_goToLastIcon = "MoveLast.gif";
  private const string c_goToPreviousIcon = "MovePrevious.gif";
  private const string c_goToNextIcon = "MoveNext.gif";
  private const string c_goToFirstInactiveIcon = "MoveFirstInactive.gif";
  private const string c_goToLastInactiveIcon = "MoveLastInactive.gif";
  private const string c_goToPreviousInactiveIcon = "MovePreviousInactive.gif";
  private const string c_goToNextInactiveIcon = "MoveNextInactive.gif";
  #endregion
  private const string c_goToCommandPrefix = "GoTo=";

  private const string c_scriptFileUrl = "BocList.js";
  private const string c_onCommandClickScript = "BocList_OnCommandClick();";
  private const string c_styleFileUrl = "BocList.css";

  private const string c_whiteSpace = "&nbsp;";

  /// <summary> The key identifying a fixed column resource entry. </summary>
  private const string c_resourceKeyFixedColumns = "FixedColumns";
  /// <summary> The key identifying a options menu item resource entry. </summary>
  private const string c_resourceKeyOptionsMenuItems = "OptionsMenuItems";
  /// <summary> The key identifying a list menu item resource entry. </summary>
  private const string c_resourceKeyListMenuItems = "ListMenuItems";

  private const string c_defaultMenuBlockItemOffset = "5pt";
  private const string c_defaultMenuBlockWidth = "70pt";
  private const string c_defaultMenuBlockOffset = "5pt";

  /// <summary> 
  ///   Text displayed when control is displayed in desinger and is read-only has no contents.
  /// </summary>
  private const string c_designModeEmptyContents = "#";
  private const string c_designModeDummyColumnTitle = "Column Title {0}";
  private const int c_designModeDummyColumnCount = 3;

  private const int c_designModeAvailableViewsListWidthInPoints = 40;

  // types
  
  /// <summary> A list of control wide resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:IResourceManager.GetString (Enum)">IResourceManager.GetString (Enum)</see>. 
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Rubicon.ObjectBinding.Web.Globalization.BocList")]
  public enum ResourceIdentifier
  {
    EmptyListMessage,
    PageInfo,
    OptionsTitle,
    AvailableViewsListTitle,
    /// <summary>The alternate text for the required icon.</summary>
    RequiredFieldAlternateText,
    /// <summary>The tool tip text for the required icon.</summary>
    RequiredFieldTitle,
    /// <summary>The alternate text for the validation error icon.</summary>
    ValidationErrorInfoAlternateText,
    /// <summary> The alternate text for the sort ascending button. </summary>
    SortAscendingAlternateText,
    /// <summary> The alternate text for the sort descending button. </summary>
    SortDescendingAlternateText,
    RowEditModeErrorMessage,
    ListEditModeErrorMessage,
    RowEditModeEditAlternateText,
    RowEditModeSaveAlternateText,
    RowEditModeCancelAlternateText,
    GoToFirstAlternateText,
    GoToLastAlternateText,
    GoToNextAlternateText,
    GoToPreviousAlternateText,
    SelectAllRowsAlternateText,
    SelectRowAlternateText,
    IndexColumnTitle,
    /// <summary> The menu title text used for an automatically generated row menu column. </summary>
    RowMenuTitle
  }

  /// <summary> The possible directions for paging through the list. </summary>
  private enum GoToOption
  {
    /// <summary> Don't page. </summary>
    Undefined,
    /// <summary> Move to first page. </summary>
    First,
    /// <summary> Move to last page. </summary>
    Last,
    /// <summary> Move to previous page. </summary>
    Previous,
    /// <summary> Move to next page. </summary>
    Next
  }

  private enum RowEditModeCommand
  {
    Edit,
    Save,
    Cancel
  }

  // static members
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectReferenceProperty) };
  
  private static readonly ILog s_log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

  private static readonly object s_listItemCommandClickEvent = new object();
  private static readonly object s_menuItemClickEvent = new object();
  private static readonly object s_customCellClickEvent = new object();

  private static readonly object s_sortingOrderChangingEvent = new object();
  private static readonly object s_sortingOrderChangedEvent = new object();

  private static readonly object s_dataRowRenderEvent = new object();

  private static readonly object s_editableRowChangesSavingEvent = new object();
  private static readonly object s_editableRowChangesSavedEvent = new object();
  private static readonly object s_editableRowChangesCancelingEvent = new object();
  private static readonly object s_editableRowChangesCanceledEvent = new object();

  private static readonly string s_scriptFileKey = typeof (BocList).FullName + "_Script";
  private static readonly string s_startUpScriptKey = typeof (BocList).FullName+ "_Startup";
  private static readonly string s_styleFileKey = typeof (BocList).FullName + "_Style";

	// member fields

  /// <summary> The <see cref="DropDownList"/> used to select the column configuration. </summary>
  private DropDownList _availableViewsList;
  /// <summary> The <see cref="string"/> that is rendered in front of the <see cref="_availableViewsList"/>. </summary>
  private string _availableViewsListTitle;
  /// <summary> The width applied to the <see cref="_availableViewsList"/>. </summary>
  private Unit _availableViewsListWidth = Unit.Empty;
  /// <summary> The predefined column definition sets that the user can choose from at run-time. </summary>
  private BocListViewCollection _availableViews;
  /// <summary> Determines whether to show the drop down list for selecting a view. </summary>
  private bool _showAvailableViewsList = true;
  /// <summary> The current <see cref="BocListView"/>. May be set at run time. </summary>
  private BocListView _selectedView;
  /// <summary> 
  ///   The zero-based index of the <see cref="BocListView"/> selected from 
  ///   <see cref="AvailableViews"/>.
  /// </summary>
  private NaInt32 _selectedViewIndex = NaInt32.Null;
  private string _availableViewsListSelectedValue = string.Empty;
  bool _isSelectedViewIndexSet = false;

  /// <summary> The <see cref="IList"/> displayed by the <see cref="BocList"/>. </summary>
  private IList _value = null;

  /// <summary> The user independent column definitions. </summary>
  private BocColumnDefinitionCollection _fixedColumns;
  /// <summary> 
  ///   Contains a <see cref="BocColumnDefinition"/> for each property of the bound 
  ///   <see cref="IBusinessObject"/>. 
  /// </summary>
  private BocColumnDefinition[] _allPropertyColumns = null;
  /// <summary> Contains the <see cref="BocColumnDefinition"/> objects during the handling of the post back events. </summary>
  private BocColumnDefinition[] _columnDefinitionsPostBackEventHandlingPhase = null;
  /// <summary> Contains the <see cref="BocColumnDefinition"/> objects during the rendering phase. </summary>
  private BocColumnDefinition[] _columnDefinitionsRenderPhase = null;
  private bool _hasAppendedAllPropertyColumnDefinitions = false;


  /// <summary> Determines whether the options menu is shown. </summary>
  private bool _showOptionsMenu = true;
  /// <summary> Determines whether the list menu is shown. </summary>
  private bool _showListMenu = true;
  private RowMenuDisplay _rowMenuDisplay = RowMenuDisplay.Undefined;
  private string _optionsTitle;
  private string[] _hiddenMenuItems;
  private Unit _menuBlockWidth = Unit.Empty;
  private Unit _menuBlockOffset = Unit.Empty;
  private Unit _menuBlockItemOffset = Unit.Empty;
  private ListMenuLineBreaks _listMenuLineBreaks = ListMenuLineBreaks.All;
  private DropDownMenu _optionsMenu;
  private WebMenuItemCollection _listMenuItems;
  /// <summary> Triplet &lt; IBusinessObject, listIndex, DropDownMenu &gt;</summary>
  private Triplet[] _rowMenus;
  private PlaceHolder _rowMenusPlaceHolder;
 
  /// <summary> 
  ///   HashTable &lt; 
  ///       Key = CustomColumn, 
  ///       Value = Triplet[] &lt; IBusinessObject, listIndex, Control &gt; &gt;
  /// </summary>
  private Hashtable _customColumns;
  private PlaceHolder _customColumnsPlaceHolder;

  /// <summary> Determines wheter an empty list will still render its headers and the additional column sets list. </summary>
  private bool _showEmptyListEditMode = true;
  private bool _showMenuForEmptyListEditMode = true;
  private bool _showEmptyListReadOnlyMode = false;
  private bool _showMenuForEmptyListReadOnlyMode = false;
  private string _emptyListMessage = null;
  private bool _showEmptyListMessage = false;

  /// <summary> Determines whether to generate columns for all properties. </summary>
  private bool _showAllProperties;
  
  /// <summary> Determines whether to show the icons for each entry in <see cref="Value"/>. </summary>
  private bool _enableIcon = true;
  
  /// <summary> Determines whether to show the sort buttons. </summary>
  private bool _enableSorting = true;
  /// <summary> Determines whether to show the sorting order after the sorting button. Undefined interpreted as True. </summary>
  private NaBooleanEnum _showSortingOrder = NaBooleanEnum.Undefined;
  /// <summary> Undefined interpreted as True. </summary>
  private NaBooleanEnum _enableMultipleSorting = NaBooleanEnum.Undefined;
  /// <summary> 
  ///   Contains <see cref="BocListSortingOrderEntry"/> objects in the order of the buttons pressed.
  /// </summary>
  private ArrayList _sortingOrder = new ArrayList();
  private BocListRow[] _indexedRowsSorted = null;

  /// <summary> Determines whether to enable the selecting of the data rows. </summary>
  private RowSelection _selection = RowSelection.Undefined;
  /// <summary> 
  ///   Contains the checked state for each of the selector controls in the <see cref="BocList"/>.
  ///   Hashtable&lt;int rowIndex, bool isChecked&gt; 
  /// </summary>
  private Hashtable _selectorControlCheckedState = new Hashtable();
  private RowIndex _index = RowIndex.Undefined;
  private string _indexColumnTitle;
  private NaInt32 _indexOffset = NaInt32.Null;

  /// <summary> Null, 0: show all objects, > 0: show n objects per page. </summary>
  private NaInt32 _pageSize = NaInt32.Null; 
  /// <summary>
  ///   Show page info ("page 1 of n") and links always (true),
  ///   or only if there is more than 1 page (false)
  /// </summary>
  private bool _alwaysShowPageInfo = false; 
  /// <summary> The text providing the current page information to the user. </summary>
  private string _pageInfo = null;
  /// <summary> 
  ///   The navigation bar command that caused the post back. 
  ///   <see cref="GoToOption.Undefined"/> unless the navigation bar caused a post back.
  /// </summary>
  private GoToOption _goTo = GoToOption.Undefined;
  /// <summary> 
  ///   The index of the current row in the <see cref="IBusinessObject"/> this control is bound to.
  /// </summary>
  private int _currentRow = 0;
  /// <summary> The index of the current page. </summary>
  private int _currentPage = 0;
  /// <summary> The total number of pages required for paging through the entire list. </summary>
  private int _pageCount = 0;

  /// <summary> Determines whether the client script is enabled. </summary>
  private bool _enableClientScript = true;
  /// <summary> Determines whether the client script will be rendered. </summary>
  private bool _hasClientScript = false;

  private EditModeController _editModeController;
  private EditableRowDataSourceFactory _editModeDataSourceFactory = new EditableRowDataSourceFactory();
  private EditableRowControlFactory _editModeControlFactory = new EditableRowControlFactory();

  private string _errorMessage;
  private ArrayList _validators;

  // construction and disposing

	public BocList()
	{
    _availableViewsList = new DropDownList();
    _editModeController = new EditModeController (this);
    _optionsMenu = new DropDownMenu (this);
    _listMenuItems = new WebMenuItemCollection (this);
    _rowMenusPlaceHolder = new PlaceHolder();
    _customColumnsPlaceHolder = new PlaceHolder();
    _fixedColumns = new BocColumnDefinitionCollection (this);
    _availableViews = new BocListViewCollection (this);
    _validators = new ArrayList();
  }

	// methods and properties

  protected override void CreateChildControls()
  {
    _optionsMenu.ID = ID + c_optionsMenuIDSuffix;
    _optionsMenu.GetSelectionCount = "function() { return BocList_GetSelectionCount ('" + ClientID + "'); }";
    Controls.Add (_optionsMenu);

    _availableViewsList.ID = ID + c_availableViewsListIDSuffix;
    _availableViewsList.EnableViewState = false;
    _availableViewsList.AutoPostBack = true;
    Controls.Add (_availableViewsList);

    _editModeController.ID = ID + "_EditModeController";
    Controls.Add (_editModeController);

    Controls.Add (_rowMenusPlaceHolder);
    Controls.Add (_customColumnsPlaceHolder);
  }

  /// <summary> Calls the parent's <c>OnInit</c> method and initializes this control's sub-controls. </summary>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);

    _optionsMenu.EventCommandClick += new WebMenuItemClickEventHandler (OptionsMenu_EventCommandClick);
    _optionsMenu.WxeFunctionCommandClick += new WebMenuItemClickEventHandler (OptionsMenu_WxeFunctionCommandClick);

    _availableViews.CollectionChanged += new CollectionChangeEventHandler(AvailableViews_CollectionChanged);
    Binding.BindingChanged += new EventHandler (Binding_BindingChanged);

    if (!IsDesignMode)
    {
      Page.RegisterRequiresPostBack (this);
      InitializeMenusItems();
    }
 }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad (e);
    
    if (! Page.IsPostBack)
      EnsureColumnsGot();
    else
      EnsureColumnsForPreviousLifeCycleGot();
    
    EnsureEditModeRestored();
    EnsureRowMenusInitialized();
    RestoreCustomColumns();
  }

  /// <summary> Implements interface <see cref="IPostBackEventHandler"/>. </summary>
  /// <param name="eventArgument"> &lt;prefix&gt;=&lt;value&gt; </param>
  void IPostBackEventHandler.RaisePostBackEvent (string eventArgument)
  {
    RaisePostBackEvent (eventArgument);
  }

  /// <param name="eventArgument"> &lt;prefix&gt;=&lt;value&gt; </param>
  protected virtual void RaisePostBackEvent (string eventArgument)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);

    eventArgument = eventArgument.Trim();
    if (eventArgument.StartsWith (c_eventListItemCommandPrefix))
      HandleListItemCommandEvent (eventArgument.Substring (c_eventListItemCommandPrefix.Length));
    else if (eventArgument.StartsWith (c_eventMenuItemPrefix))
      HandleMenuItemEvent (eventArgument.Substring (c_eventMenuItemPrefix.Length));
    else if (eventArgument.StartsWith (c_sortCommandPrefix))
      HandleResorting (eventArgument.Substring (c_sortCommandPrefix.Length));
    else if (eventArgument.StartsWith (c_customCellEventPrefix))
      HandleCustomCellEvent (eventArgument.Substring (c_customCellEventPrefix.Length));
    else if (eventArgument.StartsWith (c_eventRowEditModePrefix))
      HandleRowEditModeEvent (eventArgument.Substring (c_eventRowEditModePrefix.Length));
    else if (eventArgument.StartsWith (c_goToCommandPrefix))
      HandleGoToEvent (eventArgument.Substring (c_goToCommandPrefix.Length));
    else
      throw new ArgumentException ("Argument 'eventArgument' has unknown prefix: '" + eventArgument + "'.");
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
  void IPostBackDataHandler.RaisePostDataChangedEvent()
  {
    RaisePostDataChangedEvent();
  }

  /// <summary>
  ///   Returns always <see langword="true"/>. 
  ///   Used to raise the post data changed event for getting the selected column definition set.
  /// </summary>
  protected virtual bool LoadPostData (string postDataKey, NameValueCollection postCollection)
  {
    if (IsRowEditModeActive)
      return false;

    string dataRowSelectorControlFilter = ClientID + c_dataRowSelectorControlIDSuffix;
    string titleRowSelectorControlFilter = ClientID + c_titleRowSelectorControlIDSuffix;

    _selectorControlCheckedState.Clear();
    for (int i = 0; i < postCollection.Count; i++)
    {
      string key = postCollection.Keys[i];

      bool isDataRowSelectorControl = key.StartsWith (dataRowSelectorControlFilter);
      bool isTitleRowSelectorControl = (key == titleRowSelectorControlFilter);
      if (isDataRowSelectorControl || isTitleRowSelectorControl)
      {
        if (   (   _selection == RowSelection.SingleCheckBox
                || _selection == RowSelection.SingleRadioButton)
            && (   _selectorControlCheckedState.Count > 1 
                || isTitleRowSelectorControl))
        {
          continue;
        }
        int rowIndex = int.Parse (postCollection[i]);
        _selectorControlCheckedState[rowIndex] = true; 
      }
    }    

    string newAvailableViewsListSelectedValue = postCollection[_availableViewsList.UniqueID];
    if (   ! StringUtility.IsNullOrEmpty (newAvailableViewsListSelectedValue)
        && _availableViewsListSelectedValue != newAvailableViewsListSelectedValue)
    {
      return true;
    }
    else
    {
      return false;
    }
  }

  /// <summary> Called when the state of the control has changed between postbacks. </summary>
  protected virtual void RaisePostDataChangedEvent()
  {
    if (_availableViews.Count > 0)
    {
      string newAvailableViewsListSelectedValue = 
          PageUtility.GetPostBackCollectionItem (Page, _availableViewsList.UniqueID);
      SelectedViewIndex = int.Parse (newAvailableViewsListSelectedValue);
    }
  }

  /// <summary> Handles post back events raised by a list item event. </summary>
  /// <param name="eventArgument"> &lt;column-index&gt;,&lt;list-index&gt; </param>
  private void HandleListItemCommandEvent (string eventArgument)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);

    string[] eventArgumentParts = eventArgument.Split (new char[] {','}, 2);

    //  First part: column index
    int columnIndex;
    eventArgumentParts[0] = eventArgumentParts[0].Trim();
    try 
    {
      if (eventArgumentParts[0].Length == 0)
        throw new FormatException();
      columnIndex = int.Parse (eventArgumentParts[0]);
    }
    catch (FormatException)
    {
      throw new ArgumentException (
          "First part of argument 'eventArgument' must be an integer. Expected format: '<column-index>,<list-index>'.");
    }

    //  Second part: list index
    int listIndex;
    eventArgumentParts[1] = eventArgumentParts[1].Trim();
    try 
    {
      if (eventArgumentParts[1].Length == 0)
        throw new FormatException();
      listIndex = int.Parse (eventArgumentParts[1]);
    }
    catch (FormatException)
    {
      throw new ArgumentException (
          "Second part of argument 'eventArgument' must be an integer. Expected format: <column-index>,<list-index>'.");
    }

    BocColumnDefinition[] columns = EnsureColumnsForPreviousLifeCycleGot();

    if (columnIndex >= columns.Length)
      throw new ArgumentOutOfRangeException (
            "Column index of argument 'eventargument' was out of the range of valid values."
          + "Index must be less than the number of displayed columns.'");

    BocCommandEnabledColumnDefinition column = (BocCommandEnabledColumnDefinition) columns[columnIndex];
    if (column.Command == null)
    {
      throw new ArgumentOutOfRangeException (string.Format (
          "The BocList '{0}' does not have a command inside column {1}.", ID, columnIndex));
    }
    BocListItemCommand command = column.Command;

    if (Value == null)
    {
      throw new InvalidOperationException (string.Format (
          "The BocList '{0}' does not have a Value when attempting to handle the list item click event.", ID));
    }

    switch (command.Type)
    {
      case CommandType.Event:
      {
        IBusinessObject businessObject = null;
        if (listIndex < Value.Count)
          businessObject = (IBusinessObject) Value[listIndex];
        OnListItemCommandClick (column, listIndex, businessObject);
        break;
      }
      case CommandType.WxeFunction:
      {
        IBusinessObject businessObject = null;
        if (listIndex < Value.Count)
          businessObject = (IBusinessObject) Value[listIndex];
        if (Page is IWxePage)
          command.ExecuteWxeFunction ((IWxePage) Page, listIndex, businessObject);
        //else
        //  command.ExecuteWxeFunction (Page, listIndex, businessObject);
        break;
      }
      default:
      {
        break;
      }
    }
  }

  /// <summary> Handles post back events raised by a menu item event. </summary>
  private void HandleMenuItemEvent (string eventArgument)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);

    int index;
    try 
    {
      if (eventArgument.Length == 0)
        throw new FormatException();
      index = int.Parse (eventArgument);
    }
    catch (FormatException)
    {
      throw new ArgumentException ("First part of argument 'eventArgument' must be an integer. Expected format: '<index>'.");
    }

    if (index >= _listMenuItems.Count)
      throw new ArgumentOutOfRangeException ("eventargument");

    WebMenuItem menuItem = (WebMenuItem) _listMenuItems[index];
    if (menuItem.Command == null)
      throw new ArgumentException ("The BocList '" + ID + "' does not have a command associated with list menu item " + index + ".");

    switch (menuItem.Command.Type)
    {
      case CommandType.Event:
      {
        OnMenuItemEventCommandClick (menuItem);
        break;
      }
      case CommandType.WxeFunction:
      {
        OnMenuItemWxeFunctionCommandClick (menuItem);
        break;
      }
      default:
      {
        break;
      }
    }
  }

  /// <summary> Handles post back events raised by a custom cell event. </summary>
  /// <param name="eventArgument"> &lt;column-index&gt;,&lt;list-index&gt;[,&lt;customArgument&gt;] </param>
  private void HandleCustomCellEvent (string eventArgument)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);

    string[] eventArgumentParts = eventArgument.Split (new char[] {','}, 3);

    //  First part: column index
    int columnIndex;
    eventArgumentParts[0] = eventArgumentParts[0].Trim();
    try 
    {
      if (eventArgumentParts[0].Length == 0)
        throw new FormatException();
      columnIndex = int.Parse (eventArgumentParts[0]);
    }
    catch (FormatException)
    {
      throw new ArgumentException ("First part of argument 'eventArgument' must be an integer. Expected format: '<column-index>,<list-index>[,<customArgument>]'.");
    }

    //  Second part: list index
    int listIndex;
    eventArgumentParts[1] = eventArgumentParts[1].Trim();
    try 
    {
      if (eventArgumentParts[1].Length == 0)
        throw new FormatException();
      listIndex = int.Parse (eventArgumentParts[1]);
    }
    catch (FormatException)
    {
      throw new ArgumentException ("Second part of argument 'eventArgument' must be an integer. Expected format: <column-index>,<list-index>[,<customArgument>]'.");
    }

    //  Thrid part, optional: customCellArgument
    string customCellArgument = null;
    if (eventArgumentParts.Length == 3)
    {
      eventArgumentParts[2] = eventArgumentParts[2].Trim();
      customCellArgument = eventArgumentParts[2];
    }
    BocColumnDefinition[] columns = EnsureColumnsForPreviousLifeCycleGot();

    if (columnIndex >= columns.Length)
      throw new ArgumentOutOfRangeException ("Column index of argument 'eventargument' was out of the range of valid values. Index must be less than the number of displayed columns.'");

    BocCustomColumnDefinition column = (BocCustomColumnDefinition) columns[columnIndex];
    OnCustomCellClick (column, (IBusinessObject) Value[listIndex], customCellArgument);
  }

  /// <summary> Handles post back events raised by an row edit mode event. </summary>
  /// <param name="eventArgument"> &lt;list-index&gt;,&lt;command&gt; </param>
  private void HandleRowEditModeEvent (string eventArgument)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);

    string[] eventArgumentParts = eventArgument.Split (new char[] {','}, 2);

    //  First part: list index
    int listIndex;
    eventArgumentParts[0] = eventArgumentParts[0].Trim();
    try 
    {
      if (eventArgumentParts[0].Length == 0)
        throw new FormatException();
      listIndex = int.Parse (eventArgumentParts[0]);
    }
    catch (FormatException)
    {
      throw new ArgumentException (
          "First part of argument 'eventArgument' must be an integer. Expected format: '<list-index>,<command>'.");
    }

    //  Second part: command
    RowEditModeCommand command;
    eventArgumentParts[1] = eventArgumentParts[1].Trim();
    try 
    {
      if (eventArgumentParts[1].Length == 0)
        throw new FormatException();
      command = (RowEditModeCommand) Enum.Parse (typeof (RowEditModeCommand), eventArgumentParts[1]);
    }
    catch (FormatException)
    {
      throw new ArgumentException (
          "Second part of argument 'eventArgument' must be an integer. Expected format: <list-index>,<command>'.");
    }

    if (Value == null)
    {
      throw new InvalidOperationException (string.Format (
          "The BocList '{0}' does not have a Value when attempting to handle the list item click event.", ID));
    }

    switch (command)
    {
      case RowEditModeCommand.Edit:
      {
        if (listIndex >= Value.Count)
        {
          throw new ArgumentOutOfRangeException (
              "list-index of argument 'eventargument' was out of the range of valid values. Index must be less than the number of business objects in the list.'");
        }
        SwitchRowIntoEditMode (listIndex);
        break;
      }
      case RowEditModeCommand.Save:
      {
        EndRowEditMode (true);
        break;
      }
      case RowEditModeCommand.Cancel:
      {
        EndRowEditMode (false);
        break;
      }
      default:
      {
        break;
      }
    }
  }

  /// <summary> Handles post back events raised by a go-to button. </summary>
  /// <param name="eventArgument"> &lt;GoToOption&gt; </param>
  private void HandleGoToEvent (string eventArgument)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);

    try 
    {
      _goTo = (GoToOption) Enum.Parse (typeof (GoToOption), eventArgument);
    }
    catch (ArgumentException)
    {
      throw new ArgumentException ("Argument 'eventArgument' must be a value of the GoToOption enum.");
    }
  }

  /// <summary> Fires the <see cref="ListItemCommandClick"/> event. </summary>
  /// <include file='doc\include\UI\Controls\BocList.xml' path='BocList/OnListItemCommandClick/*' />
  protected virtual void OnListItemCommandClick (
      BocCommandEnabledColumnDefinition column, 
      int listIndex, 
      IBusinessObject businessObject)
  {
    if (column != null && column.Command != null)
    {
      column.Command.OnClick (column, listIndex, businessObject);
      BocListItemCommandClickEventHandler commandClickHandler = 
          (BocListItemCommandClickEventHandler) Events[s_listItemCommandClickEvent];
      if (commandClickHandler != null)
      {
        BocListItemCommandClickEventArgs e = 
            new BocListItemCommandClickEventArgs (column.Command, column, listIndex, businessObject);
        commandClickHandler (this, e);
      }
    }
  }

  /// <summary> 
  ///   Event handler for the <see cref="DropDownMenu.EventCommandClick"/> of the <see cref="_optionsMenu"/>.
  /// </summary>
  private void OptionsMenu_EventCommandClick (object sender, WebMenuItemClickEventArgs e)
  {
    OnMenuItemEventCommandClick (e.Item);
  }

  /// <summary> Fires the <see cref="MenuItemClick"/> event. </summary>
  /// <include file='doc\include\UI\Controls\BocList.xml' path='BocList/OnMenuItemEventCommandClick/*' />
  protected virtual void OnMenuItemEventCommandClick (WebMenuItem menuItem)
  {
    if (menuItem != null && menuItem.Command != null)
    {
      if (menuItem is BocMenuItem)
        ((BocMenuItemCommand) menuItem.Command).OnClick ((BocMenuItem) menuItem);
      else
        menuItem.Command.OnClick();
    }

    WebMenuItemClickEventHandler menuItemClickHandler = (WebMenuItemClickEventHandler) Events[s_menuItemClickEvent];
    if (menuItemClickHandler != null)
    {
      WebMenuItemClickEventArgs e = new WebMenuItemClickEventArgs (menuItem);
      menuItemClickHandler (this, e);
    }
  }

  /// <summary> 
  ///   Event handler for the <see cref="DropDownMenu.WxeFunctionCommandClick"/> of the <see cref="_optionsMenu"/>.
  /// </summary>
  private void OptionsMenu_WxeFunctionCommandClick (object sender, WebMenuItemClickEventArgs e)
  {
    OnMenuItemWxeFunctionCommandClick (e.Item);
  }

  /// <summary> Handles the click to a WXE function command. </summary>
  /// <include file='doc\include\UI\Controls\BocList.xml' path='BocList/OnMenuItemWxeFunctionCommandClick/*' />
  protected virtual void OnMenuItemWxeFunctionCommandClick (WebMenuItem menuItem)
  {
    if (menuItem != null && menuItem.Command != null)
    {
      if (menuItem is BocMenuItem)
      {
        BocMenuItemCommand command = (BocMenuItemCommand) menuItem.Command;
        if (Page is IWxePage)
          command.ExecuteWxeFunction ((IWxePage) Page, GetSelectedRows(), GetSelectedBusinessObjects());
        //else
        //  command.ExecuteWxeFunction (Page, GetSelectedRows(), GetSelectedBusinessObjects());
      }
      else
      {
        Command command = menuItem.Command;
        if (Page is IWxePage)
          command.ExecuteWxeFunction ((IWxePage) Page, null);
        //else
        //  command.ExecuteWxeFunction (Page, null, new NameValueCollection (0));
      }
    }
  }

  protected virtual void OnCustomCellClick (
      BocCustomColumnDefinition column, 
      IBusinessObject businessObject,
      string argument)
  {
    BocCustomCellClickArguments args = new BocCustomCellClickArguments (this, businessObject, column);
    column.CustomCell.Click (args, argument);
    BocCustomCellClickEventHandler clickHandler = 
        (BocCustomCellClickEventHandler) Events[s_customCellClickEvent];
    if (clickHandler != null)
    {
      BocCustomCellClickEventArgs e = new BocCustomCellClickEventArgs (column, businessObject, argument);
      clickHandler (this, e);
    }
  }

  /// <summary> Handles post back events raised by a sorting button. </summary>
  /// <param name="eventArgument"> &lt;column-index&gt; </param>
  private void HandleResorting (string eventArgument)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);

    int columnIndex;
    try 
    {
      if (eventArgument.Length == 0)
        throw new FormatException();
      columnIndex = int.Parse (eventArgument);
    }
    catch (FormatException)
    {
      throw new ArgumentException ("Argument 'eventArgument' must be an integer.");
    }

    BocColumnDefinition[] columns = EnsureColumnsForPreviousLifeCycleGot();

    if (columnIndex >= columns.Length)
      throw new ArgumentOutOfRangeException ("Column index of argument 'eventargument' was out of the range of valid values. Index must be less than the number of displayed columns.'");
    if (   ! (columns[columnIndex] is BocValueColumnDefinition) 
        && ! (   columns[columnIndex] is BocCustomColumnDefinition
              && ((BocCustomColumnDefinition) columns[columnIndex]).IsSortable))
    {
      throw new ArgumentOutOfRangeException ("The BocList '" + ID + "' does not have a value or sortable custom column at index" + columnIndex + ".");
    }

    ArrayList workingSortingOrder = new ArrayList (_sortingOrder);

    BocListSortingOrderEntry oldSortingOrderEntry = BocListSortingOrderEntry.Empty;
    BocListSortingOrderEntry newSortingOrderEntry = null;

    for (int i = 0; i < workingSortingOrder.Count; i++)
    {
      BocListSortingOrderEntry currentEntry = (BocListSortingOrderEntry) workingSortingOrder[i];
      if (currentEntry.ColumnIndex == columnIndex)
      {
        oldSortingOrderEntry = currentEntry;
        break;
      }
    }

    //  Cycle: Ascending -> Descending -> None -> Ascending
    if (! oldSortingOrderEntry.IsEmpty)
    {
      workingSortingOrder.Remove (oldSortingOrderEntry);
      switch (oldSortingOrderEntry.Direction)
      {
        case SortingDirection.Ascending:
        {
          newSortingOrderEntry = 
              new BocListSortingOrderEntry (oldSortingOrderEntry.Column, SortingDirection.Descending);
          newSortingOrderEntry.SetColumnIndex (oldSortingOrderEntry.ColumnIndex);
          break;
        }
        case SortingDirection.Descending:
        {
          newSortingOrderEntry = null;
          break;
        }
        case SortingDirection.None:
        {
          newSortingOrderEntry = 
              new BocListSortingOrderEntry (oldSortingOrderEntry.Column, SortingDirection.Ascending);
          newSortingOrderEntry.SetColumnIndex (oldSortingOrderEntry.ColumnIndex);
          break;
        }
      }
    }
    else
    {
      newSortingOrderEntry = new BocListSortingOrderEntry (columns[columnIndex], SortingDirection.Ascending);
      newSortingOrderEntry.SetColumnIndex (columnIndex);
    }

    if (newSortingOrderEntry == null)
    {
      if (workingSortingOrder.Count > 1 && ! IsMultipleSortingEnabled)
      {
        BocListSortingOrderEntry entry = (BocListSortingOrderEntry) workingSortingOrder[0];
        workingSortingOrder.Clear();
        workingSortingOrder.Add (entry);
      }
    }
    else
    {
      if (! IsMultipleSortingEnabled)
        workingSortingOrder.Clear();
      workingSortingOrder.Add (newSortingOrderEntry);
    }

    BocListSortingOrderEntry[] oldSortingOrder = 
        (BocListSortingOrderEntry[]) _sortingOrder.ToArray (typeof (BocListSortingOrderEntry));
    BocListSortingOrderEntry[] newSortingOrder = 
        (BocListSortingOrderEntry[]) workingSortingOrder.ToArray (typeof (BocListSortingOrderEntry));

    OnSortingOrderChanging (oldSortingOrder, newSortingOrder);
    _sortingOrder.Clear();
    _sortingOrder.AddRange (workingSortingOrder);
    OnSortingOrderChanged (oldSortingOrder, newSortingOrder);
    ResetRows();
  }

  protected virtual void OnSortingOrderChanging (
      BocListSortingOrderEntry[] oldSortingOrder, BocListSortingOrderEntry[] newSortingOrder)
  {
    BocListSortingOrderChangeEventHandler handler = 
        (BocListSortingOrderChangeEventHandler) Events[s_sortingOrderChangingEvent];
    if (handler != null)
    {
      BocListSortingOrderChangeEventArgs e = 
          new BocListSortingOrderChangeEventArgs (oldSortingOrder, newSortingOrder);
      handler (this, e);
    }
  }

  protected virtual void OnSortingOrderChanged (
      BocListSortingOrderEntry[] oldSortingOrder, BocListSortingOrderEntry[] newSortingOrder)
  {
    BocListSortingOrderChangeEventHandler handler = 
        (BocListSortingOrderChangeEventHandler) Events[s_sortingOrderChangedEvent];
    if (handler != null)
    {
      BocListSortingOrderChangeEventArgs e = 
          new BocListSortingOrderChangeEventArgs (oldSortingOrder, newSortingOrder);
      handler (this, e);
    }
  }

  /// <summary> Is raised when the sorting order of the <see cref="BocList"/> is about to change. </summary>
  /// <remarks> Will only be raised, if the change was caused by an UI action. </remarks>
  [Category ("Action")]
  [Description ("Occurs when the sorting order of the BocList is about to change.")]
  public event BocListSortingOrderChangeEventHandler SortingOrderChanging
  {
    add { Events.AddHandler (s_sortingOrderChangingEvent, value); }
    remove { Events.RemoveHandler (s_sortingOrderChangingEvent, value); }
  }

  /// <summary> Is raised when the sorting order of the <see cref="BocList"/> has changed. </summary>
  /// <remarks> Will only be raised, if the change was caused by an UI action. </remarks>
  [Category ("Action")]
  [Description ("Occurs when the sorting order of the BocList has to changed.")]
  public event BocListSortingOrderChangeEventHandler SortingOrderChanged
  {
    add { Events.AddHandler (s_sortingOrderChangedEvent, value); }
    remove { Events.RemoveHandler (s_sortingOrderChangedEvent, value); }
  }

  /// <summary>
  ///   Generates a <see cref="EditModeValidator"/>.
  /// </summary>
  /// <returns> Returns a list of <see cref="BaseValidator"/> objects. </returns>
  public override BaseValidator[] CreateValidators()
  {
    if (IsReadOnly)
      return new BaseValidator[0];

    BaseValidator[] validators = _editModeController.CreateValidators (GetResourceManager()); 
    _validators.AddRange (validators);

    return validators;
  }

  /// <summary> Checks whether the control conforms to the required WAI level. </summary>
  /// <exception cref="WcagException"> Thrown if the control does not conform to the required WAI level. </exception>
  protected virtual void EvaluateWaiConformity (BocColumnDefinition[] columns)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("columns", columns);

    if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
    {
      if (ShowOptionsMenu)
        WcagHelper.Instance.HandleError (1, this, "ShowOptionsMenu");
      if (ShowListMenu)
        WcagHelper.Instance.HandleError (1, this, "ShowListMenu");
      if (ShowAvailableViewsList)
        WcagHelper.Instance.HandleError (1, this, "ShowAvailableViewsList");
      bool isPagingEnabled = !_pageSize.IsNull && _pageSize.Value != 0;
      if (isPagingEnabled)
        WcagHelper.Instance.HandleError (1, this, "PageSize");
      if (EnableSorting)
        WcagHelper.Instance.HandleWarning (1, this, "EnableSorting");
      if (RowMenuDisplay == RowMenuDisplay.Automatic)
        WcagHelper.Instance.HandleError (1, this, "RowMenuDisplay");

      for (int i = 0; i < columns.Length; i++)
      {
        if (columns[i] is BocRowEditModeColumnDefinition)
          WcagHelper.Instance.HandleError (1, this, string.Format ("Columns[{0}]", i));

        BocCommandEnabledColumnDefinition commandColumn = columns[i] as BocCommandEnabledColumnDefinition;
        if (commandColumn != null)
        {
          bool hasPostBackColumnCommand =     commandColumn.Command != null
                                          && (   commandColumn.Command.Type == CommandType.Event 
                                              || commandColumn.Command.Type == CommandType.WxeFunction);
          if (hasPostBackColumnCommand)
            WcagHelper.Instance.HandleError (1, this, string.Format ("Columns[{0}].Command", i));
        }

        if (columns[i] is BocDropDownMenuColumnDefinition)
          WcagHelper.Instance.HandleError (1, this, string.Format ("Columns[{0}]", i));
      }
    }
    if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelDoubleARequired())
    {
      if (IsSelectionEnabled && ! IsIndexEnabled)
        WcagHelper.Instance.HandleError (2, this, "Selection");
    }
  }

  private void ResetRows()
  {
    _indexedRowsSorted = null;
    ResetRowMenus();
  }

  public override void PrepareValidation()
  {
    base.PrepareValidation ();

    _editModeController.PrepareValidation ();
  }

  protected override void OnPreRender(EventArgs e)
  {
    EnsureChildControls();
    base.OnPreRender (e);

    DetermineClientScriptLevel();

    if (_hasClientScript)
    {
      //  Include script file
      if (! HtmlHeadAppender.Current.IsRegistered (s_scriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            this, Context, typeof (BocList), ResourceType.Html, c_scriptFileUrl);
        HtmlHeadAppender.Current.RegisterJavaScriptInclude (s_scriptFileKey, scriptUrl);
      }

      //  Startup script initalizing the global values of the script.
      if (! Page.ClientScript.IsStartupScriptRegistered (s_startUpScriptKey))
      {
        string script = string.Format (
            "BocList_InitializeGlobals ('{0}', '{1}');",
            CssClassDataRow,
            CssClassDataRowSelected);
        ScriptUtility.RegisterStartupScriptBlock (Page, s_startUpScriptKey, script);
      }
    }

    if (! HtmlHeadAppender.Current.IsRegistered (s_styleFileKey))
    {
      string url = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (BocList), ResourceType.Html, c_styleFileUrl);
      HtmlHeadAppender.Current.RegisterStylesheetLink (s_styleFileKey, url, HtmlHeadAppender.Priority.Library);
    }

    // Must be executed before CalculateCurrentPage
    if (IsRowEditModeActive)
    {
      BocListRow[] sortedRows = EnsureGotIndexedRowsSorted ();
      for (int idxRows = 0; idxRows < sortedRows.Length; idxRows++)
      {
        int originalRowIndex = sortedRows[idxRows].Index;
        if (EditableRowIndex.Value == originalRowIndex)
        {
          _currentRow = idxRows;
          break;
        }
      }
    }
    CalculateCurrentPage (true);

    BocColumnDefinition[] columns = EnsureColumnsGot (true);

    EnsureEditModeValidatorsRestored();
    
    LoadResources (GetResourceManager());

    if (!IsDesignMode)
    {
      PreRenderMenuItems();
      
      EnsureRowMenusInitialized();
      PreRenderRowMenusItems();
      
      CreateCustomColumnControls (columns);
      InitCustomColumns();
      LoadCustomColumns();
      PreRenderCustomColumns();
    }
  }

  /// <summary> Gets a <see cref="HtmlTextWriterTag.Div"/> as the <see cref="WebControl.TagKey"/>. </summary>
  protected override HtmlTextWriterTag TagKey
  {
    get { return HtmlTextWriterTag.Div; }
  }

  protected override void AddAttributesToRender (HtmlTextWriter writer)
  {
    bool isReadOnly = IsReadOnly;
    bool isDisabled = ! Enabled;

    string backUpCssClass = CssClass; // base.CssClass and base.ControlStyle.CssClass
    if ((isReadOnly || isDisabled) && ! StringUtility.IsNullOrEmpty (CssClass))
    {
      if (isReadOnly)
        CssClass += " " + CssClassReadOnly;
      else if (isDisabled)
        CssClass += " " + CssClassDisabled;
    }
    string backUpAttributeCssClass = Attributes["class"];
    if ((isReadOnly || isDisabled) && ! StringUtility.IsNullOrEmpty (Attributes["class"]))
    {
      if (isReadOnly)
        Attributes["class"] += " " + CssClassReadOnly;
      else if (isDisabled)
        Attributes["class"] += " " + CssClassDisabled;
    }
    
    base.AddAttributesToRender (writer);

    if ((isReadOnly || isDisabled) && ! StringUtility.IsNullOrEmpty (CssClass))
      CssClass = backUpCssClass;
    if ((isReadOnly || isDisabled) && ! StringUtility.IsNullOrEmpty (Attributes["class"]))
      Attributes["class"] = backUpAttributeCssClass;
    
    if (StringUtility.IsNullOrEmpty (CssClass) && StringUtility.IsNullOrEmpty (Attributes["class"]))
    {
      string cssClass = CssClassBase;
      if (isReadOnly)
        cssClass += " " + CssClassReadOnly;
      else if (isDisabled)
        cssClass += " " + CssClassDisabled;
      writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
    }
  }

  protected void CalculateCurrentPage (bool evaluateGoTo)
  {
    if (!IsPagingEnabled || Value == null)
    {
      _pageCount = 1;
    }
    else
    {
      _currentPage = _currentRow / _pageSize.Value;
      _pageCount = (int) Math.Ceiling ((double)Value.Count / _pageSize.Value);

      if (evaluateGoTo)
      {
        switch (_goTo)
        {
          case GoToOption.First:
          {
            _currentPage = 0;
            _currentRow = 0;
            break;
          }
          case GoToOption.Last:
          {
            _currentPage = _pageCount - 1;
            _currentRow = _currentPage * _pageSize.Value;
            break;
          }
          case GoToOption.Previous:
          {
            _currentPage--;
            _currentRow = _currentPage * _pageSize.Value;
            break;
          }
          case GoToOption.Next:
          {
            _currentPage++;
            _currentRow = _currentPage * _pageSize.Value;
            break;
          }
          default:
          {
            break;
          }
        }
      }

      if (_currentPage >= _pageCount || _currentRow >= Value.Count)
      {
        _currentPage = _pageCount - 1;
        _currentRow = Value.Count - 1;
      }
      if (_currentPage < 0 || _currentRow < 0)
      {
        _currentPage = 0;
        _currentRow = 0;
      }

      if (_goTo != GoToOption.Undefined && evaluateGoTo)
      {
        _selectorControlCheckedState.Clear();
        ResetRowMenus();
      }
    }
  }

  protected override void RenderContents (HtmlTextWriter writer)
  {
    if (Page != null)
      Page.VerifyRenderingInServerForm(this);

    BocColumnDefinition[] renderColumns = EnsureColumnsGot (IsDesignMode);
    EvaluateWaiConformity (renderColumns);

    if (IsDesignMode)
    {
      //  Normally set in OnPreRender, which is omitted during design-time
      if (_pageCount == 0)
        _pageCount = 1;
    }
 
    //    bool isInternetExplorer501AndHigher = true;
    //    bool isCss21Compatible = false;
    //    if (! IsDesignMode)
    //    {
    //      bool isVersionGreaterOrEqual501 = 
    //              Context.Request.Browser.MajorVersion >= 6
    //          ||    Context.Request.Browser.MajorVersion == 5 
    //              && Context.Request.Browser.MinorVersion >= 0.01;
    //      isInternetExplorer501AndHigher = 
    //          Context.Request.Browser.Browser == "IE" && isVersionGreaterOrEqual501;
    //      bool isOperaGreaterOrEqual7 = 
    //             Context.Request.Browser.Browser == "Opera"
    //          && Context.Request.Browser.MajorVersion >= 7;
    //      bool isNetscapeCompatibleGreaterOrEqual5 = 
    //             Context.Request.Browser.Browser == "Netscape"
    //          && Context.Request.Browser.MajorVersion >= 5;
    //      isCss21Compatible = isOperaGreaterOrEqual7 || isNetscapeCompatibleGreaterOrEqual5;
    //    }

    bool hasNavigator = _alwaysShowPageInfo || _pageCount > 1;
    bool isReadOnly = IsReadOnly;
    bool showForEmptyList =  isReadOnly && _showEmptyListReadOnlyMode
                        || ! isReadOnly && _showEmptyListEditMode;
    if (! IsDesignMode && IsEmptyList && ! showForEmptyList)
      hasNavigator = false;

    //    if (isInternetExplorer501AndHigher)
    //      RenderContentsInternetExplorer501Compatible (writer, hasNavigator);
    //    else if (isCss21Compatible && ! (writer is Html32TextWriter))
    //      RenderContentsCss21Compatible (writer, hasNavigator);
    //    else
    RenderContentsLegacyBrowser (writer, hasNavigator);
  }

  private void RenderContentsInternetExplorer501Compatible (HtmlTextWriter writer, bool hasNavigator)
  {
    RenderContentsLegacyBrowser (writer, hasNavigator);
    return;
    //if (HasMenuBlock)
    //{
    //  writer.AddStyleAttribute ("display", "inline");
    //  writer.AddStyleAttribute ("float", "right");
    //  writer.AddStyleAttribute ("vertical-align", "top");
    //  string menuBlockWidth = c_defaultMenuBlockWidth;
    //  if (! _menuBlockWidth.IsEmpty)
    //    menuBlockWidth = _menuBlockWidth.ToString();
    //  writer.AddStyleAttribute (HtmlTextWriterStyle.Width, menuBlockWidth);      
    //  writer.RenderBeginTag (HtmlTextWriterTag.Div);
    //  RenderMenuBlock (writer);
    //  writer.RenderEndTag();
    //}
    //
    //writer.AddStyleAttribute ("display", "inline");
    //writer.AddStyleAttribute ("vertical-align", "top");
    //writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "auto");
    //writer.RenderBeginTag (HtmlTextWriterTag.Div);
    //RenderTableBlock (writer);
    //writer.RenderEndTag();
  }

  private void RenderContentsCss21Compatible (HtmlTextWriter writer, bool hasNavigator)
  {
    RenderContentsLegacyBrowser (writer, hasNavigator);
    return;
    //    writer.AddStyleAttribute ("display", "table");
    //    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
    //    writer.RenderBeginTag (HtmlTextWriterTag.Div);  //  Begin table
    //    
    //    writer.AddStyleAttribute ("display", "table-row");
    //    writer.RenderBeginTag (HtmlTextWriterTag.Div);  //  Begin table-row
    //
    //    writer.AddStyleAttribute ("display", "table-cell"); 
    //    writer.AddStyleAttribute ("vertical-align", "top");
    //    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "auto");
    //    writer.RenderBeginTag (HtmlTextWriterTag.Div); //  Begin table-cell
    //    RenderTableBlock (writer);
    //    writer.RenderEndTag();  //  End table-cell
    //
    //    if (HasMenuBlock)
    //    {
    //      writer.AddStyleAttribute ("display", "table-row");
    //      writer.RenderBeginTag (HtmlTextWriterTag.Div);  //  Begin table-row
    //
    //      writer.AddStyleAttribute ("display", "table-cell");
    //      writer.AddStyleAttribute ("vertical-align", "top");
    //      string menuBlockWidth = c_defaultMenuBlockWidth;
    //      if (! _menuBlockWidth.IsEmpty)
    //        menuBlockWidth = _menuBlockWidth.ToString();
    //      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, menuBlockWidth);       
    //      string menuBlockOffset = c_defaultMenuBlockOffset;
    //      if (! _menuBlockOffset.IsEmpty)
    //        menuBlockOffset = _menuBlockWidth.ToString();
    //      writer.AddStyleAttribute ("padding-left", menuBlockOffset);
    //      writer.RenderBeginTag (HtmlTextWriterTag.Div);  //  Begin table-cell
    //      RenderMenuBlock (writer);
    //      writer.RenderEndTag();  //  End table-cell
    //    }
    //    writer.RenderEndTag();  //  End table-row
    //
    //    if (hasNavigator)
    //    {
    //      writer.AddStyleAttribute ("display", "table-row");
    //      writer.RenderBeginTag (HtmlTextWriterTag.Div);  //  Begin table-row
    //
    //      writer.RenderBeginTag (HtmlTextWriterTag.Div); //  Begin table-cell
    //      RenderNavigator (writer);
    //      writer.RenderEndTag();  //  End table-cell
    //      writer.RenderBeginTag (HtmlTextWriterTag.Div); //  Begin table-cell
    //      writer.RenderEndTag();  //  End table-cell
    //
    //      writer.RenderEndTag();  //  End table-row
    //    }
    //
    //    writer.RenderEndTag();  //  End table
  }

  /// <remarks> Use display:table, display:table-row, ... for opera and mozilla/firefox </remarks>
  private void RenderContentsLegacyBrowser (HtmlTextWriter writer, bool hasNavigator)
  {
    //  Render list block / menu block
    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
    writer.RenderBeginTag (HtmlTextWriterTag.Table);

    //  Two columns
    writer.RenderBeginTag (HtmlTextWriterTag.Colgroup);

    bool isTextXml = false;
    bool isTextXhtml = false;

    if (!IsDesignMode)
    {
      isTextXml = ControlHelper.IsResponseTextXml (Context);
      isTextXhtml = ControlHelper.IsResponseTextXHtml (Context);
    }

    //  Left: list block
    writer.WriteBeginTag ("col"); //  Required because RenderBeginTag(); RenderEndTag();
    if (isTextXml || isTextXhtml)
      writer.Write (" />");
    else
      writer.Write (">");           //  writes empty tags, which is not valid for col in HTML 4.01

    if (HasMenuBlock)
    {
      //  Right: menu block
      writer.WriteBeginTag ("col");
      writer.Write (" style=\"");
      string menuBlockWidth = c_defaultMenuBlockWidth;
      if (!_menuBlockWidth.IsEmpty)
        menuBlockWidth = _menuBlockWidth.ToString ();
      writer.WriteStyleAttribute ("width", menuBlockWidth);
      string menuBlockOffset = c_defaultMenuBlockOffset;
      if (!_menuBlockOffset.IsEmpty)
        menuBlockOffset = _menuBlockOffset.ToString ();
      writer.WriteStyleAttribute ("padding-left", menuBlockOffset);
      writer.Write ("\"");
      
      if (isTextXml || isTextXhtml)
        writer.Write (" />");
      else
        writer.Write (">");
    }

    writer.RenderEndTag();  //  End ColGroup

    writer.RenderBeginTag (HtmlTextWriterTag.Tr);
    
    //  List Block
    writer.AddStyleAttribute ("vertical-align", "top");
    writer.RenderBeginTag (HtmlTextWriterTag.Td);
    RenderTableBlock (writer);
    if (hasNavigator)
      RenderNavigator (writer);
    writer.RenderEndTag();

    if (HasMenuBlock)
    {
      //  Menu Block
      writer.AddStyleAttribute ("vertical-align", "top");
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      RenderMenuBlock (writer);
      writer.RenderEndTag();
    }
    writer.RenderEndTag();  //  TR

    writer.RenderEndTag();  //  Table
  }

  protected bool HasMenuBlock
  {
    get { return HasAvailableViewsList || HasOptionsMenu || HasListMenu; }
  }

  protected bool HasAvailableViewsList
  {
    get
    {
      if (WcagHelper.Instance.IsWaiConformanceLevelARequired())
        return false;

      if (! IsInternetExplorer55OrHigher())
        return false;

      bool showAvailableViewsList =    _showAvailableViewsList 
                                    && (   _availableViews.Count > 1
                                        || IsDesignMode);
      bool isReadOnly = IsReadOnly;
      bool showForEmptyList =   isReadOnly && _showEmptyListReadOnlyMode
                             || ! isReadOnly && _showEmptyListEditMode;
      return   showAvailableViewsList
            && (! IsEmptyList || showForEmptyList);
    }
  }

  protected bool HasOptionsMenu
  {
    get
    {
      if (WcagHelper.Instance.IsWaiConformanceLevelARequired())
        return false;

      if (! IsInternetExplorer55OrHigher())
        return false;

      bool showOptionsMenu =   ShowOptionsMenu 
                            && (   OptionsMenuItems.Count > 0
                                || IsDesignMode);
      bool isReadOnly = IsReadOnly;
      bool showForEmptyList =   isReadOnly && ShowMenuForEmptyListReadOnlyMode
                             || ! isReadOnly && ShowMenuForEmptyListEditMode;
      return   showOptionsMenu
            && (! IsEmptyList || showForEmptyList);
    }
  }

  protected bool HasListMenu
  {
    get
    {
      if (WcagHelper.Instance.IsWaiConformanceLevelARequired())
        return false;

      if (! IsInternetExplorer55OrHigher())
        return false;

      bool showListMenu =   ShowListMenu 
                         && (   ListMenuItems.Count > 0
                             || IsDesignMode);
      bool isReadOnly = IsReadOnly;
      bool showForEmptyList =   isReadOnly && ShowMenuForEmptyListReadOnlyMode
                             || ! isReadOnly && ShowMenuForEmptyListEditMode;
      return   showListMenu
            && (! IsEmptyList || showForEmptyList);
    }
  }

  protected internal bool IsEmptyList
  {
    get { return Value == null || Value.Count == 0; }
  }

  protected bool IsColumnVisible (BocColumnDefinition column)
  {
    ArgumentUtility.CheckNotNull ("column", column);

    bool isReadOnly = IsReadOnly;
    BocCommandColumnDefinition commandColumn = column as BocCommandColumnDefinition;
    if (commandColumn != null && commandColumn.Command != null)
    {
      if (   WcagHelper.Instance.IsWaiConformanceLevelARequired()
          && (   commandColumn.Command.Type == CommandType.Event
              || commandColumn.Command.Type == CommandType.WxeFunction))
      {
        return false;
      }
    }

    BocRowEditModeColumnDefinition rowEditModeColumn = column as BocRowEditModeColumnDefinition;
    if (rowEditModeColumn != null)
    {
      if (WcagHelper.Instance.IsWaiConformanceLevelARequired())
        return false;
      if (   rowEditModeColumn.Show == BocRowEditColumnDefinitionShow.EditMode 
          && isReadOnly)
      {
        return false;
      }
      if (IsListEditModeActive)
        return false;
    }
    
    BocDropDownMenuColumnDefinition dropDownMenuColumn = column as BocDropDownMenuColumnDefinition;
    if (dropDownMenuColumn != null)
    {
      if (WcagHelper.Instance.IsWaiConformanceLevelARequired())
        return false;
      return IsInternetExplorer55OrHigher();
    }

    
    return true;
  }

  /// <summary> Renders the menu block of the control. </summary>
  /// <remarks> Contains the drop down list for selcting a column configuration and the options menu.  </remarks>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object that receives the server control content. </param>
  private void RenderMenuBlock (HtmlTextWriter writer)
  {
    string menuBlockItemOffset = c_defaultMenuBlockItemOffset;
    if (! _menuBlockItemOffset.IsEmpty)
      menuBlockItemOffset = _menuBlockItemOffset.ToString();

    if (HasAvailableViewsList)
    {
      PopulateAvailableViewsList();
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      writer.AddStyleAttribute ("margin-bottom", menuBlockItemOffset);
      writer.RenderBeginTag (HtmlTextWriterTag.Div);
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassAvailableViewsListLabel);
      
      writer.RenderBeginTag (HtmlTextWriterTag.Span);
      string availableViewsListTitle;
      if (StringUtility.IsNullOrEmpty (_availableViewsListTitle))
        availableViewsListTitle = GetResourceManager().GetString (ResourceIdentifier.AvailableViewsListTitle);
      else
        availableViewsListTitle = _availableViewsListTitle;
      // Do not HTML encode.
      writer.Write (availableViewsListTitle);
      writer.RenderEndTag();

      writer.Write (c_whiteSpace);
      if (IsDesignMode)
        _availableViewsList.Width = Unit.Point (c_designModeAvailableViewsListWidthInPoints);
      _availableViewsList.Enabled = ! IsRowEditModeActive && ! IsListEditModeActive;
      _availableViewsList.CssClass = CssClassAvailableViewsListDropDownList;
      _availableViewsList.RenderControl (writer);
      writer.RenderEndTag();
    }

    if (HasOptionsMenu)
    {
      if (StringUtility.IsNullOrEmpty (_optionsTitle))
        _optionsMenu.TitleText = GetResourceManager().GetString (ResourceIdentifier.OptionsTitle);
      else
        _optionsMenu.TitleText = _optionsTitle;
      _optionsMenu.Style.Add ("margin-bottom", menuBlockItemOffset);
      _optionsMenu.Enabled = ! IsRowEditModeActive;
      _optionsMenu.IsReadOnly = IsReadOnly;
      _optionsMenu.RenderControl (writer);
    }

    if (HasListMenu)
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      writer.AddStyleAttribute ("margin-bottom", menuBlockItemOffset);
      writer.AddAttribute (HtmlTextWriterAttribute.Id, ClientID + "_Boc_ListMenu");
      writer.RenderBeginTag (HtmlTextWriterTag.Div);
      RenderListMenu (writer, ClientID + "_Boc_ListMenu");
      writer.RenderEndTag();
    }
  }

  private void PopulateAvailableViewsList()
  {
    _availableViewsList.Items.Clear();

    if (_availableViews != null)
    {
      for (int i = 0; i < _availableViews.Count; i++)
      {
        BocListView columnDefinitionCollection = _availableViews[i];

        ListItem item = new ListItem (columnDefinitionCollection.Title, i.ToString());
        _availableViewsList.Items.Add (item);
        if (   ! _selectedViewIndex.IsNull 
            && _selectedViewIndex == i)
        {
          item.Selected = true;
        }
      }
    }
  }

  protected virtual bool IsInternetExplorer55OrHigher()
  {
    if (IsDesignMode)
      return true;
    
    bool isVersionGreaterOrEqual55 = 
            Context.Request.Browser.MajorVersion >= 6
        ||    Context.Request.Browser.MajorVersion == 5 
            && Context.Request.Browser.MinorVersion >= 0.5;
    bool isInternetExplorer55AndHigher = 
        Context.Request.Browser.Browser == "IE" && isVersionGreaterOrEqual55;

    return isInternetExplorer55AndHigher;
  }

  //  TODO: Move ListMenu the extra control "ContentMenu"
  private void RenderListMenu (HtmlTextWriter writer, string menuID)
  {
    if (! _hasClientScript)
      return;

    WebMenuItem[] groupedListMenuItems = _listMenuItems.GroupMenuItems (false);

    writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
    writer.RenderBeginTag (HtmlTextWriterTag.Table);
    bool isFirstItem = true;
    for (int idxItems = 0; idxItems < groupedListMenuItems.Length; idxItems++)
    {

      WebMenuItem currentItem = groupedListMenuItems[idxItems];
      if (! currentItem.EvaluateVisible())
        continue;
      // HACK: Required since ListManuItems are not added to a ListMenu's WebMenuItemCollection.
      currentItem.OwnerControl = this;

      bool isLastItem = idxItems == groupedListMenuItems.Length - 1;
      bool isFirstCategoryItem = isFirstItem || groupedListMenuItems[idxItems - 1].Category != currentItem.Category;
      bool isLastCategoryItem = isLastItem || groupedListMenuItems[idxItems + 1].Category != currentItem.Category;
      bool hasAlwaysLineBreaks = _listMenuLineBreaks == ListMenuLineBreaks.All;
      bool hasNoLineBreaks = _listMenuLineBreaks == ListMenuLineBreaks.None;

      if (hasAlwaysLineBreaks || isFirstCategoryItem || (hasNoLineBreaks && isFirstItem))
      {
        writer.RenderBeginTag (HtmlTextWriterTag.Tr);
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        writer.AddAttribute (HtmlTextWriterAttribute.Class, "contentMenuRow");
        writer.RenderBeginTag (HtmlTextWriterTag.Td);
      }
      RenderListMenuItem (writer, currentItem, menuID, _listMenuItems.IndexOf (currentItem));
      if (hasAlwaysLineBreaks || isLastCategoryItem || (hasNoLineBreaks && isLastItem))
      {
        writer.RenderEndTag();
        writer.RenderEndTag();
      }

      if (isFirstItem)
        isFirstItem = false;
    }
    writer.RenderEndTag();

    string key = UniqueID + "_ListMenuItems";
    if (! Page.ClientScript.IsStartupScriptRegistered (key))
    {
      StringBuilder script = new StringBuilder();
      script.AppendFormat ("BocList_AddMenuInfo (document.getElementById ('{0}'), \r\n\t", ClientID);
      script.AppendFormat ("new ContentMenu_MenuInfo ('{0}', new Array (\r\n", menuID);
      bool isFirstItemInGroup = true;

      for (int idxItems = 0; idxItems < groupedListMenuItems.Length; idxItems++)
      {
        WebMenuItem currentItem = groupedListMenuItems[idxItems];
        if (! currentItem.EvaluateVisible())
          continue;

        if (isFirstItemInGroup)
          isFirstItemInGroup = false;
        else
          script.AppendFormat (",\r\n");
        AppendListMenuItem (script, currentItem, menuID, _listMenuItems.IndexOf (currentItem));
      }
      script.Append (" )"); // Close Array
      script.Append (" )"); // Close new MenuInfo
      script.Append (" );\r\n"); // Close AddMenuInfo

      script.AppendFormat (
          "BocList_UpdateListMenu ( document.getElementById ('{0}'), document.getElementById ('{1}'));",
          ClientID, menuID);
      ScriptUtility.RegisterStartupScriptBlock (Page, key, script.ToString());
    }
  }

  private void AppendListMenuItem (StringBuilder stringBuilder, WebMenuItem menuItem, string menuID, int menuItemIndex)
  {
    bool isReadOnly = IsReadOnly;
    string href = "null";
    string target = "null";
    bool isCommandEnabled = true;
    if (menuItem.Command != null)
    {
      bool isActive =    menuItem.Command.Show == CommandShow.Always
                      || isReadOnly && menuItem.Command.Show == CommandShow.ReadOnly
                      || ! isReadOnly && menuItem.Command.Show == CommandShow.EditMode;

      isCommandEnabled = isActive && menuItem.Command.Type != CommandType.None;
      if (isCommandEnabled)
      {    
        bool isPostBackCommand =    menuItem.Command.Type == CommandType.Event 
                                || menuItem.Command.Type == CommandType.WxeFunction;
        if (isPostBackCommand)
        {
          // Clientside script creates an anchor with href="#" and onclick=function
          string argument = c_eventMenuItemPrefix + menuItemIndex.ToString();
          href = Page.ClientScript.GetPostBackClientHyperlink (this, argument) + ";";
          href = ScriptUtility.EscapeClientScript (href);
          href = "'" + href + "'";
        }
        else if (menuItem.Command.Type == CommandType.Href)
        {
          href = menuItem.Command.HrefCommand.FormatHref (menuItemIndex.ToString(), menuItem.ItemID);
          if (! ControlHelper.IsDesignMode (this, Context))
            href = UrlUtility.GetAbsoluteUrl (Page, href);
          href = "'" + href + "'";
          target = "'" + menuItem.Command.HrefCommand.Target + "'";
        }
      }
    }

    bool showIcon = menuItem.Style == WebMenuItemStyle.Icon ||  menuItem.Style == WebMenuItemStyle.IconAndText;
    bool showText = menuItem.Style == WebMenuItemStyle.Text ||  menuItem.Style == WebMenuItemStyle.IconAndText;
    string icon = "null";
    if (showIcon && menuItem.Icon.HasRenderingInformation)
      icon =  "'" + UrlUtility.ResolveUrl (menuItem.Icon.Url) + "'";
    string disabledIcon = "null";
    if (showIcon && menuItem.DisabledIcon.HasRenderingInformation)
      disabledIcon =  "'" + UrlUtility.ResolveUrl (menuItem.DisabledIcon.Url) + "'";
    string text = showText ? "'" +  menuItem.Text + "'" : "null";

    bool isDisabled = !menuItem.EvaluateEnabled() 
        || IsRowEditModeActive 
        || ! isCommandEnabled;
    stringBuilder.AppendFormat (
        "\t\tnew ContentMenu_MenuItemInfo ('{0}', '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8})",
        menuID + "_" + menuItemIndex.ToString(), 
        menuItem.Category, 
        text, 
        icon, 
        disabledIcon, 
        (int) menuItem.RequiredSelection,
        isDisabled ? "true" : "false",
        href,
        target);
  }

  private void RenderListMenuItem (HtmlTextWriter writer, WebMenuItem menuItem, string menuID, int index)
  {
    bool showIcon = menuItem.Style == WebMenuItemStyle.Icon ||  menuItem.Style == WebMenuItemStyle.IconAndText;
    bool showText = menuItem.Style == WebMenuItemStyle.Text ||  menuItem.Style == WebMenuItemStyle.IconAndText;

    writer.AddAttribute (HtmlTextWriterAttribute.Id, menuID + "_" + index.ToString());
    writer.RenderBeginTag (HtmlTextWriterTag.Span);
    writer.RenderBeginTag (HtmlTextWriterTag.A);
    if (showIcon && menuItem.Icon.HasRenderingInformation)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Src, UrlUtility.ResolveUrl (menuItem.Icon.Url));
      writer.AddStyleAttribute ("vertical-align", "middle");
      writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
      writer.RenderBeginTag (HtmlTextWriterTag.Img);
      writer.RenderEndTag();
      if (showText)
        writer.Write (c_whiteSpace);
    }
    if (showText)
      writer.Write (menuItem.Text); // Do not HTML encode.
    writer.RenderEndTag();
    writer.RenderEndTag();
  }

  /// <summary> Renders the list of values as an <c>table</c>. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object that receives the server control content. </param>
  private void RenderTableBlock (HtmlTextWriter writer)
  {
    bool isReadOnly = IsReadOnly;
    bool showForEmptyList =    isReadOnly && _showEmptyListReadOnlyMode
                            || ! isReadOnly && _showEmptyListEditMode;
   
    if (IsEmptyList && ! showForEmptyList)
    {
      if (IsDesignMode)
      {
        //  The table non-data sections
        RenderTableOpeningTag (writer);
        RenderColGroup (writer);
        writer.RenderBeginTag (HtmlTextWriterTag.Thead);
        RenderColumnTitlesRow (writer);
        writer.RenderEndTag();
        RenderTableClosingTag (writer);
      }
      else
      {
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
        writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
        writer.RenderBeginTag (HtmlTextWriterTag.Table);
        writer.RenderBeginTag (HtmlTextWriterTag.Tr);
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        writer.RenderBeginTag (HtmlTextWriterTag.Td);
        writer.Write ("&nbsp;");
        writer.RenderEndTag();
        writer.RenderEndTag();
        writer.RenderEndTag();
      }
    }
    else
    {
      //  The table non-data sections
      RenderTableOpeningTag (writer);
      RenderColGroup (writer);

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTableHead);
      writer.RenderBeginTag (HtmlTextWriterTag.Thead);
      RenderColumnTitlesRow (writer);
      writer.RenderEndTag();

      //  The tables data-section
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTableBody);
      writer.RenderBeginTag (HtmlTextWriterTag.Tbody);

      int firstRow = 0;
      int totalRowCount = (Value != null) ? Value.Count : 0;
      int rowCountWithOffset = totalRowCount;

      if (IsPagingEnabled && ! IsEmptyList)
      {      
        firstRow = _currentPage * _pageSize.Value;
        rowCountWithOffset = firstRow + _pageSize.Value;
        //  Check row count on last page
        rowCountWithOffset = (rowCountWithOffset < Value.Count) ? rowCountWithOffset : Value.Count;
      }

      if (IsEmptyList)
      {
        if (ShowEmptyListMessage)
          RenderEmptyListDataRow (writer);
      }
      else
      {
        bool isOddRow = true;
        BocListRow[] rows = EnsureGotIndexedRowsSorted();

        for (int idxAbsoluteRows = firstRow, idxRelativeRows = 0; 
            idxAbsoluteRows < rowCountWithOffset; 
            idxAbsoluteRows++, idxRelativeRows++)
        {
          BocListRow row = rows[idxAbsoluteRows];
          int originalRowIndex = row.Index;
          RenderDataRow (writer, row.BusinessObject, idxRelativeRows, idxAbsoluteRows, originalRowIndex, isOddRow);
          isOddRow = !isOddRow;
        }
      }

      writer.RenderEndTag(); // end Tbody

      //  Close the table
      RenderTableClosingTag (writer);
    }

    if (_hasClientScript && IsSelectionEnabled)
    {
      //  Render the init script for the client side selection handling
      int count = 0;
      if (IsPagingEnabled)
        count = _pageSize.Value;
      else if (Value != null)
        count = Value.Count;

      string script = "<script type=\"text/javascript\">\r\n<!--\r\n"
          + "BocList_InitializeList ("
          + "document.getElementById ('" + ClientID + "'), '"
          + ClientID + c_dataRowSelectorControlIDSuffix + "', "
          + count.ToString() + ","
          + (int) _selection + ");"
          + "\r\n//-->\r\n</script>";
      writer.Write (script);
    }
  }

  /// <summary> Renders the navigation bar consisting of the move buttons and the <see cref="PageInfo"/>. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object that receives the server control content. </param>
  private void RenderNavigator (HtmlTextWriter writer)
  {
    bool isFirstPage = _currentPage == 0;
    bool isLastPage = _currentPage + 1 >= _pageCount;

    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassNavigator);
    writer.AddStyleAttribute ("position", "relative");
    writer.RenderBeginTag (HtmlTextWriterTag.Div);

    //  Page info
    string pageInfo = null;
    if (StringUtility.IsNullOrEmpty (_pageInfo))
      pageInfo = GetResourceManager().GetString (ResourceIdentifier.PageInfo);
    else
      pageInfo = _pageInfo;

    string navigationText = string.Format (pageInfo, _currentPage + 1, _pageCount);
    // Do not HTML encode.
    writer.Write (navigationText);

    if (_hasClientScript)
    {
      writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);
      
      string imageUrl = null;
      //  Move to first page button
      if (isFirstPage || IsRowEditModeActive)
        imageUrl = c_goToFirstInactiveIcon;
      else
        imageUrl = c_goToFirstIcon;
      imageUrl = ResourceUrlResolver.GetResourceUrl (this, Context, typeof (BocList), ResourceType.Image, imageUrl);
      if (isFirstPage || IsRowEditModeActive)
      {
        RenderIcon (writer, new IconInfo (imageUrl), null);
      }
      else
      {
        string argument = c_goToCommandPrefix + GoToOption.First.ToString();
        string postBackEvent = Page.ClientScript.GetPostBackEventReference (this, argument);
        postBackEvent += "; return false;";
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent);
        writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
        writer.RenderBeginTag (HtmlTextWriterTag.A);
        RenderIcon (writer, new IconInfo (imageUrl), ResourceIdentifier.GoToFirstAlternateText);
        writer.RenderEndTag ();
      }
      writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);

      //  Move to previous page button
      if (isFirstPage || IsRowEditModeActive)
        imageUrl = c_goToPreviousInactiveIcon;
      else
        imageUrl = c_goToPreviousIcon;      
      imageUrl = ResourceUrlResolver.GetResourceUrl (this, Context, typeof (BocList), ResourceType.Image, imageUrl);
      if (isFirstPage || IsRowEditModeActive)
      {
        RenderIcon (writer, new IconInfo (imageUrl), null);
      }
      else
      {
        string argument = c_goToCommandPrefix + GoToOption.Previous.ToString();
        string postBackEvent = Page.ClientScript.GetPostBackEventReference (this, argument);
        postBackEvent += "; return false;";
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent);
        writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
        writer.RenderBeginTag (HtmlTextWriterTag.A);
        RenderIcon (writer, new IconInfo (imageUrl), ResourceIdentifier.GoToPreviousAlternateText);
        writer.RenderEndTag ();
      }
      writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);

      //  Move to next page button
      if (isLastPage || IsRowEditModeActive)
        imageUrl = c_goToNextInactiveIcon;
      else
        imageUrl = c_goToNextIcon;      
      imageUrl = ResourceUrlResolver.GetResourceUrl (this, Context, typeof (BocList), ResourceType.Image, imageUrl);
      if (isLastPage || IsRowEditModeActive)
      {
        RenderIcon (writer, new IconInfo (imageUrl), null);
      }
      else
      {
        string argument = c_goToCommandPrefix + GoToOption.Next.ToString();
        string postBackEvent = Page.ClientScript.GetPostBackEventReference (this, argument);
        postBackEvent += "; return false;";
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent);
        writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
        writer.RenderBeginTag (HtmlTextWriterTag.A);
        RenderIcon (writer, new IconInfo (imageUrl), ResourceIdentifier.GoToNextAlternateText);
        writer.RenderEndTag ();
      }
      writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);

      //  Move to last page button
      if (isLastPage || IsRowEditModeActive)
        imageUrl = c_goToLastInactiveIcon;
      else
        imageUrl = c_goToLastIcon;     
      imageUrl = ResourceUrlResolver.GetResourceUrl (this, Context, typeof (BocList), ResourceType.Image, imageUrl);
      if (isLastPage || IsRowEditModeActive)
      {
        RenderIcon (writer, new IconInfo (imageUrl), null);
      }
      else
      {
        string argument = c_goToCommandPrefix + GoToOption.Last.ToString();
        string postBackEvent = Page.ClientScript.GetPostBackEventReference (this, argument);
        postBackEvent += "; return false;";
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent);
        writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
        writer.RenderBeginTag (HtmlTextWriterTag.A);
        RenderIcon (writer, new IconInfo (imageUrl), ResourceIdentifier.GoToLastAlternateText);
        writer.RenderEndTag ();
      }
    }
    writer.RenderEndTag();
  }

  /// <summary> Renderes the opening tag of the table. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object that receives the server control content. </param>
  private void RenderTableOpeningTag (HtmlTextWriter writer)
  {
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTable);
    writer.AddAttribute (HtmlTextWriterAttribute.Id, ClientID + "_Table");
    writer.RenderBeginTag (HtmlTextWriterTag.Div);

    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTable);
    writer.RenderBeginTag (HtmlTextWriterTag.Table);
  }

  /// <summary> Renderes the closing tag of the table. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object that receives the server control content. </param>
  private void RenderTableClosingTag (HtmlTextWriter writer)
  {
    writer.RenderEndTag(); // table
    writer.RenderEndTag(); // div
  }

  /// <summary> Renderes the column group, which provides the table's column layout. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object that receives the server control content. </param>
  private void RenderColGroup (HtmlTextWriter writer)
  {
    BocColumnDefinition[] renderColumns = EnsureColumnsGot();

    writer.RenderBeginTag (HtmlTextWriterTag.Colgroup);

    bool isTextXml = false;
    bool isTextXhtml = false;

    if (!IsDesignMode)
    {
      isTextXml = ControlHelper.IsResponseTextXml (Context);
      isTextXhtml = ControlHelper.IsResponseTextXHtml (Context);
    }

    if (IsIndexEnabled)
    {
      writer.WriteBeginTag ("col");
      writer.Write (" style=\"");
      writer.WriteStyleAttribute ("width", "1.6em");
      writer.Write ("\"");
      if (isTextXml || isTextXhtml)
        writer.Write (" />");
      else
        writer.Write (">");
    }

    if (IsSelectionEnabled)
    {
      writer.WriteBeginTag ("col");
      writer.Write (" style=\"");
      writer.WriteStyleAttribute ("width", "1.6em");
      writer.Write ("\"");
      if (isTextXml || isTextXhtml)
        writer.Write (" />");
      else
        writer.Write (">");
    }

    //bool isFirstColumnUndefinedWidth = true;
    for (int i = 0; i < renderColumns.Length; i++)
    {
      BocColumnDefinition column = renderColumns[i];

      if (! IsColumnVisible (column))
        continue;
      
      writer.WriteBeginTag ("col");
      if (! column.Width.IsEmpty)
      {
        writer.Write (" style=\"");
        string width;
        BocValueColumnDefinition valueColumn = column as BocValueColumnDefinition;
        if (valueColumn != null && valueColumn.EnforceWidth && column.Width.Type != UnitType.Percentage)
        {
          width = "2em";
        }
        else
        {
          width = column.Width.ToString();
        }
        writer.WriteStyleAttribute ("width", width);
        writer.Write ("\"");
      }
      if (isTextXml || isTextXhtml)
        writer.Write (" />");
      else
        writer.Write (">");
    }
    
    //  Design-mode and empty table
    if (IsDesignMode && renderColumns.Length == 0)
    {
      for (int i = 0; i < c_designModeDummyColumnCount; i++)
      {
        writer.RenderBeginTag (HtmlTextWriterTag.Col);
        writer.RenderEndTag();
      }
    }
 
    writer.RenderEndTag();
  }

  /// <summary> Renders the table row containing the column titles and sorting buttons. </summary>
  /// <remarks> Title format: &lt;span&gt;label button &lt;span&gt;sort order&lt;/span&gt;&lt;/span&gt; </remarks>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object that receives the server control content. </param>
  private void RenderColumnTitlesRow (HtmlTextWriter writer)
  {
    bool isReadOnly = IsReadOnly;
    BocColumnDefinition[] renderColumns = EnsureColumnsGot();

    writer.RenderBeginTag (HtmlTextWriterTag.Tr);

    if (IsIndexEnabled)
    {
      string cssClass = CssClassTitleCell + " " + CssClassTitleCellIndex;
      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      writer.RenderBeginTag (HtmlTextWriterTag.Th);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);
      string indexColumnTitle;
      if (StringUtility.IsNullOrEmpty (_indexColumnTitle))
        indexColumnTitle = GetResourceManager().GetString (ResourceIdentifier.IndexColumnTitle);
      else
        indexColumnTitle = _indexColumnTitle;
      // Do not HTML encode.
      writer.Write (indexColumnTitle);
      writer.RenderEndTag();
      writer.RenderEndTag();
    }

    if (IsSelectionEnabled)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTitleCell);
      writer.RenderBeginTag (HtmlTextWriterTag.Th);
      if (_selection == RowSelection.Multiple)
      {
        string selectorControlName = ID + c_titleRowSelectorControlIDSuffix;
        bool isChecked = (_selectorControlCheckedState[c_titleRowIndex] != null);
        RenderSelectorControl (writer, selectorControlName, c_titleRowIndex.ToString(), isChecked, true);
      }
      else
      {
        writer.Write (c_whiteSpace);
      }
      writer.RenderEndTag();
    }

    HybridDictionary sortingDirections = new HybridDictionary();
    ArrayList sortingOrder = new ArrayList();
    if (IsClientSideSortingEnabled || HasSortingKeys)
    {
      for (int i = 0; i < _sortingOrder.Count; i++)
      {
        BocListSortingOrderEntry currentEntry = (BocListSortingOrderEntry) _sortingOrder[i];
        sortingDirections[currentEntry.ColumnIndex] = currentEntry.Direction;
        if (currentEntry.Direction != SortingDirection.None)
          sortingOrder.Add (currentEntry.ColumnIndex);
      }
    }

    for (int idxColumns = 0; idxColumns < renderColumns.Length; idxColumns++)
    {
      BocColumnDefinition column = renderColumns[idxColumns];
      BocRowEditModeColumnDefinition rowEditModeColumn = renderColumns[idxColumns] as BocRowEditModeColumnDefinition;

      if (! IsColumnVisible (column))
        continue;

      string cssClassTitleCell = CssClassTitleCell;
      if (! StringUtility.IsNullOrEmpty (column.CssClass))
        cssClassTitleCell += " " + column.CssClass;
      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTitleCell);
      writer.RenderBeginTag (HtmlTextWriterTag.Th);

      RenderTitleCellMarkers (writer, column, idxColumns);
      RenderBeginTagTitleCellSortCommand (writer, column, idxColumns);
      RenderTitleCellText (writer, column);
      if (IsClientSideSortingEnabled || HasSortingKeys)
        RenderTitleCellSortingButton (writer, idxColumns, sortingDirections, sortingOrder);
      RenderEndTagTitleCellSortCommand (writer);
      
      writer.RenderEndTag();  //  th
    }
    
    if (IsDesignMode && renderColumns.Length == 0)
    {
      for (int i = 0; i < c_designModeDummyColumnCount; i++)
      {
        writer.RenderBeginTag (HtmlTextWriterTag.Td);
        writer.Write (string.Format (c_designModeDummyColumnTitle, i + 1));
        writer.RenderEndTag();
      }
    }

    writer.RenderEndTag();  // tr
  }

  private void RenderTitleCellMarkers (HtmlTextWriter writer, BocColumnDefinition column, int columnIndex)
  {
    _editModeController.RenderTitleCellMarkers (writer, column, columnIndex);
  }

  /// <summary> Builds the input required marker. </summary>
  protected internal virtual Image GetRequiredMarker()
  {
    Image requiredIcon = new Image();
    requiredIcon.ImageUrl = ResourceUrlResolver.GetResourceUrl (
        this, Context, typeof (BocList), ResourceType.Image, c_rowEditModeRequiredFieldIcon);

    IResourceManager resourceManager = GetResourceManager();
    requiredIcon.AlternateText = resourceManager.GetString (ResourceIdentifier.RequiredFieldAlternateText);
    requiredIcon.ToolTip = resourceManager.GetString (ResourceIdentifier.RequiredFieldTitle);
    
    requiredIcon.Style["vertical-align"] = "middle";
    return requiredIcon;
  }

  /// <summary> Builds the validation error marker. </summary>
  public virtual Image GetValidationErrorMarker()
  {
    Image validationErrorIcon = new Image();
    validationErrorIcon.ImageUrl = ResourceUrlResolver.GetResourceUrl (
        this, Context, typeof (EditableRow), ResourceType.Image, c_rowEditModeValidationErrorIcon);

    IResourceManager resourceManager = GetResourceManager();
    validationErrorIcon.AlternateText = resourceManager.GetString (ResourceIdentifier.ValidationErrorInfoAlternateText);
    
    validationErrorIcon.Style["vertical-align"] = "middle";
    return validationErrorIcon;
  }

  private void RenderTitleCellText (HtmlTextWriter writer, BocColumnDefinition column)
  {
    if (IsDesignMode && column.ColumnTitleDisplayValue.Length == 0)
    {
      writer.Write (c_designModeEmptyContents);
    }
    else
    {
      string contents = HtmlUtility.HtmlEncode (column.ColumnTitleDisplayValue);
      if (StringUtility.IsNullOrEmpty (contents))
        contents = c_whiteSpace;
      writer.Write (contents);
    }
  }

  private void RenderTitleCellSortingButton (
      HtmlTextWriter writer, 
      int columnIndex, 
      HybridDictionary sortingDirections, 
      ArrayList sortingOrder)
  {
    object obj = sortingDirections[columnIndex];
    SortingDirection sortingDirection = SortingDirection.None; 
    if (obj != null)
      sortingDirection = (SortingDirection)obj;

    string imageUrl = string.Empty;
    //  Button Asc -> Button Desc -> No Button
    switch (sortingDirection)
    {
      case SortingDirection.Ascending:
      {
        imageUrl = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (BocList), ResourceType.Image, c_sortAscendingIcon);
        break;
      }
      case SortingDirection.Descending:
      {
        imageUrl = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (BocList), ResourceType.Image, c_sortDescendingIcon);
        break;
      }
      case SortingDirection.None:
      {
        break;
      }
    }

    if (sortingDirection != SortingDirection.None)
    {
      //  WhiteSpace before icon
      writer.Write (c_whiteSpace);
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassSortingOrder);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      Enum alternateTextID = null;
      if (sortingDirection == SortingDirection.Ascending)
        alternateTextID = ResourceIdentifier.SortAscendingAlternateText;
      else
        alternateTextID = ResourceIdentifier.SortDescendingAlternateText;
      RenderIcon (writer, new IconInfo (imageUrl), alternateTextID);

      if (IsShowSortingOrderEnabled && sortingOrder.Count > 1)
      {
        int orderIndex = sortingOrder.IndexOf (columnIndex);
        writer.Write (c_whiteSpace + (orderIndex + 1).ToString());
      }
      writer.RenderEndTag();
    }
  }

  private void RenderBeginTagTitleCellSortCommand (
      HtmlTextWriter writer, 
      BocColumnDefinition column,
      int columnIndex)
  {
    bool hasSortingCommand =   IsClientSideSortingEnabled 
                           && (   column is BocValueColumnDefinition
                               || (   column is BocCustomColumnDefinition 
                                   && ((BocCustomColumnDefinition) column).IsSortable));
    
    if (hasSortingCommand)
    {
      if (! IsRowEditModeActive && ! IsListEditModeActive && _hasClientScript)
      {
        string argument = c_sortCommandPrefix + columnIndex.ToString();
        string postBackEvent = Page.ClientScript.GetPostBackEventReference (this, argument);
        postBackEvent += "; return false;";
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent);
        writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
        writer.RenderBeginTag (HtmlTextWriterTag.A);
      }
      else
      {
        writer.RenderBeginTag (HtmlTextWriterTag.Span);
      }
    }
    else
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Span);
    }
  }

  private void RenderEndTagTitleCellSortCommand (HtmlTextWriter writer)
  {
    writer.RenderEndTag();
  }

  /// <summary> Renders a table row containing the data for <paramref name="businessObject"/>. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object that receives the server control content. </param>
  /// <param name="businessObject"> The <see cref="IBusinessObject"/> whose data will be rendered. </param>
  /// <param name="rowIndex"> The row number in the current view. </param>
  /// <param name="absoluteRowIndex"> The position of <paramref name="businessObject"/> in the list of values. </param>
  /// <param name="originalRowIndex"> The position of <paramref name="businessObject"/> in the list of values. </param>
  /// <param name="isOddRow"> Whether the data row is rendered in an odd or an even table row. </param>
  private void RenderDataRow (
      HtmlTextWriter writer, 
      IBusinessObject businessObject,
      int rowIndex,
      int absoluteRowIndex,
      int originalRowIndex,
      bool isOddRow)
  {
    bool isReadOnly = IsReadOnly;
    BocColumnDefinition[] renderColumns = EnsureColumnsGot();

    string objectID = null;
    IBusinessObjectWithIdentity businessObjectWithIdentity = businessObject as IBusinessObjectWithIdentity;
    if (businessObjectWithIdentity != null)
      objectID = businessObjectWithIdentity.UniqueIdentifier;

    string selectorControlID = ClientID + c_dataRowSelectorControlIDSuffix + rowIndex.ToString();
    bool isChecked = (_selectorControlCheckedState[originalRowIndex] != null);

    string cssClassTableRow;
    if (isChecked && AreDataRowsClickSensitive())
      cssClassTableRow = CssClassDataRowSelected;
    else
      cssClassTableRow = CssClassDataRow;
    
    string cssClassTableCell;
    if (isOddRow)
      cssClassTableCell = CssClassDataCellOdd;
    else
      cssClassTableCell = CssClassDataCellEven;

    if (IsSelectionEnabled && ! IsRowEditModeActive)
    {
      if (AreDataRowsClickSensitive())
      {
        string script = "BocList_OnRowClick ("
            + "document.getElementById ('" + ClientID + "'), "
            + "this, "
            + "document.getElementById ('" + selectorControlID + "'));";
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
        //  Disallow selecting text in the row.
        //  writer.AddAttribute ("onSelectStart", "return false");
      }
    }

    writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableRow);
    writer.RenderBeginTag (HtmlTextWriterTag.Tr);

    if (IsIndexEnabled)
    {
      string cssClass = cssClassTableCell + " " + CssClassDataCellIndex;
      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      if (_index == RowIndex.InitialOrder)
        RenderRowIndex (writer, originalRowIndex, selectorControlID);
      else if (_index == RowIndex.SortedOrder)
        RenderRowIndex (writer, absoluteRowIndex, selectorControlID);
      writer.RenderEndTag();
    }

    if (IsSelectionEnabled)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableCell);
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      RenderSelectorControl (writer, selectorControlID, originalRowIndex.ToString(), isChecked, false);
      writer.RenderEndTag();
    }

    BocListDataRowRenderEventArgs dataRowRenderEventArgs = 
        new BocListDataRowRenderEventArgs (originalRowIndex, businessObject);
    OnDataRowRendering (dataRowRenderEventArgs);

    bool firstValueColumnRendered = false;
    for (int idxColumns = 0; idxColumns < renderColumns.Length; idxColumns++)
    {
      bool showIcon = false;
      BocColumnDefinition column = renderColumns[idxColumns];
      if ( (!firstValueColumnRendered) && column is BocValueColumnDefinition)
      {
        firstValueColumnRendered = true;
        showIcon = EnableIcon;
      }
      RenderDataCell (
          writer, 
          idxColumns, 
          column, 
          rowIndex,
          absoluteRowIndex,
          originalRowIndex, 
          businessObject, 
          showIcon, 
          cssClassTableCell, 
          dataRowRenderEventArgs);
    }
    
    writer.RenderEndTag();
  }

  protected virtual void OnDataRowRendering (BocListDataRowRenderEventArgs e)
  {
    BocListDataRowRenderEventHandler handler = (BocListDataRowRenderEventHandler) Events[s_dataRowRenderEvent];
    if (handler != null)
      handler (this, e);
  }

  private void RenderDataCell (
      HtmlTextWriter writer, 
      int columnIndex, BocColumnDefinition column, 
      int rowIndex, int absoluteRowIndex,
      int originalRowIndex, IBusinessObject businessObject,
      bool showIcon, string cssClassTableCell,
      BocListDataRowRenderEventArgs dataRowRenderEventArgs)
  {
    bool isReadOnly = IsReadOnly;
    EditableRow editableRow = null;
    bool isEditedRow = IsRowEditModeActive && EditableRowIndex.Value == originalRowIndex;
    if (isEditedRow)
      editableRow = _editModeController._rows[0];
    else if (IsListEditModeActive)
      editableRow = _editModeController._rows[originalRowIndex];
    bool hasEditModeControl = editableRow != null && editableRow.HasEditControl (columnIndex);

    BocCommandEnabledColumnDefinition commandEnabledColumn = column as BocCommandEnabledColumnDefinition;
    BocRowEditModeColumnDefinition rowEditModeColumn = column as BocRowEditModeColumnDefinition;
    BocDropDownMenuColumnDefinition dropDownMenuColumn = column as BocDropDownMenuColumnDefinition;
    BocCustomColumnDefinition customColumn = column as BocCustomColumnDefinition;

    if (! IsColumnVisible (column))
      return;

    if (! StringUtility.IsNullOrEmpty (column.CssClass))
      cssClassTableCell += " " + column.CssClass;
    writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableCell);
    writer.RenderBeginTag (HtmlTextWriterTag.Td);

    if (commandEnabledColumn != null)
    {
      BocCommandColumnDefinition commandColumn = column as BocCommandColumnDefinition;
      BocCompoundColumnDefinition compoundColumn = column as BocCompoundColumnDefinition;
      BocSimpleColumnDefinition simpleColumn = column as BocSimpleColumnDefinition;
      BocValueColumnDefinition valueColumn = column as BocValueColumnDefinition;

      bool showEditModeControl =   hasEditModeControl 
                                && ! editableRow.GetEditControl (columnIndex).IsReadOnly;
      
      string valueColumnText = null;
      if (valueColumn != null && ! showEditModeControl)
        valueColumnText = valueColumn.GetStringValue (businessObject);


      bool enforceWidth = 
             valueColumn != null 
          && valueColumn.EnforceWidth 
          && ! column.Width.IsEmpty
          && column.Width.Type != UnitType.Percentage
          && ! showEditModeControl;

      if (enforceWidth)
      {
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, column.Width.ToString());
        writer.AddStyleAttribute ("overflow", "hidden");
        writer.AddStyleAttribute ("white-space", "nowrap");
        writer.AddStyleAttribute ("display", "block");
        writer.AddAttribute (HtmlTextWriterAttribute.Title, valueColumnText);
        writer.RenderBeginTag (HtmlTextWriterTag.Span);
      }

      //  Render the command
      bool isCommandEnabled = false;
      if (commandColumn != null || ! StringUtility.IsNullOrEmpty (valueColumnText))
      {
        isCommandEnabled = RenderBeginTagDataCellCommand (
            writer, commandEnabledColumn, businessObject, columnIndex, originalRowIndex);
      }
      if (! isCommandEnabled)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
        writer.RenderBeginTag (HtmlTextWriterTag.Span);
      }

      //  Render the icon
      if (showIcon && ! hasEditModeControl)
        RenderDataCellIcon (writer, businessObject);

      //  Render the text
      if (commandColumn != null)
      {
        RenderCommandColumnCell (writer, commandColumn);
      }
      else if (simpleColumn != null)
      {
        if (showEditModeControl)
          RenderSimpleColumnCellEditModeControl (writer, simpleColumn, businessObject, columnIndex, editableRow);
        else
          RenderValueColumnCellText (writer, valueColumnText);
      }
      else if (compoundColumn != null)
      {
        RenderValueColumnCellText (writer, valueColumnText);
      }

      if (isCommandEnabled)
        RenderEndTagDataCellCommand (writer, commandEnabledColumn); // End Command
      else
        writer.RenderEndTag(); // End Span

      if (enforceWidth)
        writer.RenderEndTag(); // End Span
    }
    else if (rowEditModeColumn != null)
    {
      RenderRowEditModeColumnCell (
          writer, 
          rowEditModeColumn, 
          isEditedRow, 
          dataRowRenderEventArgs.IsEditableRow, 
          originalRowIndex);
    }
    else if (dropDownMenuColumn != null)
    {
      RenderDropDownMenuColumnCell (writer, dropDownMenuColumn, rowIndex);
    }
    else if (customColumn != null)
    {
      RenderCustomColumnCell (
          writer, columnIndex, customColumn, rowIndex, originalRowIndex, businessObject, isEditedRow);
    }

    writer.RenderEndTag();
  }

  /// <summary> Renders the row index (Optionally as a label for the selector control). </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object that receives the server control content. </param>
  /// <param name="index"> The zero-based index to be rendered. </param>
  /// <param name="selectorControlID"> 
  ///   The ID of the selector-control for this row, or <see langword="null"/> for no control.
  /// </param>
  private void RenderRowIndex (HtmlTextWriter writer, int index, string selectorControlID) 
  {
    bool hasSelectorControl = selectorControlID != null;
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
    if (hasSelectorControl)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.For, selectorControlID);
      if (_hasClientScript)
      {
        string script = "BocList_OnSelectorControlLabelClick();";
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
      }
      writer.RenderBeginTag (HtmlTextWriterTag.Label);
    }
    else
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Span);
    }
    int renderedIndex = index + 1;
    if (! _indexOffset.IsNull)
      renderedIndex += _indexOffset.Value;
    writer.Write (renderedIndex);
    writer.RenderEndTag();
  }

  /// <summary> Renders a <see cref="CheckBox"/> or <see cref="RadioButton"/> used for row selection. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object that receives the server control content. </param>
  /// <param name="id"> The <see cref="string"/> rendered into the <c>id</c> and <c>name</c> attributes. </param>
  /// <param name="value"> The value of the <see cref="CheckBox"/> or <see cref="RadioButton"/>. </param>
  /// <param name="isChecked"> 
  ///   <see langword="true"/> if the <see cref="CheckBox"/> or <see cref="RadioButton"/> is checked. 
  /// </param>
  /// <param name="isSelectAllSelectorControl"> 
  ///   <see langword="true"/> if the rendered <see cref="CheckBox"/> or <see cref="RadioButton"/> is in the title row.
  /// </param>
  private void RenderSelectorControl (
      HtmlTextWriter writer, 
      string id, 
      string value, 
      bool isChecked, 
      bool isSelectAllSelectorControl)
  {
    if (_selection == RowSelection.SingleRadioButton)
      writer.AddAttribute (HtmlTextWriterAttribute.Type, "radio");
    else
      writer.AddAttribute (HtmlTextWriterAttribute.Type, "checkbox");
    writer.AddAttribute (HtmlTextWriterAttribute.Id, id);
    writer.AddAttribute (HtmlTextWriterAttribute.Name, id);
    string alternateText;
    if (isSelectAllSelectorControl)
      alternateText = GetResourceManager().GetString (ResourceIdentifier.SelectAllRowsAlternateText);
    else
     alternateText = GetResourceManager().GetString (ResourceIdentifier.SelectRowAlternateText);
    writer.AddAttribute (HtmlTextWriterAttribute.Alt, alternateText);

    writer.AddAttribute (HtmlTextWriterAttribute.Value, value);
    if (isChecked)
      writer.AddAttribute (HtmlTextWriterAttribute.Checked, "checked");    
    if (IsRowEditModeActive)
      writer.AddAttribute (HtmlTextWriterAttribute.Disabled, "true");
    if (isSelectAllSelectorControl)
    {
      int count = 0;
      if (IsPagingEnabled)
        count = _pageSize.Value;
      else if (! IsEmptyList)
        count = Value.Count;

      if (_hasClientScript)
      {
        string script = "BocList_OnSelectAllSelectorControlClick ("
            + "document.getElementById ('" + ClientID + "'), "
            + "this , '"
            + ClientID + c_dataRowSelectorControlIDSuffix + "', "
            + count.ToString() + ");";
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
      }
    }
    else
    {
      if (_hasClientScript)
      {
        string script = "BocList_OnSelectionSelectorControlClick();";
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
      }
    }
    writer.RenderBeginTag (HtmlTextWriterTag.Input);
    writer.RenderEndTag();
  }

  private void RenderDataCellIcon (HtmlTextWriter writer, IBusinessObject businessObject)
  {
    IconInfo icon = BusinessObjectBoundWebControl.GetIcon (
        businessObject, 
        businessObject.BusinessObjectClass.BusinessObjectProvider);

    if (icon != null)
    {
      RenderIcon (writer, icon, null);
      writer.Write (c_whiteSpace);
    }
  }

  private void RenderRowEditModeColumnCell (
      HtmlTextWriter writer, 
      BocRowEditModeColumnDefinition column,
      bool isEditedRow,
      bool isEditableRow,
      int originalRowIndex)
  {
    bool isReadOnly = IsReadOnly;
    string argument = null;
    string postBackEvent = null;

    if (isEditedRow)
    {
      if (! isReadOnly && _hasClientScript)
      {
        argument = c_eventRowEditModePrefix + originalRowIndex + "," + RowEditModeCommand.Save;
        postBackEvent = Page.ClientScript.GetPostBackEventReference (this, argument) + ";";
        writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent + c_onCommandClickScript);
      }
      writer.RenderBeginTag (HtmlTextWriterTag.A);

      bool hasSaveIcon = column.SaveIcon.HasRenderingInformation;
      bool hasSaveText = ! StringUtility.IsNullOrEmpty (column.SaveText);

      if (hasSaveIcon && hasSaveText)
      {
        RenderIcon (writer, column.SaveIcon, null);
        writer.Write (c_whiteSpace);
      }
      else if (hasSaveIcon)
      {
        RenderIcon (writer, column.SaveIcon, ResourceIdentifier.RowEditModeSaveAlternateText);
      }
      if (hasSaveText)
        writer.Write (column.SaveText); // Do not HTML encode.

      writer.RenderEndTag();

      writer.Write (" ");

      if (! isReadOnly && _hasClientScript)
      {
        argument = c_eventRowEditModePrefix + originalRowIndex + "," + RowEditModeCommand.Cancel;
        postBackEvent = Page.ClientScript.GetPostBackEventReference (this, argument) + ";";
        writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent + c_onCommandClickScript);
      }
      writer.RenderBeginTag (HtmlTextWriterTag.A);

      bool hasCancelIcon = column.CancelIcon.HasRenderingInformation;
      bool hasCancelText = ! StringUtility.IsNullOrEmpty (column.CancelText);

      if (hasCancelIcon && hasCancelText)
      {
        RenderIcon (writer, column.CancelIcon, null);
        writer.Write (c_whiteSpace);
      }
      else if (hasCancelIcon)
      {
        RenderIcon (writer, column.CancelIcon, ResourceIdentifier.RowEditModeCancelAlternateText);
      }
      if (hasCancelText)
        writer.Write (column.CancelText); // Do not HTML encode.

      writer.RenderEndTag();
    }
    else
    {
      if (isEditableRow)
      {
        if (! isReadOnly && _hasClientScript)
        {
          argument = c_eventRowEditModePrefix + originalRowIndex + "," + RowEditModeCommand.Edit;
          postBackEvent = Page.ClientScript.GetPostBackEventReference (this, argument) + ";";
          writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
          writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent + c_onCommandClickScript);
        }
        writer.RenderBeginTag (HtmlTextWriterTag.A);

        bool hasEditIcon = column.EditIcon.HasRenderingInformation;
        bool hasEditText = ! StringUtility.IsNullOrEmpty (column.EditText);

        if (hasEditIcon && hasEditText)
        {
          RenderIcon (writer, column.EditIcon, null);
          writer.Write (c_whiteSpace);
        }
        else if (hasEditIcon)
        {
          RenderIcon (writer, column.EditIcon, ResourceIdentifier.RowEditModeEditAlternateText);
        }
        if (hasEditText)
          writer.Write (column.EditText); // Do not HTML encode

        writer.RenderEndTag();
      }
      else
      {
        writer.Write (c_whiteSpace);
      }
    }
  }

  private void RenderDropDownMenuColumnCell (
      HtmlTextWriter writer, 
      BocDropDownMenuColumnDefinition column,
      int rowIndex)
  {
    if (_rowMenus == null || _rowMenus.Length < rowIndex || _rowMenus[rowIndex] == null)
    {
      writer.Write (c_whiteSpace);
      return;
    }

    DropDownMenu dropDownMenu = (DropDownMenu) _rowMenus[rowIndex].Third;
    if (dropDownMenu.MenuItems.Count == 0)
    {
      writer.Write (c_whiteSpace);
      return;
    }

    if (_hasClientScript)
      writer.AddAttribute (HtmlTextWriterAttribute.Onclick, c_onCommandClickScript);
    writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin div

    dropDownMenu.Enabled = ! IsRowEditModeActive;

    dropDownMenu.TitleText = column.MenuTitleText;
    dropDownMenu.TitleIcon = column.MenuTitleIcon;
    dropDownMenu.RenderControl (writer);
    
    writer.RenderEndTag(); // End div
  }

  private void RenderCommandColumnCell (HtmlTextWriter writer, BocCommandColumnDefinition column)
  {
    if (column.Icon.HasRenderingInformation)
      column.Icon.Render (writer);

    if (! StringUtility.IsNullOrEmpty (column.Text))
      writer.Write (column.Text); // Do not HTML encode
  }

  private void RenderValueColumnCellText (HtmlTextWriter writer, string contents) 
  {
    contents = HtmlUtility.HtmlEncode (contents);
    if (StringUtility.IsNullOrEmpty (contents))
      contents = c_whiteSpace;
    writer.Write (contents);
  }

  private void RenderSimpleColumnCellEditModeControl (
      HtmlTextWriter writer, 
      BocSimpleColumnDefinition column,
      IBusinessObject businessObject,
      int columnIndex, 
      EditableRow editableRow) 
  {
    EditModeValidator editModeValidator = null;
    for (int i = 0; i < _validators.Count; i++)
    {
      BaseValidator validator = (BaseValidator) _validators[i];
      if (validator is EditModeValidator)
        editModeValidator = (EditModeValidator) validator;
    }

    if (_hasClientScript)
      writer.AddAttribute (HtmlTextWriterAttribute.Onclick, c_onCommandClickScript);
    writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin span
    
    editableRow.RenderSimpleColumnCellEditModeControl (
        writer, column, businessObject, columnIndex,
        editModeValidator, 
        _editModeController.ShowEditModeValidationMarkers, 
        _editModeController.DisableEditModeValidationMessages);
    
    writer.RenderEndTag(); // End span
  }
 
  private bool RenderBeginTagDataCellCommand (
      HtmlTextWriter writer, 
      BocCommandEnabledColumnDefinition column,
      IBusinessObject businessObject,
      int columnIndex,
      int originalRowIndex)
  {
    bool isCommandEnabled = false;
    BocListItemCommand command = column.Command;
    if (command == null)
      return isCommandEnabled;

    bool isReadOnly = IsReadOnly;
    bool isActive =    command.Show == CommandShow.Always
                    || isReadOnly && command.Show == CommandShow.ReadOnly
                    || ! isReadOnly && command.Show == CommandShow.EditMode;
    if (   isActive
        && command.Type != CommandType.None
        && ! IsRowEditModeActive
        && (   command.CommandState == null
            || command.CommandState.IsEnabled (this, businessObject, column)))
    {
      if (WcagHelper.Instance.IsWaiConformanceLevelARequired() && command.Type != CommandType.Href)
        isCommandEnabled = false;
      else
        isCommandEnabled = true;
    }

    if (isCommandEnabled)
    {    
      string objectID = null;
      IBusinessObjectWithIdentity businessObjectWithIdentity = businessObject as IBusinessObjectWithIdentity;
      if (businessObjectWithIdentity != null)
        objectID = businessObjectWithIdentity.UniqueIdentifier;

      string argument = c_eventListItemCommandPrefix + columnIndex + "," + originalRowIndex;
      string postBackEvent = Page.ClientScript.GetPostBackEventReference (this, argument) + ";";
      string onClick = _hasClientScript ? c_onCommandClickScript : string.Empty;
      command.RenderBegin (writer, postBackEvent, onClick, originalRowIndex, objectID, businessObject as ISecurableObject);
    }

    return isCommandEnabled;
  }

  private void RenderEndTagDataCellCommand (HtmlTextWriter writer, BocCommandEnabledColumnDefinition column)
  {
    column.Command.RenderEnd (writer);
  }

  private void RenderIcon (HtmlTextWriter writer, IconInfo icon, Enum alternateTextID)
  {
    bool hasAlternateText = ! StringUtility.IsNullOrEmpty (icon.AlternateText);
    if (! hasAlternateText)
    {
      if (alternateTextID != null)
        icon.AlternateText = GetResourceManager().GetString (alternateTextID);
    }
    
    icon.Render (writer);

    if (! hasAlternateText)
      icon.AlternateText = string.Empty;
  }

  private void RenderEmptyListDataRow (HtmlTextWriter writer)
  {
    BocColumnDefinition[] renderColumns = EnsureColumnsGot();
    int columnCount = 0;

    if (IsIndexEnabled)
      columnCount++;
  
    if (IsSelectionEnabled)
      columnCount++;

    for (int idxColumns = 0; idxColumns < renderColumns.Length; idxColumns++)
    {
      BocColumnDefinition column = renderColumns[idxColumns];
      if (IsColumnVisible (column))
        columnCount++;
    }
    
    writer.RenderBeginTag (HtmlTextWriterTag.Tr);
    writer.AddAttribute (HtmlTextWriterAttribute.Colspan, columnCount.ToString());
    writer.RenderBeginTag (HtmlTextWriterTag.Td);

    string emptyListMessage = null;
    if (StringUtility.IsNullOrEmpty (_emptyListMessage))
      emptyListMessage = GetResourceManager().GetString (ResourceIdentifier.EmptyListMessage);
    else
      emptyListMessage = _emptyListMessage;
    // Do not HTML encode
    writer.Write (emptyListMessage);

    writer.RenderEndTag();
    writer.RenderEndTag();
  }

  /// <summary>
  ///   Obtains a reference to a client-side script function that causes, when invoked, a server postback to the form.
  /// </summary>
  /// <remarks> 
  ///   <para>
  ///     If the <see cref="BocList"/> is in row edit mode, <c>return false;</c> will be returned to prevent actions on 
  ///     this list.
  ///   </para><para>
  ///     Insert the return value in the rendered onClick event.
  ///   </para>
  /// </remarks>
  /// <param name="columnIndex"> The index of the column for which the post back function should be created. </param>
  /// <param name="listIndex"> The index of the business object for which the post back function should be created. </param>
  /// <param name="customCellArgument"> 
  ///   The argument to be passed to the <see cref="BocCustomColumnDefinitionCell"/>'s <c>OnClick</c> method.
  ///   Can be <see langword="null"/>.
  /// </param>
  /// <returns></returns>
  public string GetCustomCellPostBackClientEvent (int columnIndex, int listIndex, string customCellArgument)
  {
    if (IsRowEditModeActive)
      return "return false;";
    string postBackArgument = FormatCustomCellPostBackArgument (columnIndex, listIndex, customCellArgument);
    return Page.ClientScript.GetPostBackEventReference (this, postBackArgument) + ";";
  }

  /// <summary> Formats the arguments into a post back argument to be used by the client side post back event. </summary>
  private string FormatCustomCellPostBackArgument (int columnIndex, int listIndex, string customCellArgument)
  {
    if (customCellArgument == null)
      return c_customCellEventPrefix + columnIndex + "," + listIndex;
    else
      return c_customCellEventPrefix + columnIndex + "," + listIndex + "," + customCellArgument;
  }


  protected override void LoadControlState (object savedState)
  {
    object[] values = (object[]) savedState;

    base.LoadControlState (values[0]);
    _selectedViewIndex = (NaInt32) values[1];
    _availableViewsListSelectedValue = (string) values[2];
    _currentRow = (int) values[3];
    _sortingOrder = (ArrayList) values[4];
    _selectorControlCheckedState = (Hashtable) values[5];
  }

  protected override object SaveControlState ()
  {
    object[] values = new object[6];

    values[0] = base.SaveControlState ();
    values[1] = _selectedViewIndex;
    values[2] = _availableViewsListSelectedValue;
    values[3] = _currentRow;
    values[4] = _sortingOrder;
    values[5] = _selectorControlCheckedState;

    return values;
  }

  /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
  /// <include file='doc\include\UI\Controls\BocList.xml' path='BocList/LoadValue/*' />
  public override void LoadValue (bool interim)
  {
    if (Property != null && DataSource != null && DataSource.BusinessObject != null)
    {
      IList value = (IList) DataSource.BusinessObject.GetProperty (Property);
      LoadValueInternal (value, interim);
    }
  }

  /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
  /// <param name="value"> 
  ///   The <see cref="Array"/> of objects implementing <see cref="IBusinessObject"/> to load,
  ///   or <see langword="null"/>. 
  /// </param>
  /// <include file='doc\include\UI\Controls\BocList.xml' path='BocList/LoadUnboundValue/*' />
  public void LoadUnboundValue (IBusinessObject[] value, bool interim)
  {
    LoadValueInternal (value, interim);
  }

  /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
  /// <param name="value"> 
  ///   The <see cref="IList"/> of objects implementing <see cref="IBusinessObject"/> to load,
  ///   or <see langword="null"/>. 
  /// </param>
  /// <include file='doc\include\UI\Controls\BocList.xml' path='BocList/LoadUnboundValue/*' />
  public void LoadUnboundValue (IList value, bool interim)
  {
    LoadValueInternal (value, interim);
  }

  /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
  protected virtual void LoadValueInternal (IList value, bool interim)
  {
    if (! interim)
    {
      if (IsRowEditModeActive)
        EndRowEditMode (false);
      else if (IsListEditModeActive)
        EndListEditMode (false);
    }

    bool isDirtyBackUp = base.IsDirty;
    Value = value;

    if (interim)
      IsDirty = isDirtyBackUp;
    else
      IsDirty = false;
  }

  /// <summary> Saves the <see cref="Value"/> into the bound <see cref="IBusinessObject"/>. </summary>
  /// <include file='doc\include\UI\Controls\BocList.xml' path='BocList/LoadValue/*' />
  public override void SaveValue (bool interim)
  {
    if (Property != null && DataSource != null && DataSource.BusinessObject != null && ! IsReadOnly)
    {
      if (! interim)
      {
        if (IsRowEditModeActive)
          EndRowEditMode (true);
        else if (IsListEditModeActive)
          EndListEditMode (true);
      }

      if (IsDirty)
        DataSource.BusinessObject.SetProperty (Property, Value);
      
      if (! interim)
        IsDirty = false;
    }
  }

  /// <summary> Find the <see cref="IResourceManager"/> for this control. </summary>
  protected virtual IResourceManager GetResourceManager()
  {
    return GetResourceManager (typeof (ResourceIdentifier));
  }

  /// <summary> Handles refreshing the bound control. </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  private void Binding_BindingChanged (object sender, EventArgs e)
  {
    _allPropertyColumns = null;
  }

  private BocColumnDefinition[] GetAllPropertyColumns()
  {
    if (_allPropertyColumns != null)
      return _allPropertyColumns;

    IBusinessObjectProperty[] properties;
    if (DataSource == null)
      properties = new IBusinessObjectProperty[0];
    if (Property == null)
      properties = DataSource.BusinessObjectClass.GetPropertyDefinitions();
    else
      properties = ((IBusinessObjectReferenceProperty)Property).ReferenceClass.GetPropertyDefinitions();
    
    _allPropertyColumns = new BocColumnDefinition[properties.Length];
    for (int i = 0; i < properties.Length; i++)
    {
      IBusinessObjectProperty property = properties[i];
      BocSimpleColumnDefinition column = new BocSimpleColumnDefinition ();
      column.ColumnTitle = property.DisplayName;
      column.PropertyPath = property.BusinessObjectProvider.CreatePropertyPath (new IBusinessObjectProperty[] {property});
      column.OwnerControl = this;
      _allPropertyColumns[i] = column;
    }
    return _allPropertyColumns;
  }

  protected virtual void InitializeMenusItems()
  {
  }

  protected virtual void PreRenderMenuItems()
  {
    if (_hiddenMenuItems == null)
      return;

    BocDropDownMenu.HideMenuItems (ListMenuItems, _hiddenMenuItems);
    BocDropDownMenu.HideMenuItems (OptionsMenuItems, _hiddenMenuItems);
  }

  /// <summary> 
  ///   Forces the recreation of the menus to be displayed in the <see cref="BocDropDownMenuColumnDefinition"/>.
  /// </summary>
  private void ResetRowMenus()
  {
    _rowMenus = null;
  }

  /// <summary> 
  ///   Creates a <see cref="BocDropDownMenuColumnDefinition"/> if <see cref="RowMenuDisplay"/> is set to
  ///   <see cref="!:RowMenuDisplay.Automatic"/>.
  /// </summary>
  /// <returns> A <see cref="BocDropDownMenuColumnDefinition"/> instance or <see langword="null"/>. </returns>
  private BocDropDownMenuColumnDefinition GetRowMenuColumn()
  {
    if (_rowMenuDisplay == RowMenuDisplay.Automatic)
    {
      BocDropDownMenuColumnDefinition dropDownMenuColumn = new BocDropDownMenuColumnDefinition();
      dropDownMenuColumn.Width = Unit.Percentage (0);
      dropDownMenuColumn.MenuTitleText = GetResourceManager().GetString (ResourceIdentifier.RowMenuTitle);
      return dropDownMenuColumn;
    }
    return null;
  }

  /// <summary> 
  ///   Tests that the <paramref name="columnDefinitions"/> array holds exactly one
  ///   <see cref="BocDropDownMenuColumnDefinition"/> if the <see cref="RowMenuDisplay"/> is set to 
  ///   <see cref="!:RowMenuDisplay.Automatic"/> or <see cref="!:RowMenuDisplay.Manual"/>.
  /// </summary>
  private void CheckRowMenuColumns (BocColumnDefinition[] columnDefinitions)
  {
    bool isFound = false;
    for (int i = 0; i < columnDefinitions.Length; i++)
    {
      if (columnDefinitions[i] is BocDropDownMenuColumnDefinition)
      {
        if (isFound)
          throw new InvalidOperationException ("Only a single BocDropDownMenuColumnDefinition is allowed in the BocList '" + ID + "'.");
        isFound = true;
      }
    }
    if (RowMenuDisplay == RowMenuDisplay.Manual && ! isFound)
      throw new InvalidOperationException ("No BocDropDownMenuColumnDefinition was found in the BocList '" + ID + "' but the RowMenuDisplay was set to manual.");
  }

  private void EnsureRowMenusInitialized()
  {
    if (_rowMenus != null)
      return;
    if (! AreRowMenusEnabled)
      return;
    if (IsDesignMode)
      return;
    if (IsEmptyList)
      return;

    EnsureChildControls();
    CalculateCurrentPage (false);

    if (IsPagingEnabled)
      _rowMenus = new Triplet[PageSize.Value];
    else
      _rowMenus = new Triplet[Value.Count];
    _rowMenusPlaceHolder.Controls.Clear();


    int firstRow = 0;
    int totalRowCount = Value.Count;
    int rowCountWithOffset = totalRowCount;

    if (IsPagingEnabled)
    {      
      firstRow = _currentPage * _pageSize.Value;
      rowCountWithOffset = firstRow + _pageSize.Value;
      //  Check row count on last page
      rowCountWithOffset = (rowCountWithOffset < Value.Count) ? rowCountWithOffset : Value.Count;
    }

    BocListRow[] rows = EnsureGotIndexedRowsSorted();

    for (int idxAbsoluteRows = firstRow, idxRelativeRows = 0; 
        idxAbsoluteRows < rowCountWithOffset; 
        idxAbsoluteRows++, idxRelativeRows++)
    {
      BocListRow row = rows[idxAbsoluteRows];
      int originalRowIndex = row.Index;

      DropDownMenu dropDownMenu = new DropDownMenu (this);
      dropDownMenu.ID = ID + "_RowMenu_" + originalRowIndex.ToString();
      dropDownMenu.EventCommandClick += new WebMenuItemClickEventHandler (RowMenu_EventCommandClick);
      dropDownMenu.WxeFunctionCommandClick += new WebMenuItemClickEventHandler (RowMenu_WxeFunctionCommandClick);

      _rowMenusPlaceHolder.Controls.Add (dropDownMenu);
      WebMenuItem[] menuItems = InitializeRowMenuItems (row.BusinessObject, originalRowIndex);
      dropDownMenu.MenuItems.AddRange (menuItems);

      _rowMenus[idxRelativeRows] = new Triplet (row.BusinessObject, originalRowIndex, dropDownMenu);
    }
  }

  /// <summary> Creates the menu items for a data row. </summary>
  /// <param name="businessObject"> 
  ///   The <see cref="IBusinessObject"/> of the row for which the menu items are being generated. 
  /// </param>
  /// <param name="listIndex"> The position of the <paramref name="businessObject"/> in the list of values. </param>
  /// <returns> A <see cref="WebMenuItem"/> array with the menu items generated by the implementation. </returns>
  protected virtual WebMenuItem[] InitializeRowMenuItems (IBusinessObject businessObject, int listIndex)
  {
    return new WebMenuItem[0];
  }

  /// <summary> PreRenders the menu items for all row menus. </summary>
  private void PreRenderRowMenusItems()
  {
    if (_rowMenus == null)
      return;

    for (int i = 0; i < _rowMenus.Length; i++)
    {
      Triplet rowMenuTriplet = _rowMenus[i];
      if (rowMenuTriplet != null)
      {
        IBusinessObject businessObject = (IBusinessObject) rowMenuTriplet.First;
        int listIndex = (int) rowMenuTriplet.Second;       
        DropDownMenu dropDownMenu = (DropDownMenu) rowMenuTriplet.Third;
        PreRenderRowMenuItems (dropDownMenu.MenuItems, businessObject, listIndex);
      }
    }
  }

  /// <summary> PreRenders the menu items for a data row. </summary>
  /// <param name="menuItems"> The menu items to be displayed for the row. </param>
  /// <param name="businessObject"> 
  ///   The <see cref="IBusinessObject"/> of the row for which the menu items are being generated. 
  /// </param>
  /// <param name="listIndex"> The position of the <paramref name="businessObject"/> in the list of values. </param>
  protected virtual void PreRenderRowMenuItems (
      WebMenuItemCollection menuItems, 
      IBusinessObject businessObject, 
      int listIndex)
  {
  }

  /// <summary> 
  ///   Event handler for the <see cref="DropDownMenu.EventCommandClick"/> of the <b>RowMenu</b>.
  /// </summary>
  private void RowMenu_EventCommandClick (object sender, WebMenuItemClickEventArgs e)
  {
    for (int i = 0; i < _rowMenus.Length; i++)
    {
      Triplet rowMenuTriplet = _rowMenus[i];
      if (rowMenuTriplet != null)
      {
        DropDownMenu rowMenu = (DropDownMenu) rowMenuTriplet.Third;
        if (rowMenu == sender)
        {
          IBusinessObject businessObject = (IBusinessObject) rowMenuTriplet.First;
          int listIndex = (int) rowMenuTriplet.Second;       
          OnRowMenuItemEventCommandClick (e.Item, businessObject, listIndex);
          return;
        }
      }
    }
  }

  /// <summary> Handles the click on an Event command of a row menu. </summary>
  /// <include file='doc\include\UI\Controls\BocList.xml' path='BocList/OnRowMenuItemEventCommandClick/*' />
  protected virtual void OnRowMenuItemEventCommandClick (
      WebMenuItem menuItem, 
      IBusinessObject businessObject, 
      int listIndex)
  {
    if (menuItem != null && menuItem.Command != null)
    {
      if (menuItem is BocMenuItem)
        ((BocMenuItemCommand) menuItem.Command).OnClick ((BocMenuItem) menuItem);
      else
        menuItem.Command.OnClick();
    }
  }

  /// <summary> 
  ///   Event handler for the <see cref="DropDownMenu.WxeFunctionCommandClick"/> of the <b>RowMenu</b>.
  /// </summary>
  private void RowMenu_WxeFunctionCommandClick (object sender, WebMenuItemClickEventArgs e)
  {
    for (int i = 0; i < _rowMenus.Length; i++)
    {
      Triplet rowMenuTriplet = _rowMenus[i];
      if (rowMenuTriplet != null)
      {
        DropDownMenu rowMenu = (DropDownMenu) rowMenuTriplet.Third;
        if (rowMenu == sender)
        {
          IBusinessObject businessObject = (IBusinessObject) rowMenuTriplet.First;
          int listIndex = (int) rowMenuTriplet.Second;       
          OnRowMenuItemWxeFunctionCommandClick (e.Item, businessObject, listIndex);
          return;
        }
      }
    }
  }

  /// <summary> Handles the click to a WXE function command or a row menu. </summary>
  /// <include file='doc\include\UI\Controls\BocList.xml' path='BocList/OnRowMenuItemWxeFunctionCommandClick/*' />
  protected virtual void OnRowMenuItemWxeFunctionCommandClick (
      WebMenuItem menuItem, 
      IBusinessObject businessObject, 
      int listIndex)
  {
    if (menuItem != null && menuItem.Command != null)
    {
      if (menuItem is BocMenuItem)
      {
        BocMenuItemCommand command = (BocMenuItemCommand) menuItem.Command;
        if (Page is IWxePage)
          command.ExecuteWxeFunction ((IWxePage) Page, new int[1] {listIndex}, new IBusinessObject[1] {businessObject});
        //else
        //  command.ExecuteWxeFunction (Page, new int[1] {listIndex}, new IBusinessObject[1] {businessObject});
      }
      else
      {
        Command command = menuItem.Command;
        if (Page is IWxePage)
          command.ExecuteWxeFunction ((IWxePage) Page, null);
        //else
        //  command.ExecuteWxeFunction (Page, null, new NameValueCollection (0));
      }
    }
  }

  /// <summary> 
  ///   Gets a flag describing whether a <see cref="DropDownMenu"/> is shown in the 
  ///   <see cref="BocDropDownMenuColumnDefinition"/>.
  /// </summary>
  protected virtual bool AreRowMenusEnabled
  {
    get
    {
      if (WcagHelper.Instance.IsWaiConformanceLevelARequired())
        return false;
      if (   _rowMenuDisplay == RowMenuDisplay.Undefined
          || _rowMenuDisplay == RowMenuDisplay.Disabled)
      {
        return false;
      }
      return true;
    }
  }

  /// <summary> Restores the custom columns from the previous life cycle. </summary>
  private void RestoreCustomColumns()
  {
    if (! Page.IsPostBack)
      return;
    CreateCustomColumnControls (EnsureColumnsForPreviousLifeCycleGot());
    InitCustomColumns();
    LoadCustomColumns();
  }

  /// <summary> Creates the controls for the custom columns in the <paramref name="columns"/> array. </summary>
  private void CreateCustomColumnControls (BocColumnDefinition[] columns)
  {
    _customColumns = null;
    _customColumnsPlaceHolder.Controls.Clear ();
    
    if (IsDesignMode)
      return;
    if (IsEmptyList)
      return;

    EnsureChildControls();

    CalculateCurrentPage (false);

    _customColumns = new Hashtable ();

    int firstRow = 0;
    int totalRowCount = Value.Count;
    int rowCountWithOffset = totalRowCount;

    if (IsPagingEnabled)
    {      
      firstRow = _currentPage * _pageSize.Value;
      rowCountWithOffset = firstRow + _pageSize.Value;
      //  Check row count on last page
      rowCountWithOffset = (rowCountWithOffset < Value.Count) ? rowCountWithOffset : Value.Count;
    }

    BocListRow[] rows = EnsureGotIndexedRowsSorted();

    for (int idxColumns = 0; idxColumns < columns.Length; idxColumns++)
    {
      BocCustomColumnDefinition customColumn = columns[idxColumns] as BocCustomColumnDefinition;
      if (   customColumn != null
          && (   customColumn.Mode == BocCustomColumnDefinitionMode.ControlsInAllRows
              || customColumn.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow))
      {
        PlaceHolder placeHolder = new PlaceHolder();
        _customColumnsPlaceHolder.Controls.Add (placeHolder);

        Triplet[] customColumnTriplets;
        if (IsPagingEnabled)
          customColumnTriplets = new Triplet[PageSize.Value];
        else
          customColumnTriplets = new Triplet[Value.Count];
        _customColumns[customColumn] = customColumnTriplets;
       
        for (int idxAbsoluteRows = firstRow, idxRelativeRows = 0; 
            idxAbsoluteRows < rowCountWithOffset; 
            idxAbsoluteRows++, idxRelativeRows++)
        {
          BocListRow row = rows[idxAbsoluteRows];
          int originalRowIndex = row.Index;

          if (   customColumn.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow
              && (   EditableRowIndex.IsNull 
                  || EditableRowIndex.Value != originalRowIndex))
          {
            continue;
          }

          BocCustomCellArguments args = new BocCustomCellArguments (this, customColumn);
          Control control = customColumn.CustomCell.CreateControlInternal (args);
          control.ID = ID + "_CustomColumnControl_" + idxColumns.ToString() + "_" + originalRowIndex.ToString();
          placeHolder.Controls.Add (control);
          customColumnTriplets[idxRelativeRows] = new Triplet (row.BusinessObject, originalRowIndex, control);
        }
      }
    }
  }

  /// <summary> Invokes the <see cref="BocCustomColumnDefinitionCell.Init"/> method for each custom column. </summary>
  private void InitCustomColumns()
  {
    BocColumnDefinition[] columns = EnsureColumnsForPreviousLifeCycleGot ();
    for (int idxColumns = 0; idxColumns < columns.Length; idxColumns++)
    {
      BocCustomColumnDefinition customColumn = columns[idxColumns] as BocCustomColumnDefinition;
      if (   customColumn != null
          && (   customColumn.Mode == BocCustomColumnDefinitionMode.ControlsInAllRows
              || (   customColumn.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow
                  && IsRowEditModeActive)))

      {
        BocCustomCellArguments args = new BocCustomCellArguments (this, customColumn);
        customColumn.CustomCell.Init (args);
      }
    }
  }

  /// <summary>
  ///   Invokes the <see cref="BocCustomColumnDefinitionCell.Load"/> method for each cell with a control in the 
  ///   custom columns. 
  /// </summary>
  private void LoadCustomColumns()
  {
    if (_customColumns == null)
      return;

    BocColumnDefinition[] columns = EnsureColumnsForPreviousLifeCycleGot ();
    for (int idxColumns = 0; idxColumns < columns.Length; idxColumns++)
    {
      BocCustomColumnDefinition customColumn = columns[idxColumns] as BocCustomColumnDefinition;
      if (   customColumn != null
          && (   customColumn.Mode == BocCustomColumnDefinitionMode.ControlsInAllRows
              || customColumn.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow))
      {
        Triplet[] customColumnTriplets = (Triplet[]) _customColumns[customColumn];
        for (int idxRows = 0; idxRows < customColumnTriplets.Length; idxRows++)
        {
          Triplet customColumnTriplet = customColumnTriplets[idxRows];
          if (customColumnTriplet != null)
          {
            int originalRowIndex = (int) customColumnTriplet.Second;
            if (   customColumn.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow
                && (   EditableRowIndex.IsNull 
                    || EditableRowIndex.Value != originalRowIndex))
            {
              continue;
            }
            IBusinessObject businessObject = (IBusinessObject) customColumnTriplet.First;
            Control control = (Control) customColumnTriplet.Third;

            BocCustomCellLoadArguments args = 
                new BocCustomCellLoadArguments (this, businessObject, customColumn, originalRowIndex, control);
            customColumn.CustomCell.Load (args);
          }
        }
      }
    }
  }

  private bool ValidateCustomColumns()
  {
    if (_customColumns == null)
      return true;

    if (! IsRowEditModeActive)
      return true;

    bool isValid = true;
    BocColumnDefinition[] columns = EnsureColumnsForPreviousLifeCycleGot ();
    for (int idxColumns = 0; idxColumns < columns.Length; idxColumns++)
    {
      BocCustomColumnDefinition customColumn = columns[idxColumns] as BocCustomColumnDefinition;
      if (   customColumn != null
          && customColumn.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow)
      {
        Triplet[] customColumnTriplets = (Triplet[]) _customColumns[customColumn];
        for (int idxRows = 0; idxRows < customColumnTriplets.Length; idxRows++)
        {
          Triplet customColumnTriplet = customColumnTriplets[idxRows];
          if (customColumnTriplet != null)
          {
            int originalRowIndex = (int) customColumnTriplet.Second;
            if (EditableRowIndex.Value == originalRowIndex)
            {
              IBusinessObject businessObject = (IBusinessObject) customColumnTriplet.First;
              Control control = (Control) customColumnTriplet.Third;
              BocCustomCellValidationArguments args = 
                  new BocCustomCellValidationArguments (this, businessObject, customColumn, control);
              customColumn.CustomCell.Validate (args);    
              isValid &= args.IsValid;
            }
          }
        }
      }
    }
    return isValid;
  }

  /// <summary> Invokes the <see cref="BocCustomColumnDefinitionCell.PreRender"/> method for each custom column.  </summary>
  private void PreRenderCustomColumns()
  {
    BocColumnDefinition[] columns = EnsureColumnsGot (true);
    for (int i = 0; i < columns.Length; i++)
    {
      BocCustomColumnDefinition customColumn = columns[i] as BocCustomColumnDefinition;
      if (customColumn != null)
      {
        BocCustomCellArguments args = new BocCustomCellArguments (this, customColumn);
        customColumn.CustomCell.PreRender (args);
      }
    }
  }

  /// <summary> Renders the cells of the custom columns. </summary>
  private void RenderCustomColumnCell (
      HtmlTextWriter writer, 
      int columnIndex, 
      BocCustomColumnDefinition column,
      int rowIndex,
      int originalRowIndex, 
      IBusinessObject businessObject,
      bool isEditedRow)
  {
    if (_customColumns == null)
      return;

    if (   column.Mode == BocCustomColumnDefinitionMode.NoControls
        || (   column.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow
            && ! isEditedRow))
    {
      string onClick = _hasClientScript ? c_onCommandClickScript : string.Empty;
      BocCustomCellRenderArguments arguments = new BocCustomCellRenderArguments (
          this, businessObject, column, columnIndex, originalRowIndex, onClick);
      column.CustomCell.RenderInternal (writer, arguments);
    }
    else
    {
      Triplet[] customColumnTriplets = (Triplet[]) _customColumns[column];
      Triplet customColumnTriplet = customColumnTriplets[rowIndex];
      if (customColumnTriplet == null)
      {
        writer.Write (c_whiteSpace);
      }
      else
      {
        string onClick = _hasClientScript ? c_onCommandClickScript : string.Empty; 
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, onClick);
        writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin span
     
        Control control = (Control) customColumnTriplet.Third;
      
        CssStyleCollection controlStyle = null;
        bool isControlWidthEmpty = true;
        if (control is WebControl)
        {
          controlStyle = ((WebControl) control).Style;
          isControlWidthEmpty = ((WebControl) control).Width.IsEmpty;
        }
        else if (control is System.Web.UI.HtmlControls.HtmlControl)
        {
          controlStyle = ((System.Web.UI.HtmlControls.HtmlControl) control).Style;
        }
        if (controlStyle != null)
        {
          if (StringUtility.IsNullOrEmpty (controlStyle["width"]) && isControlWidthEmpty)
            controlStyle["width"] = "100%";
          if (StringUtility.IsNullOrEmpty (controlStyle["vertical-align"]))
            controlStyle["vertical-align"] = "middle";
        }
        control.RenderControl (writer);
      
        writer.RenderEndTag(); // End span
      }
    }
  }

  private BocColumnDefinition[] EnsureColumnsForPreviousLifeCycleGot()
  {
    if (_columnDefinitionsPostBackEventHandlingPhase == null)
      _columnDefinitionsPostBackEventHandlingPhase = GetColumnsInternal (true);
    return _columnDefinitionsPostBackEventHandlingPhase;
  }

  private BocColumnDefinition[] EnsureColumnsGot (bool forceRefresh)
  {
    if (_columnDefinitionsRenderPhase == null || forceRefresh)
      _columnDefinitionsRenderPhase = GetColumnsInternal (false);
    return _columnDefinitionsRenderPhase;
  }

  private BocColumnDefinition[] EnsureColumnsGot()
  {
    return EnsureColumnsGot (false);
  }

  /// <summary>
  ///   Compiles the <see cref="BocColumnDefinition"/> objects from the <see cref="FixedColumns"/>,
  ///   the <see cref="_allPropertyColumns"/> and the <see cref="SelectedView"/>
  ///   into a single array.
  /// </summary>
  /// <returns> An array of <see cref="BocColumnDefinition"/> objects. </returns>
  private BocColumnDefinition[] GetColumnsInternal (bool isPostBackEventPhase)
  {
    _hasAppendedAllPropertyColumnDefinitions = false;

    ArrayList columnDefinitionList = new ArrayList();
    
    AppendFixedColumns (columnDefinitionList);
    if (_showAllProperties)
      EnsureAllPropertyColumnsDefinitionsAppended (null, columnDefinitionList);  
    AppendRowMenuColumn (columnDefinitionList);
    AppendSelectedViewColumns (columnDefinitionList);
  
    BocColumnDefinition[] columnDefinitions = 
        (BocColumnDefinition[]) columnDefinitionList.ToArray (typeof (BocColumnDefinition));

    if (isPostBackEventPhase)
    {
      columnDefinitions = GetColumnsForPreviousLifeCycle (columnDefinitions);
      RestoreSortingOrderColumns (_sortingOrder, columnDefinitions);
    }
    else
    {
      columnDefinitions = GetColumns (columnDefinitions);
      SynchronizeSortingOrderColumns (_sortingOrder, columnDefinitions);
    }

    CheckRowMenuColumns (columnDefinitions);

    return columnDefinitions;
  }

  private void AppendFixedColumns (ArrayList columnDefinitionList)
  {
    foreach (BocColumnDefinition columnDefinition in _fixedColumns)
    {
      if (columnDefinition is BocAllPropertiesPlacehoderColumnDefinition)
      {
        EnsureAllPropertyColumnsDefinitionsAppended (
            (BocAllPropertiesPlacehoderColumnDefinition) columnDefinition, columnDefinitionList);
      }
      else
      {
        columnDefinitionList.Add (columnDefinition);
      }
    }
  }

  private void AppendRowMenuColumn (ArrayList columnDefinitionList)
  {
    BocDropDownMenuColumnDefinition dropDownMenuColumn = GetRowMenuColumn();
    if (dropDownMenuColumn != null)
      columnDefinitionList.Add (dropDownMenuColumn);
  }

  private void AppendSelectedViewColumns (ArrayList columnDefinitionList)
  {
    EnsureSelectedViewIndexSet();
    if (_selectedView == null)
      return;

    foreach (BocColumnDefinition columnDefinition in _selectedView.ColumnDefinitions)
    {
      if (columnDefinition is BocAllPropertiesPlacehoderColumnDefinition)
      {
        EnsureAllPropertyColumnsDefinitionsAppended (
            (BocAllPropertiesPlacehoderColumnDefinition) columnDefinition, columnDefinitionList);
      }
      else
      {
        columnDefinitionList.Add (columnDefinition);
      }
    }
  }

  private void EnsureAllPropertyColumnsDefinitionsAppended (
      BocAllPropertiesPlacehoderColumnDefinition placeholderColumnDefinition, ArrayList columnDefinitionList)
  {
    if (_hasAppendedAllPropertyColumnDefinitions)
      return;

    BocColumnDefinition[] allPropertyColumnDefinitions = GetAllPropertyColumns();
    Unit width = Unit.Empty;
    string cssClass = string.Empty;

    if (placeholderColumnDefinition != null)
    {
      if (! placeholderColumnDefinition.Width.IsEmpty)
      {
        double value = placeholderColumnDefinition.Width.Value / allPropertyColumnDefinitions.Length;
        value = Math.Round (value, 1);
        width = new Unit (value, placeholderColumnDefinition.Width.Type);
      }
      cssClass = placeholderColumnDefinition.CssClass;
    }

    foreach (BocColumnDefinition columnDefinition in allPropertyColumnDefinitions)
    {
      columnDefinition.CssClass = cssClass;
      columnDefinition.Width = width;
    }

    columnDefinitionList.AddRange (allPropertyColumnDefinitions);
    _hasAppendedAllPropertyColumnDefinitions = true;
  }

  private void RestoreSortingOrderColumns (ArrayList sortingOrder, BocColumnDefinition[] columnDefinitions)
  {
    ArrayList entriesToBeRemoved = new ArrayList();
    for (int i = 0; i < sortingOrder.Count; i++)
    {
      BocListSortingOrderEntry entry = (BocListSortingOrderEntry) sortingOrder[i];
      if (entry.IsEmpty)
      {
        entriesToBeRemoved.Add (entry);
        continue;
      }
      if (entry.ColumnIndex != Int32.MinValue)
        entry.SetColumn (columnDefinitions[entry.ColumnIndex]);
    }
    for (int i = 0; i < entriesToBeRemoved.Count; i++)
      sortingOrder.Remove (entriesToBeRemoved[i]);
  }

  private void SynchronizeSortingOrderColumns (ArrayList sortingOrder, BocColumnDefinition[] columnDefinitions)
  {
    ArrayList columnDefinitionList = new ArrayList (columnDefinitions);
  
    ArrayList entriesToBeRemoved = new ArrayList();
    for (int i = 0; i < sortingOrder.Count; i++)
    {
      BocListSortingOrderEntry entry = (BocListSortingOrderEntry) sortingOrder[i];
      if (entry.IsEmpty)
        continue;
      if (entry.Column == null)
      {
        entry.SetColumn (columnDefinitions[entry.ColumnIndex]);
      }
      else
      {
        entry.SetColumnIndex (columnDefinitionList.IndexOf (entry.Column));
        if (entry.ColumnIndex < 0)
        {
          sortingOrder[i] = BocListSortingOrderEntry.Empty;
          entriesToBeRemoved.Add (entry);
        }
      }
    }
    for (int i = 0; i < entriesToBeRemoved.Count; i++)
      sortingOrder.Remove (entriesToBeRemoved[i]);
  }

  /// <summary>
  ///   Override this method to modify the column definitions displayed in the <see cref="BocList"/> during the
  ///   previous page life cycle.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     The <see cref="BocColumnDefinition"/> instances displayed during the last page life cycle are required 
  ///     to correctly handle the events raised on the BocList, such as an <see cref="Command"/> event 
  ///     or a data changed event.
  ///   </para><para>
  ///     Make the method protected <see langword="virtual"/> should this feature be ever required and change the 
  ///     method's body to return the passed <paramref name="columnDefinitions"/>.
  ///   </para>
  /// </remarks>
  /// <param name="columnDefinitions"> 
  ///   The <see cref="BocColumnDefinition"/> array containing the columns defined by the <see cref="BocList"/>. 
  /// </param>
  /// <returns> The <see cref="BocColumnDefinition"/> array. </returns>
  private BocColumnDefinition[] GetColumnsForPreviousLifeCycle (BocColumnDefinition[] columnDefinitions)
  {
    //  return columnDefinitions;
    return EnsureColumnsGot();
  }

  /// <summary>
  ///   Override this method to modify the column definitions displayed in the <see cref="BocList"/> in the
  ///   current page life cycle.
  /// </summary>
  /// <remarks>
  ///   This call can happen more than once in the control's life cycle, passing different 
  ///   arrays in <paramref name="columnDefinitions" />. It is therefor important to not cache the return value
  ///   in the override of <see cref="GetColumns"/>.
  /// </remarks>
  /// <param name="columnDefinitions"> 
  ///   The <see cref="BocColumnDefinition"/> array containing the columns defined by the <see cref="BocList"/>. 
  /// </param>
  /// <returns> The <see cref="BocColumnDefinition"/> array. </returns>
  protected virtual BocColumnDefinition[] GetColumns (BocColumnDefinition[] columnDefinitions)
  {
    return columnDefinitions;
  }

  /// <summary>
  ///   Gets a flag set <see langword="true"/> if the <see cref="Value"/> is sorted before it is displayed.
  /// </summary>
  [Browsable (false)]
  public bool HasSortingKeys
  {
    get 
    { 
      for (int i = 0; i < _sortingOrder.Count; i++)
      {
        BocListSortingOrderEntry sortingOrderEntry = (BocListSortingOrderEntry) _sortingOrder[i];
        if (! sortingOrderEntry.IsEmpty)
          return true;
      }
      return false;
    }
  }

  /// <summary> Sets the sorting order for the <see cref="BocList"/>. </summary>
  /// <remarks>
  ///   <para>
  ///     It is recommended to only set the sorting order when the <see cref="BocList"/> is initialized for the first 
  ///     time. During subsequent postbacks, setting the sorting order before the post back events of the 
  ///     <see cref="BocList"/> have been handled, will undo the user's chosen sorting order.
  ///   </para><para>
  ///     Does not raise the <see cref="SortingOrderChanging"/> and <see cref="SortingOrderChanged"/>.
  ///   </para><para>
  ///     Use <see cref="ClearSortingOrder"/> if you need to clear the sorting order.
  ///   </para>
  /// </remarks>
  /// <param name="newSortingOrder"> 
  ///   The new sorting order of the <see cref="BocList"/>. Must not be <see langword="null"/> or contain 
  ///   <see langword="null"/>.
  /// </param>
  /// <exception cref="InvalidOperationException">EnableMultipleSorting == False &amp;&amp; sortingOrder.Length > 1</exception>
  public void SetSortingOrder (params BocListSortingOrderEntry[] newSortingOrder)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("newSortingOrder", newSortingOrder);

    //BocListSortingOrderEntry[] oldSortingOrder = 
    //    (BocListSortingOrderEntry[]) _sortingOrder.ToArray (typeof (BocListSortingOrderEntry));

    //OnSortingOrderChanging (oldSortingOrder, newSortingOrder);

    _sortingOrder.Clear();
    if (! IsMultipleSortingEnabled && newSortingOrder.Length > 1)
      throw new InvalidOperationException ("Attempted to set multiple sorting keys but EnableMultipleSorting is False.");
    else
      _sortingOrder.AddRange (newSortingOrder);
    SynchronizeSortingOrderColumns (_sortingOrder, EnsureColumnsGot());
    
    //OnSortingOrderChanged (oldSortingOrder, newSortingOrder);

    ResetRows();
  }

  /// <summary> Clears the sorting order for the <see cref="BocList"/>. </summary>
  /// <remarks>
  ///   Does not raise the <see cref="SortingOrderChanging"/> and <see cref="SortingOrderChanged"/>.
  /// </remarks>
  public void ClearSortingOrder ()
  {
    //BocListSortingOrderEntry[] oldSortingOrder = 
    //    (BocListSortingOrderEntry[]) _sortingOrder.ToArray (typeof (BocListSortingOrderEntry));
    //BocListSortingOrderEntry[] newSortingOrder = new BocListSortingOrderEntry[0];

    //OnSortingOrderChanging (oldSortingOrder, newSortingOrder);
    _sortingOrder.Clear();
    //OnSortingOrderChanged (oldSortingOrder, newSortingOrder);

    ResetRows();
  }
  /// <summary>
  ///   Gets the sorting order for the <see cref="BocList"/>.
  /// </summary>
  /// <remarks>
  ///   If the list also contains a collection of available views, then this method shoud only be called after the 
  ///   <see cref="AvailableViews"/> have been set. Otherwise the result can vary from a wrong sorting order to an 
  ///   <see cref="IndexOutOfRangeException"/>.
  /// </remarks>
  public BocListSortingOrderEntry[] GetSortingOrder()
  {
    EnsureColumnsGot();
    return (BocListSortingOrderEntry[]) _sortingOrder.ToArray (typeof (BocListSortingOrderEntry));
  }

  /// <summary>
  ///   Sorts the <see cref="Value"/>'s <see cref="IBusinessObject"/> instances using the sorting keys
  ///   and returns the sorted <see cref="IBusinessObject"/> instances as a new array. The original values remain
  ///   untouched.
  /// </summary>
  /// <returns> 
  ///   An <see cref="IBusinessObject"/> array sorted by the sorting keys or <see langword="null"/> if the list is
  ///   not sorted.
  /// </returns>
  public IBusinessObject[] GetSortedRows()
  {
    if (! HasSortingKeys)
      return null;

    BocListRow[] sortedRows = GetIndexedRows (true);

    IBusinessObject[] sortedBusinessObjects = new IBusinessObject[sortedRows.Length];
    for (int idxRows = 0; idxRows < sortedRows.Length; idxRows++)
      sortedBusinessObjects[idxRows] =  sortedRows[idxRows].BusinessObject;

    return sortedBusinessObjects;
  }

  /// <summary> Creates an array of indexed <see cref="IBusinessObject"/> instances, optionally sorted. </summary>
  /// <param name="sorted"> <see langword="true"/> to sort the rows before returning them. </param>
  /// <returns> Pair &lt;original index, IBusinessObject&gt; </returns>
  protected BocListRow[] GetIndexedRows (bool sorted)
  {
    int rowCount = IsEmptyList ? 0 : Value.Count;
    ArrayList rows = new ArrayList (rowCount);
    for (int idxRows = 0; idxRows < rowCount; idxRows++)
      rows.Add (new BocListRow (this, idxRows, (IBusinessObject) Value[idxRows]));

    if (sorted)
      rows.Sort();
    return (BocListRow[]) rows.ToArray (typeof (BocListRow));
  }

  protected BocListRow[] EnsureGotIndexedRowsSorted()
  {
    if (_indexedRowsSorted == null)
      _indexedRowsSorted = GetIndexedRows (true);
    return _indexedRowsSorted;
  }

  /// <summary>
  ///   Removes the columns provided by <see cref="SelectedView"/> from the <see cref="_sortingOrder"/> list.
  /// </summary>
  private void RemoveDynamicColumnsFromSortingOrder()
  {
    if (HasSortingKeys)
    {
      int fixedColumnCount = _fixedColumns.Count;
      if (_showAllProperties)
        fixedColumnCount += GetAllPropertyColumns().Length;
      ArrayList entriesToBeRemoved = new ArrayList();
      for (int idxSortingKeys = 0; idxSortingKeys < _sortingOrder.Count; idxSortingKeys++)
      {
        BocListSortingOrderEntry currentEntry = (BocListSortingOrderEntry) _sortingOrder[idxSortingKeys];
        if (currentEntry.ColumnIndex >= fixedColumnCount)
          entriesToBeRemoved.Add (currentEntry);
      }
      for (int i = 0; i < entriesToBeRemoved.Count; i++)
        _sortingOrder.Remove (entriesToBeRemoved[i]);
      if (entriesToBeRemoved.Count > 0)
        ResetRows();
    }
  }

  /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
  /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
  void IResourceDispatchTarget.Dispatch (IDictionary values)
  {
    ArgumentUtility.CheckNotNull ("values", values);
    Dispatch (values);
  }

  /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
  /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
  protected virtual void Dispatch  (IDictionary values)
  {
    HybridDictionary fixedColumnValues = new HybridDictionary();
    HybridDictionary optionsMenuItemValues = new HybridDictionary();
    HybridDictionary listMenuItemValues = new HybridDictionary();
    HybridDictionary propertyValues = new HybridDictionary();

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
          case c_resourceKeyFixedColumns:
          {
            currentCollection = fixedColumnValues;
            break;
          }
          case c_resourceKeyOptionsMenuItems:
          {
            currentCollection = optionsMenuItemValues;
            break;
          }
          case c_resourceKeyListMenuItems:
          {
            currentCollection = listMenuItemValues;
            break;
          }
          default:
          {
            //  Invalid collection property
            s_log.Debug ("BocList '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' does not contain a collection property named '" + collectionID + "'.");
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
        s_log.Debug ("BocList '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' received a resource with an invalid or unknown key '" + key + "'. Required format: 'property' or 'collectionID:elementID:property'.");
      }
    }

    //  Dispatch simple properties
    ResourceDispatcher.DispatchGeneric (this, propertyValues);

    //  Dispatch to collections
    _fixedColumns.Dispatch (fixedColumnValues, this, "FixedColumns");
    OptionsMenuItems.Dispatch (optionsMenuItemValues, this, "OptionsMenuItems");
    ListMenuItems.Dispatch (listMenuItemValues, this, "ListMenuItems");
  }

  /// <summary> Loads the resources into the control's properties. </summary>
  protected override void LoadResources  (IResourceManager resourceManager)
  {
    if (resourceManager == null)
      return;
    if (IsDesignMode)
      return;
    base.LoadResources (resourceManager);

    string key;    
    key = ResourceManagerUtility.GetGlobalResourceKey (IndexColumnTitle);
    if (! StringUtility.IsNullOrEmpty (key))
      IndexColumnTitle = resourceManager.GetString (key);
    
    key = ResourceManagerUtility.GetGlobalResourceKey (PageInfo);
    if (! StringUtility.IsNullOrEmpty (key))
      PageInfo = resourceManager.GetString (key);
    
    key = ResourceManagerUtility.GetGlobalResourceKey (EmptyListMessage);
    if (! StringUtility.IsNullOrEmpty (key))
      EmptyListMessage = resourceManager.GetString (key);
    
    key = ResourceManagerUtility.GetGlobalResourceKey (OptionsTitle);
    if (! StringUtility.IsNullOrEmpty (key))
      OptionsTitle = resourceManager.GetString (key);
    
    key = ResourceManagerUtility.GetGlobalResourceKey (AvailableViewsListTitle);
    if (! StringUtility.IsNullOrEmpty (key))
      AvailableViewsListTitle = resourceManager.GetString (key);

    key = ResourceManagerUtility.GetGlobalResourceKey (ErrorMessage);
    if (! StringUtility.IsNullOrEmpty (key))
      ErrorMessage = resourceManager.GetString (key);

    _fixedColumns.LoadResources (resourceManager);
    OptionsMenuItems.LoadResources (resourceManager);
    ListMenuItems.LoadResources (resourceManager);
  }

  /// <summary>
  ///   Sets <see cref="_hasClientScript"/>  to <see langword="true"/> if 
  ///   <see cref="EnableClientScript"/> is <see langword="true"/> and the browser is an
  ///   <c>Internet Explorer 5.5</c> or later.
  /// </summary>
  private void DetermineClientScriptLevel() 
  {
    _hasClientScript = false;

    if (! ControlHelper.IsDesignMode (this, Context))
    {
      _hasClientScript = EnableClientScript;
    }
  }

  /// <summary> Is raised when a data row is rendered. </summary>
  [Category ("Action")]
  [Description ("Occurs when a data row is rendered.")]
  public event BocListDataRowRenderEventHandler DataRowRender
  {
    add { Events.AddHandler (s_dataRowRenderEvent, value); }
    remove { Events.RemoveHandler (s_dataRowRenderEvent, value); }
  }

  /// <summary> The <see cref="IBusinessObjectReferenceProperty"/> object this control is bound to. </summary>
  /// <value>An <see cref="IBusinessObjectReferenceProperty"/> object.</value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public new IBusinessObjectReferenceProperty Property
  {
    get { return (IBusinessObjectReferenceProperty) base.Property; }
    set 
    {
      IBusinessObjectReferenceProperty property = ArgumentUtility.CheckType<IBusinessObjectReferenceProperty> ("value", value);
      if (value.IsList == false)
        throw new ArgumentException ("Only properties supporting IList can be assigned to the BocList.", "value");
      base.Property = property;
    }
  }

  /// <summary> Gets or sets the current value. </summary>
  /// <value> An object implementing <see cref="IList"/>. </value>
  /// <remarks> The dirty state is reset when the value is set. </remarks>
  [Browsable (false)]
  public new IList Value
  {
    get 
    { 
      return _value; 
    }
    set 
    {
      IsDirty = true;
      _value = value; 
      ResetRows();
    }
  }

  /// <summary> Gets or sets the current value when <see cref="Value"/> through polymorphism. </summary>
  /// <value> The value must be of type <see cref="IList"/>. </value>
  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = (IList) value; }
  }

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its 
  ///   <see cref="Control.ClientID"/>.
  /// </summary>
  public override Control TargetControl 
  {
    get { return (Control) this; }
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

  /// <summary> Gets or sets the dirty flag. </summary>
  /// <value> 
  ///   Evaluates <see langword="true"/> if either the <see cref="BocList"/> or one of the edit mode controls is 
  ///   dirty.
  /// </value>
  /// <seealso cref="BusinessObjectBoundEditableWebControl.IsDirty">BusinessObjectBoundEditableWebControl.IsDirty</seealso>
  public override bool IsDirty
  {
    get
    {
      if (base.IsDirty)
        return true;
      
      return _editModeController.IsDirty();
    }
    set
    {
      base.IsDirty = value;
    }
  }

  /// <summary> 
  ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user 
  ///   interface.
  /// </summary>
  /// <returns> 
  ///   Returns the <see cref="Control.ClientID"/> values of the edit mode controls for the row currently being edited.
  /// </returns>
  /// <seealso cref="BusinessObjectBoundEditableWebControl.GetTrackedClientIDs">BusinessObjectBoundEditableWebControl.GetTrackedClientIDs</seealso>
  public override string[] GetTrackedClientIDs()
  {
    if (IsReadOnly)
      return new string[0];
    else
      return _editModeController.GetTrackedClientIDs();
  }

  /// <summary> The <see cref="BocList"/> supports properties of type <see cref="IBusinessObjectReferenceProperty"/>. </summary>
  /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
  protected override Type[] SupportedPropertyInterfaces
  {
    get { return s_supportedPropertyInterfaces; }
  }

  /// <summary> The <see cref="BocList"/> supports only list properties. </summary>
  /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="true"/>. </returns>
  /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
  protected override bool SupportsPropertyMultiplicity (bool isList)
  {
    return isList;
  }

  /// <summary> Gets the user independent column definitions. </summary>
  /// <remarks> Behavior undefined if set after initialization phase or changed between postbacks. </remarks>
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  //  Default category
  [Description ("The user independent column definitions.")]
  [DefaultValue ((string) null)]
  public BocColumnDefinitionCollection FixedColumns
  {
    get { return _fixedColumns; }
  }

  //  No designer support intended
  /// <summary> Gets the predefined column definition sets that the user can choose from at run-time. </summary>
  //  [PersistenceMode(PersistenceMode.InnerProperty)]
  //  [ListBindable (false)]
  //  //  Default category
  //  [Description ("The predefined column definition sets that the user can choose from at run-time.")]
  //  [DefaultValue ((string) null)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public BocListViewCollection AvailableViews
  {
    get { return _availableViews; }
  }

  /// <summary>
  ///   Gets or sets the selected <see cref="BocListView"/> used to
  ///   supplement the <see cref="FixedColumns"/>.
  /// </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public BocListView SelectedView
  {
    get 
    {
      EnsureSelectedViewIndexSet();
      return _selectedView; 
    }
    set
    {
      bool hasChanged = _selectedView != value; 
      _selectedView = value; 
      ArgumentUtility.CheckNotNullOrEmpty ("AvailableViews", _availableViews);
      _selectedViewIndex = NaInt32.Null;

      if (_selectedView != null)
      {
        for (int i = 0; i < _availableViews.Count; i++)
        {
          if (_availableViews[i] == _selectedView)
          {
            _selectedViewIndex = i;
            break;
          }
        }

        if (_selectedViewIndex.IsNull) 
          throw new ArgumentOutOfRangeException ("value");
      }

      if (hasChanged)
        RemoveDynamicColumnsFromSortingOrder();
    }
  }

  private void EnsureSelectedViewIndexSet()
  {
    if (_isSelectedViewIndexSet)
      return;
    if (_selectedViewIndex.IsNull)
      SelectedViewIndex = _selectedViewIndex;
    else if (_availableViews.Count == 0)
      SelectedViewIndex = NaInt32.Null;
    else if (_selectedViewIndex.Value >= _availableViews.Count)
      SelectedViewIndex = _availableViews.Count - 1;
    else
      SelectedViewIndex = _selectedViewIndex;
    _isSelectedViewIndexSet = true;
  }

  /// <summary>
  ///   Gets or sets the index of the selected <see cref="BocListView"/> used to
  ///   supplement the <see cref="FixedColumns"/>.
  /// </summary>
  private NaInt32 SelectedViewIndex
  {
    get { return _selectedViewIndex; }
    set 
    {
      if (   ! value.IsNull 
          && (value.Value < 0 || value.Value >= _availableViews.Count))
      {
        throw new ArgumentOutOfRangeException ("value");
      }

      if (   (IsRowEditModeActive || IsListEditModeActive)
          && _isSelectedViewIndexSet
          && _selectedViewIndex != value)
      {
        throw new InvalidOperationException ("The selected column definition set cannot be changed while the BocList is in row edit mode.");
      }

      bool hasIndexChanged = _selectedViewIndex != value; 
      _selectedViewIndex = value; 

      _selectedView = null;
      if (! _selectedViewIndex.IsNull)
      {
        int selectedIndex = _selectedViewIndex.Value;
        if (selectedIndex < _availableViews.Count)
          _selectedView = (BocListView) _availableViews[selectedIndex];
      }

      if (hasIndexChanged)
        RemoveDynamicColumnsFromSortingOrder();
    }
  }

  private void AvailableViews_CollectionChanged (object sender, CollectionChangeEventArgs e)
  {
    if (   _selectedViewIndex.IsNull
        && _availableViews.Count > 0)
    {
      _selectedViewIndex = 0;
    }
    else if (_selectedViewIndex >= _availableViews.Count)
    {
      if (_availableViews.Count > 0)
        _selectedViewIndex = _availableViews.Count - 1;
      else
        _selectedViewIndex = NaInt32.Null;
    }
  }

  public void ClearSelectedRows()
  {
    _selectorControlCheckedState.Clear();
  }

  /// <summary> Gets the <see cref="IBusinessObject"/> objects selected in the <see cref="BocList"/>. </summary>
  /// <returns> An array of <see cref="IBusinessObject"/> objects. </returns>
  public IBusinessObject[] GetSelectedBusinessObjects()
  {
    if (Value == null)
      return new IBusinessObject[0];

    int[] selectedRows = GetSelectedRows();
    IBusinessObject[] selectedBusinessObjects = new IBusinessObject[selectedRows.Length];

    for (int i = 0; i < selectedRows.Length; i++)
    {  
      int rowIndex = selectedRows[i];
      IBusinessObject businessObject = Value[rowIndex] as IBusinessObject;
      if (businessObject != null)
        selectedBusinessObjects[i] = businessObject;      
    }
    return selectedBusinessObjects;
  }

  /// <summary> Gets indices for the rows selected in the <see cref="BocList"/>. </summary>
  /// <returns> An array of <see cref="int"/> values. </returns>
  public int[] GetSelectedRows()
  {
    ArrayList selectedRows = new ArrayList();
    foreach (DictionaryEntry entry in _selectorControlCheckedState)
    {
      int rowIndex = (int) entry.Key;
      if (rowIndex == c_titleRowIndex)
        continue;

      bool isChecked = (bool) entry.Value;
      if (isChecked)
        selectedRows.Add (rowIndex);
    }
    selectedRows.Sort();
    return (int[]) selectedRows.ToArray (typeof (int));
  }

  /// <summary> Sets the <see cref="IBusinessObject"/> objects selected in the <see cref="BocList"/>. </summary>
  /// <param name="selectedObjects"> An <see cref="IList"/> of <see cref="IBusinessObject"/> objects. </param>>
  /// <exception cref="InvalidOperationException"> 
  ///   Thrown if the number of rows do not match the <see cref="Selection"/> mode 
  ///   or the <see cref="Value"/> is <see langword="null"/>.
  /// </exception>
  public void SetSelectedBusinessObjects (IList selectedObjects)
  {
    ArgumentUtility.CheckNotNull ("selectedObjects", selectedObjects);
    ArgumentUtility.CheckItemsNotNullAndType ("selectedObjects", selectedObjects, typeof (IBusinessObject));
    
    if (Value == null)
      return;

    if (Value == null)
      throw new InvalidOperationException (string.Format ("The BocList '{0}' does not have a Value.", ID));

    SetSelectedRows (ListUtility.IndicesOf (Value, selectedObjects, false));
  }

  
  /// <summary> Sets indices for the rows selected in the <see cref="BocList"/>. </summary>
  /// <param name="selectedRows"> An array of <see cref="int"/> values. </param>
  /// <exception cref="InvalidOperationException"> Thrown if the number of rows do not match the <see cref="Selection"/> mode.</exception>
  public void SetSelectedRows (int[] selectedRows)
  {
    ClearSelectedRows();

    if (   (_selection == RowSelection.Undefined || _selection == RowSelection.Disabled)
        && selectedRows.Length > 0)
    {
      throw new InvalidOperationException ("Cannot select rows if the BocList is set to RowSelection.Disabled.");
    }

    if (   (   _selection == RowSelection.SingleCheckBox 
            || _selection == RowSelection.SingleRadioButton)
        && selectedRows.Length > 1)
    {
      throw new InvalidOperationException ("Cannot select more than one row if the BocList is set to RowSelection.Single.");
    }

    _selectorControlCheckedState.Clear();
    for (int i = 0; i < selectedRows.Length; i++)
    {
      int rowIndex = (int) selectedRows[i];
      _selectorControlCheckedState[rowIndex] = true;
    }
  }


  /// <summary> Adds the <paramref name="businessObjects"/> to the <see cref="Value"/> collection. </summary>
  /// <remarks> Sets the dirty state. </remarks>
  public void AddRows (IBusinessObject[] businessObjects)
  {
    _editModeController.AddRows (businessObjects, EnsureColumnsForPreviousLifeCycleGot(), EnsureColumnsGot());
  }

  internal void AddRowsInternal (IBusinessObject[] businessObjects)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("businessObjects", businessObjects);

    Value = ListUtility.AddRange (Value, businessObjects, Property, false, true);
  }

  /// <summary> Adds the <paramref name="businessObject"/> to the <see cref="Value"/> collection. </summary>
  /// <remarks> Sets the dirty state. </remarks>
  public int AddRow (IBusinessObject businessObject)
  {
    return _editModeController.AddRow (businessObject, EnsureColumnsForPreviousLifeCycleGot(), EnsureColumnsGot());
  }

  internal int AddRowInternal (IBusinessObject businessObject)
  {
    ArgumentUtility.CheckNotNull ("businessObject", businessObject);
    
    Value = ListUtility.AddRange (Value, businessObject, Property, false, true);

    if (Value == null)
      return -1;
    else
      return Value.Count - 1;
  }

  /// <summary> Removes the <paramref name="businessObjects"/> from the <see cref="Value"/> collection. </summary>
  /// <remarks> Sets the dirty state. </remarks>
  public void RemoveRows (IBusinessObject[] businessObjects)
  {
    _editModeController.RemoveRows (businessObjects);
  }

  internal void RemoveRowsInternal (IBusinessObject[] businessObjects)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("businessObjects", businessObjects);
    
    if (Value != null)
      Value = ListUtility.Remove (Value, businessObjects, Property, false);
  }

  /// <summary> Removes the <paramref name="businessObject"/> from the <see cref="Value"/> collection. </summary>
  /// <remarks> Sets the dirty state. </remarks>
  public void RemoveRow (IBusinessObject businessObject)
  {
    _editModeController.RemoveRow (businessObject);
  }

  /// <summary> 
  ///   Removes the <see cref="IBusinessObject"/> at the specifed <paramref name="index"/> from the 
  ///   <see cref="Value"/> collection. 
  /// </summary>
  /// <remarks> Sets the dirty state. </remarks>
  public void RemoveRow (int index)
  {
    if (Value == null)
      return;
    if (index > Value.Count) throw new ArgumentOutOfRangeException ("index");

    RemoveRow ((IBusinessObject) Value[index]);
  }

  internal void RemoveRowInternal (IBusinessObject businessObject)
  {
    ArgumentUtility.CheckNotNull ("businessObject", businessObject);
    
    if (Value != null)
      Value = ListUtility.Remove (Value, businessObject, Property, false);
  }

  /// <summary>
  ///   Saves changes to previous edited row and starts editing for the specified row.
  /// </summary>
  /// <remarks> 
  ///   <para>
  ///     Once the list is in edit mode, it is important not to change to index of the edited 
  ///     <see cref="IBusinessObject"/> in <see cref="Value"/>. Otherwise the wrong object would be edited.
  ///     Use <see cref="IsRowEditModeActive"/> to programatically check whether it is save to insert a row.
  ///     It is always save to add a row using the <see cref="AddRow"/> and <see cref="AddRows"/> methods.
  ///   </para><para>
  ///     While the list is in row edit mode, all commands and menus for this list are disabled with the exception
  ///     of those rendered in the <see cref="BocRowEditModeColumnDefinition"/> column.
  ///   </para>
  /// </remarks>
  /// <param name="index"> The index of the row to be edited. </param>
  public void SwitchRowIntoEditMode (int index)
  {
    _editModeController.SwitchRowIntoEditMode (index, EnsureColumnsForPreviousLifeCycleGot(), EnsureColumnsGot());
  }

  /// <summary>
  ///   Saves changes to the edited row rows and (re-)starts editing for the entire list.
  /// </summary>
  /// <remarks> 
  ///   <para>
  ///     Once the list is in edit mode, it is important not to change to order of the objects in <see cref="Value"/>. 
  ///     Otherwise the wrong objects would be edited. Use <see cref="IsListEditModeActive"/> to programatically check 
  ///     whether it is save to insert a row.
  ///   </para>
  /// </remarks>
  public void SwitchListIntoEditMode ()
  {
    if (IsPagingEnabled) 
    {
      throw new InvalidOperationException (string.Format (
          "Cannot switch BocList '{0}' in to List Edit Mode: Paging Enabled.", ID));
    }

    _editModeController.SwitchListIntoEditMode (EnsureColumnsForPreviousLifeCycleGot(), EnsureColumnsGot());
  }

  /// <summary> 
  ///   Ends the current edit mode, appends the <paramref name="businessObject"/> to the list and switches the new
  ///   row into edit mode.
  /// </summary>
  /// <remarks>
  ///   If already in row edit mode and the previous row cannot be saved, the new row will not be added to the list.
  /// </remarks>
  /// <param name="businessObject"> The <see cref="IBusinessObject"/> to add. Must not be <see langword="null"/>. </param>
  public bool AddAndEditRow (IBusinessObject businessObject)
  {
    return _editModeController.AddAndEditRow (businessObject, EnsureColumnsForPreviousLifeCycleGot(), EnsureColumnsGot());
  }

  /// <summary>
  ///   Ends the current edit mode and optionally validates and saves the changes made during edit mode.
  /// </summary>
  /// <remarks> 
  ///   If <paramref name="saveChanges"/> is <see langword="true"/>, the edit mode will only be ended once the 
  ///   validation has been successful.
  /// </remarks>
  /// <param name="saveChanges"> 
  ///   <see langword="true"/> to validate and save the changes, <see langword="false"/> to discard them.
  /// </param>
  public void EndRowEditMode (bool saveChanges)
  {
    _editModeController.EndRowEditMode (saveChanges, EnsureColumnsForPreviousLifeCycleGot());
  }

  internal void EndRowEditModeCleanUp (int modifiedRowIndex)
  {
    if (! IsReadOnly)
    {
      ResetRows();
      BocListRow[] sortedRows = EnsureGotIndexedRowsSorted();
      for (int idxRows = 0; idxRows < sortedRows.Length; idxRows++)
      {
        int originalRowIndex = sortedRows[idxRows].Index;
        if (modifiedRowIndex == originalRowIndex)
        {
          _currentRow = idxRows;
          break;
        }
      }
    }
  }

  /// <summary>
  ///   Ends the current edit mode and optionally validates and saves the changes made during edit mode.
  /// </summary>
  /// <remarks> 
  ///   If <paramref name="saveChanges"/> is <see langword="true"/>, the edit mode will only be ended once the 
  ///   validation has been successful.
  /// </remarks>
  /// <param name="saveChanges"> 
  ///   <see langword="true"/> to validate and save the changes, <see langword="false"/> to discard them.
  /// </param>
  public void EndListEditMode (bool saveChanges)
  {
    _editModeController.EndListEditMode (saveChanges, EnsureColumnsForPreviousLifeCycleGot());
  }

  internal void EndListEditModeCleanUp ()
  {
    if (! IsReadOnly)
    {
      ResetRows();
    }
  }

  private void EnsureEditModeRestored ()
  {
    _editModeController.EnsureEditModeRestored (EnsureColumnsForPreviousLifeCycleGot());
  }

  private void EnsureEditModeValidatorsRestored()
  {
    _editModeController.EnsureValidatorsRestored();
  }

  /// <summary> Explicitly validates the changes made to the edit mode. </summary>
  /// <returns> <see langword="true"/> if the rows contain only valid values. </returns>
  public bool ValidateEditableRows()
  {
    return _editModeController.Validate();
  }

  internal bool ValidateEditableRowsInternal()
  {
    return ValidateCustomColumns();
  }

  /// <summary> Gets a flag that determines wheter the <see cref="BocList"/> is n row edit mode. </summary>
  /// <remarks>
  ///   Queried where the rendering depends on whether the list is in edit mode. 
  ///   Affected code: sorting buttons, additional columns list, paging buttons, selected column definition set index
  /// </remarks>
  [Browsable (false)]
  public bool IsRowEditModeActive
  {
    get { return _editModeController.IsRowEditModeActive; } 
  }

  /// <summary> Gets a flag that determines wheter the <see cref="BocList"/> is n row edit mode. </summary>
  /// <remarks>
  ///   Queried where the rendering depends on whether the list is in edit mode. 
  ///   Affected code: sorting buttons, additional columns list, paging buttons, selected column definition set index
  /// </remarks>
  [Browsable (false)]
  public bool IsListEditModeActive
  {
    get { return _editModeController.IsListEditModeActive; } 
  }

  /// <summary> Gets the index of the currently modified row. </summary>
  [Browsable (false)]
  public NaInt32 EditableRowIndex
  {
    get { return _editModeController.EditableRowIndex; }
  }

  /// <summary>
  ///   Gets or sets a flag that determines whether to show the asterisks in the title row for columns having 
  ///   edit mode controls.
  /// </summary>
  [Description ("Set false to hide the asterisks in the title row for columns having edit mode control.")]
  [Category ("Edit Mode")]
  [DefaultValue (true)]
  public bool ShowEditModeRequiredMarkers
  {
    get { return _editModeController.ShowEditModeRequiredMarkers; }
    set { _editModeController.ShowEditModeRequiredMarkers = value; }
  }

  /// <summary>
  ///   Gets or sets a flag that determines whether to show an exclamation mark in front of each control with 
  ///   an validation error.
  /// </summary>
  [Description ("Set true to show an exclamation mark in front of each control with an validation error.")]
  [Category ("Edit Mode")]
  [DefaultValue (false)]
  public bool ShowEditModeValidationMarkers
  {
    get { return _editModeController.ShowEditModeValidationMarkers; }
    set { _editModeController.ShowEditModeValidationMarkers = value; }
  }

  /// <summary>
  ///   Gets or sets a flag that determines whether to render validation messages and client side validators.
  /// </summary>
  [Description ("Set true to prevent the validation messages from being rendered. This also disables any client side validation in the edited row.")]
  [Category ("Edit Mode")]
  [DefaultValue (false)]
  public bool DisableEditModeValidationMessages
  {
    get { return _editModeController.DisableEditModeValidationMessages; }
    set { _editModeController.DisableEditModeValidationMessages = value; }
  }

  /// <summary> Gets or sets a flag that enables the <see cref="EditModeValidator"/>. </summary>
  /// <remarks> 
  ///   <see langword="false"/> to prevent the <see cref="EditModeValidator"/> from being created by
  ///   <see cref="CreateValidators"/>.
  /// </remarks>
  [Description("Enables the EditModeValidator.")]
  [Category ("Edit Mode")]
  [DefaultValue(true)]
  public bool EnableEditModeValidator
  {
    get { return _editModeController.EnableEditModeValidator; }
    set { _editModeController.EnableEditModeValidator = value; }
  }

  /// <summary> Is raised before the changes to the editable row are saved. </summary>
  [Category ("Action")]
  [Description ("Is raised before the changes to the editable row are saved.")]
  public event BocListEditableRowChangesEventHandler EditableRowChangesSaving
  {
    add { Events.AddHandler (s_editableRowChangesSavingEvent, value); }
    remove { Events.RemoveHandler (s_editableRowChangesSavingEvent, value); }
  }

  /// <summary> Is raised after the changes to the editable row have been saved. </summary>
  [Category ("Action")]
  [Description ("Is raised after the changes to the editable row have been saved.")]
  public event BocListItemEventHandler EditableRowChangesSaved
  {
    add { Events.AddHandler (s_editableRowChangesSavedEvent, value); }
    remove { Events.RemoveHandler (s_editableRowChangesSavedEvent, value); }
  }

  /// <summary> Is raised before the changes to the editable row are canceled. </summary>
  [Category ("Action")]
  [Description ("Is raised before the changes to the editable row are canceled.")]
  public event BocListEditableRowChangesEventHandler EditableRowChangesCanceling
  {
    add { Events.AddHandler (s_editableRowChangesCancelingEvent, value); }
    remove { Events.RemoveHandler (s_editableRowChangesCancelingEvent, value); }
  }

  /// <summary> Is raised after the changes to the editable row have been canceled. </summary>
  [Category ("Action")]
  [Description ("Is raised after the changes to the editable row have been canceled.")]
  public event BocListItemEventHandler EditableRowChangesCanceled
  {
    add { Events.AddHandler (s_editableRowChangesCanceledEvent, value); }
    remove { Events.RemoveHandler (s_editableRowChangesCanceledEvent, value); }
  }

  /// <summary> 
  ///   Gets or sets the <see cref="EditableRowDataSourceFactory"/> used to create the data souce for the edit mode
  ///   controls.
  /// </summary>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public EditableRowDataSourceFactory EditModeDataSourceFactory
  {
    get 
    {
      return _editModeDataSourceFactory; 
    }
    set 
    {
      ArgumentUtility.CheckNotNull ("value", value);
      _editModeDataSourceFactory = value; 
    }
  }

  /// <summary> 
  ///   Gets or sets the <see cref="EditableRowControlFactory"/> used to create the controls for the edit mode.
  /// </summary>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public EditableRowControlFactory EditModeControlFactory
  {
    get 
    {
      return _editModeControlFactory; 
    }
    set 
    {
      ArgumentUtility.CheckNotNull ("value", value);
      _editModeControlFactory = value; 
    }
  }

  protected internal virtual void OnEditableRowChangesSaving (
      int index,
      IBusinessObject businessObject,
      IBusinessObjectDataSource dataSource,
      IBusinessObjectBoundEditableWebControl[] controls)
  {
    ArgumentUtility.CheckNotNull ("businessObject", businessObject);
    ArgumentUtility.CheckNotNull ("dataSource", dataSource);
    ArgumentUtility.CheckNotNull ("controls", controls);

    BocListEditableRowChangesEventHandler handler = 
        (BocListEditableRowChangesEventHandler) Events[s_editableRowChangesSavingEvent];
    if (handler != null)
    {
      BocListEditableRowChangesEventArgs e = 
          new BocListEditableRowChangesEventArgs (index, businessObject, dataSource, controls);
      handler (this, e);
    }
  }

  protected internal virtual void OnEditableRowChangesSaved (int index, IBusinessObject businessObject)
  {
    ArgumentUtility.CheckNotNull ("businessObject", businessObject);

    BocListItemEventHandler handler = (BocListItemEventHandler) Events[s_editableRowChangesSavedEvent];
    if (handler != null)
    {
      BocListItemEventArgs e = new BocListItemEventArgs (index, businessObject);
      handler (this, e);
    }
  }

  protected internal virtual void OnEditableRowChangesCanceling (
      int index,
      IBusinessObject businessObject,
      IBusinessObjectDataSource dataSource,
      IBusinessObjectBoundEditableWebControl[] controls)
  {
    ArgumentUtility.CheckNotNull ("businessObject", businessObject);
    ArgumentUtility.CheckNotNull ("dataSource", dataSource);
    ArgumentUtility.CheckNotNull ("controls", controls);

    BocListEditableRowChangesEventHandler handler = 
        (BocListEditableRowChangesEventHandler) Events[s_editableRowChangesCancelingEvent];
    if (handler != null)
    {
      BocListEditableRowChangesEventArgs e = 
          new BocListEditableRowChangesEventArgs (index, businessObject, dataSource, controls);
      handler (this, e);
    }
  }

  protected internal virtual void OnEditableRowChangesCanceled (int index, IBusinessObject businessObject)
  {
    ArgumentUtility.CheckNotNull ("businessObject", businessObject);

    BocListItemEventHandler handler = (BocListItemEventHandler) Events[s_editableRowChangesCanceledEvent];
    if (handler != null)
    {
      BocListItemEventArgs e = new BocListItemEventArgs (index, businessObject);
      handler (this, e);
    }
  }

  
  /// <summary> Adds the <paramref name="businessObjects"/> to the <see cref="Value"/> collection. </summary>
  /// <remarks> Sets the dirty state. </remarks>
  protected virtual void InsertBusinessObjects (IBusinessObject[] businessObjects)
  {
    AddRows (businessObjects);
  }
 
  /// <summary> Removes the <paramref name="businessObjects"/> from the <see cref="Value"/> collection. </summary>
  /// <remarks> Sets the dirty state. </remarks>
  protected virtual void RemoveBusinessObjects (IBusinessObject[] businessObjects)
  {
    ClearSelectedRows();
    RemoveRows (businessObjects);
  }

  bool IBocMenuItemContainer.IsReadOnly
  {
    get { return IsReadOnly; }
  }

  bool IBocMenuItemContainer.IsSelectionEnabled
  {
    get { return IsSelectionEnabled; }
  }
  
  IBusinessObject[] IBocMenuItemContainer.GetSelectedBusinessObjects()
  {
    return GetSelectedBusinessObjects();
  }
 
  void IBocMenuItemContainer.InsertBusinessObjects (IBusinessObject[] businessObjects)
  {
    InsertBusinessObjects (businessObjects);
  }

  void IBocMenuItemContainer.RemoveBusinessObjects (IBusinessObject[] businessObjects)
  {
    RemoveBusinessObjects (businessObjects);
  }

  /// <summary> 
  ///   Gets or sets a flag that determines wheter an empty list will still render its headers 
  ///   and the additonal column sets  (read-only mode only). 
  /// </summary>
  /// <value> <see langword="false"/> to hide the headers and the addtional column sets if the list is empty. </value>
  [Category ("Appearance")]
  [Description ("Determines whether the list headers and the additional column sets will be rendered if no data is provided (read-only mode only).")]
  [DefaultValue (false)]
  public virtual bool ShowEmptyListReadOnlyMode
  {
    get { return _showEmptyListReadOnlyMode; }
    set { _showEmptyListReadOnlyMode = value; }
  }

  /// <summary> 
  ///   Gets or sets a flag that determines wheter an empty list will still render its headers 
  ///   and the additonal column sets (edit mode only). 
  /// </summary>
  /// <value> <see langword="false"/> to hide the headers and the addtional column sets if the list is empty. </value>
  [Category ("Appearance")]
  [Description ("Determines whether the list headers and the additional column sets will be rendered if no data is provided (edit mode only).")]
  [DefaultValue (true)]
  public virtual bool ShowEmptyListEditMode
  {
    get { return _showEmptyListEditMode; }
    set { _showEmptyListEditMode = value; }
  }

  /// <summary> 
  ///   Gets or sets a flag that determines wheter an empty list will still render its option and list menus. 
  ///   (read-only mode only).
  /// </summary>
  /// <value> <see langword="false"/> to hide the option and list menus if the list is empty. </value>
  [Category ("Menu")]
  [Description ("Determines whether the options and list menus will be rendered if no data is provided (read-only mode only).")]
  [DefaultValue (false)]
  public virtual bool ShowMenuForEmptyListReadOnlyMode
  {
    get { return _showMenuForEmptyListReadOnlyMode; }
    set { _showMenuForEmptyListReadOnlyMode = value; }
  }

  /// <summary> 
  ///   Gets or sets a flag that determines wheter an empty list will still render its option and list menus
  ///   (edit mode only).
  /// </summary>
  /// <value> <see langword="false"/> to hide the option and list menus if the list is empty. </value>
  [Category ("Menu")]
  [Description ("Determines whether the options and list menus will be rendered if no data is provided (edit mode only).")]
  [DefaultValue (true)]
  public virtual bool ShowMenuForEmptyListEditMode
  {
    get { return _showMenuForEmptyListEditMode; }
    set { _showMenuForEmptyListEditMode = value; }
  }

  /// <summary>
  ///   Gets or sets a flag that indicates whether the control automatically generates a column 
  ///   for each property of the bound object.
  /// </summary>
  /// <value> <see langword="true"/> show all properties of the bound business object. </value>
  [Category ("Appearance")]
  [Description ("Indicates whether the control automatically generates a column for each property of the bound object.")]
  [DefaultValue (false)]
  public virtual bool ShowAllProperties
  {
    get { return _showAllProperties; }
    set { _showAllProperties = value; }
  }

  /// <summary>
  ///   Gets or sets a flag that indicates whether to display an icon in front of the first value 
  ///   column.
  /// </summary>
  /// <value> <see langword="true"/> to enable the icon. </value>
  [Category ("Appearance")]
  [Description ("Enables the icon in front of the first value column.")]
  [DefaultValue (true)]
  public virtual bool EnableIcon
  {
    get { return _enableIcon; }
    set { _enableIcon = value; }
  }

  /// <summary>
  ///   Gets or sets a flag that determines whether to to enable cleint side sorting.
  /// </summary>
  /// <value> <see langword="true"/> to enable the sorting buttons. </value>
  [Category ("Behavior")]
  [Description ("Enables the sorting button in front of each value column's header.")]
  [DefaultValue (true)]
  public virtual bool EnableSorting
  {
    get { return _enableSorting; }
    set { _enableSorting = value; }
  }

  protected bool IsClientSideSortingEnabled
  {
    get { return EnableSorting; }
  }

  /// <summary>
  ///   Gets or sets a flag that determines whether to display the sorting order index 
  ///   after each sorting button.
  /// </summary>
  /// <remarks> 
  ///   Only displays the index if more than one column is included in the sorting.
  /// </remarks>
  /// <value> 
  ///   <see langword="NaBooleanEnum.True"/> to show the sorting order index after the button. 
  ///   Defaults to <see cref="NaBooleanEnum.Undefined"/>, which is interpreted as <see langword="true"/>.
  /// </value>
  [Category ("Appearance")]
  [Description ("Enables the sorting order display after each sorting button. Undefined is interpreted as true.")]
  [DefaultValue (NaBooleanEnum.Undefined)]
  public virtual NaBooleanEnum ShowSortingOrder
  {
    get { return _showSortingOrder; }
    set { _showSortingOrder = value; }
  }

  protected virtual bool IsShowSortingOrderEnabled
  {
    get { return ShowSortingOrder != NaBooleanEnum.False; }
  }

  [Category ("Behavior")]
  [Description ("Enables sorting by multiple columns. Undefined is interpreted as true.")]
  [DefaultValue (NaBooleanEnum.Undefined)]
  public virtual NaBooleanEnum EnableMultipleSorting
  {
    get { return _enableMultipleSorting; }
    set 
    {
      _enableMultipleSorting = value; 
      if (_sortingOrder.Count > 1 && ! IsMultipleSortingEnabled)
      {
        BocListSortingOrderEntry entry = (BocListSortingOrderEntry) _sortingOrder[0];
        _sortingOrder.Clear();
        _sortingOrder.Add (entry);
      }
    }
  }

  protected virtual bool IsMultipleSortingEnabled
  {
    get { return EnableMultipleSorting != NaBooleanEnum.False; }
  }

  /// <summary>
  ///   Gets or sets a flag that determines whether to display the options menu.
  /// </summary>
  /// <value> <see langword="true"/> to show the options menu. </value>
  [Category ("Menu")]
  [Description ("Enables the options menu.")]
  [DefaultValue (true)]
  public virtual bool ShowOptionsMenu
  {
    get { return _showOptionsMenu; }
    set { _showOptionsMenu = value; }
  }

  /// <summary>
  ///   Gets or sets a flag that determines whether to display the list menu.
  /// </summary>
  /// <value> <see langword="true"/> to show the list menu. </value>
  [Category ("Menu")]
  [Description ("Enables the list menu.")]
  [DefaultValue (true)]
  public virtual bool ShowListMenu
  {
    get { return _showListMenu; }
    set { _showListMenu = value; }
  }

  /// <summary> Gets or sets a value that determines if the row menu is being displayed. </summary>
  /// <value> <see cref="!:RowMenuDisplay.Undefined"/> is interpreted as <see cref="!:RowMenuDisplay.Disabled"/>. </value>
  [Category ("Menu")]
  [Description ("Enables the row menu. Undefined is interpreted as Disabled.")]
  [DefaultValue (RowMenuDisplay.Undefined)]
  public RowMenuDisplay RowMenuDisplay
  {
    get { return _rowMenuDisplay; }
    set { _rowMenuDisplay = value; }
  }

  /// <summary>
  ///   Gets or sets a value that indicating the row selection mode.
  /// </summary>
  /// <remarks> 
  ///   If row selection is enabled, the control displays a checkbox in front of each row
  ///   and highlights selected data rows.
  /// </remarks>
  [Category ("Behavior")]
  [Description ("Indicates whether row selection is enabled.")]
  [DefaultValue (RowSelection.Undefined)]
  public virtual RowSelection Selection
  {
    get { return _selection; }
    set { _selection = value; }
  }

  protected bool IsSelectionEnabled
  {
    get { return _selection != RowSelection.Undefined && _selection != RowSelection.Disabled; }
  }

  /// <summary> Gets or sets a value that indicating the row index is enabled. </summary>
  /// <value> 
  ///   <see langword="RowIndex.InitialOrder"/> to show the of the initial (unsorted) list and
  ///   <see langword="RowIndex.SortedOrder"/> to show the index based on the current sorting order. 
  ///   Defaults to <see cref="RowIndex.Undefined"/>, which is interpreted as <see langword="RowIndex.Disabled"/>.
  /// </value>
  /// <remarks> If row selection is enabled, the control displays an index in front of each row. </remarks>
  [Category ("Appearance")]
  [Description ("Indicates whether the row index is enabled. Undefined is interpreted as Disabled.")]
  [DefaultValue (RowIndex.Undefined)]
  public virtual RowIndex Index
  {
    get { return _index; }
    set { _index = value; }
  }

  protected bool IsIndexEnabled
  {
    get { return _index != RowIndex.Undefined && _index != RowIndex.Disabled; }
  }

  /// <summary> Gets or sets the offset for the rendered index. </summary>
  /// <value> Defaults to <see cref="NaInt32.Null"/>. </value>
  [Category ("Appearance")]
  [Description ("The offset for the rendered index.")]
  [DefaultValue (typeof(NaInt32), "null")]
  public NaInt32 IndexOffset
  {
    get { return _indexOffset; }
    set { _indexOffset = value; }
  }


  /// <summary> Gets or sets the text that is displayed in the index column's title row. </summary>
  /// <remarks> The value will not be HTML encoded. </remarks>
  [Category ("Appearance")]
  [Description ("The text that is displayed in the index column's title row. The value will not be HTML encoded.")]
  [DefaultValue (null)]
  public string IndexColumnTitle
  {
    get { return _indexColumnTitle; }
    set { _indexColumnTitle = value; }
  }

  protected bool AreDataRowsClickSensitive()
  {
    return    _hasClientScript 
           && ! WcagHelper.Instance.IsWaiConformanceLevelARequired() 
           && IsInternetExplorer55OrHigher();
  }

  /// <summary> The number of rows displayed per page. </summary>
  /// <value> 
  ///   An integer greater than zero to limit the number of rows per page to the specified value,
  ///   or zero, less than zero or <see cref="NaInt32.Null"/> to show all rows.
  /// </value>
  [Category ("Appearance")]
  [Description ("The number of rows displayed per page. Set PageSize to 0 to show all rows.")]
  [DefaultValue (typeof(NaInt32), "null")]
  public virtual NaInt32 PageSize
  {
    get { return _pageSize; }
    set
    {
      if (value.IsNull || value.Value < 0)
        _pageSize = NaInt32.Null;
      else
        _pageSize = value; 
    }
  }

  protected bool IsPagingEnabled
  {
    get 
    { 
      return ! WcagHelper.Instance.IsWaiConformanceLevelARequired() && !_pageSize.IsNull && _pageSize.Value != 0; 
    }
  }

  /// <summary>
  ///   Gets or sets a flag that indicates whether to the show the page count even when there 
  ///   is just one page.
  /// </summary>
  /// <value> 
  ///   <see langword="true"/> to force showing the page info, even if the rows fit onto a single 
  ///   page.
  /// </value>
  [Category ("Behavior")]
  [Description ("Indicates whether to the show the page count even when there is just one page.")]
  [DefaultValue (false)]
  public virtual bool AlwaysShowPageInfo
  {
    get { return _alwaysShowPageInfo; }
    set { _alwaysShowPageInfo = value; }
  }

  /// <summary> Gets or sets the text providing the current page information to the user. </summary>
  /// <remarks> Use {0} for the current page and {1} for the total page count. The value will not be HTML encoded. </remarks>
  [Category ("Appearance")]
  [Description ("The text providing the current page information to the user. Use {0} for the current page and {1} for the total page count. The value will not be HTML encoded.")]
  [DefaultValue (null)]
  public string PageInfo
  {
    get { return _pageInfo; }
    set { _pageInfo = value; }
  }

  /// <summary> Gets or sets the text rendered if the list is empty. </summary>
  /// <remarks> The value will not be HTML encoded. </remarks>
  [Category ("Appearance")]
  [Description ("The text if the list is empty. The value will not be HTML encoded.")]
  [DefaultValue (null)]
  public string EmptyListMessage
  {
    get { return _emptyListMessage; }
    set { _emptyListMessage = value; }
  }

  /// <summary> Gets or sets a flag whether to render the <see cref="EmptyListMessage"/>. </summary>
  [Category ("Appearance")]
  [Description ("A flag that determines whether the EmpryListMessage is rendered.")]
  [DefaultValue (false)]
  public bool ShowEmptyListMessage
  {
    get { return _showEmptyListMessage; }
    set { _showEmptyListMessage = value; }
  }

  /// <summary> Gets or sets a flag that determines whether the client script is enabled. </summary>
  /// <remarks> Effects only advanced scripts used for selcting data rows. </remarks>
  /// <value> <see langref="true"/> to enable the client script. </value>
  [Category ("Behavior")]
  [Description (" True to enable the client script for the pop-up calendar. ")]
  [DefaultValue (true)]
  public bool EnableClientScript
  {
    get { return _enableClientScript; }
    set { _enableClientScript = value; }
  }

  /// <summary> Is raised when a column type <see cref="BocCustomColumnDefinition"/> is clicked on. </summary>
  [Category ("Action")]
  [Description ("Occurs when a custom column is clicked on.")]
  public event BocCustomCellClickEventHandler CustomCellClick
  {
    add { Events.AddHandler (s_customCellClickEvent, value); }
    remove { Events.RemoveHandler (s_customCellClickEvent, value); }
  }

  /// <summary> Is raised when a column with a command of type <see cref="CommandType.Event"/> is clicked. </summary>
  [Category ("Action")]
  [Description ("Occurs when a column with a command of type Event is clicked inside an column.")]
  public event BocListItemCommandClickEventHandler ListItemCommandClick
  {
    add { Events.AddHandler (s_listItemCommandClickEvent, value); }
    remove { Events.RemoveHandler (s_listItemCommandClickEvent, value); }
  }

  /// <summary> Is raised when a menu item with a command of type <see cref="CommandType.Event"/> is clicked. </summary>
  [Category ("Action")]
  [Description ("Is raised when a menu item with a command of type Event is clicked.")]
  public event WebMenuItemClickEventHandler MenuItemClick
  {
    add { Events.AddHandler (s_menuItemClickEvent, value); }
    remove { Events.RemoveHandler (s_menuItemClickEvent, value); }
  }

  /// <summary> Gets or sets the offset between the items in the <c>menu block</c>. </summary>
  /// <remarks> The <see cref="MenuBlockOffset"/> is applied as a <c>margin</c> attribute. </remarks>
  [Category ("Menu")]
  [Description ("The offset between the items in the menu section.")]
  [DefaultValue (typeof (Unit), "")]
  public Unit MenuBlockItemOffset
  {
    get { return _menuBlockItemOffset; }
    set { _menuBlockItemOffset = value; }
  }

  /// <summary> Gets the <see cref="BocMenuItem"/> objects displayed in the <see cref="BocList"/>'s options menu. </summary>
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Category ("Menu")]
  [Description ("The menu items displayed by options menu.")]
  [DefaultValue ((string) null)]
  [Editor (typeof (BocMenuItemCollectionEditor), typeof (System.Drawing.Design.UITypeEditor))]
  public WebMenuItemCollection OptionsMenuItems
  {
    get { return _optionsMenu.MenuItems; }
  }

  /// <summary> Gets the <see cref="BocMenuItem"/> objects displayed in the <see cref="BocList"/>'s menu area. </summary>
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Category ("Menu")]
  [Description ("The menu items displayed in the list's menu area.")]
  [DefaultValue ((string) null)]
  [Editor (typeof (BocMenuItemCollectionEditor), typeof (System.Drawing.Design.UITypeEditor))]
  public WebMenuItemCollection ListMenuItems
  {
    get { return _listMenuItems; }
  }

  /// <summary> Gets or sets the width reserved for the menu block. </summary>
  [Category ("Menu")]
  [Description ("The width reserved for the menu section.")]
  [DefaultValue (typeof (Unit), "")]
  public Unit MenuBlockWidth
  {
    get { return _menuBlockWidth; }
    set { _menuBlockWidth = value; }
  }

  /// <summary> Gets or sets the offset between the <c>list block</c> and the <c>menu block</c>. </summary>
  /// <remarks> The <see cref="MenuBlockOffset"/> is applied as a <c>padding</c> attribute. </remarks>
  [Category ("Menu")]
  [Description ("The offset between the list of values and the menu section.")]
  [DefaultValue (typeof (Unit), "")]
  public Unit MenuBlockOffset
  {
    get { return _menuBlockOffset; }
    set { _menuBlockOffset = value; }
  }

  /// <summary> Gets or sets the list of menu items to be hidden. </summary>
  /// <value> The <see cref="WebMenuItem.ItemID"/> values of the menu items to hide. </value>
  [Category ("Menu")]
  [Description ("The list of menu items to be hidden, identified by their ItemIDs.")]
  [DefaultValue ((string) null)]
  [PersistenceMode (PersistenceMode.Attribute)]
  [TypeConverter (typeof (Rubicon.Web.UI.Design.StringArrayConverter))]
  public string[] HiddenMenuItems 
  {
    get 
    {
      if (_hiddenMenuItems == null)
        return new string[0];
      return _hiddenMenuItems;
    }
    set {_hiddenMenuItems = value;}
  }
  /// <summary>
  ///   Gets or sets a value that indicates whether the control displays a drop down list 
  ///   containing the available column definition sets.
  /// </summary>
  [Category ("Menu")]
  [Description ("Indicates whether the control displays a drop down list containing the available views.")]
  [DefaultValue (true)]
  public bool ShowAvailableViewsList
  {
    get { return _showAvailableViewsList; }
    set { _showAvailableViewsList = value; }
  }

  /// <summary> Gets or sets the text that is rendered as a title for the drop list of additional columns. </summary>
  /// <remarks> The value will not be HTML encoded. </remarks>
  [Category ("Menu")]
  [Description ("The text that is rendered as a title for the list of available views. The value will not be HTML encoded.")]
  [DefaultValue ("")]
  public string AvailableViewsListTitle
  {
    get { return _availableViewsListTitle; }
    set { _availableViewsListTitle = value; }
  }

  /// <summary> Gets or sets the text that is rendered as a label for the <c>options menu</c>. </summary>
  [Category ("Menu")]
  [Description ("The text that is rendered as a label for the options menu.")]
  [DefaultValue ("")]
  public string OptionsTitle
  {
    get { return _optionsTitle; }
    set { _optionsTitle = value; }
  }

  /// <summary> Gets or sets the rendering option for the <c>list menu</c>. </summary>
  [Category ("Menu")]
  [Description ("Defines how the items will be rendered.")]
  [DefaultValue (ListMenuLineBreaks.All)]
  public ListMenuLineBreaks ListMenuLineBreaks
  {
    get { return _listMenuLineBreaks; }
    set { _listMenuLineBreaks = value; }
  }

  /// <summary> Gets or sets the validation error message. </summary>
  /// <value> 
  ///   The error message displayed when validation fails. The default value is an empty <see cref="String"/>.
  ///   In case of the default value, the text is read from the resources for this control.
  /// </value>
  [Description("Validation message displayed if there is an error.")]
  [Category ("Validator")]
  [DefaultValue("")]
  public string ErrorMessage
  {
    get
    { 
      return _errorMessage; 
    }
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

  #region protected virtual string CssClass...
  /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/> itself. </summary>
  /// <remarks> 
  ///   <para> Class: <c>bocList</c>. </para>
  ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassBase
  { get { return "bocList"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/> when it is displayed in read-only mode. </summary>
  /// <remarks> 
  ///   <para> Class: <c>readOnly</c>. </para>
  ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocList.readOnly</c> as a selector. </para>
  /// </remarks>
  protected virtual string CssClassReadOnly
  { get { return "readOnly"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocEnumValue"/> when it is displayed disabled. </summary>
  /// <remarks> 
  ///   <para> Class: <c>disabled</c>. </para>
  ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocEnumValue.disabled</c> as a selector. </para>
  /// </remarks>
  protected virtual string CssClassDisabled
  { get { return "disabled"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s <c>table</c> tag. </summary>
  /// <remarks> Class: <c>bocListTable</c> </remarks>
  protected virtual string CssClassTable
  { get { return "bocListTable"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s <c>thead</c> tag. </summary>
  /// <remarks> Class: <c>bocListTableHead</c> </remarks>
  protected virtual string CssClassTableHead
  { get { return "bocListTableHead"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s <c>tbody</c> tag. </summary>
  /// <remarks> Class: <c>bocListTableBody</c> </remarks>
  protected virtual string CssClassTableBody
  { get { return "bocListTableBody"; } }

  /// <summary> CSS-Class applied to the cells in the <see cref="BocList"/>'s title row. </summary>
  /// <remarks> Class: <c>bocListTitleCell</c> </remarks>
  protected virtual string CssClassTitleCell
  { get { return "bocListTitleCell"; } }

  /// <summary> Gets the CSS-Class applied to the cells in the <see cref="BocList"/>'s selected data. </summary>
  /// <remarks> Class: <c>bocListDataRow</c> </remarks>
  protected virtual string CssClassDataRow
  { get { return "bocListDataRow"; } }

  /// <summary> Gets the CSS-Class applied to the cells in the <see cref="BocList"/>'s selected data rows. </summary>
  /// <remarks> Class: <c>bocListDataRowSelected</c> </remarks>
  protected virtual string CssClassDataRowSelected
  { get { return "bocListDataRowSelected"; } }

  /// <summary> Gets the CSS-Class applied to the cells in the <see cref="BocList"/>'s odd data rows. </summary>
  /// <remarks> Class: <c>bocListDataCellOdd</c> </remarks>
  protected virtual string CssClassDataCellOdd
  { get { return "bocListDataCellOdd"; } }

  /// <summary> Gets the CSS-Class applied to the cells in the <see cref="BocList"/>'s even data rows. </summary>
  /// <remarks> Class: <c>bocListDataCellEven</c> </remarks>
  protected virtual string CssClassDataCellEven
  { get { return "bocListDataCellEven"; } }

  /// <summary> Gets the CSS-Class applied to the cell in the <see cref="BocList"/>'s title row that contains the row index header. </summary>
  /// <remarks> Class: <c>bocListTitleCellIndex</c> </remarks>
  protected virtual string CssClassTitleCellIndex
  { get { return "bocListTitleCellIndex"; } }

  /// <summary> Gets the CSS-Class applied to the cell in the <see cref="BocList"/>'s data rows that contains the row index. </summary>
  /// <remarks> Class: <c>bocListDataCellIndex</c> </remarks>
  protected virtual string CssClassDataCellIndex
  { get { return "bocListDataCellIndex"; } }

  /// <summary> Gets the CSS-Class applied to the content if there is no anchor element. </summary>
  /// <remarks> Class: <c>bocListDataCellContent</c> </remarks>
  protected virtual string CssClassContent
  { get { return "bocListContent"; } }

  /// <summary> Gets the CSS-Class applied to the text providing the sorting order's index. </summary>
  /// <remarks> Class: <c>bocListSortingOrder</c> </remarks>
  protected virtual string CssClassSortingOrder
  { get { return "bocListSortingOrder"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s navigator. </summary>
  /// <remarks> Class: <c>bocListNavigator</c> </remarks>
  protected virtual string CssClassNavigator
  { get { return "bocListNavigator"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s list of additional columns. </summary>
  /// <remarks> Class: <c>bocListAvailableViewsListDropDownList</c> </remarks>
  protected virtual string CssClassAvailableViewsListDropDownList
  { get { return "bocListAvailableViewsListDropDownList"; } }
  
  /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s label for the list of additional columns. </summary>
  /// <remarks> Class: <c>bocListAvailableViewsListLabel</c> </remarks>
  protected virtual string CssClassAvailableViewsListLabel
  { get { return "bocListAvailableViewsListLabel"; } }
  #endregion
}

public enum RowSelection
{ 
  Undefined = -1,
  Disabled = 0,
  SingleCheckBox = 1,
  SingleRadioButton = 2,
  Multiple = 3 
}

public enum RowIndex
{ 
  Undefined,
  Disabled,
  InitialOrder,
  SortedOrder
}

public enum ListMenuLineBreaks
{
  All,
  None,
  BetweenGroups
}

public enum RowMenuDisplay
{
  Undefined,
  /// <summary> No menus will be shown, even if a <see cref="BocDropDownMenuColumnDefinition"/> has been created. </summary>
  Disabled,
  /// <summary> The developer must manually provide a <see cref="BocDropDownMenuColumnDefinition"/>. </summary>
  Manual,
  /// <summary> The <see cref="BocList"/> will automatically create a <see cref="BocDropDownMenuColumnDefinition"/>. </summary>
  Automatic
}

}