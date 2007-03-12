using System;
using System.Collections.Specialized;
using Rubicon.Configuration;
using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.Configuration
{
  public class PermissionProviderMock : ExtendedProviderBase, IPermissionProvider
  {
    public PermissionProviderMock (string name, NameValueCollection config)
        : base (name, config)
    {
    }

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