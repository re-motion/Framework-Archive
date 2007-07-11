using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.Mixins;

namespace WebApplicationSample
{
  public class RenderOverwriterMixin : Mixin<object>
  {
    [Override]
    public void RenderControl (HtmlTextWriter writer)
    {
      writer.WriteLine ("<div>This control has been hijacked by a mixin. Don't try anything silly. Virtual property says '"
        + VirtualProperty + "'.</div>");
    }

    protected virtual string VirtualProperty
    {
      get { return "nothing"; }
    }
  }
}
