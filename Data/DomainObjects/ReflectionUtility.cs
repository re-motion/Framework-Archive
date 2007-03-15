using System;
using System.IO;
using System.Reflection;
using Rubicon.Utilities;
using System.Diagnostics;

namespace Rubicon.Data.DomainObjects
{
/// <summary>
/// Utility class for methods using reflection.
/// </summary>
public sealed class ReflectionUtility
{
  // types

  // static members and constants

  /// <summary>
  /// Returns the directory of the current executing assembly.
  /// </summary>
  /// <returns>The path of the current executing assembly</returns>
  public static string GetExecutingAssemblyPath ()
  {
    AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName ();

    Uri codeBaseUri = new Uri (assemblyName.CodeBase);
    return Path.GetDirectoryName (codeBaseUri.LocalPath);
  }

  /// <summary>
  /// Creates an object of a given type.
  /// </summary>
  /// <param name="type">The <see cref="System.Type"/> of the object to instantiate. Must not be <see langword="null"/>.</param>
  /// <param name="constructorParameters">The parameters for the constructor of the object.</param>
  /// <returns>The object that has been created.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException">Type <paramref name="type"/> has no suitable constructor for the given <paramref name="constructorParameters"/>.</exception>
  public static object CreateObject (Type type, params object[] constructorParameters)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    Type[] constructorParameterTypes = new Type[constructorParameters.Length];
    for (int i = 0; i < constructorParameterTypes.Length; i++)
    {
      constructorParameterTypes[i] = constructorParameters[i].GetType (); 
    }

    ConstructorInfo constructor = type.GetConstructor (
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, 
        null, 
        constructorParameterTypes, 
        null);

    if (constructor != null)
    {
      return constructor.Invoke (constructorParameters);
    }
    else
    {
      throw new ArgumentException (string.Format (
          "Type '{0}' has no suitable constructor. Parameter types: ({1})",
          type, 
          GetTypeListAsString (constructorParameterTypes)));
    }
  }
  
  internal static string GetTypeListAsString (Type[] types)
  {
    ArgumentUtility.CheckNotNull ("types", types);
    string result = string.Empty;
    foreach (Type type in types)
    {
      if (result != string.Empty)
        result += ", ";

      if (type != null)
      {
        result += type.ToString ();
      }
      else
      {
        result += "<any reference type>";
      }
    }

    return result;
  }

  /// <summary>
  /// Checks whether a given member is a property accessor method.
  /// </summary>
  /// <param name="memberInfo">The member to be checked.</param>
  /// <returns>True if the given member is either a getter or a setter method, false otherwise.</returns>
  /// <exception cref="ArgumentNullException">The argument <paramref name="memberInfo"/> is null.</exception>
  public static bool IsPropertyAccessor (MethodBase memberInfo)
  {
    ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);
    return IsPropertyGetter (memberInfo) || IsPropertySetter (memberInfo);
  }

  /// <summary>
  /// Checks whether a given member is a property getter method.
  /// </summary>
  /// <param name="memberInfo">The member to be checked.</param>
  /// <returns>True if the given member is a property's getter method, false otherwise.</returns>
  /// <exception cref="ArgumentNullException">The argument <paramref name="memberInfo"/> is null.</exception>
  public static bool IsPropertyGetter (MethodBase memberInfo)
  {
    ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);
    return memberInfo.MemberType == MemberTypes.Method
        && memberInfo.IsSpecialName
        && memberInfo.Name.StartsWith ("get_");
  }

  /// <summary>
  /// Checks whether a given member is a property setter method.
  /// </summary>
  /// <param name="memberInfo">The member to be checked.</param>
  /// <returns>True if the given member is a property's setter method, false otherwise.</returns>
  /// <exception cref="ArgumentNullException">The argument <paramref name="memberInfo"/> is null.</exception>
  public static bool IsPropertySetter (MethodBase memberInfo)
  {
    ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);
    return memberInfo.MemberType == MemberTypes.Method
        && memberInfo.IsSpecialName
        && memberInfo.Name.StartsWith ("set_");
  }

  /// <summary>
  /// Returns the property name for a given property accessor method name, or null if the method name is not the name of a property accessor method.
  /// </summary>
  /// <param name="methodName">The name of the presumed property accessor method.</param>
  /// <returns>The property name for the given method name, or null if it is not the name of a property accessor method.</returns>
  /// <exception cref="ArgumentNullException">The <paramref name="methodName"/> parameter is null.</exception>
  public static string GetPropertyNameForMethodName (string methodName)
  {
    ArgumentUtility.CheckNotNull ("methodName", methodName);
    if (methodName.Length <= 4 || (!methodName.StartsWith ("get_") && !methodName.StartsWith ("set_")))
    {
      return null;
    }
    else
    {
      return methodName.Substring (4);
    }
  }

  /// <summary>
  /// Retrieves the <see cref="PropertyInfo"/> object for the given property accessor method, or null if the method is not a property accessor method.
  /// </summary>
  /// <param name="method">The presumed accessor method whose property should be retrieved</param>
  /// <returns>The corresponing <see cref="PropertyInfo"/>, or null if the method is not a property accessor or no corresponding property could be
  /// found.</returns>
  /// <exception cref="ArgumentNullException">The <paramref name="method"/> parameter was null.</exception>
  public static PropertyInfo GetPropertyForMethod (MethodInfo method)
  {
    ArgumentUtility.CheckNotNull ("method", method);
    string propertyName = GetPropertyNameForMethodName (method.Name);
    if (propertyName == null || !method.IsSpecialName)
    {
      return null;
    }
    else
    {
      BindingFlags bindingFlags;
      if (method.IsStatic)
      {
        bindingFlags = BindingFlags.Static;
      }
      else
      {
        bindingFlags = BindingFlags.Instance;
      }
      if (method.IsPublic)
      {
        bindingFlags |= BindingFlags.Public;
      }
      else
      {
        bindingFlags |= BindingFlags.NonPublic;
      }

      return method.DeclaringType.GetProperty (propertyName, bindingFlags);
    }
  }

  // member fields

  // construction and disposing

  private ReflectionUtility ()
  {
  }

  // methods and properties

}
}
