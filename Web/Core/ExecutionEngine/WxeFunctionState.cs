using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Web;

namespace Rubicon.Web.ExecutionEngine
{

[Serializable]
public class WxeWindowStateCollection
{
  public static WxeWindowStateCollection Instance
  {
    get { return (WxeWindowStateCollection) HttpContext.Current.Session["WxeWindowStates"]; }
    set { HttpContext.Current.Session["WxeWindowStates"] = value; }
  }

  private ArrayList _windowStates = new ArrayList();

  public void DisposeExpired()
  {
    for (int i = _windowStates.Count - 1; i >= 0; --i)
    {
      WxeWindowState window = (WxeWindowState) _windowStates[i];
      if (window.IsExpired)
      {
        _windowStates.RemoveAt (i);
        window.Dispose();
      }
    }
  }

  public void Add (WxeWindowState windowState)
  {
    _windowStates.Add (windowState);
  }

  public WxeWindowState GetItem (string windowToken)
  {
    foreach (WxeWindowState window in _windowStates)
    {
      if (window.WindowToken == windowToken)
        return window;
    }
    return null;
  }

  public void Remove (WxeWindowState windowState)
  {
    _windowStates.Remove (windowState);
    windowState.Dispose();
  }
}

/// <summary>
///   Stores the session state for a single page token.
/// </summary>
[Serializable]
public class WxeWindowState: ISerializable, IDisposable
{
  private WxeFunction _function;
  private DateTime _lastAccess;
  private int _lifetime;
  private string _windowToken;

  public WxeWindowState (WxeFunction function, int lifetime)
  {
    _function = function;
    _lastAccess = DateTime.Now;
    _lifetime = lifetime;
    _windowToken = Guid.NewGuid().ToString();
  }

  protected WxeWindowState (SerializationInfo info, StreamingContext context)
  {
    _function = (WxeFunction) info.GetValue ("_function", typeof (WxeFunction));
    _lastAccess = info.GetDateTime ("_lastAccess");
    _lifetime = info.GetInt32 ("_lifetime");
    _windowToken = info.GetString ("_windowToken");
  }

  void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
  {
    info.AddValue ("_function", _function, typeof (WxeFunction));
    info.AddValue ("_lastAccess", _lastAccess);
    info.AddValue ("_lifetime", _lifetime);
    info.AddValue ("_windowToken", _windowToken);
  }

  public WxeFunction Function
  {
    get { return _function; }
  }

  public DateTime LastAccess
  {
    get { return _lastAccess; }
  }

  public int Lifetime
  {
    get { return _lifetime; }
  }

  public string WindowToken
  {
    get { return _windowToken; }
  }

  public void Touch()
  {
    _lastAccess = DateTime.Now;
  }

  public bool IsExpired
  {
    get { return _lastAccess + new TimeSpan (0, _lifetime, 0) < DateTime.Now; }
  }

  public void Dispose()
  {
    Dispose (true);
    GC.SuppressFinalize (this);
  }

  protected virtual void Dispose (bool disposing)
  {
    if (disposing)
    {
      if (_function != null)
        _function.Dispose();
    }
  }

  ~WxeWindowState()
  {
    Dispose (false);
  }
}

}
