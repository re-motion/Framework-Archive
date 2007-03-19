using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Rubicon.Utilities
{
  /// <summary>
  /// This exception is thrown when an assertion fails.
  /// </summary>
  /// <seealso cref="Assertion"/>
  [Serializable]
  public class AssertionException : Exception
  {
    public AssertionException (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
    }

    public AssertionException (string message, Exception innerException)
      : base (message, innerException)
    {
    }

    public AssertionException (string message)
      : this (message, null)
    {
    }
  }

  /// <summary>
  /// Provides methods that throw an <see cref="AssertionException"/> if an assertion fails.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///   This class contains methods that are conditional to the DEBUG and TRACE attributes (<see cref="DebugAssert"/> and <see cref="TraceAssert"/>). 
  ///   </para><para>
  ///   Note that assertion expressions passed to these methods are not evaluated (read: executed) if the respective symbol are not defined during
  ///   compilation, nor are the methods called. This increases performance for production builds, but make sure that your assertion expressions do
  ///   not cause any side effects! See <see cref="ConditionalAttribute"/> or <see cref="Debug"/> and <see cref="Trace"/> the for more information 
  ///   about conditional compilation.
  ///   </para><para>
  ///   Assertions are no replacement for checking input parameters of public methods (see <see cref="ArgumentUtility"/>).  
  ///   </para>
  /// </remarks>
  public static class Assertion
  {
    private const string c_defaultMessage = "Assertion failed.";

    [Conditional ("DEBUG")]
    public static void DebugAssert (bool assertion, string message)
    {
      Assert (assertion, message);
    }

    [Conditional ("DEBUG")]
    public static void DebugAssert (bool assertion, string message, params object[] arguments)
    {
      Assert (assertion, message, arguments);
    }

    [Conditional ("DEBUG")]
    public static void DebugAssert (bool assertion)
    {
      Assert (assertion);
    }

    [Conditional ("TRACE")]
    public static void TraceAssert (bool assertion, string message)
    {
      Assert (assertion, message);
    }

    [Conditional ("TRACE")]
    public static void TraceAssert (bool assertion, string message, params object[] arguments)
    {
      Assert (assertion, message, arguments);
    }

    [Conditional ("TRACE")]
    public static void TraceAssert (bool assertion)
    {
      Assert (assertion);
    }

    public static void Assert (bool assertion, string message)
    {
      if (! assertion)
        throw new AssertionException (message);
    }

    public static void Assert (bool assertion, string message, params object[] arguments)
    {
      Assert (assertion, string.Format (message, arguments));
    }

    public static void Assert (bool assertion)
    {
      Assert (assertion, c_defaultMessage);
    }
  }
}
