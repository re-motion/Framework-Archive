using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Security;

namespace Rubicon.Security.Data.DomainObjects.UnitTests.TestDomain
{
  public class SecurableObject : DomainObject, ISecurableObject, ISecurityContextFactory
  {
    private IObjectSecurityStrategy _securityStrategy;

    public SecurableObject (ClientTransaction clientTransaction, IObjectSecurityStrategy securityStrategy)
      : base (clientTransaction)
    {
      _securityStrategy = securityStrategy;
    }

    protected SecurableObject (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    protected override void OnLoaded ()
    {
      base.OnLoaded ();
      _securityStrategy = new ObjectSecurityStrategy (this);
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      return _securityStrategy;
    }

    public DataContainer GetDataContainer ()
    {
      return DataContainer;
    }

    public string StringProperty
    {
      get { return (string) DataContainer["StringProperty"]; }
      set { DataContainer["StringProperty"] = value; }
    }

    public string OtherStringProperty
    {
      get { return (string) DataContainer["OtherStringProperty"]; }
      set { DataContainer["OtherStringProperty"] = value; }
    }

    public SecurableObject Parent
    {
      get { return (SecurableObject) GetRelatedObject ("Parent"); }
      set { SetRelatedObject ("Parent", value); }
    }

    public DomainObjectCollection Children
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("Children"); }
    }

    public SecurableObject OtherParent
    {
      get { return (SecurableObject) GetRelatedObject ("OtherParent"); }
      set { SetRelatedObject ("OtherParent", value); }
    }

    public DomainObjectCollection OtherChildren
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("OtherChildren"); }
    }

    SecurityContext ISecurityContextFactory.CreateSecurityContext ()
    {
      return new SecurityContext (typeof (SecurableObject));
    }
  }
}
