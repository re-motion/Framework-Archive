using System;
using System.Diagnostics;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Globalization;

namespace Rubicon.Findit.Client.Controls
{

[ParseChildren (false, "Controls")]
[ControlBuilder (typeof (ViewControlBuilder))]
public class ViewControl: Control
{
  private string _title = string.Empty;
  private Unit _labelColumnWidth;
  private Unit _valueColumnWidth;

  public string Title
  {
    get { return _title; }
    set { _title = value; }
  }

  public Unit LabelColumnWidth 
  {
    get { return _labelColumnWidth; }
    set { _labelColumnWidth = value; }
  }
	
  public Unit ValueColumnWidth 
  {
    get { return _valueColumnWidth; }
    set { _valueColumnWidth = value; }
  }

	/// <summary> 
	/// Render this control to the output parameter specified.
	/// </summary>
	/// <param name="output"> The HTML writer to write out to </param>
	protected override void Render(HtmlTextWriter writer)
	{
		
    writer.Write ("<table cellpadding=\"2\" cellspacing=\"3\"");
    
    if (LabelColumnWidth.Value != 0 && ValueColumnWidth.Value != 0)
    {
      if (LabelColumnWidth.Type != ValueColumnWidth.Type)
      {
        throw new InvalidOperationException (
            "Cannot specify LabelColumnWidth and ValueColumnWidth in different units.");
      }
      double widthValue = LabelColumnWidth.Value + ValueColumnWidth.Value;
      if (LabelColumnWidth.Type == UnitType.Em)
        widthValue *= .8; // WORKAROUND: EMs in TD are only 80% of EMs in TABLE
      Unit tableWidth = new Unit (widthValue, LabelColumnWidth.Type);
      writer.Write ("style=\"width: " + tableWidth.ToString(CultureInfo.InvariantCulture) + ";\"");
    }

    writer.WriteLine (">");
    
    for (int i = 0; i < this.Controls.Count; ++i)
		{
			Control control = this.Controls[i];
      
      if (control.GetType() == typeof(EntryTitle))
      {
          writer.WriteLine("<tr><td colspan=\"2\"><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\">");
          control.RenderControl (writer);
          writer.WriteLine ("</table></td></tr>");
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
  private string _value = string.Empty;
  
  
  public string Title
  {
    get { return _title; }
    set { _title = value; }
  }

  public string Value
  {
    get { return _value; }
    set { _value = value; }
  }

  protected override void Render (HtmlTextWriter writer)
	{
    
    string labelColumnWidth = string.Empty;
    string valueColumnWidth = string.Empty;
    
    ViewControl viewControl = this.Parent as ViewControl;
    if (viewControl != null)
    { 
      labelColumnWidth = viewControl.LabelColumnWidth.ToString();
      valueColumnWidth = viewControl.ValueColumnWidth.ToString();
    }

    writer.Write ("<tr><td class=\"labelView\" valign=\"middle\" align=\"right\" ");

    if (labelColumnWidth.Length != 0)
      writer.Write ("style=\"width: " + labelColumnWidth + ";\"");

    writer.WriteLine (">");
      
    writer.Write (Title);
    
    writer.Write ("<img height=\"1\" width=\"7\" src=\"../Images/ws.gif\">");
  
    writer.Write ("</td><td class=\"text\" valign=\"middle\" align=\"left\" ");

    if (valueColumnWidth.Length != 0)
      writer.Write ("style=\"width: " + valueColumnWidth + ";\"");

    writer.WriteLine (">");

    writer.WriteLine (Value);

    writer.WriteLine ("</td></tr>"); 
    
    RenderChildren(writer);
  }
}
}

