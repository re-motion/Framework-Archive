var _wxe_context = null;

function Wxe_Initialize (
    theFormID, 
    refreshInterval, refreshUrl, 
    abortUrl, abortMessage, 
    smartScrollingFieldID, smartFocusFieldID,
    eventHandler)
{
  _wxe_context = new Wxe_Context (
      theFormID, 
      refreshInterval, refreshUrl, 
      abortUrl, abortMessage, 
      smartScrollingFieldID, smartFocusFieldID,
      eventHandler);
  Wxe_SetEventHandlers ();
}

function Wxe_Context (
    theFormID, 
    refreshInterval, refreshUrl, 
    abortUrl, abortMessage, 
    smartScrollingFieldID, smartFocusFieldID,
    eventHandler)
{
  this.TheForm = document.forms[theFormID];
  if (refreshInterval > 0)
  {
    this.RefreshUrl = refreshUrl;
    this.RefreshTimer = window.setInterval('Wxe_Refresh()', refreshInterval);
  }
  this.AbortUrl = abortUrl;
  this.IsAbortEnabled = abortUrl != null;
  this.AbortMessage = abortMessage;
  this.IsAbortConfirmationEnabled = this.IsAbortEnabled && abortMessage != null;
  this.IsSubmit = false;
  this.AspnetDoPostBack = null;
  if (smartScrollingFieldID != null)
    this.SmartScrollingField = this.TheForm.elements[smartScrollingFieldID];
  if (smartFocusFieldID != null)
    this.SmartFocusField = this.TheForm.elements[smartFocusFieldID];
  var _activeElement = null;
  this.AbortHandler = eventHandler
  
  this.GetActiveElement = function()
  {
    if (window.document.activeElement != null)
      return window.document.activeElement;
    else
      return _activeElement;
  }

  this.SetActiveElement = function (value)
  {
    _activeElement = value;
  }
  
  this.Backup = function ()
  {
    if (this.SmartScrollingField != null)
      this.SmartScrollingField.value = SmartScrolling_Backup (this.GetActiveElement());
    if (this.SmartFocusField != null)
      this.SmartFocusField.value = SmartFocus_Backup (this.GetActiveElement());
  }
  
  this.Restore = function ()
  {
    if (this.SmartScrollingField != null)
  	  SmartScrolling_Restore (this.SmartScrollingField.value);
    if (this.SmartFocusField != null)
  	  SmartFocus_Restore (this.SmartFocusField.value);
  }
}
  
function Wxe_SetEventHandlers ()
{
  window.onload = Wxe_OnLoad;
  window.onbeforeunload = Wxe_OnBeforeUnload; // IE, Mozilla 1.7, Firefox 0.9
  window.onunload = Wxe_OnUnload;
  
  var isMsIE = window.navigator.appName.toLowerCase().indexOf("microsoft") > -1;
  if (! isMsIE)
  	Wxe_SetFocusEventHandlers (document.body);
}

function Wxe_SetFocusEventHandlers (currentElement)
{
  if (currentElement != null)
  {
    if (currentElement.id != null && currentElement.id != '' && Wxe_IsFocusableTag (currentElement.tagName))
    {
		  currentElement.onfocus = Wxe_OnElementFocus;
		  currentElement.onblur  = Wxe_OnElementBlur;
    }
    
    for (var i = 0; i < currentElement.childNodes.length; i++)
    {
      var element = currentElement.childNodes[i];
      Wxe_SetFocusEventHandlers (element);
    }
  }
}

function Wxe_IsFocusableTag (tagName) 
{
  tagName = tagName.toLowerCase();
  return (tagName == "input" ||
          tagName == "textarea" ||
          tagName == "select" ||
          tagName == "button" ||
          tagName == "a");
}

function Wxe_Refresh()
{
  try 
  {
    var image = new Image();
    image.src = _wxe_context.RefreshUrl;
  }
  catch (e)
  {
  }
}

function Wxe_OnLoad()
{
	_wxe_context.TheForm.onsubmit = Wxe_FormSubmit;
	_wxe_context.AspnetDoPostBack = __doPostBack;
	__doPostBack = Wxe_DoPostBack;
	_wxe_context.Restore();
}

function Wxe_DoPostBack (eventTarget, eventArgument)
{
  if (!_wxe_context.IsSubmit == true)
  {
	  _wxe_context.IsSubmit = true;
    _wxe_context.Backup();
  }
	_wxe_context.AspnetDoPostBack (eventTarget, eventArgument);
}

function Wxe_FormSubmit ()
{
  if (!_wxe_context.IsSubmit == true)
  {
    _wxe_context.IsSubmit = true; 
    _wxe_context.Backup();
  }
}

function Wxe_OnBeforeUnload ()
{
  if (_wxe_context.IsAbortConfirmationEnabled && ! _wxe_context.IsSubmit)
  {
    var activeElement = _wxe_context.GetActiveElement();
    var isJavaScriptAnchor = false;
    if (  activeElement != null
        && activeElement.tagName.toLowerCase() == 'a'
        && activeElement.href != null
        && activeElement.href.toLowerCase().indexOf ('javascript:') >= 0)
    {
      isJavaScriptAnchor = true;
    }
    if (! isJavaScriptAnchor)
    {
      // IE alternate/official version: window.event.returnValue = _wxe_context.AbortMessage
      return _wxe_context.AbortMessage;
    }
  }
}

function Wxe_OnUnload()
{
  if (_wxe_context.IsAbortEnabled && ! _wxe_context.IsSubmit)
  {
    try 
    {
      Wxe_OnAbort();      
    }
    catch (e)
    {
    }
    
    try 
    {
      var image = new Image();
      image.src = _wxe_context.AbortUrl;
    }
    catch (e)
    {
    }
  }
}

function Wxe_OnElementBlur (evt) 
{
	_wxe_context.SetActiveElement (null);
}

function Wxe_OnElementFocus (evt)
{
	var e = evt ? evt : window.event;
	if (!e) 
	  return;
	if (e.target)
		_wxe_context.SetActiveElement (e.target);
	else if (e.srcElement)
	  _wxe_context.SetActiveElement (e.srcElement);
}

function Wxe_OnAbort()
{
  if (_wxe_context.AbortHandler != null)
  {
    var abortHandler = eval (_wxe_context.AbortHandler);
    abortHandler();
  }
}