using System;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public class DateProperty : DateTimePropertyBase
  {
    public DateProperty (Parameters parameters)
        : base (parameters)
    {
    }

    public override DateTimeType Type
    {
      get { return DateTimeType.Date; }
    }
  }
}