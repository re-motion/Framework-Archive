using System;
using System.Web;
using System.Web.UI;
using Rubicon.Globalization;
using Rubicon.Utilities;
using Rubicon.Web.UI.Globalization;
using Rubicon.Collections;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

public class WxeTemplateControlInfo
{
  private WxeHandler _wxeHandler;
  private WxePageStep _currentStep;
  private WxeFunction _currentFunction;
  private IWxeTemplateControl _control;
  /// <summary> Caches the <see cref="ResourceManagerSet"/> for this control. </summary>
  private ResourceManagerSet _cachedResourceManager;

  public WxeTemplateControlInfo (IWxeTemplateControl control)
  {
    ArgumentUtility.CheckNotNullAndType ("control", control, typeof (TemplateControl));
    _control = control;
  }

  public virtual void Initialize (HttpContext context)
  {
    if (ControlHelper.IsDesignMode (_control, context))
      return;
    ArgumentUtility.CheckNotNull ("context", context);

    if (_control is Page)
    {
      _wxeHandler = context.Handler as WxeHandler;
    }
    else
    {
      IWxePage wxePage = _control.Page as IWxePage;
      if (wxePage == null)
        throw new InvalidOperationException (string.Format ("'{0}' can only be added to a Page implementing the IWxePage interface.", _control.GetType().FullName));
      _wxeHandler = wxePage.WxeHandler;
    }
    if (_wxeHandler == null)
    {
      throw new HttpException (string.Format ("No current WxeHandler found. Most likely cause of the exception: "
          + "The page '{0}' has been called directly instead of using a WXE Handler to invoke the associated WXE Function.", 
          _control.Page.GetType()));
    }

    _currentStep = _wxeHandler.RootFunction.ExecutingStep as WxePageStep;
    _currentFunction = WxeStep.GetFunction (_currentStep);
  }

  public WxeHandler WxeHandler
  {
    get { return _wxeHandler; }
  }

  public WxePageStep CurrentStep
  {
    get { return _currentStep; }
  }
  
  public WxeFunction CurrentFunction
  {
    get { return _currentFunction; }
  }

  public NameObjectCollection Variables 
  {
    get { return (_currentStep == null) ? null : _currentStep.Variables; }
  }

  /// <summary> Find the <see cref="IResourceManager"/> for this control info. </summary>
  /// <param name="localResourcesType"> 
  ///   A type with the <see cref="MultiLingualResourcesAttribute"/> applied to it.
  ///   Typically an <b>enum</b> or the derived class itself.
  /// </param>
  protected IResourceManager GetResourceManager (Type localResourcesType)
  {
    Rubicon.Utilities.ArgumentUtility.CheckNotNull ("localResourcesType", localResourcesType);

    //  Provider has already been identified.
    if (_cachedResourceManager != null)
        return _cachedResourceManager;

    //  Get the resource managers

    IResourceManager localResourceManager = 
        MultiLingualResourcesAttribute.GetResourceManager (localResourcesType, true);
    Control namingContainer = _control.NamingContainer;
    if (namingContainer == null)
      namingContainer = (Control) _control;
    IResourceManager namingContainerResourceManager = 
        ResourceManagerUtility.GetResourceManager (namingContainer, true);

    if (namingContainerResourceManager == null)
      _cachedResourceManager = new ResourceManagerSet (localResourceManager);
    else
      _cachedResourceManager = new ResourceManagerSet (localResourceManager, namingContainerResourceManager);

    return _cachedResourceManager;
  }
}

}
