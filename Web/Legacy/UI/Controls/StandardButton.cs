using System;
using System.Web.UI.WebControls;

namespace Rubicon.Web.UI.Controls
{
	/// <summary>
	/// StandardButton to be used in web applications. 
	/// Note: The StandardButton does not support client-side validation.
	/// </summary>
	public class StandardButton : Button
	{
		public StandardButton ()
		{
		}

    protected override void OnPreRender (EventArgs e)
    {
      // Disable button client-side to give visual feedback and avoid that user clicks multiple times.
      Attributes["onClick"] = AppendTrailingSemicolonOnDemand (Attributes["onClick"]);
      Attributes["onClick"] += "this.disabled=true; " + Page.GetPostBackClientEvent (this, string.Empty);
      Attributes["onClick"] = AppendTrailingSemicolonOnDemand (Attributes["onClick"]);

      base.OnPreRender (e);
    }

    private string AppendTrailingSemicolonOnDemand (string value)
    {
      if (value != null && value != string.Empty)
      {
        value = value.Trim ();

        if (!value.EndsWith (";"))
          value += ";";
      }

      return value;
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
