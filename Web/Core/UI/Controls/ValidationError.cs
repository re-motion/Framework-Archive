using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Text;

using Rubicon;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Controls
{
/// <summary>
///   Encapsulats an validation error: the effected control, the message and the validator used.
/// </summary>
public class ValidationError
{
  // constants

  // types

  // static members

  // member fields

  /// <summary> The control with an invalid state. </summary>
  private Control _validatedControl;

  private ControlCollection _labels;

  /// <summary> The message to be displayed to the user. </summary>
  private string _validationMessage;

  /// <summary> The validator used to validate the <see cref="_validatedControl"/>. </summary>
  private IValidator _validator;

  // construction and disposing

  /// <summary>
  ///   Initializes a new instance of the <see cref="ValidationError"/> class with the
  ///   <see cref="Control"/> containing invalid data and the <see cref=" IValidator"/>
  ///   used to identify the error.
  /// </summary>
  /// <overload> Overloaded. </overload>
  /// <param name="validatedControl"> The control with an invalid state. </param>
  /// <param name="validator"> The validator used to validate the <paramref name="validatedControl"/>. </param>
	public ValidationError (Control validatedControl, IValidator validator, ControlCollection labels)
	{
    ArgumentUtility.CheckNotNull ("validatedControl", validatedControl);
    ArgumentUtility.CheckNotNull ("validator", validator);

    _validatedControl = validatedControl;
    _validationMessage = null;
    _validator = validator;
    _labels = labels;
	}

  public ValidationError (Control validatedControl, IValidator validator)
    : this (validatedControl, validator, null)
	{
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="ValidationError"/> class with the
  ///   <see cref="Control"/> containing invalid data and the message describing the error.
  /// </summary>
  /// <overload> Overloaded. </overload>
  /// <param name="validatedControl"> The control with an invalid state. </param>
  /// <param name="validationMessage"> The message to be displayed to the user. </param>
  public ValidationError (Control validatedControl, string validationMessage, ControlCollection labels)
	{
    ArgumentUtility.CheckNotNull ("validatedControl", validatedControl);
    ArgumentUtility.CheckNotNull ("validationMessage", validationMessage);

    _validatedControl = validatedControl;
    _validationMessage = validationMessage;
    _validator = null;
    _labels = labels;
  }

  public ValidationError (Control validatedControl, string validationMessage)
    : this (validatedControl, validationMessage, null)
  {
  }

  // methods and properties

  /// <summary> Gets the control with an invalid state. </summary>
  /// <value> The validated <see cref="Control"/>. </value>
  public Control ValidatedControl
  {
    get { return _validatedControl; }
  }

  public ControlCollection Labels
  {
    get { return _labels; }
  }

  /// <summary> The message to be displayed to the user. </summary>
  /// <value> A string containing the message. </value>
  public string ValidationMessage
  {
    get 
    {
      if (_validationMessage == null)
        return _validator.ErrorMessage;
      else
        return _validationMessage; 
    }
  }

  /// <summary> Gets the validator used to validate the <see cref="ValidatedControl"/>. </summary>
  /// <value> A <see cref="IValidator"/> instance or <see langname="null" />. </value>
  public IValidator Validator
  {
    get { return _validator; }
  }

  /// <summary>
  ///   Formats the <c>ValidationError</c> as a <see cref="Label"/>
  ///   and associates the <see cref="ValidatedControl"/> with it.
  /// </summary>
  /// <remarks>
  ///   Be aware that the Internet Explorer resets DropDown controls
  ///   when jumped at through a label and does not create a postback event.
  /// </remarks>
  /// <param name="cssClass"> The name of the CSS-class used to format the label. </param>
  /// <returns> A <see cref="Label"/>. </returns>
  public HtmlGenericControl ToLabel (string cssClass)
  {
    HtmlGenericControl label = new HtmlGenericControl ("label");
    
    label.ID = ValidatedControl.ClientID + "_ValidationError_Label";
    label.InnerText = ValidationMessage;
    label.Attributes["for"] = ValidatedControl.ClientID;

    if (cssClass != null && cssClass != String.Empty)
      label.Attributes ["class"] = cssClass;

    return label;
  }

  /// <summary>
  ///   Formats the <c>ValidationError</c> as a <see cref="HyperLink"/>
  ///   and references the <see cref="ValidatedControl"/> through an in-page link.
  /// </summary>
  /// <param name="cssClass"> The name of the CSS-class used to format the hyperlink. </param>
  /// <returns> A <see cref="HyperLink"/>. </returns>
  public HyperLink ToHyperLink (string cssClass)
  {
    HyperLink hyperLink = new HyperLink();

    hyperLink.Text = ValidationMessage;
    hyperLink.NavigateUrl = "#" + ValidatedControl.ClientID;

    if (cssClass != null && cssClass != String.Empty)
      hyperLink.CssClass = cssClass;

    return hyperLink;
  }

  /// <summary>
  ///   Places the <c>ValidationError</c>'s message into a <c>div</c>
  ///   and returns this construct as a <see cref="LiteralControl"/>.
  /// </summary>
  /// <param name="cssClass"> The name of the CSS-class used to format the <c>div</c>-tag. </param>
  /// <returns> A <see cref="LiteralControl"/>. </returns>
  public LiteralControl ToDiv (string cssClass)
  {
    return ToLiteralControl (cssClass, "div");
  }

  /// <summary>
  ///   Places the <c>ValidationError</c>'s message into a <c>span</c>
  ///   and returns this construct as a <see cref="LiteralControl"/>.
  /// </summary>
  /// <param name="cssClass"> The name of the CSS-class used to format the <c>span</c>-tag. </param>
  /// <returns> A <see cref="LiteralControl"/>. </returns>
  public LiteralControl ToSpan (string cssClass)
  {
    return ToLiteralControl (cssClass, "span");
  }

  /// <summary>
  ///   Places the <c>ValidationError</c>'s message into a which ever HTML tag is provided
  ///   and returns this construct as a <see cref="LiteralControl"/>.
  /// </summary>
  /// <param name="cssClass"> The name of the CSS-class used to format the HTML tag. </param>
  /// <param name="tag"> The HTML tag to be used. </param>
  /// <returns> A <see cref="LiteralControl"/>. </returns>
  private LiteralControl ToLiteralControl (string cssClass, string tag)
  {
    //  Opening tag for validation message
    string validationMessage = "<" + tag + " class=\"" + cssClass + "\">";
    //  Message
    validationMessage += ValidationMessage;
    //  Closing tag for validation message
    validationMessage += "</" + tag + ">";

    LiteralControl literalControl = new LiteralControl();
    return new LiteralControl (validationMessage.ToString());
  }
}

}
