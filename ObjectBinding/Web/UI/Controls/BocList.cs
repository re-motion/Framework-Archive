using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Reflection;
using log4net;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;
using Rubicon.Web.Utilities;
using Rubicon.ObjectBinding.Web.Design;
using Rubicon.Web;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <include file='doc\include\Controls\BocList.xml' path='BocList/Class/*' />
// TODO: BocList: Details View
// TODO: BocList: Sort-Buttons
[ToolboxItemFilter("System.Web.UI")]
public class BocList: BusinessObjectBoundModifiableWebControl, IResourceDispatchTarget
{
  //  constants
  private const string c_dataRowCheckBoxIDSuffix = "_CheckBox_";
  private const string c_titleRowCheckBoxIDSuffix = "_CheckBox_SelectAll";

  private const string c_moveFirstIcon = "MoveFirst.gif";
  private const string c_moveLastIcon = "MoveLast.gif";
  private const string c_movePreviousIcon = "MovePrevious.gif";
  private const string c_moveNextIcon = "MoveNext.gif";
  private const string c_moveFirstInactiveIcon = "MoveFirstInactive.gif";
  private const string c_moveLastInactiveIcon = "MoveLastInactive.gif";
  private const string c_movePreviousInactiveIcon = "MovePreviousInactive.gif";
  private const string c_moveNextInactiveIcon = "MoveNextInactive.gif";
  private const string c_moveCommand = "Move";
  private const string c_moveCommandFormatString = c_moveCommand + "={0}";

  private const string c_whiteSpace = "&nbsp;";

  /// <summary> 
  ///   Text displayed when control is displayed in desinger and is read-only has no contents.
  /// </summary>
  private const string c_designModeEmptyContents = "#";
  private const string c_designModeDummyColumnTitle = "Column Title {0}";
  private const int c_designModeDummyColumnCount = 3;

  // types
  
  private enum MoveOption
  {
    Undefined,
    First,
    Last,
    Previous,
    Next
  }

