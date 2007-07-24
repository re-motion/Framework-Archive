using System;
using Rubicon.Mixins;

namespace Rubicon.ObjectBinding.Web.UnitTests.Domain
{
  [BindableObject]
  public class TypeWithDateTime
  {
    public static TypeWithDateTime Create ()
    {
      return ObjectFactory.Create<TypeWithDateTime> ().With ();
    }

    private DateTime _dateTimeValue;
    private DateTime? _nullableDateTimeValue;

    protected TypeWithDateTime ()
    {
    }

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