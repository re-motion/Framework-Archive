using System;

namespace Rubicon.ObjectBinding
{
  /// <summary> The <b>IBusinessObjectDateTimeProperty</b> interface is used for accessing <see cref="DateTime"/> values. </summary>
  public interface IBusinessObjectDateTimeProperty : IBusinessObjectProperty
  {
    DateTimeType Type { get; }
  }

  /// <summary>
  /// The <see cref="DateTimeType"/> enum defines the list of possible data types supported by the <see cref="IBusinessObjectDateTimeProperty"/>.
  /// </summary>
  public enum DateTimeType
  {
    DateTime,
    Date
  }
}