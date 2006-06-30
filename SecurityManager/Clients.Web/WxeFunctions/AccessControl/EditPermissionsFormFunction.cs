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
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.SecurityManager.Configuration;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.Clients.Web.WxeFunctions.AccessControl
{
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

    public EditPermissionsFormFunction (ObjectID clientID, ObjectID securableClassDefinitionObjectID)
      : base (clientID, securableClassDefinitionObjectID)
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
