var _wxe_context = null;

function Wxe_Initialize (
    theFormID, 
    refreshInterval, refreshUrl, 
    abortUrl, 
    statusIsAbortingMessage, statusIsCachedMessage)
{
  _wxe_context = new Wxe_Context (
      theFormID, 
      refreshInterval, refreshUrl, 
      abortUrl, 
      statusIsAbortingMessage, statusIsCachedMessage);
  
  _wxe_context.Init();
}

function Wxe_Context (
      theFormID, 
      refreshInterval, refreshUrl, 
      abortUrl, 
      statusIsAbortingMessage, statusIsCachedMessage)
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

  var _isSubmitting = false;
  var _hasSubmitted = false;
  // Special flag to support the OnBeforeUnload part
  var _isSubmittingBeforeUnload = false;

  var _isAborting = false;
  var _isCached = false;
  // Special flag to support the OnBeforeUnload part
  var _isAbortingBeforeUnload = false;
  var _statusIsAbortingMessage = statusIsAbortingMessage;

  var _statusIsCachedMessage = statusIsCachedMessage;
  var _hasUnloaded = false;
  var _isMsIEAspnetPostBack = false;
  var _isMsIEFormClicked = false;
    
  var _activeElement = null;
  var _isMsIE = window.navigator.appName.toLowerCase().indexOf("microsoft") > -1;

  this.Init = function()
  {
  }

  this.OnLoad = function (hasSubmitted, isCached)
  {
    if (hasSubmitted || isCached)
      this.ShowStatusIsCachedMessage ();
  }
  
  this.OnAbort = function (hasSubmitted, isCached)
  {
    if (! isCached && _isAbortEnabled)
      this.SendSessionRequest (_abortUrl);
  }
  
  this.Refresh = function ()
  {
    this.SendSessionRequest (_refreshUrl + '&Wxe_Garbage=' + Math.random())
  }
  
  this.SendSessionRequest = function (url)
  {
    _smartPage_context.SendOutOfBandRequest (url);
  }
  
  // returns: true to continue with request
  this.CheckFormState = function (isAborting, hasSubmitted, hasUnloaded, isCached)
  {
    if (hasSubmitted || isCached || hasUnloaded)
    {
      this.ShowStatusIsCachedMessage();
      return false;
    }
    if (isAborting)
    {
      this.ShowStatusIsAbortingMessage();
      return false;
    }
    else
    {
      return true;
    }
  }

  this.ShowStatusIsAbortingMessage = function ()
  {
    if (_statusIsAbortingMessage != null)
      window._smartPage_context.ShowMessage ('WxeStatusIsAbortingMessage', _statusIsAbortingMessage);
  }

  this.ShowStatusIsCachedMessage = function ()
  {
    if (_statusIsCachedMessage != null)
      _smartPage_context.ShowMessage ('WxeStatusIsCachedMessage', _statusIsCachedMessage);
  }
}

function Wxe_OnLoad (hasSubmitted, isCached)
{
  _wxe_context.OnLoad (hasSubmitted, isCached);
}

function Wxe_OnBeforeUnload()
{
  _wxe_context.OnBeforeUnload();
}

function Wxe_OnUnload()
{
  _wxe_context.OnUnload();
}

function Wxe_OnAbort (hasSubmitted, isCached)
{
  _wxe_context.OnAbort (hasSubmitted, isCached);
}

function Wxe_Refresh()
{
  _wxe_context.Refresh();
}

function Wxe_CheckFormState (isAborting, hasSubmitted, hasUnloaded, isCached)
{
  return _wxe_context.CheckFormState (isAborting, hasSubmitted, hasUnloaded, isCached)
}
