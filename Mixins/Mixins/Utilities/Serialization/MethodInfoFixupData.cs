using System;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Utilities.Serialization
{
  [Serializable]
  public class MethodInfoFixupData
  {
    private const BindingFlags _bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    private string _typeName;
    private string _methodName;
    private string[] _argumentTypeNames;

    public MethodInfoFixupData (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);

      _typeName = method.DeclaringType.AssemblyQualifiedName;
      _methodName = method.Name;
      ParameterInfo[] parameters = method.GetParameters ();
      _argumentTypeNames = new string[parameters.Length];
      for (int i = 0; i < parameters.Length; ++i)
        _argumentTypeNames[i] = parameters[i].ParameterType.AssemblyQualifiedName;
    }

    public MethodInfo GetMethodInfo ()
    {
      Type declaringType = Type.GetType (_typeName);
      Type[] argumentTypes = new Type[_argumentTypeNames.Length];
      for (int i = 0; i < _argumentTypeNames.Length; ++i)
      {
        argumentTypes[i] = Type.GetType (_argumentTypeNames[i]);
      }
      MethodInfo method = declaringType.GetMethod (_methodName, _bindingFlags, null, argumentTypes, null);
      Assertion.Assert (method != null);
      return method;
    }

    internal static object PrepareMethodInfo (object data)
    {
      MethodInfo methodInfo = data as MethodInfo;
      if (methodInfo == null)
        throw new ArgumentException ("Invalid data object - MethodInfo expected.", "data");
      return new MethodInfoFixupData (methodInfo);
    }

    internal static object FixupMethodInfo (object data)
    {
      MethodInfoFixupData fixupData = data as MethodInfoFixupData;
      if (fixupData == null)
        throw new ArgumentException ("Invalid data object - MethodInfoFixupData expected.", "data");
      return fixupData.GetMethodInfo ();
    }

  }
}