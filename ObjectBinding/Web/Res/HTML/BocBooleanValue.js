//  BocBooleanValue.js contains client side scripts used by BocBooleanValue.

//  The string representation of the true, false, and null values.
var _bocBooleanValue_trueValue;
var _bocBooleanValue_falseValue;
var _bocBooleanValue_nullValue;

//  The descriptions used for the true, false, and null values
var _bocBooleanValue_trueDescription;
var _bocBooleanValue_falseDescription;
var _bocBooleanValue_nullDescription;

//  The descriptions used to represent the true, false, and null values
var _bocBooleanValue_trueIconUrl;
var _bocBooleanValue_falseIconUrl;
var _bocBooleanValue_nullIconUrl;

//  Initializes the strings used to represent the true, false and null values.
//  Call this method once in a startup script.
function BocBooleanValue_InitializeGlobals (
  trueValue, 
  falseValue, 
  nullValue, 
  trueDescription,
  falseDescription,
  nullDescription,
  trueIconUrl, 
  falseIconUrl, 
  nullIconUrl)
{
  _bocBooleanValue_trueValue = trueValue;
  _bocBooleanValue_falseValue = falseValue;
  _bocBooleanValue_nullValue = nullValue;

  _bocBooleanValue_trueDescription = trueDescription;
  _bocBooleanValue_falseDescription = falseDescription;
  _bocBooleanValue_nullDescription = nullDescription;

  _bocBooleanValue_trueIconUrl = trueIconUrl;
  _bocBooleanValue_falseIconUrl = falseIconUrl;
  _bocBooleanValue_nullIconUrl = nullIconUrl;
}

// Selected the next value of the tri-state checkbox, skipping the null value if isRequired is true.
// icon: The icon representing the tri-state checkbox.
// label: The label containing the description for the value. null for no description.
// hiddenField: The hidden input field used to store the value between postbacks.
// isRequired: true to enqable the null value, false to limit the choices to true and false.
function BocBooleanValue_SelectNextCheckboxValue (icon, label, hiddenField, isRequired)
{
  var trueValue = _bocBooleanValue_trueValue;
  var falseValue = _bocBooleanValue_falseValue;
  var nullValue = _bocBooleanValue_nullValue;
    
  var oldValue = hiddenField.value;
  var newValue;
  
  //  Select the next value.
  //  true -> false -> null -> true
  if (isRequired)
  {
    if (oldValue == falseValue)
      newValue = trueValue;
    else
      newValue = falseValue;
  }   
  else
  {
    if (oldValue == falseValue)
      newValue = nullValue;
    else if (oldValue == nullValue)
      newValue = trueValue;
    else
      newValue = falseValue;
  }
 
 // Update the controls
  hiddenField.value = newValue;
  if (newValue == falseValue)
  {
    icon.src = _bocBooleanValue_falseIconUrl;
    icon.alt = _bocBooleanValue_falseDescription;
    if (label != null)
      label.innerText = _bocBooleanValue_falseDescription;
  }
  else if (newValue == nullValue)
  {
    icon.src = _bocBooleanValue_nullIconUrl;
    icon.alt = _bocBooleanValue_nullDescription;
    if (label != null)
      label.innerText = _bocBooleanValue_nullDescription;
  }
  else if (newValue == trueValue)
  {
    icon.src = _bocBooleanValue_trueIconUrl;
    icon.alt = _bocBooleanValue_trueDescription;
    if (label != null)
      label.innerText = _bocBooleanValue_trueDescription;
  }
}
