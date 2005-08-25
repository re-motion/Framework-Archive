var _wxe_context = null;

function Wxe_Initialize (theFormID, refreshInterval, refreshUrl, abortUrl, abortMessage, smartNavigationFieldID)
{
  _wxe_context = new Wxe_Context (theFormID, refreshInterval, refreshUrl, abortUrl, abortMessage, smartNavigationFieldID);
  window.onload = Wxe_OnLoad;
  window.onbeforeunload = Wxe_OnBeforeUnload; // IE, Mozilla 1.7, Firefox 0.9
  window.onunload = Wxe_OnUnload;
}

function Wxe_Context (theFormID, refreshInterval, refreshUrl, abortUrl, abortMessage, smartNavigationFieldID)
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
  this.SmartNavigationField = this.TheForm.elements[smartNavigationFieldID];
  
  this.Backup = function()
  {
    return;
    if (this.SmartNavigationField != null)
      this.SmartNavigationField.value = SmartNavigation_Backup (document.activeElement);
  }
  
  this.Restore = function()
  {
    return;
    if (this.SmartNavigationField != null)
  	  SmartNavigation_Restore (this.SmartNavigationField.value);
  }
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

/*
<!--StartFragment -->
<pre>var activeElement = <font color="navy"><b>null</b></font>;
function blurHandler(evt) <font color="navy">{</font>
	activeElement = <font color="navy"><b>null</b></font>;
<font color="navy">}</font>
function focusHandler(evt) <font color="navy">{</font>
	var e = evt ? evt : window.event;
	<font color="navy"><b>if</b></font> (!e) <font color="navy"><b>return</b></font>;
	<font color="navy"><b>if</b></font> (e.target)
		activeElement = e.target;
	<font color="navy"><b>else</b></font> <font color="navy"><b>if</b></font>(e.srcElement) activeElement = e.srcElement;
  
<font color="navy">}</font>
function loadHandler() <font color="navy">{</font>
	var i, j;
	
	<font color="navy"><b>for</b></font> (i = 0; i &lt; document.forms.length; i++)
		<font color="navy"><b>for</b></font> (j = 0; j &lt; document.forms[i].elements.length; j++) <font color="navy">{</font>
			document.forms[i].elements[j].onfocus = focusHandler
			document.forms[i].elements[j].onblur  = blurHandler
		<font color="navy">}</font>
<font color="navy">}</font>
window.onload = loadHandler</pre>
*/