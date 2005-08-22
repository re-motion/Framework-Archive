using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Web;
using log4net;
using Rubicon.Utilities;
using Rubicon.Web.Configuration;

namespace Rubicon.Web.ExecutionEngine
{

// TODO: ASP.NET 2.0 deserializes session state items only as they are requested. therefore, store each window
// state, each lifetime and each access time  in a separate session state key, e.g.:
// WxeFunction-<token>, WxeFunctionLifetime-<token>, WxeFunctionLastAccess-<token>
[Serializable]
public class WxeFunctionStateCollection
{
  private static readonly string s_sessionKey = typeof (WxeFunctionStateCollection).FullName;
  public static WxeFunctionStateCollection Instance
  {
    get { return (WxeFunctionStateCollection) HttpContext.Current.Session[s_sessionKey]; }
    set { HttpContext.Current.Session[s_sessionKey] = value; }
  }

  private ArrayList _functionStates = new ArrayList();

  /// <summary> Cleans up expired <see cref="WxeFunctionState"/> objects in the collection. </summary>
  /// <remarks> Removes and aborts expired function states. </remarks>
  public void CleanUpExpired()
  {
    for (int i = _functionStates.Count - 1; i >= 0; --i)
    {
      WxeFunctionState window = (WxeFunctionState) _functionStates[i];
      if (window.IsExpired)
      {
        _functionStates.RemoveAt (i);
        window.Abort();
      }
    }
  }

  /// <summary> Adds the <paramref name="functionState"/> to the collection. </summary>
  /// <param name="functionState"> 
  ///   The <see cref="WxeFunctionState"/> to be added. Must not be <see langword="null"/> or aborted.
  /// </param>
  public void Add (WxeFunctionState functionState)
  {
    ArgumentUtility.CheckNotNull ("functionState", functionState);
    if (functionState.IsAborted)
      throw new ArgumentException ("An aborted WxeFunctionState cannot be added to the collection.", "functionState");
    _functionStates.Add (functionState);
  }

   /// <summary> Gets the <see cref="WxeFunctionState"/> for the specifed <paramref name="functionToken"/>. </summary>
  /// <param name="functionToken"> 
  ///   The token to look-up the <see cref="WxeFunctionState"/>. Must not be <see langword="null"/> or empty.
  /// </param>
  /// <returns> The <see cref="WxeFunctionState"/> for the specified <paramref name="functionToken"/>. </returns>
 public WxeFunctionState GetItem (string functionToken)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("functionToken", functionToken);
    foreach (WxeFunctionState window in _functionStates)
    {
      if (window.FunctionToken == functionToken)
        return window;
    }
    return null;
  }

  /// <summary> Removes the <paramref name="functionState"/> from the collection. </summary>
  /// <param name="functionState"> 
  ///   The <see cref="WxeFunctionState"/> to be removed. Must not be <see langword="null"/>.
  /// </param>
  protected void Remove (WxeFunctionState functionState)
  {
    ArgumentUtility.CheckNotNull ("functionState", functionState);
    _functionStates.Remove (functionState);
  }

  /// <summary> Removes and aborts the <paramref name="functionState"/> from the collection. </summary>
  /// <param name="functionState"> 
  ///   The <see cref="WxeFunctionState"/> to be removed. Must not be <see langword="null"/>.
  /// </param>
  public void Abort (WxeFunctionState functionState)
  {
    Remove (functionState);
    functionState.Abort();
  }
}

/// <summary>
///   Stores the session state for a single function token.
/// </summary>
[Serializable]
public class WxeFunctionState
{
  private static ILog s_log = LogManager.GetLogger (typeof (WxeFunctionState));

  private WxeFunction _function;
  private DateTime _lastAccess;
  private int _lifetime;
  private string _functionToken;
  private bool _isAborted;

  public WxeFunctionState (WxeFunction function, int lifetime)
    : this (function, Guid.NewGuid().ToString(), lifetime)
  {
  }

  public WxeFunctionState (WxeFunction function, string functionToken)
    : this (function, functionToken, WebConfiguration.Current.ExecutionEngine.FunctionTimeout)
  {
  }
  
  public WxeFunctionState (WxeFunction function)
    : this (function, Guid.NewGuid().ToString(), WebConfiguration.Current.ExecutionEngine.FunctionTimeout)
  {
  }

  public WxeFunctionState (WxeFunction function, string functionToken, int lifetime)
  {
    ArgumentUtility.CheckNotNull ("function", function);
    ArgumentUtility.CheckNotNullOrEmpty ("functionToken", functionToken);
    _function = function;
    _lastAccess = DateTime.Now;
    _lifetime = lifetime;
    _functionToken = functionToken;
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

  public bool IsAborted
  {
    get { return _isAborted; }
  }

  /// <summary> Aborts the <b>WxeFunctionState</b> by calling <see cref="AbortRecursive"/>. </summary>
  /// <remarks> 
  ///   Use the <see cref="WxeFunctionStateCollection.Abort">WxeFunctionStateCollection.Abort</see> method to abort
  ///   a <b>WxeFunctionState</b>.
  /// </remarks>
  protected internal void Abort()
  {
    if (! _isAborted)
    {
      s_log.Debug ("Aborting Function State " + _functionToken + ".");
      AbortRecursive();
      _isAborted = true;
    }
  }

  /// <summary> Aborts the <b>WxeFunctionState</b>. </summary>
  protected virtual void AbortRecursive()
  {
    if (_function != null)
      _function.Abort();
  }
}

}
