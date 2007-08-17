using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Rubicon.CodeGeneration;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Utilities;
using Rubicon.Collections;
using Rubicon.Utilities;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Rubicon.CodeGeneration.DPExtensions;

using Rubicon.Text;
using ReflectionUtility=Rubicon.Mixins.Utilities.ReflectionUtility;

namespace Rubicon.Mixins.CodeGeneration.DynamicProxy
{
  internal class TypeGenerator : ITypeGenerator
  {
    private static readonly MethodInfo s_concreteTypeInitializationMethod =
        typeof (GeneratedClassInstanceInitializer).GetMethod ("InitializeMixinTarget", new Type[] { typeof (IMixinTarget) });

    private ModuleManager _module;
    private TargetClassDefinition _configuration;
    private CustomClassEmitter _emitter;
    private BaseCallProxyGenerator _baseCallGenerator;

    private FieldReference _configurationField;
    private FieldReference _extensionsField;
    private FieldReference _firstField;
    private Dictionary<MethodInfo, MethodInfo> _baseCallMethods = new Dictionary<MethodInfo, MethodInfo>();
    private MixinTypeGenerator[] _mixinTypeGenerators;

    public TypeGenerator (ModuleManager module, TargetClassDefinition configuration, INameProvider nameProvider, INameProvider mixinNameProvider)
    {
      ArgumentUtility.CheckNotNull ("module", module);
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      _module = module;
      _configuration = configuration;

      bool isSerializable = configuration.Type.IsSerializable || typeof (ISerializable).IsAssignableFrom (configuration.Type);

      string typeName = nameProvider.GetNewTypeName (configuration);

      List<Type> interfaces = GetInterfacesToImplement(isSerializable);

      TypeAttributes flags = TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable;
      if (_configuration.IsAbstract)
        flags |= TypeAttributes.Abstract;

      ClassEmitter classEmitter = new ClassEmitter (_module.Scope, typeName, configuration.Type, interfaces.ToArray(), flags);
      _emitter = new CustomClassEmitter (classEmitter);

      _configurationField = _emitter.CreateStaticField ("__configuration", typeof (TargetClassDefinition));
      _extensionsField = _emitter.CreateField ("__extensions", typeof (object[]));

      _mixinTypeGenerators = CreateMixinTypeGenerators (mixinNameProvider);
      _baseCallGenerator = new BaseCallProxyGenerator (this, classEmitter, _mixinTypeGenerators);

      _firstField = _emitter.CreateField ("__first", _baseCallGenerator.TypeBuilder);

      Statement initializationStatement = new ExpressionStatement (new MethodInvocationExpression (null, s_concreteTypeInitializationMethod,
          new CastClassExpression (typeof (IMixinTarget), SelfReference.Self.ToExpression ())));

      _emitter.ReplicateBaseTypeConstructors (initializationStatement);

      AddTypeInitializer ();

      if (isSerializable)
        ImplementGetObjectData();

      ImplementIMixinTarget();
      ImplementIntroducedInterfaces ();
      ImplementRequiredDuckMethods ();
      ImplementAttributes (configuration, _emitter);

      AddMixedTypeAttribute ();
      AddDebuggerAttributes();

      ImplementOverrides ();
    }

    private MixinTypeGenerator[] CreateMixinTypeGenerators (INameProvider mixinNameProvider)
    {
      MixinTypeGenerator[] mixinTypeGenerators = new MixinTypeGenerator[Configuration.Mixins.Count];
      for (int i = 0; i < mixinTypeGenerators.Length; ++i)
      {
        MixinDefinition mixinConfiguration = Configuration.Mixins[i];
        if (NeedsDerivedMixinType (mixinConfiguration))
          mixinTypeGenerators[i] = new MixinTypeGenerator (_module, this, mixinConfiguration, mixinNameProvider);
      }
      return mixinTypeGenerators;
    }

