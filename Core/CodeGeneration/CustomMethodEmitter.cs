using System;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Rubicon.CodeGeneration.DPExtensions;
using Rubicon.Utilities;

namespace Rubicon.CodeGeneration
{
  public class CustomMethodEmitter : IAttributableEmitter
  {
    private readonly MethodEmitter _innerEmitter;
    private readonly AbstractTypeEmitter _parentEmitter;

    public CustomMethodEmitter (CustomClassEmitter parentEmitter, string name, MethodAttributes attributes)
    {
      _parentEmitter = parentEmitter.InnerEmitter;
      _innerEmitter = _parentEmitter.CreateMethod (name, attributes);
      _innerEmitter.SetReturnType (typeof (void));
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      _innerEmitter.MethodBuilder.SetCustomAttribute (customAttribute);
    }

    public void SetParameters (params Type[] parameters)
    {
      ArgumentUtility.CheckNotNull ("parameters", parameters);
      InnerEmitter.SetParameters (parameters);
    }

    public void SetReturnType (Type returnType)
    {
      ArgumentUtility.CheckNotNull ("returnType", returnType);
      InnerEmitter.SetReturnType (returnType);
    }

    public void CopyParametersAndReturnTypeFrom (MethodInfo method)
    {
      _innerEmitter.CopyParametersAndReturnTypeFrom (method, _parentEmitter); // TODO: change to get rid of _parentEmitter
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
      ArgumentUtility.CheckNotNull ("baseMethod", baseMethod);

      if (baseMethod.IsAbstract)
        throw new ArgumentException (string.Format ("The given method {0}.{1} is abstract.", baseMethod.DeclaringType.FullName, baseMethod.Name),
            "baseMethod");
      
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

    public CustomMethodEmitter AddStatement (Statement statement)
    {
      ArgumentUtility.CheckNotNull ("statement", statement);
      InnerEmitter.CodeBuilder.AddStatement (statement);
      return this;
    }
  }
}