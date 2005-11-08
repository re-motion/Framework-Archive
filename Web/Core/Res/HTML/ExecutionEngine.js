var _wxe_context = null;

function Wxe_Initialize (
    theFormID, 
    refreshInterval, refreshUrl, 
    abortUrl, abortMessage, 
    statusIsSubmittingMessage, statusIsAbortingMessage, statusIsCachedMessage,
    smartScrollingFieldID, smartFocusFieldID,
    eventHandlers)
{
  _wxe_context = new Wxe_Context (
      theFormID, 
      refreshInterval, refreshUrl, 
      abortUrl, abortMessage, 
      statusIsSubmittingMessage, statusIsAbortingMessage, statusIsCachedMessage,
      smartScrollingFieldID, smartFocusFieldID,
      eventHandlers);
  
  _wxe_context.Init();
}

function Wxe_Context (
    theFormID, 
    refreshInterval, refreshUrl, 
    abortUrl, abortMessage, 
    statusIsSubmittingMessage, statusIsAbortingMessage, statusIsCachedMessage,
    smartScrollingFieldID, smartFocusFieldID,
    eventHandlers)
{
  this.TheForm = document.forms[theFormID];
  
  var _refreshUrl = null;
  var _refreshTimer = null;
  if (refreshInterval > 0)
  {
    _refreshUrl = refreshUrl;
    _refreshTimer = window.setInterval('Wxe_Refresh()', refreshInterval);
  }
  
  var _abortUrl = abortUrl;
  var _isAbortEnabled = abortUrl != null;
  var _abortMessage = abortMessage;
  var _isAbortConfirmationEnabled = _isAbortEnabled && abortMessage != null;

  var _isSubmitting = false;
  var _hasSubmitted = false;
  // Special flag to support the OnBeforeUnload part
  var _isSubmittingBeforeUnload = false;
  var _statusIsSubmittingMessage = statusIsSubmittingMessage;

  var _isAborting = false;
  var _hasAborted = false;
  // Special flag to support the OnBeforeUnload part
  var _isAbortingBeforeUnload = false;
  var _statusIsAbortingMessage = statusIsAbortingMessage;

  var _statusIsCachedMessage = statusIsCachedMessage;

  var _aspnetDoPostBack = null;
  
  var _smartScrollingField = null;
  if (smartScrollingFieldID != null)
    _smartScrollingField = this.TheForm.elements[smartScrollingFieldID];
  var _smartFocusField = null;
  if (smartFocusFieldID != null)
    _smartFocusField = this.TheForm.elements[smartFocusFieldID];
  
  var _activeElement = null;
  var _eventHandlers = eventHandlers
  var _isMsIE = window.navigator.appName.toLowerCase().indexOf("microsoft") > -1;
  var _cacheStateHasSubmitted = 'hasSubmitted';
  var _cacheStateHasLoaded = 'hasLoaded';
 
  this.Init = function()
  {
    this.SetEventHandlers ();
    this.OverrideAspNetDoPostBack ();
  }

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
    if (_smartScrollingField != null)
      _smartScrollingField.value = SmartScrolling_Backup (this.GetActiveElement());
    if (_smartFocusField != null)
      _smartFocusField.value = SmartFocus_Backup (this.GetActiveElement());
  }
  
  this.Restore = function ()
  {
    if (_smartScrollingField != null)
  	  SmartScrolling_Restore (_smartScrollingField.value);
    if (_smartFocusField != null)
  	  SmartFocus_Restore (_smartFocusField.value);
  }
  
  this.CheckIfCached = function ()
  {
    var field = this.TheForm.wxeCacheDetectionField;
    if (field.value == _cacheStateHasSubmitted)
    {
      _hasSubmitted = true;
      this.ShowStatusIsCachedMessage ();
    }
    else if (field.value == _cacheStateHasLoaded && _isAbortEnabled)
    {
      _hasAborted = true;
      this.ShowStatusIsCachedMessage ();
    }
    else
    {
      this.SetCacheDetectionFieldLoaded();
    }
  }
  
  this.SetCacheDetectionFieldLoaded = function ()
  {
    var field = this.TheForm.wxeCacheDetectionField;
    field.value = _cacheStateHasLoaded;   
  }

  this.SetCacheDetectionFieldSubmitted = function ()
  {
    var field = this.TheForm.wxeCacheDetectionField;
    field.value = _cacheStateHasSubmitted;   
  }

  this.SetCacheDetectionFieldAborted = function ()
  {
    // Do nothing, abort cannot be remembered via hidden field
  }

  this.SetFocusEventHandlers = function (currentElement)
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
        this.SetFocusEventHandlers (element);
      }
    }
  }
  
  this.SetEventHandlers = function ()
  {
    window.onload = Wxe_OnLoad;
    window.onbeforeunload = Wxe_OnBeforeUnload; // IE, Mozilla 1.7, Firefox 0.9
    window.onunload = Wxe_OnUnload;
    
    if (! _isMsIE)
  	  this.SetFocusEventHandlers (document.body);
  }

  this.OnLoad = function ()
  {
    this.CheckIfCached();
	  this.Restore();
    this.ExecuteEventHandlers (_eventHandlers['onload']);
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
  this.OnBeforeUnload = function ()
  {
    _isAbortingBeforeUnload = false;
    
    if (   ! _isSubmittingBeforeUnload
        && ! _hasAborted && ! _hasSubmitted
        && ! _isAborting && _isAbortConfirmationEnabled)
    {
      var activeElement = this.GetActiveElement();
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
	      _isAbortingBeforeUnload = true;
        // IE alternate/official version: window.event.returnValue = _wxe_context.AbortMessage
        return _abortMessage;
      }
    }
    else if (_isSubmittingBeforeUnload)
    {
      _isSubmittingBeforeUnload = false;
    }
  }

  this.OnUnload = function ()
  {
    if (   (! _isSubmitting || _isAbortingBeforeUnload)
        && (! _hasAborted || _hasSubmitted)
        && ! _isAborting && _isAbortEnabled)
    {
      _isAborting = true;
      this.ExecuteEventHandlers (_eventHandlers['onabort']);
      
      this.SetCacheDetectionFieldAborted();

      this.SendSessionRequest (_abortUrl);
      
      _isAbortingBeforeUnload = false;
    }
  }

  this.Refresh = function ()
  {
    this.SendSessionRequest (_refreshUrl)
  }

  this.OverrideAspNetDoPostBack = function ()
  {
	  this.TheForm.onsubmit = Wxe_FormSubmit;
	  _aspnetDoPostBack = __doPostBack;
	  __doPostBack = Wxe_DoPostBack;
  }

  this.DoPostBack = function (eventTarget, eventArgument)
  {
    if (_hasSubmitted || _hasAborted)
    {
      return;
    }
    else if (_isAborting)
    {
      this.ShowStatusIsAbortingMessage();
    }
    else if (_isSubmitting)
    {
      this.ShowStatusIsSubmittingMessage();
    }
    else
    {
      _isSubmitting = true;
      _isSubmittingBeforeUnload = true;
      
      this.Backup();
      this.ExecuteEventHandlers (_eventHandlers['onpostback'], eventTarget, eventArgument);

      this.SetCacheDetectionFieldSubmitted();
          
	    _aspnetDoPostBack (eventTarget, eventArgument);
    }
  }

  this.FormSubmit = function ()
  {
    if (_hasSubmitted || _hasAborted)
    {
      return false;
    }
    if (_isAborting)
    {
      this.ShowStatusIsAbortingMessage();
      return false;
    }
    else if (_isSubmitting)
    {
      this.ShowStatusIsSubmittingMessage();
      return false;
    }
    else
    {
      _isSubmitting = true; 
      _isSubmittingBeforeUnload = true;
      
      this.Backup();
      var eventTarget = null;
      if (this.GetActiveElement() != null)
        eventTarget = this.GetActiveElement().id;
      this.ExecuteEventHandlers (_eventHandlers['onpostback'], eventTarget, '');
      
      this.SetCacheDetectionFieldSubmitted();
      
      return true;
    }
  }

  this.SendSessionRequest = function (url)
  {
    try 
    {
      var xhttp;
      // Create XHttpRequest
      if (_isMsIE) 
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

  this.ExecuteEventHandlers = function (eventHandlers)
  {
    if (eventHandlers != null)
    {
      for (var i = 0; i < eventHandlers.length; i++)
      {
        var eventHandler = this.GetFunctionPointer (eventHandlers[i]);
        if (eventHandler != null)
        {
          var arg1 = null;
          var arg2 = null;
          var args = this.ExecuteEventHandlers.arguments;
          
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

  this.GetFunctionPointer = function (functionName)
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

  this.ShowStatusIsAbortingMessage = function ()
  {
    if (_statusIsAbortingMessage != null)
      this.ShowMessage ('WxeStatusIsAbortingMessage', _statusIsAbortingMessage);
  }

  this.ShowStatusIsSubmittingMessage = function ()
  {
    if (_statusIsSubmittingMessage != null)
      this.ShowMessage ('WxeStatusIsSubmittingMessage', _statusIsSubmittingMessage);
  }

  this.ShowStatusIsCachedMessage = function ()
  {
    if (_statusIsCachedMessage != null)
      this.ShowMessage ('WxeStatusIsCachedMessage', _statusIsCachedMessage);
  }

  this.ShowMessage = function (id, message)
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
}

function Wxe_FormSubmit()
{
  return _wxe_context.FormSubmit();
}

function Wxe_DoPostBack (eventTarget, eventArgument)
{
  _wxe_context.DoPostBack (eventTarget, eventArgument);
}

function Wxe_OnLoad()
{
  _wxe_context.OnLoad();
}

function Wxe_OnBeforeUnload()
{
  return _wxe_context.OnBeforeUnload();
}

function Wxe_OnUnload()
{
  _wxe_context.OnUnload();
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
