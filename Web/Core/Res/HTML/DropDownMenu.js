var _dropDownMenu_menuInfos = new Array();

var _dropDownMenu_headClassName = 'dropDownMenuHead';
var _dropDownMenu_headFocusClassName = 'dropDownMenuHeadFocus';
var _dropDownMenu_popUpClassName = 'dropDownMenuPopUp';
var _dropDownMenu_itemClassName = 'dropDownMenuItem';
var _dropDownMenu_itemFocusClassName = 'dropDownMenuItemFocus';
var _dropDownMenu_itemDisabledClassName = 'dropDownMenuItemDisabled';
var _dropDownMenu_itemTextPaneClassName = 'dropDownMenuItemTextPane';
var _dropDownMenu_itemIconPaneClassName = 'dropDownMenuItemIconPane';
var _dropDownMenu_itemSeparatorClassName = 'dropDownMenuTextPaneSeparator';
var _dropDownMenu_currentMenu = null;
var _dropDownMenu_currentPopUp = null;
var _dropDownMenu_currentPopUpWidth = null;

var _dropDownMenu_styleSheetLink = null;
var _dropDownMenu_menuItemIDPrefix = 'menuItem_';
var _dropDownMenu_currentItem = null;

var _dropDownMenu_requiredSelectionAny = 0;
var _dropDownMenu_requiredSelectionExactlyOne = 1;
var _dropDownMenu_requiredSelectionOneOrMore = 2;

function DropDownMenu_InitializeGlobals (styleSheetLink)
{
  _dropDownMenu_styleSheetLink = styleSheetLink;
}

function DropDownMenu_MenuInfo (id, itemInfos)
{
  this.ID = id;
  this.ItemInfos = itemInfos;
}

function DropDownMenu_AddMenuInfo (menuInfo)
{
  _dropDownMenu_menuInfos[menuInfo.ID] = menuInfo;
}

function DropDownMenu_ItemInfo (id, category, text, icon, iconDisabled, requiredSelection, href, target)
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

function DropDownMenu_OnClick (context, menuID, getSelectionCount)
{
  var id = context.id + '_PopUp';
  if (context != _dropDownMenu_currentMenu)
    DropDownMenu_ClosePopUp ();
  _dropDownMenu_currentPopUp = DropDownMenu_OpenPopUp (id, menuID, context, getSelectionCount);
	_dropDownMenu_currentMenu = context;
}

function DropDownMenu_SetSelectionCount (count)
{
}

