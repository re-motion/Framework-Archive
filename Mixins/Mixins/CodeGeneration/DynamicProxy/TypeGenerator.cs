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

    private FieldReference _configurationField;
    private FieldReference _extensionsField;

    public TypeGenerator (ModuleManager module, BaseClassDefinition configuration)
    {
      ArgumentUtility.CheckNotNull ("module", module);
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      _module = module;
      _configuration = configuration;

      bool isSerializable = configuration.Type.IsSerializable;

      string typeName = string.Format ("{0}_Concrete_{1}", configuration.Type.FullName, Guid.NewGuid());
      List<Type> interfaces = new List<Type> ();
      interfaces.Add (typeof (IMixinTarget));
      if (isSerializable)
      {
        interfaces.Add (typeof (ISerializable));
      }
      _emitter = new ExtendedClassEmitter (_module.Scope, typeName, configuration.Type, interfaces.ToArray (), isSerializable);

      AddConfigurationField ();
      AddExtensionsField ();
      ReplicateConstructors ();

      if (isSerializable)
      {
        ImplementGetObjectData();
      }

      ImplementIMixinTarget ();
    }


    private TypeBuilder TypeBuilder
    {
      get { return _emitter.TypeBuilder; }
    }

    public TypeBuilder GetBuiltType()
    {
      return TypeBuilder;
    }

    public void InitializeStaticFields (Type finishedType)
    {
      finishedType.GetField (_configurationField.Reference.Name).SetValue (null, _configuration);
    }

    private void AddConfigurationField ()
    {
      _configurationField = _emitter.CreateStaticField("__configuration", typeof (BaseClassDefinition));
    }

    private void AddExtensionsField ()
    {
      _extensionsField = _emitter.CreateField ("__extensions", typeof (object[]), true);
    }

    private void ImplementGetObjectData ()
    {
      Assertion.DebugAssert (Array.IndexOf(TypeBuilder.GetInterfaces(), typeof(ISerializable)) != 0);
      bool baseIsISerializable = typeof(ISerializable).IsAssignableFrom(_emitter.BaseType);
      
      MethodInfo getObjectDataMethod = typeof(ISerializable).GetMethod("GetObjectData", new Type[] {typeof (SerializationInfo), typeof(StreamingContext)});
      MethodEmitter newMethod = _emitter.CreateInterfaceImplementationMethod (getObjectDataMethod);

      newMethod.CodeBuilder.AddStatement (new ExpressionStatement (new MethodInvocationExpression (null, 
        typeof (SerializationHelper).GetMethod ("GetObjectDataForGeneratedTypes"), 
        new ReferenceExpression(newMethod.Arguments[0]), new ReferenceExpression(newMethod.Arguments[1]),
        new ReferenceExpression(SelfReference.Self), new ReferenceExpression(_configurationField),
        new ReferenceExpression(_extensionsField), new ReferenceExpression(new ConstReference(!baseIsISerializable)))));

      if (baseIsISerializable)
      {
        ConstructorInfo baseConstructor = _emitter.BaseType.GetConstructor (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, CallingConventions.Any, new Type[] { typeof (SerializationInfo), typeof (StreamingContext) }, null);
        if (baseConstructor == null || (!baseConstructor.IsPublic && !baseConstructor.IsFamily))
        {
          string message = string.Format ("No public or protected deserialization constructor in type {0} - this is not supported.",
              _emitter.BaseType.FullName);
          throw new NotSupportedException (message);
        }

        MethodInfo baseGetObjectDataMethod = _emitter.BaseType.GetMethod("GetObjectData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (baseGetObjectDataMethod == null || (!baseGetObjectDataMethod.IsPublic && !baseGetObjectDataMethod.IsFamily))
        {
          string message = string.Format ("No public or protected GetObjectData in {0} - this is not supported.", _emitter.BaseType.FullName);
          throw new NotSupportedException (message);
        }
        newMethod.CodeBuilder.AddStatement (new ExpressionStatement (new MethodInvocationExpression (SelfReference.Self,
          baseGetObjectDataMethod, new ReferenceExpression (newMethod.Arguments[0]), new ReferenceExpression (newMethod.Arguments[1]))));
      }
      newMethod.CodeBuilder.AddStatement (new ReturnStatement ());
      newMethod.Generate ();
    }

    private void ReplicateConstructors ()
    {
      ConstructorInfo[] constructors = _emitter.BaseType.GetConstructors (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      foreach (ConstructorInfo constructor in constructors)
      {
        if (constructor.IsPublic | constructor.IsFamily)
        {
          ReplicateConstructor (constructor);
        }
      }
    }

    private void ReplicateConstructor (ConstructorInfo constructor)
    {
      ArgumentReference[] arguments = ArgumentsUtil.ConvertToArgumentReference (constructor.GetParameters ());
      ConstructorEmitter newConstructor = _emitter.CreateConstructor (arguments);

      Expression[] argumentExpressions = ArgumentsUtil.ConvertArgumentReferenceToExpression (arguments);
      newConstructor.CodeBuilder.AddStatement (new ConstructorInvocationStatement (constructor, argumentExpressions));

      newConstructor.CodeBuilder.AddStatement (new ReturnStatement ());
      newConstructor.Generate ();
    }

    private void ImplementIMixinTarget ()
    {
      Assertion.DebugAssert (Array.IndexOf (TypeBuilder.GetInterfaces (), typeof (IMixinTarget)) != 0);

      PropertyEmitter configurationProperty = _emitter.CreateInterfaceImplementationProperty (typeof (IMixinTarget).GetProperty ("Configuration"));
      _emitter.ImplementPropertyWithField (configurationProperty, _configurationField);
      configurationProperty.Generate ();

      PropertyEmitter mixinsProperty = _emitter.CreateInterfaceImplementationProperty (typeof (IMixinTarget).GetProperty ("Mixins"));
      _emitter.ImplementPropertyWithField (mixinsProperty, _extensionsField);
      mixinsProperty.Generate ();
    }
  }
}