var _smartPage_context = null;

function SmartPage_Initialize (
    theFormID, 
    statusIsSubmittingMessage, abortMessage, 
    smartScrollingFieldID, smartFocusFieldID,
    eventHandlers)
{
  _smartPage_context = new SmartPage_Context (
    theFormID, 
    statusIsSubmittingMessage, abortMessage, 
    smartScrollingFieldID, smartFocusFieldID,
    eventHandlers)
  
  _smartPage_context.Init();
}

function SmartPage_Context (
    theFormID, 
    statusIsSubmittingMessage, abortMessage, 
    smartScrollingFieldID, smartFocusFieldID,
    eventHandlers)
{
  this.TheForm = document.forms[theFormID];
    
  var _abortMessage = abortMessage;
  var _isAbortConfirmationEnabled = abortMessage != null;

  var _isSubmitting = false;
  var _hasSubmitted = false;
  // Special flag to support the OnBeforeUnload part
  var _isSubmittingBeforeUnload = false;
  var _statusIsSubmittingMessage = statusIsSubmittingMessage;

  var _isAborting = false;
  var _hasAborted = false;
  // Special flag to support the OnBeforeUnload part
  var _isAbortingBeforeUnload = false;

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

  this.OverrideAspNetDoPostBack = function ()
  {
    _aspnetFormOnSubmit = this.TheForm.onsubmit;
	  this.TheForm.onsubmit = SmartPage_OnFormSubmit;
    this.TheForm.onclick = SmartPage_OnFormClick;
	  _aspnetDoPostBack = __doPostBack;
	  __doPostBack = SmartPage_DoPostBack;
  }
  
  this.SetEventHandlers = function ()
  {
    window.onload = SmartPage_OnLoad;
    window.onbeforeunload = SmartPage_OnBeforeUnload; // IE, Mozilla 1.7, Firefox 0.9
    window.onunload = SmartPage_OnUnload;
    window.onscroll = SmartPage_OnScroll;
    window.onresize = SmartPage_OnResize;
  }
 
  this.SetFocusEventHandlers = function (currentElement)
  {
    if (currentElement != null)
    {
      if (   typeof (currentElement.id) != 'undefined' && currentElement.id != null && currentElement.id != '' 
          && IsFocusableTag (currentElement.tagName))
      {
		    currentElement.onfocus = SmartPage_OnElementFocus;
		    currentElement.onblur  = SmartPage_OnElementBlur;
      }
      
      for (var i = 0; i < currentElement.childNodes.length; i++)
      {
        var element = currentElement.childNodes[i];
        this.SetFocusEventHandlers (element);
      }
    }
  }

  this.OnStartUp = function ()
  {
    if (! _isMsIE)
  	  this.SetFocusEventHandlers (document.body);
  }

  this.OnLoad = function ()
  {
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
      // IE alternate/official version: window.event.returnValue = _smartPage_context.AbortMessage
      return _abortMessage;
    }
  }
  
  this.OnUnload = function ()
  {
    if (   (! _isSubmitting || _isAbortingBeforeUnload)
        && (! _hasAborted || _hasSubmitted)
        && ! _isAborting)
    {
      _isAborting = true;
      this.ExecuteEventHandlers (_eventHandlers['onabort']);
            
      _isAbortingBeforeUnload = false;
    }
    _hasUnloaded = true;
    this.ExecuteEventHandlers (_eventHandlers['onunload']);
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
    
	    _aspnetDoPostBack (eventTarget, eventArgument);
      if (_isMsIE)
	    {
	      if (! _isMsIEFormClicked)
  	      _isMsIEAspnetPostBack = true;
        _isMsIEFormClicked = false;
	    }
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
      
    var eventSource = GetEventSource (evt);
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

  this.SendOutOfBandRequest = function (url)
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

  this.ShowStatusIsSubmittingMessage = function ()
  {
    if (_statusIsSubmittingMessage != null)
      this.ShowMessage ('SmartPageStatusIsSubmittingMessage', _statusIsSubmittingMessage);
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
        && typeof (element.href) != 'undefined' && element.href != null
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

  this.OnElementBlur = function (evt) 
  {
	  this.SetActiveElement (null);
  }

  this.OnElementFocus = function (evt)
  {
    this.OnElementFocus (evt);
    var eventSource = GetEventSource (evt);
    if (eventSource != null)
		  this.SetActiveElement (eventSource);
  }

  function GetEventSource (evt)
  {
	  var e = evt ? evt : window.event;
	  if (!e) 
	    return null;
	  if (e.target)
		  return e.target;
	  else if (e.srcElement)
	    return e.srcElement;
  }
}

function SmartPage_OnScroll()
{
  _smartPage_context.OnScroll();
}

function SmartPage_OnResize()
{
  _smartPage_context.OnResize();
}

function SmartPage_OnFormClick (evt)
{
  return _smartPage_context.OnFormClick (evt);
}

function SmartPage_OnFormSubmit()
{
  return _smartPage_context.OnFormSubmit();
}

function SmartPage_DoPostBack (eventTarget, eventArgument)
{
  _smartPage_context.DoPostBack (eventTarget, eventArgument);
}

function SmartPage_OnStartUp()
{
  _smartPage_context.OnStartUp();
}

function SmartPage_OnLoad()
{
  _smartPage_context.OnLoad();
}

function SmartPage_OnBeforeUnload()
{
  return _smartPage_context.OnBeforeUnload();
}

function SmartPage_OnUnload()
{
  _smartPage_context.OnUnload();
}

function SmartPage_OnElementBlur (evt) 
{
  _smartPage_context.OnElementBlur (evt);
}

function SmartPage_OnElementFocus (evt)
{
  _smartPage_context.OnElementFocus (evt);
}
