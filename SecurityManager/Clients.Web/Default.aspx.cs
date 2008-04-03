using System;
using System.Web.UI;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;
using SecurityManagerUser = Remotion.SecurityManager.Domain.OrganizationalStructure.User;

namespace Remotion.SecurityManager.Clients.Web
{
  public partial class DefaultPage : Page
  {
    private ClientTransaction _clientTransaction;

    protected Global ApplicationInstance
    {
      get { return (Global) Context.ApplicationInstance; }
    }

    protected override void OnLoad (EventArgs e)
    {
      _clientTransaction = ClientTransaction.NewRootTransaction ();
      _clientTransaction.EnterDiscardingScope ();
      if (!IsPostBack)
      {
        using (new SecurityFreeSection ())
        {
          DomainObjectCollection users =
              SecurityManagerUser.FindByTenantID (ObjectID.Parse ("Tenant|00000001-0000-0000-0000-000000000000|System.Guid"));
          users.Combine (SecurityManagerUser.FindByTenantID (ObjectID.Parse ("Tenant|00000001-0000-0000-0000-000000000001|System.Guid")));
          users.Combine (SecurityManagerUser.FindByTenantID (ObjectID.Parse ("Tenant|00000001-0000-0000-0000-000000000002|System.Guid")));

          UsersField.SetBusinessObjectList (users);
          UsersField.LoadUnboundValue (ApplicationInstance.LoadUserFromSession (), false);
        }
      }
    }

    protected void UsersField_SelectionChanged (object sender, EventArgs e)
    {
      if (StringUtility.IsNullOrEmpty (UsersField.BusinessObjectID))
        ApplicationInstance.SetCurrentUser (null, true);
      else
        ApplicationInstance.SetCurrentUser (SecurityManagerUser.GetObject (ObjectID.Parse (UsersField.BusinessObjectID)), true);
    }

    protected override void OnPreRender (EventArgs e)
    {
      _clientTransaction.EnterDiscardingScope ();
      base.OnPreRender (e);
    }

    protected override void OnUnload (EventArgs e)
    {
      base.OnUnload (e);
      ClientTransactionScope.ResetActiveScope ();
    }

  }
}
