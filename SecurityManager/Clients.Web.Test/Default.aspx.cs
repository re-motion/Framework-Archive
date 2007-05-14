using System;
using System.Collections.Generic;
using System.Security.Principal;
using Rubicon.Data.DomainObjects;
using Rubicon.Security;
using Rubicon.Security.Configuration;
using Rubicon.SecurityManager.Clients.Web.Test.Domain;
using Rubicon.Utilities;
using SecurityManagerUser = Rubicon.SecurityManager.Domain.OrganizationalStructure.User;

namespace Rubicon.SecurityManager.Clients.Web.Test
{
  public partial class _Default : System.Web.UI.Page
  {
    protected Global ApplicationInstance
    {
      get { return (Global) Context.ApplicationInstance; }
    }
    
    protected void Page_Load (object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
        using (new SecurityFreeSection())
        {
          ClientTransaction clientTransaction = new ClientTransaction();
          DomainObjectCollection users =
              SecurityManagerUser.FindByClientID (ObjectID.Parse ("Client|00000001-0000-0000-0000-000000000001|System.Guid"), clientTransaction);
          users.Combine (
              SecurityManagerUser.FindByClientID (ObjectID.Parse ("Client|00000001-0000-0000-0000-000000000002|System.Guid"), clientTransaction));

          UsersField.SetBusinessObjectList (users);
          UsersField.LoadUnboundValue (ApplicationInstance.LoadUserFromSession (clientTransaction), false);
        }
      }
    }

    protected void EvaluateSecurity_Click (object sender, EventArgs e)
    {
      ISecurityProvider provider = SecurityConfiguration.Current.SecurityProvider;
      SecurityContext context =
          new SecurityContext (
              typeof (File),
              "1A",
              "{00000004-1000-0000-0000-000000000007}",
              "",
              new Dictionary<string, Enum>(),
              new Enum[] {DomainAbstractRoles.Creator});
      GenericPrincipal user = new GenericPrincipal (new GenericIdentity ("1A"), new string[0]);
      AccessType[] accessTypes = provider.GetAccess (context, user);
    }

    protected void UsersField_SelectionChanged (object sender, EventArgs e)
    {
      if (StringUtility.IsNullOrEmpty (UsersField.BusinessObjectID))
        ApplicationInstance.SetCurrentUser (null, true);
      else
        ApplicationInstance.SetCurrentUser (SecurityManagerUser.GetObject (ObjectID.Parse (UsersField.BusinessObjectID), new ClientTransaction ()), true);
    }
  }
}