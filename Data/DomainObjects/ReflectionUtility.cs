using System;
using System.IO;
using System.Reflection;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
//TODO documentation: Write summary for class
public sealed class ReflectionUtility
{
  // types

  // static members and constants

  public static string GetExecutingAssemblyPath ()
  {
    AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName ();

    Uri codeBaseUri = new Uri (assemblyName.CodeBase);
    return Path.GetDirectoryName (codeBaseUri.LocalPath);
  }

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
