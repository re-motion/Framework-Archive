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

namespace OBWTest
{

[MultiLingualResources ("OBWTest.Globalization.SingleBocTestBasePage")]
public class SingleBocTestWxeBasePage: TestWxeBasePage
{
  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
    this.EnableAbort = true;
    this.ShowAbortConfirmation = Remotion.Web.UI.ShowAbortConfirmation.OnlyIfDirty;
  }

}

}
