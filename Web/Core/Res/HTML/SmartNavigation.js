function SmartScrolling_Element (id, top, left)
{
  this.ID = id;
  this.Top = top;
  this.Left = left;
  
  this.ToString = function ()
  {
    if (this.ID == null || this.ID == '')
      return '';
    else
      return this.ID + ' ' + this.Top + ' ' + this.Left;
  }
}

SmartScrolling_Element.Parse = function (value)
{
  if (value != null && value != '')
  {
    var fields = value.split (' ');
    return new SmartScrolling_Element (fields[0], fields[1], fields[2]);
  }
  else
  {
    return null;
  }
}

function SmartScrolling_Restore (data)
{
  if (data == null || data == '')
    return;
    
  var dataFields = data.split ('*');
  if (dataFields.length == 0)
    return;
  
  var sseBody = SmartScrolling_Element.Parse (dataFields.shift());
  document.body.scrollTop = sseBody.Top;
  document.body.scrollLeft = sseBody.Left;
  
  for (var i = 0; i < dataFields.length; i++)
  {
    var scrollElement = SmartScrolling_Element.Parse (dataFields[i]);
    SmartScrolling_SetScrollPosition (scrollElement);
  } 
}

function SmartScrolling_Backup (activeElement)
{
  var data = '';
  var scrollElements = new Array();
  
  if (document.body.id == null || document.body.id == '')
  {
    var sseBody = new SmartScrolling_Element ('body', document.body.scrollTop, document.body.scrollLeft);
    scrollElements.push (sseBody);
  }
  scrollElements = scrollElements.concat (SmartScrolling_GetScrollPositions (document.body));
  
  for (var i = 0; i < scrollElements.length; i++)
  {
    if (i > 0)
      data += '*'; 
    var scrollElement = scrollElements[i];
    data += scrollElement.ToString();
  }

  return data;
}

function SmartScrolling_GetScrollPositions (currentElement)
{
  var scrollElements = new Array();
  if (currentElement != null)
  {
    if (   currentElement.id != null && currentElement.id != ''
        && (currentElement.scrollTop != 0 || currentElement.scrollLeft != 0))
    {
      var sseCurrentElement = SmartScrolling_GetScrollPosition (currentElement);
      scrollElements.push (sseCurrentElement);
    }
    
    for (var i = 0; i < currentElement.children.length; i++)
    {
      var element = currentElement.children[i];
      var scrollChilden = SmartScrolling_GetScrollPositions (element);
      scrollElements = scrollElements.concat (scrollChilden);
    }
  }
  return scrollElements;  
}

function SmartScrolling_GetScrollPosition (htmlElement)
{
  if (htmlElement != null)
    return new SmartScrolling_Element (htmlElement.id, htmlElement.scrollTop, htmlElement.scrollLeft);
  else
    return null;
}

function SmartScrolling_SetScrollPosition (scrollElement)
{
  if (scrollElement == null)
    return;
  var htmlElement = document.getElementById (scrollElement.ID)
  if (htmlElement == null)
    return;
  htmlElement.scrollTop = scrollElement.Top;
  htmlElement.scrollLeft = scrollElement.Left;
}

function SmartFocus_Backup (activeElement)
{
  var data = '';  
  if (activeElement != null)
  {
    data += activeElement.id;
  }
  
  return data;
}

function SmartFocus_Restore (data)
{
  if (data == null || data == '')
    return;

  var activeElementID = data;
  if (activeElementID != null && activeElementID != '')
  {
    var activeElement = document.getElementById (activeElementID);
    if (activeElement != null)
    {
      activeElement.focus();
    }
  }
}

