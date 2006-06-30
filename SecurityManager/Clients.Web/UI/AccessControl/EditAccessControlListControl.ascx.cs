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
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Web.UI.Controls;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI.AccessControl;

namespace Rubicon.SecurityManager.Clients.Web.UI.AccessControl
{
  [WebMultiLingualResources (typeof (EditAccessControlListControlResources))]
  public partial class EditAccessControlListControl : BaseControl
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

    public override IFocusableControl InitialFocusControl
    {
      get { return null; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);
    }
  }
}