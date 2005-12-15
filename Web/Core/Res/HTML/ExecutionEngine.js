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
  var _hasAborted = false;
  // Special flag to support the OnBeforeUnload part
  var _isAbortingBeforeUnload = false;
  var _statusIsAbortingMessage = statusIsAbortingMessage;

  var _statusIsCachedMessage = statusIsCachedMessage;
  var _hasUnloaded = false;
  var _isMsIEAspnetPostBack = false;
  var _isMsIEFormClicked = false;
    
  var _activeElement = null;
  var _isMsIE = window.navigator.appName.toLowerCase().indexOf("microsoft") > -1;
  var _cacheStateHasSubmitted = 'hasSubmitted';
  var _cacheStateHasLoaded = 'hasLoaded';

  this.Init = function()
  {
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

  this.OnLoad = function ()
  {
    this.CheckIfCached();
  }
  
  this.OnPostback = function ()
  {
    this.SetCacheDetectionFieldSubmitted();
  }
  
  this.OnAbort = function ()
  {
    if (_isAbortEnabled)
    {  
      this.SetCacheDetectionFieldAborted();
      this.SendSessionRequest (_abortUrl);
    }  
  }
  
  this.Refresh = function ()
  {
    this.SendSessionRequest (_refreshUrl + '&Wxe_Garbage=' + Math.random())
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

  this.ShowStatusIsAbortingMessage = function ()
  {
    if (_statusIsAbortingMessage != null)
      _smartPage_context.ShowMessage ('WxeStatusIsAbortingMessage', _statusIsAbortingMessage);
  }

  this.ShowStatusIsCachedMessage = function ()
  {
    if (_statusIsCachedMessage != null)
      _smartPage_context.ShowMessage ('WxeStatusIsCachedMessage', _statusIsCachedMessage);
  }
}

function Wxe_OnLoad()
{
  _wxe_context.OnLoad();
}

function Wxe_OnBeforeUnload()
{
  _wxe_context.OnBeforeUnload();
}

function Wxe_OnUnload()
{
  _wxe_context.OnUnload();
}

function Wxe_OnPostback (eventTarget, eventArgument)
{
  _wxe_context.OnPostback (eventTarget, eventArgument);
}

function Wxe_Refresh()
{
  _wxe_context.Refresh();
}
