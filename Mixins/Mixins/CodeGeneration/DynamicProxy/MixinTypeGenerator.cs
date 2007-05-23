using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Mixins.CodeGeneration.DynamicProxy.DPExtensions;
using Mixins.Definitions;
using Mixins.Definitions.Building;
using Mixins.Utilities;
using Rubicon.Utilities;
using ReflectionUtility=Mixins.Utilities.ReflectionUtility;

namespace Mixins.CodeGeneration.DynamicProxy
{
  public class MixinTypeGenerator: IMixinTypeGenerator
  {
    private ModuleManager _module;
    private MixinDefinition _configuration;
    private ExtendedClassEmitter _emitter;

    public MixinTypeGenerator (ModuleManager module, MixinDefinition configuration)
    {
      ArgumentUtility.CheckNotNull ("module", module);
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      Assertion.Assert (!configuration.Type.ContainsGenericParameters);

      _module = module;
      _configuration = configuration;

      bool isSerializable = configuration.Type.IsSerializable;

      string typeName = string.Format ("{0}_Concrete_{1}", configuration.Type.FullName, Guid.NewGuid());

      ClassEmitter classEmitter = new ClassEmitter (_module.Scope, typeName, configuration.Type, new Type[0], isSerializable);
      _emitter = new ExtendedClassEmitter (classEmitter);

      _emitter.ReplicateBaseTypeConstructors ();

      /*if (isSerializable)
      {
        ImplementGetObjectData();
      }*/

      ReplicateAttributes (_configuration.CustomAttributes, _emitter);
      ImplementOverrides();
    }

    private void ImplementOverrides ()
    {
      PropertyReference targetReference = new PropertyReference (SelfReference.Self, MixinReflector.GetTargetProperty(TypeBuilder.BaseType));
      foreach (MethodDefinition method in _configuration.Methods)
      {
        if (method.Overrides.Count > 1)
          throw new NotSupportedException ("The code generator does not support mixin methods with more than one override.");
        else if (method.Overrides.Count == 1)
        {
          if (method.Overrides[0].DeclaringClass != Configuration.BaseClass)
            throw new NotSupportedException ("The code generator only supports mixin methods to be overridden by the mixin's base class.");
          
          CustomMethodEmitter methodOverride = _emitter.CreateMethodOverrideOrInterfaceImplementation (method.MethodInfo);
          LocalReference castTargetLocal = methodOverride.InnerEmitter.CodeBuilder.DeclareLocal (Configuration.BaseClass.Type);
          methodOverride.InnerEmitter.CodeBuilder.AddStatement (
              new AssignStatement (
                  castTargetLocal,
                  new CastClassExpression (Configuration.BaseClass.Type, targetReference.ToExpression())));

          methodOverride.ImplementMethodByDelegation (castTargetLocal, method.Overrides[0].MethodInfo);
          methodOverride.InnerEmitter.Generate();
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

    public TypeBuilder GetBuiltType()
    {
      return TypeBuilder;
    }

    private void ReplicateAttributes (IEnumerable<AttributeDefinition> attributes, IAttributableEmitter target)
    {
      foreach (AttributeDefinition attribute in attributes)
        AttributeReplicator.ReplicateAttribute (target, attribute.Data);
    }
  }
}