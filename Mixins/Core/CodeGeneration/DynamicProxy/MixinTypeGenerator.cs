using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Rubicon.Mixins.CodeGeneration.DynamicProxy.DPExtensions;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Utilities;
using Rubicon.Utilities;
using System.Runtime.Serialization;
using System.Reflection;

namespace Rubicon.Mixins.CodeGeneration.DynamicProxy
{
  internal class MixinTypeGenerator : IMixinTypeGenerator
  {
    private ModuleManager _module;
    private MixinDefinition _configuration;
    private ExtendedClassEmitter _emitter;
    private FieldReference _configurationField;

    public MixinTypeGenerator (ModuleManager module, MixinDefinition configuration, INameProvider nameProvider)
    {
      ArgumentUtility.CheckNotNull ("module", module);
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      Assertion.IsFalse (configuration.Type.ContainsGenericParameters);

      _module = module;
      _configuration = configuration;

      bool isSerializable = configuration.Type.IsSerializable || typeof (ISerializable).IsAssignableFrom(configuration.Type);

      string typeName = nameProvider.GetNewTypeName (configuration);

      Type[] interfaces = isSerializable ? new Type[] {typeof (ISerializable)} : new Type[0];

      ClassEmitter classEmitter = new ClassEmitter (_module.Scope, typeName, configuration.Type, interfaces, isSerializable);
      _emitter = new ExtendedClassEmitter (classEmitter);

      _configurationField = classEmitter.CreateStaticField ("__configuration", typeof (MixinDefinition));

      AddTypeInitializer ();

      _emitter.ReplicateBaseTypeConstructors ();

      if (isSerializable)
        ImplementGetObjectData();

      AddMixinTypeAttribute ();
      ReplicateAttributes (_configuration.CustomAttributes, _emitter);
      ImplementOverrides();
    }

    private void AddTypeInitializer ()
    {
      ConstructorEmitter emitter = _emitter.InnerEmitter.CreateTypeConstructor ();

      LocalReference firstAttributeLocal = _emitter.LoadCustomAttribute (emitter.CodeBuilder, typeof (ConcreteMixinTypeAttribute), 0);

      MethodInfo getMixinDefinitionMethod = typeof (ConcreteMixinTypeAttribute).GetMethod ("GetMixinDefinition");
      Assertion.IsNotNull (getMixinDefinitionMethod);

      emitter.CodeBuilder.AddStatement (new AssignStatement (_configurationField, 
          new VirtualMethodInvocationExpression (firstAttributeLocal, getMixinDefinitionMethod)));

      emitter.CodeBuilder.AddStatement (new ReturnStatement ());
    }

    private void AddMixinTypeAttribute ()
    {
      CustomAttributeBuilder attributeBuilder = ConcreteMixinTypeAttribute.BuilderFromClassContext (Configuration.MixinIndex,
          Configuration.BaseClass.ConfigurationContext);
      Emitter.AddCustomAttribute (attributeBuilder);
    }

    private void ImplementOverrides ()
    {
      PropertyReference targetReference = new PropertyReference (SelfReference.Self, MixinReflector.GetTargetProperty (TypeBuilder.BaseType));
      foreach (MethodDefinition method in _configuration.GetAllMethods())
      {
        if (method.Overrides.Count > 1)
          throw new NotSupportedException ("The code generator does not support mixin methods with more than one override.");
        else if (method.Overrides.Count == 1)
        {
          if (method.Overrides[0].DeclaringClass != Configuration.BaseClass)
            throw new NotSupportedException ("The code generator only supports mixin methods to be overridden by the mixin's base class.");

          CustomMethodEmitter methodOverride = _emitter.CreateMethodOverride (method.MethodInfo);
          LocalReference castTargetLocal = methodOverride.InnerEmitter.CodeBuilder.DeclareLocal (Configuration.BaseClass.Type);
          methodOverride.InnerEmitter.CodeBuilder.AddStatement (
              new AssignStatement (
                  castTargetLocal,
                  new CastClassExpression (Configuration.BaseClass.Type, targetReference.ToExpression())));

          methodOverride.ImplementMethodByDelegation (castTargetLocal, method.Overrides[0].MethodInfo);
        }
      }
    }

    public TypeBuilder TypeBuilder
    {
      get { return _emitter.TypeBuilder; }
    }

    public ExtendedClassEmitter Emitter
    {
      get { return _emitter; }
    }

    public MixinDefinition Configuration
    {
      get { return _configuration; }
    }

    public Type GetBuiltType ()
    {
      Type builtType = Emitter.InnerEmitter.BuildType();
      return builtType;
    }

    private void ReplicateAttributes (IEnumerable<AttributeDefinition> attributes, IAttributableEmitter target)
    {
      foreach (AttributeDefinition attribute in attributes)
        AttributeReplicator.ReplicateAttribute (target, attribute.Data);
    }

    private void ImplementGetObjectData ()
    {
      Emitter.ImplementGetObjectDataByDelegation (
          delegate (MethodEmitter newMethod, bool baseIsISerializable)
          {
            return new MethodInvocationExpression (
                null,
                typeof (MixinSerializationHelper).GetMethod ("GetObjectDataForGeneratedTypes"),
                new ReferenceExpression (newMethod.Arguments[0]),
                new ReferenceExpression (newMethod.Arguments[1]),
                new ReferenceExpression (SelfReference.Self),
                new ReferenceExpression (_configurationField),
                new ReferenceExpression (new ConstReference (!baseIsISerializable)));
          });
    }
  }
}