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
///   protected override void RenderChildren(HtmlTextWriter writer)
///   {
///     HtmlHeadAppender.Current.EnsureAppended (HtmlHeader.Controls);
///     base.RenderChildren (writer);
///   }
/// </example>
public class HtmlHeadAppender
{
  /// <summary> Hashtable&lt;string key, string headElement&gt; </summary>
  private Hashtable _registeredHeadElements = new Hashtable();
  /// <summary> <see langword="true"/> if <see cref="EnsureAppended"/> has already executed. </summary>
  private bool _hasAppendExecuted = false;

  /// <remarks>
  ///   Factory pattern. No public construction.
  /// </remarks>
  private HtmlHeadAppender()
  {}

  /// <summary>
  ///   Gets the <see cref="HtmlHeadAppender"/> instance.
  /// </summary>
  public static HtmlHeadAppender Current
  {
    get 
    { 
      const string contextKey = "Rubicon.Web.HtmlHeadAppender.Curremt";
      HtmlHeadAppender current = (HtmlHeadAppender) CallContext.GetData (contextKey);
      if (current == null)
      {
        lock (typeof (HtmlHeadAppender))
        {
          current = (HtmlHeadAppender) CallContext.GetData (contextKey);
          if (current == null)
          {
            current = new HtmlHeadAppender();
            CallContext.SetData (contextKey, current);
          }
        }
      }
      return current;
    }
  }

  /// <summary>
  ///   Appends the <c>HTML headers</c> registered with the <see cref="Current"/>
  ///   <see cref="HtmlHeadAppender"/> to the <paramref name="headerCollection"/>.
  /// </summary>
  /// <remarks>
  ///   Call this method during in an override of <c>RenderChildren</c>.
  /// </remarks>
  /// <param name="headerCollection">
  ///   <see cref="ControlCollection"/> to which the headers will be appended.
  /// </param>
  public void EnsureAppended (ControlCollection headCollection)
  {
    if (_hasAppendExecuted)
      return;

    foreach (Control headElement in _registeredHeadElements.Values)
      headCollection.Add (headElement);

    _hasAppendExecuted = true;
  }

  /// <summary> Registers a stylesheet file. </summary>
  /// <remarks>
  ///   All calls to <see cref="RegisterStylesheetLink"/> must be completed before
  ///   <see cref="EnsureAppended"/> is called. (Typically during the <c>Render</c> phase.)
  /// </remarks>
  /// <param name="key"> The unique key identifying the stylesheet file in the headers collection. </param>
  /// <param name="href"> The url of the stylesheet file. </param>
  /// <exception cref="HttpException"> 
  ///   Thrown if method is called after <see cref="EnsureAppended"/> has executed.
  /// </exception>
  public void RegisterStylesheetLink (string key, string href)
  {
    HtmlGenericControl headElement = new HtmlGenericControl ("link");
    headElement.Attributes.Add ("type", "text/css");
    headElement.Attributes.Add ("rel", "stylesheet");
    headElement.Attributes.Add ("href", href);
    RegisterHeadElement (key, headElement);
  }

  /// <summary> Registers a javascript file. </summary>
  /// <remarks>
  ///   All calls to <see cref="RegisterJavaScriptInclude"/> must be completed before
  ///   <see cref="EnsureAppended"/> is called. (Typically during the <c>Render</c> phase.)
  /// </remarks>
  /// <param name="key"> The unique key identifying the javascript file in the headers collection. </param>
  /// <param name="src"> The url of the javascript file. </param>
  /// <exception cref="HttpException"> 
  ///   Thrown if method is called after <see cref="EnsureAppended"/> has executed.
  /// </exception>
  public void RegisterJavaScriptInclude (string key, string src)
  {
    HtmlGenericControl headElement = new HtmlGenericControl ("script");
    headElement.Attributes.Add ("type", "text/javascript");
    headElement.Attributes.Add ("src", src);
    RegisterHeadElement (key, headElement);
  }

  /// <summary> Registers a <see cref="Control"/> containing an HTML head element. </summary>
  /// <remarks>
  ///   All calls to <see cref="RegisterHeadElement"/> must be completed before
  ///   <see cref="EnsureAppended"/> is called. (Typically during the <c>Render</c> phase.)
  /// </remarks>
  /// <param name="key"> The unique key identifying the header element in the collection. </param>
  /// <param name="headElement"> The <see cref="Control"/> representing the head element. </param>
  /// <exception cref="HttpException"> 
  ///   Thrown if method is called after <see cref="EnsureAppended"/> has executed.
  /// </exception>
  public void RegisterHeadElement (string key, Control headElement)
  {
    if (_hasAppendExecuted)
      throw new HttpException ("RegisterHeadElement must not be called after EnsureAppended has executed.");
    if (! _registeredHeadElements.Contains (key))
      _registeredHeadElements.Add (key, headElement);
  }

  public bool IsRegistered (string key)
  {
    return _registeredHeadElements.Contains (key);
  }
}

}
