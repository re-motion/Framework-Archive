using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using Rubicon.Web.UI.Design;

namespace Rubicon.Web.UI.Controls
{

[ToolboxItemFilter("System.Web.UI")]
public class SmartLabel: WebControl
{
  public static string FormatLabelText (string rawString, bool underlineAccessKey)
  {
    string accessKey;
    return FormatLabelText (rawString, underlineAccessKey, out accessKey);
  }

  /// <summary>
  ///   Formats the string to support an access key:
  ///   Looks for an ampersand and optionally highlights the next letter with &lt;u&gt;&lt;/u&gt;.
  /// </summary>
  public static string FormatLabelText (string rawString, bool underlineAccessKey, out string accessKey)
  {
    //  TODO: HTMLEncode

    const string highlighter = "<u>{0}</u>";
    accessKey = String.Empty;

    if (! underlineAccessKey)
    {
      //  Remove ampersands
      return rawString.Replace("&", "");
    }

    //  Access key is preceeded by an ampersand
    int indexOfAmp = rawString.IndexOf ("&");
    
    //  No ampersand and therfor access key found
    if (indexOfAmp == -1)
    {
      //  Remove ampersands
      return rawString;
    }

    //  Split string at first ampersand

    string leftSubString = rawString.Substring(0, indexOfAmp);
    string rightSubString = rawString.Substring(indexOfAmp + 1);

    //  Remove excess ampersands
    rightSubString.Replace ("&", "");

    //  Insert highlighting code around the character preceeded by the ampersand

    //  Empty right sub-string means no char to highlight.
    if (rightSubString.Length == 0)
    {
      accessKey = String.Empty;
      return leftSubString;
    }

    accessKey = rightSubString.Substring (0, 1);

    StringBuilder stringBuilder = new StringBuilder (rawString.Length + highlighter.Length);

    stringBuilder.Append (leftSubString);
    stringBuilder.AppendFormat (highlighter, accessKey);
    stringBuilder.Append (rightSubString.Substring(1));
    
    return stringBuilder.ToString();
  }
  
  // TODO: use access key (nicht für texte aus dem control)

  private string _forControl = null;

	public SmartLabel()
    : base (HtmlTextWriterTag.Label)
	{
	}

  /// <summary>
  ///   The ID of the control to display a label for.
  /// </summary>
  [TypeConverter (typeof (SmartControlToStringConverter))]
  public string ForControl
  {
    get { return _forControl; }
    set { _forControl = value; }
  }

  protected override void Render(HtmlTextWriter writer)
  {
    this.RenderBeginTag (writer);

    ISmartControl smartControl = NamingContainer.FindControl (ForControl) as ISmartControl;
    if (smartControl != null && smartControl.DisplayName != null)
      writer.Write (smartControl.DisplayName);
    else
      writer.Write ("[Label for " + ForControl + "]");

    this.RenderEndTag (writer);
  }

  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    base.AddAttributesToRender (writer);

    Control target = NamingContainer.FindControl (ForControl);
    ISmartControl smartControl = target as ISmartControl;
    bool useLabel;
    if (smartControl != null)
    {
      target = smartControl.TargetControl;
      useLabel = smartControl.UseLabel;
    }
    else
    {
      useLabel = ! (target is DropDownList || target is HtmlSelect);
    }

    if (useLabel && target != null)
      writer.AddAttribute (HtmlTextWriterAttribute.For, target.ClientID);

    // TODO: add <a href="ToName(target.ClientID)"> ...
    // ToName: '.' -> '_'

    //  Accesskey support
  }
}

}
