using System;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public abstract class DateTimePropertyBase : PropertyBase, IBusinessObjectDateTimeProperty
  {
    protected DateTimePropertyBase (Parameters parameters)
        : base (parameters)
    {
    }

    public abstract DateTimeType Type { get; }
  }
}