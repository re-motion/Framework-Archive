// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
var _dropDownMenu_menuInfos = new Object();

var _dropDownMenu_itemClassName = 'DropDownMenuItem';
var _dropDownMenu_itemDisabledClassName = 'DropDownMenuItemDisabled';
var _dropDownMenu_itemIconClassName = 'DropDownMenuItemIcon';
var _dropDownMenu_itemSeparatorClassName = 'DropDownMenuSeparator';
var _dropDownMenu_currentMenu = null;

var _dropDownMenu_styleSheetLink = null;
var _dropDownMenu_menuItemIDPrefix = 'menuItem_';

var _dropDownMenu_requiredSelectionAny = 0;
var _dropDownMenu_requiredSelectionExactlyOne = 1;
var _dropDownMenu_requiredSelectionOneOrMore = 2;

var _dropDownMenu_currentItemIndex = -1;

var _itemClickHandler = null;
var _itemClicked = false;

function DropDownMenu_InitializeGlobals(styleSheetLink) {
    _dropDownMenu_styleSheetLink = styleSheetLink;
}

function DropDownMenu_MenuInfo(id, itemInfos) {
    this.ID = id;
    this.ItemInfos = itemInfos;
}

function DropDownMenu_AddMenuInfo(menuInfo) {
    _dropDownMenu_menuInfos[menuInfo.ID] = menuInfo;
}

function DropDownMenu_ItemInfo(id, category, text, icon, iconDisabled, requiredSelection, isDisabled, href, target) {
    this.ID = id;
    this.Category = category;
    this.Text = text;
    this.Icon = icon;
    this.IconDisabled = iconDisabled;
    this.RequiredSelection = requiredSelection;
    this.IsDisabled = isDisabled;
    this.Href = href;
    this.Target = target;
}

function DropDownMenu_OnClick(context, menuID, getSelectionCount, evt) {
    if (_itemClicked) {
        _itemClicked = false;
        return;
    }
    if (context != _dropDownMenu_currentMenu) {
        DropDownMenu_ClosePopUp();
    }
    if (_dropDownMenu_currentMenu == null) {
        DropDownMenu_OpenPopUp(menuID, context, getSelectionCount, evt);
        _dropDownMenu_currentMenu = context;
    }
}

function DropDownMenu_GetZIndexParent(element) {
    var parent = element;
    while (parent.parent('div').length > 0)
        parent = parent.parent();
    return parent;
}

function DropDownMenu_OpenPopUp(menuID, context, getSelectionCount, evt) {
    var itemInfos = _dropDownMenu_menuInfos[menuID].ItemInfos;
    var selectionCount = -1;
    if (getSelectionCount != null)
        selectionCount = getSelectionCount();

    if (itemInfos.length == 0)
        return;

    var ul = document.createElement('ul');
    ul.className = 'DropDownMenuOptions';
    context.appendChild(ul);

    $(context).css('zIndex', '1000');
    DropDownMenu_GetZIndexParent($(context)).css('zIndex', '1000');

    _itemClickHandler = function() {
        DropDownMenu_ClosePopUp();
        _itemClicked = true;
        try {
            eval(this.getAttribute('javascript'));
        }
        catch (e) {
        }
        setTimeout('_itemClicked = false;', 10);
    };
    setTimeout("$('body').bind('click', DropDownMenu_ClosePopUp);", 10);

    for (var i = 0; i < itemInfos.length; i++) {
        var item = DropDownMenu_CreateItem(itemInfos[i], selectionCount, true);
        if (item != null)
            ul.appendChild(item);
    }

    // move dropdown if there is not enough space to fit it on the page
    var titleDiv = $(context).children(':first');
    var space_top = Math.round(titleDiv.offset().top - $(document).scrollTop());
    var space_bottom = Math.round($(window).height() - titleDiv.offset().top - titleDiv.height() + $(document).scrollTop());
    var space_left = titleDiv.offset().left;
    var space_right = $(window).width() - titleDiv.offset().left - titleDiv.width();

    if (($(ul).width() > space_left) && (space_left < space_right)) {

        if ($(ul).offset().left < 0) {
            $(ul).css('left', '0');
        } else {
            $(ul).css('right', '0');
        }
    }
    if (($(ul).height() > space_bottom) && (space_top > space_bottom)) {
        $(ul).css({ top: -$(ul).height() });
    }
    else {
        $(ul).css('top', 'auto');
    }

    var iframe = $(context).children('iframe');
    iframe.css('top', $(ul).position().top + 'px');
    iframe.css('left', $(ul).position().left + 'px');
    iframe.width($(ul).width());
    iframe.height($(ul).height());
    iframe.css('position', 'absolute');
    iframe.css('zIndex', '999');
    iframe.show();
}

function DropDownMenu_ClosePopUp() {
    if (_dropDownMenu_currentMenu == null)
        return;

    var ul = $(_dropDownMenu_currentMenu).children('ul');

    $('body').unbind('click', DropDownMenu_ClosePopUp);
    DropDownMenu_GetZIndexParent($(_dropDownMenu_currentMenu)).css('zIndex', '0');
    _dropDownMenu_currentMenu = null;

    var iframe = ul.siblings('iframe');
    iframe.hide();

    ul.remove();

    _dropDownMenu_currentItemIndex = -1;
}

