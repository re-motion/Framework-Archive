using System;
using System.Text;
using Remotion.NullableValueTypes;

namespace Remotion.Text.CommandLine
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

  protected internal override void SetStringValue (string value)
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
        throw new InvalidCommandLineArgumentValueException (this, "Flag parameters support only + and - as arguments.");
    }

    base.SetStringValue (value);
  }


  public override object ValueObject
  {
    get { return Value; }
  }
  
  public NaBoolean Value
  {
    get { return _value.IsNull ? _defaultValue : _value; }
  }

  public override void AppendSynopsis (StringBuilder sb)
  {
    if (IsOptional && _defaultValue.IsFalse)
    {
      sb.Append (Parser.ArgumentDeclarationPrefix);
      sb.Append (Name);
    }
    else if (IsOptional && _defaultValue.IsTrue)
    {
      sb.Append (Parser.ArgumentDeclarationPrefix);
      sb.Append (Name);
      sb.Append ("-");
    }
    else
    {
      sb.Append (Parser.ArgumentDeclarationPrefix);
      sb.Append (Name);
      sb.Append ("+ | /");
      sb.Append (Name);
      sb.Append ("-");
    }
  }
}

}
