using System;
using System.Globalization;
using System.Web.UI.WebControls;

namespace Rubicon.Findit.Client.Controls
{
public class DateRangeValidator : RangeValidator
{
  // types
  private int _minimumValueOffsetInYears;
  private int _maximumValueOffsetInYears;

  // static members and constants

  // member fields

  // construction and disposing

  public DateRangeValidator()
  {
    Type = ValidationDataType.Date;
    
    MinimumValue = DateTime.Now.ToString ("d");
    MaximumValue = DateTime.Now.ToString ("d");
  }

  // methods and properties
  
  public int MinimumValueOffsetInYears
  {
    get { return _minimumValueOffsetInYears; }
    set 
    { 
      _minimumValueOffsetInYears = value; 

      DateTime date = DateTime.Now.AddYears (MinimumValueOffsetInYears);
      MinimumValue = date.ToString ("d");
    }
  }

  public int MaximumValueOffsetInYears
  {
    get { return _maximumValueOffsetInYears; }
    set 
    { 
      _maximumValueOffsetInYears = value; 

      DateTime date = DateTime.Now.AddYears (MaximumValueOffsetInYears);
      MaximumValue = date.ToString ("d");
    }
  }

}
}
