using System;
using System.Text;

using Rubicon.Text;

namespace Rubicon.Text.CommandLineParser
{

/// <summary>
/// Provides methods for declaring command line syntax and providing values.
/// </summary>
public class CommandLineParser
{
  // fields

  public readonly CommandLineArgumentCollection Arguments;

  // construction and disposal

  public CommandLineParser ()
  {
    Arguments = new CommandLineArgumentCollection (this);
  }

  // properties and methods

  public string GetAsciiSynopsis (string commandName)
  {
    StringBuilder sb = new StringBuilder (2048); 

    sb.Append (commandName);
    int maxLength = 0;
    int openSquareBrackets = 0;
    for (int i = 0; i < Arguments.Count; ++i)
    {
      CommandLineArgument argument = Arguments[i];
      CommandLineArgument nextArgument = ((i + 1) < Arguments.Count) ? Arguments[i+1] : null;

      sb.Append (" ");
      if (argument.IsOptional)
      {
        sb.Append ("[");
        ++ openSquareBrackets;
      }

      argument.AppendSynopsis(sb);

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
      AsciiTextFormat.AppendIndentedText (sb, maxLength + 4, 80, argument.Description);
    }

    return sb.ToString();
  }
}

}
