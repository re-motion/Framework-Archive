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
