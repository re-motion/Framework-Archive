using System;
using System.Text;
using System.Globalization;
using Rubicon.Data.NullableValueTypes;

namespace Rubicon.Text.CommandLine
{

public class CommandLineValueArgument: CommandLineArgument
{
  public CommandLineValueArgument (string name, bool isOptional)
    : base (name, isOptional)
  {
  }

  public CommandLineValueArgument (bool isOptional)
    : base (isOptional)
  {
  }

  public override void AppendSynopsis (StringBuilder sb)
  {
    if (! IsPositional)
    {
      sb.Append ("/");
      sb.Append (Name);
      if (this.Placeholder != null)
        sb.Append (Parser.Seperator);
    }
    sb.Append (this.Placeholder);
  }
}

public class CommandLineStringArgument: CommandLineValueArgument
{
  public CommandLineStringArgument (string name, bool isOptional)
    : base (name, isOptional)
  {
  }

  public CommandLineStringArgument (bool isOptional)
    : base (isOptional)
  {
  }

  public string Value
  {
    get { return StringValue; }
  }
}

public class CommandLineInt32Argument: CommandLineValueArgument
{
  private NaInt32 _value;

  public CommandLineInt32Argument (string name, bool isOptional)
    : base (name, isOptional)
  {
  }

  public CommandLineInt32Argument (bool isOptional)
    : base (isOptional)
  {
  }

  public NaInt32 Value
  {
    get { return _value; }
  }

  protected internal override void SetStringValue (string value)
  {
    if (value == null) throw new ArgumentNullException ("value");
    string strValue = value.Trim();
    if (strValue.Length == 0)
    {
      _value = NaInt32.Null;
    }
    else
    {
      double result;
      if (! double.TryParse (value, NumberStyles.Integer, null, out result))
        throw new InvalidCommandLineArgumentValueException (this, "Specify a valid integer number.");
      _value = (int) result;
    }

    base.SetStringValue (value);
  }

}

}
