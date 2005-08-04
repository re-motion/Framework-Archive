using System;
using Rubicon.Web.Configuration;

namespace Rubicon.Web.Utilities
{

public class WcagUtility
{
  public static bool IsWcagLevelAConformanceRequired (WcagConformanceLevel conformanceLevel)
  {
    return (conformanceLevel & WcagConformanceLevel.A) == WcagConformanceLevel.A;
  }

  public static bool IsWcagLevelAConformanceRequired ()
  {
    return IsWcagLevelAConformanceRequired (GetWcagConformanceLevel());
  }

  public static bool IsWcagLevelDoubleAConformanceRequired (WcagConformanceLevel conformanceLevel)
  {
    return (conformanceLevel & WcagConformanceLevel.DoubleA) == WcagConformanceLevel.DoubleA;
  }

  public static bool IsWcagLevelDoubleAConformanceRequired ()
  {
    return IsWcagLevelDoubleAConformanceRequired (GetWcagConformanceLevel());
  }

  public static bool IsWcagLevelTripleAConformanceRequired (WcagConformanceLevel conformanceLevel)
  {
    return (conformanceLevel & WcagConformanceLevel.TripleA) == WcagConformanceLevel.TripleA;
  }

  public static bool IsWcagLevelTripleAConformanceRequired ()
  {
    return IsWcagLevelTripleAConformanceRequired (GetWcagConformanceLevel());
  }

  public static bool IsWcagDebuggingEnabled()
  {
    return WebConfiguration.Current.Wcag.Debug;
  }

  public static Rubicon.Web.Configuration.WcagConformanceLevel GetWcagConformanceLevel()
  {
    return WebConfiguration.Current.Wcag.ConformanceLevel;
  }

  /// <exclude />
	private WcagUtility()
	{
	}
}

}
