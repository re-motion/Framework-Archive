using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing.Design;
using System.ComponentModel.Design;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;
using Rubicon.Web.UI.Utilities;
using Rubicon.ObjectBinding.Web.Design;

namespace Rubicon.ObjectBinding.Web.Controls
{

[ToolboxItemFilter("System.Web.UI")]
[Obsolete ("BocList implementation not completed")]
public class BocList: BusinessObjectBoundModifiableWebControl
{
  //  constants
  private const string c_dataRowCheckBoxIDSuffix = "_CheckBox_";
  private const string c_headerRowCheckBoxIDSuffix = "_CheckBox_SelectAll";

  private const string c_goToDisplayCurrentPage = "Seite {0} von {1}";
  private const string c_goToFirstLabel = "|<<";
  private const string c_goToLastLabel = ">>|";
  private const string c_goToPreviousLabel = "<";
  private const string c_goToNextLabel = ">";
  private const string c_goToCommand = "GoTo";
  private const string c_goToCommandFormatString = c_goToCommand + "={0}";

  private const string c_whiteSpace = "&nbsp;";

  /// <summary> 
  ///   Text displayed when control is displayed in desinger and is read-only has no contents.
  /// </summary>
  private const string c_designModeEmptyContents = "#";

  // types
  
  private enum GoToOption
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

	// member fields

  /// <summary></summary>
  private bool _isDirty = true;

  /// <summary> The <see cref="DropDownList"/> used to select the column configuration. </summary>
  private DropDownList _additionalColumnsList = null;

  private LinkButton goToFirstButton = null;
  private LinkButton goToLastButton = null;
  private LinkButton goToPreviousButton = null;
  private LinkButton goToNextButton = null;

  private IList _value = null;

  /// <summary> The <see cref="Style"/> applied to the <see cref="_additionalColumnsList"/>. </summary>
  private DropDownListStyle _additionalColumnsListStyle = null;

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

  private GoToOption _goTo = GoToOption.Undefined;

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
    goToFirstButton = new LinkButton();
    goToLastButton = new LinkButton();
    goToPreviousButton = new LinkButton();
    goToNextButton = new LinkButton();

    _additionalColumnsList.ID = this.ID + "_ColumnConfigurationList";
    _additionalColumnsList.EnableViewState = true;
    _additionalColumnsList.SelectedIndexChanged += new EventHandler(AdditionalColumnsList_SelectedIndexChanged);
    Controls.Add (_additionalColumnsList);

    goToFirstButton.Text = c_goToFirstLabel;
    goToFirstButton.Click += new EventHandler (GoToFirstButton_Click);
    Controls.Add (goToFirstButton);

    goToLastButton.Text = c_goToLastLabel;
    goToLastButton.Click += new EventHandler (GoToLastButton_Click);
    Controls.Add (goToLastButton);

    goToPreviousButton.Text = c_goToPreviousLabel;
    goToPreviousButton.Click += new EventHandler (GoToPreviousButton_Click);
    Controls.Add (goToPreviousButton);

    goToNextButton.Text = c_goToNextLabel;
    goToNextButton.Click += new EventHandler (GoToNextButton_Click);
    Controls.Add (goToNextButton);

    Binding.BindingChanged += new EventHandler (Binding_BindingChanged);

    if (! IsPostBack)
      PopulateAdditionalColumnsList();
    _availableColumnDefinitionSets.CollectionChanged += new CollectionChangeEventHandler(AvailableColumnDefinitionSets_CollectionChanged);
      
    if (IsPostBack && Page != null)
    {
      string dataRowCheckBoxFilter = ID + c_dataRowCheckBoxIDSuffix;
      string headerRowCheckBoxFilter = ID + c_headerRowCheckBoxIDSuffix;
      NameValueCollection formVariables = Page.Request.Form;

      for (int i = 0; i < formVariables.Count; i++)
      {
        string key = formVariables.Keys[i];

        if (key.StartsWith (dataRowCheckBoxFilter))
          _checkBoxCheckedState[key] = true; 
        else if (key.StartsWith (headerRowCheckBoxFilter))
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

      if (columnDefinitions.Length == 0)
      {
        columnDefinitions = new BocColumnDefinition[] {
          new BocDesignerColumnDefinition ("#", Unit.Empty),
          new BocDesignerColumnDefinition ("#", Unit.Empty),
          new BocDesignerColumnDefinition ("#", Unit.Empty)};
      }
    }
 
    RenderHeader (writer);
    RenderTableOpeningTag (writer);
    RenderColGroup (writer, columnDefinitions);
    RenderColumnHeadersRow (writer, columnDefinitions);

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
      for (int idxRow = firstRow; idxRow < rowCountWithOffset; idxRow++)
        RenderDataRow (writer, columnDefinitions, idxRow);
    }

    RenderTableClosingTag (writer);
    if (_alwaysShowPageInfo || _pageCount > 1)
      RenderNavigator (writer);
  }

  private void RenderHeader (HtmlTextWriter writer)
  {
    if (_showAdditionalColumnsList)
    _additionalColumnsList.RenderControl (writer);
  }

