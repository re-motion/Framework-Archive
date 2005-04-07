using System;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
public enum DateTypeEnum
{
  Date = 0,
  DateTime
}

[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property)]
public class DateTypeAttribute : Attribute
{
  private DateTypeEnum _dateType;

	public DateTypeAttribute (DateTypeEnum dateType)
	{
    _dateType = dateType;
	}

  public DateTypeEnum DateType
  {
    get { return _dateType; }
    set { _dateType = value; }
  }
}
}
