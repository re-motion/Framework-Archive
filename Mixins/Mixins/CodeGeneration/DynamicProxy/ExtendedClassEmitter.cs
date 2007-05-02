using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Mixins.CodeGeneration.DynamicProxy.DPExtensions;

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

    public CustomMethodEmitter CreateInterfaceImplementationMethod (MethodInfo interfaceMethod)
    {
      MethodAttributes methodDefinitionAttributes = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.NewSlot
          | MethodAttributes.Virtual | MethodAttributes.Final;
      if (interfaceMethod.IsSpecialName)
      {
        methodDefinitionAttributes |= MethodAttributes.SpecialName;
      }
      string methodName = string.Format ("{0}.{1}", interfaceMethod.DeclaringType.FullName, interfaceMethod.Name);
      CustomMethodEmitter methodDefinition = new CustomMethodEmitter (InnerEmitter, methodName, methodDefinitionAttributes);
      methodDefinition.CopyParametersAndReturnTypeFrom (interfaceMethod, InnerEmitter);

      TypeBuilder.DefineMethodOverride (methodDefinition.MethodBuilder, interfaceMethod);

      return methodDefinition;
    }

    // does not create the property's methods
    public CustomPropertyEmitter CreateInterfaceImplementationProperty (PropertyInfo interfaceProperty)
    {
      string propertyName = string.Format ("{0}.{1}", interfaceProperty.DeclaringType.FullName, interfaceProperty.Name);
      Type[] indexParameterTypes =
          Array.ConvertAll<ParameterInfo, Type> (interfaceProperty.GetIndexParameters(), delegate (ParameterInfo p) { return p.ParameterType; });

      CustomPropertyEmitter newProperty = new CustomPropertyEmitter (InnerEmitter, propertyName, PropertyAttributes.None, interfaceProperty.PropertyType,
        indexParameterTypes);

      return newProperty;
    }

    public CustomEventEmitter CreateInterfaceImplementationEvent (EventInfo interfaceEvent)
    {
      string eventName = string.Format ("{0}.{1}", interfaceEvent.DeclaringType.FullName, interfaceEvent.Name);
      CustomEventEmitter newEvent = new CustomEventEmitter (InnerEmitter, eventName, EventAttributes.None, interfaceEvent.EventHandlerType);
      return newEvent;
    }

    public void ImplementMethodByDelegation (MethodEmitter methodDefinition, Reference implementer, MethodInfo methodToCall)
    {
      EmitDelegatingCall (methodDefinition, methodToCall, implementer, true);
    }

    public void ImplementMethodByBaseCall (MethodEmitter methodDefinition, MethodInfo baseMethod)
    {
      EmitDelegatingCall (methodDefinition, baseMethod, SelfReference.Self, false);
    }

    public void EmitDelegatingCall (MethodEmitter methodDefinition, MethodInfo methodToCall, Reference owner, bool callVirtual)
    {
      Expression[] argumentExpressions = new Expression[methodDefinition.Arguments.Length];
      for (int i = 0; i < argumentExpressions.Length; ++i)
      {
        argumentExpressions[i] = methodDefinition.Arguments[i].ToExpression ();
      }

      MethodInvocationExpression delegatingCall;
      if (callVirtual)
      {
        delegatingCall = new VirtualMethodInvocationExpression (owner, methodToCall, argumentExpressions);
      }
      else
      {
        delegatingCall = new MethodInvocationExpression (owner, methodToCall, argumentExpressions);
      }

      methodDefinition.CodeBuilder.AddStatement (new ReturnStatement (delegatingCall));
    }

    public void ImplementPropertyWithField (CustomPropertyEmitter propertyDefinition, FieldReference backingField)
    {
      if (propertyDefinition.GetMethod != null)
      {
        propertyDefinition.GetMethod.CodeBuilder.AddStatement (new ReturnStatement (backingField));
      }
      if (propertyDefinition.SetMethod != null)
      {
        propertyDefinition.SetMethod.CodeBuilder.AddStatement (
            new AssignStatement (backingField, propertyDefinition.SetMethod.Arguments[0].ToExpression()));
      }
    }

    public LocalReference EmitMakeReferenceOfExpression (MethodEmitter methodDefinition, Type referenceType, Expression expression)
    {
      LocalReference reference = methodDefinition.CodeBuilder.DeclareLocal (referenceType);
      methodDefinition.CodeBuilder.AddStatement (new AssignStatement (reference, expression));
      return reference;
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      TypeBuilder.SetCustomAttribute (customAttribute);
    }
  }
}
