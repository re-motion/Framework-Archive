using System;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Clients.Web.UI.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.SecurityManager.Clients.Web.WxeFunctions.AccessControl
{
  [Serializable]
  public class EditPermissionsFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditPermissionsFormFunction ()
    {
    }

    protected EditPermissionsFormFunction (params object[] args)
      : base (args)
    {
    }

    public EditPermissionsFormFunction (ObjectID securableClassDefinitionObjectID)
      : base (securableClassDefinitionObjectID)
    {
    }

    // methods and properties
    public SecurableClassDefinition SecurableClassDefinition
    {
      get { return (SecurableClassDefinition) CurrentObject; }
      set { CurrentObject = value; }
    }

    WxeResourcePageStep Step1 = new WxeResourcePageStep (typeof (EditPermissionsForm), "UI/AccessControl/EditPermissionsForm.aspx");
  }
}
