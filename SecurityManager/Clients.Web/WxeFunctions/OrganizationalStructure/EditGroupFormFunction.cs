using System;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Rubicon.SecurityManager.Configuration;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Web.ExecutionEngine;

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

    public EditGroupFormFunction (ObjectID organizationalStructureObjectID)
      : base (organizationalStructureObjectID)
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
