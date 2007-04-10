using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Mixins.Definitions;
using Rubicon.Utilities;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Mixins.CodeGeneration.DynamicProxy
{
  public class TypeGenerator: ITypeGenerator
  {
    private ModuleManager _module;
    private BaseClassDefinition _configuration;
    private ExtendedClassEmitter _emitter;

    public TypeGenerator (ModuleManager module, BaseClassDefinition configuration)
    {
      ArgumentUtility.CheckNotNull ("module", module);
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      _module = module;
      _configuration = configuration;

      string typeName = string.Format ("{0}_Concrete_{1}", configuration.Type.FullName, Guid.NewGuid());
      _emitter = new ExtendedClassEmitter (_module.Scope, typeName, configuration.Type, /*new Type[] { typeof (ISerializable) }*/ Type.EmptyTypes,
        configuration.Type.IsSerializable);
    }

    public TypeBuilder GetTypeBuilder()
    {
      return _emitter.TypeBuilder;
    }

    /*public void ImplementISerializable ()
    {
      ImplementGetObjectData ();
      ImplementSerializationConstructor ();
    }

    private void ImplementGetObjectData ()
    {
      Assertion.DebugAssert(Array.IndexOf(GetTypeBuilder().GetInterfaces(), typeof(ISerializable)) != 0);
      bool baseIsISerializable = typeof(ISerializable).IsAssignableFrom(_emitter.BaseType);
      
      MethodInfo getObjectDataMethod = typeof(ISerializable).GetMethod("GetObjectData", new Type[] {typeof (SerializationInfo), typeof(StreamingContext)});
      MethodEmitter method = _emitter.CreateInterfaceImplementationMethod (getObjectDataMethod);
            method.CodeBuilder.AddStatement (new ExpressionStatement (new MethodInvocationExpression (        typeof (SerializationHelper).GetMethod ("GetObjectDataHelper"),         new ReferenceExpression(method.Arguments[0]), new ReferenceExpression(method.Arguments[1]),        new ReferenceExpression(SelfReference.Self), new ReferenceExpression(configurationField),        new ReferenceExpression(extensions), !baseIsISerializable)));

      if (baseIsISerializable)
      {
        MethodInfo baseGetObjectDataMethod = _emitter.BaseType.GetMethod("GetObjectData");
        if (baseGetObjectDataMethod == null)
        {
          string message = string.Format("{0}.GetObjectData is not public - this is not supported.", _emitter.BaseType.FullName);
          throw new NotSupportedException (message);
        }
        method.CodeBuilder.AddStatement (new ExpressionStatement (new MethodInvocationExpression (SelfReference.Self,
          baseGetObjectDataMethod, new ReferenceExpression (method.Arguments[0]), new ReferenceExpression (method.Arguments[1]))));
      }
      method.CodeBuilder.AddStatement (new ReturnStatement ());
    }

    private void ImplementSerializationConstructor ()
    {
      // throw new Exception ("The method or operation is not implemented.");
    }*/
  }
}