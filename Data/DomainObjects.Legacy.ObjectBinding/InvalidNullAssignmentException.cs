using System;
using System.Runtime.Serialization;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
/// <summary>
/// The exception that is thrown when attempting to assign <see langword="null"/> to a property that does not support it.
/// </summary>
[Serializable]
public class InvalidNullAssignmentException : Exception
{
  public InvalidNullAssignmentException () : this ("An invalid null assignment exception occured") {}
  public InvalidNullAssignmentException (Type targetType) : this (string.Format ("An invalid null assignment exception occured: Null cannot be assigned to a variable of type {0}", targetType.Name)) {}
  public InvalidNullAssignmentException (string message) : base (message) {}
  public InvalidNullAssignmentException (string message, Exception inner) : base (message, inner) {}
  protected InvalidNullAssignmentException (SerializationInfo info, StreamingContext context) : base (info, context) {}

}
}
