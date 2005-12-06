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
    _refreshTimer = window.setInterval(Wxe_Refresh, refreshInterval);
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
  var _statusMessageWindow = null;
  var _hasUnloaded = false;
  var _isMsIEAspnetPostBack = false;
  var _isMsIEFormClicked = false;

  var _aspnetFormOnSubmit = null;
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
      if (currentElement.id != null && currentElement.id != '' && IsFocusableTag (currentElement.tagName))
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
    window.onscroll = Wxe_OnScroll;
    window.onresize = Wxe_OnResize;
  }

  this.OnStartUp = function ()
  {
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
    var displayAbortMessage = false;
    
    if (   ! _hasUnloaded
        && ! _isSubmittingBeforeUnload
        && ! _hasAborted && ! _hasSubmitted
        && ! _isAborting && _isAbortConfirmationEnabled)
    {
      var activeElement = this.GetActiveElement();
      if (! IsJavaScriptAnchor (activeElement))
      {
	      _isAbortingBeforeUnload = true;
        displayAbortMessage = true;
      }
    }
    else if (_isSubmittingBeforeUnload)
    {
      _isSubmittingBeforeUnload = false;
    }
    
    this.ExecuteEventHandlers (_eventHandlers['onbeforeunload']);
    if (displayAbortMessage)
    {
      // IE alternate/official version: window.event.returnValue = _wxe_context.AbortMessage
      return _abortMessage;
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
    _hasUnloaded = true;
    this.ExecuteEventHandlers (_eventHandlers['onunload']);
  }

  this.Refresh = function ()
  {
    this.SendSessionRequest (_refreshUrl + '&Wxe_Garbage=' + Math.random())
  }

  this.OverrideAspNetDoPostBack = function ()
  {
    _aspnetFormOnSubmit = this.TheForm.onsubmit;
	  this.TheForm.onsubmit = Wxe_OnFormSubmit;
    this.TheForm.onclick = Wxe_OnFormClick;
	  _aspnetDoPostBack = __doPostBack;
	  __doPostBack = Wxe_DoPostBack;
  }

  this.DoPostBack = function (eventTarget, eventArgument)
  {
    var continueRequest = this.CheckFormState();
    if (continueRequest)
    {
      _isSubmitting = true;
      _isSubmittingBeforeUnload = true;
      
      this.Backup();
      this.ExecuteEventHandlers (_eventHandlers['onpostback'], eventTarget, eventArgument);

      this.SetCacheDetectionFieldSubmitted();
       
	    _aspnetDoPostBack (eventTarget, eventArgument);
      if (_isMsIE)
	    {
	      if (! _isMsIEFormClicked)
  	      _isMsIEAspnetPostBack = true;
        _isMsIEFormClicked = false;
	    }
    }
  }

  this.OnFormClick = function (evt)
  {
    if (_isMsIE)
    {
      if (_isMsIEAspnetPostBack)
	    {
        _isMsIEFormClicked = false;
	      _isMsIEAspnetPostBack = false;
        return null;
      }
      else
      {
        _isMsIEFormClicked = true;
      }
    }
      
    var eventSource = Wxe_GetEventSource (evt);
    this.SetActiveElement (eventSource);
    if (IsJavaScriptAnchor (eventSource))
    {
      var continueRequest = this.CheckFormState();
      if (! continueRequest)
        return false;
      else
        return null;
    }
    else
    {
      return null;
    }
  }

  this.OnFormSubmit = function ()
  {
    var continueRequest = this.CheckFormState();
    if (continueRequest)
    {
      _isSubmitting = true; 
      _isSubmittingBeforeUnload = true;
      
      this.Backup();
      var eventTarget = null;
      if (this.GetActiveElement() != null)
        eventTarget = this.GetActiveElement().id;
      this.ExecuteEventHandlers (_eventHandlers['onpostback'], eventTarget, '');
      
      this.SetCacheDetectionFieldSubmitted();
      
      if (_aspnetFormOnSubmit != null)
        return _aspnetFormOnSubmit();
      else
        return true;
    }
    else
    {
      return false;
    }
  }
  
  // returns: true to continue with request
  this.CheckFormState = function()
  {
    if (_hasUnloaded)
    {
      this.ShowIsCachedMessage();
      return false;
    }
    else if (_hasSubmitted || _hasAborted)
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
      return true;
    }
  }
  
  this.OnScroll = function()
  {
    if (_statusMessageWindow != null)
      AlignStatusMessage (_statusMessageWindow);      
    this.ExecuteEventHandlers (_eventHandlers['onscroll']);
  }

  this.OnResize = function()
  {
    if (_statusMessageWindow != null)
      AlignStatusMessage (_statusMessageWindow);      
    this.ExecuteEventHandlers (_eventHandlers['onresize']);
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
    if (_statusMessageWindow == null)
    {  
      var statusMessageWindow;
      var statusMessageBlock
      if (_isMsIE)
      {
        statusMessageWindow = document.createElement ('DIV');
        
        var iframe = document.createElement ('IFRAME');
        iframe.src = 'javascript:false;';
        iframe.style.width = '100%';
        iframe.style.height = '100%';
        iframe.style.left = '0';
        iframe.style.top = '0';
        iframe.style.position = 'absolute';
        iframe.style.filter = 'progid:DXImageTransform.Microsoft.Alpha(style=0,opacity=0)';
        iframe.style.border = 'none';
        statusMessageWindow.appendChild (iframe);
        
        statusMessageBlock = document.createElement ('DIV');
        statusMessageBlock.style.width = '100%';
        statusMessageBlock.style.height = '100%';
        statusMessageBlock.style.left = '0';
        statusMessageBlock.style.top = '0';
        statusMessageBlock.style.position = 'absolute';
        statusMessageWindow.appendChild (statusMessageBlock);      
      }
      else
      {
        statusMessageWindow = document.createElement ('DIV');
        statusMessageBlock = statusMessageWindow;
      }
      
      statusMessageWindow.id = id;
      statusMessageWindow.style.width = '50%';
      statusMessageWindow.style.height = '50%';
      statusMessageWindow.style.left = '25%';
      statusMessageWindow.style.top = '25%';
      statusMessageWindow.style.position = 'absolute';
      statusMessageBlock.innerHTML =
            '<table style="border:none; height:100%; width:100%"><tr><td style="text-align:center;">'
          + message
          + '</td></tr></table>';
  	
      document.body.insertBefore (statusMessageWindow, this.TheForm);
      AlignStatusMessage (statusMessageWindow);
      _statusMessageWindow = statusMessageWindow;
    }
  }
  
  function AlignStatusMessage (message)
  {
    var scrollLeft = window.document.body.scrollLeft;
    var scrollTop = window.document.body.scrollTop;
    var windowWidth = window.document.body.clientWidth;
    var windowHeight = window.document.body.clientHeight;
    
    message.style.left = windowWidth/2 - message.offsetWidth/2 + scrollLeft;
    message.style.top = windowHeight/2 - message.offsetHeight/2 + scrollTop;
  }

  function IsFocusableTag (tagName) 
  {
    if (tagName == null)
      return false;
    tagName = tagName.toLowerCase();
    return (tagName == "a" ||
            tagName == "button" ||
            tagName == "input" ||
            tagName == "textarea" ||
            tagName == "select");
  }

  function IsJavaScriptAnchor (element)
  {
    if (element == null)
      return false;

    var tagName = element.tagName.toLowerCase();
    if (   tagName == 'a'
        && element.href != null
        && element.href.substring (0, 11).toLowerCase() == 'javascript:')
    {
      return true;
    }
    else if (   tagName == 'p'
             || tagName == 'div'
             || tagName == 'td'
             || tagName == 'table'
             || tagName == 'form')
    {
      return false;
    }
    else
    {
      return IsJavaScriptAnchor (element.parentElement);
    }
  }
}

function Wxe_OnScroll()
{
  _wxe_context.OnScroll();
}

function Wxe_OnResize()
{
  _wxe_context.OnResize();
}

function Wxe_OnFormClick (evt)
{
  return _wxe_context.OnFormClick (evt);
}

function Wxe_OnFormSubmit()
{
  return _wxe_context.OnFormSubmit();
}

function Wxe_DoPostBack (eventTarget, eventArgument)
{
  _wxe_context.DoPostBack (eventTarget, eventArgument);
}

function Wxe_OnStartUp()
{
  _wxe_context.OnStartUp();
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

function Wxe_Refresh()
{
  _wxe_context.Refresh();
}

function Wxe_OnElementBlur (evt) 
{
	_wxe_context.SetActiveElement (null);
}

function Wxe_OnElementFocus (evt)
{
  var eventSource = Wxe_GetEventSource (evt);
  if (eventSource != null)
		_wxe_context.SetActiveElement (eventSource);
}

function Wxe_GetEventSource (evt)
{
	var e = evt ? evt : window.event;
	if (!e) 
	  return null;
	if (e.target)
		return e.target;
	else if (e.srcElement)
	  return e.srcElement;
}