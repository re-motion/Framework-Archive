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
public class CurrencyTextBox: Control, INamingContainer
{
  // types

  public enum CurrencySymbolPositionType
  {
    BeforeTextBox = 0,
    AfterTextBox = 1
  }

  // static members and constants

  // member fields
  
  private string _currencySymbol = "€";
  private CurrencySymbolPositionType _currencySymbolPosition = CurrencySymbolPositionType.BeforeTextBox;
  private string _amountText = string.Empty;
  private TextBox _amountTextBox;
  private string _cssClass = string.Empty;

  private string _errorMessageCssClass = string.Empty;
  private string _errorMessage = "Bitte gültigen Betrag eingeben.";
  private ValidatorDisplay _errorMessageDisplay = ValidatorDisplay.Dynamic;

  // construction and disposing

  /// <summary>
  /// Creates a new Instance of CurrencyTextBox.
  /// </summary>
  public CurrencyTextBox()
  {
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
      if (_amountText != string.Empty)
        return decimal.Parse (_amountText.Trim ()); 
      else
        return decimal.MinValue;
    }

    set { _amountText = value.ToString (); }
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

  private string GetAmountToDisplay ()
  {
    string amount = this.Page.Request.Form[_amountTextBox.UniqueID];

    if (!this.Page.IsPostBack && (amount == null || amount == string.Empty))
      amount = _amountText;

    return amount;
  }

  protected override void CreateChildControls()
  {
    this.Controls.Add (CreateMainTable ());

    base.CreateChildControls ();

    _amountTextBox.Text = GetAmountToDisplay ();
    AmountText = GetAmountToDisplay ();
  }

  private HtmlTable CreateMainTable ()
  {
    HtmlTable table = new HtmlTable ();
    table.CellPadding = 0;
    table.CellSpacing = 0;

    if (_cssClass != string.Empty)
      table.Attributes["class"] = _cssClass;

    HtmlTableRow row = new HtmlTableRow ();
    HtmlTableCell cell = null;

    if (CurrencySymbolPosition == CurrencySymbolPositionType.BeforeTextBox)
      row.Cells.Add (CreateCurrencySymbolCell ());

    cell = new HtmlTableCell ();
    cell.VAlign = "Middle";

    CreateAmountTextBox ();
    cell.Controls.Add (_amountTextBox);
  
    row.Cells.Add (cell);

    if (CurrencySymbolPosition == CurrencySymbolPositionType.AfterTextBox)
      row.Cells.Add (CreateCurrencySymbolCell ());

    table.Rows.Add (row);
   
    row = new HtmlTableRow ();
    cell = new HtmlTableCell ();
    cell.ColSpan = 2;

    cell.Controls.Add (CreateValidator ());  

    row.Cells.Add (cell);

    table.Rows.Add (row);

    return table;
  }

  private void CreateAmountTextBox ()
  {
    _amountTextBox = new TextBox ();
    _amountTextBox.ID = "AmountTextBox";
    _amountTextBox.MaxLength = 13;
    _amountTextBox.Width = new Unit (10, UnitType.Em);
  }

  private HtmlTableCell CreateCurrencySymbolCell ()
  {
    HtmlTableCell cell = new HtmlTableCell ();
    cell.VAlign = "Middle";
    cell.Controls.Add (CreateCurrencySymbol ());

    return cell;
  }

  private Label CreateCurrencySymbol ()
  {
    Label currencySymbolLabel = new Label ();
    currencySymbolLabel.CssClass = "labelOpen";

    if (CurrencySymbolPosition == CurrencySymbolPositionType.AfterTextBox)
      currencySymbolLabel.Controls.Add (new LiteralControl ("&nbsp;"));

    currencySymbolLabel.Controls.Add (new LiteralControl (_currencySymbol));

    if (CurrencySymbolPosition == CurrencySymbolPositionType.BeforeTextBox)
      currencySymbolLabel.Controls.Add (new LiteralControl ("&nbsp;"));

    return currencySymbolLabel;
  }

  private RangeValidator CreateValidator ()
  {
    RangeValidator rangeValidator = new RangeValidator ();
    rangeValidator.ID = "AmountRangeValidator";
    rangeValidator.ErrorMessage = _errorMessage;
    rangeValidator.Display = _errorMessageDisplay;
    rangeValidator.MinimumValue = "0";
    rangeValidator.MaximumValue = "1200000000000";
    rangeValidator.Type = ValidationDataType.Double;
    rangeValidator.CssClass = _errorMessageCssClass;
    rangeValidator.EnableClientScript = false;
    rangeValidator.ControlToValidate = _amountTextBox.ID;

    return rangeValidator;
  }
}

}