  // static members
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectProperty) };
  
  /// <summary> The log4net logger. </summary>
  private static readonly log4net.ILog s_log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	// member fields

  /// <summary></summary>
  private bool _isDirty = true;

  /// <summary> The <see cref="DropDownList"/> used to select the column configuration. </summary>
  private DropDownList _additionalColumnsList = null;

  private ImageButton moveFirstButton = null;
  private ImageButton moveLastButton = null;
  private ImageButton movePreviousButton = null;
  private ImageButton moveNextButton = null;

  private IList _value = null;

  /// <summary> The <see cref="Style"/> applied to the <see cref="_additionalColumnsList"/>. </summary>
  private DropDownListStyle _additionalColumnsListStyle = new DropDownListStyle();

  // can only be set at design time.
  private BocColumnDefinitionCollection _fixedColumns;
  
  // may be set at run time. these columnDefinitions do usually not contain commands.
  private BocColumnDefinitionSet _selectedColumnDefinitionSet;
  
  // the command to be used for the first ValueColumnDefinition column
  private BocItemCommand _firstColumnCommand;
  
  // show check boxes for each object
  private bool _showSelection = false;
  
  // show drop down list for selecting additional columnDefinitions
  private bool _showAdditionalColumnsList = true;

  // user may choose one ColumnDefinitionSet
  private BocColumnDefinitionSetCollection _availableColumnDefinitionSets;

  // Null, 0: show all objects, > 0: show n objects per page
  private NaInt32 _pageSize = NaInt32.Null; 

  // show page info ("page 1 of n") and links always (true),
  //  or only if there is more than 1 page (false)
  private bool _alwaysShowPageInfo = false; 

  private int _currentRow = 0;
  private int _currentPage = 0;
  private int _pageCount = 0;

  private int _selectedColumnDefinitionSetIndex = -1;

  /// <summary> <see langword="true"/> to show the value's icon. </summary>
  private bool _enableIcon = true;

  private Hashtable _checkBoxCheckedState = new Hashtable();

  private MoveOption _move = MoveOption.Undefined;

  private string __pageInfo = "Page {0} of {1}";

  // construction and disposing

  /// <summary></summary>
	public BocList()
	{
    _fixedColumns = new BocColumnDefinitionCollection (this);
    _availableColumnDefinitionSets = new BocColumnDefinitionSetCollection (this);
  }

	// methods and properties

  /// <summary>
  ///   Calls the parent's <c>OnInit</c> method and initializes this control's sub-controls.
  /// </summary>
  /// <param name="e">An <see cref="EventArgs"/> object that contains the event data. </param>
  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);

    _additionalColumnsList = new DropDownList();
    moveFirstButton = new ImageButton();
    moveLastButton = new ImageButton();
    movePreviousButton = new ImageButton();
    moveNextButton = new ImageButton();

    _additionalColumnsList.ID = this.ID + "_ColumnConfigurationList";
    _additionalColumnsList.EnableViewState = true;
    _additionalColumnsList.SelectedIndexChanged += new EventHandler(AdditionalColumnsList_SelectedIndexChanged);
    Controls.Add (_additionalColumnsList);

    _additionalColumnsListStyle.AutoPostback = true;

    moveFirstButton.Click += new ImageClickEventHandler (MoveFirstButton_Click);
    Controls.Add (moveFirstButton);

    moveLastButton.Click += new ImageClickEventHandler (MoveLastButton_Click);
    Controls.Add (moveLastButton);

    movePreviousButton.Click += new ImageClickEventHandler (MovePreviousButton_Click);
    Controls.Add (movePreviousButton);

    moveNextButton.Click += new ImageClickEventHandler (MoveNextButton_Click);
    Controls.Add (moveNextButton);

    Binding.BindingChanged += new EventHandler (Binding_BindingChanged);

    if (! IsPostBack)
      PopulateAdditionalColumnsList();
    _availableColumnDefinitionSets.CollectionChanged += new CollectionChangeEventHandler(AvailableColumnDefinitionSets_CollectionChanged);
    
    if (IsPostBack && Page != null)
    {
      string dataRowCheckBoxFilter = ID + c_dataRowCheckBoxIDSuffix;
      string titleRowCheckBoxFilter = ID + c_titleRowCheckBoxIDSuffix;
      NameValueCollection formVariables = Page.Request.Form;

      for (int i = 0; i < formVariables.Count; i++)
      {
        string key = formVariables.Keys[i];

        if (key.StartsWith (dataRowCheckBoxFilter))
          _checkBoxCheckedState[key] = true; 
        else if (key.StartsWith (titleRowCheckBoxFilter))
          _checkBoxCheckedState[key] = true; 
      }
    }
  }

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);
    
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
    }
  }

  protected override void Render (HtmlTextWriter writer)
  {
    BocColumnDefinition[] columnDefinitions = _fixedColumns.ToArray();
    if (_selectedColumnDefinitionSet != null)
    {
      columnDefinitions = (BocColumnDefinition[]) ArrayUtility.Combine (
        columnDefinitions, 
        _selectedColumnDefinitionSet.ColumnDefinitionCollection.ToArray());
    }

    if (IsDesignMode)
    {
      if (_pageCount == 0)
        _pageCount = 1;
    }
 
    RenderTitle (writer);
    RenderTableOpeningTag (writer);
    RenderColGroup (writer, columnDefinitions);
    RenderColumnTitlesRow (writer, columnDefinitions);

    int firstRow = 0;
    int rowCountWithOffset = (Value != null) ? Value.Count : 0;

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
      for (int idxRow = firstRow; idxRow < rowCountWithOffset; idxRow++)
      {
        RenderDataRow (writer, columnDefinitions, idxRow, isOddRow);
        isOddRow = !isOddRow;
      }
    }

    RenderTableClosingTag (writer);
    if (_alwaysShowPageInfo || _pageCount > 1)
      RenderNavigator (writer);
  }

  private void RenderTitle (HtmlTextWriter writer)
  {
    if (_showAdditionalColumnsList)
    {
      _additionalColumnsListStyle.ApplyStyle (_additionalColumnsList);
      _additionalColumnsList.RenderControl (writer);
    }
  }

  private void RenderNavigator (HtmlTextWriter writer)
  {
    bool isFirstPage = _currentPage == 0;
    bool isLastPage = _currentPage + 1 >= _pageCount;

    if (! Width.IsEmpty)
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Width.ToString());
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassNavigator);
    writer.RenderBeginTag (HtmlTextWriterTag.Div);

    writer.Write (__pageInfo, _currentPage + 1, _pageCount);
    writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);
    
    string imageUrl = null;

    if (isFirstPage)
      imageUrl = c_moveFirstInactiveIcon;
    else
      imageUrl = c_moveFirstIcon;      
    imageUrl = ResourceUrlResolver.GetResourceUrl (
      this, typeof (BocList), ResourceType.Image, imageUrl);
    if (isFirstPage)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Src, imageUrl);
      writer.RenderBeginTag (HtmlTextWriterTag.Img);
      writer.RenderEndTag();
    }
    else
    {
      moveFirstButton.ImageUrl = imageUrl;
      moveFirstButton.RenderControl (writer);
    }
    writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);

    if (isFirstPage)
      imageUrl = c_movePreviousInactiveIcon;
    else
      imageUrl = c_movePreviousIcon;      
    imageUrl = ResourceUrlResolver.GetResourceUrl (
      this, typeof (BocList), ResourceType.Image, imageUrl);
    if (isFirstPage)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Src, imageUrl);
      writer.RenderBeginTag (HtmlTextWriterTag.Img);
      writer.RenderEndTag();
    }
    else
    {
      movePreviousButton.ImageUrl = imageUrl;
      movePreviousButton.RenderControl (writer);
    }

    writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);

    if (isLastPage)
      imageUrl = c_moveNextInactiveIcon;
    else
      imageUrl = c_moveNextIcon;      
    imageUrl = ResourceUrlResolver.GetResourceUrl (
      this, typeof (BocList), ResourceType.Image, imageUrl);
    if (isLastPage)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Src, imageUrl);
      writer.RenderBeginTag (HtmlTextWriterTag.Img);
      writer.RenderEndTag();
    }
    else
    {
      moveNextButton.ImageUrl = imageUrl;
      moveNextButton.RenderControl (writer);
    }

    writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);

    if (isLastPage)
      imageUrl = c_moveLastInactiveIcon;
    else
      imageUrl = c_moveLastIcon;     
    imageUrl = ResourceUrlResolver.GetResourceUrl (
      this, typeof (BocList), ResourceType.Image, imageUrl);
    if (isLastPage)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Src, imageUrl);
      writer.RenderBeginTag (HtmlTextWriterTag.Img);
      writer.RenderEndTag();
    }
    else
    {
      moveLastButton.ImageUrl = imageUrl;
      moveLastButton.RenderControl (writer);
    }

    writer.RenderEndTag();
  }

  private void RenderTableOpeningTag (HtmlTextWriter writer)
  {
    if (! Width.IsEmpty)
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Width.ToString());
    writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTable);
    writer.RenderBeginTag (HtmlTextWriterTag.Table);
  }

  private void RenderTableClosingTag (HtmlTextWriter writer)
  {
    writer.RenderEndTag();
  }

  private void RenderColGroup (HtmlTextWriter writer, BocColumnDefinition[] columnDefinitions)
  {
    writer.RenderBeginTag (HtmlTextWriterTag.Colgroup);

    if (ShowSelection)
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Unit.Percentage(0).ToString());
      writer.RenderBeginTag (HtmlTextWriterTag.Col);
      writer.RenderEndTag();
    }

    foreach (BocColumnDefinition column in columnDefinitions)
    {
      if (! column.Width.IsEmpty)
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, column.Width.ToString());

      writer.RenderBeginTag (HtmlTextWriterTag.Col);
      writer.RenderEndTag();
    }
    
    if (IsDesignMode && columnDefinitions.Length == 0)
    {
      for (int i = 0; i < c_designModeDummyColumnCount; i++)
      {
        writer.RenderBeginTag (HtmlTextWriterTag.Col);
        writer.RenderEndTag();
      }
    }
 
    writer.RenderEndTag();
  }

  private void RenderColumnTitlesRow (HtmlTextWriter writer, BocColumnDefinition[] columnDefinitions)
  {
    writer.RenderBeginTag (HtmlTextWriterTag.Tr);

    if (ShowSelection)
    {
      // TODO: BocList: CheckBox SelectAll implementation
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTitleCell);
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      //string checkBoxName = ID + c_titleRowCheckBoxIDSuffix;
      //bool isChecked = (_checkBoxCheckedState[checkBoxName] != null);
      //RenderCheckBox (writer, checkBoxName, isChecked);
      writer.Write ("&nbsp;");
      writer.RenderEndTag();
    }

    foreach (BocColumnDefinition column in columnDefinitions)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTitleCell);
      writer.RenderBeginTag (HtmlTextWriterTag.Td);

      if (IsDesignMode && column.ColumnTitleDisplayValue.Length == 0)
          writer.Write (c_designModeEmptyContents);
      else
      {
        string contents = HttpUtility.HtmlEncode (column.ColumnTitleDisplayValue);
        if (contents == string.Empty)
          contents = "&nbsp;";
        writer.Write (contents);
      }
      writer.RenderEndTag();
    }
    
    if (IsDesignMode && columnDefinitions.Length == 0)
    {
      for (int i = 0; i < c_designModeDummyColumnCount; i++)
      {
        writer.RenderBeginTag (HtmlTextWriterTag.Td);
        writer.Write (string.Format (c_designModeDummyColumnTitle, i + 1));
        writer.RenderEndTag ();
      }
    }

    writer.RenderEndTag ();
  }

  private void RenderDataRow (
    HtmlTextWriter writer, 
    BocColumnDefinition[] columnDefinitions, 
    int rowIndex,
    bool isOddRow)
  {
    IBusinessObject businessObject = Value[rowIndex] as IBusinessObject;
    if (businessObject == null)
      return;

    IBusinessObjectWithIdentity businessObjectWithIdentity = 
      businessObject as IBusinessObjectWithIdentity;
    string objectID = null;
    if (businessObjectWithIdentity != null)
      objectID = businessObjectWithIdentity.UniqueIdentifier;

    writer.RenderBeginTag (HtmlTextWriterTag.Tr);

    bool isReadOnly = IsReadOnly;
    bool isFirstValueColumnRendered = false;
    bool isFirstValueColumnCommandEnabled = false;

    if (FirstColumnCommand != null)
    {
      if (    FirstColumnCommand.Show == BocItemCommandShow.Always
          ||  isReadOnly && FirstColumnCommand.Show == BocItemCommandShow.ReadOnly
          ||  ! isReadOnly && FirstColumnCommand.Show == BocItemCommandShow.EditMode)
      {
        isFirstValueColumnCommandEnabled = true;
      }
    }

    if (ShowSelection)
    {
      if (isOddRow)
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassDataCellOdd);
      else
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassDataCellEven);

      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      string checkBoxName = ID + c_dataRowCheckBoxIDSuffix + rowIndex.ToString();
      bool isChecked = (_checkBoxCheckedState[checkBoxName] != null);
      RenderCheckBox (writer, checkBoxName, isChecked);
      writer.RenderEndTag();
    }

    foreach (BocColumnDefinition column in columnDefinitions)
    {
      if (isOddRow)
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassDataCellOdd);
      else
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassDataCellEven);

      writer.RenderBeginTag (HtmlTextWriterTag.Td);

      BocCommandColumnDefinition commandColumn = column as BocCommandColumnDefinition;
      BocCompoundColumnDefinition compoundColumn = column as BocCompoundColumnDefinition;
      BocSimpleColumnDefinition simpleColumn = column as BocSimpleColumnDefinition;
      BocValueColumnDefinition valueColumn = column as BocValueColumnDefinition;

      if (! isFirstValueColumnRendered && valueColumn != null)
      {
        //  Render FirstColumnCommand BeginTag
        if (isFirstValueColumnCommandEnabled)
        {
          FirstColumnCommand.RenderBegin (writer, rowIndex, objectID);
        }

        //  Render Icon
        if (EnableIcon)
        {
          IBusinessObjectService service
            = businessObject.BusinessObjectClass.BusinessObjectProvider.GetService(
              typeof (IBusinessObjectWebUIService));

          IBusinessObjectWebUIService webUIService = service as IBusinessObjectWebUIService;

          IconPrototype iconPrototype = null;
          if (webUIService != null)
          {
            if (businessObjectWithIdentity != null)
              iconPrototype = webUIService.GetIcon (businessObjectWithIdentity);
            else
              iconPrototype = webUIService.GetIcon (businessObjectWithIdentity);
          }

          if (iconPrototype != null)
          {
            writer.AddAttribute (HtmlTextWriterAttribute.Src, iconPrototype.Url);

            if (! iconPrototype.Width.IsEmpty && ! iconPrototype.Height.IsEmpty)
            {
              writer.AddAttribute (HtmlTextWriterAttribute.Width, iconPrototype.Width.ToString());
              writer.AddAttribute (HtmlTextWriterAttribute.Width, iconPrototype.Height.ToString());
            }
            
            writer.RenderBeginTag (HtmlTextWriterTag.Img);
            writer.RenderEndTag();
            writer.Write (c_whiteSpace);
          }
        }
      }

      if (commandColumn != null)
      {
        bool isCommandEnabled = false;

        if (    commandColumn.Command.Show == BocItemCommandShow.Always
            ||  isReadOnly && commandColumn.Command.Show == BocItemCommandShow.ReadOnly
            ||  ! isReadOnly && commandColumn.Command.Show == BocItemCommandShow.EditMode)
        {
          isCommandEnabled = true;
        }

        if (isCommandEnabled)
          commandColumn.Command.RenderBegin (writer, rowIndex, objectID);
        
        if (commandColumn.IconPath != null)
        {
          writer.AddAttribute (HtmlTextWriterAttribute.Href, commandColumn.IconPath);
          writer.RenderBeginTag (HtmlTextWriterTag.Img);
          writer.RenderEndTag();
        }

        if (commandColumn.Label != null)
        {
          writer.Write (commandColumn.Label);
        }
          
        if (isCommandEnabled)
          commandColumn.Command.RenderEnd (writer);
      }
      else if (compoundColumn != null)
      {
        string contents = compoundColumn.GetStringValue (businessObject);
        contents = HttpUtility.HtmlEncode (contents);
        if (contents == string.Empty)
          contents = "&nbsp;";
        writer.Write (contents);
      }
      else if (simpleColumn != null)
      {
        string contents = simpleColumn.GetStringValue (businessObject);
        contents = HttpUtility.HtmlEncode (contents);
        if (contents == string.Empty)
          contents = "&nbsp;";
        writer.Write (contents);
      }

      if (! isFirstValueColumnRendered && valueColumn != null)
      {
        //  Render FirstColumnCommand EndTag
        if (isFirstValueColumnCommandEnabled)
        {
          FirstColumnCommand.RenderEnd (writer);
        }
       
        isFirstValueColumnRendered = true;
      }

      writer.RenderEndTag ();
    }
    
    writer.RenderEndTag ();
  }

  private void RenderCheckBox (HtmlTextWriter writer, string name, bool isChecked)
  {
    writer.AddAttribute (HtmlTextWriterAttribute.Type, "checkbox");
    writer.AddAttribute (HtmlTextWriterAttribute.Name, name);
    if (isChecked)
      writer.AddAttribute (HtmlTextWriterAttribute.Checked, "checked");      
    writer.RenderBeginTag (HtmlTextWriterTag.Input);
    writer.RenderEndTag();
  }

  /// <summary>
  ///   Calls the parents <c>LoadViewState</c> method and restores this control's specific data.
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
    //  _isDirty = (bool) values[];
  }

  /// <summary>
  ///   Calls the parents <c>SaveViewState</c> method and saves this control's specific data.
  /// </summary>
  /// <returns>
  ///   Returns the server control's current view state.
  /// </returns>
  protected override object SaveViewState()
  {
    object[] values = new object[4];

    values[0] = base.SaveViewState();
    values[1] = _selectedColumnDefinitionSetIndex;
    values[2] = _currentRow;

    //  values[] = _isDirty;

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
    if (! interim)
    {
      Binding.EvaluateBinding();
      if (Property != null && DataSource != null && DataSource.BusinessObject != null)
      {
        ValueImplementation = DataSource.BusinessObject.GetProperty (Property);
        _isDirty = false;
      }
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
    if (! interim)
    {
      Binding.EvaluateBinding();
      if (Property != null && DataSource != null &&  DataSource.BusinessObject != null && ! IsReadOnly)
      {
        DataSource.BusinessObject.SetProperty (Property, Value);

        //  get_Value parses the internal representation of the date/time value
        //  set_Value updates the internal representation of the date/time value
        Value = Value;
      }
    }
  }
  /// <summary> Handles refreshing the bound control. </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  private void Binding_BindingChanged (object sender, EventArgs e)
  {
    //  TODO: BocList: BindingChanged
  }

  private void PopulateAdditionalColumnsList()
  {
    _additionalColumnsList.Items.Clear();

    if (_availableColumnDefinitionSets != null)
    {
      for (int i = 0; i < _availableColumnDefinitionSets.Count; i++)
      {
        BocColumnDefinitionSet columnDefinitionCollection = 
          _availableColumnDefinitionSets[i];

        ListItem item = new ListItem (columnDefinitionCollection.Title, i.ToString());
        _additionalColumnsList.Items.Add (item);
      }

      if (_selectedColumnDefinitionSetIndex >= _availableColumnDefinitionSets.Count)
      {
        _selectedColumnDefinitionSetIndex = -1;
      }
      else if (     _selectedColumnDefinitionSetIndex < 0
                &&  _availableColumnDefinitionSets.Count > 0)
      {
        _selectedColumnDefinitionSetIndex = 0;
      }

      SelectedColumnDefinitionSetIndex = _selectedColumnDefinitionSetIndex;
    }
  }

  private void AdditionalColumnsList_SelectedIndexChanged (object sender, EventArgs e)
  {
    SelectedColumnDefinitionSetIndex = _additionalColumnsList.SelectedIndex;
  }

  private void MoveFirstButton_Click(object sender, ImageClickEventArgs e)
  {
    _move = MoveOption.First;
  }

  private void MoveLastButton_Click(object sender, ImageClickEventArgs e)
  {
    _move = MoveOption.Last;
  }
  
  private void MovePreviousButton_Click(object sender, ImageClickEventArgs e)
  {
    _move = MoveOption.Previous;
  }
  
  private void MoveNextButton_Click(object sender, ImageClickEventArgs e)
  {
    _move = MoveOption.Next;
  }

  private const string c_resourcePageInfo = "PageInfo";
  private const string c_resourceFixedColumns = "FixedColumns";

  public void Dispatch(IDictionary values)
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
          case c_resourceFixedColumns:
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
      string id = (string) fixedColumnEntry.Key;
      
      bool isValidID = false;
      foreach (BocColumnDefinition columnDefinition in _fixedColumns)
      {
        if (columnDefinition.ID == id)
        {
          DispatchToProperties (columnDefinition, (IDictionary) fixedColumnEntry.Value);
          isValidID = true;
          break;
        }
      }

      if (! isValidID)
      {
        //  Invalid collection element
        s_log.Warn ("BocList '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' does not contain a fixed column definition with an ID of '" + id + "'.");
      }
    }
  }

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
      {
        property.SetValue (obj, propertyValue, new object[0]); 
      }
    }
  }

  /// <summary>
  ///   Gets or sets the current value.
  /// </summary>
  /// <value> 
  /// </value>
  [Browsable (false)]
  public new IList Value
  {
    get 
    {
      return _value;
    }
    set 
    {
      _value = value;
    }
  }

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
  ///   
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
  ///   The list of<see cref="Type"/> objects for the <see cref="IBusinessObjectProperty"/> 
  ///   implementations that can be bound to this control.
  /// </summary>
  protected override Type[] SupportedPropertyInterfaces
  {
    get { return s_supportedPropertyInterfaces; }
  }

  /// <summary>
  ///   Indicates whether properties with the specified multiplicity are supported.
  /// </summary>
  /// <returns>
  ///   <see langword="true"/> if the multiplicity specified by <paramref name="isList"/> is 
  ///   supported.
  /// </returns>
  protected override bool SupportsPropertyMultiplicity (bool isList)
  {
    return isList;
  }

  [PersistenceMode (PersistenceMode.InnerProperty)]
  [Category ("Column Definition")]
  [Description ("The ItemCommand added to the first value column.")]
  [DefaultValue ((string) null)]
  public BocItemCommand FirstColumnCommand
  {
    get { return _firstColumnCommand; }
    set { _firstColumnCommand = value; }
  }

  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Category ("Column Definition")]
  [Description ("The user independent column defintions.")]
  [DefaultValue ((string) null)]
  public BocColumnDefinitionCollection FixedColumns
  {
    get { return _fixedColumns; }
  }

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

  private void AvailableColumnDefinitionSets_CollectionChanged(object sender, CollectionChangeEventArgs e)
  {
    PopulateAdditionalColumnsList();
  }

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
    }
  }

  [Browsable (false)]
  protected int SelectedColumnDefinitionSetIndex
  {
    get { return _selectedColumnDefinitionSetIndex; }
    set 
    {
      _selectedColumnDefinitionSetIndex = value; 
      
      if (_selectedColumnDefinitionSetIndex < -1
          ||  _selectedColumnDefinitionSetIndex >= _availableColumnDefinitionSets.Count)
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
    }
  }

  [Category ("Behavior")]
  [Description ("The number of rows displayed per page or <= 0 / null for all rows.")]
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

  [Category ("Behavior")]
  [Description ("Set true to force the showing of the page count even when there is just one page.")]
  [DefaultValue (false)]
  public bool AlwaysShowPageInfo
  {
    get { return _alwaysShowPageInfo; }
    set { _alwaysShowPageInfo = value; }
  }

  [Category ("Appearance")]
  [Description ("If true, the control displays a drop down list containing the available column definition sets.")]
  [DefaultValue (true)]
  public bool ShowAdditionalColumnsList
  {
    get { return _showAdditionalColumnsList; }
    set { _showAdditionalColumnsList = value; }
  }

  [Category ("Appearance")]
  [Description ("If true, the control displays a checkbox in front of each data.")]
  [DefaultValue (false)]
  public bool ShowSelection
  {
    get { return _showSelection; }
    set { _showSelection = value; }
  }

  /// <summary>
  ///   Set <see langword="true"/> to display an icon in front of the first value column.
  /// </summary>
  [Category ("Appearance")]
  [Description ("Set true to enable the icon in front of the first value column.")]
  [DefaultValue (true)]
  public bool EnableIcon
  {
    get { return _enableIcon; }
    set
    { _enableIcon = value; }
  }

  /// <summary> The text providing the current page information to the user. </summary>
  /// <remarks> Use {0} for the current page and {1} for the total page count. </remarks>
  [Category ("Appearance")]
  [Description ("The text providing the current page information to the user. Use {0} for the current page and {1} for the total page count.")]
  [DefaultValue ("Page {0} of {1}")]
  public string PageInfo
  {
    get { return __pageInfo; }
    set { __pageInfo = value; }
  }

  private bool IsPostBack
  {
    get { return !IsDesignMode && Page != null && Page.IsPostBack; }
  }

  [Category ("Style")]
  public DropDownListStyle AdditionalColumnsListStyle
  {
    get { return _additionalColumnsListStyle; }
    set { _additionalColumnsListStyle = value; }
  }

  /// <summary> CSS-Class applied to the <see cref="BocList"/>'s <c>table</c> tag. </summary>
  /// <remarks> Class: <c>bocListTable</c> </remarks>
  protected virtual string CssClassTable
  { get { return "bocListTable"; } }

  /// <summary> CSS-Class applied to the cells in the <see cref="BocList"/>'s title row. </summary>
  /// <remarks> Class: <c>bocListTitleCell</c> </remarks>
  protected virtual string CssClassTitleCell
  { get { return "bocListTitleCell"; } }

  /// <summary> CSS-Class applied to the cells in the <see cref="BocList"/>'s odd data rows. </summary>
  /// <remarks> Class: <c>bocListDataCellOdd</c> </remarks>
  protected virtual string CssClassDataCellOdd
  { get { return "bocListDataCellOdd"; } }

  /// <summary> CSS-Class applied to the cells in the <see cref="BocList"/>'s even data rows. </summary>
  /// <remarks> Class: <c>bocListDataCellEven</c> </remarks>
  protected virtual string CssClassDataCellEven
  { get { return "bocListDataCellEven"; } }

  /// <summary> CSS-Class applied to the <see cref="BocList"/>'s navigator. </summary>
  /// <remarks> Class: <c>bocListNavigator</c> </remarks>
  protected virtual string CssClassNavigator
  { get { return "bocListNavigator"; } }
}


}