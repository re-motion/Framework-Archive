using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using Rubicon;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Controls
{
/// <summary>
///   Encapsulats an validation error: the affected control, the message and the validator used.
/// </summary>
public class ValidationError
{
  // constants

  // types

  // static members

  // member fields

  /// <summary>
  ///   The control with an invalid state.
  /// </summary>
  private Control _validatedControl;

  /// <summary>
  ///   The message to be displayed to the user.
  /// </summary>
  private string _validationMessage;

  /// <summary>
  ///   The validator used to validate the <see cref="_validatedControl"/>.
  /// </summary>
  private BaseValidator _validator;

  // construction and disposing

  /// <summary>
  ///   Simple constructor
  /// </summary>
  /// <overload>Overloaded</overload>
  /// <param name="validatedControl">The control with an invalid state.</param>
  /// <param name="validationMessage">The message to be displayed to the user.</param>
  /// <param name="validator">The validator used to validate the <paramref name="validatedControl"/>.</param>
	public ValidationError (
    Control validatedControl,
    string validationMessage,
    BaseValidator validator)
	{
    ArgumentUtility.CheckNotNull ("validatedControl", validatedControl);
    ArgumentUtility.CheckNotNull ("validationMessage", validationMessage);

    _validatedControl = validatedControl;
    _validationMessage = validationMessage;
    _validator = validator;
	}

  /// <summary>
  ///   Simple constructor
  /// </summary>
  /// <overload>Overloaded</overload>
  /// <param name="validatedControl">The control with an invalid state.</param>
  /// <param name="validationMessage">The message to be displayed to the user.</param>
  public ValidationError (Control validatedControl, string validationMessage)
    :this (validatedControl, validationMessage, null)
	{}

  // methods and properties

  /// <summary>
  ///   The control with an invalid state.
  /// </summary>
  /// <value>The validated <see cref="Control"/>.</value>
  public Control ValidatedControl
  {
    get { return _validatedControl; }
  }

  /// <summary>
  ///   The message to be displayed to the user.
  /// </summary>
  /// <value>A string containing the message</value>
  public string ValidationMessage
  {
    get { return _validationMessage; }
  }

  /// <summary>
  ///   The validator used to validate the <see cref="ValidatedControl"/>.
  /// </summary>
  /// <value>A <see cref="BaseValidator"/> instance or <see langname="null" /></value>
  public BaseValidator Validator
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
  /// <param name="cssClass">The name of the CSS-class used to format the label</param>
  /// <returns>A <see cref="Label"/></returns>
  public Label ToLabel (string cssClass)
  {
    Label label = new Label();

    label.Text = this.ValidationMessage;
    label.AssociatedControlID = this.ValidatedControl.ID;

    if (cssClass != null && cssClass != String.Empty)
      label.CssClass = cssClass;

    return label;
  }

  /// <summary>
  ///   Formats the <c>ValidationError</c> as a <see cref="HyperLink"/>
  ///   and references the <see cref="ValidatedControl"/> through an in-page link.
  /// </summary>
  /// <param name="cssClass">The name of the CSS-class used to format the hyperlink.</param>
  /// <returns>A <see cref="HyperLink"/></returns>
  public HyperLink ToHyperLink (string cssClass)
  {
    HyperLink hyperLink = new HyperLink();

    hyperLink.Text = this.ValidationMessage;
    hyperLink.NavigateUrl = "#" + this.ValidatedControl.ID;

    if (cssClass != null && cssClass != String.Empty)
      hyperLink.CssClass = cssClass;

    return hyperLink;
  }

  /// <summary>
  ///   Places the <c>ValidationError</c>'s message into a <c>div</c>
  ///   and returns this construct as a <see cref="LiteralControl"/>.
  /// </summary>
  /// <param name="cssClass">The name of the CSS-class used to format the <c>div</c>-tag.</param>
  /// <returns>A <see cref="LiteralControl"/></returns>
  public LiteralControl ToDiv (string cssClass)
  {
    return ToLiteralControl (cssClass, "div");
  }

  /// <summary>
  ///   Places the <c>ValidationError</c>'s message into a <c>span</c>
  ///   and returns this construct as a <see cref="LiteralControl"/>.
  /// </summary>
  /// <param name="cssClass">The name of the CSS-class used to format the <c>span</c>-tag.</param>
  /// <returns>A <see cref="LiteralControl"/></returns>
  public LiteralControl ToSpan (string cssClass)
  {
    return ToLiteralControl (cssClass, "span");
  }

  /// <summary>
  ///   Places the <c>ValidationError</c>'s message into a which ever HTML tag is provided
  ///   and returns this construct as a <see cref="LiteralControl"/>.
  /// </summary>
  /// <param name="cssClass">The name of the CSS-class used to format the HTML tag.</param>
  /// <param name="tag">The HTML tag to be used</param>
  /// <returns>A <see cref="LiteralControl"/></returns>
  private LiteralControl ToLiteralControl (string cssClass, string tag)
  {
    LiteralControl literalControl = new LiteralControl();

    StringBuilder validationMessage = new StringBuilder (100);

    //  Opening tag for validation message
    validationMessage.AppendFormat ("<{0} class=\"", tag);
    validationMessage.Append (cssClass);
    validationMessage.Append ("\">");

    //  Message
    validationMessage.Append (this.ValidationMessage);

    //  Closing tag for validation message
    validationMessage.AppendFormat ("</{0}>", tag);

    return new LiteralControl (validationMessage.ToString());
  }
}

}
