using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Web;
using Rubicon.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

// TODO: ASP.NET 2.0 deserializes session state items only as they are requested. therefore, store each window
// state, each lifetime and each access time  in a seperate session state key, e.g.:
// WxeFunction-<token>, WxeFunctionLifetime-<token>, WxeFunctionLastAccess-<token>
[Serializable]
public class WxeFunctionStateCollection
{
  public static WxeFunctionStateCollection Instance
  {
    get { return (WxeFunctionStateCollection) HttpContext.Current.Session["WxeFunctionStates"]; }
    set { HttpContext.Current.Session["WxeFunctionStates"] = value; }
  }

  private ArrayList _functionStates = new ArrayList();

  public void DisposeExpired()
  {
    for (int i = _functionStates.Count - 1; i >= 0; --i)
    {
      WxeFunctionState window = (WxeFunctionState) _functionStates[i];
      if (window.IsExpired)
      {
        _functionStates.RemoveAt (i);
        window.Dispose();
      }
    }
  }

  public void Add (WxeFunctionState functionState)
  {
    _functionStates.Add (functionState);
  }

  public WxeFunctionState GetItem (string functionToken)
  {
    foreach (WxeFunctionState window in _functionStates)
    {
      if (window.FunctionToken == functionToken)
        return window;
    }
    return null;
  }

  public void Remove (WxeFunctionState functionState)
  {
    _functionStates.Remove (functionState);
    functionState.Dispose();
  }
}

/// <summary>
///   Stores the session state for a single function token.
/// </summary>
[Serializable]
public class WxeFunctionState: ISerializable, IDisposable
{
  private WxeFunction _function;
  private DateTime _lastAccess;
  private int _lifetime;
  private string _functionToken;

  public WxeFunctionState (WxeFunction function, string functionToken, int lifetime)
  {
    ArgumentUtility.CheckNotNull ("function", function);
    ArgumentUtility.CheckNotNull ("functionToken", functionToken);
    _function = function;
    _lastAccess = DateTime.Now;
    _lifetime = lifetime;
    _functionToken = functionToken;
  }

  public WxeFunctionState (WxeFunction function, int lifetime)
    : this (function, Guid.NewGuid().ToString(), lifetime)
  {
  }

  protected WxeFunctionState (SerializationInfo info, StreamingContext context)
  {
    _function = (WxeFunction) info.GetValue ("_function", typeof (WxeFunction));
    _lastAccess = info.GetDateTime ("_lastAccess");
    _lifetime = info.GetInt32 ("_lifetime");
    _functionToken = info.GetString ("_functionToken");
  }

  void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
  {
    info.AddValue ("_function", _function, typeof (WxeFunction));
    info.AddValue ("_lastAccess", _lastAccess);
    info.AddValue ("_lifetime", _lifetime);
    info.AddValue ("_functionToken", _functionToken);
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

  public string FunctionToken
  {
    get { return _functionToken; }
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

  ~WxeFunctionState()
  {
    Dispose (false);
  }
}

}
