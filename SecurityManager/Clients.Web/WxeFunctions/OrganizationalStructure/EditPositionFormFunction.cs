using System;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Rubicon.SecurityManager.Configuration;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  public class EditPositionFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditPositionFormFunction ()
    {
    }

    protected EditPositionFormFunction (params object[] args)
      : base (args)
    {
    }

    public EditPositionFormFunction (ObjectID organizationalStructureObjectID)
      : base (organizationalStructureObjectID)
    {
    }

    // methods and properties
    public Position Position
    {
      get { return (Position) CurrentObject; }
      set { CurrentObject = value; }
    }

    private void Step1 ()
    {
      // TODO check CurrentTransaction
      if (CurrentObject == null)
      {
        Position = SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreatePosition (CurrentTransaction);
      }
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditPositionForm), "UI/OrganizationalStructure/EditPositionForm.aspx");
  }
}
