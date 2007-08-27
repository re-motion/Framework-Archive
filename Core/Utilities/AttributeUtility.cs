using System;
using System.Reflection;

namespace Rubicon.Utilities
{
  /// <summary>
  /// Utility class for finding custom attributes via their type or an interface implemented by the type.
  /// </summary>
  public static class AttributeUtility
  {
    public static bool IsDefined<T> (MemberInfo element, bool inherit)
       where T : class
    {
      ArgumentUtility.CheckNotNull ("element", element);
      CheckAttributeType (typeof (T), "T");

      return IsDefined (element, typeof (T), inherit);
    }

    public static bool IsDefined (MemberInfo element, Type attributeType, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("element", element);
      CheckAttributeType (attributeType, "attributeType");

      return GetCustomAttributes (element, attributeType, inherit).Length > 0;
    }

    public static T GetCustomAttribute<T> (MemberInfo element, bool inherit)
        where T: class
    {
      ArgumentUtility.CheckNotNull ("element", element);
      CheckAttributeType (typeof (T), "T");
      
      return (T) (object) GetCustomAttribute (element, typeof (T), inherit);
    }

    public static Attribute GetCustomAttribute (MemberInfo element, Type attributeType, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("element", element);
      CheckAttributeType (attributeType, "attributeType");

      Attribute[] attributeArray = GetCustomAttributes (element, attributeType, inherit);
      if ((attributeArray == null) || (attributeArray.Length == 0))
        return null;
      if (attributeArray.Length != 1)
        throw new AmbiguousMatchException ("Multiple custom attributes of the same type found.");
      return attributeArray[0];
    }

    public static T[] GetCustomAttributes<T> (MemberInfo element, bool inherit)
        where T: class
    {
      ArgumentUtility.CheckNotNull ("element", element);
      CheckAttributeType (typeof (T), "T");
      
      Attribute[] attributesWithMatchingType = GetCustomAttributes (element, typeof (T), inherit);
      return Array.ConvertAll<Attribute, T> (attributesWithMatchingType, delegate (Attribute attribute) { return (T) (object) attribute; });
    }

    public static Attribute[] GetCustomAttributes (MemberInfo element, Type attributeType, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("element", element);
      CheckAttributeType (attributeType, "attributeType");

      Attribute[] attributes = Attribute.GetCustomAttributes (element, typeof (Attribute), inherit);
      return Array.FindAll (attributes, delegate (Attribute attribute) { return attributeType.IsInstanceOfType (attribute); });
    }

    private static void CheckAttributeType (Type attributeType, string parameterName)
    {
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      if (!typeof (Attribute).IsAssignableFrom (attributeType) && !attributeType.IsInterface)
      {
        string message = "The attribute type must be assignable to System.Attribute or an interface.";
        throw new ArgumentTypeException (message, parameterName, typeof (Attribute), attributeType);
      }
    }

    public static bool IsAttributeInherited (Type attributeType)
    {
      AttributeUsageAttribute usage = GetAttributeUsage (attributeType);
      return usage != null ? usage.Inherited : true;
    }

    public static bool IsAttributeAllowMultiple (Type attributeType)
    {
      AttributeUsageAttribute usage = GetAttributeUsage (attributeType);
      return usage != null ? usage.AllowMultiple : false;
    }

    public static AttributeUsageAttribute GetAttributeUsage (Type attributeType)
    {
      AttributeUsageAttribute[] usage =
          (AttributeUsageAttribute[]) attributeType.GetCustomAttributes (typeof (AttributeUsageAttribute), true);
      if (usage.Length == 0)
        return null;
      else
      {
        Assertion.IsTrue (usage.Length == 1, "AllowMultiple == false");
        return usage[0];
      }
    }
  }
}