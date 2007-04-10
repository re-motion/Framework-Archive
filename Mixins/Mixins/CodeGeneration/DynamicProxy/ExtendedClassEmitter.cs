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

    public MethodEmitter ImplementInterfaceMethodByDelegation (MethodInfo interfaceMethod, FieldReference implementer)
    {
      MethodEmitter methodDefinition = CreateInterfaceImplementationMethod (interfaceMethod);
      Debug.Assert (interfaceMethod.DeclaringType.IsAssignableFrom (implementer.Reference.FieldType));
      EmitDelegatingCall (methodDefinition, interfaceMethod, implementer, true);
      return methodDefinition;
    }

    public MethodEmitter ImplementInterfaceMethodByBaseCall (MethodInfo interfaceMethod, MethodInfo baseMethod)
    {
      MethodEmitter methodDefinition = CreateInterfaceImplementationMethod (interfaceMethod);
      EmitDelegatingCall (methodDefinition, baseMethod, SelfReference.Self, false);
      return methodDefinition;
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
  }
}
