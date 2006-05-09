using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Security;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain
{
  public class SecurableSearchObject : BindableSearchObject, ISecurableType
  {
    private string _stringProperty;
    private ISecurityContextFactory _securityContextFactory;

    public SecurableSearchObject (ISecurityContextFactory securityContextFactory)
    {
      _securityContextFactory = securityContextFactory;
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

    public ISecurityContextFactory GetSecurityContextFactory ()
    {
      return _securityContextFactory;
    }
  }
}