function DropDownMenu_OpenPopUp (id, menuID, context, getSelectionCount)
{
  var itemInfos = _dropDownMenu_menuInfos[menuID].ItemInfos;
  var selectionCount = -1;
  if (getSelectionCount != null)
    selectionCount = getSelectionCount();
  //  Create a temporary popup for calculating the size
	var tempPopUp = window.document.createElement("div");
	tempPopUp.className = _dropDownMenu_popUpClassName;
	
	for (var index in itemInfos)
	{
	  var item = DropDownMenu_CreateItem (window.document, itemInfos[index], selectionCount, false);
	  if(item != null)
  	  tempPopUp.appendChild (item);
	}
	
	var tempPopUpWindow = window.document.createElement("div");
	tempPopUpWindow.style.position = 'absolute';
	tempPopUpWindow.style.left = 0;
	tempPopUpWindow.style.top = 0;
  tempPopUpWindow.appendChild (tempPopUp);
	window.document.body.appendChild (tempPopUpWindow);
	var popUpWidth	= tempPopUp.offsetWidth + tempPopUp.offsetLeft;
	var popUpHeight = tempPopUp.offsetHeight + tempPopUp.offsetTop;
	window.document.body.removeChild (tempPopUpWindow);

  //  IE55up
  popUpWindow = window.createPopup();
  var popUpDocument = popUpWindow.document;
  var popUpBody = popUpWindow.document.body

  var popUpStyleSheet = popUpDocument.createStyleSheet();
  for (var idxStyleSheets = 0; idxStyleSheets < window.document.styleSheets.length; idxStyleSheets++)
  {
		styleSheet = window.document.styleSheets[idxStyleSheets];
		var isDropDownStyleSheet = styleSheet.href.toLowerCase() == _dropDownMenu_styleSheetLink.toLowerCase();
    if (isDropDownStyleSheet)
    {
      popUpDocument.createStyleSheet (_dropDownMenu_styleSheetLink);
    }
    else
    {
	    for (var idxRules = 0; idxRules < styleSheet.rules.length; idxRules++)
	    {
		    var rule = styleSheet.rules[idxRules];
		    var isBody = rule.selectorText.toLowerCase() == 'body';
        if (isBody)
        {
		      cssText = rule.style.cssText;
		      if (cssText != null && cssText != '')
  		      popUpStyleSheet.addRule (rule.selectorText, cssText);
			  }
		  }
		}
  }
	  
	var popUp = popUpDocument.createElement("div");
	if(id != null)
	  popUp.id = id;
	popUp.className = _dropDownMenu_popUpClassName;
  var itemInfos = _dropDownMenu_menuInfos[menuID].ItemInfos;
	for (var index in itemInfos)
	{
	  var item = DropDownMenu_CreateItem (popUpDocument, itemInfos[index], selectionCount, true);
	  if(item != null)
  	  popUp.appendChild (item);
	}
  popUpBody.appendChild (popUp);
	//  Brower Switch
	//  Css2.1
	//  DropDownMenu_AppendChild (document.body, popUp);
	//  _dropDownMenu_currentPopUpWidth = popUp.offsetWidth;
  //	DropDownMenu_RepositionPopUp ();
  //	window.onresize = DropDownMenu_RepositionPopUp;
	
	popUpBody.oncontextmenu = kfnDisableEvent;
	popUpBody.ondragstart = kfnDisableEvent;
	popUpBody.onselectstart = kfnDisableEvent;
	popUpBody.onkeydown = PopupKeyDown;
	popUpBody.onmouseup = PopupMouseUp;
	popUpBody.onmouseover = PopupMouseOver;
	popUpBody.onmouseleave = PopupMouseLeave;

	var parentLeft = 0;
	var currentNode;
	for (currentNode = context; 
	     currentNode != null && currentNode != window.document.body; 
	     currentNode = currentNode.offsetParent)
  {
		parentLeft += currentNode.offsetLeft - currentNode.scrollLeft;
	}
	if (currentNode != null)
	  parentLeft += currentNode.clientLeft - currentNode.scrollLeft;
	parentLeft += window.screenLeft;
	var parentWidth = (context != null) ? context.offsetWidth : 0;
	var left = parentWidth - popUpWidth;
	var top = context.offsetHeight; 
	//  popUpHeight + 7: corrective offset, reason unkown but it works
	popUpWindow.show(left, top, popUpWidth, popUpHeight, context);

	_oContents = _dropDownMenu_currentPopUp;
  _oRoot = _oContents;
  _arrSelected[_nLevel] = null;
  _wzPrefixID = popUp.uniqueID;
  
	return popUp;
}
function DropDownMenu_OpenPopUpEvent()
{
	var popUp=event.srcElement;
	if(!popUp.isOpen())
	  popUp.show (_dropDownMenu_currentMenu, null, null, -1);
}

function DropDownMenu_RepositionPopUp()
{
  var top = 0 + _dropDownMenu_currentMenu.offsetHeight;
  var left = 0 + _dropDownMenu_currentMenu.offsetWidth;
  //  Calculate the offset of the div in respect to the left top corner of the page.
  for (var currentNode = _dropDownMenu_currentMenu; 
      currentNode && (currentNode.tagName != 'BODY'); 
      currentNode = currentNode.offsetParent)
  {
    left += currentNode.offsetLeft;
    top += currentNode.offsetTop;
  }
  
	_dropDownMenu_currentPopUp.style.top = top + 'px';
	_dropDownMenu_currentPopUp.style.left = (left - _dropDownMenu_currentPopUpWidth) + 'px';
}

function DropDownMenu_ClosePopUp()
{
  if (_dropDownMenu_currentPopUp != null && _dropDownMenu_currentPopUp.document.window != null)
    _dropDownMenu_currentPopUp.document.window.close();
  _dropDownMenu_currentPopUp = null;
  if (_dropDownMenu_currentMenu != null)
    DropDownMenu_OnHeadMouseOut (_dropDownMenu_currentMenu.children[0]);
  _dropDownMenu_currentMenu = null;
}

