using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Mixins.CodeGeneration.DynamicProxy.DPExtensions;
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
      Assertion.Assert (baseOrInterfaceMethod.IsVirtual);

      MethodAttributes methodDefinitionAttributes = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.NewSlot
          | MethodAttributes.Virtual | MethodAttributes.Final;
      if (baseOrInterfaceMethod.IsSpecialName)
      {
        methodDefinitionAttributes |= MethodAttributes.SpecialName;
      }
      string methodName = string.Format ("{0}.{1}", baseOrInterfaceMethod.DeclaringType.FullName, baseOrInterfaceMethod.Name);
      CustomMethodEmitter methodDefinition = new CustomMethodEmitter (InnerEmitter, methodName, methodDefinitionAttributes);
      methodDefinition.CopyParametersAndReturnTypeFrom (baseOrInterfaceMethod, InnerEmitter);

      TypeBuilder.DefineMethodOverride (methodDefinition.MethodBuilder, baseOrInterfaceMethod);

      return methodDefinition;
    }

    // does not create the property's methods
    public CustomPropertyEmitter CreatePropertyOverrideOrInterfaceImplementation (PropertyInfo baseOrInterfaceProperty)
    {
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
      string eventName = string.Format ("{0}.{1}", baseOrInterfaceEvent.DeclaringType.FullName, baseOrInterfaceEvent.Name);
      CustomEventEmitter newEvent = new CustomEventEmitter (InnerEmitter, eventName, EventAttributes.None, baseOrInterfaceEvent.EventHandlerType);
      return newEvent;
    }

    public LocalReference AddMakeReferenceOfExpressionStatements (CustomMethodEmitter methodDefinition, Type referenceType, Expression expression)
    {
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
      ArgumentReference[] arguments = ArgumentsUtil.ConvertToArgumentReference (constructor.GetParameters ());
      ConstructorEmitter newConstructor = InnerEmitter.CreateConstructor (arguments);

      Expression[] argumentExpressions = ArgumentsUtil.ConvertArgumentReferenceToExpression (arguments);
      newConstructor.CodeBuilder.AddStatement (new ConstructorInvocationStatement (constructor, argumentExpressions));

      newConstructor.CodeBuilder.AddStatement (new ReturnStatement ());
      newConstructor.Generate ();
    }
  }
}
