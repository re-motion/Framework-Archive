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
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy.TypeGeneration
{
  /// <summary>
  /// Generates the initialization code used to initialize a concrete mixed type and its mixins.
  /// </summary>
  public class InitializationCodeGenerator
  {
    private static readonly MethodInfo s_initializeMethod = typeof (IInitializableMixinTarget).GetMethod ("Initialize");
    private static readonly MethodInfo s_createBaseCallProxyMethod = typeof (IInitializableMixinTarget).GetMethod ("CreateBaseCallProxy");
    private static readonly MethodInfo s_setFirstBaseCallProxyMethod = typeof (IInitializableMixinTarget).GetMethod ("SetFirstBaseCallProxy");
    private static readonly MethodInfo s_setExtensionsMethod = typeof (IInitializableMixinTarget).GetMethod ("SetExtensions");

    private static readonly MethodInfo s_createMixinArrayMethod = typeof (MixinArrayInitializer).GetMethod ("CreateMixinArray");
    private static readonly MethodInfo s_initializeMixinMethod = typeof (IInitializableMixin).GetMethod ("Initialize");

    private readonly TargetClassDefinition _configuration;
    private readonly FieldReference _extensionsField;
    private readonly FieldReference _firstField;
    private readonly FieldReference _configurationField;

    public InitializationCodeGenerator (TargetClassDefinition configuration, FieldReference extensionsField, FieldReference firstField, FieldReference configurationField)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      ArgumentUtility.CheckNotNull ("extensionsField", extensionsField);
      ArgumentUtility.CheckNotNull ("firstField", firstField);
      ArgumentUtility.CheckNotNull ("configurationField", configurationField);

      _configuration = configuration;
      _extensionsField = extensionsField;
      _firstField = firstField;
      _configurationField = configurationField;
    }

    public void AddMixinArrayInitializerCreationStatements (
        AbstractCodeBuilder codeBuilder, 
        FieldReference targetField)
    {
      // var expectedMixinInfos = new MixinArrayInitializer.ExpectedMixinInfo[<configuration.Mixins.Count>];
      var expectedMixinInfosLocal = codeBuilder.DeclareLocal (typeof (MixinArrayInitializer.ExpectedMixinInfo[]));
      codeBuilder.AddStatement (
          new AssignStatement (
              expectedMixinInfosLocal, 
              new NewArrayExpression (_configuration.Mixins.Count, typeof (MixinArrayInitializer.ExpectedMixinInfo))));

      // expectedMixinInfos[0] = new MixinArrayInitializer.ExpectedMixinInfo (<configuration.Mixins[0].Type>, <configuration.Mixins[0].NeedsDerivedMixin>)
      // ...

      for (int i = 0; i < _configuration.Mixins.Count; ++i)
      {
        var newMixinInfoExpression = new NewInstanceExpression (
            typeof (MixinArrayInitializer.ExpectedMixinInfo),
            new[] { typeof (Type), typeof (bool) },
            new TypeTokenExpression (_configuration.Mixins[i].Type),
            new ConstReference (_configuration.Mixins[i].NeedsDerivedMixinType ()).ToExpression ());
        codeBuilder.AddStatement (new AssignArrayStatement (expectedMixinInfosLocal, i, newMixinInfoExpression));
      }

      // <targetField> = MixinArrayInitializer (<configuration.Type>, expectedMixinInfos, <configuration>);
      var newInitializerExpression = new NewInstanceExpression (
          typeof (MixinArrayInitializer), 
          new[] { typeof (Type), typeof (MixinArrayInitializer.ExpectedMixinInfo[]), typeof (TargetClassDefinition) },
          new TypeTokenExpression (_configuration.Type),
          expectedMixinInfosLocal.ToExpression(),
          _configurationField.ToExpression());

      codeBuilder.AddStatement (new AssignStatement (targetField, newInitializerExpression));
    }

    public Statement GetInitializationStatement ()
    {
      var initializationMethodCall = new ExpressionStatement (
          new VirtualMethodInvocationExpression (
              new TypeReferenceWrapper (SelfReference.Self, typeof (IInitializableMixinTarget)),
              s_initializeMethod,
              new ConstReference (false).ToExpression()));

      var condition = new SameConditionExpression (_extensionsField.ToExpression(), NullExpression.Instance);
      return new IfStatement (condition, initializationMethodCall);
    }

    public void ImplementIInitializableMixinTarget (IClassEmitter classEmitter, BaseCallProxyGenerator baseCallProxyGenerator, FieldReference mixinArrayInitializerField)
    {
      ArgumentUtility.CheckNotNull ("classEmitter", classEmitter);
      ArgumentUtility.CheckNotNull ("baseCallProxyGenerator", baseCallProxyGenerator);

      ImplementInitializeMethod (classEmitter, mixinArrayInitializerField);

      CustomMethodEmitter createProxyMethod =
          classEmitter.CreateInterfaceMethodImplementation (s_createBaseCallProxyMethod);
      createProxyMethod.ImplementByReturning (new NewInstanceExpression(baseCallProxyGenerator.Ctor,
          SelfReference.Self.ToExpression(), createProxyMethod.ArgumentReferences[0].ToExpression()));

      CustomMethodEmitter setProxyMethod =
          classEmitter.CreateInterfaceMethodImplementation (s_setFirstBaseCallProxyMethod);
      setProxyMethod.AddStatement (new AssignStatement (_firstField, 
          new ConvertExpression(baseCallProxyGenerator.TypeBuilder, setProxyMethod.ArgumentReferences[0].ToExpression ())));
      setProxyMethod.ImplementByReturningVoid ();

      CustomMethodEmitter setExtensionsMethod =
          classEmitter.CreateInterfaceMethodImplementation (s_setExtensionsMethod);
      setExtensionsMethod.AddStatement (new AssignStatement (_extensionsField, setExtensionsMethod.ArgumentReferences[0].ToExpression ()));
      setExtensionsMethod.ImplementByReturningVoid ();
    }

    private void ImplementInitializeMethod (IClassEmitter classEmitter, FieldReference mixinArrayInitializerField)
    {
      CustomMethodEmitter initializeMethod = classEmitter.CreateInterfaceMethodImplementation (s_initializeMethod);

      var selfAsInitializableTarget = new TypeReferenceWrapper (SelfReference.Self, typeof (IInitializableMixinTarget));
      ImplementSettingFirstBaseCallProxy (initializeMethod, selfAsInitializableTarget);

      LocalReference allMixinInstancesLocal = ImplementSettingMixinInstances (initializeMethod, mixinArrayInitializerField, selfAsInitializableTarget);
      ImplementInitializingMixins(initializeMethod, allMixinInstancesLocal, selfAsInitializableTarget);

      initializeMethod.AddStatement (new ReturnStatement ());
    }

    private void ImplementSettingFirstBaseCallProxy (CustomMethodEmitter initializeMethod, TypeReferenceWrapper selfAsInitializableTarget)
    {
      // ((IInitializableMixinTarget)this).SetFirstBaseCallProxy (((IInitializableMixinTarget)this).CreateBaseCallProxy (0));

      var firstBaseCallProxyExpression = new VirtualMethodInvocationExpression (
          selfAsInitializableTarget,
          s_createBaseCallProxyMethod,
          new ConstReference (0).ToExpression ());

      initializeMethod.AddStatement (
          new ExpressionStatement (
              new VirtualMethodInvocationExpression (
                  selfAsInitializableTarget,
                  s_setFirstBaseCallProxyMethod,
                  firstBaseCallProxyExpression)));
    }

    private LocalReference ImplementSettingMixinInstances (CustomMethodEmitter initializeMethod, FieldReference mixinArrayInitializerField, TypeReferenceWrapper selfAsInitializableTarget)
    {
      // object[] allMixinInstances = <mixinArrayInitializerField>.CreateMixinArray (MixedObjectInstantiationScope.Current.SuppliedMixinInstances);

      var currentMixedObjectInstantiationScope = new PropertyReference (null, typeof (MixedObjectInstantiationScope).GetProperty ("Current"));
      var suppliedMixinInstances = new PropertyReference (
          currentMixedObjectInstantiationScope,
          typeof (MixedObjectInstantiationScope).GetProperty ("SuppliedMixinInstances"));
      
      var allMixinInstancesLocal = initializeMethod.DeclareLocal (typeof (object[]));

      var allMixinInstances = new VirtualMethodInvocationExpression (
          new TypeReferenceWrapper (mixinArrayInitializerField, typeof (MixinArrayInitializer)),
          s_createMixinArrayMethod,
          suppliedMixinInstances.ToExpression());

      initializeMethod.AddStatement (new AssignStatement (allMixinInstancesLocal, allMixinInstances));

      // mixinTarget.SetExtensions (allMixinInstances)

      initializeMethod.AddStatement (
          new ExpressionStatement (
              new VirtualMethodInvocationExpression (
                  selfAsInitializableTarget,
                  s_setExtensionsMethod,
                  allMixinInstancesLocal.ToExpression ())));

      return allMixinInstancesLocal;
    }

    private void ImplementInitializingMixins (CustomMethodEmitter initializeMethod, LocalReference allMixinInstancesLocal, TypeReferenceWrapper selfAsInitializableTarget)
    {
      var initializableMixinLocal = initializeMethod.DeclareLocal (typeof (IInitializableMixin));
      for (int i = 0; i < _configuration.Mixins.Count; ++i)
      {
        if (typeof (IInitializableMixin).IsAssignableFrom (_configuration.Mixins[i].Type))
        {
          // var initializableMixin = (IInitializableMixin) allMixins[i];
          var castMixinExpression = new ConvertExpression (
              typeof (IInitializableMixin),
              typeof (object),
              new LoadArrayElementExpression (i, allMixinInstancesLocal, typeof (object)));

          initializeMethod.AddStatement (new AssignStatement (initializableMixinLocal, castMixinExpression));

          var baseCallProxyExpression = new VirtualMethodInvocationExpression (
              selfAsInitializableTarget,
              s_createBaseCallProxyMethod,
              new ConstReference (i + 1).ToExpression ());

          // initializableMixin.Initialize (mixinTargetInstance, CreateBaseCallProxy (i + 1), deserialization);
          initializeMethod.AddStatement (
              new ExpressionStatement (
                  new VirtualMethodInvocationExpression (
                      initializableMixinLocal,
                      s_initializeMixinMethod,
                      SelfReference.Self.ToExpression (),
                      baseCallProxyExpression,
                      initializeMethod.ArgumentReferences[0].ToExpression ())));
        }
      }
    }
  }
}