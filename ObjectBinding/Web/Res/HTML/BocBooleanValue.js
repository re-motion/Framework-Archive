var _trueValue;
var _falseValue;
var _nullValue;

function SetValues (trueValue, falseValue, nullValue)
{
  _trueValue = trueValue;
  _falseValue = falseValue;
  _nullValue = nullValue;
}

function SelectNextCheckboxValue (checkBox, label, hidden, isRequired)
{
  var oldValue = document.all['hidden'].value;
  var newValue;
  
  if (isRequired)
  {
    if (oldValue == falseValue)
      newValue = nullValue + "';
    else if (oldValue == nullValue)
      newValue = trueValue;
    else
      newValue = falseValue;
  }
  else
  {
    if (oldValue == falseValue)
      newValue = trueValue;
    else
      newValue = falseValue;
  }    
}