  private void RenderNavigator (HtmlTextWriter writer)
  {
    bool isFirstPage = _currentPage == 0;
    bool isLastPage = _currentPage + 1 >= _pageCount;

    writer.Write (c_goToDisplayCurrentPage, _currentPage + 1, _pageCount);
    writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);

    if (isFirstPage)
      Controls.Remove (goToFirstButton);
    goToFirstButton.RenderControl (writer);
    writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);

    if (isFirstPage)
      Controls.Remove (goToPreviousButton);
    goToPreviousButton.RenderControl (writer);
    writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);

    if (isLastPage)
      Controls.Remove (goToNextButton);
    goToNextButton.RenderControl (writer);
    writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);

    if (isLastPage)
      Controls.Remove (goToLastButton);
    goToLastButton.RenderControl (writer);
  }

  private void RenderTableOpeningTag (HtmlTextWriter writer)
  {
    if (! Width.IsEmpty)
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Width.ToString());
    writer.AddAttribute (HtmlTextWriterAttribute.Border, "1");
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
      writer.RenderEndTag ();
    }

    foreach (BocColumnDefinition column in columnDefinitions)
    {
      if (! column.Width.IsEmpty)
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, column.Width.ToString());

      writer.RenderBeginTag (HtmlTextWriterTag.Col);
      writer.RenderEndTag ();
    }
    
    writer.RenderEndTag ();
  }

  private void RenderColumnHeadersRow (HtmlTextWriter writer, BocColumnDefinition[] columnDefinitions)
  {
    writer.RenderBeginTag (HtmlTextWriterTag.Tr);

    if (ShowSelection)
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      string checkBoxName = ID + c_headerRowCheckBoxIDSuffix;
      bool isChecked = (_checkBoxCheckedState[checkBoxName] != null);
      RenderCheckBox (writer, checkBoxName, isChecked);
      writer.RenderEndTag();
    }

    foreach (BocColumnDefinition column in columnDefinitions)
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Td);

      if (IsDesignMode && column.ColumnHeaderDisplayValue.Length == 0)
          writer.Write (c_designModeEmptyContents);
      else
        HttpUtility.HtmlEncode (column.ColumnHeaderDisplayValue, writer);

      writer.RenderEndTag ();
    }
    
    writer.RenderEndTag ();
  }

  private void RenderDataRow (HtmlTextWriter writer, BocColumnDefinition[] columnDefinitions, int rowIndex)
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

    bool isFirstValueColumnRendered = false;

    if (ShowSelection)
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      string checkBoxName = ID + c_dataRowCheckBoxIDSuffix + rowIndex.ToString();
      bool isChecked = (_checkBoxCheckedState[checkBoxName] != null);
      RenderCheckBox (writer, checkBoxName, isChecked);
      writer.RenderEndTag();
    }

    foreach (BocColumnDefinition column in columnDefinitions)
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Td);

      BocCommandColumnDefinition commandColumn = column as BocCommandColumnDefinition;
      BocCompoundColumnDefinition compoundColumn = column as BocCompoundColumnDefinition;
      BocSimpleColumnDefinition simpleColumn = column as BocSimpleColumnDefinition;
      BocValueColumnDefinition valueColumn = column as BocValueColumnDefinition;

      if (! isFirstValueColumnRendered && valueColumn != null)
      {
        //  Render FirstColumnCommand BeginTag
        if (FirstColumnCommand != null)
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
        
        commandColumn.Command.RenderEnd (writer);
      }
      else if (compoundColumn != null)
      {
        writer.Write (compoundColumn.GetStringValue (businessObject));
      }
      else if (simpleColumn != null)
      {
        writer.Write (simpleColumn.GetStringValue (businessObject));
      }

      if (! isFirstValueColumnRendered && valueColumn != null)
      {
        //  Render FirstColumnCommand EndTag
        if (FirstColumnCommand != null)
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
    //  TODO: BindingChanged
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

  private void GoToFirstButton_Click(object sender, EventArgs e)
  {
    _goTo = GoToOption.First;
  }

  private void GoToLastButton_Click(object sender, EventArgs e)
  {
    _goTo = GoToOption.Last;
  }
  
  private void GoToPreviousButton_Click(object sender, EventArgs e)
  {
    _goTo = GoToOption.Previous;
  }
  
  private void GoToNextButton_Click(object sender, EventArgs e)
  {
    _goTo = GoToOption.Next;
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

  [Category ("Column Definition")]
  [Description ("The ItemCommand added to the first value column.")]
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

 // [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode(PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Category ("Column Definition")]
  [Description ("The predefined column defintion sets that the user can choose from at run-time.")]
  [DefaultValue ((string) null)]
  public BocColumnDefinitionSetCollection AvailableColumnDefinitionSets
  {
    get { return _availableColumnDefinitionSets; }
  }

  private void AvailableColumnDefinitionSets_CollectionChanged(object sender, CollectionChangeEventArgs e)
  {
    PopulateAdditionalColumnsList();
  }

  //[Category ("Column Definition")]
  //[Description ("The active column definition set")]
  [Browsable (false)]
  //  TODO: Designer support for selecting a ColumnDefinitionSet
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
  [Description ("Set true to enable the icon in front of the first value column")]
  [DefaultValue (true)]
  public bool EnableIcon
  {
    get { return _enableIcon; }
    set
    { _enableIcon = value; }
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
}


}