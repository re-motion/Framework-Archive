using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Runtime.Remoting.Messaging;

namespace Rubicon.Web.UI
{

/// <summary>
///   Provides a mechanism to register HTML header elements (e.g., stylesheet or script links).
/// </summary>
/// <example>
///   &lt;head&gt;
///     &lt;% Rubicon.Web.UI.HeaderFactory.Current.WriteHeaders (Context); %&gt;
///   &lt;/head&gt;
/// </example>
public class HeaderFactory
{
  /// <summary> Hashtable&lt;string key, string header&gt; </summary>
  private Hashtable _registeredHeaders = new Hashtable();

  public static HeaderFactory Current
  {
    get 
    { 
      const string contextKey = "Rubicon.Web.HeaderFactory.Curremt";
      HeaderFactory current = (HeaderFactory) CallContext.GetData (contextKey);
      if (current == null)
      {
        lock (typeof (HeaderFactory))
        {
          current = (HeaderFactory) CallContext.GetData (contextKey);
          if (current == null)
          {
            current = new HeaderFactory();
            CallContext.SetData (contextKey, current);
          }
        }
      }
      return current;
    }
  }

  public void WriteHeaders (HttpContext context)
  {
    foreach (string header in _registeredHeaders.Values)
    {
      context.Response.Write (header);
    }
  }

  public void RegisterHeaderStylesheetLink (string key, string url)
  {
    RegisterHeader (key, "<link type=\"css/stylesheet\" ...");
  }

  public void RegisterHeader (string key, string header)
  {
    if (! _registeredHeaders.Contains (key))
      _registeredHeaders.Add (key, header);
  }
}

}
