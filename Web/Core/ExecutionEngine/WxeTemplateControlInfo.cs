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
  private WxePageStep _currentStep;
  private WxeFunction _currentFunction;
  private IWxeTemplateControl _control;
  /// <summary> Caches the <see cref="ResourceManagerSet"/> for this control. </summary>
  private ResourceManagerSet _cachedResourceManager;

  [Obsolete ("Use Initialize instead.")]
  public void OnInit (IWxeTemplateControl control, HttpContext context)
  {
    Initialize (control, context);
  }

  public void Initialize (IWxeTemplateControl control, HttpContext context)
  {
    ArgumentUtility.CheckNotNullAndType ("control", control, typeof (Control));
    _control = control;
    if (ControlHelper.IsDesignMode (control, context))
      return;

    WxeHandler wxeHandler = context.Handler as WxeHandler;
    if (wxeHandler == null)
      throw new HttpException ("No current WxeHandler found.");

    _currentStep = wxeHandler.RootFunction.ExecutingStep as WxePageStep;
    _currentFunction = WxeStep.GetFunction (_currentStep);
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
    IResourceManager namingContainerResourceManager = 
        ResourceManagerUtility.GetResourceManager (_control.NamingContainer, true);

    if (namingContainerResourceManager == null)
      _cachedResourceManager = new ResourceManagerSet (localResourceManager);
    else
      _cachedResourceManager = new ResourceManagerSet (localResourceManager, namingContainerResourceManager);

    return _cachedResourceManager;
  }
}

}
