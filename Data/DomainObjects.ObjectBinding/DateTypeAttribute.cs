using System;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property)]
public class IsDateTypeAttribute : Attribute
{
  private bool _isDateType;

	public IsDateTypeAttribute (bool isDateType)
	{
    _isDateType = isDateType;
	}

  public bool IsDateType
  {
    get { return _isDateType; }
    set { _isDateType = value; }
  }
}
}
