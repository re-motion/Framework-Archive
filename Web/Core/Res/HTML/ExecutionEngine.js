var _wxe_context = null;

function Wxe_Initialize (theForm, refreshInterval, refreshUrl, abortUrl, abortMessage)
{
  _wxe_context = new Wxe_Context (theForm, refreshInterval, refreshUrl, abortUrl, abortMessage);
  window.onload = Wxe_OnLoad;
  window.onbeforeunload = Wxe_OnBeforeUnload; // IE, Mozilla 1.7, Firefox 0.9
  window.onunload = Wxe_OnUnload;
}

function Wxe_Context (theForm, refreshInterval, refreshUrl, abortUrl, abortMessage)
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
	__doPostBack = function (eventTarget, eventArgument)
	    {
	      _wxe_context.IsSubmit = true;
	      //SmartNavigation (document.getElementById ('eventTarget'));
	      _wxe_context.AspnetDoPostBack (eventTarget, eventArgument);
	    };
	//SmartNavigationRestore();
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
    //SmartNavigation (null);
  }
}

function SmartNavigationRestore()
{
  var scrollParent = document.getElementById ('MultiView_ActiveView');
  var scrollTop = 169;
  var scrollLeft = 0;
  if (scrollParent != null)
  {
    scrollParent.scrollTop = scrollTop;
    scrollParent.scrollLeft = scrollLeft;
  }
  
  var focusElement = document.getElementById ('TestTabbedPersonJobsUserControl_MultilineTextField_Boc_TextBox');
  var offsetLeft = 417;
  var offsetTop = 605;  
  if (focusElement != null)
  {
    focusElement.focus();
  }
}

function SmartNavigation (srcElement)
{
  var scrollParent = null;
  for (var currentNode = srcElement; currentNode != null; currentNode = currentNode.offsetParent)
  {
    if (   currentNode.style.overflow.toLowerCase() == 'auto' 
        || currentNode.style.overflow.toLowerCase() == 'scroll')
    {
      scrollParent = currentNode;
      break;
    }
  }
  if (scrollParent != null)
  {
    var scrollElement = document.getElementById ('smartNavigationScrollElement');
    var scrollTop = document.getElementById ('smartNavigationScrollTop');
    var scrollLeft = document.getElementById ('smartNavigationScrollLeft');
    scrollElement.value = scrollParent.id;
    scrollTop.value = scrollParent.scrollTop;
    scrollLeft.value = scrollParent.scrollLeft;
  }
  if (srcElement != null)
  {
    var focus = document.getElementById ('smartNavigationFocus');
    focus.value = srcElement.id;
  }
}
