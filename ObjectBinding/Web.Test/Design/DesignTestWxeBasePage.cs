using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Collections;
using System.Threading;
using Rubicon.Web;
using Rubicon.Web.UI;
using Rubicon.Web.Utilities;
using Rubicon.Utilities;
using Rubicon.Globalization;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.UI.Controls;
using System.Web;

using OBWTest;

namespace OBWTest.Design
{

public class DesignTestWxeBasePage: TestWxeBasePage
{
  protected override bool IsAbortEnabled
  {
    get { return false; }
  }


  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);
    HtmlHeadAppender.Current.RegisterStylesheetLink ("design", "Html/Design.css");
  }

}

}
