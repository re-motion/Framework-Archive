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
  public class EditClientFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditClientFormFunction ()
    {
    }

    protected EditClientFormFunction (params object[] args)
      : base (args)
    {
    }

    public EditClientFormFunction (ObjectID organizationalStructureObjectID)
      : base (organizationalStructureObjectID)
    {
    }

    // methods and properties
    public Client Client
    {
      get { return (Client) CurrentObject; }
      set { CurrentObject = value; }
    }

    private void Step1 ()
    {
      // TODO check CurrentTransaction
      if (CurrentObject == null)
      {
        Client = SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreateClient (CurrentTransaction);
      }
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditClientForm), "UI/OrganizationalStructure/EditClientForm.aspx");
  }
}
