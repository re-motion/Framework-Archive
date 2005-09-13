using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization;
using Rubicon.Utilities;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

/// <summary> This step interrupts the server side execution to display a page to the user. </summary>
/// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Class/*' />
[Serializable]
public class WxePageStep: WxeStep
{
  private string _page = null;
  private string _pageref = null;
  private string _pageRoot;
  private string _pageToken;
  private WxeFunction _function;
  private NameValueCollection _postBackCollection;
  private string _viewState;

  /// <summary> Initializes a new instance of the <b>WxePageStep</b> type. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Ctor/param[@name="page"]' />
  public WxePageStep (string page)
    : this (null, page)
  {
  }

  /// <summary> Initializes a new instance of the <b>WxePageStep</b> type. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Ctor/param[@name="page" or @name="resourceAssembly"]' />
  protected WxePageStep (Assembly resourceAssembly, string page)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("page", page);

    _page = page;
    Initialize (resourceAssembly);
  }

  /// <summary> Initializes a new instance of the <b>WxePageStep</b> type. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Ctor/param[@name="pageref"]' />
  public WxePageStep (WxeVariableReference pageref)
    : this (null, pageref)
  {
  }

  /// <summary> Initializes a new instance of the <b>WxePageStep</b> type. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Ctor/param[@name="pageref" or @name="resourceAssembly"]' />
  protected WxePageStep (Assembly resourceAssembly, WxeVariableReference pageref)
  {
    ArgumentUtility.CheckNotNull ("pageref", pageref);

    _pageref = pageref.Name;
    Initialize (resourceAssembly);
  }

  /// <summary> Common initalization code for the <see cref="WxePageStep"/>. </summary>
  /// <param name="resourceAssembly"> The (optional) <see cref="Assembly"/> containing the page. </param>
  private void Initialize (Assembly resourceAssembly)
  {
    _pageToken = Guid.NewGuid().ToString();
    _function = null;

    if (resourceAssembly == null)
      _pageRoot = string.Empty;
    else
      _pageRoot = ResourceUrlResolver.GetAssemblyRoot (false, resourceAssembly);
  }

  /// <summary> The URL of the page to be displayed by this <see cref="WxePageStep"/>. </summary>
  protected string Page
  { 
    get
    {
      string name;
      if (_page != null)
        name = _page;
      else if (_pageref != null && Variables[_pageref] != null)
        name = (string) Variables[_pageref];
      else
        throw new WxeException ("No Page specified for " + this.GetType().FullName + ".");
      return _pageRoot + name;
    }
  }

  /// <summary> 
  ///   Displays the <see cref="WxePageStep"/>'s page or the sub-function that has been invoked by the 
  ///   <see cref="ExecuteFunction"/> method.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Execute/*' />
  public override void Execute (WxeContext context)
  {
    if (_function == null)
    {
      //  This is the PageStep if it isn't executing a sub-function
      
      //  Use the Page's postback data
      context.PostBackCollection = null;
      context.IsReturningPostBack = false;
    }
    else
    {
      //  This is the PageStep currently executing a sub-function
      
      _function.Execute (context);
      
      //  This is the PageStep after the sub-function has completed execution
      
      //  Provide the executed sub-function to the executing page
      context.ReturningFunction = _function;
      _function = null;
      context.IsPostBack = true;
      //  Provide the backed up postback data to the executing page
      context.PostBackCollection = _postBackCollection;
      _postBackCollection = null;
      context.IsReturningPostBack = true;
    }

    try 
    {
      context.HttpContext.Server.Transfer (Page, context.IsPostBack);
    }
    catch (HttpException e)
    {
      if (e.InnerException is WxeExecuteNextStepException)
        return;
      if (e.InnerException is HttpUnhandledException && e.InnerException.InnerException is WxeExecuteNextStepException)
        return;
      throw;
    }
  }

  /// <summary> Gets the currently executing <see cref="WxeStep"/>. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/ExecutingStep/*' />
  public override WxeStep ExecutingStep
  {
    get
    {
      if (_function != null)
        return _function.ExecutingStep;
      else
        return this;
    }
  }

  /// <summary> Executes the specified <see cref="WxeFunction"/>, then returns to this page. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/ExecuteFunction/*' />
  public void ExecuteFunction (IWxePage page, WxeFunction function)
  {
    //  Back-up postback data of the executing page
    _postBackCollection = new NameValueCollection (page.GetPostBackCollection());
    
    InternalExecuteFunction (page, function);
  }

  /// <summary>
  ///   Executes the specified <see cref="WxeFunction"/>, then returns to this page without raising the 
  ///   postback event after the user returns.
  /// </summary>
  /// <remarks> Invoke this method by calling <see cref="WxePageInfo.ExecuteFunctionNoRepost"/>. </remarks>
  internal void ExecuteFunctionNoRepost (IWxePage page, WxeFunction function, Control sender, bool usesEventTarget)
  {
    //  Back-up post back data of the executing page
    _postBackCollection = new NameValueCollection (page.GetPostBackCollection());

    //  Remove post back event source from the post back data
    if (usesEventTarget)
    {
      _postBackCollection.Remove (ControlHelper.PostEventSourceID);
      _postBackCollection.Remove (ControlHelper.PostEventArgumentID );
    }
    else
    {
      ArgumentUtility.CheckNotNull ("sender", sender);
      _postBackCollection.Remove (sender.UniqueID);
    }

    InternalExecuteFunction (page, function);
  }

  /// <summary> Executes the specified <see cref="WxeFunction"/>, then returns to this page. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/InternalExecuteFunction/*' />
  private void InternalExecuteFunction (IWxePage page, WxeFunction function)
  {
    if (_function != null)
      throw new InvalidOperationException ("Cannot execute function while another function executes.");

    _function = function; 
    _function.SetParentStep (this);    

    // page.SaveVieState()
    MethodInfo saveViewStateMethod = typeof (Page).GetMethod ("SavePageViewState", BindingFlags.Instance | BindingFlags.NonPublic);
    saveViewStateMethod.Invoke (page, new object[0]); 

    Execute();
  }

  /// <summary> Gets the token for this page step. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/PageToken/*' />
  public string PageToken
  {
    get { return _pageToken; }
  }

  /// <summary> Returns the string identifying the <b>ASP.NET</b> page used for this <see cref="WxePageStep"/>. </summary>
  /// <returns> The value of <see cref="Page"/>. </returns>
  public override string ToString()
  {
    return Page;
  }

  /// <summary> Saves the passed <paramref name="viewState"/> object into the <see cref="WxePageStep"/>. </summary>
  /// <param name="viewState"> An <b>ASP.NET</b> viewstate object. </param>
  public void SavePageStateToPersistenceMedium (object viewState)
  {
    LosFormatter formatter = new LosFormatter ();
    StringWriter writer = new StringWriter ();
    formatter.Serialize (writer, viewState);
    _viewState = writer.ToString();
  }

  /// <summary> 
  ///   Returns the viewstate previsously saved through the <see cref="SavePageStateToPersistenceMedium"/> method. 
  /// </summary>
  /// <returns> An <b>ASP.NET</b> viewstate object. </returns>
  public object LoadPageStateFromPersistenceMedium()
  {
    LosFormatter formatter = new LosFormatter ();
    return formatter.Deserialize (_viewState);
  }
  
  /// <summary> 
  ///   Aborts the <see cref="WxePageStep"/>. Aborting will cascade to any <see cref="WxeFunction"/> executed
  ///   in the scope of this step.
  /// </summary>
  protected override void AbortRecursive()
  {
    base.AbortRecursive ();
    if (_function != null)
      _function.Abort();
  }
}

}
