using System;
using System.Reflection;
using System.Runtime.Serialization;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Rubicon.Utilities;

namespace Rubicon.CodeGeneration
{
  public static class SerializationHelper
  {
    

    private static bool IsPublicOrProtected (MethodBase method)
    {
      return method.IsPublic || method.IsFamily || method.IsFamilyOrAssembly;
    }

    public static void ImplementGetObjectDataByDelegation (CustomClassEmitter classEmitter, Func<MethodEmitter, bool, MethodInvocationExpression> delegatingMethodInvocationGetter)
    {
      ArgumentUtility.CheckNotNull ("delegatingMethodInvocationGetter", delegatingMethodInvocationGetter);

      bool baseIsISerializable = typeof (ISerializable).IsAssignableFrom (classEmitter.BaseType);

      MethodInfo getObjectDataMethod =
          typeof (ISerializable).GetMethod ("GetObjectData", new Type[] {typeof (SerializationInfo), typeof (StreamingContext)});
      MethodEmitter newMethod = classEmitter.CreateInterfaceMethodImplementation (getObjectDataMethod).InnerEmitter;

      MethodInvocationExpression delegatingMethodInvocation = delegatingMethodInvocationGetter (newMethod, baseIsISerializable);
      if (delegatingMethodInvocation != null)
        newMethod.CodeBuilder.AddStatement (new ExpressionStatement (delegatingMethodInvocation));

      if (baseIsISerializable)
      {
        ConstructorInfo baseConstructor = classEmitter.BaseType.GetConstructor (
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            CallingConventions.Any,
            new Type[] {typeof (SerializationInfo), typeof (StreamingContext)},
            null);
        if (baseConstructor == null || !IsPublicOrProtected (baseConstructor))
        {
          string message = string.Format (
              "No public or protected deserialization constructor in type {0} - this is not supported.",
              classEmitter.BaseType.FullName);
          throw new NotSupportedException (message);
        }

        MethodInfo baseGetObjectDataMethod =
            classEmitter.BaseType.GetMethod ("GetObjectData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (baseGetObjectDataMethod == null || !IsPublicOrProtected (baseGetObjectDataMethod))
        {
          string message = string.Format ("No public or protected GetObjectData in {0} - this is not supported.", classEmitter.BaseType.FullName);
          throw new NotSupportedException (message);
        }
        newMethod.CodeBuilder.AddStatement (
            new ExpressionStatement (
                new MethodInvocationExpression (
                    SelfReference.Self,
                    baseGetObjectDataMethod,
                    new ReferenceExpression (newMethod.Arguments[0]),
                    new ReferenceExpression (newMethod.Arguments[1]))));
      }
      newMethod.CodeBuilder.AddStatement (new ReturnStatement());
    }
  }
}