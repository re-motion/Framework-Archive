using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Reflection;
using System.Text;
using System.Drawing.Design;
using log4net;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;
using Rubicon.Web.Utilities;
using Rubicon.ObjectBinding.Web.Design;
using Rubicon.Web;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Collections;
using Rubicon.Globalization;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <include file='doc\include\Controls\BocList.xml' path='BocList/Class/*' />
// TODO: see "Doc\Bugs and ToDos.txt"
[Designer (typeof (BocListDesigner))]
[DefaultEvent ("CommandClick")]
[ToolboxItemFilter("System.Web.UI")]
public class BocList:
    BusinessObjectBoundModifiableWebControl, 
    IResourceDispatchTarget, 
    IPostBackEventHandler, 
    IComparer
{
  //  constants
  private const string c_dataRowHiddenFieldIDSuffix = "_Boc_HiddenField_";
  private const string c_dataRowCheckBoxIDSuffix = "_Boc_CheckBox_";
  private const string c_titleRowCheckBoxIDSuffix = "_Boc_CheckBox_SelectAll";
  private const string c_additionalColumnsListIDSuffix = "_Boc_ColumnConfigurationList";
  private const string c_optionsMenuIDSuffix = "_Boc_OptionsMenu";

  private const int c_titleRowIndex = -1;

  /// <summary> Prefix applied to the post back argument of the event type column commands. </summary>
  private const string c_eventListItemCommandPrefix = "ListCommand=";
  /// <summary> Prefix applied to the post back argument of the event type menu commands. </summary>
  private const string c_eventMenuItemPrefix = "MenuItem=";
  /// <summary> Prefix applied to the post back argument of the custom columns. </summary>
  private const string c_customCellEventPrefix = "CustomCell=";

  private const string c_sortAscendingIcon = "SortAscending.gif";
  private const string c_sortDescendingIcon = "SortDescending.gif";
  /// <summary> Prefix applied to the post back argument of the sort buttons. </summary>
  private const string c_sortCommandPrefix = "Sort=";

  #region private const string c_move...Icon
  private const string c_moveFirstIcon = "MoveFirst.gif";
  private const string c_moveLastIcon = "MoveLast.gif";
  private const string c_movePreviousIcon = "MovePrevious.gif";
  private const string c_moveNextIcon = "MoveNext.gif";
  private const string c_moveFirstInactiveIcon = "MoveFirstInactive.gif";
  private const string c_moveLastInactiveIcon = "MoveLastInactive.gif";
  private const string c_movePreviousInactiveIcon = "MovePreviousInactive.gif";
  private const string c_moveNextInactiveIcon = "MoveNextInactive.gif";
  #endregion

  private const string c_bocListScriptUrl = "BocList.js";

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

  private const int c_designModeAdditionalColumnsListWidthInPoints = 40;

  // types
  
  /// <summary> A list of control wide resources. </summary>
  /// <remarks> Resources will be accessed using IResourceManager.GetString (Enum). </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Rubicon.ObjectBinding.Web.Globalization.BocList")]
  protected enum ResourceIdentifier
  {
    PageInfo,
    OptionsTitle,
    AdditionalColumnsTitle
  }

  /// <summary> The possible directions for paging through the list. </summary>
  private enum MoveOption
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

  /// <summary> Represents the sorting direction for an individual column. </summary>
  /// <remarks> Used when evaluating the current or new sorting order as well as to persist it into the view state. </remarks>
  [Serializable]
  private struct SortingOrderEntry
  {
    /// <summary> Gets or sets the index of the column for which the <see cref="Direction"/> is entered. </summary>
    public int ColumnIndex;
    /// <summary> Gets or sets the <see cref="SortingDirection"/> for the column at <see cref="ColumnIndex"/>. </summary>
    public SortingDirection Direction;

    /// <summary> Represents a null <see cref="SortingOrderEntry"/>. </summary>
    public static readonly SortingOrderEntry Empty = new SortingOrderEntry (Int32.MinValue, SortingDirection.None);

    /// <summary> Initializes a new instance. </summary>
    /// <include file='doc\include\Controls\BocList.xml' path='BocList/SortingOrderEntry/Constructor/*' />
    public SortingOrderEntry (int columnIndex, SortingDirection direction)
    {
      ColumnIndex = columnIndex;
      Direction = direction;
    }

    public bool IsEmpty
    {
      get 
      {
        return    this.ColumnIndex == SortingOrderEntry.Empty.ColumnIndex 
               && this.Direction == SortingOrderEntry.Empty.Direction;
      }
    }

    /// <summary>
    ///   Tests whether the specified object is a <see cref="SortingOrderEntry"/> structure 
    ///   and is equivalent to this <see cref="SortingOrderEntry"/> structure.
    /// </summary>
    /// <remarks> Required for identifying the <see cref="SortingOrderEntry"/> in an <see cref="ArrayList"/>. </remarks>
    /// <include file='doc\include\Controls\BocList.xml' path='BocList/SortingOrderEntry/Equals/*' />
    public override bool Equals (object obj)
    {
      if (obj is SortingOrderEntry)
      {
        SortingOrderEntry entry = (SortingOrderEntry) obj;
        return ColumnIndex == entry.ColumnIndex && Direction == entry.Direction;;
      }
      return false;
    }

    /// <summary>
    ///   Returns a hash code for this <see cref="SortingOrderEntry"/> structure.
    /// </summary>
    /// <include file='doc\include\Controls\BocList.xml' path='BocList/SortingOrderEntry/GetHashCode/*' />
    public override int GetHashCode()
    {
      return ColumnIndex.GetHashCode() ^ Direction.GetHashCode();
    }
  }

  // static members
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectReferenceProperty) };
  
  /// <summary> The log4net logger. </summary>
  private static readonly log4net.ILog s_log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

  private static readonly object s_listItemCommandClickEvent = new object();
  private static readonly object s_menuItemClickEvent = new object();
  private static readonly object s_customColummClickEvent = new object();

	// member fields

  /// <summary>
  ///   <see langword="true"/> if <see cref="Value"/> has been changed since last call to
  ///   <see cref="SaveValue"/>.
  /// </summary>
  private bool _isDirty = true;

  /// <summary> The <see cref="DropDownList"/> used to select the column configuration. </summary>
  private DropDownList _additionalColumnsList;
  /// <summary> The <see cref="string"/> that is rendered in front of the <see cref="_additionalColumnsList"/>. </summary>
  private string _additionalColumnsTitle;
  /// <summary> The width applied to the <see cref="_additionalColumnsList"/>. </summary>
  private Unit _additionalColumnsListWidth = Unit.Empty;
  /// <summary> The predefined column defintion sets that the user can choose from at run-time. </summary>
  private BocColumnDefinitionSetCollection _availableColumnDefinitionSets;
  /// <summary> Determines whether to show the drop down list for selecting additional column definitions. </summary>
  private bool _showAdditionalColumnsList = true;
  /// <summary> The current <see cref="BocColumnDefinitionSet"/>. May be set at run time. </summary>
  private BocColumnDefinitionSet _selectedColumnDefinitionSet;
  /// <summary> 
  ///   The zero-based index of the <see cref="BocColumnDefinitionSet"/> selected from 
  ///   <see cref="AvailableColumnDefinitionSets"/>.
  /// </summary>
  private NaInt32 _selectedColumnDefinitionSetIndex = NaInt32.Null;
  bool _isSelectedColumnDefinitionIndexSet = false;

  /// <summary> The <see cref="ImageButton"/> used to navigate to the first page. </summary>
  private ImageButton _moveFirstButton;
  /// <summary> The <see cref="ImageButton"/> used to navigate to the last page. </summary>
  private ImageButton _moveLastButton;
  /// <summary> The <see cref="ImageButton"/> used to navigate to the previous page. </summary>
  private ImageButton _movePreviousButton;
  /// <summary> The <see cref="ImageButton"/> used to navigate to the next page. </summary>
  private ImageButton _moveNextButton;

  /// <summary> The <see cref="IList"/> displayed by the <see cref="BocList"/>. </summary>
  private IList _value = null;

  private NaInt32 _editableRowIndex = NaInt32.Null;

  /// <summary> The user independent column defintions. </summary>
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

  private Unit _menuBlockItemOffset = Unit.Empty;

  private DropDownMenu _optionsMenu;
  private string _optionsTitle;
  /// <summary> The width applied to the <c>menu block</c>. </summary>
  private Unit _menuBlockWidth = Unit.Empty;
  /// <summary> The offset between the  <c>list block</c> and the <c>menu block</c>. </summary>
  private Unit _menuBlockOffset = Unit.Empty;
  private BocMenuItemCollection _optionsMenuItems;
  /// <summary> Contains the <see cref="BocMenuItem"/> objects during the handling of the post back events. </summary>
  private BocMenuItem[] _optionsMenuItemsPostBackEventHandlingPhase;
  /// <summary> Contains the <see cref="BocMenuItem"/> objects during the rendering phase. </summary>
  private BocMenuItem[] _optionsMenuItemsRenderPhase;

  private BocMenuItemCollection _listMenuItems;
  /// <summary> Contains the <see cref="BocMenuItem"/> objects during the handling of the post back events. </summary>
  private BocMenuItem[] _listMenuItemsPostBackEventHandlingPhase;
  /// <summary> Contains the <see cref="BocMenuItem"/> objects during the rendering phase. </summary>
  private BocMenuItem[] _listMenuItemsRenderPhase;
  private ListMenuLineBreaks _listMenuLineBreaks = ListMenuLineBreaks.All;
 
  /// <summary> Determines wheter an empty list will still render its headers. </summary>
  private bool _showEmptyList = true;

  /// <summary> Determines whether to generate columns for all properties. </summary>
  private bool _showAllProperties;
  
  /// <summary> Determines whether to show the icons for each entry in <see cref="Value"/>. </summary>
  private bool _enableIcon = true;
  
  /// <summary> Determines whether to show the sort buttons. </summary>
  private bool _enableSorting = true;
  /// <summary> Determines whether to show the sorting order after the sorting button. </summary>
  private bool _showSortingOrder = false;
  /// <summary> 
  ///   Contains <see cref="SortingOrderEntry"/> objects in the order of the buttons pressed.
  /// </summary>
  private ArrayList _sortingOrder = new ArrayList();

  /// <summary> Determines whether the options menu is shown. </summary>
  private bool _showOptionsMenu = true;

  /// <summary> Determines whether to enable the selecting of the data rows. </summary>
  private RowSelection _selection = RowSelection.Disabled;
  /// <summary> 
  ///   Contains the checked state for each of the selection checkBoxes in the <see cref="BocList"/>.
  ///   Hashtable&lt;int rowIndex, bool isChecked&gt; 
  /// </summary>
  private Hashtable _checkBoxCheckedState = new Hashtable();

  /// <summary> Null, 0: show all objects, > 0: show n objects per page. </summary>
  private NaInt32 _pageSize = NaInt32.Null; 
  /// <summary>
  ///   Show page info ("page 1 of n") and links always (true),
  ///   or only if there is more than 1 page (false)
  /// </summary>
  private bool _alwaysShowPageInfo = false; 
  /// <summary> The text providing the current page information to the user. </summary>
  private string _pageInfo;
  /// <summary> 
  ///   The navigation bar command that caused the post back. 
  ///   <see cref="MoveOption.Undefined"/> unless the navigation bar caused a post back.
  /// </summary>
  private MoveOption _move = MoveOption.Undefined;
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

  // construction and disposing

  /// <summary> Initializes a new instance of the <see cref="BocList"/> class. </summary>
	public BocList()
	{
    _moveFirstButton = new ImageButton();
    _moveLastButton =  new ImageButton();
    _movePreviousButton = new ImageButton();
    _moveNextButton = new ImageButton();
    _additionalColumnsList = new DropDownList();
    _optionsMenu = new DropDownMenu (this);
    _listMenuItems = new BocMenuItemCollection (this);
    _optionsMenuItems = new BocMenuItemCollection (this);
    _fixedColumns = new BocColumnDefinitionCollection (this);
    _availableColumnDefinitionSets = new BocColumnDefinitionSetCollection (this);
  }

	// methods and properties

  protected override void CreateChildControls()
  {
    _optionsMenu.ID = ID + c_optionsMenuIDSuffix;
    _optionsMenu.GetSelectionCount = "function() { return BocList_GetSelectionCount ('" + ClientID + "'); }";
    Controls.Add (_optionsMenu);

    Controls.Add (_moveFirstButton);
    Controls.Add (_moveLastButton);
    Controls.Add (_movePreviousButton);
    Controls.Add (_moveNextButton);

    _additionalColumnsList.ID = this.ID + c_additionalColumnsListIDSuffix;
    _additionalColumnsList.EnableViewState = true;
    _additionalColumnsList.AutoPostBack = true;
    _additionalColumnsList.SelectedIndexChanged += new EventHandler(AdditionalColumnsList_SelectedIndexChanged);
    Controls.Add (_additionalColumnsList);
  }

  /// <summary> Calls the parent's <c>OnInit</c> method and initializes this control's sub-controls. </summary>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);

    _optionsMenu.EventCommandClick += new WebMenuItemClickEventHandler (OptionsMenu_EventCommandClick);
    _optionsMenu.WxeFunctionCommandClick += new WebMenuItemClickEventHandler (OptionsMenu_WxeFunctionCommandClick);
    _moveFirstButton.Click += new ImageClickEventHandler (MoveFirstButton_Click);
    _moveLastButton.Click += new ImageClickEventHandler (MoveLastButton_Click);
    _movePreviousButton.Click += new ImageClickEventHandler (MovePreviousButton_Click);
    _moveNextButton.Click += new ImageClickEventHandler (MoveNextButton_Click);

    Binding.BindingChanged += new EventHandler (Binding_BindingChanged);

    if (! IsPostBack)
      PopulateAdditionalColumnsList();
    _availableColumnDefinitionSets.CollectionChanged += new CollectionChangeEventHandler(AvailableColumnDefinitionSets_CollectionChanged);
    
    if (IsPostBack && Page != null)
    {
      string dataRowCheckBoxFilter = ClientID + c_dataRowCheckBoxIDSuffix;
      string titleRowCheckBoxFilter = ClientID + c_titleRowCheckBoxIDSuffix;

      NameValueCollection formVariables = PageUtility.GetRequestCollection(Page);
      if (formVariables != null)
      {
        for (int i = 0; i < formVariables.Count; i++)
        {
          string key = formVariables.Keys[i];

          bool isDataRowCheckBox = key.StartsWith (dataRowCheckBoxFilter);
          bool isTitleRowCheckBox = (key == titleRowCheckBoxFilter);
          if (isDataRowCheckBox || isTitleRowCheckBox)
          {
            if (   _selection == RowSelection.Single 
                && (_checkBoxCheckedState.Count > 1  || isTitleRowCheckBox))
            {
              continue;
            }
            int rowIndex = int.Parse (formVariables[i]);
            _checkBoxCheckedState[rowIndex] = true; 
          }
        }
      }
    }
  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad (e);
    
    _optionsMenu.MenuItems.Clear();
    _optionsMenu.MenuItems.AddRange (EnsureOptionsMenuItemsForPreviousLifeCycleGot());

    EnsureRowEditModeRestored();
  }

  /// <summary> Implements interface <see cref="IPostBackEventHandler"/>. </summary>
  /// <param name="eventArgument"> &lt;prefix&gt;=&lt;value&gt; </param>
  void IPostBackEventHandler.RaisePostBackEvent (string eventArgument)
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
    else
      throw new ArgumentException ("Argument 'eventArgument' has unknown prefix: '" + eventArgument + "'.");
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
      throw new ArgumentException ("First part of argument 'eventArgument' must be an integer. Expected format: '<column-index>,<list-index>'.");
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
      throw new ArgumentException ("Second part of argument 'eventArgument' must be an integer. Expected format: <column-index>,<list-index>'.");
    }

    BocColumnDefinition[] columns = EnsureColumnsForPreviousLifeCycleGot();

    if (columnIndex >= columns.Length)
      throw new ArgumentOutOfRangeException ("Column index of argument 'eventargument' was out of the range of valid values. Index must be less than the number of displayed columns.'");

    BocCommandEnabledColumnDefinition column = (BocCommandEnabledColumnDefinition) columns[columnIndex];
    if (column.Command == null)
      throw new ArgumentOutOfRangeException ("The BocList '" + ID + "' does not have a command inside column " + columnIndex + ".");
    BocListItemCommand command = column.Command;

    switch (command.Type)
    {
      case CommandType.Event:
      {
        OnListItemCommandClick (column, listIndex, (IBusinessObject) Value[listIndex]);
        break;
      }
      case CommandType.WxeFunction:
      {
        command.ExecuteWxeFunction ((IWxePage) Page, listIndex, (IBusinessObject) Value[listIndex]);
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

    BocMenuItem[] menuItems = EnsureListMenuItemsForPreviousLifeCycleGot();
    if (index >= menuItems.Length)
      throw new ArgumentOutOfRangeException ("Index of argument 'eventargument' was out of the range of valid values. Index must be less than the number of displayed menu items.'");

    BocMenuItem menuItem = menuItems[index];
    if (menuItem.Command == null)
      throw new ArgumentOutOfRangeException ("The BocList '" + ID + "' does not have a command associated with list menu item " + index + ".");

    BocMenuItemCommand command = (BocMenuItemCommand) menuItem.Command;
    switch (command.Type)
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

    SortingOrderEntry sortingOrderEntry = SortingOrderEntry.Empty;
    foreach (SortingOrderEntry currentEntry in _sortingOrder)
    {
      if (currentEntry.ColumnIndex == columnIndex)
      {
        sortingOrderEntry = currentEntry;
        break;
      }
    }

    //  Cycle: Ascending -> Descending -> None -> Ascending
    if (! sortingOrderEntry.IsEmpty)
    {
      _sortingOrder.Remove (sortingOrderEntry);
      switch (sortingOrderEntry.Direction)
      {
        case SortingDirection.Ascending:
        {
          sortingOrderEntry.Direction = SortingDirection.Descending;
          break;
        }
        case SortingDirection.Descending:
        {
          sortingOrderEntry = SortingOrderEntry.Empty;
          break;
        }
        case SortingDirection.None:
        {
          sortingOrderEntry.Direction = SortingDirection.Ascending;
          break;
        }
      }
    }
    else
    {
      sortingOrderEntry = new SortingOrderEntry (columnIndex, SortingDirection.Ascending);
    }

    if (! sortingOrderEntry.IsEmpty)
      _sortingOrder.Add (sortingOrderEntry);
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

  /// <summary> Fires the <see cref="ListItemCommandClick"/> event. </summary>
  /// <include file='doc\include\Controls\BocList.xml' path='BocList/OnListItemCommandClick/*' />
  protected virtual void OnListItemCommandClick (
      BocCommandEnabledColumnDefinition column, 
      int listIndex, 
      IBusinessObject businessObject)
  {
    if (column != null && column.Command != null)
      column.Command.OnClick (column, listIndex, businessObject);
    BocListItemCommandClickEventHandler commandClickHandler = 
        (BocListItemCommandClickEventHandler) Events[s_listItemCommandClickEvent];
    if (commandClickHandler != null)
    {
      BocListItemCommandClickEventArgs e = new BocListItemCommandClickEventArgs (column, listIndex, businessObject);
      commandClickHandler (this, e);
    }
  }

  /// <summary> 
  ///   Event handler for the <see cref="DropDownMenu.EventCommandClick"/> of the <see cref="_optionsMenu"/>.
  /// </summary>
  private void OptionsMenu_EventCommandClick (object sender, WebMenuItemClickEventArgs e)
  {
    OnMenuItemEventCommandClick ((BocMenuItem) e.Item);
  }

  /// <summary> Fires the <see cref="MenuItemClick"/> event. </summary>
  /// <include file='doc\include\Controls\BocList.xml' path='BocList/OnMenuItemEventCommandClick/*' />
  protected virtual void OnMenuItemEventCommandClick (BocMenuItem menuItem)
  {
    if (menuItem != null && menuItem.Command != null)
      ((BocMenuItemCommand) menuItem.Command).OnClick (menuItem);
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
    OnMenuItemWxeFunctionCommandClick ((BocMenuItem) e.Item);
  }

  /// <summary> Handles the click to a WXE function command. </summary>
  /// <include file='doc\include\Controls\BocList.xml' path='BocList/OnMenuItemWxeFunctionCommandClick/*' />
  protected virtual void OnMenuItemWxeFunctionCommandClick (BocMenuItem menuItem)
  {
    if (menuItem != null && menuItem.Command != null)
    {
      BocMenuItemCommand command = (BocMenuItemCommand) menuItem.Command;
      command.ExecuteWxeFunction ((IWxePage) Page, GetSelectedRows(), GetSelectedBusinessObjects());
    }
  }

  protected virtual void OnCustomCellClick (
      BocCustomColumnDefinition column, 
      IBusinessObject businessObject,
      string argument)
  {
    column.CustomCell.OnClick (this, businessObject, column, argument);
    BocCustomColumnClickEventHandler clickHandler = 
        (BocCustomColumnClickEventHandler) Events[s_customColummClickEvent];
    if (clickHandler != null)
    {
      BocCustomColumnClickEventArgs e = new BocCustomColumnClickEventArgs (column, businessObject, argument);
      clickHandler (this, e);
    }
  }

  /// <summary> Overrides the parent's <c>OnPreRender</c> method. </summary>
  /// <remarks> Calculates the page count depending on an optional move command, and registeres the client scripts. </remarks>
  /// <param name="e"> The <see cref="EventArgs"/>. </param>
  protected override void OnPreRender(EventArgs e)
  {
    DetermineClientScriptLevel();

    if (_pageSize.IsNull || Value == null)
    {
      _pageCount = 1;
    }
    else
    {
      _currentPage = _currentRow / _pageSize.Value;
      _pageCount = (int) Math.Round ((double)Value.Count / _pageSize.Value + 0.5, 0);

      switch (_move)
      {
        case MoveOption.First:
        {
          _currentPage = 0;
          _currentRow = 0;
          break;
        }
        case MoveOption.Last:
        {
          _currentPage = _pageCount - 1;
          _currentRow = _currentPage * _pageSize.Value;
          break;
        }
        case MoveOption.Previous:
        {
          _currentPage--;
          _currentRow = _currentPage * _pageSize.Value;
          break;
        }
        case MoveOption.Next:
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

      if (_move != MoveOption.Undefined)
        _checkBoxCheckedState.Clear();
    }

    string key;

    if (_hasClientScript)
    {
      //  Include script file
      key = typeof (BocList).FullName + "_Script";
      if (! HtmlHeadAppender.Current.IsRegistered (key))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            this, Context, typeof (BocList), ResourceType.Html, c_bocListScriptUrl);
        HtmlHeadAppender.Current.RegisterJavaScriptInclude (key, scriptUrl);
      }

      //  Startup script initalizing the global values of the script.
      key = typeof (BocList).FullName+ "_Startup";
      if (! Page.IsStartupScriptRegistered (key))
      {
        string script = string.Format (
            "BocList_InitializeGlobals ('{0}', '{1}', '{2}', '{3}');",
            CssClassDataCellOdd,
            CssClassDataCellEven,
            CssClassDataCellOddSelected,
            CssClassDataCellEvenSelected);
        PageUtility.RegisterStartupScriptBlock (Page, key, script);
      }
    }

    key = typeof (BocList).FullName + "_Style";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      string url = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (BocList), ResourceType.Html, "BocList.css");
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, url);
    }

    base.OnPreRender (e);
  }

  /// <summary> Overrides the parent's <c>Render</c> method. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object that receives the server control content. </param>
  protected override void Render (HtmlTextWriter writer)
  {
    if (Page != null)
      Page.VerifyRenderingInServerForm(this);

    BocColumnDefinition[] renderColumns = EnsureColumnsGot (true);
    EnsureOptionsMenuItemsGot (true);
    EnsureListMenuItemsGot (true);
    
    if (IsDesignMode)
    {
      //  Normally set in OnPreRender, which is omitted during design-time
      if (_pageCount == 0)
        _pageCount = 1;
    }
 
    //  Render control opening tag
    writer.AddAttribute (HtmlTextWriterAttribute.Id, ClientID);
    if (! Width.IsEmpty)
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Width.ToString());
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClass);
    writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Control-Tag

    bool isInternetExplorer501AndHigher = true;
    bool isCss21Compatible = false;
    if (! IsDesignMode)
    {
      bool isVersionGreaterOrEqual501 = 
              Context.Request.Browser.MajorVersion >= 6
          ||    Context.Request.Browser.MajorVersion == 5 
              && Context.Request.Browser.MinorVersion >= 0.01;
      isInternetExplorer501AndHigher = 
          Context.Request.Browser.Browser == "IE" && isVersionGreaterOrEqual501;
      bool isOperaGreaterOrEqual7 = 
             Context.Request.Browser.Browser == "Opera"
          && Context.Request.Browser.MajorVersion >= 7;
      bool isNetscapeCompatibleGreaterOrEqual5 = 
             Context.Request.Browser.Browser == "Netscape"
          && Context.Request.Browser.MajorVersion >= 5;
      isCss21Compatible = isOperaGreaterOrEqual7 || isNetscapeCompatibleGreaterOrEqual5;
    }

    if (isInternetExplorer501AndHigher)
      RenderTableAndMenuInternetExplorer501Compatible (writer);
    else if (isCss21Compatible && ! (writer is Html32TextWriter))
      RenderTableAndMenuCss21Compatible (writer);
    else
      RenderTableAndMenuLegacyBrowser (writer);

    //  The navigation block
    if (_alwaysShowPageInfo || _pageCount > 1)
      RenderNavigator (writer);

    writer.RenderEndTag(); // End control-tag
  }

  private void RenderTableAndMenuInternetExplorer501Compatible (HtmlTextWriter writer)
  {
    RenderTableAndMenuLegacyBrowser (writer);
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

  private void RenderTableAndMenuCss21Compatible (HtmlTextWriter writer)
  {
    writer.AddStyleAttribute ("display", "table");
    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
    writer.RenderBeginTag (HtmlTextWriterTag.Div);  //  Begin table
    writer.AddStyleAttribute ("display", "table-row");
    writer.RenderBeginTag (HtmlTextWriterTag.Div);  //  Begin table-row

    writer.AddStyleAttribute ("display", "table-cell"); 
    writer.AddStyleAttribute ("vertical-align", "top");
    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "auto");
    writer.RenderBeginTag (HtmlTextWriterTag.Div); //  Begin table-cell
    RenderTableBlock (writer);
    writer.RenderEndTag();  //  End table-cell

    if (HasMenuBlock)
    {
      writer.AddStyleAttribute ("display", "table-cell");
      writer.AddStyleAttribute ("vertical-align", "top");
      string menuBlockWidth = c_defaultMenuBlockWidth;
      if (! _menuBlockWidth.IsEmpty)
        menuBlockWidth = _menuBlockWidth.ToString();
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, menuBlockWidth);       
      string menuBlockOffset = c_defaultMenuBlockOffset;
      if (! _menuBlockOffset.IsEmpty)
        menuBlockOffset = _menuBlockWidth.ToString();
      writer.AddStyleAttribute ("padding-left", menuBlockOffset);
      writer.RenderBeginTag (HtmlTextWriterTag.Div);  //  Begin table-cell
      RenderMenuBlock (writer);
      writer.RenderEndTag();  //  End table-cell
    }

    writer.RenderEndTag();  //  End table-row
    writer.RenderEndTag();  //  End table
  }

  /// <remarks> Use display:table, display:table-row, ... for opera and mozilla/firefox </remarks>
  private void RenderTableAndMenuLegacyBrowser (HtmlTextWriter writer)
  {
    //  Render list block / menu block
    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
    writer.RenderBeginTag (HtmlTextWriterTag.Table);

    //  Two columns
    writer.RenderBeginTag (HtmlTextWriterTag.Colgroup);

    //  Left: list block
    writer.RenderBeginTag (HtmlTextWriterTag.Col);
    writer.RenderEndTag();

    //  Right: menu block
    string menuBlockWidth = c_defaultMenuBlockWidth;
    if (! _menuBlockWidth.IsEmpty)
      menuBlockWidth = _menuBlockWidth.ToString();
    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, menuBlockWidth);
    string menuBlockOffset = c_defaultMenuBlockOffset;
    if (! _menuBlockOffset.IsEmpty)
      menuBlockOffset = _menuBlockOffset.ToString();
    writer.AddStyleAttribute ("padding-left", menuBlockOffset);
    writer.RenderBeginTag (HtmlTextWriterTag.Col);
    writer.RenderEndTag();

    writer.RenderEndTag();  //  End ColGroup

    writer.RenderBeginTag (HtmlTextWriterTag.Tr);
    
    //  List Block
    writer.AddStyleAttribute ("vertical-align", "top");
    writer.RenderBeginTag (HtmlTextWriterTag.Td);
    RenderTableBlock (writer);
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

  private bool HasMenuBlock
  {
    get { return HasAdditionalColumnsList || HasOptionsMenu || HasListMenu; }
  }

  private bool HasAdditionalColumnsList
  {
    get { return _showAdditionalColumnsList && _additionalColumnsList.Items.Count > 1; }
  }

  private bool HasOptionsMenu
  {
    get { return _showOptionsMenu && EnsureOptionsMenuItemsGot().Length > 0; }
  }

  private bool HasListMenu
  {
    get { return EnsureListMenuItemsGot().Length > 0; }
  }

  /// <summary> Renders the menu block of the control. </summary>
  /// <remarks> Contains the drop down list for selcting a column configuration and the options menu.  </remarks>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object that receives the server control content. </param>
  private void RenderMenuBlock (HtmlTextWriter writer)
  {
    string menuBlockItemOffset = c_defaultMenuBlockItemOffset;
    if (! _menuBlockItemOffset.IsEmpty)
      menuBlockItemOffset = _menuBlockItemOffset.ToString();

    if (HasAdditionalColumnsList)
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      writer.AddStyleAttribute ("margin-bottom", menuBlockItemOffset);
      writer.RenderBeginTag (HtmlTextWriterTag.Div);
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassAdditionalColumnsListLabel);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);
      if (StringUtility.IsNullOrEmpty (_additionalColumnsTitle))
        writer.Write (GetResourceManager().GetString (ResourceIdentifier.AdditionalColumnsTitle));
      else
        writer.Write (_additionalColumnsTitle);
      writer.RenderEndTag();
      writer.Write (c_whiteSpace);
      if (IsDesignMode)
        _additionalColumnsList.Width = Unit.Point (c_designModeAdditionalColumnsListWidthInPoints);
      _additionalColumnsList.Enabled = ! IsRowEditMode;
      _additionalColumnsList.CssClass = CssClassAdditionalColumnsListDropDownList;
      _additionalColumnsList.RenderControl (writer);
      writer.RenderEndTag();
    }

    if (HasOptionsMenu)
    {
      _optionsMenu.MenuItems.Clear();
      _optionsMenu.MenuItems.AddRange (EnsureOptionsMenuItemsGot());
      if (StringUtility.IsNullOrEmpty (_optionsTitle))
        _optionsMenu.TitleText = GetResourceManager().GetString (ResourceIdentifier.OptionsTitle);
      else
        _optionsMenu.TitleText = _optionsTitle;
      _optionsMenu.Style.Add ("margin-bottom", menuBlockItemOffset);
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

  //  TODO: Move ListMenu the extra control "ContentMenu"
  private void RenderListMenu (HtmlTextWriter writer, string menuID)
  {
    ArrayList listMenuItems = new ArrayList (EnsureListMenuItemsGot());
    WebMenuItem[] groupedListMenuItems = WebMenuItemCollection.GroupMenuItems (
        (WebMenuItem[]) listMenuItems.ToArray (typeof (WebMenuItem)), 
        false);

    writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
    writer.RenderBeginTag (HtmlTextWriterTag.Table);
    for (int idxItems = 0; idxItems < groupedListMenuItems.Length; idxItems++)
    {
      WebMenuItem currentItem = groupedListMenuItems[idxItems];

      bool isFirstItem = idxItems == 0;
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
      RenderListMenuItem (writer, currentItem, menuID, listMenuItems.IndexOf (currentItem));
      if (hasAlwaysLineBreaks || isLastCategoryItem || (hasNoLineBreaks && isLastItem))
      {
        writer.RenderEndTag();
        writer.RenderEndTag();
      }
    }
    writer.RenderEndTag();

    string key = UniqueID + "_ListMenuItems";
    if (! Page.IsStartupScriptRegistered (key))
    {
      StringBuilder script = new StringBuilder();
      script.AppendFormat ("BocList_AddMenuInfo (document.getElementById ('{0}'), \r\n\t", ClientID);
      script.AppendFormat ("new ContentMenu_MenuInfo ('{0}', new Array (\r\n", menuID);
      bool isFirstItem = true;

      for (int idxItems = 0; idxItems < groupedListMenuItems.Length; idxItems++)
      {
        WebMenuItem currentItem = groupedListMenuItems[idxItems];
        if (isFirstItem)
          isFirstItem = false;
        else
          script.AppendFormat (",\r\n");
        AppendListMenuItem (script, currentItem, menuID, listMenuItems.IndexOf (currentItem));
      }
      script.Append (" )"); // Close Array
      script.Append (" )"); // Close new MenuInfo
      script.Append (" );\r\n"); // Close AddMenuInfo

      script.AppendFormat (
          "BocList_UpdateListMenu ( document.getElementById ('{0}'), document.getElementById ('{1}'));",
          ClientID, menuID);
      PageUtility.RegisterStartupScriptBlock (Page, key, script.ToString());
    }
  }

  private void AppendListMenuItem (StringBuilder stringBuilder, WebMenuItem menuItem, string menuID, int menuItemIndex)
  {
    bool isReadOnly = IsReadOnly;
    string href = "null";
    string target = "null";
    if (menuItem.Command != null)
    {
      bool isActive =    menuItem.Command.Show == CommandShow.Always
                      || isReadOnly && menuItem.Command.Show == CommandShow.ReadOnly
                      || ! isReadOnly && menuItem.Command.Show == CommandShow.EditMode;

      bool isCommandEnabled = isActive && menuItem.Command.Type != CommandType.None;
      if (isCommandEnabled)
      {    
        bool isPostBackCommand =    menuItem.Command.Type == CommandType.Event 
                                || menuItem.Command.Type == CommandType.WxeFunction;
        if (isPostBackCommand)
        {
          string argument = c_eventMenuItemPrefix + menuItemIndex.ToString();
          href = Page.GetPostBackClientHyperlink (this, argument);
          //  HACK: EscapeJavaScript will be moved to extra class 
          href = DropDownMenu.EscapeJavaScript (href);
          href = "'" + href + "'";
        }
        else if (menuItem.Command.Type == CommandType.Href)
        {
          href = "'" + menuItem.Command.HrefCommand.FormatHref (menuItemIndex.ToString(), menuItem.ItemID) + "'";
          target = "'" + menuItem.Command.HrefCommand.Target + "'";
        }
      }
    }

    bool showIcon = menuItem.Style == WebMenuItemStyle.Icon ||  menuItem.Style == WebMenuItemStyle.IconAndText;
    bool showText = menuItem.Style == WebMenuItemStyle.Text ||  menuItem.Style == WebMenuItemStyle.IconAndText;
    string icon = "null";
    if (showIcon && ! StringUtility.IsNullOrEmpty (menuItem.Icon))
      icon =  "'" + menuItem.Icon + "'";
    string disabledIcon = "null";
    if (showIcon && ! StringUtility.IsNullOrEmpty (menuItem.DisabledIcon))
      disabledIcon =  "'" + menuItem.DisabledIcon + "'";
    string text = showText ? "'" +  menuItem.Text + "'" : "null";

    stringBuilder.AppendFormat (
        "\t\tnew ContentMenu_MenuItemInfo ('{0}', '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8})",
        menuID + "_" + menuItemIndex.ToString(), 
        menuItem.Category, 
        text, 
        icon, 
        disabledIcon, 
        (int) menuItem.RequiredSelection,
        menuItem.IsDisabled ? "true" : "false",
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
    if (showIcon && ! StringUtility.IsNullOrEmpty (menuItem.Icon))
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Src, menuItem.Icon);
      writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
      writer.AddStyleAttribute ("vertical-align", "middle");
      writer.RenderBeginTag (HtmlTextWriterTag.Img);
      writer.RenderEndTag();
      if (showText)
        writer.Write (c_whiteSpace);
    }
    if (showText)
      writer.Write (menuItem.Text);
    writer.RenderEndTag();
    writer.RenderEndTag();
  }

  /// <summary> Renders the list of values as an <c>table</c>. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object that receives the server control content. </param>
  private void RenderTableBlock (HtmlTextWriter writer)
  {
    bool isEmpty = Value == null || Value.Count == 0;
    
    if (isEmpty && ! _showEmptyList)
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
    else
    {
      //  The table non-data sections
      RenderTableOpeningTag (writer);
      RenderColGroup (writer);
      RenderColumnTitlesRow (writer);

      //  The tables data-section
      int firstRow = 0;
      int totalRowCount = (Value != null) ? Value.Count : 0;
      int rowCountWithOffset = totalRowCount;

      if (!_pageSize.IsNull && Value != null)
      {      
        firstRow = _currentPage * _pageSize.Value;
        rowCountWithOffset = firstRow + _pageSize.Value;
        //  Check row count on last page
        rowCountWithOffset = (rowCountWithOffset < Value.Count) ? rowCountWithOffset : Value.Count;
      }

      if (! isEmpty)
      {
        bool isOddRow = true;

        ArrayList rows = new ArrayList (Value.Count);
        for (int idxRows = 0; idxRows < Value.Count; idxRows++)
          rows.Add (new Pair (idxRows, Value[idxRows]));

        if (EnableSorting)
          rows.Sort (this);

        for (int idxAbsoluteRows = firstRow, idxRelativeRows = 0; 
            idxAbsoluteRows < rowCountWithOffset; 
            idxAbsoluteRows++, idxRelativeRows++)
        {
          Pair rowPair = (Pair)rows[idxAbsoluteRows];
          if (rowPair.Second == null)
            throw new NullReferenceException ("Null item found in IList 'Value' of BocList " + ID + ".");
          int originalRowIndex = (int) rowPair.First;
          IBusinessObject businessObject = rowPair.Second as IBusinessObject;
          if (businessObject == null)
            throw new InvalidCastException ("List item " + originalRowIndex + " in IList 'Value' of BocList " + ID + " is not of type IBusinessObject.");
          RenderDataRow (writer, businessObject, idxRelativeRows, originalRowIndex, isOddRow);
          isOddRow = !isOddRow;
        }
      }
      //  Close the table
      RenderTableClosingTag (writer);
    }

    if (_hasClientScript && IsSelectionEnabled)
    {
      //  Render the init script for the client side selection handling
      int count = 0;
      if (! _pageSize.IsNull)
        count = _pageSize.Value;
      else if (Value != null)
        count = Value.Count;

      string script = "<script type=\"text/javascript\">\r\n<!--\r\n"
          + "BocList_InitializeList ("
          + "document.getElementById ('" + ClientID + "'), '"
          + ClientID + c_dataRowCheckBoxIDSuffix + "', "
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

    if (! Width.IsEmpty)
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Width.ToString());
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassNavigator);
    writer.AddStyleAttribute ("position", "relative");
    writer.RenderBeginTag (HtmlTextWriterTag.Div);

    //  Page info
    if (StringUtility.IsNullOrEmpty (_pageInfo))
      _pageInfo = GetResourceManager().GetString (ResourceIdentifier.PageInfo);

    writer.Write (_pageInfo, _currentPage + 1, _pageCount);
    writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);
    
    string imageUrl = null;

    //  Move to first page button
    if (isFirstPage || IsRowEditMode)
      imageUrl = c_moveFirstInactiveIcon;
    else
      imageUrl = c_moveFirstIcon;      
    imageUrl = ResourceUrlResolver.GetResourceUrl (this, Context, typeof (BocList), ResourceType.Image, imageUrl);
    if (isFirstPage || IsRowEditMode)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Src, imageUrl);
      writer.RenderBeginTag (HtmlTextWriterTag.Img);
      writer.RenderEndTag();
    }
    else
    {
      _moveFirstButton.ImageUrl = imageUrl;
      _moveFirstButton.RenderControl (writer);
    }
    writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);

    //  Move to previous page button
    if (isFirstPage || IsRowEditMode)
      imageUrl = c_movePreviousInactiveIcon;
    else
      imageUrl = c_movePreviousIcon;      
    imageUrl = ResourceUrlResolver.GetResourceUrl (this, Context, typeof (BocList), ResourceType.Image, imageUrl);
    if (isFirstPage || IsRowEditMode)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Src, imageUrl);
      writer.RenderBeginTag (HtmlTextWriterTag.Img);
      writer.RenderEndTag();
    }
    else
    {
      _movePreviousButton.ImageUrl = imageUrl;
      _movePreviousButton.RenderControl (writer);
    }
    writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);

    //  Move to next page button
    if (isLastPage || IsRowEditMode)
      imageUrl = c_moveNextInactiveIcon;
    else
      imageUrl = c_moveNextIcon;      
    imageUrl = ResourceUrlResolver.GetResourceUrl (this, Context, typeof (BocList), ResourceType.Image, imageUrl);
    if (isLastPage || IsRowEditMode)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Src, imageUrl);
      writer.RenderBeginTag (HtmlTextWriterTag.Img);
      writer.RenderEndTag();
    }
    else
    {
      _moveNextButton.ImageUrl = imageUrl;
      _moveNextButton.RenderControl (writer);
    }
    writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);

    //  Move to last page button
    if (isLastPage || IsRowEditMode)
      imageUrl = c_moveLastInactiveIcon;
    else
      imageUrl = c_moveLastIcon;     
    imageUrl = ResourceUrlResolver.GetResourceUrl (this, Context, typeof (BocList), ResourceType.Image, imageUrl);
    if (isLastPage || IsRowEditMode)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Src, imageUrl);
      writer.RenderBeginTag (HtmlTextWriterTag.Img);
      writer.RenderEndTag();
    }
    else
    {
      _moveLastButton.ImageUrl = imageUrl;
      _moveLastButton.RenderControl (writer);
    }

    writer.RenderEndTag();
  }

  /// <summary> Renderes the opening tag of the table. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object that receives the server control content. </param>
  private void RenderTableOpeningTag (HtmlTextWriter writer)
  {
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
    writer.RenderEndTag();
  }

  /// <summary> Renderes the column group, which provides the table's column layout. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object that receives the server control content. </param>
  private void RenderColGroup (HtmlTextWriter writer)
  {
    BocColumnDefinition[] renderColumns = EnsureColumnsGot();

    writer.RenderBeginTag (HtmlTextWriterTag.Colgroup);

    if (IsSelectionEnabled)
    {
      //  1% would lead to automatic resizing if all widths don't add up to 100%
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Unit.Percentage(0).ToString());
      writer.RenderBeginTag (HtmlTextWriterTag.Col);
      writer.RenderEndTag();
    }

    //bool isFirstColumnUndefinedWidth = true;
    foreach (BocColumnDefinition column in renderColumns)
    {
      if (! column.Width.IsEmpty)
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, column.Width.ToString());

      writer.RenderBeginTag (HtmlTextWriterTag.Col);
      writer.RenderEndTag();
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

    if (IsSelectionEnabled)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTitleCell);
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      if (_selection == RowSelection.Multiple)
      {
        string checkBoxName = ID + c_titleRowCheckBoxIDSuffix;
        bool isChecked = (_checkBoxCheckedState[c_titleRowIndex] != null);
        RenderCheckBox (writer, checkBoxName, c_titleRowIndex.ToString(), isChecked, true);
      }
      else
      {
        writer.Write (c_whiteSpace);
      }
      writer.RenderEndTag();
    }

    HybridDictionary sortingDirections = new HybridDictionary();
    ArrayList sortingOrder = new ArrayList();
    if (EnableSorting)
    {
      foreach (SortingOrderEntry currentEntry in _sortingOrder)
      {
        sortingDirections[currentEntry.ColumnIndex] = currentEntry.Direction;
        if (currentEntry.Direction != SortingDirection.None)
          sortingOrder.Add (currentEntry.ColumnIndex);
      }
    }

    for (int idxColumns = 0; idxColumns < renderColumns.Length; idxColumns++)
    {
      BocColumnDefinition column = renderColumns[idxColumns];

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTitleCell);
      writer.RenderBeginTag (HtmlTextWriterTag.Td);

      // Click on Label or Button toggles direction -> span-tag around both
      bool hasSortingButton =    EnableSorting 
                              && (   column is BocValueColumnDefinition
                                  || (   column is BocCustomColumnDefinition 
                                      && ((BocCustomColumnDefinition) column).IsSortable));
      if (hasSortingButton && ! IsRowEditMode)
      {
        string argument = c_sortCommandPrefix + idxColumns.ToString();
        if (_hasClientScript)
        {
          string postBackScript = Page.GetPostBackClientHyperlink (this, argument);
          writer.AddAttribute (HtmlTextWriterAttribute.Href, postBackScript);
        }
        writer.RenderBeginTag (HtmlTextWriterTag.A);
      }
      if (IsDesignMode && column.ColumnTitleDisplayValue.Length == 0)
      {
        writer.Write (c_designModeEmptyContents);
      }
      else
      {
        string contents = HttpUtility.HtmlEncode (column.ColumnTitleDisplayValue);
        if (StringUtility.IsNullOrEmpty (contents))
          contents = c_whiteSpace;
        writer.Write (contents);
      }

      if (hasSortingButton)
      {
        object obj = sortingDirections[idxColumns];
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

          writer.AddAttribute (HtmlTextWriterAttribute.Src, imageUrl);
          writer.AddStyleAttribute ("border", "none");
          writer.AddStyleAttribute ("vertical-align", "middle");
          writer.RenderBeginTag (HtmlTextWriterTag.Img);
          writer.RenderEndTag();

          if (_showSortingOrder && sortingOrder.Count > 1)
          {
            int orderIndex = sortingOrder.IndexOf (idxColumns);
            writer.Write (c_whiteSpace + (orderIndex + 1).ToString());
          }
          writer.RenderEndTag();
        }

        if (hasSortingButton && ! IsRowEditMode)
          writer.RenderEndTag();  //  close A sorting
      }
      writer.RenderEndTag();  //  td
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

  /// <summary> Renders a table row containing the data for <paramref name="businessObject"/>. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object that receives the server control content. </param>
  /// <param name="businessObject"> The <see cref="IBusinessObject"/> whose data will be rendered. </param>
  /// <param name="rowIndex"> The row number in the current view. </param>
  /// <param name="originalRowIndex"> The position of <paramref name="businessObject"/> in the list of values. </param>
  /// <param name="isOddRow"> Whether the data row is rendered in an odd or an even table row. </param>
  private void RenderDataRow (
      HtmlTextWriter writer, 
      IBusinessObject businessObject,
      int rowIndex,
      int originalRowIndex,
      bool isOddRow)
  {
    bool isReadOnly = IsReadOnly;
    BocColumnDefinition[] renderColumns = EnsureColumnsGot();

    string objectID = null;
    IBusinessObjectWithIdentity businessObjectWithIdentity = businessObject as IBusinessObjectWithIdentity;
    if (businessObjectWithIdentity != null)
      objectID = businessObjectWithIdentity.UniqueIdentifier;

    string checkBoxID = ClientID + c_dataRowCheckBoxIDSuffix + rowIndex.ToString();
    bool isChecked = (_checkBoxCheckedState[originalRowIndex] != null);

    string cssClassTableCell;
    if (isChecked && _hasClientScript)
    {
      if (isOddRow)
        cssClassTableCell = CssClassDataCellOddSelected;
      else
        cssClassTableCell = CssClassDataCellEvenSelected;
    }
    else
    {
      if (isOddRow)
        cssClassTableCell = CssClassDataCellOdd;
      else
        cssClassTableCell = CssClassDataCellEven;
    }

    if (   IsSelectionEnabled 
        && (   EditableRowIndex.IsNull 
            || EditableRowIndex.Value != originalRowIndex))
    {
      if (_hasClientScript)
      {
        string isOddRowString = (isOddRow ? "true" : "false");
        string script = "BocList_OnRowClick ("
            + "document.getElementById ('" + ClientID + "'), "
            + "this, "
            + "document.getElementById ('" + checkBoxID + "'), "
            + isOddRowString 
            + ");";
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
        writer.AddAttribute ("onSelectStart", "return false");
      }
    }
    writer.RenderBeginTag (HtmlTextWriterTag.Tr);

    if (IsSelectionEnabled)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableCell);
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      RenderCheckBox (writer, checkBoxID, originalRowIndex.ToString(), isChecked, false);
      writer.RenderEndTag();
    }

    bool firstValueColumnRendered = false;
    for (int idxColumns = 0; idxColumns < renderColumns.Length; idxColumns++)
    {
      bool showIcon = false;
      BocColumnDefinition column = renderColumns[idxColumns];
      if ( (!firstValueColumnRendered) && column is BocValueColumnDefinition)
      {
        firstValueColumnRendered = true;
        if (EditableRowIndex.IsNull || EditableRowIndex.Value != originalRowIndex)
          showIcon = EnableIcon;
      }
      RenderDataCell (writer, idxColumns, column, originalRowIndex, businessObject, showIcon, cssClassTableCell);
    }
    
    writer.RenderEndTag();
  }

  private void RenderDataCell (
      HtmlTextWriter writer, 
      int columnIndex, BocColumnDefinition column, 
      int originalRowIndex, IBusinessObject businessObject,
      bool showIcon, string cssClassTableCell)
  {
    string objectID = null;
    IBusinessObjectWithIdentity businessObjectWithIdentity = businessObject as IBusinessObjectWithIdentity;
    if (businessObjectWithIdentity != null)
      objectID = businessObjectWithIdentity.UniqueIdentifier;
    bool isReadOnly = IsReadOnly;
    bool hasEditModeControl =   ! EditableRowIndex.IsNull 
                              && EditableRowIndex.Value == originalRowIndex
                              && _rowEditModeControls != null 
                              && _rowEditModeControls.Length > 0 
                              && _rowEditModeControls[columnIndex] != null;

    writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableCell);
    writer.RenderBeginTag (HtmlTextWriterTag.Td);

    BocCommandEnabledColumnDefinition commandEnabledColumn = column as BocCommandEnabledColumnDefinition;
    BocCustomColumnDefinition customColumn = column as BocCustomColumnDefinition;

    if (commandEnabledColumn != null)
    {
      BocCommandColumnDefinition commandColumn = column as BocCommandColumnDefinition;
      BocCompoundColumnDefinition compoundColumn = column as BocCompoundColumnDefinition;
      BocSimpleColumnDefinition simpleColumn = column as BocSimpleColumnDefinition;
      BocValueColumnDefinition valueColumn = column as BocValueColumnDefinition;

      //  Render the command
      bool isCommandEnabled = false;
      BocListItemCommand command = commandEnabledColumn.Command;
      if (command != null)
      {
        bool isActive =    command.Show == CommandShow.Always
                        || isReadOnly && command.Show == CommandShow.ReadOnly
                        || ! isReadOnly && command.Show == CommandShow.EditMode;
        if (   isActive
            && command.Type != CommandType.None
            && ! hasEditModeControl
            && (   command.CommandState == null
                || command.CommandState.IsEnabled (this, businessObject, commandEnabledColumn)))
        {
          isCommandEnabled = true;
        }
      }

      if (isCommandEnabled)
      {    
        string argument = c_eventListItemCommandPrefix + columnIndex + "," + originalRowIndex;
        string postBackLink = Page.GetPostBackClientHyperlink (this, argument);
        string onClick = "BocList_OnCommandClick();";
        command.RenderBegin (writer, postBackLink, onClick, originalRowIndex, objectID);
      }

      //  Render the icon
      if (showIcon)
      {
        IconInfo icon = BusinessObjectBoundWebControl.GetIcon (
            businessObject, 
            businessObject.BusinessObjectClass.BusinessObjectProvider);

        if (icon != null)
        {
          writer.AddAttribute (HtmlTextWriterAttribute.Src, icon.Url);
          if (! icon.Width.IsEmpty && ! icon.Height.IsEmpty)
          {
            writer.AddAttribute (HtmlTextWriterAttribute.Width, icon.Width.ToString());
            writer.AddAttribute (HtmlTextWriterAttribute.Height, icon.Height.ToString());
          }
          writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
          writer.AddStyleAttribute ("vertical-align", "middle");
          writer.RenderBeginTag (HtmlTextWriterTag.Img);
          writer.RenderEndTag();
          writer.Write (c_whiteSpace);
        }
      }

      //  Render the label
      if (commandColumn != null)
      {
        if (commandColumn.IconPath != null)
        {
          writer.AddAttribute (HtmlTextWriterAttribute.Src, commandColumn.IconPath);
          writer.RenderBeginTag (HtmlTextWriterTag.Img);
          writer.RenderEndTag();
        }

        if (commandColumn.Text != null)
          writer.Write (commandColumn.Text);
      }
      else if (compoundColumn != null)
      {
        string contents = compoundColumn.GetStringValue (businessObject);
        contents = HttpUtility.HtmlEncode (contents);
        if (StringUtility.IsNullOrEmpty (contents))
          contents = c_whiteSpace;
        writer.Write (contents);
      }
      else if (simpleColumn != null)
      {
        if (hasEditModeControl)
        {
          _rowEditModeControls[columnIndex].RenderControl (writer);
          BaseValidator[] validators = _rowEditModeValidators[columnIndex];
          foreach (BaseValidator validator in validators)
            validator.RenderControl (writer);
        }
        else
        {
          string contents = simpleColumn.GetStringValue (businessObject);
          contents = HttpUtility.HtmlEncode (contents);
          if (StringUtility.IsNullOrEmpty (contents))
            contents = c_whiteSpace;
          writer.Write (contents);
        }
      }

      if (isCommandEnabled)
        command.RenderEnd (writer);
    }
    else if (customColumn != null)
    {
      string onClick = "BocList_OnCommandClick();";
      customColumn.CustomCell.Render (
          writer, this, businessObject, customColumn, columnIndex, originalRowIndex, onClick);
    }

    writer.RenderEndTag();
  }

  /// <summary> Renders a <see cref="CheckBox"/> used for row selection. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object that receives the server control content. </param>
  /// <param name="id"> The <see cref="string"/> rendered into the <c>id</c> and <c>name</c> attributes. </param>
  /// <param name="value"> The value of the <see cref="CheckBox"/>. </param>
  /// <param name="isChecked"> <see langword="true"/> if the <c>CheckBox</c> is checked. </param>
  /// <param name="isSelectAllCheckBox"> 
  ///   <see langword="true"/> if the rendered <c>CheckBox</c> is the title row's <c>CheckBox</c>. </param>
  private void RenderCheckBox (
      HtmlTextWriter writer, 
      string id, 
      string value, 
      bool isChecked, 
      bool isSelectAllCheckBox)
  {
    writer.AddAttribute (HtmlTextWriterAttribute.Type, "checkbox");
    writer.AddAttribute (HtmlTextWriterAttribute.Id, id);
    writer.AddAttribute (HtmlTextWriterAttribute.Name, id);
    writer.AddAttribute (HtmlTextWriterAttribute.Value, value);
    if (isChecked)
      writer.AddAttribute (HtmlTextWriterAttribute.Checked, "checked");      
    if (isSelectAllCheckBox)
    {
      int count = 0;
      if (! _pageSize.IsNull)
        count = _pageSize.Value;
      else if (Value != null)
        count = Value.Count;

      if (_hasClientScript)
      {
        string script = "BocList_OnSelectAllCheckBoxClick ("
            + "document.getElementById ('" + ClientID + "'), "
            + "this , '"
            + ClientID + c_dataRowCheckBoxIDSuffix + "', "
            + count.ToString() + ");";
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
      }
    }
    else
    {
      if (_hasClientScript)
      {
        string script = "BocList_OnSelectionCheckBoxClick();";
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
      }
    }
    writer.RenderBeginTag (HtmlTextWriterTag.Input);
    writer.RenderEndTag();
  }

  /// <summary>
  ///   Obtains a reference to a client-side script function that causes, when invoked, a server postback to the form.
  /// </summary>
  /// <param name="columnIndex"> The index of the column for which the post back function should be created. </param>
  /// <param name="listIndex"> The index of the business object for which the post back function should be created. </param>
  /// <param name="customCellArgument"> 
  ///   The argument to be passed to the <see cref="IBocCustomColumnDefinitionCell"/>'s <c>OnClick</c> method.
  ///   Can be <see langword="null"/>.
  /// </param>
  /// <returns></returns>
  public string GetCustomCellPostBackClientEvent (int columnIndex, int listIndex, string customCellArgument)
  {
    string postBackArgument = FormatCustomCellPostBackArgument (columnIndex, listIndex, customCellArgument);
    return Page.GetPostBackClientEvent (this, postBackArgument);
  }

  /// <summary>
  ///   Obtains a hyperlink reference that causes, when invoked, a server postback to the form.
  /// </summary>
  /// <param name="columnIndex"> The index of the column for which the post back function should be created. </param>
  /// <param name="listIndex"> The index of the business object for which the post back function should be created. </param>
  /// <param name="customCellArgument"> 
  ///   The argument to be passed to the <see cref="IBocCustomColumnDefinitionCell"/>'s <c>OnClick</c> method.
  ///   Can be <see langword="null"/>.
  /// </param>
  /// <returns></returns>
  public string GetCustomCellPostBackClientHyperlink (int columnIndex, int listIndex, string customCellArgument)
  {
    string postBackArgument = FormatCustomCellPostBackArgument (columnIndex, listIndex, customCellArgument);
    return Page.GetPostBackClientHyperlink (this, postBackArgument);
  }

  /// <summary> Formats the arguments into a post back argument to be used by the client side post back event. </summary>
  private string FormatCustomCellPostBackArgument (int columnIndex, int listIndex, string customCellArgument)
  {
    if (customCellArgument == null)
      return c_customCellEventPrefix + columnIndex + "," + listIndex;
    else
      return c_customCellEventPrefix + columnIndex + "," + listIndex + "," + customCellArgument;
  }

  /// <summary> Calls the parent's <c>LoadViewState</c> method and restores this control's specific data. </summary>
  /// <param name="savedState"> An <see cref="Object"/> that represents the control state to be restored. </param>
  protected override void LoadViewState(object savedState)
  {
    object[] values = (object[]) savedState;
    
    base.LoadViewState (values[0]);
    _selectedColumnDefinitionSetIndex = (NaInt32) values[1];
    _currentRow = (int) values[2];
    _sortingOrder = (ArrayList) values[3];
    _editableRowIndex = (NaInt32) values[4];
    _isDirty = (bool) values[5];
  }

  /// <summary> Calls the parent's <c>SaveViewState</c> method and saves this control's specific data. </summary>
  /// <returns> Returns the server control's current view state. </returns>
  protected override object SaveViewState()
  {
    object[] values = new object[6];

    values[0] = base.SaveViewState();
    values[1] = _selectedColumnDefinitionSetIndex;
    values[2] = _currentRow;
    values[3] = _sortingOrder;
    values[4] = _editableRowIndex;
    values[5] = _isDirty;

    return values;
  }

  /// <summary>
  ///   Loads the <see cref="Value"/> from the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/> or uses the cached
  ///   information if <paramref name="interim"/> is <see langword="false"/>.
  /// </summary>
  /// <param name="interim">
  ///   <see langword="false"/> to load the <see cref="Value"/> from the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/>.
  /// </param>
  public override void LoadValue (bool interim)
  {
    //Binding.EvaluateBinding();
    if (Property != null && DataSource != null && DataSource.BusinessObject != null)
    {
      ValueImplementation = DataSource.BusinessObject.GetProperty (Property);
      _isDirty = false;
    }
  }

  /// <summary>
  ///   Writes the <see cref="Value"/> into the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/> if <paramref name="interim"/> 
  ///   is <see langword="false"/>.
  /// </summary>
  /// <param name="interim">
  ///   <see langword="false"/> to write the <see cref="Value"/> into the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/>.
  /// </param>
  public override void SaveValue (bool interim)
  {
    //Binding.EvaluateBinding();
    if (Property != null && DataSource != null && DataSource.BusinessObject != null && ! IsReadOnly)
      DataSource.BusinessObject.SetProperty (Property, Value);
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

    if (Property == null)
      return new BocColumnDefinition[0];

    IBusinessObjectProperty[] properties = 
        ((IBusinessObjectReferenceProperty)Property).ReferenceClass.GetPropertyDefinitions();
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

  /// <summary> Refreshes the <see cref="_additionalColumnsList"/>. </summary>
  private void PopulateAdditionalColumnsList()
  {
    _additionalColumnsList.Items.Clear();

    if (_availableColumnDefinitionSets != null)
    {
      for (int i = 0; i < _availableColumnDefinitionSets.Count; i++)
      {
        BocColumnDefinitionSet columnDefinitionCollection = _availableColumnDefinitionSets[i];

        ListItem item = new ListItem (columnDefinitionCollection.Title, i.ToString());
        _additionalColumnsList.Items.Add (item);
      }

      if (_selectedColumnDefinitionSetIndex >= _availableColumnDefinitionSets.Count)
      {
        _selectedColumnDefinitionSetIndex = NaInt32.Null;
      }
      else if (   _selectedColumnDefinitionSetIndex.IsNull
               && _availableColumnDefinitionSets.Count > 0)
      {
        _selectedColumnDefinitionSetIndex = 0;
      }

      SelectedColumnDefinitionSetIndex = _selectedColumnDefinitionSetIndex;
    }
  }

  /// <summary>
  ///   Handles the <see cref="ListControl.SelectedIndexChanged"/> event of the <see cref="_additionalColumnsList"/>.
  /// </summary>
  private void AdditionalColumnsList_SelectedIndexChanged (object sender, EventArgs e)
  {
    SelectedColumnDefinitionSetIndex = _additionalColumnsList.SelectedIndex;
  }

  /// <summary>
  ///   Handles the <c>CollectionChanged</c> event of the <see cref="AvailableColumnDefinitionSets"/> collection.
  /// </summary>
  private void AvailableColumnDefinitionSets_CollectionChanged(object sender, CollectionChangeEventArgs e)
  {
    PopulateAdditionalColumnsList();
  }

  /// <summary> Handles the <see cref="ImageButton.Click"/> event of the <see cref="_moveFirstButton"/>. </summary>
  private void MoveFirstButton_Click(object sender, ImageClickEventArgs e)
  {
    _move = MoveOption.First;
  }

  /// <summary> Handles the <see cref="ImageButton.Click"/> event of the <see cref="_moveLastButton"/>. </summary>
  private void MoveLastButton_Click(object sender, ImageClickEventArgs e)
  {
    _move = MoveOption.Last;
  }
  
  /// <summary> Handles the <see cref="ImageButton.Click"/> event of the <see cref="_movePreviousButton"/>. </summary>
  private void MovePreviousButton_Click(object sender, ImageClickEventArgs e)
  {
    _move = MoveOption.Previous;
  }
  
  /// <summary> Handles the <see cref="ImageButton.Click"/> event of the <see cref="_moveNextButton"/>. </summary>
  private void MoveNextButton_Click(object sender, ImageClickEventArgs e)
  {
    _move = MoveOption.Next;
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
  ///   the <see cref="_allPropertyColumns"/> and the <see cref="SelectedColumnDefinitionSet"/>
  ///   into a single array.
  /// </summary>
  /// <returns> An array of <see cref="BocColumnDefinition"/> objects. </returns>
  private BocColumnDefinition[] GetColumnsInternal (bool isPostBackEventPhase)
  {
    BocColumnDefinition[] allPropertyColumns = null;
    if (_showAllProperties)
      allPropertyColumns = GetAllPropertyColumns();
    else
      allPropertyColumns = new BocColumnDefinition[0];

    BocColumnDefinition[] selectedColumns = null;
    EnsureSelectedColumnDefinitionIndexSet();
    if (_selectedColumnDefinitionSet != null)
      selectedColumns = _selectedColumnDefinitionSet.ColumnDefinitions.ToArray();
    else
      selectedColumns = new BocColumnDefinition[0];

    BocColumnDefinition[] columnDefinitions = (BocColumnDefinition[]) ArrayUtility.Combine (
      _fixedColumns.ToArray(),
      allPropertyColumns,
      selectedColumns);

    if (isPostBackEventPhase)
      return GetColumnsForPreviousLifeCycle (columnDefinitions);
    else
      return GetColumns (columnDefinitions);
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
  ///     Make the method <c>protected virtual</c> should this feature be ever required and change the 
  ///     method's body to return the passed <c>columnDefinitions</c>.
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

  private BocMenuItem[] EnsureOptionsMenuItemsForPreviousLifeCycleGot()
  {
    if (_optionsMenuItemsPostBackEventHandlingPhase == null)
    {
      _optionsMenuItemsPostBackEventHandlingPhase = 
          GetOptionsMenuItemsForPreviousLifeCycle (_optionsMenuItems.ToArray());
    }
    return _optionsMenuItemsPostBackEventHandlingPhase;
  }

  private BocMenuItem[] EnsureOptionsMenuItemsGot(bool forceRefresh)
  {
    if (_optionsMenuItemsRenderPhase == null || forceRefresh)
      _optionsMenuItemsRenderPhase = GetOptionsMenuItems (_optionsMenuItems.ToArray());
    return _optionsMenuItemsRenderPhase;
  }

  private BocMenuItem[] EnsureOptionsMenuItemsGot()
  {
    return EnsureOptionsMenuItemsGot (false);
  }

  /// <summary>
  ///   Override this method to modify the menu items displayed in the <see cref="BocList"/>'s options menu
  ///   during the previous page life cycle.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     The <see cref="BocColumnDefinition"/> instances displayed during the last page life cycle are required 
  ///     to correctly handle the events raised on the BocList, such as an <see cref="Command"/> event 
  ///     or a data changed event.
  ///   </para><para>
  ///     Make the method <c>protected virtual</c> should this feature be ever required and change the 
  ///     method's body to return the passed <c>menuItems</c>.
  ///   </para>
  /// </remarks>
  /// <param name="menuItems"> 
  ///   The <see cref="BocMenuItem"/> array containing the menu item available in the options menu. 
  /// </param>
  /// <returns> The <see cref="BocMenuItem"/> array. </returns>
  private BocMenuItem[] GetOptionsMenuItemsForPreviousLifeCycle (BocMenuItem[] menuItems)
  {
    //  return menuItems;
    return EnsureOptionsMenuItemsGot();
  }

  /// <summary>
  ///   Override this method to modify the menu items displayed in the <see cref="BocList"/>'s options menu
  ///   in the current page life cycle.
  /// </summary>
  /// <remarks>
  ///   This call can happen more than once in the control's life cycle, passing different 
  ///   arrays in <paramref name="menuItems" />. It is therefor important to not cache the return value
  ///   in the override of <see cref="GetOptionsMenuItems"/>.
  /// </remarks>
  /// <param name="menuItems"> 
  ///   The <see cref="BocMenuItem"/> array containing the menu item available in the options menu. 
  /// </param>
  /// <returns> The <see cref="BocMenuItem"/> array. </returns>
  protected virtual BocMenuItem[] GetOptionsMenuItems (BocMenuItem[] menuItems)
  {
    return menuItems;
  }

  private BocMenuItem[] EnsureListMenuItemsForPreviousLifeCycleGot()
  {
    if (_listMenuItemsPostBackEventHandlingPhase == null)
    {
      _listMenuItemsPostBackEventHandlingPhase = 
          GetListMenuItemsForPreviousLifeCycle (_listMenuItems.ToArray());
    }
    return _listMenuItemsPostBackEventHandlingPhase;
  }

  private BocMenuItem[] EnsureListMenuItemsGot (bool forceRefresh)
  {
    if (_listMenuItemsRenderPhase == null || forceRefresh)
      _listMenuItemsRenderPhase = GetListMenuItems (_listMenuItems.ToArray());
    return _listMenuItemsRenderPhase;
  }

  private BocMenuItem[] EnsureListMenuItemsGot()
  {
    return EnsureListMenuItemsGot (false);
  }

  /// <summary>
  ///   Override this method to modify the menu items displayed in the <see cref="BocList"/>'s menu area
  ///   during the previous page life cycle.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     The <see cref="BocColumnDefinition"/> instances displayed during the last page life cycle are required 
  ///     to correctly handle the events raised on the BocList, such as an <see cref="Command"/> event 
  ///     or a data changed event.
  ///   </para><para>
  ///     Make the method <c>protected virtual</c> should this feature be ever required and change the 
  ///     method's body to return the passed <c>menuItems</c>.
  ///   </para>
  /// </remarks>
  /// <param name="menuItems"> 
  ///   The <see cref="BocMenuItem"/> array containing the menu item available in the options menu. 
  /// </param>
  /// <returns> The <see cref="BocMenuItem"/> array. </returns>
  private BocMenuItem[] GetListMenuItemsForPreviousLifeCycle (BocMenuItem[] menuItems)
  {
    //  return menuItems;
    return EnsureListMenuItemsGot();
  }

  /// <summary>
  ///   Override this method to modify the menu items displayed in the <see cref="BocList"/>'s menu area
  ///   in the current page life cycle.
  /// </summary>
  /// <remarks>
  ///   This call can happen more than once in the control's life cycle, passing different 
  ///   arrays in <paramref name="menuItems" />. It is therefor important to not cache the return value
  ///   in the override of <see cref="GetListMenuItems"/>.
  /// </remarks>
  /// <param name="menuItems"> 
  ///   The <see cref="BocMenuItem"/> array containing the menu item available in the options menu. 
  /// </param>
  /// <returns> The <see cref="BocMenuItem"/> array. </returns>
  protected virtual BocMenuItem[] GetListMenuItems (BocMenuItem[] menuItems)
  {
    return menuItems;
  }

  /// <summary>
  ///   Removes the columns provided by <see cref="SelectedColumnDefinitionSet"/> from the 
  ///   <see cref="_sortingOrder"/> list.
  /// </summary>
  private void RemoveDynamicColumnsFromSortingOrder()
  {
    if (EnableSorting)
    {
      int fixedColumnCount = _fixedColumns.Count;
      if (_showAllProperties)
        fixedColumnCount += GetAllPropertyColumns().Length;
      ArrayList entriesToBeRemoved = new ArrayList();
      for (int idxSortingKeys = 0; idxSortingKeys < _sortingOrder.Count; idxSortingKeys++)
      {
        SortingOrderEntry currentEntry = (SortingOrderEntry) _sortingOrder[idxSortingKeys];
        if (currentEntry.ColumnIndex >= fixedColumnCount)
          entriesToBeRemoved.Add (currentEntry);
      }
      foreach (SortingOrderEntry currentEntry in entriesToBeRemoved)
        _sortingOrder.Remove (currentEntry);
    }
  }

  /// <summary>
  ///   Compares <paramref name="objectA"/> and <paramref name="objectB"/>.
  /// </summary>
  /// <param name="objectA"> 
  ///   First <see cref="Pair"/> (&lt;int index, IBusniessObject object&gt;) to compare.
  ///   Must not be <see langword="null"/>.
  /// </param>
  /// <param name="objectB"> 
  ///   Second <see cref="Pair"/> (&lt;int index, IBusniessObject object&gt;) to compare.
  ///   Must not be <see langword="null"/>.
  /// </param>
  /// <returns>
  ///   <list type="table">
  ///     <listheader>
  ///       <term> Value </term>
  ///       <description> Condition </description>
  ///     </listheader>
  ///     <item>
  ///       <term> Less than zero </term>
  ///       <description> <paramref name="objectA"/> is less than <paramref name="objectB"/>. </description>
  ///     </item>
  ///     <item>
  ///       <term> Zero </term>
  ///       <description> <paramref name="objectA"/> eqals <paramref name="objectB"/>. </description>
  ///     </item>
  ///     <item>
  ///       <term> Greater than zero </term>
  ///       <description> <paramref name="objectA"/> is greater than <paramref name="objectB"/>. </description>
  ///     </item>
  ///   </list>
  /// </returns>
  int IComparer.Compare (object objectA , object objectB)
  {
    ArgumentUtility.CheckNotNullAndType ("objectA", objectA, typeof (Pair));
    ArgumentUtility.CheckNotNullAndType ("objectB", objectB, typeof (Pair));

    Pair pairA = (Pair) objectA;
    Pair pairB = (Pair) objectB;

    ArgumentUtility.CheckNotNullAndType ("pairA.Second", pairA.Second, typeof (IBusinessObject));
    ArgumentUtility.CheckNotNullAndType ("pairB.Second", pairB.Second, typeof (IBusinessObject));

    IBusinessObject businessObjectA = (IBusinessObject) pairA.Second;
    IBusinessObject businessObjectB = (IBusinessObject) pairB.Second;

    BocColumnDefinition[] renderColumns = EnsureColumnsGot();
    foreach (SortingOrderEntry currentEntry in _sortingOrder)
    {
      if (currentEntry.Direction != SortingDirection.None)
      {
        BocColumnDefinition column = renderColumns[currentEntry.ColumnIndex];
        if (   ! (column is BocValueColumnDefinition) 
            && ! (   column is BocCustomColumnDefinition
                  && ((BocCustomColumnDefinition) column).IsSortable))
        {
          throw new ArgumentOutOfRangeException ("The BocList '" + ID + "' does not have a value column at index" + currentEntry.ColumnIndex + ".");
        }

        BocSimpleColumnDefinition simpleColumn = column as BocSimpleColumnDefinition;
        BocCompoundColumnDefinition compoundColumn = column as BocCompoundColumnDefinition;
        BocCustomColumnDefinition customColumn = column as BocCustomColumnDefinition;
        
        if (simpleColumn != null || customColumn != null)
        {
          BusinessObjectPropertyPath propertyPath;
          string formatString = null;
          if (simpleColumn != null)
          {
            propertyPath = simpleColumn.PropertyPath;
            formatString = simpleColumn.FormatString;
          }
          else
          {
            propertyPath = customColumn.PropertyPath;
          }
          //  single value
          int compareResult = 0;
          if (currentEntry.Direction == SortingDirection.Ascending)
            compareResult = ComparePropertyPathValues (propertyPath, businessObjectA, businessObjectB);
          else
            compareResult = ComparePropertyPathValues (propertyPath, businessObjectB, businessObjectA);
          if (compareResult != 0)
            return compareResult;

          string stringValueA = null;
          string stringValueB = null;
          try
          {
            if (simpleColumn != null)
              stringValueA = simpleColumn.GetStringValue (businessObjectA);
            else
              stringValueA = propertyPath.GetString (businessObjectA, "");
          }
          catch
          {
          }
          try
          {
            if (simpleColumn != null)
              stringValueB = simpleColumn.GetStringValue (businessObjectB);
            else
              stringValueB = propertyPath.GetString (businessObjectB, "");
          }
          catch
          {
          }
          if (currentEntry.Direction == SortingDirection.Ascending)
            compareResult = string.Compare (stringValueA, stringValueB);
          else
            compareResult = string.Compare (stringValueB, stringValueA);
          if (compareResult != 0)
            return compareResult;
        }
        else if (compoundColumn != null)
        {
          int compareResult = 0;
          //  Compund column, list of values.
          foreach (PropertyPathBinding propertyPathBinding in compoundColumn.PropertyPathBindings)
          {
            if (currentEntry.Direction == SortingDirection.Ascending)
              compareResult = ComparePropertyPathValues (propertyPathBinding.PropertyPath, businessObjectA, businessObjectB);
            else
              compareResult = ComparePropertyPathValues (propertyPathBinding.PropertyPath, businessObjectB, businessObjectA);
            if (compareResult != 0)
              return compareResult;
          }
              
          string stringValueA = null;
          string stringValueB = null;
          try
          {
            stringValueA = compoundColumn.GetStringValue (businessObjectA);
          }
          catch
          {
          }
          try
          {
            stringValueB = compoundColumn.GetStringValue (businessObjectB);
          }
          catch
          {
          }
          if (currentEntry.Direction == SortingDirection.Ascending)
            compareResult = string.Compare (stringValueA, stringValueB);
          else
            compareResult = string.Compare (stringValueB, stringValueA);
          if (compareResult != 0)
            return compareResult;
        }
      } 
    }
    if (businessObjectA != null && businessObjectB != null)
    {
      int indexObjectA = (int) pairA.First;
      int indexObjectB = (int) pairB.First;
      return indexObjectA - indexObjectB;
    }
    return 0;
  }

  /// <summary>
  ///   Compares the values of the <see cref="IBusinessObjectProperty"/> identified by <paramref name="propertyPath"/>
  ///   for <paramref name="businessObjectA"/> and <paramref name="businessObjectB"/>.
  /// </summary>
  /// <remarks>
  ///   Only compares values supporting <see cref="ICompareable"/>. Returns 0 for other value types.
  /// </remarks>
  /// <param name="propertyPath">
  ///   The <see cref="BusinessObjectPropertyPath"/> to be used for accessing the values.
  /// </param>
  /// <param name="businessObjectA"> 
  ///   First <see cref="IBusinessObject"/> to compare. Must not be <see langword="null"/>.
  /// </param>
  /// <param name="businessObjectB"> 
  ///   Second <see cref="IBusinessObject"/> to compare. Must not be <see langword="null"/>.
  /// </param>
  private int ComparePropertyPathValues (
      BusinessObjectPropertyPath propertyPath, 
      IBusinessObject businessObjectA, 
      IBusinessObject businessObjectB)
  {
    object valueA = null;
    object valueB = null;

    try
    {
      valueA = propertyPath.GetValue (businessObjectA, false, true);
    }
    catch 
    { 
    }

    try
    {
      valueB = propertyPath.GetValue (businessObjectB, false, true);
    }
    catch 
    {
    }

    if (valueA == null && valueB == null)
      return 0;
    if (valueA == null)
      return -1;
    if (valueB == null)
      return 1;

    IList listA = valueA as IList;
    IList listB = valueB as IList;
    if (listA != null || listB != null)
    {
      if (listA == null || listB == null)
        return 0;
      if (listA.Count == 0 && listB.Count == 0)
        return 0;
      if (listA.Count == 0)
        return -1;
      if (listB.Count == 0)
        return 1;
      valueA = listA[0];
      valueB = listB[0];
    }

    if (valueA is IComparable && valueB is IComparable)
      return Comparer.Default.Compare (valueA, valueB);

    return 0;
    //Better leave the comparisson of non-ICompareables to the calling method. ToString is not always the right choice.
    //return Comparer.Default.Compare (valueA.ToString(), valueB.ToString());
  }

  /// <summary> Dispatches the resources passed in <paramref name="values"/> to the <see cref="BocList"/>'s properties. </summary>
  /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
  public void Dispatch (IDictionary values)
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
    DispatchToColumns (_fixedColumns, fixedColumnValues, "FixedColumns");
    DispatchToMenuItems (_optionsMenuItems, optionsMenuItemValues, "OptionsMenuItems");
    DispatchToMenuItems (_listMenuItems, listMenuItemValues, "ListMenuItems");
  }

  /// <summary>
  ///   Dispatches the resources passed in <paramref name="values"/> to the properties of the 
  ///   <see cref="BocColumnDefinition"/> objects in the collection <paramref name="columns"/>.
  /// </summary>
  private void DispatchToColumns (BocColumnDefinitionCollection columns, IDictionary values, string collectionName)
  {
    foreach (DictionaryEntry entry in values)
    {
      string columnID = (string) entry.Key;
      
      bool isValidID = false;
      foreach (BocColumnDefinition columnDefinition in columns)
      {
        if (columnDefinition.ColumnID == columnID)
        {
          ResourceDispatcher.DispatchGeneric (columnDefinition, (IDictionary) entry.Value);
          isValidID = true;
          break;
        }
      }

      if (! isValidID)
      {
        //  Invalid collection element
        s_log.Debug ("BocList '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' does not contain an item with an ID of '" + columnID + "' inside the collection '" + collectionName + "'.");
      }
    }
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
        s_log.Debug ("BocList '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' does not contain an item with an ID of '" + itemID + "' inside the collection '" + collectionName + "'.");
      }
    }
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
      if (EnableClientScript) 
      {
        bool isVersionGreaterOrEqual55 = 
               Context.Request.Browser.MajorVersion >= 6
            ||    Context.Request.Browser.MajorVersion == 5 
               && Context.Request.Browser.MinorVersion >= 0.5;
        bool isInternetExplorer55AndHigher = 
            Context.Request.Browser.Browser == "IE" && isVersionGreaterOrEqual55;

        _hasClientScript = isInternetExplorer55AndHigher;

        // // The next set of checks involve looking at the capabilities of the
        // // browser making the request.
        // //
        // // The DatePicker needs to verify whether the browser has EcmaScript (JavaScript)
        // // version 1.2+, and whether the browser supports DHTML, and optionally,
        // // DHTML behaviors.
        //
        // HttpBrowserCapabilities browserCaps = Page.Request.Browser;
        // bool hasEcmaScript = (browserCaps.EcmaScriptVersion.CompareTo(new Version(1, 2)) >= 0);
        // bool hasDOM = (browserCaps.MSDomVersion.Major >= 4);
        // bool hasBehaviors = (browserCaps.MajorVersion > 5) ||
        //                     ((browserCaps.MajorVersion == 5) && (browserCaps.MinorVersion >= .5));
        //
        // _hasClientScript = hasEcmaScript && hasDOM;
      }
    }
  }

  private IBusinessObjectReferenceDataSource _rowEditModeDataSource;
  private IBusinessObjectBoundModifiableWebControl[] _rowEditModeControls;
  private bool _isRowEditModeRestored = false;
  /// <summary> &lt;column index&gt;&lt;validator index&gt; </summary>
  private BaseValidator[][] _rowEditModeValidators;

  /// <summary>
  ///   Saves changes to previous edited row and starts editing for the new row.
  /// </summary>
  /// <param name="index"></param>
  public void SwitchRowIntoEditMode (int index)
  {
    EnsureRowEditModeRestored();

    if (IsRowEditMode)
      EndRowEditMode (true);
    if (IsReadOnly || IsRowEditMode)
      return;

    if (Value.Count > index)
      _editableRowIndex = index;
    else
      throw new ArgumentOutOfRangeException ("index");

    CreateRowEditModeControls (EnsureColumnsGot());
    _rowEditModeDataSource.LoadValues (false);
  }

  public void EndRowEditMode (bool saveChanges)
  {
    EnsureRowEditModeRestored();

    if (! IsRowEditMode)
      return;

    if (saveChanges && ! IsReadOnly)
    {
      bool isValid = ValiadateModifiableRow();
      if (!isValid)
        return;

      if (! _isDirty)
      {
        foreach (IBusinessObjectBoundModifiableWebControl control in _rowEditModeControls)
        {
          if (control != null)
            _isDirty |= control.IsDirty;
        }
      }
      _rowEditModeDataSource.SaveValues (false);
    }

    RemoveEditModeControls();
    _editableRowIndex = NaInt32.Null;
  }

  private void EnsureRowEditModeRestored()
  {
    if (_isRowEditModeRestored)
      return;
    _isRowEditModeRestored = true;
    if (! IsRowEditMode)
      return;

    CreateRowEditModeControls (EnsureColumnsForPreviousLifeCycleGot());
  }

  private void CreateRowEditModeControls (BocColumnDefinition[] columns)
  {
    if (! IsRowEditMode)
      return;

    _rowEditModeDataSource = CreateRowEditModeDataSource ((IBusinessObject) Value[_editableRowIndex.Value]);
    _rowEditModeControls = new IBusinessObjectBoundModifiableWebControl[columns.Length];
    _rowEditModeValidators = new BaseValidator[columns.Length][];

    for (int i = 0; i < columns.Length; i++)
    {
      BocSimpleColumnDefinition simpleColumn = columns[i] as BocSimpleColumnDefinition;
      if (simpleColumn == null)
        continue;
      if (simpleColumn.PropertyPath.Properties.Length > 1)
        continue;

      IBusinessObjectBoundModifiableWebControl control = CreateRowEditModeControl (simpleColumn, i);
      control.DataSource = _rowEditModeDataSource;
      _rowEditModeControls[i] = control;
      if (control != null)
      {
        Controls.Add ((Control) control);
        _rowEditModeValidators[i] = control.CreateValidators();
      }
    }    
    
    foreach (BaseValidator[] columnValidators in _rowEditModeValidators)
    {
      if (columnValidators == null)
        continue;
      foreach (BaseValidator validator in columnValidators)
        Controls.Add (validator);
    }
  }

  private bool ValiadateModifiableRow()
  {
    bool isValid = true;
    foreach (BaseValidator[] columnValidators in _rowEditModeValidators)
    {
      if (columnValidators == null)
        continue;
      foreach (BaseValidator validator in columnValidators)
      {
        validator.Validate();
        isValid &= validator.IsValid;
      }
    }
    return isValid;
  }

  protected virtual IBusinessObjectReferenceDataSource CreateRowEditModeDataSource (IBusinessObject businessObject)
  {
    IBusinessObjectReferenceDataSource dataSource = new BusinessObjectReferenceDataSource();
    dataSource.BusinessObject = businessObject;
    return dataSource;
  }

  protected virtual IBusinessObjectBoundModifiableWebControl CreateRowEditModeControl (
    BocSimpleColumnDefinition column,
    int columnIndex)
  {
    IBusinessObjectBoundModifiableWebControl control;
    IBusinessObjectProperty property = column.PropertyPath.LastProperty;

    if (IsBocReferenceValueSupported (property))
      control = new BocReferenceValue();
    else if (IsBocDateTimeValueSupported (property))
      control = new BocDateTimeValue();
    else if (IsBocBooleanValueSupported (property))
      control = new BocBooleanValue();
    else if (IsBocEnumValueSupported (property))
      control = new BocEnumValue();
    else if (IsBocMultilineTextValueSupported (property))
      control = new BocMultilineTextValue();
    else if (IsBocTextValueSupported (property))
      control = new BocTextValue();
    else
      return null;
    
    control.ID = ID + "_RowEditControl_" + columnIndex.ToString();
    control.Property = property;
    if (control is WebControl)
      ((WebControl) control).Width = Unit.Percentage (100);
    else if (control is System.Web.UI.HtmlControls.HtmlControl)
      ((System.Web.UI.HtmlControls.HtmlControl) control).Attributes["Width"] = "100%";
    return control;
  }

  private void RemoveEditModeControls()
  {
    foreach (Control control in _rowEditModeControls)
      Controls.Remove (control);
  }
  protected bool IsBocTextValueSupported (IBusinessObjectProperty property)
  {
    if (! BocTextValue.IsPropertyMultiplicitySupported (property.IsList))
      return false;
    return BusinessObjectBoundWebControl.IsPropertyInterfaceSupported (
        property, 
        BocTextValue.GetSupportedPropertyInterfaces());
  }

  protected bool IsBocMultilineTextValueSupported (IBusinessObjectProperty property)
  {
    if (! BocMultilineTextValue.IsPropertyMultiplicitySupported (property.IsList))
      return false;
    return BusinessObjectBoundWebControl.IsPropertyInterfaceSupported (
        property, 
        BocMultilineTextValue.GetSupportedPropertyInterfaces());
  }

  protected bool IsBocBooleanValueSupported (IBusinessObjectProperty property)
  {
    if (! BocBooleanValue.IsPropertyMultiplicitySupported (property.IsList))
      return false;
    return BusinessObjectBoundWebControl.IsPropertyInterfaceSupported (
        property, 
        BocBooleanValue.GetSupportedPropertyInterfaces());
  }

  protected bool IsBocEnumValueSupported (IBusinessObjectProperty property)
  {
    if (! BocEnumValue.IsPropertyMultiplicitySupported (property.IsList))
      return false;
    return BusinessObjectBoundWebControl.IsPropertyInterfaceSupported (
        property, 
        BocEnumValue.GetSupportedPropertyInterfaces());
  }

  protected bool IsBocDateTimeValueSupported (IBusinessObjectProperty property)
  {
    if (! BocDateTimeValue.IsPropertyMultiplicitySupported (property.IsList))
      return false;
    return BusinessObjectBoundWebControl.IsPropertyInterfaceSupported (
        property, 
        BocDateTimeValue.GetSupportedPropertyInterfaces());
  }

  protected bool IsBocReferenceValueSupported (IBusinessObjectProperty property)
  {
    if (! BocReferenceValue.IsPropertyMultiplicitySupported (property.IsList))
      return false;
    return BusinessObjectBoundWebControl.IsPropertyInterfaceSupported (
        property, 
        BocReferenceValue.GetSupportedPropertyInterfaces());
  }

  public NaInt32 EditableRowIndex
  {
    get { return _editableRowIndex; }
  }

  /// <remarks>
  ///   Queried where the rendering depends on whether the list is in edit mode. 
  ///   Affected code: sorting buttons, additional columns list, paging buttons, selected column definition set index
  /// </remarks>
  protected bool IsRowEditMode
  {
    get { return ! _editableRowIndex.IsNull; } 
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
      ArgumentUtility.CheckType ("value", value, typeof (IBusinessObjectReferenceProperty));
      if (value.IsList == false)
        throw new ArgumentException ("Only properties supporting IList can be assigned to the BocList.", "value");
      base.Property = (IBusinessObjectReferenceProperty) value; 
    }
  }

  /// <summary> Gets or sets the current value. </summary>
  /// <value> An object implementing <see cref="IList"/>. </value>
  [Browsable (false)]
  public new IList Value
  {
    get { return _value; }
    set { _value = value; }
  }

  /// <summary> Gets or sets the current value when <see cref="Value"/> through polymorphism. </summary>
  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = (IList) value; }
  }

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its ClientID.
  /// </summary>
  public override Control TargetControl 
  {
    get { return (Control) this; }
  }

  /// <summary>
  ///   Gets or sets the dirty flag.
  /// </summary>
  /// <remarks>
  ///   Initially, the value of <c>IsDirty</c> is <see langword="true"/>. The value is 
  ///   set to <see langword="false"/> during loading
  ///   and saving values. Text changes by the user cause <c>IsDirty</c> to be reset to 
  ///   <see langword="false"/> during the
  ///   loading phase of the request (i.e., before the page's <c>Load</c> event is raised).
  /// </remarks>
  public override bool IsDirty
  {
    get { return _isDirty; }
    set { _isDirty = value; }
  }

  /// <summary>
  ///   Gets or sets the list of<see cref="Type"/> objects for the <see cref="IBusinessObjectProperty"/> 
  ///   implementations that can be bound to this control.
  /// </summary>
  protected override Type[] SupportedPropertyInterfaces
  {
    get { return s_supportedPropertyInterfaces; }
  }

  /// <summary> Gets a value that indicates whether properties with the specified multiplicity are supported. </summary>
  /// <returns> <see langword="true"/> if the multiplicity specified by <paramref name="isList"/> is supported. </returns>
  protected override bool SupportsPropertyMultiplicity (bool isList)
  {
    return isList;
  }

  /// <summary> Gets the user independent column defintions. </summary>
  /// <remarks> Behavior undefined if set after initialization phase or changed between postbacks. </remarks>
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  //  Default category
  [Description ("The user independent column defintions.")]
  [DefaultValue ((string) null)]
  public BocColumnDefinitionCollection FixedColumns
  {
    get { return _fixedColumns; }
  }

  //  No designer support intended
  /// <summary> Gets the predefined column defintion sets that the user can choose from at run-time. </summary>
  //  [PersistenceMode(PersistenceMode.InnerProperty)]
  //  [ListBindable (false)]
  //  //  Default category
  //  [Description ("The predefined column defintion sets that the user can choose from at run-time.")]
  //  [DefaultValue ((string) null)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public BocColumnDefinitionSetCollection AvailableColumnDefinitionSets
  {
    get { return _availableColumnDefinitionSets; }
  }

  /// <summary>
  ///   Gets or sets the selected <see cref="BocColumnDefinitionSet"/> used to
  ///   supplement the <see cref="FixedColumns"/>.
  /// </summary>
  [Browsable (false)]
  public BocColumnDefinitionSet SelectedColumnDefinitionSet
  {
    get 
    {
      EnsureSelectedColumnDefinitionIndexSet();
      return _selectedColumnDefinitionSet; 
    }
    set
    {
      bool hasChanged = _selectedColumnDefinitionSet != value; 
      _selectedColumnDefinitionSet = value; 
      ArgumentUtility.CheckNotNullOrEmpty ("AvailableColumnDefinitionSets", _availableColumnDefinitionSets);
      _selectedColumnDefinitionSetIndex = NaInt32.Null;

      if (_selectedColumnDefinitionSet != null)
      {
        for (int i = 0; i < _availableColumnDefinitionSets.Count; i++)
        {
          if (_availableColumnDefinitionSets[i] == _selectedColumnDefinitionSet)
          {
            _selectedColumnDefinitionSetIndex = i;
            break;
          }
        }

        if (_selectedColumnDefinitionSetIndex.IsNull) 
          throw new IndexOutOfRangeException ("The specified ColumnDefinitionSet could not be found in AvailableColumnDefinitionSets");
      }

      _additionalColumnsList.SelectedIndex = _selectedColumnDefinitionSetIndex.Value;
      if (hasChanged)
        RemoveDynamicColumnsFromSortingOrder();
    }
  }

  private void EnsureSelectedColumnDefinitionIndexSet()
  {
    if (_isSelectedColumnDefinitionIndexSet)
      return;
    SelectedColumnDefinitionSetIndex = _selectedColumnDefinitionSetIndex;
  }

  /// <summary>
  ///   Gets or sets the index of the selected <see cref="BocColumnDefinitionSet"/> used to
  ///   supplement the <see cref="FixedColumns"/>.
  /// </summary>
  protected NaInt32 SelectedColumnDefinitionSetIndex
  {
    get { return _selectedColumnDefinitionSetIndex; }
    set 
    {
      if (   ! value.IsNull 
          && (value.Value < 0 || value.Value >= _availableColumnDefinitionSets.Count))
      {
        throw new ArgumentOutOfRangeException ("value", value, "SelectedColumnDefinitionSetIndex is outside the bounds of AvailableColumnDefinitionSets");
      }

      if (   IsRowEditMode
          && _isSelectedColumnDefinitionIndexSet
          && _selectedColumnDefinitionSetIndex != value)
      {
        throw new InvalidOperationException ("The selected column defintion set cannot be changed while the BocList is in row edit mode.");
      }

      bool hasIndexChanged = _selectedColumnDefinitionSetIndex != value; 
      _selectedColumnDefinitionSetIndex = value; 
      _isSelectedColumnDefinitionIndexSet = true;

      if (! _selectedColumnDefinitionSetIndex.IsNull)
      {
        int selectedIndex = _selectedColumnDefinitionSetIndex.Value;

        if (selectedIndex < _availableColumnDefinitionSets.Count)
          _selectedColumnDefinitionSet = (BocColumnDefinitionSet) _availableColumnDefinitionSets[selectedIndex];
        else
          _selectedColumnDefinitionSet = null;
      }
      else
      {
        _selectedColumnDefinitionSet = null;
      }
      if (hasIndexChanged)
        RemoveDynamicColumnsFromSortingOrder();
    }
  }

  public void ClearSelectedRows()
  {
    _checkBoxCheckedState.Clear();
  }

  /// <summary> Gets the <see cref="IBusinessObject"/> objects selected in the <see cref="BocList"/>. </summary>
  /// <returns> An array of <see cref="IBusinessObject"/> objects. </returns>
  public IBusinessObject[] GetSelectedBusinessObjects()
  {
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

  /// <summary> Gets indeces for the rows selected in the <see cref="BocList"/>. </summary>
  /// <returns> An array of <see cref="int"/> values. </returns>
  public int[] GetSelectedRows()
  {
    ArrayList selectedRows = new ArrayList();
    foreach (DictionaryEntry entry in _checkBoxCheckedState)
    {
      int rowIndex = (int) entry.Key;
      if (rowIndex == c_titleRowIndex)
        continue;

      bool isChecked = (bool) entry.Value;
      if (isChecked)
        selectedRows.Add (rowIndex);
    }
    return (int[]) selectedRows.ToArray (typeof (int));
  }

  /// <summary> Sets the <see cref="IBusinessObject"/> objects selected in the <see cref="BocList"/>. </summary>
  /// <param name="selectedObjects"> An <see cref="IList"/> of <see cref="IBusinessObject"/> objects. </param>>
  /// <exception cref="InvalidOperationException"> Thrown if the number of rows do not match the <see cref="Selection"/> mode.</exception>
  public void SetSelectedBusinessObjects (IList selectedObjects)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("selectedObjects", selectedObjects);
    ArgumentUtility.CheckItemsNotNullAndType ("selectedObjects", selectedObjects, typeof (IBusinessObject));
    
    if (Value == null)
      return;
    IList value = (IList) Value;

    try
    {
      ArrayList selectedRows = new ArrayList (selectedObjects.Count);
      foreach (IBusinessObject selectedObject in selectedObjects)
      {
        int index = value.IndexOf (selectedObject);
        if (index != -1)
          selectedRows.Add (index);
      }
      SetSelectedRows ((int[]) selectedRows.ToArray (typeof (int)));
    }
    catch (NotSupportedException)
    {
    }
  }

  /// <summary> Sets indeces for the rows selected in the <see cref="BocList"/>. </summary>
  /// <param name="selectedRows"> An array of <see cref="int"/> values. </param>
  /// <exception cref="InvalidOperationException"> Thrown if the number of rows do not match the <see cref="Selection"/> mode.</exception>
  public void SetSelectedRows (int[] selectedRows)
  {
    ClearSelectedRows();

    if (_selection == RowSelection.Disabled && selectedRows.Length > 0)
      throw new InvalidOperationException ("Cannot select rows if the BocList is set to RowSelection.Disabled.");

    if (_selection == RowSelection.Single && selectedRows.Length > 1)
      throw new InvalidOperationException ("Cannot select more than one row if the BocList is set to RowSelection.Single.");

    foreach (int rowIndex in selectedRows)
      _checkBoxCheckedState[rowIndex] = true;
  }

  /// <summary> Gets or sets a flag that determines wheter an empty list will still render its headers. </summary>
  /// <value> <see langword="false"/> to hide the headers if the list is empty. </value>
  [Category ("Behavior")]
  [Description ("Determines whether the list headers will be rendered if no data is provided.")]
  [DefaultValue (true)]
  public bool ShowEmptyList
  {
    get { return _showEmptyList; }
    set { _showEmptyList = value; }
  }

  /// <summary>
  ///   Gets or sets a flag that indicates whether the control automatically generates a column 
  ///   for each property of the bound object.
  /// </summary>
  /// <value> <see langword="true"/> show all properties of the bound business object. </value>
  [Category ("Appearance")]
  [Description ("Indicates whether the control automatically generates a column for each property of the bound object.")]
  [DefaultValue (false)]
  public bool ShowAllProperties
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
  public bool EnableIcon
  {
    get { return _enableIcon; }
    set
    { _enableIcon = value; }
  }

  /// <summary>
  ///   Gets or sets a flag that determines whether to to display a sorting button 
  ///   in front of each <see cref="BocValueColumnDefinition"/>'s header.
  /// </summary>
  /// <value> <see langword="true"/> to enable the sorting buttons. </value>
  [Category ("Behavior")]
  [Description ("Enables the sorting button in front of each value column's header.")]
  [DefaultValue (true)]
  public bool EnableSorting
  {
    get { return _enableSorting; }
    set { _enableSorting = value; }
  }

  /// <summary>
  ///   Gets or sets a flag that determines whether to display the sorting order index 
  ///   in front of each sorting button.
  /// </summary>
  /// <remarks> 
  ///   Only displays the index if more than one column is included in the sorting.
  /// </remarks>
  /// <value> <see langword="true"/> to show the sorting order index after the button. </value>
  [Category ("Appearance")]
  [Description ("Enables the sorting order display in front of each sorting button.")]
  [DefaultValue (false)]
  public bool ShowSortingOrder
  {
    get { return _showSortingOrder; }
    set { _showSortingOrder = value; }
  }

  /// <summary>
  ///   Gets or sets a flag that determines whether to display the options menu.
  /// </summary>
  /// <value> <see langword="true"/> to show the options menu. </value>
  [Category ("Menu")]
  [Description ("Enables the options menu.")]
  [DefaultValue (true)]
  public bool ShowOptionsMenu
  {
    get { return _showOptionsMenu; }
    set { _showOptionsMenu = value; }
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
  [DefaultValue (RowSelection.Disabled)]
  public RowSelection Selection
  {
    get { return _selection; }
    set { _selection = value; }
  }

  protected bool IsSelectionEnabled
  {
    get { return _selection != RowSelection.Disabled; }
  }

  /// <summary> The number of rows displayed per page. </summary>
  /// <value> 
  ///   An integer greater than zero to limit the number of rows per page to the specified value,
  ///   or zero, less than zero or <see cref="NaInt32.Null"/> to show all rows.
  /// </value>
  [Category ("Appearance")]
  [Description ("The number of rows displayed per page. Set PageSize to 0 to show all rows.")]
  [DefaultValue (typeof(NaInt32), "null")]
  public NaInt32 PageSize
  {
    get { return _pageSize; }
    set
    {
      if (value.IsNull || value.Value <= 0)
        _pageSize = NaInt32.Null;
      else
        _pageSize = value; 
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
  public bool AlwaysShowPageInfo
  {
    get { return _alwaysShowPageInfo; }
    set { _alwaysShowPageInfo = value; }
  }

  /// <summary> Gets or sets the text providing the current page information to the user. </summary>
  /// <remarks> Use {0} for the current page and {1} for the total page count. </remarks>
  [Category ("Appearance")]
  [Description ("The text providing the current page information to the user. Use {0} for the current page and {1} for the total page count.")]
  [DefaultValue ("Page {0} of {1}")]
  public string PageInfo
  {
    get { return _pageInfo; }
    set { _pageInfo = value; }
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
  public event BocCustomColumnClickEventHandler CustomColumnClick
  {
    add { Events.AddHandler (s_customColummClickEvent, value); }
    remove { Events.RemoveHandler (s_customColummClickEvent, value); }
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
  public BocMenuItemCollection OptionsMenuItems
  {
    get { return _optionsMenuItems; }
  }

  /// <summary> Gets the <see cref="BocMenuItem"/> objects displayed in the <see cref="BocList"/>'s menu area. </summary>
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Category ("Menu")]
  [Description ("The menu items displayed in the list's menu area.")]
  [DefaultValue ((string) null)]
  public BocMenuItemCollection ListMenuItems
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

  /// <summary>
  ///   Gets or sets a value that indicates whether the control displays a drop down list 
  ///   containing the available column definition sets.
  /// </summary>
  [Category ("Menu")]
  [Description ("Indicates whether the control displays a drop down list containing the available column definition sets.")]
  [DefaultValue (true)]
  public bool ShowAdditionalColumnsList
  {
    get { return _showAdditionalColumnsList; }
    set { _showAdditionalColumnsList = value; }
  }

  /// <summary> Gets or sets the text that is rendered as a title for the drop list of additional columns. </summary>
  [Category ("Menu")]
  [Description ("The text that is rendered as a title for the list of additional columns.")]
  [DefaultValue ("")]
  public string AdditionalColumnsTitle
  {
    get { return _additionalColumnsTitle; }
    set { _additionalColumnsTitle = value; }
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

  #region protected virtual string CssClass...
  /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s <c>table</c> tag. </summary>
  /// <remarks> Class: <c>bocListTable</c> </remarks>
  protected virtual string CssClassTable
  { get { return "bocListTable"; } }

  /// <summary> CSS-Class applied to the cells in the <see cref="BocList"/>'s title row. </summary>
  /// <remarks> Class: <c>bocListTitleCell</c> </remarks>
  protected virtual string CssClassTitleCell
  { get { return "bocListTitleCell"; } }

  /// <summary> Gets the CSS-Class applied to the cells in the <see cref="BocList"/>'s odd data rows. </summary>
  /// <remarks> Class: <c>bocListDataCellOdd</c> </remarks>
  protected virtual string CssClassDataCellOdd
  { get { return "bocListDataCellOdd"; } }

  /// <summary> Gets the CSS-Class applied to the cells in the <see cref="BocList"/>'s even data rows. </summary>
  /// <remarks> Class: <c>bocListDataCellEven</c> </remarks>
  protected virtual string CssClassDataCellEven
  { get { return "bocListDataCellEven"; } }

  /// <summary> Gets the CSS-Class applied to the cells in the <see cref="BocList"/>'s odd data rows when the row is selected. </summary>
  /// <remarks> Class: <c>bocListDataCellOddSelected</c> </remarks>
  protected virtual string CssClassDataCellOddSelected
  { get { return "bocListDataCellOddSelected"; } }

  /// <summary> Gets the CSS-Class applied to the cells in the <see cref="BocList"/>'s even data rows when the row is selected. </summary>
  /// <remarks> Class: <c>bocListDataCellEvenSelected</c> </remarks>
  protected virtual string CssClassDataCellEvenSelected
  { get { return "bocListDataCellEvenSelected"; } }

  /// <summary> Gets the CSS-Class applied to the text providing the sorting order's index. </summary>
  /// <remarks> Class: <c>bocListSortingOrder</c> </remarks>
  protected virtual string CssClassSortingOrder
  { get { return "bocListSortingOrder"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s navigator. </summary>
  /// <remarks> Class: <c>bocListNavigator</c> </remarks>
  protected virtual string CssClassNavigator
  { get { return "bocListNavigator"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s list of additional columns. </summary>
  /// <remarks> Class: <c>bocListAdditionalColumnsListDropDownList</c> </remarks>
  protected virtual string CssClassAdditionalColumnsListDropDownList
  { get { return "bocListAdditionalColumnsListDropDownList"; } }
  
  /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s label for the list of additional columns. </summary>
  /// <remarks> Class: <c>bocListAdditionalColumnsListLabel</c> </remarks>
  protected virtual string CssClassAdditionalColumnsListLabel
  { get { return "bocListAdditionalColumnsListLabel"; } }
  
  #endregion

  /// <summary>
  ///   Gets a flag that indicated whether the page is in post back mode.
  /// </summary>
  private bool IsPostBack
  {
    get { return !IsDesignMode && Page != null && Page.IsPostBack; }
  }
}

/// <summary> The possible sorting directions. </summary>
public enum SortingDirection
{
  /// <summary> Don't sort. </summary>
  None,
  /// <summary> Sort ascending. </summary>
  Ascending,
  /// <summary> Sort descending. </summary>
  Descending
}

public enum RowSelection
{ 
  Disabled = 0,
  Single = 1,
  Multiple = 2 
}

public enum ListMenuLineBreaks
{
  All,
  None,
  BetweenGroups
}

}