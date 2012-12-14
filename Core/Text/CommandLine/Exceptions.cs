using System;
using System.Runtime.Serialization;

namespace Rubicon.Text.CommandLine
{

internal abstract class FormatArgument
{
  private const string c_messageByName = "Argument \"/{0}\"";
  private const string c_messageByPlaceholder = "Argument \"{0}\"";
  private const string c_messageByNumber = "Argument no. {0}";
  private const string c_messageUnknownArgument = "Unknown Argument";

  public static string Format (CommandLineArgument argument)
  {
    if (argument.Name != null)
      return string.Format (c_messageByName, argument.Name);
    else if (argument.Placeholder != null)
      return string.Format (c_messageByPlaceholder, argument.Placeholder);
    else if (argument.Parser != null)
      return string.Format (c_messageByNumber, argument.Position + 1);
    else 
      return c_messageUnknownArgument;
  }
}

/// <summary>
/// Base class for all exceptions that indicate errors in the command line.
/// </summary>
/// <remarks>
/// Throw <see cref="CommandLineArgumentApplicationException"/> to indicate application-defined command line
/// errors (e.g., use of arguments that exclude each other).
/// </remarks>
[Serializable]
public abstract class CommandLineArgumentException: Exception
{
  protected CommandLineArgumentException (string message)
    : base (message)
  {
  }

  protected CommandLineArgumentException (string message, Exception innerException)
    : base (message, innerException)
  {
  }

  protected CommandLineArgumentException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }
}

/// <summary>
/// This exception is thrown if the value of a parameter cannot be interpreted.
/// </summary>
[Serializable]
public class InvalidCommandLineArgumentValueException: CommandLineArgumentException
{ 
	public InvalidCommandLineArgumentValueException (CommandLineArgument argument, string message)
    : this (FormatArgument.Format (argument) + ": " + message)
	{
	}

  public InvalidCommandLineArgumentValueException (string message)
    : base (message)
	{
	}

  protected InvalidCommandLineArgumentValueException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }
}

/// <summary>
/// This exception is thrown if the command line contains a named argument that is not defined.
/// </summary>
/// <remarks>
/// The exception is thrown either because there is no argument definition with the specified name,
/// or (if <see cref="CommandLineParser.IncrementalNameValidation"/> is <c>true</c>), because there 
/// is more than one argument that starts with the specified string.
/// </remarks>
[Serializable]
public class InvalidCommandLineArgumentNameException: CommandLineArgumentException
{
  internal const string MessageNotFound = "Argument /{0}: invalid argument name.";
  internal const string MessageAmbiguous = "Argument /{0}: ambiguous argument name.";
 
	public InvalidCommandLineArgumentNameException (string name, string message)
    : base (string.Format (message, name))
	{
	}

  protected InvalidCommandLineArgumentNameException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }
}

/// <summary>
/// This exception is thrown if the command line contains too many unnamed arguments.
/// </summary>
[Serializable]
public class InvalidNumberOfCommandLineArgumentsException: CommandLineArgumentException
{
  private const string c_message = "Argument /{0}: unexpected argument. Only {1} unnamed arguments are allowed.";
 
	public InvalidNumberOfCommandLineArgumentsException (string argument, int number)
    : base (string.Format (c_message, argument, number))
	{
	}

  protected InvalidNumberOfCommandLineArgumentsException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }
}

/// <summary>
/// This exception is thrown if a non-optional command line argument is not contained in the command line.
/// </summary>
[Serializable]
public class MissingRequiredCommandLineParameterException: CommandLineArgumentException
{
  private const string c_message = ": Required Argument not specified.";
 
	public MissingRequiredCommandLineParameterException (CommandLineArgument argument)
    : base (FormatArgument.Format (argument) + c_message)
	{
	}

  protected MissingRequiredCommandLineParameterException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }
}

/// <summary>
/// This exception class indicates application-defined error conditions.
/// </summary>
/// <remarks>
/// Use this class to indicate application-defined command line errors (e.g., 
/// use of arguments that exclude each other).
/// </remarks>
[Serializable]
public class CommandLineArgumentApplicationException: CommandLineArgumentException
{
	public CommandLineArgumentApplicationException (string message)
    : base (message)
	{
	}

	public CommandLineArgumentApplicationException (string message, Exception innerException)
    : base (message, innerException)
	{
	}

  protected CommandLineArgumentApplicationException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }
}

}
