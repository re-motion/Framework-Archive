using System;

namespace Rubicon.Logging
{
  public static class LogManager
  {
    private static IExtendedLogManager s_logManager = new Log4NetLogManager ();

    public static IExtendedLog GetLogger (string name)
    {
      return s_logManager.GetLogger (name);
    }

    public static IExtendedLog GetLogger (Type type)
    {
      return s_logManager.GetLogger (type);
    }
  }
}