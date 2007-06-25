using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Rubicon.Mixins.CodeGeneration.DynamicProxy.DPExtensions;

namespace Rubicon.Mixins.CodeGeneration.DynamicProxy
{
  public class CustomMethodEmitter : IAttributableEmitter
  {
    private MethodEmitter _innerEmitter;
    private AbstractTypeEmitter _parentEmitter;

    public CustomMethodEmitter (AbstractTypeEmitter parentEmitter, string name, MethodAttributes attributes)
    {
      _parentEmitter = parentEmitter;
      _innerEmitter = parentEmitter.CreateMethod (name, attributes);
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      _innerEmitter.MethodBuilder.SetCustomAttribute (customAttribute);
    }

    public void CopyParametersAndReturnTypeFrom (MethodInfo method)
    {
      _innerEmitter.CopyParametersAndReturnTypeFrom (method, _parentEmitter);
    }

    public MethodBuilder MethodBuilder
    {
      get { return _innerEmitter.MethodBuilder; }
    }

    public MethodEmitter InnerEmitter
    {
      get { return _innerEmitter; }
    }

    public void ImplementMethodByDelegation (Reference implementer, MethodInfo methodToCall)
    {
      AddDelegatingCallStatements (methodToCall, implementer, true);
    }

    public void ImplementMethodByBaseCall (MethodInfo baseMethod)
    {
      AddDelegatingCallStatements (baseMethod, SelfReference.Self, false);
    }

    public void ImplementMethodByThrowing (Type exceptionType, string message)
    {
      InnerEmitter.CodeBuilder.AddStatement (new ThrowStatement (exceptionType, message));
    }

    private void AddDelegatingCallStatements (MethodInfo methodToCall, Reference owner, bool callVirtual)
    {
      Expression[] argumentExpressions = new Expression[InnerEmitter.Arguments.Length];
      for (int i = 0; i < argumentExpressions.Length; ++i)
      {
        argumentExpressions[i] = InnerEmitter.Arguments[i].ToExpression ();
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

      InnerEmitter.CodeBuilder.AddStatement (new ReturnStatement (delegatingCall));
    }
  }
}