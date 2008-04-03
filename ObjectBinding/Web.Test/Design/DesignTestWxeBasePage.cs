using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Collections;
using System.Threading;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.Utilities;
using Remotion.Utilities;
using Remotion.Globalization;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Globalization;
using Remotion.Web.UI.Controls;
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