function DropDownMenu_CreateItem(itemInfo, selectionCount) {
    if (itemInfo == null)
        return null;

    var item;
    if (itemInfo.Text == '-')
        item = DropDownMenu_CreateSeparatorItem();
    else
        item = DropDownMenu_CreateTextItem(itemInfo, selectionCount);

    return item;
}

function DropDownMenu_CreateTextItem(itemInfo, selectionCount) {
    var isEnabled = true;

    if (itemInfo.IsDisabled) {
        isEnabled = false;
    }
    else {
        if (itemInfo.RequiredSelection == _dropDownMenu_requiredSelectionExactlyOne
        && selectionCount != 1) {
            isEnabled = false;
        }
        if (itemInfo.RequiredSelection == _dropDownMenu_requiredSelectionOneOrMore
        && selectionCount < 1) {
            isEnabled = false;
        }
    }

    var item = document.createElement("li");
    item.id = itemInfo.ID;

    var className = _dropDownMenu_itemClassName
    if (!isEnabled)
        className = _dropDownMenu_itemDisabledClassName;

    item.setAttribute('class', className);
    $(item).bind('click', _itemClickHandler);

    var anchor = document.createElement("a");
    anchor.setAttribute('href', '#');
    item.appendChild(anchor);
    if (isEnabled && itemInfo.Href != null) {

        var isJavaScript = itemInfo.Href.toLowerCase().indexOf('javascript:') >= 0;
        if (isJavaScript) {
            item.setAttribute('javascript', itemInfo.Href);
        }
        else {
            var href = itemInfo.Href;
            var target;
            if (itemInfo.Target != null && itemInfo.Target.length > 0)
                target = itemInfo.Target;
            else
                target = '_self';
            item.setAttribute('javascript', 'window.open (\'' + href + '\', \'' + target + '\');');
        }
    }

    if (itemInfo.Icon != null) {
        var icon = document.createElement('img');
        if (isEnabled || itemInfo.IconDisabled == null)
            icon.src = itemInfo.Icon;
        else
            icon.src = itemInfo.IconDisabled;

        icon.setAttribute('class', _dropDownMenu_itemIconClassName);
        icon.style.verticalAlign = 'middle';
        anchor.appendChild(icon);
    }
    else {
        var iconPlaceholder = document.createElement('span');
        iconPlaceholder.setAttribute('class', _dropDownMenu_itemIconClassName);
        anchor.appendChild(iconPlaceholder);
    }

    var text = ''
    if (itemInfo.Text != null)
        text = itemInfo.Text;

    var textNode = document.createTextNode(text);
    anchor.appendChild(textNode);

    return item;
}

function DropDownMenu_CreateSeparatorItem() {
    var item = document.createElement('li');

    var textPane = document.createElement('div');
    textPane.setAttribute('class', _dropDownMenu_itemSeparatorClassName);
    textPane.className = _dropDownMenu_itemSeparatorClassName;
    textPane.innerHTML = '&nbsp;';

    item.appendChild(textPane);

    return textPane;
}

function DropDownMenu_OnKeyDown(event, dropDownMenu, getSelectionCount) {
    // alert(event.keyCode + ', ' + dropDownMenu.nodeName + '#' + dropDownMenu.id);
    
    var itemInfos = _dropDownMenu_menuInfos[dropDownMenu.id].ItemInfos;

    switch (event.keyCode) {
        case 13: //enter
        case 32: //space
            if (_dropDownMenu_currentItemIndex >= 0) {
                var itemAnchor = $($(dropDownMenu).find('ul').children()[_dropDownMenu_currentItemIndex]).children('a');
                itemAnchor.click();
            }

            if (dropDownMenu != _dropDownMenu_currentMenu)
                DropDownMenu_OnClick(dropDownMenu, dropDownMenu.id, getSelectionCount, null);
            else
                DropDownMenu_ClosePopUp();
            break;
        case 27: //escape
            DropDownMenu_ClosePopUp();
            break;
        case 39: // right arrow
        case 40: // down arrow
            if (_dropDownMenu_currentItemIndex < itemInfos.length - 1) {
                _dropDownMenu_currentItemIndex++;
                if( itemInfos[_dropDownMenu_currentItemIndex].ID == -1 )
                    _dropDownMenu_currentItemIndex++;
            }
            else
                _dropDownMenu_currentItemIndex = 0;
            break;
        case 37: // left arrow
        case 38: // up arrow
            if (_dropDownMenu_currentItemIndex > 0) {
                _dropDownMenu_currentItemIndex--;
                if( itemInfos[_dropDownMenu_currentItemIndex].ID == -1 )
                    _dropDownMenu_currentItemIndex--;
            }
            else
                _dropDownMenu_currentItemIndex = itemInfos.length - 1;
            break;
    }
    if (0 <= _dropDownMenu_currentItemIndex && _dropDownMenu_currentItemIndex < itemInfos.length) {
        ul = $(dropDownMenu).find('ul');
        ul.children('li').removeAttr('class');
        if (ul.length > 0) {
            var li = ul.children()[_dropDownMenu_currentItemIndex];
            $(li).attr('class', 'selected');
        }
    }
}