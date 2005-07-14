using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Runtime.Remoting.Messaging;
using System.Web.UI.HtmlControls;
using Rubicon.Web.Utilities;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UI
{

/// <summary>
///   Provides a mechanism to register HTML header elements (e.g., stylesheet or script links).
/// </summary>
/// <example>
///   Insert the following line into the head element of the webform you want to add 
///   the registered controls to.
///   <code>
///     &lt;rwc:htmlheadcontents runat="server" id="HtmlHeadContents"&gt;&lt;/rwc:htmlheadcontents&gt;
///   </code>
///   Register a new <c>HTML head element</c> using the following syntax.
///   <code>
///     HtmlHeadAppender.Current.Register...(key, ...);
///   </code>
/// </example>
public class HtmlHeadAppender
{
  public enum Prioritiy
  {
    Library = 0, // Absolute values to emphasize sorted nature of enum valies
    UserControl = 1,
    Page = 2
  }

  /// <summary> ListDictionary&lt;string key, Control headElement&gt; </summary>
  private ListDictionary _registeredHeadElements = new ListDictionary();
  /// <summary> SortedList&lt;HtmlHeadPrioritiy (int) priority + "." + string key, Control headElement&gt; </summary>
  private SortedList _sortedHeadElements = new SortedList();
  /// <summary> <see langword="true"/> if <see cref="EnsureAppended"/> has already executed. </summary>
  private bool _hasAppendExecuted = false;
  
  /// <remarks>
  ///   Factory pattern. No public construction.
  /// </remarks>
  private HtmlHeadAppender()
  {
  }

  /// <summary>
  ///   Gets the <see cref="HtmlHeadAppender"/> instance.
  /// </summary>
  public static HtmlHeadAppender Current
  {
    get 
    { 
      const string contextKey = "Rubicon.Web.UI.HtmlHeadAppender.Current";
      HtmlHeadAppender current = null;
      try
      {
        current = (HtmlHeadAppender) CallContext.GetData (contextKey);
      }
      catch (InvalidCastException)
      {
        //  HACK: Only thrown in design mode. Occasionally.
        return new HtmlHeadAppender();
      }
      
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
  ///   Appends the <c>HTML head elements</c> registered with the <see cref="Current"/>
  ///   <see cref="HtmlHeadAppender"/> to the <paramref name="htmlHeadContents"/>' <b>Controls</b> collection.
  /// </summary>
  /// <remarks>
  ///   Call this method during the rendering of the web form's <c>head element</c>.
  /// </remarks>
  /// <param name="htmlHeadContents">
  ///   <see cref="HtmlHeadContents"/> to whose <b>Controls</b> collection the headers will be appended.
  /// </param>
  public void EnsureAppended (HtmlHeadContents htmlHeadContents)
  {
    if (_hasAppendExecuted)
      return;

    if (ControlHelper.IsDesignMode (htmlHeadContents))
      return;

    IList headElements = _sortedHeadElements.GetValueList();
    for (int i = 0; i < headElements.Count; i++)
    {
      Control headElement = (Control) headElements[i];
      if (! htmlHeadContents.Controls.Contains (headElement))
        htmlHeadContents.Controls.Add (headElement);
    }
  }

  /// <summary>
  ///   Sets the <c>title</c> of the page.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     All calls to <see cref="RegisterStylesheetLink"/> must be completed before
  ///     <see cref="EnsureAppended"/> is called. (Typically during the <c>Render</c> phase.)
  ///   </para><para>
  ///     Remove the title tag from the aspx-source.
  ///   </para><para>
  ///     Registeres the title with a default priority of Page.
  ///   </para>
  /// </remarks>
  /// <param name="title"> The stirng to be isnerted as the title. </param>
  public void SetTitle (string title)
  {
    string key = "title";

    if (IsRegistered (key))
    {
      ((HtmlGenericControl) _registeredHeadElements[key]).InnerText = title;
    }
    else
    {
      HtmlGenericControl headElement = new HtmlGenericControl ("title");
      headElement.EnableViewState = false;
      headElement.InnerText = title;
      RegisterHeadElement ("title", headElement, Prioritiy.Page);
    }
  }

  /// <summary> Registers a stylesheet file. </summary>
  /// <remarks>
  ///   All calls to <see cref="RegisterStylesheetLink"/> must be completed before
  ///   <see cref="EnsureAppended"/> is called. (Typically during the <c>Render</c> phase.)
  /// </remarks>
  /// <param name="key"> The unique key identifying the stylesheet file in the headers collection. </param>
  /// <param name="href"> The url of the stylesheet file. </param>
  /// <param name="priority"> 
  ///   The priority level of the head element. Elements are rendered in the following order:
  ///   Library, UserControl, Page.
  /// </param>
  /// <exception cref="HttpException"> 
  ///   Thrown if method is called after <see cref="EnsureAppended"/> has executed.
  /// </exception>
  public void RegisterStylesheetLink (string key, string href, Prioritiy priority)
  {
    HtmlGenericControl headElement = new HtmlGenericControl ("link");
    headElement.EnableViewState = false;
    headElement.Attributes.Add ("type", "text/css");
    headElement.Attributes.Add ("rel", "stylesheet");
    headElement.Attributes.Add ("href", href);
    RegisterHeadElement (key, headElement, priority);
  }

  /// <summary> Registers a stylesheet file. </summary>
  /// <remarks>
  ///   <para>
  ///     All calls to <see cref="RegisterStylesheetLink"/> must be completed before
  ///     <see cref="EnsureAppended"/> is called. (Typically during the <c>Render</c> phase.)
  ///   </para><para>
  ///     Registeres the javascript file with a default priority of Page.
  ///   </para>
  /// </remarks>
  /// <param name="key"> The unique key identifying the stylesheet file in the headers collection. </param>
  /// <param name="href"> The url of the stylesheet file. </param>
  /// <exception cref="HttpException"> 
  ///   Thrown if method is called after <see cref="EnsureAppended"/> has executed.
  /// </exception>
  public void RegisterStylesheetLink (string key, string href)
  {
    RegisterStylesheetLink (key, href, Prioritiy.Page);
  }

  /// <summary> Registers a javascript file. </summary>
  /// <remarks>
  ///   All calls to <see cref="RegisterJavaScriptInclude"/> must be completed before
  ///   <see cref="EnsureAppended"/> is called. (Typically during the <c>Render</c> phase.)
  /// </remarks>
  /// <param name="key"> The unique key identifying the javascript file in the headers collection. </param>
  /// <param name="src"> The url of the javascript file. </param>
  /// <param name="priority"> 
  ///   The priority level of the head element. Elements are rendered in the following order:
  ///   Library, UserControl, Page.
  /// </param>
  /// <exception cref="HttpException"> 
  ///   Thrown if method is called after <see cref="EnsureAppended"/> has executed.
  /// </exception>
  public void RegisterJavaScriptInclude (string key, string src, Prioritiy priority)
  {
    HtmlGenericControl headElement = new HtmlGenericControl ("script");
    headElement.EnableViewState = false;
    headElement.Attributes.Add ("type", "text/javascript");
    headElement.Attributes.Add ("src", src);
    RegisterHeadElement (key, headElement, priority);
  }

  /// <summary> Registers a javascript file. </summary>
  /// <remarks>
  ///   <para>
  ///     All calls to <see cref="RegisterJavaScriptInclude"/> must be completed before
  ///     <see cref="EnsureAppended"/> is called. (Typically during the <c>Render</c> phase.)
  ///   </para><para>
  ///     Registeres the javascript file with a default priority of Page.
  ///   </para>
  /// </remarks>
  /// <param name="key"> The unique key identifying the javascript file in the headers collection. </param>
  /// <param name="src"> The url of the javascript file. </param>
  /// <exception cref="HttpException"> 
  ///   Thrown if method is called after <see cref="EnsureAppended"/> has executed.
  /// </exception>
  public void RegisterJavaScriptInclude (string key, string src)
  {
    RegisterJavaScriptInclude (key, src, Prioritiy.Page);
  }

  /// <summary> Registers a <see cref="Control"/> containing an HTML head element. </summary>
  /// <remarks>
  ///   All calls to <see cref="RegisterHeadElement"/> must be completed before
  ///   <see cref="EnsureAppended"/> is called. (Typically during the <c>Render</c> phase.)
  /// </remarks>
  /// <param name="key"> The unique key identifying the header element in the collection. </param>
  /// <param name="headElement"> The <see cref="Control"/> representing the head element. </param>
  /// <param name="priority"> 
  ///   The priority level of the head element. Elements are rendered in the following order:
  ///   Library, UserControl, Page.
  /// </param>
  /// <exception cref="HttpException"> 
  ///   Thrown if method is called after <see cref="EnsureAppended"/> has executed.
  /// </exception>
  public void RegisterHeadElement (string key, Control headElement, Prioritiy priority)
  {
    if (_hasAppendExecuted)
      throw new HttpException ("RegisterHeadElement must not be called after EnsureAppended has executed.");
    if (! IsRegistered (key))
    {
      _registeredHeadElements.Add (key, headElement);
      string priorityKey = ((int)priority).ToString() + "." + key;
      _sortedHeadElements.Add (priorityKey, headElement);
    }
  }

  /// <summary>
  ///   Test's whether an element with this <paramref name="key"/> has already been registered.
  /// </summary>
  /// <param name="key"> The string to test. </param>
  /// <returns>
  ///   <see langword="true"/> if an element with this <paramref name="key"/> has already been 
  ///   registered.
  /// </returns>
  public bool IsRegistered (string key)
  {
    return _registeredHeadElements.Contains (key);
  }
}

}
