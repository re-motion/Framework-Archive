using System;
using System.Reflection;
using System.Runtime.Serialization;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Rubicon.Utilities;

namespace Rubicon.CodeGeneration
{
  public static class SerializationImplementer
  {
    private static bool IsPublicOrProtected (MethodBase method)
    {
      return method.IsPublic || method.IsFamily || method.IsFamilyOrAssembly;
    }

    public static void ImplementGetObjectDataByDelegation (
        CustomClassEmitter classEmitter, Func<CustomMethodEmitter, bool, MethodInvocationExpression> delegatingMethodInvocationGetter)
    {
      ArgumentUtility.CheckNotNull ("delegatingMethodInvocationGetter", delegatingMethodInvocationGetter);

      bool baseIsISerializable = typeof (ISerializable).IsAssignableFrom (classEmitter.BaseType);

      MethodInfo getObjectDataMethod =
          typeof (ISerializable).GetMethod ("GetObjectData", new Type[] {typeof (SerializationInfo), typeof (StreamingContext)});
      CustomMethodEmitter newMethod = classEmitter.CreatePublicInterfaceMethodImplementation (getObjectDataMethod);

      if (baseIsISerializable)
        ImplementBaseGetObjectDataCall (classEmitter, newMethod);

      MethodInvocationExpression delegatingMethodInvocation = delegatingMethodInvocationGetter (newMethod, baseIsISerializable);
      if (delegatingMethodInvocation != null)
        newMethod.AddStatement (new ExpressionStatement (delegatingMethodInvocation));

      newMethod.ImplementByReturningVoid();
    }

    private static void ImplementBaseGetObjectDataCall (CustomClassEmitter classEmitter, CustomMethodEmitter getObjectDataMethod)
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
      getObjectDataMethod.AddStatement (
          new ExpressionStatement (
              new MethodInvocationExpression (
                  SelfReference.Self,
                  baseGetObjectDataMethod,
                  new ReferenceExpression (getObjectDataMethod.ArgumentReferences[0]),
                  new ReferenceExpression (getObjectDataMethod.ArgumentReferences[1]))));
    }

    public static void ImplementDeserializationConstructorByThrowing (CustomClassEmitter classEmitter)
    {
      ConstructorEmitter emitter = classEmitter.CreateConstructor (new Type[] {typeof (SerializationInfo), typeof (StreamingContext)});
      emitter.CodeBuilder.AddStatement (
          new ThrowStatement (
              typeof (NotImplementedException),
              "The deserialization constructor should never be called; generated types are deserialized via IObjectReference helpers."));
    }

    public static void ImplementDeserializationConstructorByThrowingIfNotExistsOnBase (CustomClassEmitter classEmitter)
    {
      Type[] serializationConstructorSignature = new Type[] {typeof (SerializationInfo), typeof (StreamingContext)};
      ConstructorInfo baseConstructor = classEmitter.BaseType.GetConstructor (
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
          null,
          serializationConstructorSignature,
          null);
      if (baseConstructor == null)
        SerializationImplementer.ImplementDeserializationConstructorByThrowing (classEmitter);
    }
  }
}