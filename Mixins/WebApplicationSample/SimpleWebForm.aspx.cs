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
using Rubicon.Mixins;

namespace WebApplicationSample
{
  [Uses (typeof (PoliteGreetingsMixin))]
  public partial class SimpleWebForm : System.Web.UI.Page
  {
    protected void Page_Load (object sender, EventArgs e)
    {
      LoadControl (typeof (DynamicallyLoadedControl));
      using (MixinConfiguration.ScopedExtend (typeof (DynamicallyLoadedControl), typeof (RenderOverwriterMixin)))
      {
        LoadControl (typeof (DynamicallyLoadedControl));
      }
    }

    private void LoadControl (Type type)
    {
      Type realType = type;
      if (MixinConfiguration.ActiveContext.ContainsClassContext (type))
        realType = TypeFactory.GetConcreteType (type);

      Control loadedControl = LoadControl (realType, new object[0]);
      placeholder.Controls.Add (loadedControl);
    }

    public virtual string GetGreetings ()
    {
      return "Hi from SimpleWebForm!";
    }
  }
}
