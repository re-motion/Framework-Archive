var _wxe_context = null;

function Wxe_Initialize (
    theFormID, 
    refreshInterval, refreshUrl, 
    abortUrl, abortMessage, 
    isSubmittingMessage, isAbortingMessage,
    smartScrollingFieldID, smartFocusFieldID,
    eventHandlers)
{
  _wxe_context = new Wxe_Context (
      theFormID, 
      refreshInterval, refreshUrl, 
      abortUrl, abortMessage, 
      isSubmittingMessage, isAbortingMessage,
      smartScrollingFieldID, smartFocusFieldID,
      eventHandlers);
  
  Wxe_SetEventHandlers();
  Wxe_OverrideAspNetDoPostBack();
}

function Wxe_Context (
    theFormID, 
    refreshInterval, refreshUrl, 
    abortUrl, abortMessage, 
    isSubmittingMessage, isAbortingMessage,
    smartScrollingFieldID, smartFocusFieldID,
    eventHandlers)
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

  this.IsSubmitting = false;
  // Special flag to support the OnBeforeUnload part
  this.IsSubmittingBeforeUnload = false;
  this.IsSubmittingMessage = isSubmittingMessage;

  this.IsAborting = false;
  // Special flag to support the OnBeforeUnload part
  this.IsAbortingBeforeUnload = false;
  this.IsAbortingMessage = isAbortingMessage;

  this.AspnetDoPostBack = null;
  if (smartScrollingFieldID != null)
    this.SmartScrollingField = this.TheForm.elements[smartScrollingFieldID];
  if (smartFocusFieldID != null)
    this.SmartFocusField = this.TheForm.elements[smartFocusFieldID];
  var _activeElement = null;
  var _eventHandlers = eventHandlers
  this.IsMsIE = window.navigator.appName.toLowerCase().indexOf("microsoft") > -1;
 
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
  
  this.GetEventHandlers = function (eventName)
  {
    return _eventHandlers[eventName];
  }
}

function Wxe_OverrideAspNetDoPostBack ()
{
	_wxe_context.TheForm.onsubmit = Wxe_FormSubmit;
	_wxe_context.AspnetDoPostBack = __doPostBack;
	__doPostBack = Wxe_DoPostBack;
}
  
function Wxe_SetEventHandlers ()
{
  window.onload = Wxe_OnLoad;
  window.onbeforeunload = Wxe_OnBeforeUnload; // IE, Mozilla 1.7, Firefox 0.9
  window.onunload = Wxe_OnUnload;
  
  if (! _wxe_context.IsMsIE)
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
  Wxe_SendSessionRequest (_wxe_context.RefreshUrl)
}

function Wxe_OnLoad()
{
	_wxe_context.Restore();
  Wxe_ExecuteEventHandlers (_wxe_context.GetEventHandlers('onload'));
}

function Wxe_DoPostBack (eventTarget, eventArgument)
{
  if (_wxe_context.IsAborting)
  {
    Wxe_ShowIsAbortingMessage();
  }
  else if (_wxe_context.IsSubmitting)
  {
    Wxe_ShowIsSubmittingMessage();
  }
  else
  {
    _wxe_context.IsSubmitting = true;
    _wxe_context.IsSubmittingBeforeUnload = true;
    
    _wxe_context.Backup();
    Wxe_ExecuteEventHandlers (_wxe_context.GetEventHandlers('onpostback'), eventTarget, eventArgument);

	_wxe_context.AspnetDoPostBack (eventTarget, eventArgument);
  }
}

function Wxe_FormSubmit ()
{
  if (_wxe_context.IsAborting)
  {
    Wxe_ShowIsAbortingMessage();
    return false;
  }
  else if (_wxe_context.IsSubmitting)
  {
    Wxe_ShowIsSubmittingMessage();
    return false;
  }
  else
  {
    _wxe_context.IsSubmitting = true; 
    _wxe_context.IsSubmittingBeforeUnload = true;
    
    _wxe_context.Backup();
    var eventTarget = null;
    if (_wxe_context.GetActiveElement() != null)
      eventTarget = _wxe_context.GetActiveElement().id;
    Wxe_ExecuteEventHandlers (_wxe_context.GetEventHandlers('onpostback'), eventTarget, '');
    return true;
  }
}

