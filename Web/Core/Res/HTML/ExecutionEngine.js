var _wxe_context = null;

function Wxe_Initialize (theForm, refreshInterval, refreshUrl, abortUrl, abortMessage, smartNavigationFieldID)
{
  _wxe_context = new Wxe_Context (theForm, refreshInterval, refreshUrl, abortUrl, abortMessage, smartNavigationFieldID);
  window.onload = Wxe_OnLoad;
  window.onbeforeunload = Wxe_OnBeforeUnload; // IE, Mozilla 1.7, Firefox 0.9
  window.onunload = Wxe_OnUnload;
}

function Wxe_Context (theForm, refreshInterval, refreshUrl, abortUrl, abortMessage, smartNavigationFieldID)
{
  this.TheForm = document.getElementById (theForm);
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
  this.SmartNavigationField = document.getElementById (smartNavigationFieldID);
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
	_wxe_context.TheForm.onsubmit = function() { _wxe_context.IsSubmit = true; };
	
	_wxe_context.AspnetDoPostBack = __doPostBack;
	__doPostBack = Wxe_DoPostBack;
//	SmartNavigationRestore ( _wxe_context.SmartNavigationField.value);
}

function Wxe_DoPostBack (eventTarget, eventArgument)
{
	_wxe_context.IsSubmit = true;
//	_wxe_context.SmartNavigationField.value = SmartNavigationBackup (eventTarget, eventArgument);
	_wxe_context.AspnetDoPostBack (eventTarget, eventArgument);
}

function Wxe_OnBeforeUnload ()
{
  if (_wxe_context.IsAbortConfirmationEnabled && ! _wxe_context.IsSubmit)
  {
    var activeElement = window.document.activeElement;
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
      var image = new Image();
      image.src = _wxe_context.AbortUrl;
    }
    catch (e)
    {
    }
  }
}
