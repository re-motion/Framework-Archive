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

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <include file='doc\include\Controls\BocList.xml' path='BocList/Class/*' />
// TODO: see "Doc\Bugs and ToDos.txt"
[Designer (typeof (BocListDesigner))]
[DefaultEvent ("CommandClick")]
[ToolboxItemFilter("System.Web.UI")]
[ParseChildren (true)]
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
  private const string c_optionsMenuIDSuffix = "_Boc_optionsMenu";
  private const string c_optionsMenuGroupID = "BocList";

  private const int c_titleRowIndex = -1;

  /// <summary> Prefix applied to the post back argument of the event type command columns. </summary>
  private const string c_eventCommandPrefix = "Event=";

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

  private const string c_defaultMenuBlockWidth = "200px";
  private const string c_defaultMenuBlockOffset = "0px";

  /// <summary> 
  ///   Text displayed when control is displayed in desinger and is read-only has no contents.
  /// </summary>
  private const string c_designModeEmptyContents = "#";
  private const string c_designModeDummyColumnTitle = "Column Title {0}";
  private const int c_designModeDummyColumnCount = 3;

  private const int c_designModeAdditionalColumnsListWidthInPoints = 70;

  // types
  
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

  /// <summary> The possible sorting directions. </summary>
  private enum SortingDirection
  {
    /// <summary> Don't sort. </summary>
    None,
    /// <summary> Sort ascending. </summary>
    Ascending,
    /// <summary> Sort descending. </summary>
    Descending
  }

  /// <summary> Represents the sorting direction for an individual column. </summary>
  [Serializable]
  private struct SortingOrderEntry
  {
    private int _columnIndex;
    private SortingDirection _direction;

    /// <summary> Represents a null <see cref="SortingOrderEntry"/>. </summary>
    public static readonly SortingOrderEntry Empty = 
        new SortingOrderEntry (Int32.MinValue, SortingDirection.None);

    /// <summary>
    ///   Initializes a new instance of the <see cref="SortingOrderEntry"/> class with
    ///   a column index and the <see cref="SortingDirection"/> for this column.
    /// </summary>
    /// <include file='doc\include\Controls\BocList.xml' path='BocList/SortingOrderEntry/Constructor/*' />
    public SortingOrderEntry (int columnIndex, SortingDirection direction)
    {
      _columnIndex = columnIndex;
      _direction = direction;
    }

    /// <summary>
    ///   Tests whether two specified <see cref="SortingOrderEntry"/> structures are equivalent.
    /// </summary>
    /// <include file='doc\include\Controls\BocList.xml' path='BocList/SortingOrderEntry/OperatorEqual/*' />
    public static bool operator == (SortingOrderEntry left, SortingOrderEntry right)
    {
      return left.ColumnIndex == right.ColumnIndex && left.Direction == right.Direction;
    }

    /// <summary>
    ///   Tests whether two specified <see cref="SortingOrderEntry"/> structures are different.
    /// </summary>
    /// <include file='doc\include\Controls\BocList.xml' path='BocList/SortingOrderEntry/OperatorDifferent/*' />
    public static bool operator != (SortingOrderEntry left, SortingOrderEntry right)
    {
      return !(left == right);
    }

    /// <summary>
    ///   Tests whether the specified object is a <see cref="SortingOrderEntry"/> structure 
    ///   and is equivalent to this <see cref="SortingOrderEntry"/> structure.
    /// </summary>
    /// <include file='doc\include\Controls\BocList.xml' path='BocList/SortingOrderEntry/Equals/*' />
    public override bool Equals (object obj)
    {
      if (obj is SortingOrderEntry)
      {
        SortingOrderEntry entry = (SortingOrderEntry) obj;
        return _columnIndex == entry.ColumnIndex && _direction == entry.Direction;;
      }
      return false;
    }

    /// <summary>
    ///   Returns a hash code for this <see cref="SortingOrderEntry"/> structure.
    /// </summary>
    /// <include file='doc\include\Controls\BocList.xml' path='BocList/SortingOrderEntry/GetHashCode/*' />
    public override int GetHashCode()
    {
      return _columnIndex.GetHashCode() ^ _direction.GetHashCode();
    }

    /// <summary>
    ///   Gets or sets the index of the column for which the <see cref="Direction"/> is entered.
    /// </summary>
    public int ColumnIndex
    {
      get { return _columnIndex; }
      set { _columnIndex = value; }
    }

    /// <summary>
    ///   Gets or sets the <see cref="SortingDirection"/> for the column at <see cref="ColumnIndex"/>.
    /// </summary>
    public SortingDirection Direction
    {
      get { return _direction; }
      set { _direction = value; }
    }
  }

  // static members
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectReferenceProperty) };
  
  /// <summary> The log4net logger. </summary>
  private static readonly log4net.ILog s_log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

  private static readonly object EventCommandClick = new object();

	// member fields

  /// <summary>
  ///   <see langword="true"/> if <see cref="Value"/> has been changed since last call to
  ///   <see cref="SaveValue"/>.
  /// </summary>
  private bool _isDirty = true;

  /// <summary> The <see cref="DropDownList"/> used to select the column configuration. </summary>
  private DropDownList _additionalColumnsList = new DropDownList();

  /// <summary> 
  ///   The <see cref="string"/> that is rendered in front of the <see cref="_additionalColumnsList"/>.
  /// </summary>
  private string _additionalColumnsTitle = "Additional Columns";

  /// <summary> The width applied to the <see cref="_additionalColumnsList"/>. </summary>
  private Unit _additionalColumnsListWidth = Unit.Empty;

  /// <summary> The <see cref="ImageButton"/> used to navigate to the first page. </summary>
  private ImageButton _moveFirstButton = new ImageButton();
  /// <summary> The <see cref="ImageButton"/> used to navigate to the last page. </summary>
  private ImageButton _moveLastButton =  new ImageButton();
  /// <summary> The <see cref="ImageButton"/> used to navigate to the previous page. </summary>
  private ImageButton _movePreviousButton = new ImageButton();
  /// <summary> The <see cref="ImageButton"/> used to navigate to the next page. </summary>
  private ImageButton _moveNextButton = new ImageButton();

  /// <summary> The <see cref="IList"/> displayed by the <see cref="BocList"/>. </summary>
  private IList _value = null;

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

  private DropDownMenu _optionsMenu;
  private string _optionsTitle = "Options";
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

  /// <summary> The predefined column defintion sets that the user can choose from at run-time. </summary>
  private BocColumnDefinitionSetCollection _availableColumnDefinitionSets;
  /// <summary> 
  ///   Determines whether to show the drop down list for selecting additional column definitions. 
  /// </summary>
  private bool _showAdditionalColumnsList = true;
  /// <summary> The current <see cref="BocColumnDefinitionSet"/>. May be set at run time. </summary>
  private BocColumnDefinitionSet _selectedColumnDefinitionSet;
  /// <summary> 
  ///   The zero-based index of the <see cref="BocColumnDefinitionSet"/> selected from 
  ///   <see cref="AvailableColumnDefinitionSets"/>.
  /// </summary>
  private int _selectedColumnDefinitionSetIndex = -1;
 
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
  /// <summary> Contains the row indices of the business objects in the list, before the objects are sorted. </summary>
  private Hashtable _originalRowIndices = null;

  /// <summary> Determines whether the options menu is shown. </summary>
  private bool _showOptionsMenu = true;

  /// <summary> Determines whether to enable the selecting of the data rows. </summary>
  private bool _enableSelection = false;
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
  private string _pageInfo = "Page {0} of {1}";
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
    //  Reference 'this' is not used for anything but storing the reference to the BocList
    _optionsMenu = new DropDownMenu (c_optionsMenuGroupID, this);
    //  Reference 'this' is not used for anything but storing the reference to the BocList
    _listMenuItems = new BocMenuItemCollection (this);
    //  Reference 'this' is not used for anything but storing the reference to the BocList
    _optionsMenuItems = new BocMenuItemCollection (this);
    //  Reference 'this' is not used for anything but storing the reference to the BocList
    _fixedColumns = new BocColumnDefinitionCollection (this);
    //  Reference 'this' is not used for anything but storing the reference to the BocList
    _availableColumnDefinitionSets = new BocColumnDefinitionSetCollection (this);
  }

	// methods and properties

  /// <summary>
  ///   Calls the parent's <c>OnInit</c> method and initializes this control's sub-controls.
  /// </summary>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);

    _additionalColumnsList.ID = this.ID + c_additionalColumnsListIDSuffix;
    _additionalColumnsList.EnableViewState = true;
    _additionalColumnsList.AutoPostBack = true;
    _additionalColumnsList.SelectedIndexChanged += new EventHandler(AdditionalColumnsList_SelectedIndexChanged);
    Controls.Add (_additionalColumnsList);

    _optionsMenu.ID = this.ID + c_optionsMenuIDSuffix;
    Controls.Add (_optionsMenu);

    _moveFirstButton.Click += new ImageClickEventHandler (MoveFirstButton_Click);
    Controls.Add (_moveFirstButton);
    _moveLastButton.Click += new ImageClickEventHandler (MoveLastButton_Click);
    Controls.Add (_moveLastButton);
    _movePreviousButton.Click += new ImageClickEventHandler (MovePreviousButton_Click);
    Controls.Add (_movePreviousButton);
    _moveNextButton.Click += new ImageClickEventHandler (MoveNextButton_Click);
    Controls.Add (_moveNextButton);

    Binding.BindingChanged += new EventHandler (Binding_BindingChanged);

    if (! IsPostBack)
      PopulateAdditionalColumnsList();
    _availableColumnDefinitionSets.CollectionChanged += new CollectionChangeEventHandler(AvailableColumnDefinitionSets_CollectionChanged);
    
    if (IsPostBack && Page != null)
    {
      string dataRowCheckBoxFilter = ID + c_dataRowCheckBoxIDSuffix;
      string titleRowCheckBoxFilter = ID + c_titleRowCheckBoxIDSuffix;

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
            int rowIndex = int.Parse (formVariables[i]);
            _checkBoxCheckedState[rowIndex] = true; 
          }
        }
      }
    }
  }

  /// <summary> Implements interface <see cref="IPostBackEventHandler"/>. </summary>
  /// <param name="eventArgument"> &lt;prefix&gt;=&lt;value&gt; </param>
  public void RaisePostBackEvent (string eventArgument)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);

    eventArgument = eventArgument.Trim();

    if (eventArgument.StartsWith (c_eventCommandPrefix))
      HandleEventCommand (eventArgument.Substring (c_eventCommandPrefix.Length));
    else if (eventArgument.StartsWith (c_sortCommandPrefix))
      HandleResorting (eventArgument.Substring (c_sortCommandPrefix.Length));
    else
      throw new ArgumentException ("Argument 'eventArgument' must begin with on of the following prefixes: '" + c_eventCommandPrefix + "' or '" + c_sortCommandPrefix + "'.");
  }

  /// <summary> Handles post back events raised by an <see cref="Command"/> of type event. </summary>
  /// <param name="eventArgument">
  ///   &lt;column-index&gt;,&lt;list-index&gt;[,&lt;business-object-id&gt;]
  /// </param>
  private void HandleEventCommand (string eventArgument)
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
      throw new ArgumentException ("First part of argument 'eventArgument' must be an integer. Expected format: '<column-index>,<list-index>[,<business-object-id>]'.");
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
      throw new ArgumentException ("Second part of argument 'eventArgument' must be an integer. Expected format: <column-index>,<list-index>[,<business-object-id>]'.");
    }
    
    //  Third part, optional: business object ID
    string businessObjectID = null;
    if (eventArgumentParts.Length == 3)
      businessObjectID = eventArgumentParts[2].Trim();

    BocColumnDefinition[] columns = EnsureGetColumnsForPreviousLifeCycle();

    if (columnIndex >= columns.Length)
      throw new ArgumentOutOfRangeException ("Column index of argument 'eventargument' was out of the range of valid values. Index must be less than the number of displayed columns.'");

    BocColumnDefinition column = columns[columnIndex];
    if (column.Command == null)
      throw new ArgumentOutOfRangeException ("The BocList '" + ID + "' does not have a command inside column " + columnIndex + ".");
    BocListItemCommand command = column.Command;

    switch (command.Type)
    {
      case CommandType.Event:
      {
        string columnID = string.Empty;
        if (column != null)
        {
          if (StringUtility.IsNullOrEmpty (column.ColumnID))
            throw new InvalidOperationException ("The column No. " + columnIndex + " does not have an ID but raised an event.");
          columnID = column.ColumnID;
        }

        OnCommandClick (columnID, listIndex, businessObjectID);
        break;
      }
      case CommandType.WxeFunction:
      {
        command.ExecuteWxeFunction ((WxePage) this.Page, listIndex, (IBusinessObject) this.Value[listIndex], businessObjectID);
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

    BocColumnDefinition[] columns = EnsureGetColumnsForPreviousLifeCycle();

    if (columnIndex >= columns.Length)
      throw new ArgumentOutOfRangeException ("Column index of argument 'eventargument' was out of the range of valid values. Index must be less than the number of displayed columns.'");
    if (! (columns[columnIndex] is BocValueColumnDefinition))
      throw new ArgumentOutOfRangeException ("The BocList '" + ID + "' does not have a value column at index" + columnIndex + ".");

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
    if (sortingOrderEntry != SortingOrderEntry.Empty)
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
          sortingOrderEntry.Direction = SortingDirection.None;
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

    _sortingOrder.Add (sortingOrderEntry);
  }

  /// <summary> Fires the <see cref="CommandClick"/> event. </summary>
  /// <include file='doc\include\Controls\BocList.xml' path='BocList/OnCommandClick/*' />
  protected virtual void OnCommandClick (string columnID, int listIndex, string businessObjectID)
  {
    BocCommandClickEventHandler commandClickHandler = (BocCommandClickEventHandler) Events[EventCommandClick];
    if (commandClickHandler != null)
    {
      BocCommandClickEventArgs e = new BocCommandClickEventArgs (columnID, listIndex, businessObjectID);
      commandClickHandler (this, e);
    }
  }

  /// <summary> Overrides the parent's <c>OnPreRender</c> method. </summary>
  /// <remarks>  
  ///   Calculates the page count depending on an optional move command, 
  ///   and registeres the client scripts.
  /// </remarks>
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

    if (_showOptionsMenu)
      _optionsMenu.MenuItems.AddRange (EnsureGetOptionsMenuItemsForCurrentLifeCycle());
     
    base.OnPreRender (e);
  }

  /// <summary>
  ///   Overrides the parent's <c>Render</c> method.
  /// </summary>
  /// <param name="writer"> 
  ///   The <see cref="HtmlTextWriter"/> object that receives the server control content.
  /// </param>
  protected override void Render (HtmlTextWriter writer)
  {
    if (Page != null)
      Page.VerifyRenderingInServerForm(this);

    BocColumnDefinition[] renderColumns = EnsureGetColumnsForCurrentLifeCycle();

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
    writer.AddStyleAttribute ("display", "inline");
    writer.AddStyleAttribute ("float", "right");
    writer.AddStyleAttribute ("vertical-align", "top");
    string menuBlockWidth = c_defaultMenuBlockWidth;
    if (! _menuBlockWidth.IsEmpty)
      menuBlockWidth = _menuBlockWidth.ToString();
    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, menuBlockWidth);      
    writer.RenderBeginTag (HtmlTextWriterTag.Div);
    RenderMenuBlock (writer);
    writer.RenderEndTag();
    
    writer.AddStyleAttribute ("display", "inline");
    writer.AddStyleAttribute ("vertical-align", "top");
    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "auto");
    writer.RenderBeginTag (HtmlTextWriterTag.Div);
    RenderTableBlock (writer);
    writer.RenderEndTag();
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

    writer.RenderEndTag();  //  End table-row
    writer.RenderEndTag();  //  End table
  }

  /// <remarks>
  ///   Use display:table, display:table-row, ... for opera and mozilla/firefox
  /// </remarks>
  private void RenderTableAndMenuLegacyBrowser (HtmlTextWriter writer)
  {
    //  Render list block / menu block
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
      menuBlockOffset = _menuBlockWidth.ToString();
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

    //  Menu Block
    writer.AddStyleAttribute ("vertical-align", "top");
    writer.RenderBeginTag (HtmlTextWriterTag.Td);
    RenderMenuBlock (writer);
    writer.RenderEndTag();

    writer.RenderEndTag();  //  TR

    writer.RenderEndTag();  //  Table
  }

  /// <summary> Renders the menu block of the control. </summary>
  /// <remarks> 
  ///   Contains the drop down list for selcting a column configuration 
  ///   and the options menu. 
  /// </remarks>
  /// <param name="writer"> 
  ///   The <see cref="HtmlTextWriter"/> object that receives the server control content.
  /// </param>
  private void RenderMenuBlock (HtmlTextWriter writer)
  {
    if (_showAdditionalColumnsList)
    {
//      writer.AddStyleAttribute ("position", "relative");
//      writer.AddStyleAttribute ("z-index", "0");
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      writer.RenderBeginTag (HtmlTextWriterTag.Div);
      writer.Write (_additionalColumnsTitle + c_whiteSpace);
      if (IsDesignMode && _additionalColumnsListWidth.IsEmpty)
        _additionalColumnsList.Width = Unit.Point (c_designModeAdditionalColumnsListWidthInPoints);
//      _additionalColumnsList.Style["z-index"] = "0";
      _additionalColumnsList.RenderControl (writer);
      writer.RenderEndTag();
    }

    if (_showOptionsMenu)
    {
      _optionsMenu.TitleText = _optionsTitle;
      _optionsMenu.RenderControl (writer);
    }

    #region Temporay filling material
//    writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundColor, "#ffffcc");
//    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "80%");
//    writer.RenderBeginTag (HtmlTextWriterTag.Div);
//    writer.Write ("Other stuff 1");
//    writer.RenderEndTag();
//    writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundColor, "#ffffcc");
//    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "80%");
//    writer.RenderBeginTag (HtmlTextWriterTag.Div);
//    writer.Write ("Other stuff 2");
//    writer.RenderEndTag();
//    writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundColor, "#ffffcc");
//    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "80%");
//    writer.RenderBeginTag (HtmlTextWriterTag.Div);
//    writer.Write ("Other stuff 3");
//    writer.RenderEndTag();
//    writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundColor, "#ffffcc");
//    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "80%");
//    writer.RenderBeginTag (HtmlTextWriterTag.Div);
//    writer.Write ("Other stuff 4");
//    writer.RenderEndTag();
//    writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundColor, "#ffffcc");
//    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "80%");
//    writer.RenderBeginTag (HtmlTextWriterTag.Div);
//    writer.Write ("Other stuff 5");
//    writer.RenderEndTag();
//    writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundColor, "#ffffcc");
//    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "80%");
//    writer.RenderBeginTag (HtmlTextWriterTag.Div);
//    writer.Write ("Other stuff 6");
//    writer.RenderEndTag();
//    writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundColor, "#ffffcc");
//    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "80%");
//    writer.RenderBeginTag (HtmlTextWriterTag.Div);
//    writer.Write ("Other stuff 7");
//    writer.RenderEndTag();
//    writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundColor, "#ffffcc");
//    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "80%");
//    writer.RenderBeginTag (HtmlTextWriterTag.Div);
//    writer.Write ("Other stuff 8");
//    writer.RenderEndTag();
    #endregion
  }

  /// <summary> Renders the list of values as an <c>table</c>. </summary>
  /// <param name="writer"> 
  ///   The <see cref="HtmlTextWriter"/> object that receives the server control content.
  /// </param>
  private void RenderTableBlock (HtmlTextWriter writer)
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

    if (Value != null)
    {
      bool isOddRow = true;

      ArrayList rows = new ArrayList (Value);

      _originalRowIndices = null;
      if (EnableSorting)
      {
        _originalRowIndices = new Hashtable();
        for (int idxRows = 0; idxRows < totalRowCount; idxRows++)
        {
          if (rows[idxRows] == null)
            continue;
          _originalRowIndices.Add (rows[idxRows], idxRows);
        }
        rows.Sort (this);
      }
      for (int idxSortedRows = firstRow, idxRows = 0; 
          idxSortedRows < rowCountWithOffset; 
          idxSortedRows++, idxRows++)
      {
        if (rows[idxSortedRows] == null)
          throw new NullReferenceException ("Null item found in IList 'Value' of BocList " + ID + ".");
        IBusinessObject businessObject = rows[idxSortedRows] as IBusinessObject;
        int originalRowIndex = idxSortedRows;
        if (EnableSorting)
          originalRowIndex = (int) _originalRowIndices[businessObject];
        if (businessObject == null)
          throw new InvalidCastException ("List item " + originalRowIndex + " in IList 'Value' of BocList " + ID + " is not of type IBusinessObject.");
        RenderDataRow (writer, businessObject, idxRows, originalRowIndex, isOddRow);
        isOddRow = !isOddRow;
      }
    }

    //  Close the table
    RenderTableClosingTag (writer);

        if (_hasClientScript && _enableSelection)
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
          + count.ToString() + ");"
          + "\r\n//-->\r\n</script>";
      writer.Write (script);
    }
  }

  /// <summary> 
  ///   Renders the navigation bar consisting of the move buttons and the <see cref="PageInfo"/>.
  /// </summary>
  /// <param name="writer"> 
  ///   The <see cref="HtmlTextWriter"/> object that receives the server control content.
  /// </param>
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
    writer.Write (_pageInfo, _currentPage + 1, _pageCount);
    writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);
    
    string imageUrl = null;

    //  Move to first page button
    if (isFirstPage)
      imageUrl = c_moveFirstInactiveIcon;
    else
      imageUrl = c_moveFirstIcon;      
    imageUrl = ResourceUrlResolver.GetResourceUrl (
      this, Context, typeof (BocList), ResourceType.Image, imageUrl);
    if (isFirstPage)
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
    if (isFirstPage)
      imageUrl = c_movePreviousInactiveIcon;
    else
      imageUrl = c_movePreviousIcon;      
    imageUrl = ResourceUrlResolver.GetResourceUrl (
      this, Context, typeof (BocList), ResourceType.Image, imageUrl);
    if (isFirstPage)
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
    if (isLastPage)
      imageUrl = c_moveNextInactiveIcon;
    else
      imageUrl = c_moveNextIcon;      
    imageUrl = ResourceUrlResolver.GetResourceUrl (
      this, Context, typeof (BocList), ResourceType.Image, imageUrl);
    if (isLastPage)
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
    if (isLastPage)
      imageUrl = c_moveLastInactiveIcon;
    else
      imageUrl = c_moveLastIcon;     
    imageUrl = ResourceUrlResolver.GetResourceUrl (
      this, Context, typeof (BocList), ResourceType.Image, imageUrl);
    if (isLastPage)
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
  /// <param name="writer"> 
  ///   The <see cref="HtmlTextWriter"/> object that receives the server control content.
  /// </param>
  private void RenderTableOpeningTag (HtmlTextWriter writer)
  {
    //  TODO: Browser Switch: Css2.1: 100%, IE 5.01: inherit
    if (! (writer is Html32TextWriter))
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "inherit");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTable);
    writer.RenderBeginTag (HtmlTextWriterTag.Table);
  }

  /// <summary> Renderes the closing tag of the table. </summary>
  /// <param name="writer"> 
  ///   The <see cref="HtmlTextWriter"/> object that receives the server control content.
  /// </param>
  private void RenderTableClosingTag (HtmlTextWriter writer)
  {
    writer.RenderEndTag();
  }

  /// <summary> Renderes the column group, which provides the table's column layout. </summary>
  /// <param name="writer"> 
  ///   The <see cref="HtmlTextWriter"/> object that receives the server control content.
  /// </param>
  private void RenderColGroup (HtmlTextWriter writer)
  {
    BocColumnDefinition[] renderColumns = EnsureGetColumnsForCurrentLifeCycle();

    writer.RenderBeginTag (HtmlTextWriterTag.Colgroup);

    if (EnableSelection)
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Unit.Percentage(0).ToString());
      writer.RenderBeginTag (HtmlTextWriterTag.Col);
      writer.RenderEndTag();
    }

    bool isFirstColumnUndefinedWidth = true;
    foreach (BocColumnDefinition column in renderColumns)
    {
      if (! column.Width.IsEmpty)
      {
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, column.Width.ToString());
      }
      else 
      {
        if (isFirstColumnUndefinedWidth)
        {
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Unit.Percentage(1).ToString());
          isFirstColumnUndefinedWidth = false;
        }
        else
        {
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "auto");
        }
      }

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
  /// <remarks> 
  ///   Title format: &lt;span&gt;label button &lt;span&gt;sort order&lt;/span&gt;&lt;/span&gt;
  /// </remarks>
  /// <param name="writer"> 
  ///   The <see cref="HtmlTextWriter"/> object that receives the server control content.
  /// </param>
  private void RenderColumnTitlesRow (HtmlTextWriter writer)
  {
    bool isReadOnly = IsReadOnly;
    BocColumnDefinition[] renderColumns = EnsureGetColumnsForCurrentLifeCycle();

    writer.RenderBeginTag (HtmlTextWriterTag.Tr);

    if (EnableSelection)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTitleCell);
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      string checkBoxName = ID + c_titleRowCheckBoxIDSuffix;
      bool isChecked = (_checkBoxCheckedState[c_titleRowIndex] != null);
      RenderCheckBox (writer, checkBoxName, c_titleRowIndex.ToString(), isChecked, true);
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

    for (int idxColumn = 0; idxColumn < renderColumns.Length; idxColumn++)
    {
      BocColumnDefinition column = renderColumns[idxColumn];

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTitleCell);
      writer.RenderBeginTag (HtmlTextWriterTag.Td);

      // Click on Label or Button toggles direction -> span-tag around both
      bool hasSortingButton =    EnableSorting 
                              && column is BocValueColumnDefinition;
      if (hasSortingButton)
      {
        string argument = c_sortCommandPrefix + idxColumn.ToString();
        string postBackScript = Page.GetPostBackClientHyperlink (this, argument);
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackScript);
        writer.RenderBeginTag (HtmlTextWriterTag.Span);
      }
      if (IsDesignMode && column.ColumnTitleDisplayValue.Length == 0)
      {
        writer.Write (c_designModeEmptyContents);
      }
      else
      {
        string contents = HttpUtility.HtmlEncode (column.ColumnTitleDisplayValue);
        if (contents == string.Empty)
          contents = c_whiteSpace;
        writer.Write (contents);
      }

      if (hasSortingButton)
      {
        object obj = sortingDirections[idxColumn];
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

        if (_showSortingOrder && sortingDirection != SortingDirection.None)
        {
          //  WhiteSpace before icon
          writer.Write (c_whiteSpace);
          writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassSortingOrder);
          writer.RenderBeginTag (HtmlTextWriterTag.Span);

          writer.AddAttribute (HtmlTextWriterAttribute.Src, imageUrl);
          writer.AddStyleAttribute ("vertical-align", "middle");
          writer.RenderBeginTag (HtmlTextWriterTag.Img);
          writer.RenderEndTag();

          if (sortingOrder.Count > 1)
          {
            int orderIndex = sortingOrder.IndexOf (idxColumn);
            writer.Write (c_whiteSpace + (orderIndex + 1).ToString());
          }
          writer.RenderEndTag();
        }

        writer.RenderEndTag();  //  span
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
  /// <param name="writer"> 
  ///   The <see cref="HtmlTextWriter"/> object that receives the server control content.
  /// </param>
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
    BocColumnDefinition[] renderColumns = EnsureGetColumnsForCurrentLifeCycle();

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

    if (_enableSelection)
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

    if (_enableSelection)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableCell);
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      RenderCheckBox (writer, checkBoxID, originalRowIndex.ToString(), isChecked, false);
      writer.RenderEndTag();
    }

    bool isFirstValueColumnRendered = false;

    for (int idxColumn = 0; idxColumn < renderColumns.Length; idxColumn++)
    {
      BocColumnDefinition column = renderColumns[idxColumn];

      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableCell);
      writer.RenderBeginTag (HtmlTextWriterTag.Td);

      BocCommandColumnDefinition commandColumn = column as BocCommandColumnDefinition;
      BocCompoundColumnDefinition compoundColumn = column as BocCompoundColumnDefinition;
      BocSimpleColumnDefinition simpleColumn = column as BocSimpleColumnDefinition;
      BocValueColumnDefinition valueColumn = column as BocValueColumnDefinition;

      bool isFirstValueColumn = valueColumn != null && !isFirstValueColumnRendered;

      //  Render the command
      bool isCommandEnabled = false;
      if (column.Command != null)
      {
        bool isActive =    column.Command.Show == CommandShow.Always
                        || isReadOnly && column.Command.Show == CommandShow.ReadOnly
                        || ! isReadOnly && column.Command.Show == CommandShow.EditMode;
        if (   isActive
            && column.Command.Type != CommandType.None)
        {
          isCommandEnabled = true;
        }
      }

      if (isCommandEnabled)
      {    
        string argument = c_eventCommandPrefix + idxColumn + "," + originalRowIndex;
        if (businessObjectWithIdentity != null)
          argument += "," + businessObjectWithIdentity.UniqueIdentifier; 
        string postBackLink = Page.GetPostBackClientHyperlink (this, argument);
        string onClick = "BocList_OnCommandClick();";
        column.Command.RenderBegin (writer, postBackLink, onClick, originalRowIndex, objectID);
      }

      //  Render the icon
      if (EnableIcon && isFirstValueColumn)
      {
        IBusinessObjectService service
          = businessObject.BusinessObjectClass.BusinessObjectProvider.GetService(
            typeof (IBusinessObjectWebUIService));

        IBusinessObjectWebUIService webUIService = service as IBusinessObjectWebUIService;

        IconInfo icon = null;
        if (webUIService != null)
          icon = webUIService.GetIcon (businessObject);

        if (icon != null)
        {
          writer.AddAttribute (HtmlTextWriterAttribute.Src, icon.Url);
          if (! icon.Width.IsEmpty && ! icon.Height.IsEmpty)
          {
            writer.AddAttribute (HtmlTextWriterAttribute.Width, icon.Width.ToString());
            writer.AddAttribute (HtmlTextWriterAttribute.Width, icon.Height.ToString());
          }
          writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
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

        if (commandColumn.Label != null)
          writer.Write (commandColumn.Label);
      }
      else if (compoundColumn != null)
      {
        string contents = compoundColumn.GetStringValue (businessObject);
        contents = HttpUtility.HtmlEncode (contents);
        if (contents == string.Empty)
          contents = c_whiteSpace;
        writer.Write (contents);
      }
      else if (simpleColumn != null)
      {
        string contents = simpleColumn.GetStringValue (businessObject);
        contents = HttpUtility.HtmlEncode (contents);
        if (contents == string.Empty)
          contents = c_whiteSpace;
        writer.Write (contents);
      }

      if (isCommandEnabled)
        column.Command.RenderEnd (writer);

      if (isFirstValueColumn)
        isFirstValueColumnRendered = true;
      writer.RenderEndTag();
    }
    
    writer.RenderEndTag();
  }

  /// <summary> Renders a <see cref="CheckBox"/> used for row selection. </summary>
  /// <param name="writer"> 
  ///   The <see cref="HtmlTextWriter"/> object that receives the server control content.
  /// </param>
  /// <param name="id"> 
  ///   The <see cref="string"/> rendered into the <c>id</c> and <c>name</c> attributes.
  /// </param>
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

      string script = "BocList_OnSelectAllCheckBoxClick ("
          + "document.getElementById ('" + ClientID + "'), "
          + "this , '"
          + ClientID + c_dataRowCheckBoxIDSuffix + "', "
          + count.ToString() + ");";
      writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
    }
    else
    {
      string script = "BocList_OnSelectionCheckBoxClick();";
      writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
    }
    writer.RenderBeginTag (HtmlTextWriterTag.Input);
    writer.RenderEndTag();
  }

  /// <summary>
  ///   Calls the parent's <c>LoadViewState</c> method and restores this control's specific data.
  /// </summary>
  /// <param name="savedState">
  ///   An <see cref="Object"/> that represents the control state to be restored.
  /// </param>
  protected override void LoadViewState(object savedState)
  {
    object[] values = (object[]) savedState;
    
    base.LoadViewState (values[0]);
    SelectedColumnDefinitionSetIndex = (int) values[1];
    _currentRow = (int) values[2];
    _sortingOrder = (ArrayList) values[3];
    _isDirty = (bool) values[4];
  }

  /// <summary>
  ///   Calls the parent's <c>SaveViewState</c> method and saves this control's specific data.
  /// </summary>
  /// <returns>
  ///   Returns the server control's current view state.
  /// </returns>
  protected override object SaveViewState()
  {
    object[] values = new object[5];

    values[0] = base.SaveViewState();
    values[1] = _selectedColumnDefinitionSetIndex;
    values[2] = _currentRow;
    values[3] = _sortingOrder;
    values[4] = _isDirty;

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
    {
      DataSource.BusinessObject.SetProperty (Property, Value);

      //  get_Value parses the internal representation of the date/time value
      //  set_Value updates the internal representation of the date/time value
      Value = Value;
    }
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
      column.PropertyPath = new BusinessObjectPropertyPath (new IBusinessObjectProperty[] {property});
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
        _selectedColumnDefinitionSetIndex = -1;
      }
      else if (   _selectedColumnDefinitionSetIndex < 0
               && _availableColumnDefinitionSets.Count > 0)
      {
        _selectedColumnDefinitionSetIndex = 0;
      }

      SelectedColumnDefinitionSetIndex = _selectedColumnDefinitionSetIndex;
    }
  }

  /// <summary>
  ///   Handles the <see cref="ListControl.SelectedIndexChanged"/> event of the 
  ///   <see cref="_additionalColumnsList"/>.
  /// </summary>
  private void AdditionalColumnsList_SelectedIndexChanged (object sender, EventArgs e)
  {
    SelectedColumnDefinitionSetIndex = _additionalColumnsList.SelectedIndex;
  }

  /// <summary>
  ///   Handles the <see cref="BocColumnDefinitionSetCollection.CollectionChanged"/> event of the 
  ///   <see cref="AvailableColumnDefinitionSets"/> collection.
  /// </summary>
  private void AvailableColumnDefinitionSets_CollectionChanged(object sender, CollectionChangeEventArgs e)
  {
    PopulateAdditionalColumnsList();
  }

  /// <summary>
  ///   Handles the <see cref="ImageButton.Click"/> event of the <see cref="_moveFirstButton"/>.
  /// </summary>
  private void MoveFirstButton_Click(object sender, ImageClickEventArgs e)
  {
    _move = MoveOption.First;
  }

  /// <summary>
  ///   Handles the <see cref="ImageButton.Click"/> event of the <see cref="_moveLastButton"/>.
  /// </summary>
  private void MoveLastButton_Click(object sender, ImageClickEventArgs e)
  {
    _move = MoveOption.Last;
  }
  
  /// <summary>
  ///   Handles the <see cref="ImageButton.Click"/> event of the <see cref="_movePreviousButton"/>.
  /// </summary>
  private void MovePreviousButton_Click(object sender, ImageClickEventArgs e)
  {
    _move = MoveOption.Previous;
  }
  
  /// <summary>
  ///   Handles the <see cref="ImageButton.Click"/> event of the <see cref="_moveNextButton"/>.
  /// </summary>
  private void MoveNextButton_Click(object sender, ImageClickEventArgs e)
  {
    _move = MoveOption.Next;
  }

  private BocColumnDefinition[] EnsureGetColumnsForPreviousLifeCycle()
  {
    if (_columnDefinitionsPostBackEventHandlingPhase == null)
      _columnDefinitionsPostBackEventHandlingPhase = GetColumns (true);
    return _columnDefinitionsPostBackEventHandlingPhase;
  }

  private BocColumnDefinition[] EnsureGetColumnsForCurrentLifeCycle()
  {
    if (_columnDefinitionsRenderPhase == null)
      _columnDefinitionsRenderPhase = GetColumns (false);
    return _columnDefinitionsRenderPhase;
  }

  /// <summary>
  ///   Compiles the <see cref="BocColumnDefinition"/> objects from the <see cref="FixedColumns"/>,
  ///   the <see cref="_allPropertyColumns"/> and the <see cref="SelectedColumnDefinitionSet"/>
  ///   into a single array.
  /// </summary>
  /// <returns> An array of <see cref="BocColumnDefinition"/> objects. </returns>
  private BocColumnDefinition[] GetColumns (bool isPostBackEventPhase)
  {
    BocColumnDefinition[] allPropertyColumns = null;
    if (_showAllProperties)
      allPropertyColumns = GetAllPropertyColumns();
    else
      allPropertyColumns = new BocColumnDefinition[0];

    BocColumnDefinition[] selectedColumns = null;
    if (_selectedColumnDefinitionSet != null)
      selectedColumns = _selectedColumnDefinitionSet.ColumnDefinitionCollection.ToArray();
    else
      selectedColumns = new BocColumnDefinition[0];

    BocColumnDefinition[] columnDefinitions = (BocColumnDefinition[]) ArrayUtility.Combine (
      _fixedColumns.ToArray(),
      allPropertyColumns,
      selectedColumns);

    if (isPostBackEventPhase)
      return GetColumnsForPreviousLifeCycle (columnDefinitions);
    else
      return GetColumnsForCurrentLifeCycle (columnDefinitions);
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
    return EnsureGetColumnsForCurrentLifeCycle();
  }

  /// <summary>
  ///   Override this method to modify the column definitions displayed in the <see cref="BocList"/> in the
  ///   current page life cycle.
  /// </summary>
  /// <param name="columnDefinitions"> 
  ///   The <see cref="BocColumnDefinition"/> array containing the columns defined by the <see cref="BocList"/>. 
  /// </param>
  /// <returns> The <see cref="BocColumnDefinition"/> array. </returns>
  protected virtual BocColumnDefinition[] GetColumnsForCurrentLifeCycle (BocColumnDefinition[] columnDefinitions)
  {
    return columnDefinitions;
  }

  private BocMenuItem[] EnsureGetOptionsMenuItemsForPreviousLifeCycle()
  {
    if (_optionsMenuItemsPostBackEventHandlingPhase == null)
    {
      _optionsMenuItemsPostBackEventHandlingPhase = 
          GetOptionsMenuItemsForPreviousLifeCycle (_optionsMenuItems.ToArray());
    }
    return _optionsMenuItemsPostBackEventHandlingPhase;
  }

  private BocMenuItem[] EnsureGetOptionsMenuItemsForCurrentLifeCycle()
  {
    if (_optionsMenuItemsRenderPhase == null)
      _optionsMenuItemsRenderPhase = GetOptionsMenuItemsForCurrentLifeCycle (_optionsMenuItems.ToArray());
    return _optionsMenuItemsRenderPhase;
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
    return EnsureGetOptionsMenuItemsForCurrentLifeCycle();
  }

  /// <summary>
  ///   Override this method to modify the menu items displayed in the <see cref="BocList"/>'s options menu
  ///   in the current page life cycle.
  /// </summary>
  /// <param name="menuItems"> 
  ///   The <see cref="BocMenuItem"/> array containing the menu item available in the options menu. 
  /// </param>
  /// <returns> The <see cref="BocMenuItem"/> array. </returns>
  protected virtual BocMenuItem[] GetOptionsMenuItemsForCurrentLifeCycle (BocMenuItem[] menuItems)
  {
    return menuItems;
  }

  private BocMenuItem[] EnsureGetListMenuItemsForPreviousLifeCycle()
  {
    if (_listMenuItemsPostBackEventHandlingPhase == null)
    {
      _listMenuItemsPostBackEventHandlingPhase = 
          GetListMenuItemsForPreviousLifeCycle (_listMenuItems.ToArray());
    }
    return _listMenuItemsPostBackEventHandlingPhase;
  }

  private BocMenuItem[] EnsureGetListMenuItemsForCurrentLifeCycle()
  {
    if (_listMenuItemsRenderPhase == null)
      _listMenuItemsRenderPhase = GetListMenuItemsForCurrentLifeCycle (_listMenuItems.ToArray());
    return _listMenuItemsRenderPhase;
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
    return EnsureGetListMenuItemsForCurrentLifeCycle();
  }

  /// <summary>
  ///   Override this method to modify the menu items displayed in the <see cref="BocList"/>'s menu area
  ///   in the current page life cycle.
  /// </summary>
  /// <param name="menuItems"> 
  ///   The <see cref="BocMenuItem"/> array containing the menu item available in the options menu. 
  /// </param>
  /// <returns> The <see cref="BocMenuItem"/> array. </returns>
  protected virtual BocMenuItem[] GetListMenuItemsForCurrentLifeCycle (BocMenuItem[] menuItems)
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
  ///   First <see cref="IBusinessObject"/> to compare. Must not be <see langword="null"/>.
  /// </param>
  /// <param name="objectB"> 
  ///   Second <see cref="IBusinessObject"/> to compare. Must not be <see langword="null"/>.
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
    ArgumentUtility.CheckNotNullAndType ("objectA", objectA, typeof (IBusinessObject));
    ArgumentUtility.CheckNotNullAndType ("objectB", objectB, typeof (IBusinessObject));

    IBusinessObject businessObjectA = objectA as IBusinessObject;
    IBusinessObject businessObjectB = objectB as IBusinessObject;

    BocColumnDefinition[] renderColumns = EnsureGetColumnsForCurrentLifeCycle();
    foreach (SortingOrderEntry currentEntry in _sortingOrder)
    {
      if (currentEntry.Direction != SortingDirection.None)
      {
        if (! (renderColumns[currentEntry.ColumnIndex] is BocValueColumnDefinition))
          throw new ArgumentOutOfRangeException ("The BocList '" + ID + "' does not have a value column at index" + currentEntry.ColumnIndex + ".");

        BocSimpleColumnDefinition simpleColumn = renderColumns[currentEntry.ColumnIndex] as BocSimpleColumnDefinition;
        BocCompoundColumnDefinition compoundColumn = renderColumns[currentEntry.ColumnIndex] as BocCompoundColumnDefinition;
        
        if (simpleColumn != null)
        {
          //  Simple column, one value
          int compareResult = 0;
          if (currentEntry.Direction == SortingDirection.Ascending)
            compareResult = ComparePropertyPathValues (simpleColumn.PropertyPath, businessObjectA, businessObjectB);
          else
            compareResult = ComparePropertyPathValues (simpleColumn.PropertyPath, businessObjectB, businessObjectA);
          if (compareResult != 0)
            return compareResult;
        }
        else if (compoundColumn != null)
        {
          //  Compund column, list of values.
          foreach (PropertyPathBinding propertyPathBinding in compoundColumn.PropertyPathBindings)
          {
            int compareResult = 0;
            if (currentEntry.Direction == SortingDirection.Ascending)
              compareResult = ComparePropertyPathValues (propertyPathBinding.PropertyPath, businessObjectA, businessObjectB);
            else
              compareResult = ComparePropertyPathValues (propertyPathBinding.PropertyPath, businessObjectB, businessObjectA);
            if (compareResult != 0)
              return compareResult;
          }
        }
      } 
    }
    if (businessObjectA != null && businessObjectB != null)
    {
      int indexObjectA = (int) _originalRowIndices[businessObjectA];
      int indexObjectB = (int) _originalRowIndices[businessObjectB];
      return indexObjectA - indexObjectB;
    }
    return 0;
  }

  /// <summary>
  ///   Compares the values of the <see cref="IBusinessObjectProperty"/> identified by <paramref name="propertyPath"/>
  ///   for <paramref name="businessObjectA"/> and <paramref name="businessObjectB"/>.
  /// </summary>
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
    object valueA = propertyPath.GetValue (businessObjectA);
    object valueB = propertyPath.GetValue (businessObjectB);

    if (valueA == null && valueB == null)
      return 0;
    if (valueA == null)
      return -1;
    if (valueB == null)
      return 1;
    if (valueA is IComparable && valueB is IComparable)
      return Comparer.Default.Compare (valueA, valueB);
    return Comparer.Default.Compare (valueA.ToString(), valueB.ToString());
  }

  /// <summary>
  ///   Dispatches the resources passed in <paramref name="values"/> to the <see cref="BocList"/>'s
  ///   properties.
  /// </summary>
  /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
  public void Dispatch (IDictionary values)
  {
    Hashtable fixedColumns = new Hashtable();
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

        Hashtable currentCollection = null;

        //  Switch to the right collection
        switch (collectionID)
        {
          case c_resourceKeyFixedColumns:
          {
            currentCollection = fixedColumns;
            break;
          }
          default:
          {
            //  Invalid collection property
            s_log.Warn ("BocList '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' does not contain a collection property named '" + collectionID + "'.");
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
        s_log.Warn ("BocList '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' received a resource with an invalid or unknown key '" + key + "'. Required format: 'property' or 'collectionID:elementID:property'.");
      }
    }

    //  Dispatch simple properties
    DispatchToProperties (this, propertyValues);

    //  Dispatch fixed column definition properties
    foreach (DictionaryEntry fixedColumnEntry in fixedColumns)
    {
      string columnID = (string) fixedColumnEntry.Key;
      
      bool isValidID = false;
      foreach (BocColumnDefinition columnDefinition in _fixedColumns)
      {
        if (columnDefinition.ColumnID == columnID)
        {
          DispatchToProperties (columnDefinition, (IDictionary) fixedColumnEntry.Value);
          isValidID = true;
          break;
        }
      }

      if (! isValidID)
      {
        //  Invalid collection element
        s_log.Warn ("BocList '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' does not contain a fixed column definition with an ID of '" + columnID + "'.");
      }
    }
  }

  /// <summary>
  ///   Dispatches the resources passed in <paramref name="values"/> to the 
  ///   properties of <paramref name="obj"/>.
  /// </summary>
  /// <param name="obj"> The object receving the resources. </param>
  /// <param name="values"> An <c>IDictonary</c>: &lt;string property-name, string value&gt;. </param>
  private void DispatchToProperties (object obj, IDictionary values)
  {
    ArgumentUtility.CheckNotNull ("obj", obj);
    ArgumentUtility.CheckNotNull ("values", values);

    foreach (DictionaryEntry entry in values)
    {
      string propertyName = (string) entry.Key;
      string propertyValue = (string) entry.Value;

      PropertyInfo property = obj.GetType ().GetProperty (propertyName, typeof (string));
      if (property != null)
        property.SetValue (obj, propertyValue, new object[0]); 
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

  /// <summary>
  ///   The <see cref="IBusinessObjectReferenceProperty"/> object this control is bound to.
  /// </summary>
  /// <remarks>
  ///   Explicit setting of <see cref="Property"/> is not offically supported.
  /// </remarks>
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

  /// <summary>
  ///   Gets or sets the current value when <see cref="Value"/> through polymorphism.
  /// </summary>
  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = (IList) value; }
  }

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; 
  ///   using it's ClientID.
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
  ///   Gets or sets the list of<see cref="Type"/> objects for the 
  ///   <see cref="IBusinessObjectProperty"/> implementations that can be bound to this control.
  /// </summary>
  protected override Type[] SupportedPropertyInterfaces
  {
    get { return s_supportedPropertyInterfaces; }
  }

  /// <summary>
  ///   Gets a value that indicates whether properties with the specified multiplicity are 
  ///   supported.
  /// </summary>
  /// <returns>
  ///   <see langword="true"/> if the multiplicity specified by <paramref name="isList"/> is 
  ///   supported.
  /// </returns>
  protected override bool SupportsPropertyMultiplicity (bool isList)
  {
    return isList;
  }

  /// <summary> Gets the user independent column defintions. </summary>
  /// <remarks> Behavior undefined if set after initialization phase or changed between postbacks. </remarks>
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Category ("Column Definition")]
  [Description ("The user independent column defintions.")]
  [DefaultValue ((string) null)]
  public BocColumnDefinitionCollection FixedColumns
  {
    get { return _fixedColumns; }
  }

  /// <summary> Gets the predefined column defintion sets that the user can choose from at run-time. </summary>
  //  No designer support intended
  //  [PersistenceMode(PersistenceMode.InnerProperty)]
  //  [ListBindable (false)]
  //  [Category ("Column Definition")]
  //  [Description ("The predefined column defintion sets that the user can choose from at run-time.")]
  //  [DefaultValue ((string) null)]
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
    get { return _selectedColumnDefinitionSet; }
    set
    {
      _selectedColumnDefinitionSet = value; 
      ArgumentUtility.CheckNotNullOrEmpty ("AvailableColumnDefinitionSets", _availableColumnDefinitionSets);
      _selectedColumnDefinitionSetIndex = -1;

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

        if (_selectedColumnDefinitionSetIndex < 0) 
          throw new IndexOutOfRangeException ("The specified ColumnDefinitionSet could not be found in AvailableColumnDefinitionSets");
      }

      _additionalColumnsList.SelectedIndex = _selectedColumnDefinitionSetIndex;
      RemoveDynamicColumnsFromSortingOrder();
    }
  }

  /// <summary>
  ///   Gets or sets the index of the selected <see cref="BocColumnDefinitionSet"/> used to
  ///   supplement the <see cref="FixedColumns"/>.
  /// </summary>
  [Browsable (false)]
  protected int SelectedColumnDefinitionSetIndex
  {
    get { return _selectedColumnDefinitionSetIndex; }
    set 
    {
      _selectedColumnDefinitionSetIndex = value; 
      
      if (   _selectedColumnDefinitionSetIndex < -1
          || _selectedColumnDefinitionSetIndex >= _availableColumnDefinitionSets.Count)
      {
        throw new ArgumentOutOfRangeException ("value", value, "SelectedColumnDefinitionSetIndex was outside the bounds of AvailableColumnDefinitionSets");
      }

      if (_selectedColumnDefinitionSetIndex != -1)
      {
        int selectedIndex = _selectedColumnDefinitionSetIndex;

        if (selectedIndex < _availableColumnDefinitionSets.Count)
          _selectedColumnDefinitionSet = _availableColumnDefinitionSets[selectedIndex];
        else
          _selectedColumnDefinitionSet = null;
      }
      else
      {
        _selectedColumnDefinitionSet = null;
      }
      RemoveDynamicColumnsFromSortingOrder();
    }
  }

  /// <summary> Gets the <see cref="IBusinessObject"/> objects selected in the <see cref="BocList"/>. </summary>
  /// <value> An array of <see cref="IBusinessObject"/> objects. </value>
  [Browsable (false)]
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
  /// <value> An array of <see cref="int"/> values. </value>
  [Browsable (false)]
  public int[] GetSelectedRows()
  {
    string commonCheckBoxID = ID + c_dataRowCheckBoxIDSuffix;
    string titleCheckBoxID = ID + c_titleRowCheckBoxIDSuffix;
    int commonCheckBoxIDLength = commonCheckBoxID.Length;
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
  [Category ("Appearance")]
  [Description ("Enables the options menu.")]
  [DefaultValue (true)]
  public bool ShowOptionsMenu
  {
    get { return _showOptionsMenu; }
    set { _showOptionsMenu = value; }
  }

  /// <summary>
  ///   Gets or sets a flag that indicates whether row selection is enabled.
  /// </summary>
  /// <remarks> 
  ///   If row selection is enabled, the control displays a checkbox in front of each row
  ///   and highlights selected data rows.
  /// </remarks>
  /// <value> <see langword="true"/> to enable row selection. </value>
  [Category ("Behavior")]
  [Description ("Indicates whether row selection is enabled.")]
  [DefaultValue (false)]
  public bool EnableSelection
  {
    get { return _enableSelection; }
    set { _enableSelection = value; }
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

  /// <summary> 
  ///   Occurs when a command of type <see cref="CommandType.Event"/> 
  ///   or <see cref="CommandType.WxeFunction"/> is clicked. 
  /// </summary>
  [Category ("Action")]
  [Description ("Occurs when a command of type Event or WxeFunction is clicked.")]
  public event BocCommandClickEventHandler CommandClick
  {
    add 
    {
      Events.AddHandler (EventCommandClick, value);
    }
    remove 
    { 
      Events.RemoveHandler (EventCommandClick, value);
    }
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
  /// <remarks> 
  ///   The <see cref="MenuBlockOffset"/> is applied as a <c>padding</c> attribute.
  /// </remarks>
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

  /// <summary> 
  ///   Gets or sets the text that is rendered as a title for the drop list of additional columns.
  /// </summary>
  [Category ("Menu")]
  [Description ("The text that is rendered as a title for the list of additional columns.")]
  [DefaultValue ("Additional Columns")]
  public string AdditionalColumnsTitle
  {
    get { return _additionalColumnsTitle; }
    set { _additionalColumnsTitle = value; }
  }

  /// <summary>
  ///   Gets or sets the width that you want to apply to the drop down list used to display the 
  ///   user selectable columns.
  /// </summary>
  [Category ("Menu")]
  [Description ("The width that you want to apply to the drop down list used to display the user selectable columns.")]
  [DefaultValue (typeof (Unit), "")]
  public Unit AdditionalColumnsListWidth
  {
    get { return _additionalColumnsListWidth; }
    set { _additionalColumnsListWidth = value; }
  }

  /// <summary> 
  ///   Gets or sets the text that is rendered as a label for the <c>Options</c> menu.
  /// </summary>
  [Category ("Menu")]
  [Description ("The text that is rendered as a label for the Options menu.")]
  [DefaultValue ("Options")]
  public string OptionsTitle
  {
    get { return _optionsTitle; }
    set { _optionsTitle = value; }
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

  /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s list of additionalc columns. </summary>
  /// <remarks> Class: <c>bocListAdditionalColumnsList</c> </remarks>
  protected virtual string CssClassAdditionalColumnsList
  { get { return "bocListAdditionalColumnsList"; } }
  #endregion

  /// <summary>
  ///   Gets a flag that indicated whether the page is in post back mode.
  /// </summary>
  private bool IsPostBack
  {
    get { return !IsDesignMode && Page != null && Page.IsPostBack; }
  }
}

/// <summary>
///   Represents the method that handles the <see cref="BocList.CommandClick"/> event
///   raised when clicking on an <see cref="Command"/> 
///   of type <see cref="CommandType.Event"/>.
/// </summary>
public delegate void BocCommandClickEventHandler (object sender, BocCommandClickEventArgs e);

/// <summary>
///   Provides data for the <see cref="BocList.CommandClick"/> event.
/// </summary>
public class BocCommandClickEventArgs: EventArgs
{
  /// <summary>
  ///   The <see cref="BocColumnDefinition.ID"/> of the column to which the clicked command 
  ///   belongs to.
  /// </summary>
  private string _columnID;
  /// <summary>
  ///   An index that identifies the <see cref="IBusinessObject"/> on which the rendered command is 
  ///   applied on.
  /// </summary>
  private int _listIndex;
  /// <summary>
  ///   The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/>, if the 
  ///   <see cref="IBusinessObject"/> on which the rendered command is applied on 
  ///   is of type <see cref="IBusinessObjectWithIdentity"/>.
  /// </summary>
  private string _businessObjectID;

  /// <summary> Simple Constructor. </summary>
  /// <param name="columnID">
  ///   The <see cref="BocColumnDefinition.ID"/> of the column to which the command belongs.
  /// </param>
  /// <param name="listIndex">
  ///   An index that identifies the <see cref="IBusinessObject"/> on which the rendered command is 
  ///   applied on.
  /// </param>
  /// <param name="businessObjectID">
  ///   The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/>, if the 
  ///   <see cref="IBusinessObject"/> on which the rendered command is applied on 
  ///   is of type <see cref="IBusinessObjectWithIdentity"/>.
  /// </param>
  public BocCommandClickEventArgs (
      string columnID, 
      int listIndex, 
      string businessObjectID)
  {
    _columnID = columnID;
    _listIndex = listIndex;
    _businessObjectID = StringUtility.EmptyToNull (businessObjectID);
  }

  /// <summary>
  ///   The <see cref="BocColumnDefinition.ID"/> of the column to which the command belongs.
  /// </summary>
  public string ColumnID
  {
    get { return _columnID; }
  }

  /// <summary>
  ///   An index that identifies the <see cref="IBusinessObject"/> on which the rendered command is 
  ///   applied on.
  /// </summary>
  public int ListIndex
  {
    get { return _listIndex; }
  }

  /// <summary>
  ///   The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/>, if the 
  ///   <see cref="IBusinessObject"/> on which the rendered command is applied on 
  ///   is of type <see cref="IBusinessObjectWithIdentity"/>.
  /// </summary>
  public string BusinessObjectID
  {
    get { return _businessObjectID; }
  }
}

}