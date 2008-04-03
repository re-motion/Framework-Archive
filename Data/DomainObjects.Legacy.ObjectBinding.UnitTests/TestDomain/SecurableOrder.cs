using System;
using Remotion.Security;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain
{
  public class SecurableOrder: Order, ISecurableObject
  {
    // types

    // static members

    // member fields

    private IObjectSecurityStrategy _securityStrategy;
    private string _displayName;

    // construction and disposing

    public SecurableOrder (IObjectSecurityStrategy securityStrategy)
    {
      _securityStrategy = securityStrategy;
    }

    // methods and properties

    public override string DisplayName
    {
      get { return _displayName; }
    }

    public void SetDisplayName (string displayName)
    {
      _displayName = displayName;
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      return _securityStrategy;
    }

    public Type GetSecurableType ()
    {
      return GetPublicDomainObjectType ();
    }
  }
}