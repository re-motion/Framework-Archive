//  BocListe.js contains client side scripts used by BocList.

//  The css classes used for odd and even rows in their selected and unselected state.
var _bocList_TdOddClassName = '';
var _bocList_TdEvenClassName = '';
var _bocList_TdOddClassNameSelected = '';
var _bocList_TdEvenClassNameSelected = '';

//  A two-dimensional associative array containing the selected rows 
//  grouped by the BocList instances.
//  First Vector: BocList ID
//  Second Vector: Selected rows (non-null values) for this BocList
var _bocList_selectedRows = new Array();
//  An associative array containing the number of selected rows (non-null values) for each BocList.
var _bocList_selectedRowsLength = new Array();

//  A flag that indicates that the OnClick event for a selection checkBox has been raised
//  prior to the row's OnClick event.
var _bocList_isCheckBoxClick = false;
  
//  A flag that indicates that the OnClick event for an anchor tag (command) has been raised
//  prior to the row's OnClick event.
var _bocList_isCommandClick = false;

var _bocList_listMenuInfos = new Array();

var _contentMenu_itemClassName = 'contentMenuItem';
var _contentMenu_itemFocusClassName = 'contentMenuItemFocus';
var _contentMenu_itemDisabledClassName = 'contentMenuItemDisabled';
var _contentMenu_requiredSelectionAny = 0;
var _contentMenu_requiredSelectionExactlyOne = 1;
var _contentMenu_requiredSelectionOneOrMore = 2;

function BocList_RowBlock (row, checkBox, isOdd)
{
  this.Row = row;
  this.CheckBox = checkBox;
  this.IsOdd = isOdd;
}

//  Initializes the class names of the css classes used to format the table cells.
//  Call this method once in a startup script.
function BocList_InitializeGlobals (
  tdOddClassName, 
  tdEvenClassName, 
  tdOddClassNameSelected,
  tdEvenClassNameSelected)
{
  _bocList_TdOddClassName = tdOddClassName;
  _bocList_TdEvenClassName = tdEvenClassName;
  _bocList_TdOddClassNameSelected = tdOddClassNameSelected;
  _bocList_TdEvenClassNameSelected = tdEvenClassNameSelected;
}

//  Initalizes an individual BocList's List. The initialization synchronizes the selection state 
//  arrays with the BocList's selected rows.
//  Call this method once for each BocList on the page.
//  bocList: The BocList to which the row belongs.
//  checkBoxPrefix: The common part of the checkBoxes' ID (everything before the index).
//  count: The number of data rows in the BocList.
function BocList_InitializeList (bocList, checkBoxPrefix, count)
{
  var selectedRows = new Array();
  var selectedRowsLength = 0
  for (var i = 0, isOdd = true; i < count; i++, isOdd = !isOdd)
  {
    var checkBoxID = checkBoxPrefix + i;
    var checkBox = document.getElementById (checkBoxID);
    if (checkBox == null)
      continue;
    var row =  checkBox.parentElement.parentElement;
    if (checkBox.checked)      
    {
      var rowBlock = new BocList_RowBlock (row, checkBox, isOdd);
      selectedRows[checkBox.id] = rowBlock;
      selectedRowsLength++;
    }
  }
  _bocList_selectedRows[bocList.id] = selectedRows;
  _bocList_selectedRowsLength[bocList.id] = selectedRowsLength;
}

