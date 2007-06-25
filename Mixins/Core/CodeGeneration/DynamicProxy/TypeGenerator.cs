using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Utilities;
using Rubicon.Collections;
using Rubicon.Utilities;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Rubicon.Mixins.CodeGeneration.DynamicProxy.DPExtensions;

using LoadArrayElementExpression = Rubicon.Mixins.CodeGeneration.DynamicProxy.DPExtensions.LoadArrayElementExpression;
using Rubicon.Text;

namespace Rubicon.Mixins.CodeGeneration.DynamicProxy
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
    private Dictionary<MethodInfo, MethodInfo> _baseCallMethods = new Dictionary<MethodInfo, MethodInfo>();

    public TypeGenerator (ModuleManager module, BaseClassDefinition configuration)
    {
      ArgumentUtility.CheckNotNull ("module", module);
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      _module = module;
      _configuration = configuration;

      bool isSerializable = configuration.Type.IsSerializable || typeof (ISerializable).IsAssignableFrom (configuration.Type);

      string typeName = string.Format ("{0}_Concrete_{1}", configuration.Type.FullName, Guid.NewGuid());

      List<Type> interfaces = GetInterfacesToImplement(isSerializable);
      ClassEmitter classEmitter = new ClassEmitter (_module.Scope, typeName, configuration.Type, interfaces.ToArray(), isSerializable);
      _emitter = new ExtendedClassEmitter (classEmitter);

      _configurationField = _emitter.InnerEmitter.CreateStaticField ("__configuration", typeof (BaseClassDefinition));
      _extensionsField = _emitter.InnerEmitter.CreateField ("__extensions", typeof (object[]), true);

      _baseCallGenerator = new BaseCallProxyGenerator (this, classEmitter);

      _firstField = _emitter.InnerEmitter.CreateField ("__first", _baseCallGenerator.TypeBuilder, true);

      _emitter.ReplicateBaseTypeConstructors();

      if (isSerializable)
        ImplementGetObjectData();

      ImplementIMixinTarget();
      ImplementIntroducedInterfaces();
      ImplementOverrides();

      ReplicateClassAttributes();

      AddDebuggerAttributes();
    }

    private List<Type> GetInterfacesToImplement (bool isSerializable)
    {
      List<Type> interfaces = new List<Type>();
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
      get { return Emitter.TypeBuilder; }
    }

    public ExtendedClassEmitter Emitter
    {
      get { return _emitter; }
    }

    public BaseClassDefinition Configuration
    {
      get { return _configuration; }
    }

    public Type GetBuiltType ()
    {
      Type builtType = Emitter.InnerEmitter.BuildType();
      InitializeStaticFields (builtType);
      return builtType;
    }

    internal FieldInfo ExtensionsField
    {
      get { return _extensionsField.Reference; }
    }

    private void InitializeStaticFields (Type finishedType)
    {
      finishedType.GetField (_configurationField.Reference.Name).SetValue (null, _configuration);
    }

    private void ImplementGetObjectData ()
    {
      Emitter.ImplementGetObjectDataByDelegation (
          delegate (MethodEmitter newMethod, bool baseIsISerializable)
          {
            return new MethodInvocationExpression (
                null,
                typeof (SerializationHelper).GetMethod ("GetObjectDataForGeneratedTypes"),
                new ReferenceExpression (newMethod.Arguments[0]),
                new ReferenceExpression (newMethod.Arguments[1]),
                new ReferenceExpression (SelfReference.Self),
                new ReferenceExpression (_configurationField),
                new ReferenceExpression (_extensionsField),
                new ReferenceExpression (new ConstReference (!baseIsISerializable)));
          });
    }

    private void ImplementIMixinTarget ()
    {
      Assertion.DebugAssert (Array.IndexOf (TypeBuilder.GetInterfaces(), typeof (IMixinTarget)) != 0);

      CustomPropertyEmitter configurationProperty =
          Emitter.CreatePropertyOverrideOrInterfaceImplementation (typeof (IMixinTarget).GetProperty ("Configuration"));
      configurationProperty.GetMethod =
          Emitter.CreateMethodOverrideOrInterfaceImplementation (typeof (IMixinTarget).GetMethod ("get_Configuration")).InnerEmitter;
      configurationProperty.ImplementPropertyWithField (_configurationField);

      CustomPropertyEmitter mixinsProperty =
          Emitter.CreatePropertyOverrideOrInterfaceImplementation (typeof (IMixinTarget).GetProperty ("Mixins"));
      mixinsProperty.GetMethod =
          Emitter.CreateMethodOverrideOrInterfaceImplementation (typeof (IMixinTarget).GetMethod ("get_Mixins")).InnerEmitter;
      mixinsProperty.ImplementPropertyWithField (_extensionsField);

      CustomPropertyEmitter firstProperty =
          Emitter.CreatePropertyOverrideOrInterfaceImplementation (typeof (IMixinTarget).GetProperty ("FirstBaseCallProxy"));
      firstProperty.GetMethod =
          Emitter.CreateMethodOverrideOrInterfaceImplementation (typeof (IMixinTarget).GetMethod ("get_FirstBaseCallProxy")).InnerEmitter;
      firstProperty.ImplementPropertyWithField (_firstField);
    }

    private void ImplementIntroducedInterfaces ()
    {
      foreach (InterfaceIntroductionDefinition introduction in _configuration.IntroducedInterfaces)
      {
        Expression implementerExpression = new CastClassExpression (
            introduction.Type,
            new LoadArrayElementExpression (introduction.Implementer.MixinIndex, _extensionsField, typeof (object)));

        foreach (MethodIntroductionDefinition method in introduction.IntroducedMethods)
          ImplementIntroducedMethod (implementerExpression, method.ImplementingMember, method.InterfaceMember);

        foreach (PropertyIntroductionDefinition property in introduction.IntroducedProperties)
          ImplementIntroducedProperty (implementerExpression, property);

        foreach (EventIntroductionDefinition eventIntro in introduction.IntroducedEvents)
          ImplementIntroducedEvent (eventIntro, implementerExpression);
      }
    }

    private CustomMethodEmitter ImplementIntroducedMethod (
        Expression implementerExpression,
        MethodDefinition implementingMember,
        MethodInfo interfaceMember)
    {
      CustomMethodEmitter customMethodEmitter = Emitter.CreateMethodOverrideOrInterfaceImplementation (interfaceMember);

      LocalReference localImplementer = Emitter.AddMakeReferenceOfExpressionStatements (
          customMethodEmitter,
          interfaceMember.DeclaringType,
          implementerExpression);
      customMethodEmitter.ImplementMethodByDelegation (localImplementer, interfaceMember);

      ReplicateAttributes (implementingMember.CustomAttributes, customMethodEmitter);
      return customMethodEmitter;
    }

    private CustomPropertyEmitter ImplementIntroducedProperty (Expression implementerExpression, PropertyIntroductionDefinition property)
    {
      CustomPropertyEmitter propertyEmitter = Emitter.CreatePropertyOverrideOrInterfaceImplementation (property.InterfaceMember);

      if (property.IntroducesGetMethod)
        propertyEmitter.GetMethod = ImplementIntroducedMethod (
            implementerExpression,
            property.ImplementingMember.GetMethod,
            property.InterfaceMember.GetGetMethod()).InnerEmitter;

      if (property.IntroducesSetMethod)
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

      CustomEventEmitter eventEmitter = Emitter.CreateEventOverrideOrInterfaceImplementation (eventIntro.InterfaceMember);
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

    private void ImplementOverrides ()
    {
      foreach (MethodDefinition method in _configuration.Methods)
      {
        if (method.Overrides.Count > 0)
          ImplementOverride (method);
      }
    }

    private void ImplementOverride (MethodDefinition method)
    {
      MethodInfo proxyMethod = _baseCallGenerator.GetProxyMethodForOverriddenMethod (method);
      CustomMethodEmitter methodOverride = Emitter.CreateMethodOverrideOrInterfaceImplementation (method.MethodInfo);
      methodOverride.ImplementMethodByDelegation (_firstField, proxyMethod);
    }

    private void ReplicateAttributes (IEnumerable<AttributeDefinition> attributes, IAttributableEmitter target)
    {
      foreach (AttributeDefinition attribute in attributes)
        AttributeReplicator.ReplicateAttribute (target, attribute.Data);
    }

    private void ReplicateClassAttributes ()
    {
      ReplicateAttributes (_configuration.CustomAttributes, Emitter);
      foreach (MixinDefinition mixin in _configuration.Mixins)
        ReplicateAttributes (mixin.CustomAttributes, Emitter);
    }

    private void AddDebuggerAttributes ()
    {
      string debuggerString = "Mix of " + _configuration.Type.FullName + " + "
                              + SeparatedStringBuilder.Build (" + ", _configuration.Mixins, delegate (MixinDefinition m) { return m.FullName; });
      CustomAttributeBuilder debuggerAttribute =
          new CustomAttributeBuilder (
              typeof (DebuggerDisplayAttribute).GetConstructor (new Type[] {typeof (string)}),
              new object[] {debuggerString});
      Emitter.AddCustomAttribute (debuggerAttribute);
    }

    public MethodInfo GetBaseCallMethodFor (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      if (!method.DeclaringType.IsAssignableFrom (TypeBuilder.BaseType))
      {
        string message = string.Format (
            "Cannot create base call method for a method defined on a different type than the base type: {0}.{1}.",
            method.DeclaringType.FullName,
            method.Name);
        throw new ArgumentException (message, "method");
      }
      if (!_baseCallMethods.ContainsKey (method))
        _baseCallMethods.Add (method, ImplementBaseCallMethod (method));
      return _baseCallMethods[method];
    }

    private MethodInfo ImplementBaseCallMethod (MethodInfo method)
    {
      MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig;
      CustomMethodEmitter baseCallMethod = new CustomMethodEmitter (Emitter.InnerEmitter, "__base__" + method.Name, attributes);
      baseCallMethod.CopyParametersAndReturnTypeFrom (method);
      baseCallMethod.ImplementMethodByBaseCall (method);
      return baseCallMethod.MethodBuilder;
    }
  }
}