//  DatePicker.js contains client side scripts used by DatePickerPage 
//  and the caller of the DatePickerFrom.aspx IFrame contents page.

//  The currently displayed date picker
//  Belongs to the parent page.
var _datePicker_currentDatePicker = null;

//  Helper variable for event handling.
//  Belongs to the parent page.
//  The click event of the document fires after the methods bound to the button click have been 
//  executed. _datePicker_isEventAfterDatePickerButtonClick is used to identify those click events fired
//  because a date picker button had been clicked in contrast to events fired
//  beause of a click somewhere on the page.
var _datePicker_isEventAfterDatePickerButtonClick = false;

//  Shows the date picker frame below the button.
//  Belongs to the parent page.
//  button: The button that opened the date picker frame.
//  container: The page element containing the properties to be passed to the picker.
//  target: The input element receiving the value returned by the date picker.
//  frame: The IFrame containing the date picker that will be shown.
function DatePicker_ShowDatePicker (button, container, target, frame)
{
  //  Tried to open the already open date picker?
  //  Close it and return.
  if (DatePicker_CloseVisibleDatePickerFrame (frame))
    return;
    
  if (target.disabled || target.readOnly)
    return;

  var left = button.offsetWidth;
  var top = 0;
  
  //  Claculate the offset of the frame in respect to the left top corner of the page.
  for (var currentNode = button; 
      currentNode && (currentNode.tagName != 'BODY'); 
      currentNode = currentNode.offsetParent)
  {
    left += currentNode.offsetLeft;
    top += currentNode.offsetTop;
  }

  //  Position at the top bottom corner of the button
  frame.style.left = left;
  frame.style.top = top;
  if (container.dp_width != null)
      frame.style.width = container.dp_width;
  if (container.dp_height != null)
      frame.style.height = container.dp_height;
  
  //  Adjust position so the date picker is shown below 
  //  and aligned with the right border of the button.
  frame.style.display = '';
  frame.style.left = frame.offsetLeft - frame.offsetWidth;
  frame.style.top = frame.offsetTop + container.offsetHeight;
  frame.style.display = 'none';

  if (window.frames[frame.id].DatePicker_InitializeCalendarFrame != null)
  {
    window.frames[frame.id].DatePicker_InitializeCalendarFrame(target, frame);
  
    _datePicker_currentDatePicker = frame;
    _datePicker_isEventAfterDatePickerButtonClick = true;
    target.document.onclick = DatePicker_OnDocumentClick;
  }
  frame.style.display = '';
}

//  Closes the currently visible date picker frame.
//  Belongs to the parent page.
//  newDatePicker: The newly selected date picker frame, used to test whether the current frame 
//      is identical to the new frame.
//  returns true if the newDatePicker is equal to the visible date picker.
function DatePicker_CloseVisibleDatePickerFrame (newDatePicker)
{
  if (   newDatePicker == _datePicker_currentDatePicker
      && newDatePicker.style.display != 'none')
  {
    return true;
  }        
  if (_datePicker_currentDatePicker != null)
  {
    window.frames[_datePicker_currentDatePicker.id].DatePicker_CloseDatePicker();
    _datePicker_currentDatePicker = null;
  }
  return false;
}

//  Initializes the date picker. 
//  Belongs to the date picker frame.
//  Persists the target and frame ids inside the picker frame to support post back.
//  Sets the currently selected date and calls a post back.
//  target: The input element receiving the value returned by the date picker.
//  frame: The IFrame containing the date picker that will be shown.
function DatePicker_InitializeCalendarFrame (target, frame)
{
  document.getElementById ('TargetIDField').value = target.id;
  document.getElementById ('FrameIDField').value = frame.id;
  
  if (document.getElementById ('DateValueField').value == '' && target.value != '')
  {
    document.getElementById ('DateValueField').value = target.value;
    __doPostBack('','');
  }
}

//  Called by the date picker when a new date is selected in the calendar. 
//  Belongs to the date picker frame.
function DatePicker_Calendar_SelectionChanged (value)
{
  target = window.parent.document.getElementById (document.getElementById ('TargetIDField').value);
  target.value = value;

  DatePicker_CloseDatePicker();
}

//  Closes the date picker frame
//  Belongs to the date picker frame.
function DatePicker_CloseDatePicker() 
{
  document.getElementById ('DateValueField').value = '';
  target = window.parent.document.getElementById (document.getElementById ('TargetIDField').value);
  frame = window.parent.document.getElementById (document.getElementById ('FrameIDField').value);
  target.document.onclick = null;
  target.focus();
  frame.style.display = 'none';
}

//  Called by th event handler for the onclick event of the parent pages's document.
//  Belongs to the parent page.
//  Closes the currently open date picker frame, 
//  unless _datePicker_isEventAfterDatePickerButtonClick is set to false.
function DatePicker_OnDocumentClick()
{
  if (_datePicker_isEventAfterDatePickerButtonClick)
  {
    _datePicker_isEventAfterDatePickerButtonClick = false;
  }
  else if (_datePicker_currentDatePicker != null)
  {
    window.frames[_datePicker_currentDatePicker.id].DatePicker_CloseDatePicker();
    _datePicker_currentDatePicker = null;
  }  
}
