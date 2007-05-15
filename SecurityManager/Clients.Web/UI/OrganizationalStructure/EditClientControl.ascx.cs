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
using Rubicon.SecurityManager.Clients.Web.Classes;
using Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Web.UI.Controls;

namespace Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (EditClientControlResources))]
  public partial class EditClientControl : BaseControl
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties
    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected new EditClientFormFunction CurrentFunction
    {
      get { return (EditClientFormFunction) base.CurrentFunction; }
    }

    public override IFocusableControl InitialFocusControl
    {
      get { return NameField; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      FillParentField ();

      if (ChildrenList.IsReadOnly)
        ChildrenList.Selection = RowSelection.Disabled;
    }

    private void FillParentField ()
    {
      ParentField.SetBusinessObjectList (CurrentFunction.Client.GetPossibleParentClients (CurrentFunction.ClientID));
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();

      return isValid;
    }
  }
}