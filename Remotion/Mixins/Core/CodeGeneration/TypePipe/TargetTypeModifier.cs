// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370: Docs.
  public class TargetTypeModifier : ITargetTypeModifier
  {
    private static readonly ConstructorInfo s_debuggerBrowsableAttributeConstructor =
        MemberInfoFromExpressionUtility.GetConstructor (() => new DebuggerBrowsableAttribute (DebuggerBrowsableState.Never));

    private static readonly ConstructorInfo s_debuggerDisplayAttributeConstructor =
        MemberInfoFromExpressionUtility.GetConstructor (() => new DebuggerDisplayAttribute ("display message"));
    private static readonly PropertyInfo s_debuggerDisplayAttributeNameProperty =
        MemberInfoFromExpressionUtility.GetProperty ((DebuggerDisplayAttribute o) => o.Name);

    private static readonly ConstructorInfo s_mixinArrayInitializerCtor =
        MemberInfoFromExpressionUtility.GetConstructor (() => new MixinArrayInitializer (null, Type.EmptyTypes));
    private static readonly MethodInfo s_createMixinArrayMethod =
        MemberInfoFromExpressionUtility.GetMethod ((MixinArrayInitializer o) => o.CreateMixinArray (new object[0]));
    private static readonly MethodInfo s_checkMixinArrayMethod =
        MemberInfoFromExpressionUtility.GetMethod ((MixinArrayInitializer o) => o.CheckMixinArray (new object[0]));

    private static readonly MethodInfo s_initializeTargetMethod =
        MemberInfoFromExpressionUtility.GetMethod ((IInitializableMixinTarget o) => o.Initialize());
    private static readonly MethodInfo s_initializeTargetAfterDeserializationMethod =
        MemberInfoFromExpressionUtility.GetMethod ((IInitializableMixinTarget o) => o.InitializeAfterDeserialization(new object[0]));

    private static readonly PropertyInfo s_classContextProperty = MemberInfoFromExpressionUtility.GetProperty ((IMixinTarget o) => o.ClassContext);
    private static readonly PropertyInfo s_mixinProperty = MemberInfoFromExpressionUtility.GetProperty ((IMixinTarget o) => o.Mixins);
    private static readonly PropertyInfo s_firstNextCallProperty = MemberInfoFromExpressionUtility.GetProperty ((IMixinTarget o) => o.FirstNextCallProxy);

    private static readonly MethodInfo s_initializeMixinMethod =
        MemberInfoFromExpressionUtility.GetMethod ((IInitializableMixin o) => o.Initialize (null, null, false));

    private static readonly PropertyInfo s_currentMixedObjectInstantiationScopeProperty =
        MemberInfoFromExpressionUtility.GetProperty (() => MixedObjectInstantiationScope.Current);
    private static readonly PropertyInfo s_suppliedMixinInstancesProperty =
        MemberInfoFromExpressionUtility.GetProperty ((MixedObjectInstantiationScope o) => o.SuppliedMixinInstances);

    private static readonly Expression s_false = Expression.Constant (false);
    private static readonly Expression s_true = Expression.Constant (true);


    private readonly IExpressionBuilder _expressionBuilder;

    public TargetTypeModifier (IExpressionBuilder expressionBuilder)
    {
      ArgumentUtility.CheckNotNull ("expressionBuilder", expressionBuilder);

      _expressionBuilder = expressionBuilder;
    }

    public TargetTypeModifierContext CreateContext (Type target, MutableType concreteTarget)
    {
      ArgumentUtility.CheckNotNull ("target", target);
      ArgumentUtility.CheckNotNull ("concreteTarget", concreteTarget);

      return new TargetTypeModifierContext (target, concreteTarget);
    }

    public void ImplementInterfaces (TargetTypeModifierContext context, IEnumerable<Type> interfacesToImplement)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("interfacesToImplement", interfacesToImplement);

      foreach (var ifc in interfacesToImplement)
        context.ConcreteTarget.AddInterface (ifc);
    }

    public void AddFields (TargetTypeModifierContext context, Type nextCallProxyType)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("nextCallProxyType", nextCallProxyType);

      var ct = context.ConcreteTarget;
      var privateStatic = FieldAttributes.Private | FieldAttributes.Static;
      context.ClassContextField = AddDebuggerInvisibleField (ct, "__classContext", typeof (ClassContext), privateStatic);
      context.MixinArrayInitializerField = AddDebuggerInvisibleField (ct, "__mixinArrayInitializer", typeof (MixinArrayInitializer), privateStatic);
      context.ExtensionsField = AddDebuggerInvisibleField (ct, "__extensions", typeof (object[]), FieldAttributes.Private);
      context.FirstField = AddDebuggerInvisibleField (ct, "__first", nextCallProxyType, FieldAttributes.Private);
    }

    public void AddTypeInitializations (TargetTypeModifierContext context, ClassContext classContext, IEnumerable<Type> concreteMixinTypes)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("concreteMixinTypes", concreteMixinTypes);

      context.ConcreteTarget.AddTypeInitialization (
          ctx => Expression.Block (
              typeof (void),
              InitializeClassContextField (context.ClassContextField, classContext),
              InitializeMixinArrayInitializerField (context.MixinArrayInitializerField, context.Target, concreteMixinTypes)));
    }

    public void AddInitializations (TargetTypeModifierContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      context.ConcreteTarget.AddInitialization (ctx => _expressionBuilder.CreateInitializationExpression (ctx.DeclaringType, context.ExtensionsField));
    }

    public void ImplementIInitializableMixinTarget (TargetTypeModifierContext context, IEnumerable<Type> expectedMixinTypes)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("expectedMixinTypes", expectedMixinTypes);

      var mixinTypes = expectedMixinTypes.ToList();
      var ct = context.ConcreteTarget;

      ct.AddExplicitOverride (
          s_initializeTargetMethod,
          ctx => Expression.Block (
              ImplementSettingFirstNextCallProxy (ctx.This, context.FirstField, context.NextCallProxyConstructor),
              ImplementCreatingMixinInstances (ctx.This, context.MixinArrayInitializerField, context.ExtensionsField),
              ImplementInitializingMixins (ctx.This, mixinTypes, context.ExtensionsField, context.NextCallProxyConstructor, deserialization: s_false)));

      ct.AddExplicitOverride (
          s_initializeTargetAfterDeserializationMethod,
          ctx => Expression.Block (
              ImplementSettingFirstNextCallProxy (ctx.This, context.FirstField, context.NextCallProxyConstructor),
              ImplementSettingMixinInstances (ctx.This, ctx.Parameters[0], context.MixinArrayInitializerField, context.ExtensionsField),
              ImplementInitializingMixins (ctx.This, mixinTypes, context.ExtensionsField, context.NextCallProxyConstructor, deserialization: s_true)));
    }

    public void ImplementIMixinTarget (TargetTypeModifierContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var ct = context.ConcreteTarget;
      var noInitialization = Expression.Empty();
      var classContextDebuggerDisplay = "Class context for " + context.Target.Name;
      // Initialize this instance in case we're being called before the ctor has finished running.
      var initialization = _expressionBuilder.CreateInitializationExpression (ct, context.ExtensionsField);

      ImplementReadOnlyProperty (ct, context.ClassContextField, noInitialization, s_classContextProperty, "ClassContext", classContextDebuggerDisplay);
      ImplementReadOnlyProperty (ct, context.ExtensionsField, initialization, s_mixinProperty, "Mixins", "Count = {__extensions.Length}");
      ImplementReadOnlyProperty (ct, context.FirstField, initialization, s_firstNextCallProperty, "FirstNextCallProxy", "Generated proxy");
    }

    public void ImplementIntroducedInterfaces (TargetTypeModifierContext context, TargetClassDefinition targetClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var ct = context.ConcreteTarget;
      foreach (var introduction in targetClassDefinition.ReceivedInterfaces)
      {
        var implementer = GetIntroducedInterfaceImplementer (ct, context.ExtensionsField, introduction);

        foreach (var method in introduction.IntroducedMethods)
          ImplementIntroducedMethod (ct, implementer, method);

      }

    }

    private void ImplementIntroducedMethod (MutableType concreteType, Expression implementer, MethodIntroductionDefinition method)
    {
      throw new NotImplementedException();
    }

    public void ImplementRequiredDuckMethods (TargetTypeModifierContext context)
    {
      throw new NotImplementedException();
    }

    public void AddMixedTypeAttribute (TargetTypeModifierContext context)
    {
      throw new NotImplementedException();
    }

    public void AddDebuggerAttributes (TargetTypeModifierContext context)
    {
      throw new NotImplementedException();
    }

    public void ImplementOverrides (TargetTypeModifierContext context)
    {
      throw new NotImplementedException();
    }

    public void ImplementOverridingMethods (TargetTypeModifierContext context)
    {
      throw new NotImplementedException();
    }

    private MutableFieldInfo AddDebuggerInvisibleField (MutableType targetType, string name, Type type, FieldAttributes attributes)
    {
      var field = targetType.AddField (name, attributes, type);
      var debuggerAttribute = new CustomAttributeDeclaration (s_debuggerBrowsableAttributeConstructor, new object[] { DebuggerBrowsableState.Never });
      field.AddCustomAttribute (debuggerAttribute);

      return field;
    }

    private BinaryExpression InitializeClassContextField (FieldInfo classContextField, ClassContext classContext)
    {
      // __classContext = new ClassContext (...);

      return Expression.Assign (Expression.Field (null, classContextField), _expressionBuilder.CreateNewClassContextExpression (classContext));
    }

    private static BinaryExpression InitializeMixinArrayInitializerField (
        FieldInfo mixinArrayInitializerField, Type targetType, IEnumerable<Type> concreteMixinTypes)
    {
      // __mixinArrayInitializer = new MixinArrayInitializer (targetType, concreteMixinTypes);

      return Expression.Assign (
          Expression.Field (null, mixinArrayInitializerField),
          Expression.New (
              s_mixinArrayInitializerCtor,
              Expression.Constant (targetType),
              Expression.Constant (concreteMixinTypes.ToArray())));
    }

    private Expression ImplementSettingFirstNextCallProxy (ThisExpression @this, FieldInfo firstField, ConstructorInfo nextCallProxyConstructor)
    {
      // __first = <NewNextCallProxy (0)>;

      return Expression.Assign (Expression.Field (@this, firstField), NewNextCallProxy (nextCallProxyConstructor, @this, depth: 0));
    }

    private Expression ImplementCreatingMixinInstances (ThisExpression @this, FieldInfo mixinArrayInitializerField, FieldInfo extensionsField)
    {
      // __extensions = __mixinArrayInitializer.CreateMixinArray (MixedObjectInstantiationScope.Current.SuppliedMixinInstances);

      return Expression.Assign (
          Expression.Field (@this, extensionsField),
          Expression.Call (
              Expression.Field (null, mixinArrayInitializerField),
              s_createMixinArrayMethod,
              Expression.Property (Expression.Property (null, s_currentMixedObjectInstantiationScopeProperty), s_suppliedMixinInstancesProperty)));
    }

    private Expression ImplementSettingMixinInstances (
        ThisExpression @this, Expression mixinInstancs, FieldInfo mixinArrayInitializerField, FieldInfo extensionsField)
    {
      // __mixinArrayInitializer.CheckMixinArray (<arguments[0]>);
      // __extensions = <arguments[0]>;

      return Expression.Block (
          Expression.Call (Expression.Field (null, mixinArrayInitializerField), s_checkMixinArrayMethod, mixinInstancs),
          Expression.Assign (Expression.Field (@this, extensionsField), mixinInstancs));
    }

    private Expression ImplementInitializingMixins (
        ThisExpression @this, List<Type> mixinTypes, FieldInfo extensionsField, ConstructorInfo nextCallProxyConstructor, Expression deserialization)
    {
      var mixinInitExpressions = new List<Expression>();

      for (int i = 0; i < mixinTypes.Count; i++)
      {
        if (typeof (IInitializableMixin).IsAssignableFrom (mixinTypes[i]))
        {
          // ((IInitializableMixin) __extensions[i]).Initialize (mixinTargetInstance, <NewNextCallProxy (i + 1)>, deserialization);

          var initExpression = Expression.Call (
              Expression.Convert (
                  Expression.ArrayAccess (Expression.Field (@this, extensionsField), Expression.Constant (i)),
                  typeof (IInitializableMixin)),
              s_initializeMixinMethod,
              @this,
              NewNextCallProxy (nextCallProxyConstructor, @this, i + 1),
              deserialization);

          mixinInitExpressions.Add (initExpression);
        }
      }

      return Expression.Block (mixinInitExpressions);
    }

    private Expression NewNextCallProxy (ConstructorInfo nextCallProxyConstructor, ThisExpression @this, int depth)
    {
      // new NextCallProxy (this, depth)

      return Expression.New (nextCallProxyConstructor, @this, Expression.Constant (depth));
    }

    private void ImplementReadOnlyProperty (
        MutableType concreteTarget,
        FieldInfo backingField,
        Expression initialization,
        PropertyInfo interfaceProperty,
        string nameString,
        string debuggerDisplayString)
    {
      var name = MemberImplementationUtility.GetNameForExplicitImplementation (interfaceProperty);
      var getMethod = concreteTarget.AddExplicitOverride (
          interfaceProperty.GetGetMethod(),
          ctx => Expression.Block (initialization, Expression.Field (backingField.IsStatic ? null : ctx.This, backingField)));
      var property = concreteTarget.AddProperty (name, PropertyAttributes.None, getMethod, setMethod: null);

      var debuggerDisplayAttribute = new CustomAttributeDeclaration (
          s_debuggerDisplayAttributeConstructor,
          new object[] { debuggerDisplayString },
          new NamedArgumentDeclaration (s_debuggerDisplayAttributeNameProperty, nameString));
      property.AddCustomAttribute (debuggerDisplayAttribute);
    }

    private Expression GetIntroducedInterfaceImplementer (
        MutableType concreteTarget, FieldInfo extensionsField, InterfaceIntroductionDefinition introduction)
    {
      // ((InterfaceType) __extensions[implementerIndex])

      return Expression.Convert (
          Expression.ArrayAccess (
              Expression.Field (new ThisExpression (concreteTarget), extensionsField),
              Expression.Constant (introduction.Implementer.MixinIndex)),
          introduction.InterfaceType);
    }
  }
}