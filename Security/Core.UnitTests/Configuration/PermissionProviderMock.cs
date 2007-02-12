using System;
using System.Configuration.Provider;
using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.Configuration
{
  public class PermissionProviderMock : ProviderBase, IPermissionProvider
  {
    public Enum[] GetRequiredMethodPermissions (Type type, string methodName)
    {
      throw new NotImplementedException();
    }

    public Enum[] GetRequiredStaticMethodPermissions (Type type, string methodName)
    {
      throw new NotImplementedException();
    }

    public Enum[] GetRequiredPropertyReadPermissions (Type type, string methodName)
    {
      throw new NotImplementedException();
    }

    public Enum[] GetRequiredPropertyWritePermissions (Type type, string methodName)
    {
      throw new NotImplementedException();
    }
  }
}