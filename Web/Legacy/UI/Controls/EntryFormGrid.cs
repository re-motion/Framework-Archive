using System;
using System.Diagnostics;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

namespace Rubicon.Findit.Client.Controls
{

public class EntryFieldBreak: Control
{
  protected override void Render (HtmlTextWriter writer)
  {
    writer.Write ("</td></tr><tr><td colspan=\"4\"></td><td>");
  }
}
  
  

[ParseChildren (false, "Controls")]
[ControlBuilder (typeof (EntryFormGridControlBuilder))]
public class EntryFormGrid: Control
{
  private Unit _labelColumnWidth;
  private Unit _valueColumnWidth;
  private FontUnit _fieldFontSize;

  public Unit LabelColumnWidth 
  {
    get { return _labelColumnWidth; }
    set { _labelColumnWidth = value; }
  }
	
  public Unit ValueColumnWidth 
  {
    get { return _valueColumnWidth; }
    set { _valueColumnWidth = value; }
  }

  public FontUnit FieldFontSize
  {
    get { return _fieldFontSize; }
    set { _fieldFontSize = value; }
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

    writer.WriteLine ("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");

		for (int i = 0; i < this.Controls.Count; ++i)
		{
			Control control = this.Controls[i];

			// write vertical empty space before titles
			if (i != 0 && control is EntryTitle)
				writer.WriteLine ("<tr><td> <img src=\"../Images/ws.gif\" height=\"10\" width=\"1\"></td></tr>");

			control.RenderControl(writer);
		}

		writer.WriteLine ("</table>");
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
      writer.WriteLine ("<tr><td><img src=\"../Images/ws.gif\" width=\"1\" height=\"{0}\"/></td></tr>",
          Padding);
    }

		writer.WriteLine ("<tr><td class=\"formGroup\" colspan=\"" + ColSpan + "\"> {0} </td></tr>", this.Title);
    if (this.Title != String.Empty)
    {
		  writer.WriteLine ("<tr><td colspan=\"" + ColSpan + "\"> "
          + "<table bgcolor=\"black\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\"><tr><td width=\"100%\">"
			  	+ "<img src=\"../Images/ws.gif\" height=\"2\" width=\"1\">"
          + "</td></tr></table>"
          + "</td></tr>");
    }

		writer.WriteLine ("<tr> <td><img height=\"3\" width=\"1\" src=\"../Images/ws.gif\"/></td> </tr>");
	}
}

[ParseChildren (false)]
public class EntryField: Control
{
	private string _label = String.Empty;
	private string _for = String.Empty;
	private string _infoUrl = String.Empty;
	private bool _isRequired = false;
  private string _title = string.Empty;

  private bool   _showErrors = true;

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
        if( !ExecuteValidation (validator, ignoreRequiredFieldValidators) )
      {
        isValid = false;
      }
    }

    _showErrors = showErrors;
    return isValid;
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

    if (   validator != null
        && ! (    ignoreRequiredFieldValidators 
                && (    validator is RequiredFieldValidator
                    || validator.ID.EndsWith ("RequiredValidator"))))
    {
      validator.Validate();
      if (! validator.IsValid)
        isValid = false;
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
    string clientId = String.Empty;
    string validatorMessages = String.Empty ;
    bool validatorsInvalid = false;

    // search for Validator child controls and keep if at least one is invalid

    CheckForInvalidValidators (this.Controls, ref validatorMessages, ref validatorsInvalid);

    Control labeledControl = null;
    if (For == String.Empty)
    {
      if (this.Controls.Count >= 2)
        labeledControl = this.Controls[1];
    }
    else
    {
      labeledControl = this.FindControl (For);
    }

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
      label = string.Format ("<div {0}>&nbsp;{1}</div>", labelTitleAttribute, Label);
		else
      label = string.Format ("<label for=\"{0}\" {1}>&nbsp;{2}</label>", clientId, labelTitleAttribute, Label);

    string labelWidthAttribute = string.Empty;
    string valueWidthAttribute = string.Empty;
    EntryFormGrid parentGrid = Parent as EntryFormGrid;
    if (parentGrid != null)
    {
      if (! parentGrid.LabelColumnWidth.IsEmpty)
        labelWidthAttribute = string.Format ("style=\"width: {0};\"", parentGrid.LabelColumnWidth.ToString(System.Globalization.CultureInfo.InvariantCulture));
      if (! parentGrid.ValueColumnWidth.IsEmpty)
        valueWidthAttribute = string.Format ("style=\"width: {0};\"", parentGrid.ValueColumnWidth.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }

    string tagStyle = string.Empty;
    if (! Visible)
      tagStyle = " style=\"display:none\"";

		writer.WriteLine ("<tr{0}>", tagStyle);
		writer.WriteLine ("<td class=\"label\" valign=\"center\" align=\"right\" {0} >{1}</td>", 
        labelWidthAttribute, label);
		writer.WriteLine ("<td class=\"label\"><img height=\"1\" width=\"7\" src=\"../Images/ws.gif\"/></td>");
		writer.WriteLine ("<td><img height=\"1\" width=\"3\" src=\"../Images/ws.gif\"/></td>");
		writer.WriteLine ("<td nowrap>");

    if (validatorsInvalid)
    {
      // at least one Validator is invalid
      // => display "invalid field indicator" which has higher priority than the "required field indicator"
      writer.WriteLine ("<img src=\"../Images/field-error.gif\" alt=\"" + validatorMessages + "\""
        + "width=\"12\" height=\"20\" border=\"0\"/>");
    }
    else
    {
      // all Validators are valid
      // => display the "required field indicator" if requested
      if (this.IsRequired)
      {
        writer.WriteLine ("<img src=\"../Images/field-required.gif\" alt=\"Dieses Feld muss ausgef&uuml;llt werden\" "
          + "width=\"12\" height=\"20\" border=\"0\"/>");
      }
      else
      {
        writer.WriteLine ("<img src=\"../Images/ws.gif\" width=\"12\" height=\"20\" border=\"0\" />");
      }
    }
    
		if (this.InfoUrl != String.Empty)
		{
			writer.WriteLine (
					"<a href=\"{0}\" target=\"_new\">"
						+ "<img src=\"../Images/field-info.gif\" alt=\"Hilfe zum Ausf�llen per Mausklick\""
						+ "width=\"15\" height=\"20\" border=\"0\"/></a>",
					this.InfoUrl);
    }
		else
		{
			writer.WriteLine ("<img src=\"../Images/ws.gif\" width=\"12\" height=\"20\" border=\"0\" />");
		}

		writer.WriteLine ("</td><td {0}>", valueWidthAttribute);

    RenderChildren(writer);

		writer.WriteLine ("</td>");
		writer.WriteLine ("</tr>");
		writer.WriteLine ("<tr> <td><img height=\"1\" width=\"1\" src=\"../Images/ws.gif\"/></td> </tr>");
	}

  private void CheckForInvalidValidators(ControlCollection controls, ref string validatorMessages, ref bool validatorsInvalid)
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
  
      if ( validatorMessages.Length > 0 )
        validatorMessages += "\r\n";
  
      validatorMessages += validator.ErrorMessage;
    }
  }
}

}