/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Web.UI;
using Remotion.Logging;
using Remotion.Utilities;
using Remotion.Web.Configuration;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI
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
    if (ControlHelper.IsDesignMode (control))
      return;

    string message = string.Format (
       "{0} '{1}' on page '{2}' might not comply with a priority {3} checkpoint.", 
        control.GetType().Name, control.ID, control.Page.GetType().FullName, priority);
    HandleWarning (message);
  }

  public virtual void HandleWarning (int priority, Control control, string property)
  {
    ArgumentUtility.CheckNotNull ("control", control);
    if (ControlHelper.IsDesignMode (control))
      return;

    string message = string.Format (
        "The value of property '{0}' for {1} '{2}' on page '{3}' might not comply with a priority {4} checkpoint.", 
        property, control.GetType().Name, control.ID, control.Page.GetType().FullName, priority);
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
    if (ControlHelper.IsDesignMode (control))
      return;

    string message = string.Format (
       "{0} '{1}' on page '{2}' does not comply with a priority {3} checkpoint.", 
        control.GetType().Name, control.ID, control.Page.GetType().FullName, priority);
    HandleError (message);
  }

  public virtual void HandleError (int priority, Control control, string property)
  {
    ArgumentUtility.CheckNotNull ("control", control);
    if (ControlHelper.IsDesignMode (control))
      return;

    string message = string.Format (
        "The value of property '{0}' for {1} '{2}' on page '{3}' does not comply with a priority {4} checkpoint.", 
        property, control.GetType().Name, control.ID, control.Page.GetType().FullName, priority);
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
