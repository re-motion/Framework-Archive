// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Collections;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;
using ReflectionUtility=Remotion.Mixins.Utilities.ReflectionUtility;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  public class MixinTypeGenerator : IMixinTypeGenerator
  {
    private static readonly ConstructorInfo s_debuggerDisplayAttributeConstructor = 
        typeof (DebuggerDisplayAttribute).GetConstructor (new[] { typeof (string) });

    private readonly AttributeGenerator _attributeGenerator = new AttributeGenerator();

    private readonly ICodeGenerationModule _module;
    private readonly ITypeGenerator _targetGenerator;
    private readonly MixinDefinition _configuration;
    private readonly IClassEmitter _emitter;
    private readonly FieldReference _requestingClassContextField;
    private readonly FieldReference _identifierField;

    public MixinTypeGenerator (ICodeGenerationModule module, ITypeGenerator targetGenerator, MixinDefinition configuration, INameProvider nameProvider)
    {
      ArgumentUtility.CheckNotNull ("module", module);
      ArgumentUtility.CheckNotNull ("targetGenerator", targetGenerator);
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      ArgumentUtility.CheckNotNull ("nameProvider", nameProvider);

      Assertion.IsFalse (configuration.Type.ContainsGenericParameters);

      _module = module;
      _targetGenerator = targetGenerator;
      _configuration = configuration;

      string typeName = nameProvider.GetNewTypeName (configuration);
      typeName = CustomClassEmitter.FlattenTypeName (typeName);

      var interfaces = new[] { typeof (ISerializable), typeof (IGeneratedMixinType) };
      const TypeAttributes flags = TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable;

      bool forceUnsigned = !targetGenerator.IsAssemblySigned;
      _emitter = _module.CreateClassEmitter (typeName, configuration.Type, interfaces, flags, forceUnsigned);

      _requestingClassContextField = _emitter.CreateStaticField ("__requestingClassContext", typeof (ClassContext));
      _identifierField = _emitter.CreateStaticField ("__identifier", typeof (ConcreteMixinTypeIdentifier));
    }

    public IClassEmitter Emitter
    {
      get { return _emitter; }
    }

    public MixinDefinition Configuration
    {
      get { return _configuration; }
    }

    public bool IsAssemblySigned
    {
      get { return ReflectionUtility.IsAssemblySigned (Emitter.TypeBuilder.Assembly); }
    }

    public ConcreteMixinType GetBuiltType ()
    {
      Generate();
      Tuple<MethodInfo, MethodInfo>[] methodWrappers = GenerateMethodWrappers().ToArray();

      Type generatedType = Emitter.BuildType();
      var result = new ConcreteMixinType (generatedType);
      foreach (var methodWrapper in methodWrappers)
        result.AddMethodWrapper (methodWrapper.A, methodWrapper.B);

      return result;
    }

    protected virtual void Generate ()
    {
      AddTypeInitializer ();

      _emitter.ReplicateBaseTypeConstructors (delegate { }, delegate { });

      ImplementGetObjectData();

      AddMixinTypeAttribute();
      AddDebuggerAttributes();
      ReplicateAttributes (_configuration.CustomAttributes, _emitter);
      ImplementOverrides();
    }

    private void AddTypeInitializer ()
    {
      var typeInitializerEmitter = _emitter.CreateTypeConstructor ();

      var classContextSerializer = new CodeGenerationClassContextSerializer (typeInitializerEmitter.CodeBuilder);
      _configuration.TargetClass.ConfigurationContext.Serialize (classContextSerializer);
      typeInitializerEmitter.CodeBuilder.AddStatement (
          new AssignStatement (
              _requestingClassContextField, 
              classContextSerializer.GetConstructorInvocationExpression ()));

      var identifierSerializer = new CodeGenerationConcreteMixinTypeIdentifierSerializer (typeInitializerEmitter.CodeBuilder);
      _configuration.GetConcreteMixinTypeIdentifier().Serialize (identifierSerializer);
      typeInitializerEmitter.CodeBuilder.AddStatement (
          new AssignStatement (
              _identifierField,
              identifierSerializer.GetConstructorInvocationExpression ()));

      typeInitializerEmitter.CodeBuilder.AddStatement (new ReturnStatement ());
    }

    private IEnumerable<Tuple<MethodInfo, MethodInfo>> GenerateMethodWrappers ()
    {
      return from m in Configuration.GetProtectedOverriders()
             select Tuple.NewTuple (m.MethodInfo, Emitter.GetPublicMethodWrapper (m.MethodInfo));
    }

    private void AddMixinTypeAttribute ()
    {
      CustomAttributeBuilder attributeBuilder = 
          ConcreteMixinTypeAttributeUtility.CreateAttributeBuilder (Configuration.GetConcreteMixinTypeIdentifier());
      Emitter.AddCustomAttribute (attributeBuilder);
    }

    private void AddDebuggerAttributes ()
    {
      string debuggerString = "Derived mixin: " + _configuration.Type.Name + " on class " + _configuration.TargetClass.Type.Name;
      var debuggerAttribute = new CustomAttributeBuilder (s_debuggerDisplayAttributeConstructor, new object[] { debuggerString });
      Emitter.AddCustomAttribute (debuggerAttribute);
    }

    private void ImplementOverrides ()
    {
      var overrideInterfaceGenerator = OverrideInterfaceGenerator.CreateNestedGenerator (Emitter, "IOverriddenMethods");

      PropertyReference targetReference = GetTargetReference();
      foreach (MethodDefinition method in _configuration.GetAllMethods())
      {
        if (method.Overrides.Count > 1)
          throw new NotSupportedException ("The code generator does not support mixin methods with more than one override.");
        else if (method.Overrides.Count == 1)
        {
          if (method.Overrides[0].DeclaringClass != Configuration.TargetClass)
            throw new NotSupportedException ("The code generator only supports mixin methods to be overridden by the mixin's target class.");

          var methodOverride = _emitter.CreateMethodOverride (method.MethodInfo);
          MethodDefinition overrider = method.Overrides[0];
          MethodInfo methodToCall = GetOverriderMethodToCall (overrider);
          AddCallToOverrider (methodOverride, targetReference, methodToCall);

          overrideInterfaceGenerator.AddOverriddenMethod (method.MethodInfo);
        }
      }
    }

    private PropertyReference GetTargetReference()
    {
      PropertyInfo targetProperty = MixinReflector.GetTargetProperty (Emitter.TypeBuilder.BaseType);
      if (targetProperty == null)
      {
        throw new NotSupportedException (
            "The code generator does not support mixins with overridden methods or non-public overriders if the mixin doesn't derive from the "
            + "generic Mixin base classes.");
      }

      return new PropertyReference (SelfReference.Self, targetProperty);
    }

    private MethodInfo GetOverriderMethodToCall (MethodDefinition overrider)
    {
      if (overrider.MethodInfo.IsPublic)
        return overrider.MethodInfo.GetBaseDefinition();
      else
        return _targetGenerator.GetPublicMethodWrapper (overrider);
    }

    private void AddCallToOverrider (IMethodEmitter methodOverride, Reference targetReference, MethodInfo targetMethod)
    {
      LocalReference castTargetLocal = methodOverride.DeclareLocal (targetMethod.DeclaringType);
      methodOverride.AddStatement (
          new AssignStatement (
              castTargetLocal,
              new ConvertExpression (targetMethod.DeclaringType, targetReference.ToExpression())));

      methodOverride.ImplementByDelegating (castTargetLocal, targetMethod);
    }

    private void ReplicateAttributes (IEnumerable<AttributeDefinition> attributes, IAttributableEmitter target)
    {
      foreach (AttributeDefinition attribute in attributes)
      {
        // only replicate those attributes from the base which are not inherited anyway
        if (!attribute.IsCopyTemplate && !AttributeUtility.IsAttributeInherited (attribute.AttributeType))
          _attributeGenerator.GenerateAttribute (target, attribute.Data);
      }
    }

    private void ImplementGetObjectData ()
    {
      SerializationImplementer.ImplementGetObjectDataByDelegation (
          Emitter,
          (newMethod, baseIsISerializable) =>
          new MethodInvocationExpression (
              null,
              typeof (MixinSerializationHelper).GetMethod ("GetObjectDataForGeneratedTypes"),
              newMethod.ArgumentReferences[0].ToExpression (),
              newMethod.ArgumentReferences[1].ToExpression (),
              SelfReference.Self.ToExpression (),
              _requestingClassContextField.ToExpression(),
              _identifierField.ToExpression (),
              new ReferenceExpression (new ConstReference (!baseIsISerializable))));
    }
  }
}
