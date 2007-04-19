using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Mixins.CodeGeneration.DynamicProxy
{
  public class ExtendedClassEmitter : ClassEmitter
  {
    private Dictionary<MethodEmitter, MethodInfo> _interfaceImplementationMethods = new Dictionary<MethodEmitter, MethodInfo> ();

    public ExtendedClassEmitter (ModuleScope modulescope, string name, Type baseType, Type[] interfaces, bool serializable)
        : base (modulescope, name, baseType, interfaces, serializable)
    {
    }

    public Type BaseType
    {
      get { return TypeBuilder.BaseType; }
    }

    public IDictionary<MethodEmitter, MethodInfo> InterfaceImplementationMethods
    {
      get { return _interfaceImplementationMethods; }
    }

    public void RegisterInterfaceImplementationMethod (MethodInfo interfaceMethod, MethodEmitter implementingMethod)
    {
      _interfaceImplementationMethods.Add (implementingMethod, interfaceMethod);
    }

    public MethodEmitter CreateInterfaceImplementationMethod (MethodInfo interfaceMethod)
    {
      MethodAttributes methodDefinitionAttributes = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.NewSlot
          | MethodAttributes.Virtual | MethodAttributes.Final;
      string methodName = string.Format ("{0}.{1}", interfaceMethod.DeclaringType.FullName, interfaceMethod.Name);
      MethodEmitter methodDefinition = CreateMethod (methodName, methodDefinitionAttributes);
      methodDefinition.CopyParametersAndReturnTypeFrom (interfaceMethod, this);

      TypeBuilder.DefineMethodOverride (methodDefinition.MethodBuilder, interfaceMethod);
      RegisterInterfaceImplementationMethod (interfaceMethod, methodDefinition);

      return methodDefinition;
    }

    public PropertyEmitter CreateInterfaceImplementationProperty (PropertyInfo interfaceProperty)
    {
      string propertyName = string.Format ("{0}.{1}", interfaceProperty.DeclaringType.FullName, interfaceProperty.Name);
      PropertyEmitter newProperty = CreateProperty (propertyName, PropertyAttributes.None, interfaceProperty.PropertyType);

      MethodInfo getMethod = interfaceProperty.GetGetMethod ();
      if (getMethod != null)
      {
        newProperty.GetMethod = CreateInterfaceImplementationMethod (getMethod);
      }

      MethodInfo setMethod = interfaceProperty.GetSetMethod ();
      if (setMethod != null)
      {
        newProperty.SetMethod = CreateInterfaceImplementationMethod (setMethod);
      }

      return newProperty;
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

    public void ImplementPropertyWithField (PropertyEmitter propertyDefinition, FieldReference backingField)
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
  }
}
