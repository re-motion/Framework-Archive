using System;
using System.Reflection;
using log4net.Core;
using Rubicon.Utilities;

namespace Rubicon.Logging
{
  public class Log4NetLogManager : IExtendedLogManager
  {
    public IExtendedLog GetLogger (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return new Log4NetLog (LoggerManager.GetLogger (Assembly.GetCallingAssembly (), name));
    }

    public IExtendedLog GetLogger (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return new Log4NetLog (LoggerManager.GetLogger (Assembly.GetCallingAssembly (), type));
    }
  }
}