var _dropDownMenu_menuInfos = new Array();

var _dropDownMenu_popUpClassName = 'dropDownMenuPopUp';
var _dropDownMenu_itemClassName = 'dropDownMenuItem';
var _dropDownMenu_itemHoverClassName = 'dropDownMenuItemHover';

var _dropDownMenu_currentMenu = null;
var _dropDownMenu_currentPopUp = null;
var _dropDownMenu_currentPopUpWidth = null;

function DropDownMenu_MenuInfo (id, itemInfos)
{
  this.ID = id;
  this.ItemInfos = itemInfos;
}

function DropDownMenu_AddMenuInfo (menuInfo)
{
  _dropDownMenu_menuInfos[menuInfo.ID] = menuInfo;
}

function DropDownMenu_ItemInfo (id, category, text, icon)
{
  this.ID = id;
  this.Category = category;
  this.Text = text;
  this.Icon = icon;
}

function DropDownMenu_OnClick (context, menuID)
{
  var id = context.id + '_PopUp';
  DropDownMenu_ClosePopUp (_dropDownMenu_currentPopUp);
  DropDownMenu_OpenPopUp (id, menuID, context)
}

//  <div style="position: absolute; right: 0px" class="_dropDownMenu_popUpClassName">
//    CreateItem
//  </div>
function DropDownMenu_OpenPopUp (id, menuID, context)
{
	var popUp = document.createElement("div");
	if(popUp == null)
	  return null;
	if(id != null)
	  popUp.id = id;
	popUp.style.position = 'absolute';
	popUp.className = _dropDownMenu_popUpClassName;
  var itemInfos = _dropDownMenu_menuInfos[menuID].ItemInfos;
	for (var index in itemInfos)
	{
	  var item = DropDownMenu_CreateItem (itemInfos[index]);
	  if(item != null)
  	  DropDownMenu_AppendChild (popUp, item);
	}

	DropDownMenu_AppendChild (document.body, popUp);
	_dropDownMenu_currentPopUpWidth = popUp.offsetWidth;
	_dropDownMenu_currentPopUp = popUp;
	_dropDownMenu_currentMenu = context;
	//  Brower Switch
	//  Css2.1
  //	DropDownMenu_RepositionPopUp ();
  //	window.onresize = DropDownMenu_RepositionPopUp;
  //  IE
	popUp.style.behavior = "url(res/Rubicon.Web/HTML/DropDownMenu.htc)";
  popUp.onreadystatechange=OMenuEvnt;
  //  IE501

	return popUp;
}
function OMenuEvnt()
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

function DropDownMenu_ClosePopUp (popUp)
{
  if (popUp != null)
    return;//
}
//  <div class="_dropDownMenu_itemClassName">
//    item contents
//  </div>
function DropDownMenu_CreateItem (itemInfo)
{
  if (itemInfo == null)
    return null;
	var item = document.createElement("div");
	if(item == null)
	  return null;
	  
	//  item.setAttribute("type", "option");
	item.setAttribute ('id', itemInfo.ID);
	item.innerHTML = itemInfo.Text;
	item.className = _dropDownMenu_itemClassName;
	//item.setAttribute("onClick", wzAct);
	//AImg(mo,wzISrc,wzIAlt);
	return item;
}

function AImg(mi,wzISrc,wzIAlt)
{
	if(!mi)return;
	if(FNEmpWz(wzISrc))mi.setAttribute("iconSrc",wzISrc);
	if(FNEmpWz(wzIAlt))
        mi.setAttribute("iconAltText",wzIAlt);
    else
        mi.setAttribute("iconAltText","");
}

function DropDownMenu_AppendChild (parent, child)
{
	if(parent != null && child != null)
	  parent.appendChild (child);
}
