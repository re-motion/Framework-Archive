using System;
using System.ComponentModel;
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
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

  /// <summary> A list of validation state viewer wide resources. </summary>
  /// <remarks> Resources will be accessed using IResourceManager.GetString (Enum). </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Rubicon.Web.UI.Globalization.ValidationStateViewer")]
  protected enum ResourceIdentifier
  {
    /// <summary>The summary message displayed if validation errors where found. </summary>
    NoticeText
  }

  // constants

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
  /// <summary> Caches the <see cref="ResourceManagerSet"/> for this <see cref="ValidationStateViewer"/>. </summary>
  private ResourceManagerSet _cachedResourceManager;

  // construction and disposing

  /// <summary> Initializes a new instance of the <see cref="ValidationStateViewer"/> class. </summary>
  public ValidationStateViewer()
  {
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
      PopulateFormGridManagerList (childControl);
    }
  }

  protected override void Render (HtmlTextWriter writer)
  {
    if (!ControlHelper.IsDesignMode (this, this.Context))
    {
      switch (_validationErrorStyle)
      {
        case ValidationErrorStyle.Notice:
        {
          RenderValidationNotice (writer);
          break;
        }
        case ValidationErrorStyle.DetailedMessages:
        {
          RenderValidationMessages (writer);
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
    bool isPageValid = true;
    foreach (IValidator validator in Page.Validators)
    {
      if (! validator.IsValid)
      {
        isPageValid = false;
        break;
      }
    }

    //  Enclose the validation error notice inside a div
    if (! isPageValid)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassValidationNotice);
      writer.RenderBeginTag (HtmlTextWriterTag.Div);
      if (StringUtility.IsNullOrEmpty (_noticeText))
      {
        IResourceManager resourceManager = GetResourceManager();
        writer.WriteLine (resourceManager.GetString (ResourceIdentifier.NoticeText));
      }
      else
        writer.WriteLine (_noticeText);
      writer.RenderEndTag();
    }
  }

  /// <summary> Displays the validation messages for each error. </summary>
  protected virtual void RenderValidationMessages (HtmlTextWriter writer)
  {
    _formGridManagers = new ArrayList();
    PopulateFormGridManagerList (NamingContainer);

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

        if (validationError.Labels  != null)
        {
          writer.AddStyleAttribute ("padding-right", "3pt");
          writer.RenderBeginTag (HtmlTextWriterTag.Td);
          foreach (Control control in validationError.Labels)
          {
            if (control is SmartLabel)
              writer.Write (((SmartLabel) control).GetText());
            else if (control is FormGridLabel)
              writer.Write (((FormGridLabel) control).Text);
            else if (control is Label)
              writer.Write (((Label) control).Text);
            else if (control is LiteralControl)
              writer.Write (((LiteralControl) control).Text);
          }
          writer.RenderEndTag();
        }
        else
        {
          writer.RenderBeginTag (HtmlTextWriterTag.Td);
          writer.RenderEndTag();
        }
      
        writer.RenderBeginTag (HtmlTextWriterTag.Td);
        //  Label resets the <select> element, ruining postback
        if (! (validationError.ValidatedControl is DropDownList || validationError.ValidatedControl is HtmlSelect))
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
  ///   Find the <see cref="IResourceManager"/> for this <see cref="ValidationStateViewer"/>.
  /// </summary>
  /// <returns></returns>
  protected IResourceManager GetResourceManager()
  {
    //  Provider has already been identified.
    if (_cachedResourceManager != null)
        return _cachedResourceManager;

    //  Get the resource managers

    IResourceManager localResourceManager = 
        MultiLingualResourcesAttribute.GetResourceManager (typeof (ResourceIdentifier), true);
    IResourceManager namingContainerReosurceManager = 
        ResourceManagerUtility.GetResourceManager (NamingContainer);
    _cachedResourceManager = new ResourceManagerSet (localResourceManager, namingContainerReosurceManager);

    return _cachedResourceManager;
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