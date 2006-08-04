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
  // <WxePageFunction pageType="$PROJECT_ROOTNAMESPACE$.UI.Edit$DOMAIN_CLASSNAME$Form" aspxFile="UI/Edit$DOMAIN_CLASSNAME$Form.aspx" functionName="Edit$DOMAIN_CLASSNAME$Function"
  //      functionBaseType="Rubicon.Data.DomainObjects.Web.ExecutionEngine.WxeTransactedFunction">
  //   <Parameter name="p_id" type="String" required="false" />
  //   <Variable name="v_$DOMAIN_CLASSNAME$" type="$DOMAIN_ROOTNAMESPACE$.$DOMAIN_CLASSNAME$" />
  // </WxePageFunction>
  public partial class Edit$DOMAIN_CLASSNAME$Form : EditFormPage
  {
    private bool _isSaved;

    protected void Page_Load (object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
        if (p_id != null)
          v_$DOMAIN_CLASSNAME$ = $DOMAIN_CLASSNAME$.GetObject(ObjectID.Parse (p_id));
        else
          v_$DOMAIN_CLASSNAME$ = new $DOMAIN_CLASSNAME$();
      }

      LoadObject (v_$DOMAIN_CLASSNAME$);
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