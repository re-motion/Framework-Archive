using System;

namespace Rubicon.ObjectBinding
{
  //TODO: Enum for Date and DateTime
  /// <summary> The <b>IBusinessObjectDateTimeProperty</b> interface is used for accessing <see cref="DateTime"/> values. </summary>
  public interface IBusinessObjectDateTimeProperty : IBusinessObjectProperty
  {
  }

  /// <summary> 
  ///   The <b>IBusinessObjectDateProperty</b> interface is used for accessing <see cref="DateTime"/> values whose time
  ///   component will be ignored and potentially not persisted. 
  /// </summary>
  public interface IBusinessObjectDateProperty : IBusinessObjectProperty
  {
  }
}