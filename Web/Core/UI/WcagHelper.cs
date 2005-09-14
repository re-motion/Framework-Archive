using System;
using Rubicon.Web.Configuration;

namespace Rubicon.Web.Utilities
{

public class WcagUtility
{
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
    return WebConfiguration.Current.Wcag.Debug;
  }

  public static WaiConformanceLevel GetWaiConformanceLevel()
  {
    return WebConfiguration.Current.Wcag.ConformanceLevel;
  }

  /// <exclude />
	private WcagUtility()
	{
	}
}

}
