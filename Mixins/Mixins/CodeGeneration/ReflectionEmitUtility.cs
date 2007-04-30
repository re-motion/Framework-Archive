using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Rubicon.Utilities;

namespace Mixins.CodeGeneration
{
  public static class ReflectionEmitUtility
  {
    public static CustomAttributeBuilder CreateAttributeBuilderFromData (CustomAttributeData attributeData)
    {
      object[] constructorArgs = new object[attributeData.ConstructorArguments.Count];
      for (int i = 0; i < constructorArgs.Length; ++i)
        constructorArgs[i] = attributeData.ConstructorArguments[i].Value;

      List<PropertyInfo> namedProperties = new List<PropertyInfo> ();
      List<object> propertyValues = new List<object> ();
      List<FieldInfo> namedFields = new List<FieldInfo> ();
      List<object> fieldValues = new List<object> ();

      foreach (CustomAttributeNamedArgument namedArgument in attributeData.NamedArguments)
      {
        switch (namedArgument.MemberInfo.MemberType)
        {
          case MemberTypes.Field:
            namedFields.Add ((FieldInfo) namedArgument.MemberInfo);
            fieldValues.Add (namedArgument.TypedValue.Value);
            break;
          case MemberTypes.Property:
            namedProperties.Add ((PropertyInfo) namedArgument.MemberInfo);
            propertyValues.Add (namedArgument.TypedValue.Value);
            break;
          default:
            Assertion.Assert (false);
            break;
        }
      }

      return new CustomAttributeBuilder (attributeData.Constructor, constructorArgs, namedProperties.ToArray (),
                                        propertyValues.ToArray (), namedFields.ToArray (), fieldValues.ToArray ());
    }
  }
}
