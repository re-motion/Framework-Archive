using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.Data.DomainObjects;
using Rubicon.Web.ExecutionEngine;
using Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.SecurityManager.Configuration;

namespace Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  public class EditUserFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditUserFormFunction ()
    {
    }

    protected EditUserFormFunction (params object[] args)
      : base (args)
    {
    }

    public EditUserFormFunction (ObjectID clientID, ObjectID organizationalStructureObjectID)
      : base (clientID, organizationalStructureObjectID)
    {
    }

    // methods and properties
    public User User
    {
      get { return (User) CurrentObject; }
      set { CurrentObject = value; }
    }

    private void Step1 ()
    {
      // TODO check CurrentTransaction
      if (CurrentObject == null)
      {
        User = SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreateUser (CurrentTransaction);
        User.Client = Rubicon.SecurityManager.Domain.OrganizationalStructure.Client.GetObject (ClientID, CurrentTransaction);
      }
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditUserForm), "UI/OrganizationalStructure/EditUserForm.aspx");
  }
}
