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
    private readonly string _name;
    
    private Type _returnType;
    private Type[] _parameterTypes;

    public CustomMethodEmitter (CustomClassEmitter parentEmitter, string name, MethodAttributes attributes)
    {
      _parentEmitter = parentEmitter.InnerEmitter;
      _innerEmitter = _parentEmitter.CreateMethod (name, attributes);
      _name = name;
      _returnType = null;
      _parameterTypes = new Type[0];
      SetReturnType (typeof (void));
    }

    public MethodBuilder MethodBuilder
    {
      get { return _innerEmitter.MethodBuilder; }
    }

    internal MethodEmitter InnerEmitter
    {
      get { return _innerEmitter; }
    }

    public string Name
    {
      get { return _name; }
    }

    public ArgumentReference[] ArgumentReferences
    {
      get { return InnerEmitter.Arguments; }
    }

    public Type ReturnType
    {
      get { return _returnType; }
    }

    public Type[] ParameterTypes
    {
      get { return _parameterTypes; }
    }

    public CustomMethodEmitter SetParameterTypes (params Type[] parameters)
    {
      ArgumentUtility.CheckNotNull ("parameters", parameters);
      InnerEmitter.SetParameters (parameters);
      _parameterTypes = parameters;
      return this;
    }

    public CustomMethodEmitter SetReturnType (Type returnType)
    {
      ArgumentUtility.CheckNotNull ("returnType", returnType);
      InnerEmitter.SetReturnType (returnType);
      _returnType = returnType;
      return this;
    }

    public CustomMethodEmitter CopyParametersAndReturnType (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);

      _innerEmitter.CopyParametersAndReturnTypeFrom (method, _parentEmitter);
      return this;
    }

    public CustomMethodEmitter ImplementByReturning (Expression result)
    {
      ArgumentUtility.CheckNotNull ("result", result);
      return AddStatement (new ReturnStatement (result));
    }

    public CustomMethodEmitter ImplementByReturningVoid ()
    {
      return AddStatement (new ReturnStatement ());
    }

    public CustomMethodEmitter ImplementByReturningDefault ()
    {
      if (ReturnType == typeof (void))
        return ImplementByReturningVoid ();
      else
      {
        return ImplementByReturning (new InitObjectExpression (this, ReturnType));
      }
    }

    public CustomMethodEmitter ImplementByDelegating (Reference implementer, MethodInfo methodToCall)
    {
      AddDelegatingCallStatements (methodToCall, implementer, true);
      return this;
    }

    public CustomMethodEmitter ImplementByBaseCall (MethodInfo baseMethod)
    {
      ArgumentUtility.CheckNotNull ("baseMethod", baseMethod);

      if (baseMethod.IsAbstract)
        throw new ArgumentException (string.Format ("The given method {0}.{1} is abstract.", baseMethod.DeclaringType.FullName, baseMethod.Name),
            "baseMethod");
      
      AddDelegatingCallStatements (baseMethod, SelfReference.Self, false);
      return this;
    }

    private void AddDelegatingCallStatements (MethodInfo methodToCall, Reference owner, bool callVirtual)
    {
      Expression[] argumentExpressions = new Expression[ArgumentReferences.Length];
      for (int i = 0; i < argumentExpressions.Length; ++i)
        argumentExpressions[i] = ArgumentReferences[i].ToExpression ();

      MethodInvocationExpression delegatingCall;
      if (callVirtual)
        delegatingCall = new VirtualMethodInvocationExpression (owner, methodToCall, argumentExpressions);
      else
        delegatingCall = new MethodInvocationExpression (owner, methodToCall, argumentExpressions);

      AddStatement (new ReturnStatement (delegatingCall));
    }


    public CustomMethodEmitter ImplementByThrowing (Type exceptionType, string message)
    {
      ArgumentUtility.CheckNotNull ("exceptionType", exceptionType);
      ArgumentUtility.CheckNotNull ("message", message);
      AddStatement (new ThrowStatement (exceptionType, message));
      return this;
    }

    public CustomMethodEmitter AddStatement (Statement statement)
    {
      ArgumentUtility.CheckNotNull ("statement", statement);
      InnerEmitter.CodeBuilder.AddStatement (statement);
      return this;
    }

    public LocalReference DeclareLocal (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return InnerEmitter.CodeBuilder.DeclareLocal (type);
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      ArgumentUtility.CheckNotNull ("customAttribute", customAttribute);
      _innerEmitter.MethodBuilder.SetCustomAttribute (customAttribute);
    }
  }
}