using System;
using System.Text;
using Rubicon.Data.NullableValueTypes;

namespace Rubicon.Text.CommandLineParser
{

public class CommandLineFlagArgument: CommandLineArgument
{
  // fields

  private NaBoolean _defaultValue;
  private NaBoolean _value;

  // construction and disposal

  public CommandLineFlagArgument (string name, NaBoolean defaultValue)
    : base (name, true)
  {
    _defaultValue = defaultValue;
  }

  public CommandLineFlagArgument (string name)
    : base (name, true)
  {
    _defaultValue = NaBoolean.Null;
  }

  // properties and methods

  public NaBoolean DefaultValue
  {
    get { return _defaultValue; }
  }

  protected internal override void SetStringValue (string   value)
  {
    if (value == null) throw new ArgumentNullException ("value");

    switch (value)
    {
      case "": 
        _value = NaBoolean.True;
        break;

      case "+":
        _value = NaBoolean.True;
        break;
      
      case "-":
        _value = NaBoolean.False;
        break;

      default:
        throw new ArgumentOutOfRangeException ("value", value, "Flag parameters support only + and - as arguments.");
    }

    base.SetStringValue (value);
  }

  public NaBoolean Value
  {
    get { return _value.IsNull ? _defaultValue : _value; }
  }

  public override void AppendSynopsis (StringBuilder sb)
  {
    if (IsOptional && _defaultValue == NaBoolean.False)
    {
      sb.Append ("/");
      sb.Append (Name);
    }
    else if (IsOptional && _defaultValue == NaBoolean.True)
    {
      sb.Append ("/");
      sb.Append (Name);
      sb.Append ("-");
    }
    else
    {
      sb.Append ("/");
      sb.Append (Name);
      sb.Append ("+ | /");
      sb.Append (Name);
      sb.Append ("-");
    }
  }
}

}
