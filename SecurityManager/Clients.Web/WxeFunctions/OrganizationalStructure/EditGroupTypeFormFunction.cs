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

namespace Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  public class EditGroupTypeFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditGroupTypeFormFunction ()
    {
    }

    protected EditGroupTypeFormFunction (params object[] args)
      : base (args)
    {
    }

    public EditGroupTypeFormFunction (ObjectID clientID, ObjectID organizationalStructureObjectID)
      : base (clientID, organizationalStructureObjectID)
    {
    }

    // methods and properties
    public GroupType GroupType
    {
      get { return (GroupType) CurrentObject; }
      set { CurrentObject = value; }
    }

    private void Step1 ()
    {
      // TODO check CurrentTransaction
      if (CurrentObject == null)
      {
        GroupType = new GroupType (CurrentTransaction);
        GroupType.Client = Rubicon.SecurityManager.Domain.OrganizationalStructure.Client.GetObject (ClientID, CurrentTransaction);
      }
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditGroupTypeForm), "UI/OrganizationalStructure/EditGroupTypeForm.aspx");
  }
}
