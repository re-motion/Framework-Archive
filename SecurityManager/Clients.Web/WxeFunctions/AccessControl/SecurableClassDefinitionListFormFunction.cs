using System;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Clients.Web.UI.AccessControl;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.SecurityManager.Clients.Web.WxeFunctions.AccessControl
{
  [Serializable]
  public class SecurableClassDefinitionListFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SecurableClassDefinitionListFormFunction ()
    {
    }

    // TODO: Make protected once delegation works
    public SecurableClassDefinitionListFormFunction (params object[] args)
      : base (args)
    {
    }

    public SecurableClassDefinitionListFormFunction (ObjectID tenantID)
      : base (tenantID)
    {
    }

    // methods and properties

    private void Step1 ()
    {
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (SecurableClassDefinitionListForm), "UI/AccessControl/SecurableClassDefinitionListForm.aspx");

  }
}
