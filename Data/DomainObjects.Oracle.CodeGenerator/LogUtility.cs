using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Oracle.CodeGenerator
{
  public static class LogUtility
  {
    // types

    // static members and constants

    private static ILog s_logger;

    // construction and disposing

    // static methods and properties

    private static ILog Logger
    {
      get
      {
        lock (typeof (LogUtility))
        {
          if (s_logger == null)
          {
            log4net.Config.XmlConfigurator.Configure ();
            s_logger = LogManager.GetLogger ("Rubicon.Data.DomainObjects.Oracle.CodeGenerator");
          }
        }

        return s_logger;
      }
    }

    public static void LogError (string message, Exception exception)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("message", message);
      ArgumentUtility.CheckNotNull ("exception", exception);

      Logger.Error (message, exception);
    }

    public static void LogWarning (string message)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("message", message);

      Logger.Warn (message);
    }

    public static void LogWarning (string message, params object[] args)
    {
      LogWarning (string.Format (message, args));
    }

    public static void Shutdown ()
    {
      s_logger = null;
      LogManager.Shutdown ();
    }
  }
}
