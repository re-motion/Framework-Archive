using System;
using System.IO;
using System.Reflection;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
//Documentation: All done

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
  /// <param name="type">The <see cref="System.Type"/> of the object to instantiate.</param>
  /// <param name="constructorParameters">The parameters for the constructor of the object.</param>
  /// <returns>The object that has been created.</returns>
  /// <exception cref="System.ArgumentNullException"><i>type</i> is a null reference.</exception>
  /// <exception cref="System.ArgumentException">Type <i>type</i> has no suitable constructor for the given <i>constructorParameters</i>.</exception>
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
  
  private static string GetTypeListAsString (Type[] types)
  {
    string result = string.Empty;
    foreach (Type type in types)
    {
      if (result != string.Empty)
        result += ", ";

      result += type.ToString ();
    }

    return result;
  }

  // member fields

  // construction and disposing

  private ReflectionUtility ()
  {
  }

  // methods and properties

}
}
