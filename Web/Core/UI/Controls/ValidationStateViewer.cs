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
  private const string c_noticeText = "Incorrect input found.";

  /// <summary> CSS-Class applied to the individual validation messages. </summary>
  /// <remarks> Class: <c>formGridValidationMessage</c>. </remarks>
  private const string c_cssClassValidationMessage = "formGridValidationMessage";
  
  /// <summary> CSS-Class applied to the validation notice. </summary>
  /// <remarks> Class: <c>formGridValidationMessage</c>. </remarks>
  private const string c_cssClassValidationNotice = "formGridValidationNotice";

  // static members

  // member fields

  /// <summary> Collection of <see cref="FormGridManager" /> instances in the page. </summary>
  private ArrayList _formGridManagers;

  /// <summary>
  ///   The Text displayed if <see cref="ValidationStateViewer.ValidationErrorStyle"/> is set to 
  ///   <see cref="Rubicon.Web.UI.Controls.ValidationErrorStyle.Notice"/>.
  /// </summary>
  private string _noticeText;
  /// <summary> The style in which the validation errors should be displayed on the page. </summary>
  private ValidationErrorStyle _validationErrorStyle = ValidationErrorStyle.Notice;
  private bool _showLabels = true;
  private bool _skipNamingContainers = false;

  // construction and disposing

  /// <summary> Initializes a new instance of the <see cref="ValidationStateViewer"/> class. </summary>
  public ValidationStateViewer()
  {
  }

  /// <summary>
  ///   Registers <see cref="ParentPage_PreRender"/> with the parent page's <c>PreRender</c>
  ///   event, before calling the base class's <c>OnInit</c>.
  /// </summary>
  /// <param name="e"> The <see cref="EventArgs"/>. </param>
  protected override void OnInit (EventArgs e)
	{
    this.Page.PreRender += new EventHandler(ParentPage_PreRender);
    base.OnInit(e);
	}

  /// <summary>
  ///   Event handler for the control's <c>PreRender</c> event tasked with 
  ///   filling the form grid manager list.
  /// </summary>
  /// <param name="sender"> The <see cref="Page"/> object. </param>
  /// <param name="e"> The <see cref="EventArgs"/>. </param>
  private void ParentPage_PreRender (object sender, EventArgs e)
  {
    EnsureFormGridManagerListPopulated ();
  }

  private void EnsureFormGridManagerListPopulated ()
  {
    if (_formGridManagers == null)
    {
      _formGridManagers = new ArrayList();
      PopulateFormGridManagerList (NamingContainer);
    }
  }

  /// <summary> Registers all instances of <see cref="FormGridManager"/>. </summary>
  /// <param name="parent"> Parent element of the FormGridManager objects. </param>
  private void PopulateFormGridManagerList (Control parent)
  {
    ArgumentUtility.CheckNotNull ("parent", parent);

    //  Add all FormGridManager instances
    foreach (Control childControl in parent.Controls)
    {
      FormGridManager formGridManager = childControl as FormGridManager;

      if (formGridManager != null)
        _formGridManagers.Add(formGridManager);

      bool isChildNamingContainer = childControl is INamingContainer;
      if (! _skipNamingContainers || isChildNamingContainer)
        PopulateFormGridManagerList (childControl);
    }
  }

  protected override void RenderChildren(HtmlTextWriter writer)
  {
    if (ControlHelper.IsDesignMode (this, this.Context))
    {
      writer.WriteLine ("No validation at design time");
    }
    else
    {
      switch (_validationErrorStyle)
      {
        case ValidationErrorStyle.DetailedMessages:
        {
          RenderValidationMessages (writer);
          break;
        }
        case ValidationErrorStyle.Notice:
        {
          RenderValidationNotice (writer);
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
  }

  /// <summary> Displays a short notice if validation errors where found. </summary>
  protected virtual void RenderValidationNotice (HtmlTextWriter writer)
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
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassValidationNotice);
      writer.RenderBeginTag (HtmlTextWriterTag.Div);
      if (StringUtility.IsNullOrEmpty (_noticeText))
        writer.WriteLine (c_noticeText);
      else
        writer.WriteLine (_noticeText);
      writer.RenderEndTag();
    }
  }

  /// <summary> Displays the validation messages for each error. </summary>
  protected virtual void RenderValidationMessages (HtmlTextWriter writer)
  {
    writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
    writer.RenderBeginTag (HtmlTextWriterTag.Table);
    foreach (FormGridManager formGridManager in _formGridManagers)
    {
      ValidationError[] validationErrors = formGridManager.GetValidationErrors();
      //  Get validation messages
      foreach (ValidationError validationError in validationErrors)
      {
        if (validationError == null)
          continue;

        writer.RenderBeginTag (HtmlTextWriterTag.Tr);

        if (validationError.Label  != null)
        {
          writer.AddStyleAttribute ("padding-right", "3pt");
          writer.RenderBeginTag (HtmlTextWriterTag.Td);
          foreach (Control control in validationError.Label)
            control.RenderControl (writer);
          writer.RenderEndTag();
        }
        else
        {
          writer.RenderBeginTag (HtmlTextWriterTag.Td);
          writer.RenderEndTag();
        }
      
        writer.RenderBeginTag (HtmlTextWriterTag.Td);
        //  Label resets the <select> element, ruining postback
        if (validationError.ValidatedControl is TextBox)
          validationError.ToLabel(CssClassValidationMessage).RenderControl (writer);
        else
          validationError.ToSpan(CssClassValidationMessage).RenderControl (writer);
        writer.RenderEndTag();

        writer.RenderEndTag();
      }
    }
    writer.RenderEndTag();
  }

  /// <summary>
  ///   The Text displayed if <see cref="ValidationStateViewer.ValidationErrorStyle"/> is set to 
  ///   <see cref="Rubicon.Web.UI.Controls.ValidationErrorStyle.Notice"/>
  /// </summary>
  /// <value> A string. </value>
  [CategoryAttribute("Appearance")]
  [Description("Sets the Text to be displayed if ValidationErrorStyle is set to Notice.")]
  [DefaultValue("")]
  public string NoticeText
  {
    get { return _noticeText; }
    set { _noticeText = value; }
  }

  /// <summary> Gets or sets a value that defines how the validation errors are displayed on the page. </summary>
  /// <value> A symbol defined in the <see cref="ValidationErrorStyle"/>enumeration. </value>
  [CategoryAttribute("Behavior")]
  [Description("Defines how the validation messages are displayed.")]
  [DefaultValue(ValidationErrorStyle.Notice)]
  public ValidationErrorStyle ValidationErrorStyle
  {
    get { return _validationErrorStyle; }
    set { _validationErrorStyle = value; }
  }

  /// <summary>
  ///   Gets or sets a flag that determines whether to render the label associated with the erroneous control 
  ///   in front of the error message.
  /// </summary>
  /// <value> <see langword="true"/> to render the label. </value>
  [Category ("Apearance")]
  [Description ("true to render the label associated with the erroneous control in front of the error message.")]  
  [DefaultValue (true)]
  public bool ShowLabels
  {
    get { return _showLabels; }
    set { _showLabels = value; }
  }

  /// <summary>
  ///   Gets or sets a flag that determines whether the <see cref="ValidationStateViewer"/> skips
  ///   <see cref="INamingContainer"/> controls when searching for <see cref="FormGridManager"/> controls.
  /// </summary>
  /// <value> <see langword="false"/> to include the naming containers in the search path. </value>
  [Category ("Behavior")]
  [Description ("false to branch into naming containers when searching for FormGridManagers.")]  
  [DefaultValue (false)]
  public bool SkipNamingContainers
  {
    get { return _skipNamingContainers; }
    set { _skipNamingContainers = value; }
  }

  /// <summary> CSS-Class applied to the individual validation messages. </summary>
  /// <remarks> Class: <c>formGridValidationMessage</c>. </remarks>
  protected virtual string CssClassValidationMessage
  { get { return c_cssClassValidationMessage;} }

  /// <summary> CSS-Class applied to the validation notice. </summary>
  /// <remarks> Class: <c>formGridValidationMessage</c>. </remarks>
  protected virtual string CssClassValidationNotice
  { get { return c_cssClassValidationNotice;} }
}  


/// <summary> A list of possible ways to displau the validation messages. </summary>
public enum ValidationErrorStyle
{
  /// <summary> Display no messages. </summary>
  HideErrors,
  /// <summary> Display a short notice if validation errors where found. </summary>
  Notice,
  /// <summary> Display the individual validation messages provided by the FormGridManager. </summary>
  DetailedMessages
}

}