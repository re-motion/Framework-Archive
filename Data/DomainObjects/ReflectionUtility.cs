using System;
using System.IO;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Utility class for methods using reflection.
  /// </summary>
  public static class ReflectionUtility
  {
    // types

    // static members and constants

    /// <summary>
    /// Returns the directory of the current executing assembly.
    /// </summary>
    /// <returns>The path of the current executing assembly</returns>
    public static string GetExecutingAssemblyPath()
    {
      return GetAssemblyPath (Assembly.GetExecutingAssembly());
    }

    public static string GetAssemblyPath(Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      Uri codeBaseUri = new Uri (assembly.CodeBase);
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
        constructorParameterTypes[i] = constructorParameters[i].GetType();

      ConstructorInfo constructor = type.GetConstructor (
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
          null,
          constructorParameterTypes,
          null);

      if (constructor != null)
        return constructor.Invoke (constructorParameters);
      else
      {
        throw new ArgumentException (
            string.Format (
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
          result += type.ToString();
        else
          result += "<any reference type>";
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
        return null;
      else
        return methodName.Substring (4);
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
        return null;
      else
      {
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
        if (method.IsStatic)
          bindingFlags |= BindingFlags.Static;
        else
          bindingFlags |= BindingFlags.Instance;

        return method.DeclaringType.GetProperty (propertyName, bindingFlags);
      }
    }

    public static string GetSignatureForArguments (object[] args)
    {
      Type[] argumentTypes = GetTypesForArgs (args);
      return GetTypeListAsString (argumentTypes);
    }

    public static Type[] GetTypesForArgs (object[] args)
    {
      Type[] types = new Type[args.Length];
      for (int i = 0; i < args.Length; i++)
      {
        if (args[i] == null)
          types[i] = null;
        else
          types[i] = args[i].GetType();
      }
      return types;
    }

    /// <summary>
    /// Returns the reflection based property identifier for a given property member.
    /// </summary>
    /// <param name="propertyInfo">The property whose identifier should be returned. Must not be <see langword="null" />.</param>
    /// <returns>The property identifier for the given property.</returns>
    /// <remarks>
    /// Currently, the identifier is defined to be the full name of the property's declaring type, suffixed with a dot (".") and the
    /// property's name (e.g. MyNamespace.MyType.MyProperty). However, this might change in the future, so this API should be used whenever the
    /// identifier must be retrieved programmatically.
    /// </remarks>
    // TODO: Move to reflection based mapping
    public static string GetPropertyName (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      return GetOriginalDeclaringType (propertyInfo).FullName + "." + propertyInfo.Name;
    }

    /// <summary>
    /// Returns the <see cref="Type"/> where the property was initially decelared.
    /// </summary>
    /// <param name="propertyInfo">The property whose identifier should be returned. Must not be <see langword="null" />.</param>
    /// <returns>The <see cref="Type"/> where the property was declared for the first time.</returns>
    public static Type GetOriginalDeclaringType (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      
      MethodInfo[] accessors = propertyInfo.GetAccessors (true);
      if (accessors.Length == 0)
      {
        throw new ArgumentException (
            string.Format ("The property does not define any accessors.\r\n  Type: {0}, property: {1}", propertyInfo.DeclaringType, propertyInfo.Name),
            "propertyInfo");
      }

      Type baseDeclaringType = accessors[0].GetBaseDefinition().DeclaringType;
      for (int i = 1; i < accessors.Length; i++)
      {
        if (accessors[i].GetBaseDefinition().DeclaringType.IsSubclassOf (baseDeclaringType))
          baseDeclaringType = accessors[i].GetBaseDefinition().DeclaringType;
      }

      return baseDeclaringType;
    }
  }
}