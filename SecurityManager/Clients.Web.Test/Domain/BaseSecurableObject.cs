using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Rubicon.Utilities;
using Rubicon.Security;
using Rubicon.Data.DomainObjects;
using System.Collections.Generic;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon;

namespace Rubicon.SecurityManager.Clients.Web.Test.Domain
{
  [PermanentGuid ("C9FC9EC0-9F41-4636-9A4C-4927A9B47E85")]
  public abstract class BaseSecurableObject : BaseObject, ISecurableObject, ISecurityContextFactory
  {
    // types

    // static members

    // member fields

    private IObjectSecurityStrategy _objectSecurityStrategy;

    // construction and disposing

    protected BaseSecurableObject (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected BaseSecurableObject (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      if (_objectSecurityStrategy == null)
        _objectSecurityStrategy = new ObjectSecurityStrategy (this);

      return _objectSecurityStrategy;
    }

    public SecurityContext CreateSecurityContext ()
    {
      return new SecurityContext (GetType (), GetOwnerName (), GetOwnerGroupName (), GetOwnerClientName (), GetStates (), GetAbstractRoles ());
    }

    private string GetOwnerClientName ()
    {
      Client client = GetOwnerClient ();
      if (client == null)
        return null;
      return client.Name;
    }

    private string GetOwnerGroupName ()
    {
      Group group = GetOwnerGroup ();
      if (group == null)
        return null;
      return group.Name;
    }

    private string GetOwnerName ()
    {
      User user = GetOwner ();
      if (user == null)
        return null;
      return user.UserName;
    }

    public abstract User GetOwner ();

    public abstract Group GetOwnerGroup ();

    public abstract Client GetOwnerClient ();

    public virtual IDictionary<string, Enum> GetStates ()
    {
      return new Dictionary<string, Enum> ();
    }

    public virtual ICollection<Enum> GetAbstractRoles ()
    {
      return new Enum[0];
    }
  }
}