using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Runtime.Remoting.Messaging;
using System.Web.UI.HtmlControls;

namespace Rubicon.Web.UI
{

/// <summary>
///   Provides a mechanism to register HTML header elements (e.g., stylesheet or script links).
/// </summary>
/// <example>
///   &lt;head&gt;
///     &lt;% Rubicon.Web.UI.HtmlHeaderFactory.Current.WriteHeaders (Context); %&gt;
///   &lt;/head&gt;
/// </example>
public class HtmlHeaderFactory
{
  /// <summary> Hashtable&lt;string key, string header&gt; </summary>
  private Hashtable _registeredHeaders = new Hashtable();
  /// <summary> <see langword="true"/> if <see cref="AppendHeaders"/> has already executed. </summary>
  private bool _hasAppendHeadersExecuted = false;

  /// <summary>
  ///   Gets the <see cref="HtmlHeaderFactory"/> instance.
  /// </summary>
  public static HtmlHeaderFactory Current
  {
    get 
    { 
      const string contextKey = "Rubicon.Web.HtmlHeaderFactory.Curremt";
      HtmlHeaderFactory current = (HtmlHeaderFactory) CallContext.GetData (contextKey);
      if (current == null)
      {
        lock (typeof (HtmlHeaderFactory))
        {
          current = (HtmlHeaderFactory) CallContext.GetData (contextKey);
          if (current == null)
          {
            current = new HtmlHeaderFactory();
            CallContext.SetData (contextKey, current);
          }
        }
      }
      return current;
    }
  }

  /// <summary>
  ///   Appends the <c>HTML headers</c> registered with the <see cref="Current"/>
  ///   <see cref="HtmlHeaderFactory"/> to the <paramref name="headerCollection"/>.
  /// </summary>
  /// <remarks>
  ///   Call this method during the <c>PreRender</c> phase.
  /// </remarks>
  /// <param name="headerCollection">
  ///   <see cref="ControlCollection"/> to which the headers will be appended.
  /// </param>
  public void EnsureAppendHeaders (ControlCollection headerCollection)
  {
    if (_hasAppendHeadersExecuted)
      return;

    foreach (Control header in _registeredHeaders.Values)
      headerCollection.Add (header);

    _hasAppendHeadersExecuted = true;
  }

  /// <summary> Registers a stylesheet file. </summary>
  /// <remarks>
  ///   All calls to <see cref="RegisterStylesheetLink"/> must be completed before
  ///   <see cref="AppendHeaders"/> is called. (Typically during <c>Render</c> phase.)
  /// </remarks>
  /// <param name="key"> The unique key identifying the stylesheet file in the headers collection. </param>
  /// <param name="href"> The url of the stylesheet file. </param>
  /// <exception cref="HttpException"> 
  ///   Thrown if method is called after <see cref="AppendHeaders"/> has executed.
  /// </exception>
  public void RegisterStylesheetLink (string key, string href)
  {
    HtmlGenericControl header = new HtmlGenericControl ("link");
    header.Attributes.Add ("type", "text/css");
    header.Attributes.Add ("rel", "stylesheet");
    header.Attributes.Add ("href", href);
    RegisterHeader (key, header);
  }

  /// <summary> Registers a javascript file. </summary>
  /// <remarks>
  ///   All calls to <see cref="RegisterJavaScriptInclude"/> must be completed before
  ///   <see cref="AppendHeaders"/> is called. (Typically during <c>Render</c> phase.)
  /// </remarks>
  /// <param name="key"> The unique key identifying the javascript file in the headers collection. </param>
  /// <param name="src"> The url of the javascript file. </param>
  /// <exception cref="HttpException"> 
  ///   Thrown if method is called after <see cref="AppendHeaders"/> has executed.
  /// </exception>
  public void RegisterJavaScriptInclude (string key, string src)
  {
    HtmlGenericControl header = new HtmlGenericControl ("script");
    header.Attributes.Add ("type", "text/javascript");
    header.Attributes.Add ("src", src);
    RegisterHeader (key, header);
  }

  /// <summary> Registers a <see cref="Control"/> containing an HTML header element. </summary>
  /// <remarks>
  ///   All calls to <see cref="RegisterHeader"/> must be completed before
  ///   <see cref="AppendHeaders"/> is called. (Typically during <c>Render</c> phase.)
  /// </remarks>
  /// <param name="key"> The unique key identifying the header element in the collection. </param>
  /// <param name="header"> The <see cref="Control"/> representing the header element. </param>
  /// <exception cref="HttpException"> 
  ///   Thrown if method is called after <see cref="AppendHeaders"/> has executed.
  /// </exception>
  public void RegisterHeader (string key, Control header)
  {
    if (_hasAppendHeadersExecuted)
      throw new HttpException ("RegisterHeader must not be called after AppendHeaders has executed.");
    if (! _registeredHeaders.Contains (key))
      _registeredHeaders.Add (key, header);
  }

  public bool IsRegistered (string key)
  {
    return _registeredHeaders.Contains (key);
  }
}

}
