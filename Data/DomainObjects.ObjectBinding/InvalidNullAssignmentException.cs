using System;
using System.Runtime.Serialization;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
public class InvalidNullAssignmentException : Exception
{
  public InvalidNullAssignmentException () : this ("An invalid null assignment exception occured") {}
  public InvalidNullAssignmentException (Type targetType) : this (string.Format ("An invalid null assignment exception occured: Null cannot be assigned to a variable of type {0}", targetType.Name)) {}
  public InvalidNullAssignmentException (string message) : base (message) {}
  public InvalidNullAssignmentException (string message, Exception inner) : base (message, inner) {}
  protected InvalidNullAssignmentException (SerializationInfo info, StreamingContext context) : base (info, context) {}

}
}
