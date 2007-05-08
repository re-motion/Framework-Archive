using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Mixins.Definitions;
using Rubicon.Utilities;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Mixins.CodeGeneration.DynamicProxy.DPExtensions;

using LoadArrayElementExpression = Mixins.CodeGeneration.DynamicProxy.DPExtensions.LoadArrayElementExpression;

namespace Mixins.CodeGeneration.DynamicProxy
{
  public class TypeGenerator: ITypeGenerator
  {
    private ModuleManager _module;
    private BaseClassDefinition _configuration;
    private ExtendedClassEmitter _emitter;
    private BaseCallProxyGenerator _baseCallGenerator;

    private FieldReference _configurationField;
    private FieldReference _extensionsField;
    private FieldReference _firstField;

    public TypeGenerator (ModuleManager module, BaseClassDefinition configuration)
    {
      ArgumentUtility.CheckNotNull ("module", module);
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      _module = module;
      _configuration = configuration;

      bool isSerializable = configuration.Type.IsSerializable;

      string typeName = string.Format ("{0}_Concrete_{1}", configuration.Type.FullName, Guid.NewGuid());

      List<Type> interfaces = GetInterfacesToImplement(isSerializable);
      ClassEmitter classEmitter = new ClassEmitter (_module.Scope, typeName, configuration.Type, interfaces.ToArray(), isSerializable);
      _emitter = new ExtendedClassEmitter (classEmitter);
      _baseCallGenerator = new BaseCallProxyGenerator (this, classEmitter);

      _configurationField = _emitter.InnerEmitter.CreateStaticField ("__configuration", typeof (BaseClassDefinition));
      _extensionsField = _emitter.InnerEmitter.CreateField ("__extensions", typeof (object[]), true);
      _firstField = _emitter.InnerEmitter.CreateField ("__first", _baseCallGenerator.TypeBuilder, true);

      _emitter.ReplicateBaseTypeConstructors();

      if (isSerializable)
      {
        ImplementGetObjectData();
      }

      ImplementIMixinTarget ();
      ImplementIntroducedInterfaces();

      ReplicateClassAttributes();
    }

    private List<Type> GetInterfacesToImplement(bool isSerializable)
    {
      List<Type> interfaces = new List<Type> ();
      interfaces.Add (typeof (IMixinTarget));
      
      foreach (InterfaceIntroductionDefinition introduction in _configuration.IntroducedInterfaces)
        interfaces.Add (introduction.Type);

      Set<Type> alreadyImplementedInterfaces = new Set<Type> (interfaces);
      alreadyImplementedInterfaces.AddRange (_configuration.ImplementedInterfaces);

      foreach (RequiredFaceTypeDefinition requiredFaceType in _configuration.RequiredFaceTypes)
      {
        if (requiredFaceType.Type.IsInterface && !alreadyImplementedInterfaces.Contains (requiredFaceType.Type))
          interfaces.Add (requiredFaceType.Type);
      }

      if (isSerializable)
        interfaces.Add (typeof (ISerializable));

      return interfaces;
    }

    public TypeBuilder TypeBuilder
    {
      get { return _emitter.TypeBuilder; }
    }

    public ExtendedClassEmitter Emitter
    {
      get { return _emitter; }
    }

    public BaseClassDefinition Configuration
    {
      get { return _configuration; }
    }

    public TypeBuilder GetBuiltType()
    {
      _baseCallGenerator.Finish();
      return TypeBuilder;
    }

    public void InitializeStaticFields (Type finishedType)
    {
      finishedType.GetField (_configurationField.Reference.Name).SetValue (null, _configuration);
    }

