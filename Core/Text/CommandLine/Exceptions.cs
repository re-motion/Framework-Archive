using System;
using System.Runtime.Serialization;

namespace Rubicon.Text.CommandLineParser
{

[Serializable]
public class InvalidCommandLineArgumentValueException: Exception
{
  private const string c_messageByName = "Argument /{0}: {1}";
  private const string c_messageByPlaceholder = "Argument {0}: {1}";
  private const string c_messageByNumber = "Argument no. {0}: {1}";
 
	public InvalidCommandLineArgumentValueException (CommandLineArgument argument, string message)
    : this (FormatArgument (argument, message))
	{
	}

  public InvalidCommandLineArgumentValueException (string message)
    : base (message)
	{
	}

  public InvalidCommandLineArgumentValueException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }

  private static string FormatArgument (CommandLineArgument argument, string message)
  {
    if (argument.Name != null)
      return string.Format (c_messageByName, argument.Name, message);
    else if (argument.Placeholder != null)
      return string.Format (c_messageByPlaceholder, argument.Placeholder, message);
    else if (argument.Parser != null)
      return string.Format (c_messageByNumber, argument.Position + 1, message);
    else 
      return message;
  }
}

}
