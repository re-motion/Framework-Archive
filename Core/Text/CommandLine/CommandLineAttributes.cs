using System;
using System.Reflection;
using Rubicon.Utilities;
using Rubicon.NullableValueTypes;

namespace Rubicon.Text.CommandLine
{

[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public abstract class CommandLineArgumentAttribute: Attribute
{
  private CommandLineArgument _argument;

  #region dummy constructor
  /// <summary> do not use this constructor </summary>
  /// <remarks> this constructor is necessary because, even in abstract attribute classes, one constructor must have arguments that meet the constraints of attribute declarations. </remarks>
  [Obsolete]
  protected CommandLineArgumentAttribute (int doNotUseThisConstructor)
  {
    throw new NotSupportedException();
  }
  #endregion

  protected CommandLineArgumentAttribute (CommandLineArgument argument)
  {
    _argument = argument;
  }

  public string Name
  {
    get { return _argument.Name; }
    set { _argument.Name = value; }
  }

  public bool IsOptional
  {
    get { return _argument.IsOptional; }
    set { _argument.IsOptional = value; }
  }

  public string Placeholder
  {
    get { return _argument.Placeholder; }
    set { _argument.Placeholder = value; }
  }

  public string Description
  {
    get { return _argument.Description; }
    set { _argument.Description = value; }
  }

  public CommandLineArgument Argument
  {
    get { return _argument; }
  }

  public virtual void SetMember (MemberInfo fieldOrProperty)
  {
  }
}

[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class CommandLineStringArgumentAttribute: CommandLineArgumentAttribute
{
  public CommandLineStringArgumentAttribute (bool isOptional)
    : base (new CommandLineStringArgument (isOptional))
  {
  }

  public CommandLineStringArgumentAttribute (string name, bool isOptional)
    : base (new CommandLineStringArgument (name, isOptional))
  {
  }
}

[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class CommandLineFlagArgumentAttribute: CommandLineArgumentAttribute
{
  public CommandLineFlagArgumentAttribute (string name)
    : base (new CommandLineFlagArgument (name))
  {
  }

  public CommandLineFlagArgumentAttribute (string name, bool defaultValue)
    : base (new CommandLineFlagArgument (name, defaultValue))
  {
  }
}


[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class CommandLineInt32ArgumentAttribute: CommandLineArgumentAttribute
{
  public CommandLineInt32ArgumentAttribute (string name, bool isOptional)
    : base (new CommandLineInt32Argument (name, isOptional))
  {
  }

  public CommandLineInt32ArgumentAttribute (bool isOptional)
    : base (new CommandLineInt32Argument (isOptional))
  {
  }
}

[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class CommandLineEnumArgumentAttribute: CommandLineArgumentAttribute
{
  public CommandLineEnumArgumentAttribute (bool isOptional)
    : base (new CommandLineEnumArgument (isOptional, null))
  {
  }

  public CommandLineEnumArgumentAttribute (string name, bool isOptional)
    : base (new CommandLineEnumArgument (name, isOptional, null))
  {
  }

  public override void SetMember (MemberInfo fieldOrProperty)
  {
    Type enumType = ReflectionUtility.GetFieldOrPropertyType (fieldOrProperty);
    if (! enumType.IsEnum)
      throw new ApplicationException (string.Format ("Attribute {0} can only be applied to enumeration fields or properties.", typeof (CommandLineEnumArgumentAttribute).FullName));
    ((CommandLineEnumArgument) Argument).EnumType = enumType;
  }
}

}
