using System;
using System.Diagnostics;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Globalization;

using Rubicon.Globalization;
using Rubicon.Web.UI.Utilities;

namespace Rubicon.Web.UI.Controls
{

/// <summary>
/// Inserts line breaks in the value column.
/// </summary>
/// <remarks>
/// If the part after the break contains any white space, the resulting row will be at least as high
/// as the current font size. Avoid white space by attaching opening tags to the preceeding closing tags.
/// Example:
/// &lt;EntryFieldBreak /&gt;&lt;
/// SomeOtherTag&gt; ... &lt;/SomeOtherTag
/// &gt;&lt;/EntryField&gt;
/// </remarks>
public class EntryFieldBreak: Control
{
  protected override void Render (HtmlTextWriter writer)
  {
    EntryField parentField = (EntryField) this.Parent;
    writer.Write ("</td></tr><tr><td colspan=\"5\">{0}</td><td>{0}", 
      EntryFormGrid.GetWhitespaceImage (0, 0));
  }
}

[ParseChildren (false, "Controls")]
[ControlBuilder (typeof (EntryFormGridControlBuilder))]
public class EntryFormGrid: Control
{

  // member fields

  private Unit _labelColumnWidth;
  private Unit _width;
  private FontUnit _fieldFontSize;
  private string _infoBase = string.Empty;
  private string _infoImage = "field-info.gif";

  // methods and properties

  public string InfoImage 
  {
    get { return _infoImage; }
    set { _infoImage = value; }
  }

  public string InfoBase
  {
    get { return _infoBase; }
    set { _infoBase = value; }
  }

  public Unit LabelColumnWidth 
  {
    get { return _labelColumnWidth; }
    set { _labelColumnWidth = value; }
  }
	
  public Unit Width 
  {
    get { return _width; }
    set { _width = value; }
  }

  public FontUnit FieldFontSize
  {
    get { return _fieldFontSize; }
    set { _fieldFontSize = value; }
  }

  public bool FixedLayout
  {
    get { return (! _labelColumnWidth.IsEmpty) && (! _width.IsEmpty); }    
  }

  /// <summary>
  /// Validate all EntryFields
  /// </summary>
  /// <remarks>
  /// Calls Validate() on all EntryFields.
  /// </remarks>
  /// <param name="ignoreRequiredFieldValidators"> RequiredFieldValidators are not validated when
  /// this parameter is true.</param>
  /// <returns> Returns false if any EntryField is not valid. </returns>
  public bool Validate (bool ignoreRequiredFieldValidators, bool showErrors)
  {
    bool isValid = true;
    foreach (Control control in Controls)
    {
      EntryField field = control as EntryField;
      if (field != null && field.Visible)
      {
        if (! field.Validate (ignoreRequiredFieldValidators, showErrors))
          isValid = false;
      }
    }
    return isValid;
  }
  
  public bool Validate (bool ignoreRequiredFieldValidators)
  {
    return Validate (ignoreRequiredFieldValidators, true);
  }

  protected override void Render (HtmlTextWriter writer)
	{
    // adjust child control font sizes
    if (! FieldFontSize.IsEmpty)
    {
      ArrayList controls = ControlHelper.GetControlsRecursive (this, typeof (WebControl));
      foreach (WebControl control in controls)
      {
        if ((control is TextBox || control is DropDownList) && control.Font.Size.IsEmpty)
          control.Font.Size = FieldFontSize;
      }
    }

		if (this.Site != null && this.Site.DesignMode)
		{
			writer.WriteLine ("[EntryFormGrid - edit in HTML view]");
			return;
		}

    writer.WriteLine (
        "<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"width: {0}; {1}\">",
        Width.ToString (CultureInfo.InvariantCulture),
        FixedLayout ? "table-layout: fixed;" : string.Empty);

    if (FixedLayout)
    {
      writer.WriteLine ("<colgroup>");

      // label column
      if (!_labelColumnWidth.IsEmpty)
      {
        writer.WriteLine ("  <col style=\"width: {0}\">", 
            _labelColumnWidth.ToString (CultureInfo.InvariantCulture));
      }
      else
      {
        writer.WriteLine ("  <col>");
      }

      // seperator columns
      writer.WriteLine ("  <col style=\"width: 7\">");
      writer.WriteLine ("  <col style=\"width: 3\">");

      // indicator columns
      writer.WriteLine ("  <col style=\"width: 16\">");
      writer.WriteLine ("  <col style=\"width: 16\">");

      // value column
      writer.WriteLine ("  <col style=\"width: 100%\">");

      writer.WriteLine ("</colgroup>");
    }

		for (int i = 0; i < this.Controls.Count; ++i)
		{
			Control control = this.Controls[i];

			// write vertical empty space before titles
			if (i != 0 && (control is EntryTitle) && control.Visible)
				writer.WriteLine ("<tr><td colspan=\"6\">{0}</td></tr>", EntryFormGrid.GetWhitespaceImage (1, 10));

			control.RenderControl (writer);
		}

		writer.WriteLine ("</table>");
	}

