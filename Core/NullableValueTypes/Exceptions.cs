using System;
using System.Runtime.Serialization;

namespace Rubicon.NullableValueTypes
{

/// <summary>
/// This exception indicates invalid use of Null values.
/// </summary>
/// <remarks>
/// This exception is thrown if 
/// <list type="bullet">
///   <item>an operation is attempted with nullable value type instances,</item>
///   <item>one or more of these instances are Null,</item>
///   <item>and the operation requires the instances to be not Null.</item>
/// </list>
/// </remarks>
[Serializable]
public class NaNullValueException: Exception
{
  /// <summary>
  /// Creates a new <see cref="NaNullValueException"/> with the specified message.
  /// </summary>
  public NaNullValueException (string msg)
    : base (msg)
  {
  }

  /// <summary>
  /// Creates a new <see cref="NaNullValueException"/> for a member method or property.
  /// </summary>
  /// <param name="memberName">The name of the method or property that caused this exception.</param>
  /// <returns></returns>
  public static NaNullValueException AccessingMember (string memberName)
  {
    return new NaNullValueException (string.Format (NaResources.NullValueMemberAccessMsg, memberName));
  }
}

internal class NaResources
{
  internal static readonly string NullValueMemberAccessMsg = "Cannot access the member \"{0}\" because the value is null.";
  internal static readonly string ArithmeticOverflowMsg = "Arithmetic overflow in operation.";
  internal static readonly string DivideByZeroMsg = "Division by zero attempted in operation.";
}


}