    public static bool NeedsDerivedMixinType (MixinDefinition configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      return configuration.HasOverriddenMembers () || configuration.HasProtectedOverriders ();
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

    public CustomClassEmitter Emitter
    {
      get { return _emitter; }
    }

    public TargetClassDefinition Configuration
    {
      get { return _configuration; }
    }

    public Type GetBuiltType ()
    {
      Type builtType = Emitter.BuildType();
      return builtType;
    }

    public IEnumerable<Tuple<MixinDefinition, Type>> GetBuiltMixinTypes ()
    {
      foreach (MixinTypeGenerator mixinTypeGenerator in _mixinTypeGenerators)
      {
        if (mixinTypeGenerator != null)
          yield return new Tuple<MixinDefinition, Type> (mixinTypeGenerator.Configuration, mixinTypeGenerator.GetBuiltType());
      }
    }

    internal FieldInfo ExtensionsField
    {
      get { return _extensionsField.Reference; }
    }

    private void AddTypeInitializer ()
    {
      ConstructorEmitter emitter = _emitter.CreateTypeConstructor ();

      LocalReference firstAttributeLocal = GetFirstAttributeLocal (emitter, typeof (ConcreteMixedTypeAttribute));

      MethodInfo getTargetClassDefinitionMethod = typeof (ConcreteMixedTypeAttribute).GetMethod ("GetTargetClassDefinition");
      Assertion.IsNotNull (getTargetClassDefinitionMethod);
      emitter.CodeBuilder.AddStatement (new AssignStatement (_configurationField,
          new VirtualMethodInvocationExpression (firstAttributeLocal, getTargetClassDefinitionMethod)));

      emitter.CodeBuilder.AddStatement (new ReturnStatement ());
    }

    internal static LocalReference GetFirstAttributeLocal (ConstructorEmitter emitter, Type attributeType)
    {
      LocalReference thisTypeLocal = emitter.CodeBuilder.DeclareLocal (typeof (Type));
      emitter.CodeBuilder.AddStatement (new AssignStatement (thisTypeLocal, new TypeTokenExpression (emitter.ConstructorBuilder.DeclaringType)));

      Expression firstAttributeExpression = new CustomAttributeExpression (thisTypeLocal, attributeType, 0, false);
      LocalReference firstAttributeLocal = emitter.CodeBuilder.DeclareLocal (attributeType);
      emitter.CodeBuilder.AddStatement (new AssignStatement (firstAttributeLocal, firstAttributeExpression));
      return firstAttributeLocal;
    }

    private void ImplementGetObjectData ()
    {
      GetObjectMethodImplementer.ImplementGetObjectDataByDelegation (Emitter, delegate (CustomMethodEmitter newMethod, bool baseIsISerializable)
          {
            return new MethodInvocationExpression (
                null,
                typeof (SerializationHelper).GetMethod ("GetObjectDataForGeneratedTypes"),
                new ReferenceExpression (newMethod.ArgumentReferences[0]),
                new ReferenceExpression (newMethod.ArgumentReferences[1]),
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
          Emitter.CreateInterfacePropertyImplementation (typeof (IMixinTarget).GetProperty ("Configuration"));
      configurationProperty.GetMethod =
          Emitter.CreateInterfaceMethodImplementation (typeof (IMixinTarget).GetMethod ("get_Configuration"));
      configurationProperty.ImplementWithBackingField (_configurationField);

      CustomPropertyEmitter mixinsProperty =
          Emitter.CreateInterfacePropertyImplementation (typeof (IMixinTarget).GetProperty ("Mixins"));
      mixinsProperty.GetMethod =
          Emitter.CreateInterfaceMethodImplementation (typeof (IMixinTarget).GetMethod ("get_Mixins"));
      mixinsProperty.ImplementWithBackingField (_extensionsField);

      CustomPropertyEmitter firstProperty =
          Emitter.CreateInterfacePropertyImplementation (typeof (IMixinTarget).GetProperty ("FirstBaseCallProxy"));
      firstProperty.GetMethod =
          Emitter.CreateInterfaceMethodImplementation (typeof (IMixinTarget).GetMethod ("get_FirstBaseCallProxy"));
      firstProperty.ImplementWithBackingField (_firstField);
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
      CustomMethodEmitter customMethodEmitter = Emitter.CreateInterfaceMethodImplementation (interfaceMember);
      ExpressionReference implementer =
          new ExpressionReference (interfaceMember.DeclaringType, implementerExpression, customMethodEmitter);
      customMethodEmitter.ImplementByDelegating (implementer, interfaceMember);

      ReplicateAttributes (implementingMember, customMethodEmitter);
      return customMethodEmitter;
    }

    private CustomPropertyEmitter ImplementIntroducedProperty (Expression implementerExpression, PropertyIntroductionDefinition property)
    {
      CustomPropertyEmitter propertyEmitter = Emitter.CreateInterfacePropertyImplementation (property.InterfaceMember);

      if (property.IntroducesGetMethod)
        propertyEmitter.GetMethod = ImplementIntroducedMethod (
            implementerExpression,
            property.ImplementingMember.GetMethod,
            property.InterfaceMember.GetGetMethod());

      if (property.IntroducesSetMethod)
        propertyEmitter.SetMethod = ImplementIntroducedMethod (
            implementerExpression,
            property.ImplementingMember.SetMethod,
            property.InterfaceMember.GetSetMethod());

      ReplicateAttributes (property.ImplementingMember, propertyEmitter);
      return propertyEmitter;
    }

    private CustomEventEmitter ImplementIntroducedEvent (EventIntroductionDefinition eventIntro, Expression implementerExpression)
    {
      Assertion.IsNotNull (eventIntro.ImplementingMember.AddMethod);
      Assertion.IsNotNull (eventIntro.ImplementingMember.RemoveMethod);

      CustomEventEmitter eventEmitter = Emitter.CreateInterfaceEventImplementation (eventIntro.InterfaceMember);
      eventEmitter.AddMethod = ImplementIntroducedMethod (
          implementerExpression,
          eventIntro.ImplementingMember.AddMethod,
          eventIntro.InterfaceMember.GetAddMethod());
      eventEmitter.RemoveMethod = ImplementIntroducedMethod (
          implementerExpression,
          eventIntro.ImplementingMember.RemoveMethod,
          eventIntro.InterfaceMember.GetRemoveMethod());

      ReplicateAttributes (eventIntro.ImplementingMember, eventEmitter);
      return eventEmitter;
    }

    private void ImplementRequiredDuckMethods ()
    {
      foreach (RequiredFaceTypeDefinition faceRequirement in Configuration.RequiredFaceTypes)
      {
        if (faceRequirement.Type.IsInterface && !Configuration.ImplementedInterfaces.Contains (faceRequirement.Type)
            && !Configuration.IntroducedInterfaces.ContainsKey (faceRequirement.Type))
        {
          foreach (RequiredMethodDefinition requiredMethod in faceRequirement.Methods)
            ImplementRequiredDuckMethod (requiredMethod);
        }
      }
    }

    private void ImplementRequiredDuckMethod (RequiredMethodDefinition requiredMethod)
    {
      Assertion.IsTrue (requiredMethod.ImplementingMethod.DeclaringClass == Configuration,
        "Duck typing is only supported with members from the base type");

      CustomMethodEmitter methodImplementation = _emitter.CreateInterfaceMethodImplementation (requiredMethod.InterfaceMethod);
      methodImplementation.ImplementByDelegating (SelfReference.Self, requiredMethod.ImplementingMethod.MethodInfo);
    }

    private void ImplementOverrides ()
    {
      foreach (MemberDefinition member in _configuration.GetAllMembers())
      {
        if (member.Overrides.Count > 0)
        {
          IAttributableEmitter emitter = ImplementOverride (member);
          ImplementAttributes (member, emitter);
        }
      }
    }

    private IAttributableEmitter ImplementOverride (MemberDefinition member)
    {
      MethodDefinition method = member as MethodDefinition;
      if (method != null)
        return ImplementMethodOverride (method);

      PropertyDefinition property = member as PropertyDefinition;
      if (property != null)
        return ImplementPropertyOverride (property);

      EventDefinition eventDefinition = member as EventDefinition;
      Assertion.IsNotNull (eventDefinition, "Only methods, properties, and events can be overridden.");
      return ImplementEventOverride (eventDefinition);
    }

    private CustomMethodEmitter ImplementMethodOverride (MethodDefinition method)
    {
      MethodInfo proxyMethod = _baseCallGenerator.GetProxyMethodForOverriddenMethod (method);
      CustomMethodEmitter methodOverride = Emitter.CreateMethodOverride (method.MethodInfo);
      methodOverride.ImplementByDelegating (_firstField, proxyMethod);
      return methodOverride;
    }

    private CustomPropertyEmitter ImplementPropertyOverride (PropertyDefinition property)
    {
      CustomPropertyEmitter propertyOverride = Emitter.CreatePropertyOverride (property.PropertyInfo);
      if (property.GetMethod != null && property.GetMethod.Overrides.Count > 0)
        propertyOverride.GetMethod = ImplementMethodOverride (property.GetMethod);
      if (property.SetMethod != null && property.SetMethod.Overrides.Count > 0)
        propertyOverride.SetMethod = ImplementMethodOverride (property.SetMethod);
      return propertyOverride;
    }

    private CustomEventEmitter ImplementEventOverride (EventDefinition eventDefinition)
    {
      CustomEventEmitter eventOverride = Emitter.CreateEventOverride (eventDefinition.EventInfo);
      if (eventDefinition.AddMethod.Overrides.Count > 0)
        eventOverride.AddMethod = ImplementMethodOverride (eventDefinition.AddMethod);
      if (eventDefinition.RemoveMethod.Overrides.Count > 0)
        eventOverride.RemoveMethod = ImplementMethodOverride (eventDefinition.RemoveMethod);
      return eventOverride;
    }

    private void ImplementAttributes (IAttributeIntroductionTargetDefinition targetConfiguration, IAttributableEmitter targetEmitter)
    {
      foreach (AttributeDefinition attribute in targetConfiguration.CustomAttributes)
      {
        // only replicate those attributes from the base which are not inherited anyway
        if (!CanInheritAttributesFromBase (targetConfiguration) || !AttributeUtility.IsAttributeInherited (attribute.AttributeType))
          AttributeReplicator.ReplicateAttribute (targetEmitter, attribute.Data);
      }

      // Replicate introduced attributes
      foreach (AttributeIntroductionDefinition attribute in targetConfiguration.IntroducedAttributes)
        AttributeReplicator.ReplicateAttribute (targetEmitter, attribute.Attribute.Data);
    }

    private bool CanInheritAttributesFromBase (IAttributeIntroductionTargetDefinition configuration)
    {
      // only methods and base classes can supply attributes for inheritance
      return configuration is TargetClassDefinition || configuration is MethodDefinition;
    }

    private void ReplicateAttributes (IAttributableDefinition source, IAttributableEmitter targetEmitter)
    {
      foreach (AttributeDefinition attribute in source.CustomAttributes)
        AttributeReplicator.ReplicateAttribute (targetEmitter, attribute.Data);
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

    private void AddMixedTypeAttribute ()
    {
      CustomAttributeBuilder attributeBuilder = ConcreteMixedTypeAttribute.BuilderFromClassContext (Configuration.ConfigurationContext);
      Emitter.AddCustomAttribute (attributeBuilder);
    }

    public MethodInfo GetBaseCallMethod (MethodInfo overriddenMethod)
    {
      ArgumentUtility.CheckNotNull ("method", overriddenMethod);
      if (!overriddenMethod.DeclaringType.IsAssignableFrom (TypeBuilder.BaseType))
      {
        string message = string.Format (
            "Cannot create base call method for a method defined on a different type than the base type: {0}.{1}.",
            overriddenMethod.DeclaringType.FullName,
            overriddenMethod.Name);
        throw new ArgumentException (message, "overriddenMethod");
      }
      if (!_baseCallMethods.ContainsKey (overriddenMethod))
        _baseCallMethods.Add (overriddenMethod, ImplementBaseCallMethod (overriddenMethod));
      return _baseCallMethods[overriddenMethod];
    }

    private MethodInfo ImplementBaseCallMethod (MethodInfo method)
    {
      Assertion.IsTrue (ReflectionUtility.IsPublicOrProtected (method));

      MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig;
      CustomMethodEmitter baseCallMethod = new CustomMethodEmitter (Emitter, "__base__" + method.Name, attributes);
      baseCallMethod.CopyParametersAndReturnType (method);
      baseCallMethod.ImplementByBaseCall (method);
      return baseCallMethod.MethodBuilder;
    }

    public MethodInfo GetPublicMethodWrapper (MethodDefinition methodToBeWrapped)
    {
      ArgumentUtility.CheckNotNull ("methodToBeWrapped", methodToBeWrapped);
      if (methodToBeWrapped.DeclaringClass != Configuration)
        throw new ArgumentException ("Only methods from class " + Configuration.FullName + " can be wrapped.");

      return Emitter.GetPublicMethodWrapper (methodToBeWrapped.MethodInfo).MethodBuilder;
    }
  }
}