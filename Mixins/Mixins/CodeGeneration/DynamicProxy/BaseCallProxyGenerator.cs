using System;
using System.Collections.Generic;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Mixins.Definitions;
using System.Reflection;

namespace Mixins.CodeGeneration.DynamicProxy
{
  public class BaseCallProxyGenerator
  {
    private readonly TypeGenerator _surroundingType;
    private readonly NestedClassEmitter _nestedEmitter;
    private FieldReference _depthField;
    private FieldReference _thisField;

    public BaseCallProxyGenerator (TypeGenerator surroundingType)
    {
      _surroundingType = surroundingType;

      List<Type> interfaces = new List<Type> ();
      /*foreach (RequiredBaseCallTypeDefinition requiredType in _surroundingType.Configuration.RequiredBaseCallTypes)
        interfaces.Add (requiredType.Type);*/
      
      _nestedEmitter = new NestedClassEmitter (surroundingType.Emitter, "BaseCallProxy", typeof (object), interfaces.ToArray());

      _thisField = _nestedEmitter.CreateField ("_this", _surroundingType.TypeBuilder);
      _depthField = _nestedEmitter.CreateField ("_depth", typeof (int));

      GenerateConstructor();
    }

    private void GenerateConstructor ()
    {
      ArgumentReference arg1 = new ArgumentReference (_surroundingType.TypeBuilder);
      ArgumentReference arg2 = new ArgumentReference (typeof (int));
      ConstructorEmitter ctor = _nestedEmitter.CreateConstructor (arg1, arg2);
      ctor.CodeBuilder.InvokeBaseConstructor();
      ctor.CodeBuilder.AddStatement (new AssignStatement (_thisField, arg1.ToExpression ()));
      ctor.CodeBuilder.AddStatement (new AssignStatement (_depthField, arg2.ToExpression ()));
      ctor.CodeBuilder.AddStatement (new ReturnStatement ());
      ctor.Generate();
    }

    public void Finish()
    {
      _nestedEmitter.BuildType();
    }
  }
}