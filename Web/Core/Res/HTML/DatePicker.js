//  DatePicker.js contains client side scripts used by DatePickerPage 
//  and the caller of the DatePickerFrom.aspx IFrame contents page.

//  The currently displayed date picker
//  Belongs to the parent page.
var _currentDatePicker = null;

//  Helper variable for event handling.
//  Belongs to the parent page.
//  The click event of the document fires after the methods bound to the button click have been 
//  executed. _isEventAfterDatePickerButtonClick is used to identify those click events fired
//  because a date picker button had been clicked in contrast to events fired
//  beause of a click somewhere on the page.
var _isEventAfterDatePickerButtonClick = false;

//  Shows the date picker frame below the button.
//  Belongs to the parent page.
//  button: The button that opened the date picker frame.
//  container: The page element containing the properties to be passed to the picker.
//  target: The input element receiving the value returned by the date picker.
//  frame: The IFrame containing the date picker that will be shown.
function ShowDatePicker (button, container, target, frame)
{
  //  Tried to open the already open date picker?
  //  Close it and return.
  if (CloseVisibleDatePickerFrame (frame))
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

  window.frames[frame.id].InitializeCalendarFrame(target, frame);
  
  _currentDatePicker = frame;
  frame.style.display = '';
  _isEventAfterDatePickerButtonClick = true;
}

//  Closes the currently visible date picker frame.
//  Belongs to the parent page.
//  newDatePicker: The newly selected date picker frame, used to test whether the current frame 
//      is identical to the new frame.
//  returns true if the newDatePicker is equal to the visible date picker.
function CloseVisibleDatePickerFrame (newDatePicker)
{
  if (   newDatePicker == _currentDatePicker
      && newDatePicker.style.display != 'none')
  {
    return true;
  }        
  if (_currentDatePicker != null)
  {
    window.frames[_currentDatePicker.id].CloseDatePicker();
    _currentDatePicker = null;
  }
  return false;
}

//  Initializes the date picker. 
//  Belongs to the date picker frame.
//  Persists the target and frame ids inside the picker frame to support post back.
//  Sets the currently selected date and calls a post back.
//  target: The input element receiving the value returned by the date picker.
//  frame: The IFrame containing the date picker that will be shown.
function InitializeCalendarFrame (target, frame)
{
  document.all['TargetIDField'].value = target.id;
  document.all['FrameIDField'].value = frame.id;
  
  if (document.all['DateValueField'].value == '' && target.value != '')
  {
    document.all['DateValueField'].value = target.value;
    __doPostBack('','');
  }
}

//  Called by the date picker when a new date is selected in the calendar. 
//  Belongs to the date picker frame.
function Calendar_SelectionChanged (value)
{
  target = window.parent.document.all[document.all['TargetIDField'].value];
  target.value = value;

  CloseDatePicker();
}

//  Closes the date picker frame
//  Belongs to the date picker frame.
function CloseDatePicker() 
{
  document.all['DateValueField'].value = '';
  target = window.parent.document.all[document.all['TargetIDField'].value];
  frame = window.parent.document.all[document.all['FrameIDField'].value];
  target.focus();
  frame.style.display = 'none';
}

//  Called by th event handler for the onclick event of the parent pages's document.
//  Belongs to the parent page.
//  Closes the currently open date picker frame, 
//  unless _isEventAfterDatePickerButtonClick is set to false.
function DatePicker_OnDocumentClick()
{
  if (_isEventAfterDatePickerButtonClick)
  {
    _isEventAfterDatePickerButtonClick = false;
  }
  else if (_currentDatePicker != null)
  {
    window.frames[_currentDatePicker.id].CloseDatePicker();
    _currentDatePicker = null;
  }  
}

//  Eventhandler for the parent page's document onclick event. 
//  Must be rendered inside the parent page.
//  <script for="document" event="onclick()" language="JScript" type="text/jscript">
//  { DatePicker_OnDocumentClick(); return true; }
//  </script>
