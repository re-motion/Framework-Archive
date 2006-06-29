using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.SecurityManager.Clients.Web.Classes;
using Rubicon.Data.DomainObjects;
using Rubicon.Web.ExecutionEngine;
using Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.SecurityManager.Configuration;

namespace Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  public class EditGroupFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditGroupFormFunction ()
    {
    }

    protected EditGroupFormFunction (params object[] args)
      : base (args)
    {
    }

    public EditGroupFormFunction (ObjectID clientID, ObjectID organizationalStructureObjectID)
      : base (clientID, organizationalStructureObjectID)
    {
    }

    // methods and properties
    public Group Group
    {
      get { return (Group) CurrentObject; }
      set { CurrentObject = value; }
    }

    private void Step1 ()
    {
      // TODO check CurrentTransaction
      if (CurrentObject == null)
      {
        Group = SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreateGroup (CurrentTransaction);
        Group.Client = Rubicon.SecurityManager.Domain.OrganizationalStructure.Client.GetObject (ClientID, CurrentTransaction);
      }
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditGroupForm), "UI/OrganizationalStructure/EditGroupForm.aspx");
  }
}
