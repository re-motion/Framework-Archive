using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Rubicon.Utilities;
using Rubicon.Web;
using Rubicon.Web.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary> 
///   Displayes a <see cref="Calendar"/> and updates an input control with the newly selected date.
/// </summary>
/// <remarks>
///   <para> 
///     The form is designed to be displayed inside an IFrame.
///   </para>
///   <para> 
///     The inherited aspx form must contain a control derived from <see cref="Calendar"/>
///     named <c>Calendar</c>.
///   </para>
///   <para> 
///     The inherited aspx form's form instance must named <c>Form</c>.
///   </para>
///   <para>
///     Open the date picker using the <c>ShowDatePicker</c> function located in <c>DatePicker.js</c>.
///   </para>
///   <para>
///     Popup does not work on FireFox, Internet Explorer 5.1 and below
///   </para>
/// </remarks>
public class DatePickerPage : Page
{
  private const string c_datePickerScriptUrl = "DatePicker.js";
  
  protected HtmlForm Form;
  protected Calendar Calendar;
  /// <summary> Preserves the target control's ID during post backs. </summary>
  private HtmlInputHidden TargetIDField;
  /// <summary> Preserves the frame's ID in the parent page during post backs. </summary>
  private HtmlInputHidden FrameIDField;
  /// <summary> Contains the date to be selected in the calendar. </summary>
  private HtmlInputHidden DateValueField;

	override protected void OnInit(EventArgs e)
	{
    if (Form == null)
      throw new HttpException (this.GetType().FullName + " does not initialize field 'Form'.");
    if (Calendar == null)
      throw new HttpException (this.GetType().FullName + " does not initialize field 'Calendar'.");

    Calendar.SelectionChanged += new EventHandler(Calendar_SelectionChanged);
    
    TargetIDField = new HtmlInputHidden();
    TargetIDField.ID = "TargetIDField";
    TargetIDField.EnableViewState = false;
    Form.Controls.Add (TargetIDField);

    FrameIDField = new HtmlInputHidden();
    FrameIDField.ID = "FrameIDField";
    FrameIDField.EnableViewState = false;
    Form.Controls.Add (FrameIDField);

    DateValueField = new HtmlInputHidden();
    DateValueField.ID = "DateValueField";
    DateValueField.EnableViewState = false;
    Form.Controls.Add (DateValueField);

    //  Force the creation of the postback function
    Page.GetPostBackClientHyperlink(this, "");

    base.OnInit(e);
	}

  protected override void OnLoad(EventArgs e)
  {
    if (IsPostBack)
    {
      //  Initalize the calendar
      try
      {
        if (! StringUtility.IsNullOrEmpty (DateValueField.Value))
        {
          Calendar.SelectedDate = DateTime.Parse (DateValueField.Value);
          Calendar.VisibleDate = Calendar.SelectedDate;
        }
      }
      catch (FormatException)
      {
        //  Do nothing since user wishes to pick a valid date using the calendar
      }
      DateValueField.Value = string.Empty;
    }

    base.OnLoad (e);
  }

  protected override void OnPreRender(EventArgs e)
  {
    string scriptUrl = ResourceUrlResolver.GetResourceUrl (
        this, Context, this.GetType(), ResourceType.Html, c_datePickerScriptUrl);

    PageUtility.RegisterClientScriptInclude (
        this,
        typeof (DatePickerPage).FullName, 
        scriptUrl);

    base.OnPreRender (e);
  }

  private void Calendar_SelectionChanged(object sender, EventArgs e)
  {
    PageUtility.RegisterStartupScriptBlock (
        this, 
        typeof (DatePickerPage).FullName + "_Calendar_SelectionChanged",
        "Calendar_SelectionChanged ('" + Calendar.SelectedDate.ToShortDateString() + "')");
  }
}

}
