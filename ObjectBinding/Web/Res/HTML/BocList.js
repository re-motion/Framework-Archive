//  BocListe.js contains client side scripts used by BocList.

//  The array indices identifying the row, the checkBox and the isOdd-flag
//  when collecting these values in an array. Simulates .net Framework's System.Triplet class.
var _bocList_rowKey = 0;
var _bocList_checkBoxKey = 1;
var _bocList_isOddKey = 2;

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
//  firstRow: The index of the first data row in the BocList.
//  count: The number of data rows in the BocList.
function BocList_InitializeList (bocList, checkBoxPrefix, firstRow, count)
{
  var selectedRows = new Array();
  var selectedRowsLength = 0
  for (var i = 0, rowNumber = firstRow, isOdd = true; i < count; i++, rowNumber++, isOdd = !isOdd)
  {
    var checkBoxID = checkBoxPrefix + rowNumber;
    var checkBox = document.getElementById (checkBoxID);
    if (checkBox.checked)      
    {
      var rowBlock = new Array();
      rowBlock[_bocList_rowKey] = row;
      rowBlock[_bocList_checkBoxKey] = checkBox;
      rowBlock[_bocList_isOddKey] = isOdd;
      selectedRows[row.id] = rowBlock;
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
  
  var selectedRows = _bocList_selectedRows[bocList.id];
  var selectedRowsLength = _bocList_selectedRowsLength[bocList.id];
  var className; //  The css-class
  var isCtrlKeyPress = window.event.ctrlKey;
  
  if (isCtrlKeyPress || _bocList_isCheckBoxClick)
  {
    if (selectedRows[currentRow.id] == null)
    {
      //  Add currentRow to list and select it
      BocList_SelectRow (bocList, currentRow, checkBox, isOdd);
    }
    else
    {
      //  Remove currentRow to list and unselect it
      BocList_UnselectRow (bocList, currentRow, checkBox, isOdd);
    }
  }
  else // ! isCtrlKeyPress
  {
    if (selectedRowsLength > 1)
    {
      //  Unselect all rows and clear the list
      BocList_UnselectAllRows (bocList);
      //  Add currentRow to list and select it
      BocList_SelectRow (bocList, currentRow, checkBox, isOdd);
    }
    else if (selectedRowsLength == 1)
    {
      //  Unselect row and clear the list
      BocList_UnselectAllRows (bocList);
      if (selectedRows[currentRow.id] == null)
      {
        //  Add current row to list and select it
        BocList_SelectRow (bocList, currentRow, checkBox, isOdd);
      }
    }
    else
    {
      //  Add currentRow to list and select it
      BocList_SelectRow (bocList, currentRow, checkBox, isOdd);
    }
  }
  checkBox.focus();
  _bocList_isCheckBoxClick = false;
}

//  Selects a row.
//  Adds the row to the _bocList_selectedRows array and increments _bocList_selectedRowsLength.
//  bocList: The BocList to which the row belongs.
//  currentRow: The row to be selected.
//  checkBox: The selection checkBox in this row.
//  isOdd: True if it is an odd data row, otherwise false.
function BocList_SelectRow (bocList, currentRow, checkBox, isOdd)
{
  //  Add currentRow to list
  var currentRowBlock = new Array();
  currentRowBlock[_bocList_rowKey] = currentRow;
  currentRowBlock[_bocList_checkBoxKey] = checkBox;
  currentRowBlock[_bocList_isOddKey] = isOdd;
  
  _bocList_selectedRows[bocList.id][currentRow.id] = currentRowBlock;
  _bocList_selectedRowsLength[bocList.id]++;
    
  // Select currentRow
  var className;
  if (isOdd)
    className = _bocList_TdOddClassNameSelected;
  else
    className = _bocList_TdEvenClassNameSelected;
  for (var i = 0; i < currentRow.children.length; i++)
    currentRow.children[i].className = className;

  checkBox.checked = true;
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
      BocList_UnselectRow (
          bocList,
          rowBlock[_bocList_rowKey],
          rowBlock[_bocList_checkBoxKey],
          rowBlock[_bocList_isOddKey]);
    }
  }
  
  //  Start over with a new array
  selectedRows = new Array();
  _bocList_selectedRowsLength[bocList.id] = 0;
}

//  Unselects a row.
//  Removes the row frin the _bocList_selectedRows array and decrements _bocList_selectedRowsLength.
//  bocList: The BocList to which the row belongs.
//  currentRow: The row to be unselected.
//  checkBox: The selection checkBox in this row.
//  isOdd: True if it is an odd data row, otherwise false.
function BocList_UnselectRow (bocList, currentRow, checkBox, isOdd)
{
  //  Remove currentRow from list
  _bocList_selectedRows[bocList.id][currentRow.id] = null;
  _bocList_selectedRowsLength[bocList.id]--;
  
  // Select currentRow
  var className;
  if (isOdd)
    className = _bocList_TdOddClassName;
  else
    className = _bocList_TdEvenClassName;
  for (var i = 0; i < currentRow.children.length; i++)
    currentRow.children[i].className = className;
  
  checkBox.checked = false;
}

//  Event handler for the selection checkBox in the title row.
//  Applies the checked state of the title's checkBox to all data rows' selectu=ion checkBoxes.
//  bocList: The BocList to which the checkBox belongs.
//  checkBoxPrefix: The common part of the checkBoxes' ID (everything before the index).
//  firstRow: The index of the first data row in the BocList.
//  count: The number of data rows in the BocList.
function BocList_OnSelectAllCheckBoxClick (
    bocList, 
    selectAllCheckBox, 
    checkBoxPrefix,
    firstRow,
    count)
{
  //  BocList_SelectRow will increment the length, therefor initialize it to zero.
  if (selectAllCheckBox.checked)      
    _bocList_selectedRowsLength[bocList.id] = 0;

  for (var i = 0, rowNumber = firstRow, isOdd = true; i < count; i++, rowNumber++, isOdd = !isOdd)
  {
    var rowID = rowPrefix + rowNumber;
    var checkBoxID = checkBoxPrefix + rowNumber;
    var checkBox = document.getElementById (checkBoxID);
    if (selectAllCheckBox.checked)      
      BocList_SelectRow (bocList, row, checkBox, isOdd)
    else
      BocList_UnselectRow (bocList, row, checkBox, isOdd)
  }
  
  if (! selectAllCheckBox.checked)      
    _bocList_selectedRowsLength[bocList.id] = 0;
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