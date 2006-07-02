using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.Configuration
{
  public class PermissionProviderMock : IPermissionProvider
  {
    public Enum[] GetRequiredMethodPermissions (Type type, string methodName)
    {
      throw new Exception ("The method or operation is not implemented.");
    }

    public Enum[] GetRequiredStaticMethodPermissions (Type type, string methodName)
    {
      throw new Exception ("The method or operation is not implemented.");
    }

    public Enum[] GetRequiredPropertyReadPermissions (Type type, string methodName)
    {
      throw new Exception ("The method or operation is not implemented.");
    }

    public Enum[] GetRequiredPropertyWritePermissions (Type type, string methodName)
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}
