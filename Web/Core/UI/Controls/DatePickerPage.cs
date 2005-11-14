using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Rubicon.Utilities;
using Rubicon.Web;
using Rubicon.Web.Utilities;
using Rubicon.Web.UI;

namespace Rubicon.Web.UI.Controls
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
  
#if ! net20
  protected HtmlForm Form;
#endif
  protected HtmlHeadContents HtmlHeadContents;
  protected Calendar Calendar;
  /// <summary> Preserves the target control's ID during post backs. </summary>
  private HtmlInputHidden TargetIDField;
  /// <summary> Preserves the frame's ID in the parent page during post backs. </summary>
  private HtmlInputHidden DatePickerIDField;
  /// <summary> Contains the date to be selected in the calendar. </summary>
  private HtmlInputHidden DateValueField;

	override protected void OnInit(EventArgs e)
	{
    if (Form == null)
      throw new HttpException (this.GetType().FullName + " does not initialize field 'Form'.");
    if (HtmlHeadContents == null)
      throw new HttpException (this.GetType().FullName + " does not initialize field 'HtmlHeadContents'.");
    if (Calendar == null)
      throw new HttpException (this.GetType().FullName + " does not initialize field 'Calendar'.");

    Calendar.SelectionChanged += new EventHandler(Calendar_SelectionChanged);

    TargetIDField = new HtmlInputHidden();
    TargetIDField.ID = "TargetIDField";
    TargetIDField.EnableViewState = false;
    Form.Controls.Add (TargetIDField);

    DatePickerIDField = new HtmlInputHidden();
    DatePickerIDField.ID = "DatePickerIDField";
    DatePickerIDField.EnableViewState = false;
    Form.Controls.Add (DatePickerIDField);

    DateValueField = new HtmlInputHidden();
    DateValueField.ID = "DateValueField";
    DateValueField.EnableViewState = false;
    Form.Controls.Add (DateValueField);

    //  Force the creation of the postback function
    Page.GetPostBackEventReference (this, "");

    base.OnInit(e);
	}

  protected override void OnLoad(EventArgs e)
  {
    string dateValue = null;
    if (IsPostBack)
    {
      dateValue = DateValueField.Value;
    }
    else
    {
      dateValue = Request.Params["DateValueField"];
      TargetIDField.Value = Request.Params["TargetIDField"];
      DatePickerIDField.Value = Request.Params["DatePickerIDField"];
    }

    //  Initalize the calendar
    try
    {
      if (! StringUtility.IsNullOrEmpty (dateValue))
      {
        Calendar.SelectedDate = DateTime.Parse (dateValue);
        Calendar.VisibleDate = Calendar.SelectedDate;
      }
    }
    catch (FormatException)
    {
      //  Do nothing since user wishes to pick a valid date using the calendar
    }
    DateValueField.Value = string.Empty;

    base.OnLoad (e);
  }

  protected override void OnPreRender(EventArgs e)
  {
    string key = typeof (DatePickerPage).FullName + "_Script";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      string scriptUrl = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (DatePickerPage), ResourceType.Html, c_datePickerScriptUrl);
      HtmlHeadAppender.Current.RegisterJavaScriptInclude (key, scriptUrl);
    }

    base.OnPreRender (e);
  }

  private void Calendar_SelectionChanged(object sender, EventArgs e)
  {
    string key = typeof (DatePickerPage).FullName + "_Calendar_SelectionChanged";
    string script = "DatePicker_Calendar_SelectionChanged ('" + Calendar.SelectedDate.ToShortDateString() + "');";
    if (! Page.IsStartupScriptRegistered (key))
      PageUtility.RegisterStartupScriptBlock (this, key, script);
  }
}

}
