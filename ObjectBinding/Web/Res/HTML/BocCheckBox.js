//  BocCheckBox.js contains client side scripts used by BocCheckBox.

//  The descriptions used for the true, false, and null values
var _bocCheckBox_trueDescription;
var _bocCheckBox_falseDescription;

//  Initializes the strings used to represent the true, false and null values.
//  Call this method once in a startup script.
function BocCheckBox_InitializeGlobals (trueDescription, falseDescription)
{
  _bocCheckBox_trueDescription = trueDescription;
  _bocCheckBox_falseDescription = falseDescription;
}

// Toggle the value of the checkbox.
// checkBox: The check box.
// label: The label containing the description for the value. null for no description.
function BocCheckBox_ToggleCheckboxValue (checkBox, label, trueDescription, falseDescription)
{    
  checkBox.checked = !checkBox.checked;
  BocCheckBox_UpdateValue (checkBox, label, trueDescription, falseDescription);
}

function BocCheckBox_UpdateValue (checkBox, label, trueDescription, falseDescription)
{    
 // Update the controls
  var checkBoxToolTip;
  var labelText;
  
  if (checkBox.checked)
  {
    var description;
    if (trueDescription == null)
      description = _bocCheckBox_trueDescription;
    else
      description = trueDescription;
    checkBoxToolTip = description;
    labelText = description;
  }
  else
  {
    var description;
    if (falseDescription == null)
      description = _bocCheckBox_falseDescription;
    else
      description = falseDescription;
    checkBoxToolTip = description;
    labelText = description;
  }
  checkBox.title = checkBoxToolTip;
  if (label != null)
    label.innerHTML = labelText;
}

function BocCheckBox_OnKeyDown (context)
{
  if (event.keyCode == 32)
  {
    context.click();
    event.cancelBubble = true;
    event.returnValue = false;
  }
}