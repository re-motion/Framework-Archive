function SN_Element (id, top, left)
{
  this.ID = id;
  this.Top = top;
  this.Left = left;
  
  this.ToString = SN_Element_ToString;  
}

function SN_Element_ToString()
{
  if (this.ID == null || this.ID == '')
    return '';
  else
    return this.ID + ' ' + this.Top + ' ' + this.Left;
}

function SN_Element_Static_Parse (value)
{
  if (value != null && value != '')
  {
    var fields = value.split (' ');
    return new SN_Element (fields[0], fields[1], fields[2]);
  }
  else
  {
    return null;
  }
}

function SmartNavigationRestore (data)
{
  if (data == null || data == '')
    return;
    
  var dataFields = data.split ('*');
  
  var sneScrollParent = null;
  if (dataFields.length > 1)
  {
    if (dataFields[0] != null && dataFields[0] != '')
    {
      sneScrollParent = SN_Element_Static_Parse (dataFields[0]);
    }
  }
  
  var focusElementID = null;
  var focusElementTop = 0;
  var focusElementLeft = 0;
  if (dataFields.length > 0)
  {
    if (dataFields[dataFields.length - 1] != null && dataFields[dataFields.length - 1] != '')
    {
      var focusElementFields = dataFields[dataFields.length - 1].split (' ');
      focusElementID = focusElementFields[0];
      focusElementTop = focusElementFields[1];
      focusElementLeft = focusElementFields[2];
    }
  }
  
  if (sneScrollParent != null)
  {
    var scrollParent = document.getElementById (sneScrollParent.ID);
    if (scrollParent != null)
    {
      scrollParent.scrollTop = sneScrollParent.Top;
      scrollParent.scrollLeft = sneScrollParent.Left;
    }
  }
  
  if (focusElementID != null && focusElementID != '')
  {
    var focusElement = document.getElementById (focusElementID);
    if (focusElement != null)
    {
//      focusElement.offsetTop = focusElementTop;
//      focusElement.offsetLeft = focusElementLeft;
      focusElement.focus();
    }
  }
}

function SmartNavigationBackup ()
{
  var activeElement = window.document.activeElement;
  
  var scrollParent = null;
  for (var currentNode = activeElement; currentNode != null; currentNode = currentNode.offsetParent)
  {
    if (   currentNode.style.overflow.toLowerCase() == 'auto' 
        || currentNode.style.overflow.toLowerCase() == 'scroll')
    {
      scrollParent = currentNode;
      break;
    }
  }
  
  var data = '';
  if (scrollParent != null)
  {
    var sneScrollParent = new SN_Element (scrollParent.id, scrollParent.scrollTop, scrollParent.scrollLeft);
    data += sneScrollParent.ToString();
  }
  data += '*'; 
  
  if (activeElement != null)
  {
    var sneActiveElement = new SN_Element (activeElement.id, activeElement.offsetTop, activeElement.offsetLeft);
    data += sneActiveElement.ToString();
  }

  return data;
}
