using System;
using System.Diagnostics;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

namespace Rubicon.Findit.Client.Controls
{

[ParseChildren (false, "Controls")]
[ControlBuilder (typeof (ViewControlBuilder))]
public class ViewControl: Control
{
  private string _title = string.Empty;

  public string Title
  {
    get { return _title; }
    set { _title = value; }
  }

	/// <summary> 
	/// Render this control to the output parameter specified.
	/// </summary>
	/// <param name="output"> The HTML writer to write out to </param>
	protected override void Render(HtmlTextWriter writer)
	{
		
    writer.WriteLine ("<table cellpadding=\"2\" cellspacing=\"3\">");
    
		
    for (int i = 0; i < this.Controls.Count; ++i)
		{
			Control control = this.Controls[i];
      
      if (control.GetType() == typeof(EntryTitle))
      {
          writer.WriteLine("<tr><td colspan=\"2\"><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\">"
              + "<tr><td>");
          control.RenderControl (writer);
          writer.WriteLine ("</td></tr></table></td></tr>");
      }
      else
      {
        control.RenderControl (writer);
      }
		}
    
    

    writer.WriteLine ("</table>");
	}
}
  
public class ViewControlBuilder: ControlBuilder
{
	public override bool AllowWhitespaceLiterals ()
	{
		return false;
	}
	
	public override Type GetChildControlType (string tag, IDictionary attribs)
	{
		string unqualifiedTag = tag;
		int posColon = tag.IndexOf (':');
		if (posColon >= 0 && posColon < tag.Length + 1)
			unqualifiedTag = tag.Substring (posColon + 1);

		if (unqualifiedTag == "ViewField")
			return typeof (ViewField);
		if (unqualifiedTag == "EntryTitle")
			return typeof (EntryTitle);

    throw new ApplicationException ("Only ViewField and EntryTitle tags are allowed in ViewControls");
	}
}

[ParseChildren (false)]
public class ViewField: Control
{
  
  private string _title = string.Empty;
  private string _text = string.Empty;

  public string Title
  {
    get { return _title; }
    set { _title = value; }
  }

  public string Text
  {
    get { return _text; }
    set { _text = value; }
  }

  protected override void Render (HtmlTextWriter writer)
	{
    
    writer.WriteLine ("<tr><td class=\"labelView\" valign=\"center\" align=\"right\">");
      
    writer.Write (Title);
    
    writer.Write ("<img height=\"1\" width=\"7\" src=\"../Images/ws.gif\"");
  
    writer.WriteLine ("</td><td class=\"text\" valign=\"top\" align=\"left\">");

    writer.WriteLine (Text);

    writer.WriteLine ("</td></tr>"); 
    
    RenderChildren(writer);
  }
}
}

