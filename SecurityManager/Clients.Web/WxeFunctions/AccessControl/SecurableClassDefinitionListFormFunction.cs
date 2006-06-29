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
using Rubicon.SecurityManager.Clients.Web.UI.AccessControl;

namespace Rubicon.SecurityManager.Clients.Web.WxeFunctions.AccessControl
{
  public class SecurableClassDefinitionListFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SecurableClassDefinitionListFormFunction ()
    {
    }

    protected SecurableClassDefinitionListFormFunction (params object[] args)
      : base (args)
    {
    }

    public SecurableClassDefinitionListFormFunction (ObjectID clientID)
      : base (clientID)
    {
    }

    // methods and properties

    private void Step1 ()
    {
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (SecurableClassDefinitionListForm), "UI/AccessControl/SecurableClassDefinitionListForm.aspx");

  }
}
