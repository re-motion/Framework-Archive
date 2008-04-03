using System;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Security;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain
{
  public class SecurableSearchObject : BindableSearchObject, ISecurableObject
  {
    private string _stringProperty;
    private IObjectSecurityStrategy _securityStrategy;

    public SecurableSearchObject (IObjectSecurityStrategy securityStrategy)
    {
      _securityStrategy = securityStrategy;
    }

    public string StringProperty
    {
      get { return _stringProperty; }
      set { _stringProperty = value; }
    }

    public override IQuery CreateQuery ()
    {
      throw new NotImplementedException ();
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      return _securityStrategy;
    }

    public Type GetSecurableType ()
    {
      return GetType ();
    }
  }
}
