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


    private readonly MutableType _concreteTarget;
    private readonly IExpressionBuilder _expressionBuilder;
    private readonly IAttributeGenerator _attributeGenerator;

    private Expression _classContextField;
    private Expression _mixinArrayInitializerField;
    private Expression _extensionsField;
    private Expression _firstField;

    public TargetTypeModifier (MutableType concreteTarget, IExpressionBuilder expressionBuilder, IAttributeGenerator attributeGenerator)
    {
      ArgumentUtility.CheckNotNull ("concreteTarget", concreteTarget);
      ArgumentUtility.CheckNotNull ("expressionBuilder", expressionBuilder);
      ArgumentUtility.CheckNotNull ("attributeGenerator", attributeGenerator);

      _concreteTarget = concreteTarget;
      _expressionBuilder = expressionBuilder;
      _attributeGenerator = attributeGenerator;
    }

    public void AddInterfaces (IEnumerable<Type> interfacesToImplement)
    {
      ArgumentUtility.CheckNotNull ("interfacesToImplement", interfacesToImplement);

      foreach (var ifc in interfacesToImplement)
        _concreteTarget.AddInterface (ifc);
    }

    public void AddFields (Type nextCallProxyType)
    {
      ArgumentUtility.CheckNotNull ("nextCallProxyType", nextCallProxyType);

      var privateStatic = FieldAttributes.Private | FieldAttributes.Static;
      _classContextField = AddDebuggerInvisibleField ("__classContext", typeof (ClassContext), privateStatic);
      _mixinArrayInitializerField = AddDebuggerInvisibleField ("__mixinArrayInitializer", typeof (MixinArrayInitializer), privateStatic);
      _extensionsField = AddDebuggerInvisibleField ("__extensions", typeof (object[]), FieldAttributes.Private);
      _firstField = AddDebuggerInvisibleField ("__first", nextCallProxyType, FieldAttributes.Private);
    }

    public void AddTypeInitializations (ClassContext classContext, IEnumerable<Type> expectedMixinTypes)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);
      ArgumentUtility.CheckNotNull ("expectedMixinTypes", expectedMixinTypes);

      _concreteTarget.AddTypeInitialization (
          ctx => Expression.Block (
              typeof (void),
              InitializeClassContextField (classContext),
              InitializeMixinArrayInitializerField (classContext.Type, expectedMixinTypes)));
    }

    public void AddInitializations ()
    {
      _concreteTarget.AddInitialization (ctx => _expressionBuilder.CreateInitialization (ctx.DeclaringType, _extensionsField));
    }

    public void ImplementIInitializableMixinTarget (IList<Type> expectedMixinTypes, ConstructorInfo nextCallProxyConstructor)
    {
      ArgumentUtility.CheckNotNull ("expectedMixinTypes", expectedMixinTypes);

      _concreteTarget.AddExplicitOverride (
          s_initializeTargetMethod,
          ctx => Expression.Block (
              ImplementSettingFirstNextCallProxy (ctx.This, nextCallProxyConstructor),
              ImplementCreatingMixinInstances (),
              ImplementInitializingMixins (ctx.This, expectedMixinTypes, nextCallProxyConstructor, deserialization: s_false)));

      _concreteTarget.AddExplicitOverride (
          s_initializeTargetAfterDeserializationMethod,
          ctx => Expression.Block (
              ImplementSettingFirstNextCallProxy (ctx.This, nextCallProxyConstructor),
              ImplementSettingMixinInstances (ctx.Parameters[0]),
              ImplementInitializingMixins (ctx.This, expectedMixinTypes, nextCallProxyConstructor, deserialization: s_true)));
    }

    public void ImplementIMixinTarget (string targetClassName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("targetClassName", targetClassName);

      var noInitialization = Expression.Empty();
      var classContextDebuggerDisplay = "Class context for " + targetClassName;
      // Initialize this instance in case we're being called before the ctor has finished running.
      var initialization = _expressionBuilder.CreateInitialization (_concreteTarget, _extensionsField);

      ImplementReadOnlyProperty (_classContextField, noInitialization, s_classContextProperty, "ClassContext", classContextDebuggerDisplay);
      ImplementReadOnlyProperty (_extensionsField, initialization, s_mixinProperty, "Mixins", "Count = {__extensions.Length}");
      ImplementReadOnlyProperty (_firstField, initialization, s_firstNextCallProperty, "FirstNextCallProxy", "Generated proxy");
    }

    public void ImplementIntroducedInterfaces (IEnumerable<InterfaceIntroductionDefinition> introducedInterfaces)
    {
      ArgumentUtility.CheckNotNull ("introducedInterfaces", introducedInterfaces);

      foreach (var introduction in introducedInterfaces)
      {
        var implementer = GetIntroducedInterfaceImplementer (introduction);

        foreach (var method in introduction.IntroducedMethods)
          ImplementIntroducedMethod (implementer, method.InterfaceMember, method.ImplementingMember, method.Visibility);
        foreach (var property in introduction.IntroducedProperties)
          ImplementIntroducedProperty (implementer, property);
        foreach (var @event in introduction.IntroducedEvents)
          ImplementIntroducedEvent (implementer, @event);
      }
    }

    public virtual MutableMethodInfo ImplementIntroducedMethod (
        Expression implementer, MethodInfo interfaceMethod, MethodDefinition implementingMethod, MemberVisibility visibility)
    {
      var method = visibility == MemberVisibility.Public
              ? _concreteTarget.GetOrAddOverride (interfaceMethod)
              : _concreteTarget.AddExplicitOverride (interfaceMethod, ctx => Expression.Default (ctx.ReturnType));

      method.SetBody (ctx => _expressionBuilder.CreateInitializingDelegation (ctx, _extensionsField, implementer, interfaceMethod));

      _attributeGenerator.AddIntroducedMemberAttribute (method, interfaceMethod, implementingMethod);
      _attributeGenerator.ReplicateAttributes (implementingMethod, method);

      return method;
    }

    public virtual void ImplementIntroducedProperty (Expression implementer, PropertyIntroductionDefinition introducedProperty)
    {
      var interfaceProperty = introducedProperty.InterfaceMember;
      var implementingProperty = introducedProperty.ImplementingMember;
      var visibility = introducedProperty.Visibility;

      MutableMethodInfo getMethod = null, setMethod = null;
      if (introducedProperty.IntroducesGetMethod)
      {
        getMethod = ImplementIntroducedMethod (implementer, interfaceProperty.GetGetMethod(), implementingProperty.GetMethod, visibility);
      }
      if (introducedProperty.IntroducesSetMethod)
      {
        setMethod = ImplementIntroducedMethod (implementer, interfaceProperty.GetSetMethod(), implementingProperty.SetMethod, visibility);
      }

      var name = GetIntroducedMemberName (visibility, interfaceProperty);
      var property = _concreteTarget.AddProperty (name, PropertyAttributes.None, getMethod, setMethod);

      _attributeGenerator.AddIntroducedMemberAttribute (property, interfaceProperty, implementingProperty);
      _attributeGenerator.ReplicateAttributes (implementingProperty, property);
    }

    public virtual void ImplementIntroducedEvent (Expression implementer, EventIntroductionDefinition introducedEvent)
    {
      var interfaceEvent = introducedEvent.InterfaceMember;
      var implementingEvent = introducedEvent.ImplementingMember;
      var visibility = introducedEvent.Visibility;

      var addMethod = ImplementIntroducedMethod (implementer, interfaceEvent.GetAddMethod(), implementingEvent.AddMethod, visibility);
      var removeMethod = ImplementIntroducedMethod (implementer, interfaceEvent.GetRemoveMethod(), implementingEvent.RemoveMethod, visibility);

      var name = GetIntroducedMemberName (visibility, interfaceEvent);
      var @event = _concreteTarget.AddEvent (name, EventAttributes.None, addMethod, removeMethod);

      _attributeGenerator.AddIntroducedMemberAttribute (@event, interfaceEvent, implementingEvent);
      _attributeGenerator.ReplicateAttributes (implementingEvent, @event);
    }

    public void ImplementRequiredDuckMethods (TargetClassDefinition targetClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);

      foreach (var faceRequirement in targetClassDefinition.RequiredTargetCallTypes)
      {
        if (faceRequirement.Type.IsInterface && !targetClassDefinition.ImplementedInterfaces.Contains (faceRequirement.Type)
            && !targetClassDefinition.ReceivedInterfaces.ContainsKey (faceRequirement.Type))
        {
          foreach (var requiredMethod in faceRequirement.Methods)
          {
            Assertion.IsTrue (
                requiredMethod.ImplementingMethod.DeclaringClass == targetClassDefinition,
                "Duck typing is only supported with members from the base type.");

            ImplementRequiredDuckMethod (requiredMethod);
          }
        }
      }
    }

    public void ImplementAttributes (IAttributeIntroductionTarget targetConfiguration, TargetClassDefinition targetClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("targetConfiguration", targetConfiguration);
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);

      ImplementAttributes (_concreteTarget, targetConfiguration, targetClassDefinition);
    }

    public virtual void ImplementAttributes (
        IMutableMember member, IAttributeIntroductionTarget targetConfiguration, TargetClassDefinition targetClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      ArgumentUtility.CheckNotNull ("targetConfiguration", targetConfiguration);
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);

      foreach (var attribute in targetConfiguration.CustomAttributes)
      {
        if (_attributeGenerator.ShouldBeReplicated (attribute, targetConfiguration, targetClassDefinition))
          _attributeGenerator.AddAttribute (member, attribute.Data);
      }

      foreach (var introducedAttribute in targetConfiguration.ReceivedAttributes)
        _attributeGenerator.AddAttribute (member, introducedAttribute.Attribute.Data);
    }

    public void AddMixedTypeAttribute (TargetClassDefinition targetClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);

      var classContext = targetClassDefinition.ConfigurationContext;
      var orderedMixinTypes = targetClassDefinition.Mixins.Select (m => m.Type);

      _attributeGenerator.AddConcreteMixedTypeAttribute (_concreteTarget, classContext, orderedMixinTypes);
    }

    public void AddDebuggerDisplayAttribute (TargetClassDefinition targetClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);

      if (!targetClassDefinition.ReceivedAttributes.ContainsKey (typeof (DebuggerDisplayAttribute))
          && !targetClassDefinition.CustomAttributes.ContainsKey (typeof (DebuggerDisplayAttribute)))
      {
        var debuggerDisplayString = "{ToString(),nq} (mixed)";
        _attributeGenerator.AddDebuggerDisplayAttribute (_concreteTarget, debuggerDisplayString, debuggerDisplayNameStringOrNull: null);
      }
    }

    public void ImplementOverrides (TargetClassDefinition targetClassDefinition, INextCallProxy nextCallProxy)
    {
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);

      foreach (var member in targetClassDefinition.GetAllMembers())
      {
        if (member.Overrides.Count > 0)
        {
          var memberOverride = ImplementOverride (member, nextCallProxy);
          ImplementAttributes (memberOverride, member, targetClassDefinition);
        }
      }
    }

    public virtual IMutableMember ImplementOverride (MemberDefinitionBase member, INextCallProxy nextCallProxy)
    {
      ArgumentUtility.CheckNotNull ("member", member);

      if (member is MethodDefinition)
        return ImplementMethodOverride ((MethodDefinition) member, nextCallProxy);
      if (member is PropertyDefinition)
        return ImplementPropertyOverride ((PropertyDefinition) member, nextCallProxy);
      Assertion.IsNotNull (member is EventDefinition, "Only methods, properties, and events can be overridden.");
      return ImplementEventOverride ((EventDefinition) member, nextCallProxy);
    }

    public virtual MutableMethodInfo ImplementMethodOverride (MethodDefinition method, INextCallProxy nextCallProxy)
    {
      ArgumentUtility.CheckNotNull ("method", method);

      var proxyMethod = nextCallProxy.GetProxyMethodForOverriddenMethod (method);
      var methodOverride = _concreteTarget.GetOrAddOverride (method.MethodInfo);
      methodOverride.SetBody (ctx => _expressionBuilder.CreateInitializingDelegation (ctx, _extensionsField, _firstField, proxyMethod));

      return methodOverride;
    }

    public virtual IMutableMember ImplementPropertyOverride (PropertyDefinition property, INextCallProxy nextCallProxy)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      MutableMethodInfo getMethodOverride = null, setMethodOverride = null;
      if (property.GetMethod != null && property.GetMethod.Overrides.Count > 0)
        getMethodOverride = ImplementMethodOverride (property.GetMethod, nextCallProxy);
      if (property.SetMethod != null && property.SetMethod.Overrides.Count > 0)
        setMethodOverride = ImplementMethodOverride (property.SetMethod, nextCallProxy);

      return _concreteTarget.AddProperty (property.Name, PropertyAttributes.None, getMethodOverride, setMethodOverride);
    }

    public virtual IMutableMember ImplementEventOverride (EventDefinition @event, INextCallProxy nextCallProxy)
    {
      MutableMethodInfo addMethodOverride = null, removeMethodOverride = null;
      if (@event.AddMethod.Overrides.Count > 0)
        addMethodOverride = ImplementMethodOverride (@event.AddMethod, nextCallProxy);
      if (@event.RemoveMethod.Overrides.Count > 0)
        removeMethodOverride = ImplementMethodOverride (@event.RemoveMethod, nextCallProxy);

      return _concreteTarget.AddEvent (@event.Name, EventAttributes.None, addMethodOverride, removeMethodOverride);
    }

    public void ImplementOverridingMethods (TargetClassDefinition targetClassDefinition, IList<ConcreteMixinType> concreteMixinTypesWithNulls)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinTypesWithNulls", concreteMixinTypesWithNulls);

      var overriders = targetClassDefinition.GetAllMethods().Where (methodDefinition => methodDefinition.Base != null);
      foreach (var overrider in overriders)
      {
        var mixin = overrider.Base.DeclaringClass as MixinDefinition;
        Assertion.IsNotNull (mixin, "We only support mixins as overriders of target class members.");
        var concreteMixinType = concreteMixinTypesWithNulls[mixin.MixinIndex];
        Assertion.IsNotNull (concreteMixinType, "If a mixin method is overridden, a concrete type must have been created for it.");

        var methodInOverrideInterface = concreteMixinType.GetOverrideInterfaceMethod (overrider.Base.MethodInfo);

        // It's necessary to explicitly implement some members defined by the concrete mixins' override interfaces: implicit implementation doesn't
        // work if the overrider is non-public or generic. Because it's simpler, we just implement all the members explicitly.
        var methodToCall = overrider.MethodInfo;
        _concreteTarget.AddExplicitOverride (methodInOverrideInterface, ctx => _expressionBuilder.CreateDelegation (ctx, ctx.This, methodToCall));
      }
    }

    private Expression AddDebuggerInvisibleField (string name, Type type, FieldAttributes attributes)
    {
      var field = _concreteTarget.AddField (name, attributes, type);
      _attributeGenerator.AddDebuggerBrowsableAttribute (field, DebuggerBrowsableState.Never);

      var instance = field.IsStatic ? null : new ThisExpression (_concreteTarget);
      return Expression.Field (instance, field);
    }

    private BinaryExpression InitializeClassContextField (ClassContext classContext)
    {
      // __classContext = new ClassContext (...);

      return Expression.Assign (_classContextField, _expressionBuilder.CreateNewClassContext (classContext));
    }

    private BinaryExpression InitializeMixinArrayInitializerField (Type targetType, IEnumerable<Type> concreteMixinTypes)
    {
      // __mixinArrayInitializer = new MixinArrayInitializer (targetClassName, expectedMixinTypes);

      return Expression.Assign (
          _mixinArrayInitializerField,
          Expression.New (s_mixinArrayInitializerCtor, Expression.Constant (targetType), Expression.ArrayConstant (concreteMixinTypes)));
    }

    private Expression ImplementSettingFirstNextCallProxy (ThisExpression @this, ConstructorInfo nextCallProxyConstructor)
    {
      // __first = <NewNextCallProxy (0)>;

      return Expression.Assign (_firstField, NewNextCallProxy (nextCallProxyConstructor, @this, depth: 0));
    }

    private Expression ImplementCreatingMixinInstances ()
    {
      // __extensions = __mixinArrayInitializer.CreateMixinArray (MixedObjectInstantiationScope.Current.SuppliedMixinInstances);

      return Expression.Assign (
          _extensionsField,
          Expression.Call (
              _mixinArrayInitializerField,
              s_createMixinArrayMethod,
              Expression.Property (Expression.Property (null, s_currentMixedObjectInstantiationScopeProperty), s_suppliedMixinInstancesProperty)));
    }

    private Expression ImplementSettingMixinInstances (Expression mixinInstancs)
    {
      // __mixinArrayInitializer.CheckMixinArray (<arguments[0]>);
      // __extensions = <arguments[0]>;

      return Expression.Block (
          Expression.Call (_mixinArrayInitializerField, s_checkMixinArrayMethod, mixinInstancs),
          Expression.Assign (_extensionsField, mixinInstancs));
    }

    private Expression ImplementInitializingMixins (
        ThisExpression @this, IList<Type> expectedMixinTypes, ConstructorInfo nextCallProxyConstructor, Expression deserialization)
    {
      var mixinInitExpressions = new List<Expression>();

      for (int i = 0; i < expectedMixinTypes.Count; i++)
      {
        if (typeof (IInitializableMixin).IsAssignableFrom (expectedMixinTypes[i]))
        {
          // ((IInitializableMixin) __extensions[i]).Initialize (mixinTargetInstance, <NewNextCallProxy (i + 1)>, deserialization);

          var initExpression = Expression.Call (
              Expression.Convert (
                  Expression.ArrayAccess (_extensionsField, Expression.Constant (i)),
                  typeof (IInitializableMixin)),
              s_initializeMixinMethod,
              @this,
              NewNextCallProxy (nextCallProxyConstructor, @this, i + 1),
              deserialization);

          mixinInitExpressions.Add (initExpression);
        }
      }

      return Expression.BlockOrEmpty (mixinInitExpressions);
    }

    private Expression NewNextCallProxy (ConstructorInfo nextCallProxyConstructor, ThisExpression @this, int depth)
    {
      // new NextCallProxy (this, depth)

      return Expression.New (nextCallProxyConstructor, @this, Expression.Constant (depth));
    }

    private void ImplementReadOnlyProperty (
        Expression backingField,
        Expression initialization,
        PropertyInfo interfaceProperty,
        string debuggerDisplayNameString,
        string debuggerDisplayString)
    {
      var name = MemberImplementationUtility.GetNameForExplicitImplementation (interfaceProperty);
      var getMethod = _concreteTarget.AddExplicitOverride (interfaceProperty.GetGetMethod(), ctx => Expression.Block (initialization, backingField));
      var property = _concreteTarget.AddProperty (name, PropertyAttributes.None, getMethod, setMethod: null);
      _attributeGenerator.AddDebuggerDisplayAttribute (property, debuggerDisplayString, debuggerDisplayNameString);
    }

    private Expression GetIntroducedInterfaceImplementer (InterfaceIntroductionDefinition introduction)
    {
      // ((InterfaceType) __extensions[implementerIndex])

      return Expression.Convert (
          Expression.ArrayAccess (_extensionsField, Expression.Constant (introduction.Implementer.MixinIndex)),
          introduction.InterfaceType);
    }

    private string GetIntroducedMemberName (MemberVisibility visibility, MemberInfo interfaceMember)
    {
      return visibility == MemberVisibility.Public
                 ? interfaceMember.Name
                 : MemberImplementationUtility.GetNameForExplicitImplementation (interfaceMember);
    }

    private void ImplementRequiredDuckMethod (RequiredMethodDefinition requiredMethod)
    {
      _concreteTarget.AddExplicitOverride (
          requiredMethod.InterfaceMethod,
          ctx => Expression.Call (ctx.This, requiredMethod.ImplementingMethod.MethodInfo, ctx.Parameters.Cast<Expression>()));
    }
  }
}