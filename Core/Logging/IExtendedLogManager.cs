using System;

namespace Rubicon.Logging
{
  public interface IExtendedLogManager
  {
    IExtendedLog GetLogger (string name);

    IExtendedLog GetLogger (Type type);
  }
}