using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Rubicon.Web.UI.Controls
{
/// <summary>
/// Control which renders a TextBox control and a currency symbol (description).
/// </summary>
/// <remarks>
/// Default currency symbol is € (Euro).
/// Default position is before the text box.
/// </remarks>
[ValidationPropertyAttribute("AmountText")]
public class CurrencyTextBox: Control, INamingContainer, IPostBackDataHandler
{
  // types

  public enum CurrencySymbolPositionType
  {
    BeforeTextBox = 0,
    AfterTextBox = 1
  }

  public delegate void TextChangedHandler ();
  public event TextChangedHandler TextChanged;

  // static members and constants

  // member fields
  
  private string _currencySymbol = "€";
  private CurrencySymbolPositionType _currencySymbolPosition = CurrencySymbolPositionType.BeforeTextBox;
  private string _amountText = string.Empty;
  private TextBox _amountTextBox;
  private string _cssClass = string.Empty;

  private RangeValidator _rangeValidator;
  private string _errorMessageCssClass = string.Empty;
  private string _errorMessage = "Bitte gültigen Betrag eingeben.";
  private ValidatorDisplay _errorMessageDisplay = ValidatorDisplay.Dynamic;

  // construction and disposing

  /// <summary>
  /// Creates a new Instance of CurrencyTextBox.
  /// </summary>
  public CurrencyTextBox()
  {
    CreateAmountTextBox ();
  }

  // abstract methods and properties 

  // methods and properties

  /// <summary>
  /// Gets or sets the currency symbol.
  /// </summary>
  public string CurrencySymbol
  {
    get { return _currencySymbol; }
    set { _currencySymbol = value; }
  }

  /// <summary>
  /// Gets or sets the position of the currency symbol.
  /// </summary>
  public CurrencySymbolPositionType CurrencySymbolPosition
  {
    get { return _currencySymbolPosition; }
    set { _currencySymbolPosition = value; }
  }

  /// <summary>
  /// Gets wether the Textbox is empty.
  /// </summary>
  public bool IsEmpty
  {
    get { return (_amountText == string.Empty); }
  }

  /// <summary>
  /// Gets or sets the textual amount.
  /// </summary>
  public string AmountText
  {
    get { return _amountText; }
    set { _amountText = value; }
  }

  /// <summary>
  /// Gets or sets the decimal amount.
  /// </summary>
  public decimal Amount
  {
    get 
    { 
      decimal amount = 0;

      try
      {
        amount = decimal.Parse (_amountText);
      }
      catch
      {
      }

      return amount; 
    }

    set { _amountText = value.ToString (); }
  }

  /// <summary>
  /// Gets or sets wether the control is enabled
  /// </summary>
  public bool Enabled
  {
    get { return _amountTextBox.Enabled; }
    set { _amountTextBox.Enabled = value; }
  }
 
  /// <summary>
  /// Gets or sets the css-class
  /// </summary>
  public string CssClass
  {
    get { return _cssClass; }
    set { _cssClass = value; }
  }

  /// <summary>
  /// Gets or sets the css-class for the error message.
  /// </summary>
  public string ErrorMessageCssClass
  {
    get { return _errorMessageCssClass; }
    set { _errorMessageCssClass = value; }
  }

  /// <summary>
  /// Gets or sets the error message.
  /// </summary>
  public string ErrorMessage
  {
    get { return _errorMessage; }
    set { _errorMessage = value; }
  }

  /// <summary>
  /// Gets or sets how the errorMessage should be displayed.
  /// </summary>
  public ValidatorDisplay ErrorMessageDisplay
  {
    get { return _errorMessageDisplay; }
    set { _errorMessageDisplay = value; }
  }

  protected override void LoadViewState (object savedState)
  {
    object[] values = (object[]) savedState;
    base.LoadViewState (values[0]);
    _amountText = (string) values[1];

    if (_amountText == null)
      _amountText = string.Empty;
  }

  protected override object SaveViewState ()
  {
    object[] values = new object[4];
    values[0] = base.SaveViewState ();
    values[1] = _amountText;
    return values;
  }

  protected override void OnInit(EventArgs e)
  {
    EnsureChildControls ();

    base.OnInit (e);
  }

  protected override void CreateChildControls()
  {
    //this.Controls.Clear ();

    CreateAndAppendMainTable ();

    base.CreateChildControls ();
  }

  protected override void OnLoad(EventArgs e)
  {
    // Display amount for validators
    DisplayAmount ();

    base.OnLoad (e);
  }

  protected override void OnPreRender(EventArgs e)
  {
    DisplayAmount ();

    base.OnPreRender (e);
  }


  private void DisplayAmount ()
  {
    //if (Amount > 0)
    //  _amountTextBox.Text = Amount.ToString ("n");
    //else
      _amountTextBox.Text = _amountText;
  }

  protected override void Render(HtmlTextWriter writer)
  {
    // Render dummy input control with the id of the CurrencyTextBox-control to
    //  be able to handle posted data
    writer.Write("<input type=\"text\" style=\"display:none;\" name=\"" + this.UniqueID + "\">");
    
    base.Render (writer);
  }

  private void CreateAndAppendMainTable ()
  {
    Table table = new Table ();
    table.CellPadding = 0;
    table.CellSpacing = 0;

    if (_cssClass != string.Empty)
      table.CssClass = _cssClass;

    TableRow row = new TableRow ();
    TableCell cell = null;

    if (CurrencySymbolPosition == CurrencySymbolPositionType.BeforeTextBox)
      row.Cells.Add (CreateCurrencySymbolCell ());

    cell = new TableCell ();
    cell.VerticalAlign = VerticalAlign.Middle;
    cell.HorizontalAlign = HorizontalAlign.Left;

    cell.Controls.Add (_amountTextBox);
  
    row.Cells.Add (cell);

    if (CurrencySymbolPosition == CurrencySymbolPositionType.AfterTextBox)
      row.Cells.Add (CreateCurrencySymbolCell ());

    table.Rows.Add (row);

    row = new TableRow ();
    cell = new TableCell ();
    cell.ColumnSpan = 2;

    CreateValidator ();
    cell.Controls.Add (_rangeValidator);  

    row.Cells.Add (cell);

    table.Rows.Add (row);
   
    this.Controls.Add (table);
  }

  private void CreateAmountTextBox ()
  {
    _amountTextBox = new TextBox ();
    _amountTextBox.ID = "AmountTextBox";
    _amountTextBox.MaxLength = 13;

    if (_cssClass != string.Empty)
      _amountTextBox.CssClass = _cssClass;

    _amountTextBox.Style["width"] = "10em";
    _amountTextBox.Style["text-align"] = "right";
  }

  private TableCell CreateCurrencySymbolCell ()
  {
    TableCell cell = new TableCell ();
    cell.VerticalAlign = VerticalAlign.Middle;
    cell.Width = new Unit (1, UnitType.Percentage);
    cell.Controls.Add (CreateCurrencySymbol ());

    return cell;
  }

  private Label CreateCurrencySymbol ()
  {
    Label currencySymbolLabel = new Label ();

    if (CurrencySymbolPosition == CurrencySymbolPositionType.AfterTextBox)
      currencySymbolLabel.Controls.Add (new LiteralControl ("&nbsp;"));

    currencySymbolLabel.Controls.Add (new LiteralControl (_currencySymbol));

    if (CurrencySymbolPosition == CurrencySymbolPositionType.BeforeTextBox)
      currencySymbolLabel.Controls.Add (new LiteralControl ("&nbsp;"));

    return currencySymbolLabel;
  }

  private void CreateValidator ()
  {
    _rangeValidator = new RangeValidator ();
    _rangeValidator.ID = "AmountRangeValidator";
    _rangeValidator.ErrorMessage = _errorMessage;
    _rangeValidator.Display = _errorMessageDisplay;
    _rangeValidator.MinimumValue = "0";
    _rangeValidator.MaximumValue = "1200000000000";
    _rangeValidator.Type = ValidationDataType.Double;
    _rangeValidator.CssClass = _errorMessageCssClass;
    _rangeValidator.EnableClientScript = false;
    _rangeValidator.ControlToValidate = _amountTextBox.ID;
  }

  #region IPostBackDataHandler Members

  public void RaisePostDataChangedEvent()
  {
    OnTextChanged ();
  }

  public bool LoadPostData (string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
  {
    string currentValue = _amountText;

    string postedValue = postCollection[_amountTextBox.UniqueID];

    if (!currentValue.Equals (postedValue))
    {
      _amountText = postedValue;
      return true;
    }

    return false;
  }

  #endregion

  protected virtual void OnTextChanged ()
  {
    if (TextChanged != null)
      TextChanged ();
  }
}

}
