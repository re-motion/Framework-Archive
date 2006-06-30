using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.Web.UI.Globalization;
using Rubicon.SecurityManager.Clients.Web.Classes;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI.AccessControl;
using Rubicon.SecurityManager.Clients.Web.WxeFunctions.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;

namespace Rubicon.SecurityManager.Clients.Web.UI.AccessControl
{
  [WebMultiLingualResources (typeof (SecurableClassDefinitionListFormResources))]
  public partial class SecurableClassDefinitionListForm : BasePage
  {

    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties

    protected new SecurableClassDefinitionListFormFunction CurrentFunction
    {
      get { return (SecurableClassDefinitionListFormFunction) base.CurrentFunction; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      SecurableClassDefinitionList.LoadUnboundValue (SecurableClassDefinition.FindAll (CurrentFunction.CurrentTransaction), false);
    }

    protected void SecurableClassDefinitionList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      if (!IsReturningPostBack)
      {
        EditPermissionsFormFunction function = new EditPermissionsFormFunction (CurrentFunction.ClientID, ((SecurableClassDefinition) e.BusinessObject).ID);
        function.TransactionMode = WxeTransactionMode.None;
        ExecuteFunction (function);
      }
      else
      {
        if (!((EditPermissionsFormFunction) ReturningFunction).HasUserCancelled)
          SecurableClassDefinitionList.LoadUnboundValue (SecurableClassDefinition.FindAll (CurrentFunction.CurrentTransaction), false);
      }
    }
  }
}
