using System;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Definitions
{
  public class SignatureChecker
  {
    public bool SignatureMatch (MethodInfo methodOne, MethodInfo methodTwo)
    {
      ArgumentUtility.CheckNotNull ("methodOne", methodOne);
      ArgumentUtility.CheckNotNull ("methodTwo", methodTwo);

      if (!TypeEquals (methodOne.ReturnType, methodTwo.ReturnType))
      {
        return false;
      }

      if (!ParametersMatch (methodOne, methodTwo))
      {
        return false;
      }

      if (!GenericParametersMatch (methodOne, methodTwo))
      {
        return false;
      }

      return true;
    }

    private bool ParametersMatch (MethodInfo methodOne, MethodInfo methodTwo)
    {
      ParameterInfo[] paramsOne = methodOne.GetParameters ();
      ParameterInfo[] paramsTwo = methodTwo.GetParameters ();
      if (paramsOne.Length != paramsTwo.Length)
      {
        return false;
      }

      for (int i = 0; i < paramsOne.Length; ++i)
      {
        if (!ParametersEqual (paramsOne[i], paramsTwo[i]))
        {
          return false;
        }
      }
      return true;
    }

    private bool GenericParametersMatch (MethodInfo methodOne, MethodInfo methodTwo)
    {
      if (methodOne.IsGenericMethod != methodTwo.IsGenericMethod)
      {
        return false;
      }

      if (!methodOne.IsGenericMethod)
      {
        return true;
      }

      Type[] genericArgsOne = methodOne.GetGenericArguments ();
      Type[] genericArgsTwo = methodTwo.GetGenericArguments ();

      if (genericArgsOne.Length != genericArgsTwo.Length)
      {
        return false;
      }

      for (int i = 0; i < genericArgsOne.Length; ++i)
      {
        if (!TypeEquals (genericArgsOne[i], genericArgsTwo[i]))
        {
          return false;
        }
      }
      return true;
    }

    private bool ParametersEqual (ParameterInfo one, ParameterInfo two)
    {
      return one.IsIn == two.IsIn
            && one.IsOut == two.IsOut
            && TypeEquals (one.ParameterType, two.ParameterType);
    }

    private bool TypeEquals (Type one, Type two)
    {
      if (one.IsGenericParameter != two.IsGenericParameter)
      {
        return false;
      }

      if (one.IsGenericParameter)
      {
        return (one.GenericParameterAttributes == two.GenericParameterAttributes) && (one.GenericParameterPosition == two.GenericParameterPosition);
      }
      else
      {
        return one.Equals (two);
      }
    }
  }
}
