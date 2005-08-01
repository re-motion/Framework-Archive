using System;
using Rubicon.Web.Configuration;

namespace Rubicon.Web.Utilities
{

public class WaiUtility
{
  public static bool IsWaiLevelAConformityRequired (WaiLevel level)
  {
    return (level & WaiLevel.A) == WaiLevel.A;
  }

  public static bool IsWaiLevelAConformityRequired ()
  {
    return IsWaiLevelAConformityRequired (GetWaiLevel());
  }

  public static bool IsWaiLevelAAConformityRequired (WaiLevel level)
  {
    return (level & WaiLevel.AA) == WaiLevel.AA;
  }

  public static bool IsWaiLevelAAConformityRequired ()
  {
    return IsWaiLevelAAConformityRequired (GetWaiLevel());
  }

  public static bool IsWaiLevelAAAConformityRequired (WaiLevel level)
  {
    return (level & WaiLevel.AAA) == WaiLevel.AAA;
  }

  public static bool IsWaiLevelAAAConformityRequired ()
  {
    return IsWaiLevelAAAConformityRequired (GetWaiLevel());
  }

  public static bool IsWaiDebuggingEnabled()
  {
    return WebConfiguration.Current.Wai.Debug;
  }

  public static Rubicon.Web.Configuration.WaiLevel GetWaiLevel()
  {
    return WebConfiguration.Current.Wai.Level;
  }

  /// <exclude />
	private WaiUtility()
	{
	}
}

}
