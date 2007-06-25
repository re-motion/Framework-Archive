using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Mixins.CodeGeneration.DynamicProxy.DPExtensions;
using Rubicon;
using Rubicon.Utilities;

namespace Mixins.CodeGeneration.DynamicProxy
{
  public class ExtendedClassEmitter : IAttributableEmitter
  {
    private AbstractTypeEmitter _innerEmitter;

    public ExtendedClassEmitter (AbstractTypeEmitter innerEmitter)
    {
      _innerEmitter = innerEmitter;
    }

    public AbstractTypeEmitter InnerEmitter
    {
      get { return _innerEmitter; }
    }

    public Type BaseType
    {
      get { return TypeBuilder.BaseType; }
    }

    public TypeBuilder TypeBuilder
    {
      get { return InnerEmitter.TypeBuilder; }
    }

    public CustomMethodEmitter CreateMethodOverrideOrInterfaceImplementation (MethodInfo baseOrInterfaceMethod)
    {
      ArgumentUtility.CheckNotNull ("baseOrInterfaceMethod", baseOrInterfaceMethod);
      Assertion.Assert (baseOrInterfaceMethod.IsVirtual);

      MethodAttributes methodDefinitionAttributes = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.NewSlot
          | MethodAttributes.Virtual | MethodAttributes.Final;
      if (baseOrInterfaceMethod.IsSpecialName)
      {
        methodDefinitionAttributes |= MethodAttributes.SpecialName;
      }
      string methodName = string.Format ("{0}.{1}", baseOrInterfaceMethod.DeclaringType.FullName, baseOrInterfaceMethod.Name);
      CustomMethodEmitter methodDefinition = new CustomMethodEmitter (InnerEmitter, methodName, methodDefinitionAttributes);
      methodDefinition.CopyParametersAndReturnTypeFrom (baseOrInterfaceMethod);

      TypeBuilder.DefineMethodOverride (methodDefinition.MethodBuilder, baseOrInterfaceMethod);

      return methodDefinition;
    }

    // does not create the property's methods
    public CustomPropertyEmitter CreatePropertyOverrideOrInterfaceImplementation (PropertyInfo baseOrInterfaceProperty)
    {
      ArgumentUtility.CheckNotNull ("baseOrInterfaceProperty", baseOrInterfaceProperty);

      string propertyName = string.Format ("{0}.{1}", baseOrInterfaceProperty.DeclaringType.FullName, baseOrInterfaceProperty.Name);
      Type[] indexParameterTypes =
          Array.ConvertAll<ParameterInfo, Type> (baseOrInterfaceProperty.GetIndexParameters(), delegate (ParameterInfo p) { return p.ParameterType; });

      CustomPropertyEmitter newProperty = new CustomPropertyEmitter (InnerEmitter, propertyName, PropertyAttributes.None, baseOrInterfaceProperty.PropertyType,
        indexParameterTypes);

      return newProperty;
    }

    // does not create the event's methods
    public CustomEventEmitter CreateEventOverrideOrInterfaceImplementation (EventInfo baseOrInterfaceEvent)
    {
      ArgumentUtility.CheckNotNull ("baseOrInterfaceEvent", baseOrInterfaceEvent);

      string eventName = string.Format ("{0}.{1}", baseOrInterfaceEvent.DeclaringType.FullName, baseOrInterfaceEvent.Name);
      CustomEventEmitter newEvent = new CustomEventEmitter (InnerEmitter, eventName, EventAttributes.None, baseOrInterfaceEvent.EventHandlerType);
      return newEvent;
    }

    public LocalReference AddMakeReferenceOfExpressionStatements (CustomMethodEmitter methodDefinition, Type referenceType, Expression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      LocalReference reference = methodDefinition.InnerEmitter.CodeBuilder.DeclareLocal (referenceType);
      methodDefinition.InnerEmitter.CodeBuilder.AddStatement (new AssignStatement (reference, expression));
      return reference;
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      TypeBuilder.SetCustomAttribute (customAttribute);
    }

    public void ReplicateBaseTypeConstructors()
    {
      ConstructorInfo[] constructors = BaseType.GetConstructors (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      foreach (ConstructorInfo constructor in constructors)
      {
        if (constructor.IsPublic | constructor.IsFamily)
        {
          ReplicateBaseTypeConstructor (constructor);
        }
      }
    }

    private void ReplicateBaseTypeConstructor (ConstructorInfo constructor)
    {
      ArgumentUtility.CheckNotNull ("constructor", constructor);
      ArgumentReference[] arguments = ArgumentsUtil.ConvertToArgumentReference (constructor.GetParameters ());
      ConstructorEmitter newConstructor = InnerEmitter.CreateConstructor (arguments);

      Expression[] argumentExpressions = ArgumentsUtil.ConvertArgumentReferenceToExpression (arguments);
      newConstructor.CodeBuilder.AddStatement (new ConstructorInvocationStatement (constructor, argumentExpressions));

      newConstructor.CodeBuilder.AddStatement (new ReturnStatement ());
    }

    public void ImplementGetObjectDataByDelegation(Func<MethodEmitter, bool, MethodInvocationExpression> delegatingMethodInvocationGetter)
    {
      ArgumentUtility.CheckNotNull ("delegatingMethodInvocationGetter", delegatingMethodInvocationGetter);

      bool baseIsISerializable = typeof (ISerializable).IsAssignableFrom (BaseType);

      MethodInfo getObjectDataMethod =
          typeof (ISerializable).GetMethod ("GetObjectData", new Type[] {typeof (SerializationInfo), typeof (StreamingContext)});
      MethodEmitter newMethod = CreateMethodOverrideOrInterfaceImplementation (getObjectDataMethod).InnerEmitter;

      MethodInvocationExpression delegatingMethodInvocation = delegatingMethodInvocationGetter(newMethod, baseIsISerializable);
      if (delegatingMethodInvocation != null)
        newMethod.CodeBuilder.AddStatement (new ExpressionStatement (delegatingMethodInvocation));

      if (baseIsISerializable)
      {
        ConstructorInfo baseConstructor = BaseType.GetConstructor (
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            CallingConventions.Any,
            new Type[] {typeof (SerializationInfo), typeof (StreamingContext)},
            null);
        if (baseConstructor == null || (!baseConstructor.IsPublic && !baseConstructor.IsFamily))
        {
          string message = string.Format (
              "No public or protected deserialization constructor in type {0} - this is not supported.",
              BaseType.FullName);
          throw new NotSupportedException (message);
        }

        MethodInfo baseGetObjectDataMethod =
            BaseType.GetMethod ("GetObjectData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (baseGetObjectDataMethod == null || (!baseGetObjectDataMethod.IsPublic && !baseGetObjectDataMethod.IsFamily))
        {
          string message = string.Format ("No public or protected GetObjectData in {0} - this is not supported.", BaseType.FullName);
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
