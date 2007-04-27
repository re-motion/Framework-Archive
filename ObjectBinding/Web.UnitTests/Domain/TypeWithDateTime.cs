using System;
using Rubicon.ObjectBinding.Reflection;

namespace Rubicon.ObjectBinding.Web.UnitTests.Domain
{

public class TypeWithDateTime: ReflectionBusinessObject
{
  private DateTime _dateTimeValue;
  private DateTime? _nullableDateTimeValue;

  public DateTime DateTimeValue
  {
    get { return _dateTimeValue; }
    set { _dateTimeValue = value; }
  }

  public DateTime? NullableDateTimeValue
  {
    get { return _nullableDateTimeValue; }
    set { _nullableDateTimeValue = value; }
  }
}

}
