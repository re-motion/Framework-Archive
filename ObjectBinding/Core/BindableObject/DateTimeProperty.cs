using System;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public class DateTimeProperty : DateTimePropertyBase
  {
    public DateTimeProperty (Parameters parameters)
        : base (parameters)
    {
    }

    public override DateTimeType Type
    {
      get { return DateTimeType.DateTime; }
    }
  }
}