  public static string GetImagePath (string imgFileName)
  {
    return "../images/" + imgFileName;
    //return PageUtility.GetPhysicalPageUrl (sourcePage, RelativeImagePath + imgFileName);
  }

  public string InfoImagePath 
  {
    get { return EntryFormGrid.GetImagePath (InfoImage); }
  }
	
  public static string GetWhitespaceImage (int width, int height)
  {
    // Specify at least an empty alt text to be HTML 4.0 conform (eGov Gütesiegel)
    return string.Format ("<img border=\"0\" width=\"{0}\" height=\"{1}\" src=\"{2}\" alt=\"\">", 
        width, height, EntryFormGrid.GetImagePath ("ws.gif"));
  }

  public static string GetWhitespaceImage (string width, string height)
  {
    // Specify at least an empty alt text to be HTML 4.0 conform (eGov Gütesiegel)
    return string.Format ("<img border=\"0\" width=\"{0}\" height=\"{1}\" src=\"{2}\" alt=\"\">", 
        width, height, EntryFormGrid.GetImagePath ("ws.gif"));
  }
}

public class EntryFormGridControlBuilder: ControlBuilder
{
	public override bool AllowWhitespaceLiterals ()
	{
		return false;
	}
	
	public override Type GetChildControlType (string tag, IDictionary attribs)
	{
		string unqualifiedTag = tag;
		int posColon = tag.IndexOf (':');
		if (posColon >= 0 && posColon < tag.Length + 1)
			unqualifiedTag = tag.Substring (posColon + 1);

		if (unqualifiedTag == "EntryField")
			return typeof (EntryField);
		if (unqualifiedTag == "EntryTitle")
			return typeof (EntryTitle);

    throw new ApplicationException ("Only EntryField and EntryTitle tags are allowed in EntryFormGrid controls");
	}
}

public class EntryTitle: Control
{
	private string _title = String.Empty;
  private string _padding = String.Empty;
  
  private int _colSpan = 6;

  public int ColSpan
  {
    get { return _colSpan; }
    set { _colSpan = value; }
  }

	public string Title 
	{
		get { return _title; }
		set { _title = value; }
	}
	public string Padding
	{
		get { return _padding; }
		set { _padding = value; }
	} 
	protected override void Render (HtmlTextWriter writer)
	{
    if (Padding != String.Empty)
    {
      writer.WriteLine ("<tr><td>{0}</td></tr>", EntryFormGrid.GetWhitespaceImage ("1", Padding));
    }

		writer.WriteLine ("<tr><td class=\"formGroup\" colspan=\"" + ColSpan + "\"> {0} </td></tr>", this.Title);
    if (this.Title != String.Empty)
    {
		  writer.WriteLine ("<tr><td colspan=\"" + ColSpan + "\"> "
          + "<table cellspacing=\"0\" cellpadding=\"0\" width=\"100%\"><tr><td class=\"formGroupSeparatorLine\" width=\"100%\">"
			  	+ EntryFormGrid.GetWhitespaceImage (1, 1)
          + "</td></tr></table>"
          + "</td></tr>");
    }

		writer.WriteLine ("<tr> <td>{0}</td> </tr>", EntryFormGrid.GetWhitespaceImage (1, 3));
	}
}

[ParseChildren (false)]
[MultiLingualResources ("Rubicon.Web.UI.Globalization.EntryFormGrid")] 
public class EntryField: Control
{
	private string _label = String.Empty;
	private string _for = String.Empty;
	private string _infoUrl = String.Empty;
	private bool _isRequired = false;
  private string _title = string.Empty;
  private bool   _showErrors = true;
  private bool   _externalValidState = true;
  private string _externalErrorMessage = string.Empty;

//  private int _height = -1;
//  private int _width = -1;

