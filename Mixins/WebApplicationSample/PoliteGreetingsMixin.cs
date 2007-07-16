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
  public class PoliteGreetingsMixin : Mixin<object, PoliteGreetingsMixin.IGreeter>
  {
    public interface IGreeter
    {
      string GetGreetings ();
    }

    [Override]
    public virtual string GetGreetings ()
    {
      return "This is the mixin - base text: " + Base.GetGreetings ();
    }
  }
}
