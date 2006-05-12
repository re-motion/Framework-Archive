using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests.SampleDomain.PermissionReflection
{
  public class SecurableClass : ISecurableType
  {
    public static void CheckPermissions ()
    {
    }

    [RequiredMethodPermission (GeneralAccessType.Create)]
    public static SecurableClass CreateForSpecialCase ()
    {
      return new SecurableClass ();
    }

    public static bool IsValid ()
    {
      return false;
    }

    [RequiredMethodPermission (GeneralAccessType.Read)]
    public static bool IsValid (SecurableClass securableClass)
    {
      return true;
    }

    private ISecurityContextFactory _contextFactory;

    public SecurableClass ()
    {
    }

    public SecurableClass (ISecurityContextFactory contextFactory)
    {
      _contextFactory = contextFactory;
    }

    public ISecurityContextFactory GetSecurityContextFactory ()
    {
      return _contextFactory;
    }

    [RequiredMethodPermission (GeneralAccessType.Edit)]
    [RequiredMethodPermission (GeneralAccessType.Create)]
    public void Show ()
    {
    }

    [RequiredMethodPermission (GeneralAccessType.Edit)]
    public void Record ()
    {
    }

    [RequiredMethodPermission (GeneralAccessType.Delete)]
    public void Load ()
    {
    }

    [RequiredMethodPermission (GeneralAccessType.Create)]
    public void Load (string filename)
    {
    }

    [RequiredMethodPermission (GeneralAccessType.Find)]
    public virtual void Print ()
    {
    }

    [RequiredMethodPermission (GeneralAccessType.Delete)]
    public void Send ()
    {
    }

    [RequiredMethodPermission (GeneralAccessType.Create)]
    [RequiredMethodPermission (GeneralAccessType.Create)]
    public void Create ()
    {
    }

    public void Save ()
    {
    }
  }
}
