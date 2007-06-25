using System;
using System.Reflection;

namespace Rubicon.Utilities
{
  public static class ReflectionUtility
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


    public static object GetFieldOrPropertyValue (object obj, string fieldOrPropertyName)
    {
      return ReflectionUtility.GetFieldOrPropertyValue (obj, fieldOrPropertyName, BindingFlags.Public);
    }
     
    public static object GetFieldOrPropertyValue (object obj, string fieldOrPropertyName, BindingFlags bindingFlags)
    {
      ArgumentUtility.CheckNotNull("obj", obj);
      MemberInfo fieldOrProperty = ReflectionUtility.GetFieldOrProperty (obj.GetType(), fieldOrPropertyName, bindingFlags, true);
      return ReflectionUtility.GetFieldOrPropertyValue (obj, fieldOrProperty);
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


    public static void SetFieldOrPropertyValue (object obj, string fieldOrPropertyName, object value)
    {
      ReflectionUtility.SetFieldOrPropertyValue (obj, fieldOrPropertyName, BindingFlags.Public, value);
    }
     
    public static void SetFieldOrPropertyValue (object obj, string fieldOrPropertyName, BindingFlags bindingFlags, object value)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);
      MemberInfo fieldOrProperty = ReflectionUtility.GetFieldOrProperty (obj.GetType(), fieldOrPropertyName, bindingFlags, true);
      ReflectionUtility.SetFieldOrPropertyValue (obj, fieldOrProperty, value);
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

    /// <summary>
    /// Evaluates whether the <paramref name="type"/> can be ascribed to the <paramref name="genericType"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check. Must not be <see langword="null" />.</param>
    /// <param name="genericType">The <see cref="Type"/> to check the <paramref name="type"/> against. Must not be <see langword="null" />.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="type"/> is not the <paramref name="genericType"/> or it's instantiation, 
    /// it's subclass or the implementation of an interface in case the <paramref name="genericType"/> is an interface..
    /// </returns>
    public static bool CanAscribe (Type type, Type genericType)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("genericType", genericType);

      if (genericType.IsInterface)
        return Array.Exists (type.GetInterfaces (), delegate (Type current) { return CanAscribeInternal (current, genericType); });
      else
        return CanAscribeInternal(type, genericType);
    }

    /// <summary>
    /// Returns the type arguments for the ascribed <paramref name="genericType"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> for which to return the type parameter. Must not be <see langword="null" />.</param>
    /// <param name="genericType">The <see cref="Type"/> to check the <paramref name="type"/> against. Must not be <see langword="null" />.</param>
    /// <returns>A <see cref="Type"/> array containing the generic arguments of the <paramref name="type"/>.</returns>
    /// <exception cref="ArgumentTypeException">
    /// Thrown if the <paramref name="type"/> is not the <paramref name="genericType"/> or it's instantiation, it's subclass or the implementation
    /// of an interface in case the <paramref name="genericType"/> is an interface.
    /// </exception>
    public static Type[] GetAscribedGenericArguments (Type type, Type genericType)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("genericType", genericType);
      
      if (genericType.IsInterface)
      {
        Type interfaceType = Array.Find (type.GetInterfaces (), delegate (Type current) { return CanAscribeInternal (current, genericType); });
        if (interfaceType == null)
          throw new ArgumentTypeException ("type", genericType, type);

        return GetAscribedGenericArgumentsInternal (interfaceType, genericType);
      }
      else
      {
        return GetAscribedGenericArgumentsInternal (type, genericType);
      }
    }

    private static bool CanAscribeInternal (Type type, Type genericType)
    {
      for (Type currentType = type; currentType != null; currentType = currentType.BaseType)
      {
        if (currentType.IsGenericType && genericType.IsAssignableFrom (currentType.GetGenericTypeDefinition ()))
          return true;
      }

      return false;
    }

    private static Type[] GetAscribedGenericArgumentsInternal (Type type, Type genericType)
    {
      for (Type currentType = type; currentType != null; currentType = currentType.BaseType)
      {
        if (currentType.IsGenericType && genericType.IsAssignableFrom (currentType.GetGenericTypeDefinition ()))
          return currentType.GetGenericArguments ();
      }

      throw new ArgumentTypeException ("type", genericType, type);
    }
  }
}
