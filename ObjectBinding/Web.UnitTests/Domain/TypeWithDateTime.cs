using System;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Reflection;

namespace Rubicon.ObjectBinding.Web.UnitTests.Domain
{

public class TypeWithDateTime: ReflectionBusinessObject
{
  private DateTime _dateTimeValue;
  private NaDateTime _naDateTimeValue;

  public DateTime DateTimeValue
  {
    get { return _dateTimeValue; }
    set { _dateTimeValue = value; }
  }

  public NaDateTime NaDateTimeValue
  {
    get { return _naDateTimeValue; }
    set { _naDateTimeValue = value; }
  }
}

}
