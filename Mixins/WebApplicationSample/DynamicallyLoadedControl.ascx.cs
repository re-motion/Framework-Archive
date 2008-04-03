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
using Remotion.Mixins;

namespace WebApplicationSample
{
  public partial class DynamicallyLoadedControl : System.Web.UI.UserControl
  {
    protected void Page_Load (object sender, EventArgs e)
    {
    }

    protected override void Render (HtmlTextWriter writer)
    {
      writer.WriteLine ("<div>This is control text.</div>");
    }

    [Override]
    public virtual string VirtualProperty
    {
      get { return "Help me!"; }
    }
  }
}