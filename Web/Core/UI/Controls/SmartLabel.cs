using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using Rubicon.Web.UI.Design;
using System.Text;
using Rubicon.Utilities;

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

  private string _forControl = null;
  //  Unfinished implementation of SmartLabel populated by ResourceDispatchter
  //  private string _text = string.Empty;

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
    string text = string.Empty;

    ISmartControl smartControl = NamingContainer.FindControl (ForControl) as ISmartControl;
    if (smartControl != null && smartControl.DisplayName != null)
    {
      text = smartControl.DisplayName;
    }
    // TODO: use access key (nicht für texte aus dem control)
    //  Unfinished implementation of SmartLabel populated by ResourceDispatchter
    //    else if (!StringUtility.IsNullOrEmpty (_text))
    //    {
    //      // TODO: use access key (nicht für texte aus dem control)
    //      Control associatedControl = NamingContainer.FindControl (ForControl);
    //    
    //      if (associatedControl != null)
    //      {
    //        ISmartControl smartControl = control as ISmartControl;
    //        if (smartControl != null && smartControl.UseLabel)
    //        {
    //          string accessKey;
    //          text = SmartLabel.FormatLabelText (label.Text, true, out accessKey);
    //          label.AccessKey = accessKey;
    //        }
    //        else if (control is DropDownList || control is HtmlSelect)
    //        {
    //          text = SmartLabel.FormatLabelText (label.Text, false);
    //          label.AccessKey = "";
    //        }
    //        else
    //        {
    //          string accessKey;
    //          text = SmartLabel.FormatLabelText (label.Text, true, out accessKey);
    //          label.AccessKey = accessKey;
    //        }
    //      }
    //      else
    //      {
    //        text = SmartLabel.FormatLabelText (label.Text, false);
    //      }
    //    }
    else
    {
      text = "[Label for " + ForControl + "]";
    }

    writer.Write (text);
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
  }

  //  Unfinished implementation of SmartLabel populated by ResourceDispatchter
  //  /// <summary>
  //  ///   Gets or sets the text displayed if the <see cref="SmartLabel"/> is not bound to an 
  //  ///   <see cref="ISmartControl "/> or the <see cref="ISmartControl"/> does provide a 
  //  ///   <see cref="ISmartControl.DisplayName"/>.
  //  /// </summary>
  //  [Category ("Appearance")]
  //  [Description ("The text displayed if the SmartLabel is not bound to an ISmartControl or the ISmartControl does provide a DisplayName.")]
  //  [DefaultValue ("")]
  //  public string Text
  //  {
  //    get { return _text; }
  //    set { _text = value; }
  //  }
}

}
