using System;
using System.Text;
using System.Reflection;
using System.Globalization;

namespace Rubicon.Text.CommandLineParser
{

public class CommandLineEnumArgument: CommandLineValueArgument
{
  // fields

  private Type _enumType;
  private System.Enum _value;
  private bool _isCaseSensitive = false;

  // construction and disposal

  public CommandLineEnumArgument (string name, bool isOptional, Type enumType)
    : base (name, isOptional)
  {
    if (! enumType.IsEnum) throw new ArgumentOutOfRangeException ("enumType", enumType, "Argument must be an enumeration type.");
    _enumType = enumType;
  }

  public CommandLineEnumArgument (bool isOptional, Type enumType)
    : this (null, isOptional, enumType)
  {
  }

  // properties and methods

  protected internal override void SetStringValue (string value)
  {
    if (value == null) throw new ArgumentNullException ("value");
    if (value.Length != 0)
    {
      bool foundExact = false;
      bool foundIncremental = false;
      bool found2ndIncremental = false;
      foreach (FieldInfo enumValue in _enumType.GetFields (BindingFlags.Public | BindingFlags.Static))
      {
        string enumName = enumValue.Name;
        if (string.Compare (enumName, value, !_isCaseSensitive, CultureInfo.InvariantCulture) == 0)
        {
          foundExact = true;
          _value = (System.Enum) enumValue.GetValue (null);
          break;
        }
        else if (enumName.Length > value.Length 
               && string.Compare (enumValue.Name, 0, value, 0, value.Length, !_isCaseSensitive, CultureInfo.InvariantCulture) == 0)
        {
          if (foundIncremental)
          {
            found2ndIncremental = true;
          }
          else
          {
            foundIncremental = true;
            _value = (System.Enum) enumValue.GetValue (null);
          }
        }
      }

      if (! foundExact && ! foundIncremental)
      {
        StringBuilder values = new StringBuilder();
        FieldInfo[] enumValues = _enumType.GetFields (BindingFlags.Static | BindingFlags.Public);
        for (int i = 0; i < enumValues.Length; ++i)
        {
          if (i == enumValues.Length - 1)
            values.Append (" or ");
          else if (i > 0)
            values.Append (", ");
          values.Append (enumValues[i].Name);
        }
        throw new InvalidCommandLineArgumentValueException (this, "Use one of the following values: " + values.ToString() + ".");
      }
      else if (found2ndIncremental)
      {
        throw new InvalidCommandLineArgumentValueException (this, "Ambiguous value " + value + ".");
      }
    }
    base.SetStringValue (value);
  }

  public bool IsCaseSensitive
  {
    get { return _isCaseSensitive; }
    set { _isCaseSensitive = value; }
  }

  public override void AppendSynopsis (StringBuilder sb)
  {
    if (! IsPositional)
    {
      sb.Append ("/");
      sb.Append (Name);
    }
    sb.Append (this.Placeholder);
    sb.Append ("{");
    bool first = true;
    foreach (FieldInfo enumValue in _enumType.GetFields (BindingFlags.Public | BindingFlags.Static))
    {
      if (first)
        first = false;
      else 
        sb.Append ("|");
      sb.Append (enumValue.Name);
    }
    sb.Append ("}");
  }

  public System.Enum Value
  {
    get { return _value; }
  }

}

}
