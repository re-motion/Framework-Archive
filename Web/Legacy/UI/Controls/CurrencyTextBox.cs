using System;
using System.Web.UI;
using System.Web.UI.WebControls;

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

  protected override void CreateChildControls()
  {
    if (CurrencySymbolPosition == CurrencySymbolPositionType.BeforeTextBox)
      CreateCurrencySymbol ();

    CreateAmountTextBox ();
   
    if (CurrencySymbolPosition == CurrencySymbolPositionType.AfterTextBox)
      CreateCurrencySymbol ();

    this.Controls.Add (new LiteralControl ("<br>"));

    CreateValidator ();    

    base.CreateChildControls ();
  }

  private void CreateAmountTextBox ()
  {
    _amountTextBox = new TextBox ();
    _amountTextBox.ID = "AmountTextBox";
    _amountTextBox.MaxLength = 13;
    _amountTextBox.Width = new Unit (10, UnitType.Em);
    this.Controls.Add (_amountTextBox);

    string amount = this.Page.Request.Form[_amountTextBox.UniqueID];

    if (!this.Page.IsPostBack && (amount == null || amount == string.Empty))
      amount = _amountText;
    
    _amountTextBox.Text = amount;
    AmountText = amount;
  }

  private void CreateCurrencySymbol ()
  {
    Label currencySymbolLabel = new Label ();
    currencySymbolLabel.CssClass = "labelOpen";

    if (CurrencySymbolPosition == CurrencySymbolPositionType.AfterTextBox)
      currencySymbolLabel.Controls.Add (new LiteralControl ("&nbsp;"));

    currencySymbolLabel.Controls.Add (new LiteralControl (_currencySymbol));

    if (CurrencySymbolPosition == CurrencySymbolPositionType.BeforeTextBox)
      currencySymbolLabel.Controls.Add (new LiteralControl ("&nbsp;"));

    this.Controls.Add (currencySymbolLabel);
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

    this.Controls.Add (_rangeValidator);
  }
}

}
