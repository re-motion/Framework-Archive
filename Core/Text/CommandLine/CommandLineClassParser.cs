using System;
using System.Globalization;
using System.Text;
using System.Collections;
using System.Reflection;
using Rubicon.Utilities;
using Rubicon.Text;
using Rubicon.NullableValueTypes;

namespace Rubicon.Text.CommandLine
{

public class CommandLineClassParser: CommandLineParser
{
  private Type _argumentClass;

  /// <summary> IDictionary &lt;CommandLineArgument, MemberInfo&gt; </summary>
  private IDictionary _arguments;
  
  public CommandLineClassParser (Type argumentClass)
  {
    _argumentClass = argumentClass;
    _arguments = new System.Collections.Specialized.ListDictionary();

    foreach (MemberInfo member in argumentClass.GetMembers (BindingFlags.Public | BindingFlags.Instance))
    {
      if (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)
      {
        CommandLineArgumentAttribute argumentAttribute = (CommandLineArgumentAttribute) ReflectionUtility.GetSingleAttribute (
            member, typeof (CommandLineArgumentAttribute), false, false);
        if (argumentAttribute != null)
        {
          argumentAttribute.SetMember (member);
          this.Arguments.Add (argumentAttribute.Argument);
          _arguments.Add (argumentAttribute.Argument, member);
        }
      }
    }
  }

  public new object Parse (string commandLine, bool includeFirstArgument)
  {
    return Parse (SplitCommandLine (commandLine, includeFirstArgument));
  }

  public new object Parse (string[] args)
  {
    base.Parse (args);
    object obj = Activator.CreateInstance (_argumentClass);

    foreach (DictionaryEntry entry in _arguments)
    {
      CommandLineArgument argument = (CommandLineArgument) entry.Key;
      MemberInfo fieldOrProperty = (MemberInfo) entry.Value;
      Type memberType = ReflectionUtility.GetFieldOrPropertyType (fieldOrProperty);
      object value = argument.ValueObject;
      if (value is NaBoolean && memberType == typeof (bool))
      {
        NaBoolean naboolval = (NaBoolean) value;
        if (naboolval.IsNull)
          throw new ApplicationException (string.Format ("{0} {1}: Cannot convert Rubicon.NaBoolean.Null to System.Boolean. Use NaBoolean type for optional attributes without default values.", fieldOrProperty.MemberType, fieldOrProperty.Name));
        value = (bool) naboolval;
      }
      try
      {
        ReflectionUtility.SetFieldOrPropertyValue (obj, fieldOrProperty, value);
      }
      catch (Exception e)
      {
        throw new ApplicationException (string.Format ("Error setting value of {0} {1}: {2}", fieldOrProperty.MemberType, fieldOrProperty.Name, e.Message), e);
      }
    }
    return obj;
  }
}

}