//  Event handler for a table row in the BocList. 
//  Selects/unselects a row/all rows depending on it's selection state,
//      whether CTRL has been pressed and if _bocList_isCheckBoxClick is true.
//  Aborts the execution if _bocList_isCommandClick is true.
//  bocList: The BocList to which the row belongs.
//  currentRow: The row that fired the click event.
//  checkBox: The selection checkBox in this row.
//  isOdd: True if it is an odd data row, otherwise false.
function BocList_OnRowClick (bocList, currentRow, checkBox, isOdd)
{
  if (_bocList_isCommandClick)
  {
    _bocList_isCommandClick = false;
    return;
  }  
  
  var currentRowBlock = new BocList_RowBlock (currentRow, checkBox, isOdd);
  var selectedRows = _bocList_selectedRows[bocList.id];
  var selectedRowsLength = _bocList_selectedRowsLength[bocList.id];
  var className; //  The css-class
  var isCtrlKeyPress = window.event.ctrlKey;
  
  if (isCtrlKeyPress || _bocList_isCheckBoxClick)
  {
    if (selectedRows[checkBox.id] == null)
    {
      //  Add currentRow to list and select it
      BocList_SelectRow (bocList, currentRowBlock);
    }
    else
    {
      //  Remove currentRow to list and unselect it
      BocList_UnselectRow (bocList, currentRowBlock);
    }
  }
  else // ! isCtrlKeyPress
  {
    if (selectedRowsLength > 1)
    {
      //  Unselect all rows and clear the list
      BocList_UnselectAllRows (bocList);
      //  Add currentRow to list and select it
      BocList_SelectRow (bocList, currentRowBlock);
    }
    else if (selectedRowsLength == 1)
    {
      //  Unselect row and clear the list
      BocList_UnselectAllRows (bocList);
      if (selectedRows[checkBox.id] == null)
      {
        //  Add current row to list and select it
        BocList_SelectRow (bocList, currentRowBlock);
      }
    }
    else
    {
      //  Add currentRow to list and select it
      BocList_SelectRow (bocList, currentRowBlock);
    }
  }
  checkBox.focus();
  _bocList_isCheckBoxClick = false;

  BocList_UpdateListMenu (bocList);
}

//  Selects a row.
//  Adds the row to the _bocList_selectedRows array and increments _bocList_selectedRowsLength.
//  bocList: The BocList to which the row belongs.
//  rowBlock: The row to be selected.
function BocList_SelectRow (bocList, rowBlock)
{
  //  Add currentRow to list  
  _bocList_selectedRows[bocList.id][rowBlock.CheckBox.id] = rowBlock;
  _bocList_selectedRowsLength[bocList.id]++;
    
  // Select currentRow
  var className;
  if (rowBlock.IsOdd)
    className = _bocList_TdOddClassNameSelected;
  else
    className = _bocList_TdEvenClassNameSelected;
  for (var i = 0; i < rowBlock.Row.children.length; i++)
    rowBlock.Row.children[i].className = className;

  rowBlock.CheckBox.checked = true;
}

//  Unselects all rows in a BocList.
//  Clears _bocList_selectedRows array and sets _bocList_selectedRowsLength to zero.
//  bocList: The BocList whose rows should be unselected.
function BocList_UnselectAllRows (bocList)
{
  var selectedRows = _bocList_selectedRows[bocList.id];
  for (var rowID in selectedRows)
  {
    var rowBlock = selectedRows[rowID];
    if (rowBlock != null)
    {
      BocList_UnselectRow (bocList, rowBlock);
    }
  }
  
  //  Start over with a new array
  selectedRows = new Array();
  _bocList_selectedRowsLength[bocList.id] = 0;
}

//  Unselects a row.
//  Removes the row frin the _bocList_selectedRows array and decrements _bocList_selectedRowsLength.
//  bocList: The BocList to which the row belongs.
//  rowBlock: The row to be unselected.
function BocList_UnselectRow (bocList, rowBlock)
{
  //  Remove currentRow from list
  _bocList_selectedRows[bocList.id][rowBlock.CheckBox.id] = null;
  _bocList_selectedRowsLength[bocList.id]--;
  
  // Select currentRow
  var className;
  if (rowBlock.IsOdd)
    className = _bocList_TdOddClassName;
  else
    className = _bocList_TdEvenClassName;
  for (var i = 0; i < rowBlock.Row.children.length; i++)
    rowBlock.Row.children[i].className = className;
  
  rowBlock.CheckBox.checked = false;
}