function DropDownMenu_CreateItem (popUpDocument, itemInfo, selectionCount, showExpandSeparators)
{
  if (itemInfo == null)
    return null;
	var item = popUpDocument.createElement ('div');
	if(item == null)
	  return null;

	if (itemInfo.Text == '-')
	  DropDownMenu_CreateSeparatorItem (popUpDocument, item, showExpandSeparators);
	 else
	  DropDownMenu_CreateTextItem (popUpDocument, item, itemInfo, selectionCount);
	  
	return item;
}

function DropDownMenu_CreateTextItem (popUpDocument, item, itemInfo, selectionCount)
{
  var isEnabled = true;
  if (   itemInfo.RequiredSelection == _dropDownMenu_requiredSelectionExactlyOne
      && selectionCount != 1)
  {
    isEnabled = false;
  }
  if (   itemInfo.RequiredSelection == _dropDownMenu_requiredSelectionOneOrMore
      && selectionCount < 1)
  {
    isEnabled = false;
  }
  
	item.id = itemInfo.ID;
	
	if (isEnabled)
  	item.className = _dropDownMenu_itemClassName;
  else
    item.className = _dropDownMenu_itemDisabledClassName;
    
	if (itemInfo.Href != null && isEnabled)
  	item.onclick = function () { window.location = itemInfo.Href; };

	var iconPane = popUpDocument.createElement ('span');
	iconPane.className = _dropDownMenu_itemIconPaneClassName;
	if (itemInfo.Icon != null)
	{
  	var icon = popUpDocument.createElement ('img');
  	if (isEnabled || itemInfo.IconDisabled == null)
  	  icon.src = itemInfo.Icon;
  	else
  	  icon.src = itemInfo.IconDisabled;
	  icon.style.verticalAlign = 'middle';
	  iconPane.appendChild (icon);
	}
	item.appendChild (iconPane);

	var textPane = popUpDocument.createElement ('span');
	textPane.className = _dropDownMenu_itemTextPaneClassName;	
  textPane.innerHTML = itemInfo.Text;
	item.appendChild (textPane);

  if (isEnabled)
  {
	  item.onmouseover = function () { DropDownMenu_SelectItem (this); };
	  item.onmouseleave = function () { DropDownMenu_UnselectItem (this); };
	}
}

function DropDownMenu_CreateSeparatorItem (popUpDocument, item, showExpandSeparators)
{
	var textPane = popUpDocument.createElement ('span');
	textPane.className = _dropDownMenu_itemSeparatorClassName;
	textPane.innerHTML = '&nbsp;';
	if (showExpandSeparators)
	  textPane.style.width = '100%';
	else
	  textPane.style.width = '5px';
	item.appendChild (textPane);
}

function DropDownMenu_SelectItem (menuItem)
{
	if (menuItem == null)
	  return;
	menuItem.className = _dropDownMenu_itemFocusClassName;
	_dropDownMenu_currentItem = menuItem;
}

function DropDownMenu_UnselectItem (menuItem)
{
	if (menuItem == null)
	  return;
	menuItem.className = _dropDownMenu_itemClassName;
	_dropDownMenu_currentItem = null;
}

function DropDownMenu_OnHeadMouseOver (head)
{
  if (_dropDownMenu_currentPopUp == null)
  	head.className = _dropDownMenu_headFocusClassName;
}

function DropDownMenu_OnHeadMouseOut (head)
{
  if (_dropDownMenu_currentPopUp == null)
  	head.className = _dropDownMenu_headClassName;
}

/* copy and pasted, contains stuff for the popUpBody event handlers. */
var kfnDisableEvent = new Function("return false");
var _nLevel = 0;
var _oContents;						
var _oRoot;
var _arrSelected = new Array();
var _wzPrefixID;		

function HideMenu()
{
	if (IsOpen()) _dropDownMenu_currentPopUp.hide();
}

function IsOpen()
{
	var oPopup = _dropDownMenu_currentPopUp;
	return oPopup && oPopup.isOpen;
}

