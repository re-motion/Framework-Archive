using System;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  [Serializable]
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

    public EditGroupTypeFormFunction (ObjectID organizationalStructureObjectID)
      : base (organizationalStructureObjectID)
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
        GroupType = GroupType.NewObject ();
      }
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditGroupTypeForm), "UI/OrganizationalStructure/EditGroupTypeForm.aspx");
  }
}
