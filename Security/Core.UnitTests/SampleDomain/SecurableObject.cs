using System;

namespace Rubicon.Security.UnitTests.SampleDomain
{
  public class SecurableObject : ISecurableObject
  {
    public static void CheckPermissions ()
    {
    }

    [DemandMethodPermission (GeneralAccessTypes.Create)]
    public static SecurableObject CreateForSpecialCase ()
    {
      return new SecurableObject ();
    }

    public static bool IsValid ()
    {
      return false;
    }

    [DemandMethodPermission (GeneralAccessTypes.Read)]
    public static bool IsValid (SecurableObject securableClass)
    {
      return true;
    }

    [DemandMethodPermission (GeneralAccessTypes.Read)]
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

    [DemandMethodPermission (GeneralAccessTypes.Edit, GeneralAccessTypes.Create)]
    public void Show ()
    {
    }

    [DemandMethodPermission (GeneralAccessTypes.Edit)]
    public void Record ()
    {
    }

    [DemandMethodPermission (GeneralAccessTypes.Delete)]
    public void Load ()
    {
    }

    [DemandMethodPermission (GeneralAccessTypes.Create)]
    public void Load (string filename)
    {
    }

    [DemandMethodPermission (GeneralAccessTypes.Find)]
    public virtual void Print ()
    {
    }

    [DemandMethodPermission (GeneralAccessTypes.Delete)]
    public void Send ()
    {
    }

    public void Save ()
    {
    }

    public void Delete ()
    {
    }

    [DemandMethodPermission (GeneralAccessTypes.Delete)]
    public void Delete (int count)
    {
    }

    [DemandMethodPermission (GeneralAccessTypes.Edit, GeneralAccessTypes.Find, GeneralAccessTypes.Edit)]
    public void Close ()
    {
    }

    public bool IsEnabled
    {
      get { return true; }
    }

    [DemandPropertyReadPermission (TestAccessTypes.Third)]
    [DemandPropertyWritePermission (TestAccessTypes.Fourth)]
    public bool IsVisible
    {
      get { return true; }
      set { }
    }

    [DemandPropertyReadPermission (TestAccessTypes.First)]
    [DemandPropertyWritePermission (TestAccessTypes.Second)]
    private object NonPublicProperty
    {
      get { return null; }
      set {  }
    }
	
  }
}
