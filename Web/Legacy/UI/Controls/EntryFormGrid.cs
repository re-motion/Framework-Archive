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
[ControlBuilder (typeof (EntryFormGridControlBuilder))]
public class EntryFormGrid: Control
{
	protected override void Render (HtmlTextWriter writer)
	{
		writer.WriteLine ("<table border=\"0\" cellspacing=\"\0\" cellpadding=\"0\">");

		for (int i = 0; i < this.Controls.Count; ++i)
		{
			Control control = this.Controls[i];

			// write vertical empty space before titles
			if (i != 0 && control is EntryTitle)
				writer.WriteLine ("<tr><td> <img src=\"../Images/ws.gif\" height=\"10\" width=\"1\"></td></tr>");

			control.RenderControl(writer);
		}

		writer.WriteLine ("</table>");
	}
	
}

public class EntryFormGridControlBuilder: ControlBuilder
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

		if (unqualifiedTag == "EntryField")
			return typeof (EntryField);
		if (unqualifiedTag == "EntryTitle")
			return typeof (EntryTitle);

    throw new ApplicationException ("Only EntryField and EntryTitle tags are allowed in EntryFormGrid controls");
	}
}

public class EntryTitle: Control
{
	private string _title = String.Empty;
  private string _padding = String.Empty;
	public string Title 
	{
		get { return _title; }
		set { _title = value; }
	}
	public string Padding
	{
		get { return _padding; }
		set { _padding = value; }
	}
	protected override void Render (HtmlTextWriter writer)
	{
    if (Padding != String.Empty)
    {
      writer.WriteLine ("<tr><td><img src=\"../Images/ws.gif\" width=\"1\" height=\"{0}\"/></td></tr>",
          Padding);
    }

		writer.WriteLine ("<tr><td class=\"formGroup\" colspan=\"6\"> {0} </td></tr>", this.Title);
		writer.WriteLine ("<tr><td bgcolor=\"black\" colspan=\"6\"> "
				+ "<img src=\"../Images/ws.gif\" height=\"2\" width=\"1\"></td></tr>");
		writer.WriteLine ("<tr> <td><img height=\"3\" width=\"1\" src=\"../Images/ws.gif\"/></td> </tr>");
	}
}

[ParseChildren (false)]
public class EntryField: Control
{
	private string _label = String.Empty;
	private string _for = String.Empty;
	private string _infoUrl = String.Empty;
	private bool _isRequired = false;

	public string Label 
	{
		get { return _label; }
		set { _label = value; }
	}
	public string For
	{
		get { return _for; }
		set { _for = value; }
	}
	public string InfoUrl
	{
		get { return _infoUrl; }
		set { _infoUrl = value; }
	}
	public bool IsRequired
	{
		get { return _isRequired; }
		set { _isRequired = value; }
	}

	protected override void Render (HtmlTextWriter writer)
	{
		string label;
    string clientId = String.Empty;
    Control labeledControl;
    string validatorMessages = String.Empty ;
    bool validatorsInvalid = false;

    // search for Validator child controls and keep if at least one is invalid
    foreach ( Control childControl in this.Controls )
    {
      if ( childControl is BaseValidator )
      {
        BaseValidator validator = (BaseValidator) childControl;
        if ( !validator.IsValid )
        {
          // Validator is invalid => save status and its error message
          validatorsInvalid = true;
          if ( validatorMessages.Length > 0 )
            validatorMessages += "\r\n";
          validatorMessages += validator.ErrorMessage;
        }
      }
    }

    if (For == String.Empty)
      labeledControl = this.Controls[1];
    else
      labeledControl = this.FindControl (For);

    if (labeledControl != null)
    {
      // WORKAROUND: prevent the following bug in IE 6.0: clicking on the label of a <select> control
      // selects the first item in the list (but does not fire a changed-event)
      if (! (   labeledControl is System.Web.UI.WebControls.DropDownList 
             || labeledControl is System.Web.UI.HtmlControls.HtmlSelect))
        clientId = labeledControl.ClientID;
    }

		if (clientId == String.Empty)
			label = Label;
		else
			label = "<label for=\"" + clientId + "\">" +  Label + "</label>";

		writer.WriteLine ("<tr>");
		writer.WriteLine ("<td class=\"label\" valign=\"center\" align=\"right\">&nbsp;{0}</td>", label);
		writer.WriteLine ("<td class=\"label\"><img height=\"1\" width=\"7\" src=\"../Images/ws.gif\"/></td>");
		writer.WriteLine ("<td><img height=\"1\" width=\"3\" src=\"../Images/ws.gif\"/></td>");
		writer.WriteLine ("<td nowrap>");

		if (this.InfoUrl == String.Empty && ! this.IsRequired && ! validatorsInvalid)
		{
			writer.WriteLine ("<img src=\"../Images/ws.gif\" width=\"27\" height=\"20\"/>");
		}
		else
		{
			if (this.IsRequired)
			{
				writer.WriteLine ("<img src=\"../Images/field-required.gif\" alt=\"Dieses Feld muss ausgef&uuml;llt sein\" "
							+ "width=\"12\" height=\"20\" border=\"0\"/>");
			}
			else
			{
				writer.WriteLine ("<img src=\"../Images/ws.gif\" width=\"12\" height=\"20\"/>");
			}

			if (this.InfoUrl != String.Empty)
			{
				writer.WriteLine (
						"<a href=\"{0}\" target=\"_new\">"
							+ "<img src=\"../Images/field-info.gif\" alt=\"Hilfe zum Ausfüllen per Mausklick\""
							+ "width=\"15\" height=\"20\" border=\"0\"/></a>",
						this.InfoUrl);
      }
			else
			{
				writer.WriteLine ("<img src=\"../Images/ws.gif\" width=\"12\" height=\"20\"/>");
			}

      if (validatorsInvalid)
      {
        writer.WriteLine ("<img src=\"../Images/field-error.gif\" alt=\"" + validatorMessages + "\""
          + "width=\"12\" height=\"20\" border=\"0\"/>");
      }
      else
      {
        writer.WriteLine ("<img src=\"../Images/ws.gif\" width=\"12\" height=\"20\"/>");
      }
		}

		writer.WriteLine ("</td><td>");

    RenderChildren(writer);

		writer.WriteLine ("</td>");
		writer.WriteLine ("</tr>");
		writer.WriteLine ("<tr> <td><img height=\"1\" width=\"1\" src=\"../Images/ws.gif\"/></td> </tr>");
	}
	
}

}