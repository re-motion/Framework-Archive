function SmartNavigation_Element (id, top, left)
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

SmartNavigation_Element.Parse = function (value)
{
  if (value != null && value != '')
  {
    var fields = value.split (' ');
    return new SmartNavigation_Element (fields[0], fields[1], fields[2]);
  }
  else
  {
    return null;
  }
}

function SmartNavigation_Restore (data)
{
  if (data == null || data == '')
    return;
    
  var dataFields = data.split ('*');
  if (dataFields.length < 2)
    return;
  
  var snBody = SmartNavigation_Element.Parse (dataFields.shift());
  document.body.scrollTop = snBody.Top;
  document.body.scrollLeft = snBody.Left;
  
  for (var i = 0; i < dataFields.length - 1; i++)
  {
    var snElement = SmartNavigation_Element.Parse (dataFields[i]);
    SmartNavigation_SetScrollPosition (snElement);
  }
  var snScrollElement = null;
  
  var snFocusElement = SmartNavigation_Element.Parse (dataFields.pop());
  if (snFocusElement != null)
  {
    var focusElement = document.getElementById (snFocusElement.ID);
    if (focusElement != null)
    {
//      focusElement.offsetTop = focusElementTop;
//      focusElement.offsetLeft = focusElementLeft;
      focusElement.focus();
    }
  }
}

function SmartNavigation_Backup (activeElement)
{
  var data = '';
  var snScrollElements = new Array();
  
  if (document.body.id == null || document.body.id == '')
  {
    var snBody = new SmartNavigation_Element ('body', document.body.scrollTop, document.body.scrollLeft);
    snScrollElements.push (snBody);
  }
  snScrollElements = snScrollElements.concat (SmartNavigation_GetScrollPositions (document.body));
  
  for (var i = 0; i < snScrollElements.length; i++)
  {
    var snScrollElement = snScrollElements[i];
    data += snScrollElement.ToString();
    data += '*'; 
  }
  
  if (activeElement != null)
  {
    var sneActiveElement = new SmartNavigation_Element (activeElement.id, 0, 0);
    data += sneActiveElement.ToString();
  }

  return data;
}

function SmartNavigation_GetScrollPositions (currentElement)
{
  var snElements = new Array();
  if (currentElement != null)
  {
    if (   currentElement.id != null && currentElement.id != ''
        && (currentElement.scrollTop != 0 || currentElement.scrollLeft != 0))
    {
      var snCurrentElement = SmartNavigation_GetScrollPosition (currentElement);
      snElements.push (snCurrentElement);
    }
    
    for (var i = 0; i < currentElement.children.length; i++)
    {
      var element = currentElement.children[i];
      var snChildElements = SmartNavigation_GetScrollPositions (element);
      snElements = snElements.concat (snChildElements);
    }
  }
  return snElements;  
}

function SmartNavigation_GetScrollPosition (htmlElement)
{
  if (htmlElement != null)
    return new SmartNavigation_Element (htmlElement.id, htmlElement.scrollTop, htmlElement.scrollLeft);
  else
    return null;
}

function SmartNavigation_SetScrollPosition (snElement)
{
  if (snElement == null)
    return;
  var htmlElement = document.getElementById (snElement.ID)
  if (htmlElement == null)
    return;
  htmlElement.scrollTop = snElement.Top;
  htmlElement.scrollLeft = snElement.Left;
}