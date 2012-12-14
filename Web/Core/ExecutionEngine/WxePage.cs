using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.ComponentModel;
using Rubicon.Collections;
using Rubicon.Web.UI;

namespace Rubicon.Web.ExecutionEngine
{
  /// <summary>
///   Base class for pages that can be called by <see cref="WxePageStep"/>.
/// </summary>
/// <remarks>
///   The <see cref="HtmlForm"/> must use the ID "Form". 
///   If you cannot derive your pages from this class (e.g., because you need to derive from another class), you may
///   implement <see cref="IWxePage"/> and override <see cref="DeterminePostBackMode"/> and <see cref="Dispose"/>. 
///   Use <see cref="WxePageInfo"/> to implementat all methods and properties.
/// </remarks>
public class WxePage: Page, IWxePage
{
  private WxePageInfo _wxeInfo;
  private ValidatableControlInitializer _validatableControlInitializer;
  private PostLoadInvoker _postLoadInvoker;

  // protected HtmlForm Form; - won't work in VS 2005

  public WxePage ()
  {
    _wxeInfo = new WxePageInfo (this);
    _validatableControlInitializer = new ValidatableControlInitializer (this);
    _postLoadInvoker = new PostLoadInvoker (this);
  }

  protected override NameValueCollection DeterminePostBackMode()
  {
    NameValueCollection result = _wxeInfo.DeterminePostBackMode (Context);
    _wxeInfo.Initialize (Context);
    OnBeforeInit();
    return result;
  }

  protected virtual void OnBeforeInit ()
  {
  }

  public NameValueCollection GetPostBackCollection ()
  {
    return _wxeInfo.DeterminePostBackMode (Context);
  }

  public IWxePageStep CurrentStep
  {
    get { return (IWxePageStep) _wxeInfo.CurrentStep; }
  }
  
  public WxeFunction CurrentFunction
  {
    get { return _wxeInfo.CurrentFunction; }
  }

  public NameObjectCollection Variables 
  {
    get { return _wxeInfo.Variables; }
  }

  public void ExecuteNextStep ()
  {
    _wxeInfo.ExecuteNextStep();
  }

  public bool IsReturningPostBack
  {
    get { return ((WxeContext.Current == null) ? false : WxeContext.Current.IsReturningPostBack); }
  }

  public void ExecuteFunction (WxeFunction function, string target, Control sender, bool returningPostback)
  {
    _wxeInfo.RestoreOriginalWxeHandler();
    _wxeInfo.ExecuteFunction(function, target, sender, returningPostback);
  }

  public void ExecuteFunction (WxeFunction function)
  {
    _wxeInfo.RestoreOriginalWxeHandler();
    CurrentStep.ExecuteFunction (this, function);
  }

  /// <summary>
  ///   Executes the specified WXE function, then returns to this page without causing the current event again.
  /// </summary>
  /// <remarks>
  ///   This overload tries to determine automatically whether the current event was caused by the __EVENTTARGET field.
  /// </remarks>
  public void ExecuteFunctionNoRepost (WxeFunction function, Control sender)
  {
    ExecuteFunctionNoRepost (function, sender, _wxeInfo.UsesEventTarget);
  }

  /// <summary>
  ///   Executes the specified WXE function, then returns to this page without causing the current event again.
  /// </summary>
  /// <remarks>
  ///   This overload allows you to specify whether the current event was caused by the __EVENTTARGET field.
  ///   When in doubt, use <see cref="ExecuteFunctionNoRepost (IWxePage, WxeFunction, Control)"/>.
  /// </remarks>
  public void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool usesEventTarget)
  {
    CurrentStep.ExecuteFunctionNoRepost (this, function, sender, usesEventTarget);
  }

  public WxeFunction ReturningFunction
  {
    get { return ((WxeContext.Current == null) ? null : WxeContext.Current.ReturningFunction); }
  }

  protected WxeForm WxeForm
  {
    get { return (WxeForm) _wxeInfo.Form; }
  }

  public override void Dispose()
  {
    base.Dispose ();
    _wxeInfo.Dispose();
  }

  [EditorBrowsable (EditorBrowsableState.Never)]
  public virtual HtmlForm HtmlForm
  {
    get { return _wxeInfo.HtmlFormDefaultImplementation; }
    set { _wxeInfo.HtmlFormDefaultImplementation = value; }
  }

  public WxeHandler WxeHandler
  {
    get { return _wxeInfo.WxeHandler; }
  }

  /// <summary>
  ///   Makes sure that PostLoad is called on all controls that support <see cref="ISupportsPostLoadControl"/>
  /// </summary>
  public void EnsurePostLoadInvoked ()
  {
    _postLoadInvoker.EnsurePostLoadInvoked();
  }

  /// <summary>
  ///   Makes sure that all validators are registered with their <see cref="IValidatableControl"/> controls.
  /// </summary>
  public void EnsureValidatableControlsInitialized ()
  {
    _validatableControlInitializer.EnsureValidatableControlsInitialized ();
  }

  /// <summary>
  ///   Call this method before validating when using <see cref="Rubicon.Web.UI.Controls.FormGridManager"/> 
  ///   and <see cref="Rubicon.ObjectBinding.Web.Controls.IBusinessObjectDataSourceControl.Validate"/>.
  /// </summary>
  public void PrepareValidation()
  {
    EnsurePostLoadInvoked();
    EnsureValidatableControlsInitialized();
  }
}

}
