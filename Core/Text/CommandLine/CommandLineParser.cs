using System;
using System.Globalization;
using System.Text;

using Rubicon.Text;

namespace Rubicon.Text.CommandLine
{

/// <summary>
/// Provides methods for declaring command line syntax and providing values.
/// </summary>
public class CommandLineParser
{
  // fields

  public readonly CommandLineArgumentCollection Arguments;

  private char _seperator = ':';
  private bool _incrementalNameValidation = true;
  private bool _isCaseSensitive = false;
  
  // construction and disposal

  public CommandLineParser ()
  {
    Arguments = new CommandLineArgumentCollection (this);
  }

  // properties and methods

  public char Seperator
  {
    get 
    { 
      return _seperator; 
    }
    set 
    { 
      if (char.IsWhiteSpace(value))  throw new ArgumentOutOfRangeException ("value", value, "Whitespace is not supported as seperator.");
      _seperator = value; 
    }
  }

  public bool IncrementalNameValidation
  {
    get { return _incrementalNameValidation; }
    set { _incrementalNameValidation = value; }
  }

  public bool IsCaseSensitive
  {
    get { return _isCaseSensitive; }
    set { _isCaseSensitive = value; }
  }

  public void Parse (string[] args)
  {
    int nextPositionalArgument = 0;
    for (int i = 0; i < args.Length; ++i)
    {
      string arg = args[i];
      if (arg.StartsWith ("/"))
      {
        string name = null;
        string value = null;

        arg = arg.Substring (1);
        int pos = arg.IndexOf (_seperator);
        if (pos >= 0)
        {
          name = arg.Substring (0, pos);
          value = arg.Substring (pos + 1);
        }
        else 
        {
          pos = arg.IndexOfAny (new char[] { '+', '-' });
          if (pos >= 0)
          {
            name = arg.Substring (0, pos);
            value = arg.Substring (pos);
          }
        }
        if (name == null)
          name = arg;

        CommandLineArgument argument = GetArgument (name);

        argument.SetStringValue ((value != null) ? value : string.Empty);
      }
      else
      {
        CommandLineArgument argument = GetPositionalArgument (nextPositionalArgument);
        if (argument == null)
          throw new InvalidNumberOfCommandLineArgumentsException (arg, nextPositionalArgument);
        ++ nextPositionalArgument;

        argument.SetStringValue (arg);
      }
    }

    foreach (CommandLineArgument argument in this.Arguments)
    {
      if (! argument.IsOptional && argument.StringValue == null)
        throw new MissingRequiredCommandLineParameterException (argument);
    }
  }

  private CommandLineArgument GetPositionalArgument (int position)
  {
    int currentPosition = 0;
    foreach (CommandLineArgument argument in this.Arguments)
    {
      if (argument.IsPositional)
      {
        if (currentPosition == position)
          return argument;
        else
          ++ currentPosition;
      }
    }
    return null;
  }

  private CommandLineArgument GetArgument (string name)
  {
    if (_incrementalNameValidation)
    {
      CommandLineArgument foundArgument = null;
      bool found2ndArgument = false;
      foreach (CommandLineArgument argument in this.Arguments)
      {
        string argumentName = argument.Name;
        if (argumentName == null)
          continue;

        if (string.Compare (argumentName, name, !_isCaseSensitive, CultureInfo.InvariantCulture) == 0)
        {
          return argument;
        }
        else if (argumentName.Length > name.Length
               && string.Compare (argumentName, 0, name, 0, name.Length, !_isCaseSensitive, CultureInfo.InvariantCulture) == 0)
        {
          if (foundArgument != null)
            found2ndArgument = true;
          else
            foundArgument = argument;
        }
      }

      if (foundArgument == null)
        throw new InvalidCommandLineArgumentNameException (name, InvalidCommandLineArgumentNameException.MessageNotFound);
      else if (found2ndArgument)
        throw new InvalidCommandLineArgumentNameException (name, InvalidCommandLineArgumentNameException.MessageAmbiguous);

      return foundArgument;
    }
    else
    {
      foreach (CommandLineArgument argument in this.Arguments)
      {
        if (argument.Name == name)
          return argument;
      }
      throw new InvalidCommandLineArgumentNameException (name, InvalidCommandLineArgumentNameException.MessageNotFound);
    }

  }

  public string GetAsciiSynopsis (string commandName, int maxWidth)
  {
    StringBuilder sb = new StringBuilder (2048); 

    sb.Append (commandName);
    int maxLength = 0;
    int openSquareBrackets = 0;
    for (int i = 0; i < Arguments.Count; ++i)
    {
      CommandLineArgument argument = Arguments[i];
      CommandLineArgument nextArgument = ((i + 1) < Arguments.Count) ? Arguments[i+1] : null;

      // append opening square bracket
      sb.Append (" ");
      if (argument.IsOptional)
      {
        sb.Append ("[");
        ++ openSquareBrackets;
      }

      argument.AppendSynopsis(sb);

      // append closing square brackets after last optional argument
      if (argument.IsOptional && nextArgument == null || ! nextArgument.IsOptional || ! nextArgument.IsPositional)
      {
        for (int k = 0; k < openSquareBrackets; ++k)
          sb.Append ("]");
        openSquareBrackets = 0;
      }

      if (argument.Name != null)
        maxLength = Math.Max (maxLength, argument.Name.Length + 1);
      else if (argument.Placeholder != null)
        maxLength = Math.Max (maxLength, argument.Placeholder.Length);
    }

    sb.Append ("\n");
    foreach (CommandLineArgument argument in Arguments)
    {
      sb.AppendFormat ("\n  {0,-" + maxLength.ToString() + "}  ", 
          (argument.Name != null) ? "/" + argument.Name : argument.Placeholder);
      AsciiTextFormat.AppendIndentedText (sb, maxLength + 4, maxWidth, argument.Description);
    }

    return sb.ToString();
  }
}

}
