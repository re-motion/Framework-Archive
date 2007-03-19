using System;
using System.Reflection;

namespace Rubicon.Utilities
{
  /// <summary>
  /// Utility class for finding custom attributes via their type or an interface implemented by the type.
  /// </summary>
  public static class AttributeUtility
  {
    public static T GetCustomAttribute<T> (MemberInfo element, bool inherit)
        where T: class
    {
      ArgumentUtility.CheckNotNull ("element", element);

      T[] attributeArray = GetCustomAttributes<T> (element, inherit);
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

      if (!typeof (Attribute).IsAssignableFrom (typeof (T)) && ! typeof (T).IsInterface)
        throw new ArgumentException ("The type parameter must be assignable to System.Attribute or an interface.", "T");
      
      Attribute[] attributes = Attribute.GetCustomAttributes (element, typeof (Attribute), inherit);
      Attribute[] attributesWithMatchingType = Array.FindAll (attributes, delegate (Attribute attribute) { return attribute is T; });
      return Array.ConvertAll<Attribute, T> (attributesWithMatchingType, delegate (Attribute attribute) { return (T) (object) attribute; });
    }
  }
}