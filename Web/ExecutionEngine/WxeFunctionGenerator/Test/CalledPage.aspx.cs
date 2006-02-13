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
using Rubicon.Web.ExecutionEngine;

namespace Test
{
  [WxePageFunction ("CalledPage.aspx")]
  [WxePageParameter (1, "input", typeof (string))]
  [WxePageParameter (2, "output", typeof (string), WxeParameterDirection.Out, IsReturnValue = true)]
  public partial class CalledPage: WxePage
  {
    protected void Page_Load (object sender, EventArgs e)
    {
    }
  }
}
