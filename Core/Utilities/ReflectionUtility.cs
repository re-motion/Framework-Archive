using System;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace Rubicon
{

public sealed class ReflectionUtility
{
  public delegate bool CompareValues (object propertyOrFieldValue, object compareToValue);

  public static object GetSingleAttribute (MemberInfo member, Type attributeType, bool inherit, bool throwExceptionIfNotPresent)
  {
    object[] attributes = member.GetCustomAttributes (attributeType, inherit);
    if (attributes.Length > 1)
      throw new InvalidOperationException (string.Format ("More that one attribute of type {0} found for {1} {2}. Only single attributes are supported by this method.", attributeType.FullName, member.MemberType, member));
    if (attributes.Length == 0)
    {
      if (throwExceptionIfNotPresent)
        throw new ApplicationException (string.Format ("{0} {1} does not have attribute {2}.", member.MemberType, member, attributeType.FullName));
      else
        return null;
    }
    return attributes[0];
  }

  public static object GetAttributeArrayMemberValue (
      MemberInfo reflectionObject, 
      Type attributeType, 
      bool inherit, 
      MemberInfo fieldOrProperty, 
      MemberInfo comparePropertyOrField, 
      object compareToValue,
      CompareValues comparer)
  {
    object[] attributes = reflectionObject.GetCustomAttributes (attributeType, inherit);
    if (attributes == null || attributes.Length == 0)
      return null;
    foreach (Attribute attribute in attributes)
    {
      if (comparer (GetFieldOrPropertyValue (attribute, comparePropertyOrField), compareToValue))
        return GetFieldOrPropertyValue (attribute, fieldOrProperty);
    }
    return null;
  }

  public static object GetAttributeMemberValue (MemberInfo reflectionObject, Type attributeType, bool inherit, MemberInfo fieldOrProperty)
  {
    object[] attributes = reflectionObject.GetCustomAttributes (attributeType, inherit);
    if (attributes == null || attributes.Length == 0)
      return null;
    if (attributes.Length > 1)
      throw new NotSupportedException (string.Format ("Cannot get member value for multiple attributes. Reflection object {0} has {1} instances of attribute {2}", reflectionObject.Name, attributes.Length, attributeType.FullName));      
    return GetFieldOrPropertyValue (attributes[0], fieldOrProperty);
  }

  public static MemberInfo GetFieldOrProperty (Type type, string fieldOrPropertyName, BindingFlags bindingFlags, bool throwExceptionIfNotFound)
  {
    MemberInfo member = type.GetField (fieldOrPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    if (member != null)
      return member;
    
    member = type.GetProperty (fieldOrPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    if (member != null)
      return member;

    if (throwExceptionIfNotFound)      
      throw new ArgumentException (string.Format ("{0} is not an instance field or property of type {1}.", fieldOrPropertyName, type.FullName), "memberName");
    return null;
  }

  public static object GetFieldOrPropertyValue (object obj, MemberInfo fieldOrProperty)
  {
    if (obj == null)
      throw new ArgumentNullException("obj");
    if (fieldOrProperty == null)
      throw new ArgumentNullException("member");

    if (fieldOrProperty is FieldInfo)
      return ((FieldInfo)fieldOrProperty).GetValue (obj);
    else if (fieldOrProperty is PropertyInfo)
      return ((PropertyInfo)fieldOrProperty).GetValue (obj, new object[0]);
    else
      throw new ArgumentException (string.Format ("Argument must be either FieldInfo or PropertyInfo but is {0}.", fieldOrProperty.GetType().FullName), "member");
  }

  public static void SetFieldOrPropertyValue (object obj, MemberInfo fieldOrProperty, object value)
  {
    if (obj == null)
      throw new ArgumentNullException("obj");
    if (fieldOrProperty == null)
      throw new ArgumentNullException("member");

    if (fieldOrProperty is FieldInfo)
      ((FieldInfo)fieldOrProperty).SetValue (obj, value);
    else if (fieldOrProperty is PropertyInfo)
      ((PropertyInfo)fieldOrProperty).SetValue (obj, value, new object[0]);
    else
      throw new ArgumentException (string.Format ("Argument must be either FieldInfo or PropertyInfo but is {0}.", fieldOrProperty.GetType().FullName), "member");
  }

  public static Type GetFieldOrPropertyType (MemberInfo fieldOrProperty)
  {
    if (fieldOrProperty is FieldInfo)
      return ((FieldInfo)fieldOrProperty).FieldType;
    else if (fieldOrProperty is PropertyInfo)
      return ((PropertyInfo)fieldOrProperty).PropertyType;
    else 
      throw new ArgumentException ("Argument must be FieldInfo or PropertyInfo.", "fieldOrProperty");
  }

  private ReflectionUtility()
  {
  }
}

}
