using System;
using System.ComponentModel;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using Rubicon;
using Rubicon.Utilities;
using Rubicon.Globalization;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.UI.Controls
{

/// <summary>
///   Collects the validation errors from all <see cref="FormGridManager"/> instances on the page
///   and displays the validation state.
/// </summary>
[ToolboxData("<{0}:ValidationStateViewer runat='server' visible='true'></{0}:ValidationStateViewer>")]
[ToolboxItemFilter("System.Web.UI")]
public class ValidationStateViewer : WebControl, IControl
{
  // types

  // constants
  private const string c_noticeTextResourceID = "auto:ValidationStateViewer:NoticeText";

  /// <summary>
  ///   CSS-Class applied to the individual validation messages
  /// </summary>
  /// <remarks>
  ///   Class: <c>formGridValidationMessage</c>
  /// </remarks>
  private const string c_cssClassValidationMessage = "formGridValidationMessage";
  
  /// <summary>
  ///   CSS-Class applied to the validation notice
  /// </summary>
  /// <remarks>
  ///   Class: <c>formGridValidationMessage</c>
  /// </remarks>
  private const string c_cssClassValidationNotice = "formGridValidationNotice";

  // static members

  // member fields

  /// <summary>
  ///   Collection of <see cref="FormGridManager" /> instances in the page 
  /// </summary>
  private ArrayList _formGridManagers;

  /// <summary>
  ///   The Text displayed if <see cref="ValidationStateViewer.ValidationErrorStyle"/> is set to 
  ///   <see cref="Rubicon.Web.UI.Controls.ValidationErrorStyle.Notice"/>
  /// </summary>
  private string _noticeText;

  /// <summary>
  ///   The style in which the validation errors should be displayed on the page
  /// </summary>
  private ValidationErrorStyle _validationErrorStyle;

  /// <summary>
  ///   The <c>LiteralControl</c> containing the text displayed as a validation notice.
  /// </summary>
  private LiteralControl _validationErrorNotice;

  // construction and disposing

  /// <summary>
  ///   Simple constructor
  /// </summary>
  public ValidationStateViewer()
  {
    _validationErrorStyle = ValidationErrorStyle.Notice;
    _validationErrorNotice = new LiteralControl();
    _noticeText = String.Empty;
  }

  /// <summary>
  ///   Registers <see cref="ParentPage_PreRender"/> with the parent page's <c>PreRender</c>
  ///   event, before calling the base class's <c>OnInit</c>
  /// </summary>
  /// <param name="e">The <see cref="EventArgs"/>.</param>
  protected override void OnInit (EventArgs e)
	{
    this.Page.PreRender += new EventHandler(ParentPage_PreRender);
    base.OnInit(e);
	}

  /// <summary>
  ///   Event handler for the control's PreRender event tasked with 
  ///   filling the form grid manager list.
  /// </summary>
  /// <param name="sender">The <see cref="Page"/> object.</param>
  /// <param name="e">The <see cref="EventArgs"/>.</param>
  private void ParentPage_PreRender (object sender, EventArgs e)
  {
    PopulateFormGridManagerList (this.Parent);
    OutputValidationState();
  }

  /// <summary>
  ///   Registers all instances of <see cref="FormGridManager"/>
  /// </summary>
  /// <param name="control">Parent element of the FormGridManager objects</param>
  private void PopulateFormGridManagerList (Control control)
  {
    ArgumentUtility.CheckNotNull ("control", control);

    //  Has already pupulated
    if (_formGridManagers != null)
      return;

    _formGridManagers = new ArrayList();

    //  Add all FormGridManager instances
    foreach (Control childControl in control.Controls)
    {
      FormGridManager formGridManager = childControl as FormGridManager;

      if (formGridManager != null)
        _formGridManagers.Add(formGridManager);

      //  For perfomance, only recursivly call PopulateFormGridList if control-collection is filled
      if (childControl.Controls.Count > 0)
        PopulateFormGridManagerList (childControl);
    }
  }

  /// <summary>
  ///   Outputs the validation state to the page as defined in <see cref="ValidationErrorStyle"/>.
  /// </summary>
  protected virtual void OutputValidationState()
  {
    switch (_validationErrorStyle)
    {
      case ValidationErrorStyle.DetailedMessages:
      {
        DisplayValidationMessages();
        break;
      }
      case ValidationErrorStyle.Notice:
      {
        DisplayValidationNotice();
        break;
      }
      case ValidationErrorStyle.HideErrors:
      {
        //  Do nothing
        break;
      }
      default:
      {
        //  Do nothing
        break;
      }
    }
  }

  /// <summary>
  ///   Displays a short notice if validation errors where found.
  /// </summary>
  protected virtual void DisplayValidationNotice()
  {
    bool hasValidationErrors = false;

    foreach (FormGridManager formGridManager in _formGridManagers)
    {
      if (formGridManager.GetValidationErrors().Length > 0)
      {
        hasValidationErrors = true;
        break;
      }
    }

    //  Enclose the validation error notice inside a div
    if (hasValidationErrors)
    {
      StringBuilder startTagStringBuilder = new StringBuilder(50);
      startTagStringBuilder.AppendFormat ("<div class=\"{0}\">", CssClassValidationNotice);

      LiteralControl startTag = new LiteralControl();
      startTag.Text = startTagStringBuilder.ToString();
      Controls.Add (startTag);

      Controls.Add (_validationErrorNotice);

      LiteralControl endTag = new LiteralControl();
      endTag.Text = "</div>";
      Controls.Add (endTag);
    }
  }

  /// <summary>
  ///   Displays the validation messages for each error.
  /// </summary>
  protected virtual void DisplayValidationMessages()
  {
    foreach (FormGridManager formGridManager in _formGridManagers)
    {
      ValidationError[] validationErrors = formGridManager.GetValidationErrors();

      //  Get validation messages
      foreach (ValidationError validationError in validationErrors)
      {
        if (validationError == null)
          continue;

        //  Label resets the <select> element, ruining postback
        if (validationError.ValidatedControl is TextBox)
          Controls.Add (validationError.ToLabel(CssClassValidationMessage));
        else
          Controls.Add (validationError.ToSpan(CssClassValidationMessage));

        Controls.Add (new LiteralControl ("<br />"));
      }
    }
  }

  /// <summary> 
	///   Render this control to the output parameter specified.
	/// </summary>
	/// <param name="output"> The HTML writer to write out to </param>
	protected override void Render(HtmlTextWriter output)
	{
    if (ControlHelper.IsDesignMode (this, this.Context))
    {
      if (Controls.Count == 0)
        Controls.Add(new LiteralControl("No validation at design time"));
    }

    base.Render(output);
  }

  /// <summary>
  ///   The Text displayed if <see cref="ValidationStateViewer.ValidationErrorStyle"/> is set to 
  ///   <see cref="Rubicon.Web.UI.Controls.ValidationErrorStyle.Notice"/>
  /// </summary>
  /// <value>A string.</value>
  [CategoryAttribute("Appearance")]
  [DefaultValue("")]
  [Description("Sets the Text to be displayed if ValidationErrorStyle is set to Notice.")]
  public string NoticeText
  {
    get { return _noticeText; }
    set { _noticeText = value; }
  }

  /// <summary>
  ///   Defines how the validation errors are displayed on the page.
  /// </summary>
  /// <value>A symbol defined in the <see cref="ValidationErrorStyle"/>enumeration.</value>
  [CategoryAttribute("Behavior")]
  [DefaultValue(ValidationErrorStyle.Notice)]
  [Description("Defines how the validation messages are displayed.")]
  public ValidationErrorStyle ValidationErrorStyle
  {
    get { return _validationErrorStyle; }
    set { _validationErrorStyle = value; }
  }

  /// <summary>
  ///   CSS-Class applied to the individual validation messages.
  /// </summary>
  /// <remarks>
  ///   Class: <c>formGridValidationMessage</c>
  /// </remarks>
  protected virtual string CssClassValidationMessage
  { get { return c_cssClassValidationMessage;} }

  /// <summary>
  ///   CSS-Class applied to the validation notice.
  /// </summary>
  /// <remarks>
  ///   Class: <c>formGridValidationMessage</c>
  /// </remarks>
  protected virtual string CssClassValidationNotice
  { get { return c_cssClassValidationNotice;} }
}  


/// <summary>
///   A list of possible ways to displau the validation messages.
/// </summary>
public enum ValidationErrorStyle
{
  /// <summary>
  ///   Display no messages.
  /// </summary>
  HideErrors,
  /// <summary>
  ///   Display a short notice if validation errors where found.
  /// </summary>
  Notice,
  /// <summary>
  ///   Display the individual validation messages provided by the FormGridManager.
  /// </summary>
  DetailedMessages
}

}