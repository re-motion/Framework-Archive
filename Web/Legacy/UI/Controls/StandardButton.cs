using System;
using System.Web.UI.WebControls;

namespace Rubicon.Web.UI.Controls
{
	/// <summary>
	/// StandardButton to be used in web applications.
	/// </summary>
	public class StandardButton : Button
	{
		public StandardButton ()
		{
		}

    protected override void OnPreRender (EventArgs e)
    {
      // Disable button client-side to give visual feedback and avoid that user clicks multiple times.
      string onClickJavascript = 
        "this.disabled=true; " + Page.GetPostBackClientEvent (this, string.Empty);

      Attributes.Add ("onClick", onClickJavascript);

      base.OnPreRender (e);
    }

    protected StandardPage StandardPage
    {
      get 
      {
        return Page as StandardPage;
      }
    }
 	}
}