  public string Label 
	{
		get { return _label; }
		set { _label = value; }
	}
	public string For
	{
		get { return _for; }
		set { _for = value; }
	}
	public string InfoUrl
	{
		get { return _infoUrl; }
		set { _infoUrl = value; }
	}
	public bool IsRequired
	{
		get { return _isRequired; }
		set { _isRequired = value; }
	}
  public string Title
  {
    get { return _title; }
    set { _title = value; }
  }

  protected EntryFormGrid ParentGrid
  {
    get { return (EntryFormGrid) this.Parent; }
  }

  /// <summary>
  /// Validate all controls.
  /// </summary>
  /// <remarks>
  /// Validates all Validators.
  /// </remarks>
  /// <param name="ignoreRequiredFieldValidators"> RequiredFieldValidators and other validators whose IDs
  /// end with "...RequiredValidator" are not validated when this parameter is true.</param>
  /// <returns> Returns false if any Validator is not valid. </returns>
  public bool Validate (bool ignoreRequiredFieldValidators, bool showErrors)
  {
    bool isValid = true;
    foreach (Control control in Controls)
    {
      BaseValidator validator = control as BaseValidator;

      if (validator == null)
      {
        if( !ValidateSubControls (control, ignoreRequiredFieldValidators) )
          isValid = false;
      }
      else
      {
        if( !ExecuteValidation (validator, ignoreRequiredFieldValidators) )
        {
          isValid = false;
        }
      }
    }

    _showErrors = showErrors;
    return isValid && _externalValidState;
  }

  /// <summary>
  /// Set an external validation state.
  /// </summary>
  /// <remarks>
  /// The external validation state indicates errors which can't be validated by the validators.
  /// </remarks>
  /// <param name="valid">Validation state</param>
  /// <param name="errorMessage">Associated error message</param>
  public void SetExternalValidState (bool valid, string errorMessage)
  {
    _externalValidState = valid;
    _externalErrorMessage = errorMessage;
  }

  private bool ValidateSubControls (Control control, bool ignoreRequiredFieldValidators)
  {
    //  check if given control is a user control

    Control userControl = control as Control;

    if (userControl == null)
      return true;

    bool isValid = true;

    foreach (Control subControl in userControl.Controls)
    {
      BaseValidator validator = subControl as BaseValidator;

      if (validator == null)
      {
        if( !ValidateSubControls (subControl, ignoreRequiredFieldValidators) )
          isValid = false;
      }
      else
        if ( !ExecuteValidation (validator, ignoreRequiredFieldValidators) )
          isValid = false;
    }

    return isValid;
  }


  private bool ExecuteValidation (BaseValidator validator, bool ignoreRequiredFieldValidators)
  {
    bool isValid = true;

    if (   validator != null)
    {
      if (! (      ignoreRequiredFieldValidators 
              && (    validator is RequiredFieldValidator
                   || validator.ID.EndsWith ("RequiredValidator"))))
      {
        validator.Validate();
        if (! validator.IsValid)
          isValid = false;
      }
    }

    return isValid;
  }


  public bool Validate (bool ignoreRequiredFieldValidators)
  {
    return Validate (ignoreRequiredFieldValidators, true);
  }

