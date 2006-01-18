// Requires: Utilities.js, SmartPage.js

// The context contains all information required by the WXE page.
// refreshInterval: The refresh interfal in milli-seconds. zero to disable refreshing.
// refreshUrl: The URL used to post the refresh request to. Must not be null if refreshInterval is greater than zero.
// abortUrl: The URL used to post the abort request to. null to disable the abort request.
// statusIsAbortingMessage: The message displayed when the user attempts to submit while an abort is in progress. 
//    null to disable the message.
// statusIsCachedMessage: The message displayed when the user returns to a cached page. null to disable the message.
function WxePage_Context (
      refreshInterval, refreshUrl, 
      abortUrl, 
      statusIsAbortingMessage, statusIsCachedMessage)
{
  ArgumentUtility.CheckNotNullAndTypeIsNumber ('refreshInterval', refreshInterval);
  ArgumentUtility.CheckTypeIsString ('refreshUrl', refreshUrl);
  ArgumentUtility.CheckTypeIsString ('abortUrl', abortUrl);
  ArgumentUtility.CheckTypeIsString ('statusIsAbortingMessage', statusIsAbortingMessage);
  ArgumentUtility.CheckTypeIsString ('statusIsCachedMessage', statusIsCachedMessage);

  // The URL used to post the refresh request to.
  var _refreshUrl = null;
  // The timer used to invoke the refreshing.
  var _refreshTimer = null;
  if (refreshInterval > 0)
  {
    ArgumentUtility.CheckNotNull ('refreshUrl', refreshUrl);
    _refreshUrl = refreshUrl;
    _refreshTimer = window.setInterval (function() { WxePage_Context.Instance.Refresh(); }, refreshInterval);
  };

  // The URL used to post the abort request to.
  var _abortUrl = abortUrl;
  var _isAbortEnabled = abortUrl != null;

  // The message displayed when the user attempts to submit while an abort is in progress. null to disable the message.
  var _statusIsAbortingMessage = statusIsAbortingMessage;
  // The message displayed when the user returns to a cached page. null to disable the message.
  var _statusIsCachedMessage = statusIsCachedMessage;

  // Handles the page load event.
  this.OnLoad = function (hasSubmitted, isCached)
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean ('hasSubmitted', hasSubmitted);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean ('isCached', isCached);

    if (hasSubmitted || isCached)
      this.ShowStatusIsCachedMessage ();
  };
  
  // Handles the page abort event.
  this.OnAbort = function (hasSubmitted, isCached)
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean ('hasSubmitted', hasSubmitted);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean ('isCached', isCached);

    if (   (! isCached || hasSubmitted)
        && _isAbortEnabled)
    {
      SmartPage_Context.Instance.SendOutOfBandRequest (_abortUrl);
    }
  };
  
  // Handles the refresh timer events
  this.Refresh = function ()
  {
    SmartPage_Context.Instance.SendOutOfBandRequest (_refreshUrl + '&WxePage_Garbage=' + Math.random())
  };
    
  // Evaluates whether the postback request should continue.
  // returns: true to continue with request
  this.CheckFormState = function (isAborting, hasSubmitted, hasUnloaded, isCached)
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean ('isAborting', isAborting);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean ('hasSubmitted', hasSubmitted);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean ('hasUnloaded', hasUnloaded);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean ('isCached', isCached);

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
  };
  
  // Shows the "page is aborting" message
  this.ShowStatusIsAbortingMessage = function ()
  {
    if (_statusIsAbortingMessage != null)
      SmartPage_Context.Instance.ShowMessage ('WxeStatusIsAbortingMessage', _statusIsAbortingMessage);
  };

  // Shows the "page is cached" message
  this.ShowStatusIsCachedMessage = function ()
  {
    if (_statusIsCachedMessage != null)
      SmartPage_Context.Instance.ShowMessage ('WxeStatusIsCachedMessage', _statusIsCachedMessage);
  };
}

// The single instance of the WxePage_Context object
WxePage_Context.Instance = null;

function WxePage_OnLoad (hasSubmitted, isCached)
{
  WxePage_Context.Instance.OnLoad (hasSubmitted, isCached);
}

function WxePage_OnUnload()
{
  WxePage_Context.Instance.OnUnload();
}

function WxePage_OnAbort (hasSubmitted, isCached)
{
  WxePage_Context.Instance.OnAbort (hasSubmitted, isCached);
}

function WxePage_CheckFormState (isAborting, hasSubmitted, hasUnloaded, isCached)
{
  return WxePage_Context.Instance.CheckFormState (isAborting, hasSubmitted, hasUnloaded, isCached);
}
