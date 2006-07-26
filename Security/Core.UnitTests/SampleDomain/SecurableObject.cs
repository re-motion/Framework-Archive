using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests.SampleDomain
{
  public class SecurableObject : ISecurableObject
  {
    public static void CheckPermissions ()
    {
    }

    [DemandMethodPermission (GeneralAccessType.Create)]
    public static SecurableObject CreateForSpecialCase ()
    {
      return new SecurableObject ();
    }

    public static bool IsValid ()
    {
      return false;
    }

    [DemandMethodPermission (GeneralAccessType.Read)]
    public static bool IsValid (SecurableObject securableClass)
    {
      return true;
    }

    [DemandMethodPermission (GeneralAccessType.Read)]
    public static string GetObjectName (SecurableObject securableObject)
    {
      return null;
    }

    private IObjectSecurityStrategy _securityStrategy;

    public SecurableObject ()
    {
    }

    public SecurableObject (IObjectSecurityStrategy objectSecurityStrategy)
    {
      _securityStrategy = objectSecurityStrategy;
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      return _securityStrategy;
    }

    [DemandMethodPermission (GeneralAccessType.Edit, GeneralAccessType.Create)]
    public void Show ()
    {
    }

    [DemandMethodPermission (GeneralAccessType.Edit)]
    public void Record ()
    {
    }

    [DemandMethodPermission (GeneralAccessType.Delete)]
    public void Load ()
    {
    }

    [DemandMethodPermission (GeneralAccessType.Create)]
    public void Load (string filename)
    {
    }

    [DemandMethodPermission (GeneralAccessType.Find)]
    public virtual void Print ()
    {
    }

    [DemandMethodPermission (GeneralAccessType.Delete)]
    public void Send ()
    {
    }

    public void Save ()
    {
    }

    public void Delete ()
    {
    }

    [DemandMethodPermission (GeneralAccessType.Delete)]
    public void Delete (int count)
    {
    }

    [DemandMethodPermission (GeneralAccessType.Edit, GeneralAccessType.Find, GeneralAccessType.Edit)]
    public void Close ()
    {
    }

    public bool IsEnabled
    {
      get { return true; }
    }

    [DemandPropertyReadPermission (GeneralAccessType.Create)]
    [DemandPropertyWritePermission (GeneralAccessType.Find)]
    public bool IsVisible
    {
      get { return true; }
    }
  }
}
