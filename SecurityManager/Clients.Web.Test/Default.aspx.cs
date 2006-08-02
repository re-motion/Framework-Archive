using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Security;
using System.Security.Principal;

using SecurityManagerUser = Rubicon.SecurityManager.Domain.OrganizationalStructure.User;
using Rubicon.Security.Configuration;
using Rubicon.SecurityManager.Clients.Web.Test.Domain;
using System.Collections.Generic;
using System.Web;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Clients.Web.Test
{
  public partial class _Default : System.Web.UI.Page
  {
    protected void Page_Load (object sender, EventArgs e)
    {
      if (!IsPostBack)
        UsersField.SetBusinessObjectList (SecurityManagerUser.FindByClientID (ObjectID.Parse ("Client|00000001-0000-0000-0000-000000000001|System.Guid"), new ClientTransaction ()));
    }

    protected void EvaluateSecurity_Click (object sender, EventArgs e)
    {
      ISecurityService service = SecurityConfiguration.Current.SecurityService;
      SecurityContext context = new SecurityContext (typeof (File), "1A", "{00000004-1000-0000-0000-000000000007}", "", new Dictionary<string, Enum> (), new Enum[] { DomainAbstractRoles.Creator });
      GenericPrincipal user = new GenericPrincipal (new GenericIdentity ("1A"), new string[0]);
      AccessType[] accessTypes = service.GetAccess (context, user);
    }

    protected void UsersField_SelectionChanged (object sender, EventArgs e)
    {
      if (StringUtility.IsNullOrEmpty (UsersField.BusinessObjectID))
      {
        ((Global) HttpContext.Current.ApplicationInstance).SetUser (null);
      }
      else
      {
        SecurityManagerUser user = SecurityManagerUser.GetObject (ObjectID.Parse (UsersField.BusinessObjectID), new ClientTransaction ());
        ((Global) HttpContext.Current.ApplicationInstance).SetUser (new GenericPrincipal (new GenericIdentity (user.UserName), new string[0]));
      }
    }
  }
}
