using System;
using System.Web.UI;
using System.Web.UI.WebControls;

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
[Obsolete ("Currently being implemented.")]
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

  protected override void Render (HtmlTextWriter writer)
  {
    writer.AddStyleAttribute ("position", "relative");
   // writer.AddStyleAttribute ("z-index", zIndex.ToString());
    if (Width.IsEmpty)
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
    else
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Width.ToString());
    writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Options-Div

    //  Options Drop Down Titel
    writer.AddStyleAttribute ("position", "relative");
    writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundColor, "lightcyan");
    writer.RenderBeginTag (HtmlTextWriterTag.Div);
    writer.Write (_titleText);
    writer.RenderEndTag();
    
    //  Options Drop Down Icon 
    writer.AddStyleAttribute ("position", "absolute");
    writer.AddStyleAttribute ("right", "0px");
    writer.AddStyleAttribute ("top", "0px");
    writer.AddStyleAttribute ("height", "100%");
    writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundColor, "cyan");
    writer.RenderBeginTag (HtmlTextWriterTag.Div);
    writer.Write ("V");
    writer.RenderEndTag();

    //  Options Drop Down Menu
    writer.AddStyleAttribute ("position", "absolute");
    writer.AddStyleAttribute ("right", "0px");
    writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundColor, "lime");
    writer.RenderBeginTag (HtmlTextWriterTag.Div);
    writer.WriteLine ("<div>Item 1</div>"); // width: 100% spans menu across the whole options-div
    writer.WriteLine ("<div>Item 2</div>");
    writer.WriteLine ("<div>Item 3</div>");
    writer.WriteLine ("<div>Item 4</div>");
    writer.RenderEndTag();

    writer.RenderEndTag(); // End Options-Div
  }

}
}
