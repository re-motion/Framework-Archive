using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.Web.UI;
using Rubicon.Web;

namespace Rubicon.Web.UI.Controls
{

/*

  <script>
	  function getmenuforlist1()
	  {
		  var menu = new array();
		  array.add ("editattributes.gif", "Bearbeiten", "edit");
		  ...
		  return menu;
	  }

	  var menu1 = new array();
	  addmenuitem ("menu1", "edid.gif", "Bearbeiten", "edit");
	  addseperator ("menu1");
	  addmenuitem ("menu1", "edid.gif", "Bearbeiten", "edit");
  </>



  <table id=list1 onclick="showmenu(getmenuforlist1())"



  ID = cut
  category = edit
  icon = cut.gif
  text = "Ausschneiden"

  ID = open
  category = object
  icon = opne.gif
  text = "Öffnen"

  ID = duplicate
  category = edit
  icon = dup.gif
  text = "Duplizieren"



  Edit
  Duplicate
  ---------
  Öffnen



  auto:list1:menu:cut:text = "Ausschneiden"
*/
/// <remarks>
///   Work in Progress
/// </remarks>
public class DropDownMenu: WebControl
{
	public DropDownMenu()
	{
	}

  private string _titleText;
  private string _titleIcon;

  public string TitleText
  {
    get { return _titleText; }
    set { _titleText = value; }
  }

  public string TitleIcon
  {
    get { return _titleIcon; }
    set { _titleIcon = value; }
  }

  protected override void OnPreRender (EventArgs e)
  {
    string key = typeof (DropDownMenu).FullName + "_Style";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      string url = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (DropDownMenu), ResourceType.Html, "DropDownMenu.css");
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, url);
    }
    base.OnPreRender (e);
  }

  private const string c_dropDownIcon = "DropDownArrow.gif";

  protected override void Render (HtmlTextWriter writer)
  {
    if (! Width.IsEmpty)
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Width.ToString());
    if (! Height.IsEmpty)
      writer.AddStyleAttribute (HtmlTextWriterStyle.Height, Height.ToString());
    writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Control-Tag

    //  Menu-Div filling the control's div is required to apply internal css attributes
    //  for position, width and height. This allows the Head and th popup-div to align themselves
    writer.AddStyleAttribute ("position", "relative");
    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
    writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
    writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Menu-Div

    //  Head-Div is used to group the title and the button, providing a single point of reference
    //  for the popup-div.
    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
    writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHead);
    writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Drop Down Head-Div

    
    //  Options Drop Down Button 
    writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
    writer.AddStyleAttribute ("min-height", "1em");
    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1em");
    writer.AddStyleAttribute ("float", "right");
    writer.AddStyleAttribute ("text-align", "center");
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHeadButton);
    writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin Drop Down Button-San
    string url = ResourceUrlResolver.GetResourceUrl (
        this, Context, typeof (DropDownMenu), ResourceType.Image, c_dropDownIcon);
    writer.AddAttribute (HtmlTextWriterAttribute.Src, url);
    writer.AddAttribute(HtmlTextWriterAttribute.Type, "image");
    writer.AddStyleAttribute ("vertical-align", "middle");
    writer.RenderBeginTag (HtmlTextWriterTag.Input);
    writer.RenderEndTag();
    writer.RenderEndTag();  // End Drop Down Button-Span

    //  TODO: IE 5.01 has trouble with height
    //  Options Drop Down Titel
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHeadTitle);
    writer.RenderBeginTag (HtmlTextWriterTag.Span);
    writer.Write (_titleText);
    writer.RenderEndTag();
    writer.RenderEndTag();  // End Drop Down Head-Div

    //  Options Drop Down PopUp
    writer.AddStyleAttribute ("position", "absolute");
    writer.AddStyleAttribute ("right", "0px");
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassPopUpTextPane);
    writer.RenderBeginTag (HtmlTextWriterTag.Div);
    writer.WriteLine ("<div>Item 1</div>"); // width: 100% spans menu across the whole options-div
    writer.WriteLine ("<div>Item 2</div>");
    writer.WriteLine ("<div>Item 3</div>");
    writer.WriteLine ("<div>Item 4</div>");
    writer.RenderEndTag();

    writer.RenderEndTag(); // End Menu-Div

    writer.RenderEndTag(); // End Control-Tag
  }

  protected virtual string CssClassHead
  { get { return "dropDownMenuHead"; } }

  protected virtual string CssClassHeadFocus
  { get { return "dropDownMenuHeadFocus"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="DropDownMenu"/>'s title. </summary>
  /// <remarks> Class: <c></c> </remarks>
  protected virtual string CssClassHeadTitle
  { get { return "dropDownMenuHeadTitle"; } }

  protected virtual string CssClassHeadTitleFocus
  { get { return "dropDownMenuHeadTitleFocus"; } }

  protected virtual string CssClassHeadButton
  { get { return "dropDownMenuHeadButton"; } }

  protected virtual string CssClassMenuButtonFocus
  { get { return "dropDownMenuButtonFocus"; } }

  protected virtual string CssClassPopUpTextPane
  { get { return "dropDownMenuPopUpTextPane"; } }

  protected virtual string CssClassPopUpIconPane
  { get { return "dropDownMenuPopUpIconPane"; } }
}

}
