using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests.SampleDomain.PermissionReflection
{
  public class SecurableObject : ISecurableObject
  {
    public static void CheckPermissions ()
    {
    }

    [RequiredMethodPermission (GeneralAccessType.Create)]
    public static SecurableObject CreateForSpecialCase ()
    {
      return new SecurableObject ();
    }

    public static bool IsValid ()
    {
      return false;
    }

    [RequiredMethodPermission (GeneralAccessType.Read)]
    public static bool IsValid (SecurableObject securableClass)
    {
      return true;
    }

    [RequiredMethodPermission (GeneralAccessType.Read)]
    public static string GetObjectName (SecurableObject securableObject)
    {
      return null;
    }

    private ISecurityContextFactory _contextFactory;

    public SecurableObject ()
    {
    }

    public SecurableObject (ISecurityContextFactory contextFactory)
    {
      _contextFactory = contextFactory;
    }

    public ISecurityContextFactory GetSecurityContextFactory ()
    {
      return _contextFactory;
    }

    [RequiredMethodPermission (GeneralAccessType.Edit, GeneralAccessType.Create)]
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

    public void Save ()
    {
    }

    public void Delete ()
    {
    }

    [RequiredMethodPermission (GeneralAccessType.Delete)]
    public void Delete (int count)
    {
    }
  }
}