  protected override void Render (HtmlTextWriter writer)
	{
		string label;
    string clientId = string.Empty;
    string validatorMessages = string.Empty ;
    bool validatorsInvalid = false;

    // search for Validator child controls and keep if at least one is invalid
    CheckExternalValidState (ref validatorMessages, ref validatorsInvalid);
    CheckForInvalidValidators (this.Controls, ref validatorMessages, ref validatorsInvalid);

    Control labeledControl = null;
    if (For != null && For != string.Empty)
      labeledControl = this.FindControl (For);

    if (labeledControl != null)
    {
      // WORKAROUND: prevent the following bug in IE 6.0: clicking on the label of a <select> control
      // selects the first item in the list (but does not fire a changed-event)
      if (! (   labeledControl is System.Web.UI.WebControls.DropDownList 
             || labeledControl is System.Web.UI.HtmlControls.HtmlSelect))
        clientId = labeledControl.ClientID;
    }

    string labelTitleAttribute = string.Empty;
    if (Title != string.Empty)
      labelTitleAttribute = string.Format ("title=\"{0}\"", Title);

    if (clientId == String.Empty)
      label = string.Format ("<span {0}>{1}</span>", labelTitleAttribute, Label);
		else
      label = string.Format ("<label for=\"{0}\" {1}>{2}</label>", clientId, labelTitleAttribute, Label);

    string labelWidthAttribute = string.Empty;
    string valueWidthAttribute = string.Empty;
    EntryFormGrid parentGrid = Parent as EntryFormGrid;
    if (parentGrid != null && ! parentGrid.FixedLayout)
    {
      if (! parentGrid.LabelColumnWidth.IsEmpty)
        labelWidthAttribute = string.Format ("style=\"width: {0};\"", parentGrid.LabelColumnWidth.ToString(CultureInfo.InvariantCulture));
    }

    string tagStyle = string.Empty;
    if (! Visible)
      tagStyle = "style=\"display:none\"";

		writer.WriteLine ("<tr valign=\"middle\" {0}>", tagStyle);
		writer.WriteLine ("<td class=\"label\" align=\"right\" {0} >{1}</td>", 
        labelWidthAttribute, label);
		writer.WriteLine ("<td class=\"label\">{0}</td>", EntryFormGrid.GetWhitespaceImage (7, 1));
		writer.WriteLine ("<td>{0}</td>", EntryFormGrid.GetWhitespaceImage (3, 1));
		writer.WriteLine ("<td nowrap>");

    if (validatorsInvalid)
    {
      // at least one Validator is invalid
      // => display "invalid field indicator" which has higher priority than the "required field indicator"

      writer.WriteLine (UIUtility.GetIconImage (
          Page,
          validatorMessages, 
          EntryFormGrid.GetImagePath ("field-error.gif")));
    }
    else
    {
      // all Validators are valid
      // => display the "required field indicator" if requested
      if (this.IsRequired)
      {
        writer.WriteLine (UIUtility.GetIconImage (
            Page,
            ResourceManagerPool.GetResourceText (this, "RequiredFieldText"), 
            EntryFormGrid.GetImagePath ("field-required.gif")));
      }
      else
      {
        writer.WriteLine (EntryFormGrid.GetWhitespaceImage (12, 20));
      }
    }

    writer.WriteLine ("</td><td>");
    
		if (this.InfoUrl != String.Empty)
		{
      string infoUrl = parentGrid.InfoBase + this.InfoUrl;
			writer.WriteLine (
					"<a href=\"{0}\" target=\"_new\">"
						+ "<img src=\"{1}\" alt=\"{2}\""
						+ "width=\"15\" height=\"20\" border=\"0\"/></a>",
					infoUrl,
          ParentGrid.InfoImagePath,
          ResourceManagerPool.GetResourceText (this, "HelpInfoText"));
    }
		else
		{
			writer.WriteLine (EntryFormGrid.GetWhitespaceImage (12, 20));
		}

		writer.WriteLine ("</td><td {0}>", valueWidthAttribute);

    RenderChildren(writer);

		writer.Write ("</td>");
		writer.WriteLine ("</tr>");
		writer.WriteLine ("<tr> <td colspan=\"6\">{0}</td> </tr>", EntryFormGrid.GetWhitespaceImage (1, 1));
	}

  protected void CheckExternalValidState (ref string errorMessages, ref bool invalid)
  {
    if (_externalValidState == false)
    {
      invalid = true;
      errorMessages = AddErrorMessage (errorMessages, _externalErrorMessage);
    }
  }

  protected void CheckForInvalidValidators(ControlCollection controls, ref string validatorMessages, ref bool validatorsInvalid)
  {
    // search for Validator child controls and keep if at least one is invalid

    foreach ( Control childControl in controls )
    {
      if ( childControl is BaseValidator && _showErrors)
      {
        CheckInvalidValidator (childControl, ref validatorMessages, ref validatorsInvalid);
      }
      else
      {
        Control subControl = childControl as Control;

        if ( (subControl != null) && (subControl.Controls != null) )
          CheckForInvalidValidators (subControl.Controls, ref validatorMessages, ref validatorsInvalid);
      }
    }
  }
	

  private void CheckInvalidValidator(Control control, ref string validatorMessages, ref bool validatorsInvalid)
  {
    BaseValidator validator = control as BaseValidator;

    if ( (validator != null) && (!validator.IsValid) )
    {
      validatorsInvalid = true;
      validatorMessages = AddErrorMessage (validatorMessages, validator.ErrorMessage);
    }
  }

  private string AddErrorMessage (string errorMessages, string newErrorMessage)
  {
    if (errorMessages.Length > 0)
      errorMessages += "\r\n";

    return errorMessages + newErrorMessage;
  }
}

}