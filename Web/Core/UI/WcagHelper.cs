using System;
using System.Web.UI;
using log4net;
using Rubicon.Utilities;
using Rubicon.Web.UI;
using Rubicon.Web.Configuration;

namespace Rubicon.Web.Utilities
{

public class WcagUtility
{
  private static ILog s_log = LogManager.GetLogger (typeof (WcagUtility));

  public static bool IsWaiConformanceLevelARequired (WaiConformanceLevel conformanceLevel)
  {
    return (conformanceLevel & WaiConformanceLevel.A) == WaiConformanceLevel.A;
  }

  public static bool IsWaiConformanceLevelARequired ()
  {
    return IsWaiConformanceLevelARequired (GetWaiConformanceLevel());
  }

  public static bool IsWaiConformanceLevelDoubleARequired (WaiConformanceLevel conformanceLevel)
  {
    return (conformanceLevel & WaiConformanceLevel.DoubleA) == WaiConformanceLevel.DoubleA;
  }

  public static bool IsWaiConformanceLevelDoubleARequired ()
  {
    return IsWaiConformanceLevelDoubleARequired (GetWaiConformanceLevel());
  }

  public static bool IsWaiConformanceLevelTripleARequired (WaiConformanceLevel conformanceLevel)
  {
    return (conformanceLevel & WaiConformanceLevel.TripleA) == WaiConformanceLevel.TripleA;
  }

  public static bool IsWaiConformanceLevelTripleARequired ()
  {
    return IsWaiConformanceLevelTripleARequired (GetWaiConformanceLevel());
  }

  public static bool IsWcagDebuggingEnabled()
  {
    return WebConfiguration.Current.Wcag.Debugging != WcagDebugMode.Disabled;
  }

  public static bool IsWcagDebugLogEnabled()
  {
    return WebConfiguration.Current.Wcag.Debugging == WcagDebugMode.Logging
        || WebConfiguration.Current.Wcag.Debugging == WcagDebugMode.Exception;
  }

  public static bool IsWcagDebugExceptionEnabled()
  {
    return WebConfiguration.Current.Wcag.Debugging == WcagDebugMode.Exception;
  }

  public static WaiConformanceLevel GetWaiConformanceLevel()
  {
    return WebConfiguration.Current.Wcag.ConformanceLevel;
  }

  public static void HandleWarning (int priority)
  {
    string message = string.Format (
        "An element on the page might not comply with a priority {0} checkpoint.", priority);
    HandleWarning (message);
  }

  public static void HandleWarning (int priority, Control control)
  {
    ArgumentUtility.CheckNotNull ("control", control);

    string message = string.Format (
       "{0} '{1}' might not comply with a priority {2} checkpoint.", 
        control.GetType().Name, control.ID, priority);
    HandleWarning (message);
  }

  public static void HandleWarning (int priority, Control control, string property)
  {
    ArgumentUtility.CheckNotNull ("control", control);

    string message = string.Format (
        "The value of property '{0}' for {1} '{2}' might not comply with a priority {3} checkpoint.", 
        property, control.GetType().Name, control.ID, priority);
    HandleWarning (message);
  }

  public static void HandleWarning (string message)
  {
    if (IsWcagDebugLogEnabled())
      s_log.Warn (message);
  }

  public static void HandleError (int priority)
  {
    string message = string.Format (
        "An element on the page does comply with a priority {0} checkpoint.", priority);
    HandleError (message);
  }

  public static void HandleError (int priority, Control control)
  {
    ArgumentUtility.CheckNotNull ("control", control);

    string message = string.Format (
       "{0} '{1}' does not comply with a priority {2} checkpoint.", 
        control.GetType().Name, control.ID, priority);
    HandleError (message);
  }

  public static void HandleError (int priority, Control control, string property)
  {
    ArgumentUtility.CheckNotNull ("control", control);

    string message = string.Format (
        "The value of property '{0}' for {1} '{2}' does not comply with a priority {3} checkpoint.", 
        property, control.GetType().Name, control.ID, priority);
    HandleError (message);
  }

  public static void HandleError (string message)
  {
    if (IsWcagDebugLogEnabled())
      s_log.Error (message);
    if (IsWcagDebugExceptionEnabled())
      throw new WcagException (message, null);
  }

  /// <exclude />
	private WcagUtility()
	{
	}
}

}
