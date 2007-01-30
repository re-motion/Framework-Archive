using System;

namespace Rubicon.Logging
{
  public static class LogFactory
  {
    // TODO: Get from config section
    private static IExtendedLogFactory s_current = new Log4NetLogFactory ();

    public static IExtendedLog CreateLogger (string name)
    {
      return s_current.CreateLogger (name);
    }

    public static IExtendedLog CreateLogger (Type type)
    {
      return s_current.CreateLogger (type);
    }
  }
}