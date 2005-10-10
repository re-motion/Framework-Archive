using System;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
/// <summary>
/// Indicates the default date format for bound controls.
/// </summary>
public enum DateTypeEnum
{
  Date = 0,
  DateTime
}

/// <summary>
/// Specifies the default date format of a property or field.
/// </summary>
[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property)]
public class DateTypeAttribute : Attribute
{
  private DateTypeEnum _dateType;

  /// <summary>
  /// Instantiates a new object.
  /// </summary>
  /// <param name="dateType">A value indicating the default date format.</param>
	public DateTypeAttribute (DateTypeEnum dateType)
	{
    _dateType = dateType;
	}

  /// <summary>
  /// Gets or sets the default date format.
  /// </summary>
  public DateTypeEnum DateType
  {
    get { return _dateType; }
    set { _dateType = value; }
  }
}
}
