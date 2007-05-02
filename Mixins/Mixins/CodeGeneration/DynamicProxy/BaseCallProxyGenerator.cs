using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Mixins.CodeGeneration.DynamicProxy.DPExtensions;
using Mixins.Definitions;

namespace Mixins.CodeGeneration.DynamicProxy
{
  public class BaseCallProxyGenerator
  {
    private readonly TypeGenerator _surroundingType;
    private readonly ExtendedClassEmitter _nestedEmitter;
    private readonly BaseClassDefinition _baseClassConfiguration;
    private FieldReference _depthField;
    private FieldReference _thisField;

    public BaseCallProxyGenerator (TypeGenerator surroundingType, ClassEmitter surroundingTypeEmitter)
    {
      _surroundingType = surroundingType;
      _baseClassConfiguration = surroundingType.Configuration;

      List<Type> interfaces = new List<Type> ();
      foreach (RequiredBaseCallTypeDefinition requiredType in _baseClassConfiguration.RequiredBaseCallTypes)
      {
        if (_baseClassConfiguration.ImplementedInterfaces.Contains(requiredType.Type)) // TODO: remove check
          interfaces.Add (requiredType.Type);
      }

      _nestedEmitter = new ExtendedClassEmitter (new NestedClassEmitter (surroundingTypeEmitter, "BaseCallProxy", typeof (object),
          interfaces.ToArray ()));
      _nestedEmitter.AddCustomAttribute (new CustomAttributeBuilder (typeof (SerializableAttribute).GetConstructor(Type.EmptyTypes), new object[0]));

      _thisField = _nestedEmitter.InnerEmitter.CreateField ("_this", _surroundingType.TypeBuilder);
      _depthField = _nestedEmitter.InnerEmitter.CreateField ("_depth", typeof (int));

      GenerateConstructor();
      ImplementRequiredBaseCallTypes();
    }

    public Type TypeBuilder
    {
      get { return _nestedEmitter.TypeBuilder; }
    }

    private void GenerateConstructor ()
    {
      ArgumentReference arg1 = new ArgumentReference (_surroundingType.TypeBuilder);
      ArgumentReference arg2 = new ArgumentReference (typeof (int));
      ConstructorEmitter ctor = _nestedEmitter.InnerEmitter.CreateConstructor (arg1, arg2);
      ctor.CodeBuilder.InvokeBaseConstructor();
      ctor.CodeBuilder.AddStatement (new AssignStatement (_thisField, arg1.ToExpression ()));
      ctor.CodeBuilder.AddStatement (new AssignStatement (_depthField, arg2.ToExpression ()));
      ctor.CodeBuilder.AddStatement (new ReturnStatement ());
      ctor.Generate();
    }

    private void ImplementRequiredBaseCallTypes ()
    {
      foreach (RequiredBaseCallTypeDefinition requiredType in _baseClassConfiguration.RequiredBaseCallTypes)
        foreach (RequiredBaseCallMethodDefinition requiredMethod in requiredType.BaseCallMethods)
          ImplementRequiredBaseCallMethod (requiredMethod);
    }

    private void ImplementRequiredBaseCallMethod (RequiredBaseCallMethodDefinition requiredMethod)
    {
      if (requiredMethod.ImplementingMethod.DeclaringClass == _baseClassConfiguration)
      {
        ImplementRequiredBaseCallMethodOnThis (requiredMethod);
      }
      else
      {
        // TODO
        // ImplementRequiredBaseCallMethodOnExtension (requiredMethod);
      }
    }

    // TODO: check for overrides
    private void ImplementRequiredBaseCallMethodOnThis (RequiredBaseCallMethodDefinition requiredMethod)
    {
      CustomMethodEmitter methodImplementation = _nestedEmitter.CreateInterfaceImplementationMethod (requiredMethod.InterfaceMethod);
      _nestedEmitter.ImplementMethodByDelegation (methodImplementation.InnerEmitter, _thisField, requiredMethod.ImplementingMethod.MethodInfo);
      methodImplementation.InnerEmitter.Generate();
    }

    // TODO: implement by conditional delegation
    private void ImplementRequiredBaseCallMethodOnExtension (RequiredBaseCallMethodDefinition requiredMethod)
    {
      throw new Exception ("The method or operation is not implemented.");
    }

    public void Finish()
    {
      _nestedEmitter.InnerEmitter.BuildType ();
    }
  }
}