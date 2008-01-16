using System;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI.Globalization;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UI.Controls;
using $PROJECT_ROOTNAMESPACE$;
using $PROJECT_ROOTNAMESPACE$.Classes;
using $DOMAIN_ROOTNAMESPACE$;

namespace $PROJECT_ROOTNAMESPACE$.UI
{
  // <WxePageFunction>
  //   <Parameter name="obj" type="$DOMAIN_CLASSNAME$" />
  // </WxePageFunction>
  public partial class Edit$DOMAIN_CLASSNAME$Form : EditFormPage
  {
    protected void Page_Load (object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
        if (obj == null)
          obj = $DOMAIN_CLASSNAME$.NewObject();
      }

      LoadObject (obj);
    }

    protected override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected override TabbedMultiView UserControlMultiView
    {
      get { return MultiView; }
    }

    protected void SaveButton_Click (object sender, EventArgs e)
    {
      if (SaveObject())
        Return();
    }

    protected void CancelButton_Click (object sender, EventArgs e)
    {
      throw new WxeUserCancelException ();
    }
  }
}