using System;

namespace Rubicon.Logging
{
  public interface IExtendedLogFactory
  {
    IExtendedLog CreateLogger (string name);

    IExtendedLog CreateLogger (Type type);
  }
}