    private void ImplementGetObjectData()
    {
      Assertion.DebugAssert (Array.IndexOf (TypeBuilder.GetInterfaces(), typeof (ISerializable)) != 0);
      bool baseIsISerializable = typeof (ISerializable).IsAssignableFrom (_emitter.BaseType);

      MethodInfo getObjectDataMethod =
          typeof (ISerializable).GetMethod ("GetObjectData", new Type[] {typeof (SerializationInfo), typeof (StreamingContext)});
      MethodEmitter newMethod = _emitter.CreateInterfaceImplementationMethod (getObjectDataMethod).InnerEmitter;

      newMethod.CodeBuilder.AddStatement (
          new ExpressionStatement (
              new MethodInvocationExpression (
                  null,
                  typeof (SerializationHelper).GetMethod ("GetObjectDataForGeneratedTypes"),
                  new ReferenceExpression (newMethod.Arguments[0]),
                  new ReferenceExpression (newMethod.Arguments[1]),
                  new ReferenceExpression (SelfReference.Self),
                  new ReferenceExpression (_configurationField),
                  new ReferenceExpression (_extensionsField),
                  new ReferenceExpression (_firstField),
                  new ReferenceExpression (new ConstReference (!baseIsISerializable)))));

      if (baseIsISerializable)
      {
        ConstructorInfo baseConstructor = _emitter.BaseType.GetConstructor (
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            CallingConventions.Any,
            new Type[] {typeof (SerializationInfo), typeof (StreamingContext)},
            null);
        if (baseConstructor == null || (!baseConstructor.IsPublic && !baseConstructor.IsFamily))
        {
          string message = string.Format (
              "No public or protected deserialization constructor in type {0} - this is not supported.",
              _emitter.BaseType.FullName);
          throw new NotSupportedException (message);
        }

        MethodInfo baseGetObjectDataMethod =
            _emitter.BaseType.GetMethod ("GetObjectData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (baseGetObjectDataMethod == null || (!baseGetObjectDataMethod.IsPublic && !baseGetObjectDataMethod.IsFamily))
        {
          string message = string.Format ("No public or protected GetObjectData in {0} - this is not supported.", _emitter.BaseType.FullName);
          throw new NotSupportedException (message);
        }
        newMethod.CodeBuilder.AddStatement (
            new ExpressionStatement (
                new MethodInvocationExpression (
                    SelfReference.Self,
                    baseGetObjectDataMethod,
                    new ReferenceExpression (newMethod.Arguments[0]),
                    new ReferenceExpression (newMethod.Arguments[1]))));
      }
      newMethod.CodeBuilder.AddStatement (new ReturnStatement());
      newMethod.Generate();
    }

    private void ImplementIMixinTarget()
    {
      Assertion.DebugAssert (Array.IndexOf (TypeBuilder.GetInterfaces(), typeof (IMixinTarget)) != 0);

      CustomPropertyEmitter configurationProperty =
          _emitter.CreateInterfaceImplementationProperty (typeof (IMixinTarget).GetProperty ("Configuration"));
      configurationProperty.GetMethod =
          _emitter.CreateInterfaceImplementationMethod (typeof (IMixinTarget).GetMethod ("get_Configuration")).InnerEmitter;
      _emitter.ImplementPropertyWithField (configurationProperty, _configurationField);
      configurationProperty.Generate();

      CustomPropertyEmitter mixinsProperty =
          _emitter.CreateInterfaceImplementationProperty (typeof (IMixinTarget).GetProperty ("Mixins"));
      mixinsProperty.GetMethod =
          _emitter.CreateInterfaceImplementationMethod (typeof (IMixinTarget).GetMethod ("get_Mixins")).InnerEmitter;
      _emitter.ImplementPropertyWithField (mixinsProperty, _extensionsField);
      mixinsProperty.Generate();

      CustomPropertyEmitter firstProperty =
          _emitter.CreateInterfaceImplementationProperty (typeof (IMixinTarget).GetProperty ("FirstBaseCallProxy"));
      firstProperty.GetMethod =
          _emitter.CreateInterfaceImplementationMethod (typeof (IMixinTarget).GetMethod ("get_FirstBaseCallProxy")).InnerEmitter;
      _emitter.ImplementPropertyWithField (firstProperty, _firstField);
      firstProperty.Generate ();
    }

    private void ImplementIntroducedInterfaces()
    {
      foreach (InterfaceIntroductionDefinition introduction in _configuration.IntroducedInterfaces)
      {
        Expression implementerExpression = new CastClassExpression (
            introduction.Type,
            new LoadArrayElementExpression (introduction.Implementer.MixinIndex, _extensionsField, typeof (object)));

        foreach (MethodIntroductionDefinition method in introduction.IntroducedMethods)
          ImplementIntroducedMethod (implementerExpression, method.ImplementingMember, method.InterfaceMember).InnerEmitter.Generate();

        foreach (PropertyIntroductionDefinition property in introduction.IntroducedProperties)
          ImplementIntroducedProperty (implementerExpression, property).Generate();

        foreach (EventIntroductionDefinition eventIntro in introduction.IntroducedEvents)
          ImplementIntroducedEvent (eventIntro, implementerExpression).Generate();
      }
    }

    private CustomMethodEmitter ImplementIntroducedMethod (
        Expression implementerExpression,
        MethodDefinition implementingMember,
        MethodInfo interfaceMember)
    {
      CustomMethodEmitter customMethodEmitter = _emitter.CreateInterfaceImplementationMethod (interfaceMember);
      MethodEmitter methodEmitter = customMethodEmitter.InnerEmitter;

      LocalReference localImplementer = _emitter.EmitMakeReferenceOfExpression (methodEmitter, interfaceMember.DeclaringType, implementerExpression);
      _emitter.ImplementMethodByDelegation (methodEmitter, localImplementer, interfaceMember);

      ReplicateAttributes (implementingMember.CustomAttributes, customMethodEmitter);
      return customMethodEmitter;
    }

    private CustomPropertyEmitter ImplementIntroducedProperty (Expression implementerExpression, PropertyIntroductionDefinition property)
    {
      CustomPropertyEmitter propertyEmitter = _emitter.CreateInterfaceImplementationProperty (property.InterfaceMember);

      if (property.ImplementingMember.GetMethod != null)
        propertyEmitter.GetMethod = ImplementIntroducedMethod (
            implementerExpression,
            property.ImplementingMember.GetMethod,
            property.InterfaceMember.GetGetMethod()).InnerEmitter;

      if (property.ImplementingMember.SetMethod != null)
        propertyEmitter.SetMethod = ImplementIntroducedMethod (
            implementerExpression,
            property.ImplementingMember.SetMethod,
            property.InterfaceMember.GetSetMethod()).InnerEmitter;

      ReplicateAttributes (property.ImplementingMember.CustomAttributes, propertyEmitter);
      return propertyEmitter;
    }

    private CustomEventEmitter ImplementIntroducedEvent (EventIntroductionDefinition eventIntro, Expression implementerExpression)
    {
      Assertion.Assert (eventIntro.ImplementingMember.AddMethod != null);
      Assertion.Assert (eventIntro.ImplementingMember.RemoveMethod != null);

      CustomEventEmitter eventEmitter = _emitter.CreateInterfaceImplementationEvent (eventIntro.InterfaceMember);
      eventEmitter.AddMethod = ImplementIntroducedMethod (
          implementerExpression,
          eventIntro.ImplementingMember.AddMethod,
          eventIntro.InterfaceMember.GetAddMethod()).InnerEmitter;
      eventEmitter.RemoveMethod = ImplementIntroducedMethod (
          implementerExpression,
          eventIntro.ImplementingMember.RemoveMethod,
          eventIntro.InterfaceMember.GetRemoveMethod()).InnerEmitter;

      ReplicateAttributes (eventIntro.ImplementingMember.CustomAttributes, eventEmitter);
      return eventEmitter;
    }

    private void ReplicateAttributes (IEnumerable<AttributeDefinition> attributes, IAttributableEmitter target)
    {
      foreach (AttributeDefinition attribute in attributes)
        AttributeReplicator.ReplicateAttribute(target, attribute.Data);
    }

    private void ReplicateClassAttributes()
    {
      ReplicateAttributes (_configuration.CustomAttributes, _emitter);
      foreach (MixinDefinition mixin in _configuration.Mixins)
        ReplicateAttributes (mixin.CustomAttributes, _emitter);
    }
  }
}