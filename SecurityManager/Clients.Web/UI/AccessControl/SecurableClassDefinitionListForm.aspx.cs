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
using Rubicon.Web.UI.Controls;
using Rubicon.Data.DomainObjects;

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

      LoadTree (IsPostBack, false);
    }

    private void LoadTree (bool interim, bool refreshTreeNodes)
    {
      SecurableClassDefinitionTree.LoadUnboundValue (SecurableClassDefinition.FindAllBaseClasses (new ClientTransaction ()), interim);
      if (refreshTreeNodes)
        SecurableClassDefinitionTree.RefreshTreeNodes ();
      ExpandTreeNodes (SecurableClassDefinitionTree.Nodes);
    }

    private void ExpandTreeNodes (WebTreeNodeCollection webTreeNodeCollection)
    {
      foreach (WebTreeNode treeNode in webTreeNodeCollection)
      {
        treeNode.EvaluateExpand ();
        ExpandTreeNodes (treeNode.Children);
      }
    }

    protected void SecurableClassDefinitionTree_Click (object sender, BocTreeNodeClickEventArgs e)
    {
      if (!IsReturningPostBack)
      {
        SecurableClassDefinition classDefinition = (SecurableClassDefinition) e.BusinessObjectTreeNode.BusinessObject;
        EditPermissionsFormFunction function = new EditPermissionsFormFunction (CurrentFunction.ClientID, classDefinition.ID);
        string features = "width=1000, height=700, resizable=yes, menubar=no, toolbar=no, location=no, status=no";
        ExecuteFunctionExternal (function, "_blank", features, (Control) sender, true);
      }
      else
      {
        LoadTree (false, true);
      }
    }
  }
}