//  Event handler for the selection checkBox in the title row.
//  Applies the checked state of the title's checkBox to all data rows' selectu=ion checkBoxes.
//  bocList: The BocList to which the checkBox belongs.
//  checkBoxPrefix: The common part of the checkBoxes' ID (everything before the index).
//  count: The number of data rows in the BocList.
function BocList_OnSelectAllCheckBoxClick (bocList, selectAllCheckBox, checkBoxPrefix, count)
{
  //  BocList_SelectRow will increment the length, therefor initialize it to zero.
  if (selectAllCheckBox.checked)      
    _bocList_selectedRowsLength[bocList.id] = 0;

  for (var i = 0, isOdd = true; i < count; i++, isOdd = !isOdd)
  {
    var checkBoxID = checkBoxPrefix + i;
    var checkBox = document.getElementById (checkBoxID);
    if (checkBox == null)
      continue;
    var row =  checkBox.parentElement.parentElement;
    var rowBlock = new BocList_RowBlock (row, checkBox, isOdd);
    if (selectAllCheckBox.checked)      
      BocList_SelectRow (bocList, rowBlock)
    else
      BocList_UnselectRow (bocList, rowBlock)
  }
  
  if (! selectAllCheckBox.checked)      
    _bocList_selectedRowsLength[bocList.id] = 0;
    
  BocList_UpdateListMenu (bocList);
}

//  Event handler for the selection checkBox in a data row.
//  Sets the _bocList_isCheckBoxClick flag.
function BocList_OnSelectionCheckBoxClick()
{
  _bocList_isCheckBoxClick = true;
}

//  Event handler for the anchor tags (commands) in a data row.
//  Sets the _bocList_isCommandClick flag.
function BocList_OnCommandClick()
{
  _bocList_isCommandClick = true;
}

//  Returns the number of rows selected for the specified BocList
function BocList_GetSelectionCount (bocListID)
{
  var selectionCount = _bocList_selectedRowsLength[bocListID];
  if (selectionCount == null)
    return 0;
  return selectionCount;
}

function ContentMenu_MenuInfo (id, itemInfos)
{
  this.ID = id;
  this.ItemInfos = itemInfos;
}

function BocList_AddMenuInfo (bocList, menuInfo)
{
  _bocList_listMenuInfos[bocList.ID] = menuInfo;
}

function ContentMenu_MenuItemInfo (id, category, text, icon, iconDisabled, requiredSelection, href, target)
{
  this.ID = id;
  this.Category = category;
  this.Text = text;
  this.Icon = icon;
  this.IconDisabled = iconDisabled;
  this.RequiredSelection = requiredSelection;
  this.Href = href;
  this.Target = target;
}

function BocList_UpdateListMenu (bocList)
{
  var menuInfo = _bocList_listMenuInfos[bocList.id];
  if (menuInfo == null)
    return;
    
  var itemInfos = menuInfo.ItemInfos;
  var selectionCount = BocList_GetSelectionCount (bocList.id);
  
  for (var i = 0; i < itemInfos.length; i++)
  {
    var itemInfo = itemInfos[i];
    var isEnabled = true;
    if (   itemInfo.RequiredSelection == _contentMenu_requiredSelectionExactlyOne
        && selectionCount != 1)
    {
      isEnabled = false;
    }
    if (   itemInfo.RequiredSelection == _contentMenu_requiredSelectionOneOrMore
        && selectionCount < 1)
    {
      isEnabled = false;
    }
    
    var item = document.getElementById (itemInfo.ID);
    var icon = item.children[0];
    if (isEnabled)
    {
      if (icon != null)
        icon.src = itemInfo.Icon;
  	  item.className = _contentMenu_itemClassName;
  	  item.setAttribute ('href', itemInfo.Href);
      item.onclick = function () { ContentMenu_GoTo (this); };
	    item.onmouseover = function () { ContentMenu_SelectItem (this); };
	    item.onmouseleave = function () { ContentMenu_UnselectItem (this); };
    }
    else
    {
      if (icon != null)
        icon.src = itemInfo.IconDisabled;
      item.className = _contentMenu_itemDisabledClassName;
      item.onclick = null;
	    item.onmouseover = null;
	    item.onmouseleave = null;
    }
  }
}
  
function ContentMenu_GoTo (menuItem)
{
  window.location = menuItem.href;
}

function ContentMenu_SelectItem (menuItem)
{
	if (menuItem == null)
	  return;
	menuItem.className = _contentMenu_itemFocusClassName;
}

function ContentMenu_UnselectItem (menuItem)
{
	if (menuItem == null)
	  return;
	menuItem.className = _contentMenu_itemClassName;
}