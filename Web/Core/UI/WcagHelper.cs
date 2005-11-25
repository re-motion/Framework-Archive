using System;
using System.Web.UI;
using log4net;
using Rubicon.Utilities;
using Rubicon.Web.UI;
using Rubicon.Web.Configuration;

namespace Rubicon.Web.UI
{

public class WcagHelper
{
  private static ILog s_log = LogManager.GetLogger (typeof (WcagHelper));
  private static WcagHelper s_instance = new WcagHelper();

  public static WcagHelper Instance
  {
    get { return s_instance; }
  }

  public static void SetInstance (WcagHelper instance)
  {
    ArgumentUtility.CheckNotNull ("instance", instance);
    lock (typeof (WcagHelper))
    {
      s_instance = instance;
    }
  }

	protected WcagHelper()
	{
	}

  public virtual bool IsWaiConformanceLevelARequired (WaiConformanceLevel conformanceLevel)
  {
    return (conformanceLevel & WaiConformanceLevel.A) == WaiConformanceLevel.A;
  }

  public virtual bool IsWaiConformanceLevelARequired ()
  {
    return IsWaiConformanceLevelARequired (GetWaiConformanceLevel());
  }

  public virtual bool IsWaiConformanceLevelDoubleARequired (WaiConformanceLevel conformanceLevel)
  {
    return (conformanceLevel & WaiConformanceLevel.DoubleA) == WaiConformanceLevel.DoubleA;
  }

  public virtual bool IsWaiConformanceLevelDoubleARequired ()
  {
    return IsWaiConformanceLevelDoubleARequired (GetWaiConformanceLevel());
  }

  public virtual bool IsWaiConformanceLevelTripleARequired (WaiConformanceLevel conformanceLevel)
  {
    return (conformanceLevel & WaiConformanceLevel.TripleA) == WaiConformanceLevel.TripleA;
  }

  public virtual bool IsWaiConformanceLevelTripleARequired ()
  {
    return IsWaiConformanceLevelTripleARequired (GetWaiConformanceLevel());
  }

  public virtual bool IsWcagDebuggingEnabled()
  {
    return WebConfiguration.Current.Wcag.Debugging != WcagDebugMode.Disabled;
  }

  public virtual bool IsWcagDebugLogEnabled()
  {
    return WebConfiguration.Current.Wcag.Debugging == WcagDebugMode.Logging
        || WebConfiguration.Current.Wcag.Debugging == WcagDebugMode.Exception;
  }

  public virtual bool IsWcagDebugExceptionEnabled()
  {
    return WebConfiguration.Current.Wcag.Debugging == WcagDebugMode.Exception;
  }

  public virtual WaiConformanceLevel GetWaiConformanceLevel()
  {
    return WebConfiguration.Current.Wcag.ConformanceLevel;
  }

  public virtual void HandleWarning (int priority)
  {
    string message = string.Format (
        "An element on the page might not comply with a priority {0} checkpoint.", priority);
    HandleWarning (message);
  }

  public virtual void HandleWarning (int priority, Control control)
  {
    ArgumentUtility.CheckNotNull ("control", control);

    string message = string.Format (
       "{0} '{1}' might not comply with a priority {2} checkpoint.", 
        control.GetType().Name, control.ID, priority);
    HandleWarning (message);
  }

  public virtual void HandleWarning (int priority, Control control, string property)
  {
    ArgumentUtility.CheckNotNull ("control", control);

    string message = string.Format (
        "The value of property '{0}' for {1} '{2}' might not comply with a priority {3} checkpoint.", 
        property, control.GetType().Name, control.ID, priority);
    HandleWarning (message);
  }

  public virtual void HandleWarning (string message)
  {
    if (IsWcagDebugLogEnabled())
      s_log.Warn (message);
  }

  public virtual void HandleError (int priority)
  {
    string message = string.Format (
        "An element on the page does comply with a priority {0} checkpoint.", priority);
    HandleError (message);
  }

  public virtual void HandleError (int priority, Control control)
  {
    ArgumentUtility.CheckNotNull ("control", control);

    string message = string.Format (
       "{0} '{1}' does not comply with a priority {2} checkpoint.", 
        control.GetType().Name, control.ID, priority);
    HandleError (message);
  }

  public virtual void HandleError (int priority, Control control, string property)
  {
    ArgumentUtility.CheckNotNull ("control", control);

    string message = string.Format (
        "The value of property '{0}' for {1} '{2}' does not comply with a priority {3} checkpoint.", 
        property, control.GetType().Name, control.ID, priority);
    HandleError (message);
  }

  public virtual void HandleError (string message)
  {
    if (IsWcagDebugLogEnabled())
      s_log.Error (message);
    if (IsWcagDebugExceptionEnabled())
      throw new WcagException (message, null);
  }
}

}