function GetEventFromLevel(nLevel)
{
	if (nLevel >= 0 && nLevel <= _nLevel)
		{
		var oPopup = _dropDownMenu_currentPopUp;
		if (oPopup) return oPopup.document.parentWindow.event;
		}
	return null;	
}
function GetEventLevel()
{
	for (var nIndex = _nLevel; nIndex >= 0; nIndex--)
		if (GetEventFromLevel(nIndex)) return nIndex;
	return -1;
}
function UpdateLevel(nLevel)
{
	var oPopup;
	while (_nLevel > nLevel)
		{
		oPopup = _dropDownMenu_currentPopUp;
		if (oPopup)
			{
			ClearShowSubMenuEvnt();
			oPopup.hide();
			}
		_dropDownMenu_currentPopUp = null;
		_arrSelected[_nLevel] = null;
		_oRoot = _oRoot.parentNode;
		_nLevel--;
		}
	oPopup = _dropDownMenu_currentPopUp;
	if (oPopup) ClearShowSubMenuEvnt();
}

function ClearShowSubMenuEvnt()
{
return;
	var oPopup = _arrPopup[_nLevel];
	var oWnd = _nLevel == 0 ? window : _arrPopup[_nLevel - 1].document.parentWindow;
	if (oPopup && oWnd)
		{
		var oPopupBody = oPopup.document.body;
		var id = oPopupBody.getAttribute("timeoutID");
		if (typeof(id)=="number")
			{
			oWnd.clearTimeout(id);
			}
		oPopupBody.removeAttribute("timeoutID");
		}
}
function GetEventSrcItem(oEvent)
{
	if (oEvent)
	{
		for (var oSrc = oEvent.srcElement; oSrc && !FIStringEquals(oSrc.tagName, "BODY"); oSrc = oSrc.parentNode)
		{
			if (   FIStringEquals(oSrc.tagName, "DIV") 
			    && oSrc.id.substring(0, _dropDownMenu_menuItemIDPrefix.length) == _dropDownMenu_menuItemIDPrefix) 
			{
			  return oSrc;
			}
	  }
	}
	return null;
}
function PopupMouseOver()
{
	var nLevel = GetEventLevel();
	if (nLevel < 0) return;
	var oPopupEvent = GetEventFromLevel(nLevel);
	var oSrcElem = GetEventSrcItem(oPopupEvent);
	if (oSrcElem)
		{
		if (oSrcElem != _arrSelected[nLevel])
			{
			if (FIsIType(oSrcElem, "separator")) return;
			ToggleMenuItem(nLevel, oSrcElem);
			if (FIsIType(oSrcElem, "submenu")) EngageSelection(true, true, false);
			}
		else if (nLevel < _nLevel)
			{
			UnselectCurrentOption();
			}
		}
}
function PopupMouseLeave()
{
	if (GetEventLevel() == _nLevel) UnselectCurrentOption();
}
function PopupMouseUp()
{
	var nLevel = GetEventLevel();
	if (nLevel < 0) return;
	var oItem = _arrSelected[nLevel];
	if (GetEventFromLevel(nLevel).button == 1 || FIsIType(oItem, "submenu"))
		{
		UpdateLevel(nLevel);
		EngageSelection(true, false, false);
		}
}
function PopupKeyDown()
{
	var nLevel = GetEventLevel();
	if (nLevel < 0) return;
	var oPopupEvent = GetEventFromLevel(nLevel);
	var nKeyCode = oPopupEvent.keyCode;
	if (nKeyCode == 9) nKeyCode = !oPopupEvent.shiftKey ? 40 : 38;
	ClearShowSubMenuEvnt();
	switch (nKeyCode)
		{
	case 38:	
		MoveMenuSelection(-1);
		break;
	case 40:	
		MoveMenuSelection(1);
		break;
	case 37:	
	case 27: 
		CloseCurrentLevel(nKeyCode == 27);
		break;
	case 39:	
	case 13:	
		EngageSelection(nKeyCode == 13, false, true);
		break;
		}
	oPopupEvent.returnValue = false;
}
function EngageSelection(fDoSelection, fDelayExpandSM, fEnterSM)
{
	var oItem = _arrSelected[_nLevel];
	if (!oItem || oItem.optionDisabled) return;
	if (FIsIType(oItem, "submenu"))
		{
		if (fDelayExpandSM)
			{
			SetShowSubMenuEvnt();
			}
		else
			{
			ShowSubMenu(_nLevel, oItem);
			if (fEnterSM) MoveMenuSelection(1);
			}
		}
	else if (fDoSelection)
		{
		var fnOnClick = oItem.getAttribute("onMenuClick");
		if (fnOnClick)
			{
			if (FIStringEquals(typeof(fnOnClick), "string"))
				fnOnClick = new Function("var document=window.document; {" + fnOnClick + "}");
			var oTemp = window.document.body.appendChild(window.document.createElement("span"));
			oTemp.onclick = fnOnClick;
			oTemp.click();
			oTemp.removeNode();
			HideMenu();
			}
		}
}
function GetIType(oItem)
{
	return oItem ? oItem.getAttribute("type") : null;
}
function FIsIType(oItem, wzType)
{
	return FIStringEquals(GetIType(oItem), wzType);
}
function SetIType(oItem, wzType)
{
	if (oItem) oItem.setAttribute("type", wzType);
}
function FIStringEquals(wzX, wzY)
{
	return wzX != null && wzY != null && wzX.toLowerCase() == wzY.toLowerCase();
}
function FIStringContains(wzX, wzY)
{
	return wzX != null && wzY != null && wzX.toLowerCase().indexOf(wzY.toLowerCase()) >= 0;
}
function FIStringStartsWith(wzX, wzY)
{
	return wzX != null && wzY != null && wzX.toLowerCase().indexOf(wzY.toLowerCase()) == 0;
}
function FIStringEndsWith(wzX, wzY)
{
	return wzX != null && wzY != null && wzX.length >= wzY.length &&
		(wzY == "" || wzX.toLowerCase().indexOf(wzY.toLowerCase()) == wzX.length - wzY.length);
}
function CloseCurrentLevel(fAllowHideRoot)
{
	if (_nLevel > 0) UpdateLevel(_nLevel - 1);
	else if (fAllowHideRoot) HideMenu();
}
function UnselectCurrentOption()
{
	if (_nLevel >= 0)
		{
		var oItem = _arrSelected[_nLevel];
		if (FIsIType(oItem, "option"))
			{
			DropDownMenu_UnselectItem(oItem);
			_arrSelected[_nLevel] = null;
			}
		}
}
function MakeID(nLevel, nIndex)
{
	return _wzPrefixID + "_" + nLevel + "_" + nIndex;
}
function GetItem(nLevel, nIndex)
{
	var oPopup = _dropDownMenu_currentPopUp;
	return oPopup ? oPopup.document.getElementById(MakeID(nLevel, nIndex)) : null;
}
function MoveMenuSelection(iDir)
{
	var nIndex = -1;
	var nNumItems = _oRoot.children.length;
	var oSelected = _arrSelected[_nLevel];
	if (oSelected)
		{
		var wzSelectedID = oSelected ? oSelected.id : null;
		if (wzSelectedID)
			{
			var nCurIndex = parseInt(wzSelectedID.substring(wzSelectedID.lastIndexOf("_") + 1, wzSelectedID.length));
			nIndex = (nCurIndex + nNumItems + iDir) % nNumItems; 
			}
		}
	if (nIndex < 0)
		nIndex = iDir > 0 ? 0 : (nNumItems - 1);
	var oItem;
	var nIndexStart = nIndex;
	do
		{
		oItem = GetItem(_nLevel, nIndex);
		nIndex = (nIndex + nNumItems + iDir) % nNumItems; 
		}
	while (nIndex != nIndexStart &&
			 (!oItem || oItem.style.display == "none" ||
			  !(FIsIType(oItem, "option") || FIsIType(oItem, "submenu"))));
	ToggleMenuItem(_nLevel, oItem);
}
function ToggleMenuItem(nLevel, oItem)
{
	var oOld = _arrSelected[nLevel];
	if (!oItem || (oOld && oItem.id == oOld.id)) return;
	if (oOld) DropDownMenu_UnselectItem(oOld);
	UpdateLevel(nLevel);
	DropDownMenu_SelectItem(oItem);
	_arrSelected[nLevel] = oItem;
	oItem.tabIndex = 0;
	oItem.setActive();
}
