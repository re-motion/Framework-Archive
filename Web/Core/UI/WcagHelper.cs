using System;
using Rubicon.Web.Configuration;

namespace Rubicon.Web.Utilities
{

public class WaiUtility
{
  public static bool IsWaiLevelAConformanceRequired (WaiConformanceLevel conformanceLevel)
  {
    return (conformanceLevel & WaiConformanceLevel.A) == WaiConformanceLevel.A;
  }

  public static bool IsWaiLevelAConformanceRequired ()
  {
    return IsWaiLevelAConformanceRequired (GetWaiConformanceLevel());
  }

  public static bool IsWaiLevelDoubleAConformanceRequired (WaiConformanceLevel conformanceLevel)
  {
    return (conformanceLevel & WaiConformanceLevel.DoubleA) == WaiConformanceLevel.DoubleA;
  }

  public static bool IsWaiLevelDoubleAConformanceRequired ()
  {
    return IsWaiLevelDoubleAConformanceRequired (GetWaiConformanceLevel());
  }

  public static bool IsWaiLevelTripleAConformanceRequired (WaiConformanceLevel conformanceLevel)
  {
    return (conformanceLevel & WaiConformanceLevel.TripleA) == WaiConformanceLevel.TripleA;
  }

  public static bool IsWaiLevelTripleAConformanceRequired ()
  {
    return IsWaiLevelTripleAConformanceRequired (GetWaiConformanceLevel());
  }

  public static bool IsWaiDebuggingEnabled()
  {
    return WebConfiguration.Current.Wai.Debug;
  }

  public static Rubicon.Web.Configuration.WaiConformanceLevel GetWaiConformanceLevel()
  {
    return WebConfiguration.Current.Wai.ConformanceLevel;
  }

  /// <exclude />
	private WaiUtility()
	{
	}
}

}