// __doPostBack
// {
//   Form.submit()
//   {
//     OnBeforeUnload()
//   } 
// }
// Wait For Response
// OnUnload()
function Wxe_OnBeforeUnload ()
{
  _wxe_context.IsAbortingBeforeUnload = false;
  
  if (   ! _wxe_context.IsSubmittingBeforeUnload
      && ! _wxe_context.IsAborting && _wxe_context.IsAbortConfirmationEnabled)
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
	  _wxe_context.IsAbortingBeforeUnload = true;
      // IE alternate/official version: window.event.returnValue = _wxe_context.AbortMessage
      return _wxe_context.AbortMessage;
    }
  }
  else if (_wxe_context.IsSubmittingBeforeUnload)
  {
    _wxe_context.IsSubmittingBeforeUnload = false;
  }
}

function Wxe_OnUnload()
{
  if (   (! _wxe_context.IsSubmitting || _wxe_context.IsAbortingBeforeUnload)
      && ! _wxe_context.IsAborting && _wxe_context.IsAbortEnabled)
  {
    _wxe_context.IsAborting = true;
    Wxe_ExecuteEventHandlers (_wxe_context.GetEventHandlers('onabort'));
    
    Wxe_SendSessionRequest (_wxe_context.AbortUrl);
    
    _wxe_context.IsAbortingBeforeUnload = false;
  }
}

function Wxe_SendSessionRequest (url)
{
  try 
  {
    var xhttp;
    // Create XHttpRequest
    if (_wxe_context.IsMsIE) 
      xhttp = new ActiveXObject('Microsoft.XMLHTTP'); 
    else
      xhttp = new XMLHttpRequest(); 

    var method = 'GET'
    var isSynchronousCall = false;
    xhttp.open (method, url, isSynchronousCall);
    xhttp.send ();    
  }
  catch (e)
  {
    try 
    {
      var image = new Image();
      image.src = url;
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

function Wxe_ExecuteEventHandlers (eventHandlers)
{
  if (eventHandlers != null)
  {
    for (var i = 0; i < eventHandlers.length; i++)
    {
      var eventHandler = Wxe_GetFunctionPointer (eventHandlers[i]);
      if (eventHandler != null)
      {
        var arg1 = null;
        var arg2 = null;
        var args = Wxe_ExecuteEventHandlers.arguments;
        
        if (args.length > 1)
          arg1 = args[1];
        if (args.length > 2)
          arg2 = args[2];
          
        try
        {
          eventHandler (arg1, arg2);
        }
        catch (e)
        {
        }
      }
    }
  }
}

function Wxe_GetFunctionPointer (functionName)
{
  try
  {
    return eval (functionName);
  }
  catch (e)
  {
    return null;
  }
}

function Wxe_ShowIsAbortingMessage()
{
  Wxe_ShowMessage ('WxeIsAbortingMessage', _wxe_context.IsAbortingMessage);
}

function Wxe_ShowIsSubmittingMessage()
{
  Wxe_ShowMessage ('WxeIsSubmittingMessage', _wxe_context.IsSubmittingMessage);
}

function Wxe_ShowMessage (id, message)
{
  if (document.getElementById (id) == null)
  {
    var div = document.createElement('DIV');
    div.id = id;
    div.style.width = '50%';
    div.style.height = '50%';
    div.style.position = 'absolute';
    div.style.left = '25%';
    div.style.top = '25%';
    div.innerHTML = 
          '<table style="border:none; height:100%; width:100%"><tr><td style="text-align:center;">'
        + message
        + '</td></tr></table>';
	
    document.body.appendChild(div);
  }
}