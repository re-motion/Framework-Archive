using System;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Rubicon.SecurityManager.Configuration;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  [Serializable]
  public class EditTenantFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditTenantFormFunction ()
    {
    }

    protected EditTenantFormFunction (params object[] args)
      : base (args)
    {
    }

    public EditTenantFormFunction (ObjectID organizationalStructureObjectID)
      : base (organizationalStructureObjectID)
    {
    }

    // methods and properties
    public Tenant Tenant
    {
      get { return (Tenant) CurrentObject; }
      set { CurrentObject = value; }
    }

    private void Step1 ()
    {
      // TODO check CurrentTransaction
      if (CurrentObject == null)
      {
        Tenant = SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreateTenant ();
      }
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditTenantForm), "UI/OrganizationalStructure/EditTenantForm.aspx");
  }
}
