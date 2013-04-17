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


    private readonly IExpressionBuilder _expressionBuilder;
    private readonly IAttributeGenerator _attributeGenerator;

    public TargetTypeModifier (IExpressionBuilder expressionBuilder, IAttributeGenerator attributeGenerator)
    {
      ArgumentUtility.CheckNotNull ("expressionBuilder", expressionBuilder);
      ArgumentUtility.CheckNotNull ("attributeGenerator", attributeGenerator);

      _expressionBuilder = expressionBuilder;
      _attributeGenerator = attributeGenerator;
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

      context.ConcreteTarget.AddInitialization (ctx => _expressionBuilder.CreateInitialization (ctx.DeclaringType, context.ExtensionsField));
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
              ImplementCreatingMixinInstances (context.MixinArrayInitializerField, context.ExtensionsField),
              ImplementInitializingMixins (ctx.This, mixinTypes, context.ExtensionsField, context.NextCallProxyConstructor, deserialization: s_false)));

      ct.AddExplicitOverride (
          s_initializeTargetAfterDeserializationMethod,
          ctx => Expression.Block (
              ImplementSettingFirstNextCallProxy (ctx.This, context.FirstField, context.NextCallProxyConstructor),
              ImplementSettingMixinInstances (ctx.Parameters[0], context.MixinArrayInitializerField, context.ExtensionsField),
              ImplementInitializingMixins (ctx.This, mixinTypes, context.ExtensionsField, context.NextCallProxyConstructor, deserialization: s_true)));
    }

    public void ImplementIMixinTarget (TargetTypeModifierContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var ct = context.ConcreteTarget;
      var noInitialization = Expression.Empty();
      var classContextDebuggerDisplay = "Class context for " + context.Target.Name;
      // Initialize this instance in case we're being called before the ctor has finished running.
      var initialization = _expressionBuilder.CreateInitialization (ct, context.ExtensionsField);

      ImplementReadOnlyProperty (ct, context.ClassContextField, noInitialization, s_classContextProperty, "ClassContext", classContextDebuggerDisplay);
      ImplementReadOnlyProperty (ct, context.ExtensionsField, initialization, s_mixinProperty, "Mixins", "Count = {__extensions.Length}");
      ImplementReadOnlyProperty (ct, context.FirstField, initialization, s_firstNextCallProperty, "FirstNextCallProxy", "Generated proxy");
    }

    public void ImplementIntroducedInterfaces (TargetTypeModifierContext context, IEnumerable<InterfaceIntroductionDefinition> introducedInterfaces)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var ct = context.ConcreteTarget;
      var extensionsField = context.ExtensionsField;

      foreach (var introduction in introducedInterfaces)
      {
        var implementer = GetIntroducedInterfaceImplementer (extensionsField, introduction);

        foreach (var method in introduction.IntroducedMethods)
          ImplementIntroducedMethod (ct, extensionsField, implementer, method.InterfaceMember, method.ImplementingMember, method.Visibility);
        foreach (var property in introduction.IntroducedProperties)
          ImplementIntroducedProperty (ct, extensionsField, implementer, property);
        foreach (var @event in introduction.IntroducedEvents)
          ImplementIntroducedEvent (ct, extensionsField, implementer, @event);
      }
    }

    public virtual MutableMethodInfo ImplementIntroducedMethod (
        MutableType concreteTarget,
        Expression extensionsField,
        Expression implementer,
        MethodInfo interfaceMethod,
        MethodDefinition implementingMethod,
        MemberVisibility visibility)
    {
      var method = visibility == MemberVisibility.Public
              ? concreteTarget.GetOrAddOverride (interfaceMethod)
              : concreteTarget.AddExplicitOverride (interfaceMethod, ctx => Expression.Default (ctx.ReturnType));

      method.SetBody (ctx => _expressionBuilder.CreateInitializingDelegation (ctx, extensionsField, implementer, interfaceMethod));

      _attributeGenerator.AddIntroducedMemberAttribute (method, interfaceMethod, implementingMethod);
      _attributeGenerator.ReplicateAttributes (implementingMethod, method);

      return method;
    }

    public virtual void ImplementIntroducedProperty (
        MutableType concreteTarget, Expression extensionsField, Expression implementer, PropertyIntroductionDefinition introducedProperty)
    {
      var interfaceProperty = introducedProperty.InterfaceMember;
      var implementingProperty = introducedProperty.ImplementingMember;
      var visibility = introducedProperty.Visibility;

      MutableMethodInfo getMethod = null, setMethod = null;
      if (introducedProperty.IntroducesGetMethod)
      {
        getMethod = ImplementIntroducedMethod (
            concreteTarget, extensionsField, implementer, interfaceProperty.GetGetMethod(), implementingProperty.GetMethod, visibility);
      }
      if (introducedProperty.IntroducesSetMethod)
      {
        setMethod = ImplementIntroducedMethod (
            concreteTarget, extensionsField, implementer, interfaceProperty.GetSetMethod(), implementingProperty.SetMethod, visibility);
      }

      var name = GetIntroducedMemberName (visibility, interfaceProperty);
      var property = concreteTarget.AddProperty (name, PropertyAttributes.None, getMethod, setMethod);

      _attributeGenerator.AddIntroducedMemberAttribute (property, interfaceProperty, implementingProperty);
      _attributeGenerator.ReplicateAttributes (implementingProperty, property);
    }

    public virtual void ImplementIntroducedEvent (
        MutableType concreteTarget, Expression extensionsField, Expression implementer, EventIntroductionDefinition introducedEvent)
    {
      var interfaceEvent = introducedEvent.InterfaceMember;
      var implementingEvent = introducedEvent.ImplementingMember;
      var visibility = introducedEvent.Visibility;

      var addMethod = ImplementIntroducedMethod (
          concreteTarget, extensionsField, implementer, interfaceEvent.GetAddMethod(), implementingEvent.AddMethod, visibility);
      var removeMethod = ImplementIntroducedMethod (
          concreteTarget, extensionsField, implementer, interfaceEvent.GetRemoveMethod(), implementingEvent.RemoveMethod, visibility);

      var name = GetIntroducedMemberName (visibility, interfaceEvent);
      var @event = concreteTarget.AddEvent (name, EventAttributes.None, addMethod, removeMethod);

      _attributeGenerator.AddIntroducedMemberAttribute (@event, interfaceEvent, implementingEvent);
      _attributeGenerator.ReplicateAttributes (implementingEvent, @event);
    }

    public void ImplementRequiredDuckMethods (TargetTypeModifierContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var configuration = context.TargetClassDefinition;
      foreach (var faceRequirement in configuration.RequiredTargetCallTypes)
      {
        if (faceRequirement.Type.IsInterface && !configuration.ImplementedInterfaces.Contains (faceRequirement.Type)
            && !configuration.ReceivedInterfaces.ContainsKey (faceRequirement.Type))
        {
          foreach (var requiredMethod in faceRequirement.Methods)
          {
            Assertion.IsTrue (
                requiredMethod.ImplementingMethod.DeclaringClass == configuration,
                "Duck typing is only supported with members from the base type.");

            ImplementRequiredDuckMethod (context.ConcreteTarget, requiredMethod);
          }
        }
      }
    }

    public void ImplementAttributes (
        TargetTypeModifierContext context, IAttributeIntroductionTarget targetConfiguration, TargetClassDefinition targetClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("targetConfiguration", targetConfiguration);

      ImplementAttributes (context.ConcreteTarget, targetConfiguration, targetClassDefinition);
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

    public void AddMixedTypeAttribute (TargetTypeModifierContext context, TargetClassDefinition targetClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var classContext = targetClassDefinition.ConfigurationContext;
      var orderedMixinTypes = targetClassDefinition.Mixins.Select (m => m.Type);

      _attributeGenerator.AddMixedTypeAttribute (context.ConcreteTarget, classContext, orderedMixinTypes);
    }

    public void AddDebuggerDisplayAttribute (TargetTypeModifierContext context, TargetClassDefinition targetClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);

      if (!targetClassDefinition.ReceivedAttributes.ContainsKey (typeof (DebuggerDisplayAttribute))
          && !targetClassDefinition.CustomAttributes.ContainsKey (typeof (DebuggerDisplayAttribute)))
      {
        var debuggerDisplayString = "{ToString(),nq} (mixed)";
        _attributeGenerator.AddDebuggerDisplayAttribute (context.ConcreteTarget, debuggerDisplayString, debuggerDisplayNameStringOrNull: null);
      }
    }

    public void ImplementOverrides (TargetTypeModifierContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      foreach (var memberDefinition in context.TargetClassDefinition.GetAllMembers())
      {
        if (memberDefinition.Overrides.Count > 0)
        {
          var memberOverride = ImplementOverride (context, memberDefinition);
          ImplementAttributes (memberOverride, memberDefinition, context.TargetClassDefinition);
        }
      }
    }

    public virtual IMutableMember ImplementOverride (TargetTypeModifierContext context, MemberDefinitionBase member)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("member", member);

      if (member is MethodDefinition)
        return ImplementMethodOverride (context, (MethodDefinition) member);
      if (member is PropertyDefinition)
        return ImplementPropertyOverride (context, (PropertyDefinition) member);
      Assertion.IsNotNull (member is EventDefinition, "Only methods, properties, and events can be overridden.");
      return ImplementEventOverride (context, (EventDefinition) member);
    }

    public virtual MutableMethodInfo ImplementMethodOverride (TargetTypeModifierContext context, MethodDefinition method)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("method", method);

      var proxyMethod = context.NextCallProxyGenerator.GetProxyMethodForOverriddenMethod (method);
      var methodOverride = context.ConcreteTarget.GetOrAddOverride (method.MethodInfo);
      methodOverride.SetBody (ctx => _expressionBuilder.CreateInitializingDelegation (ctx, context.ExtensionsField, context.FirstField, proxyMethod));

      return methodOverride;
    }

    public virtual IMutableMember ImplementPropertyOverride (TargetTypeModifierContext context, PropertyDefinition property)
    {
      MutableMethodInfo getMethodOverride = null, setMethodOverride = null;
      if (property.GetMethod != null && property.GetMethod.Overrides.Count > 0)
        getMethodOverride = ImplementMethodOverride (context, property.GetMethod);
      if (property.SetMethod != null && property.SetMethod.Overrides.Count > 0)
        setMethodOverride = ImplementMethodOverride (context, property.SetMethod);

      return context.ConcreteTarget.AddProperty (property.Name, PropertyAttributes.None, getMethodOverride, setMethodOverride);
    }

    public virtual IMutableMember ImplementEventOverride (TargetTypeModifierContext context, EventDefinition @event)
    {
      MutableMethodInfo addMethodOverride = null, removeMethodOverride = null;
      if (@event.AddMethod.Overrides.Count > 0)
        addMethodOverride = ImplementMethodOverride (context, @event.AddMethod);
      if (@event.RemoveMethod.Overrides.Count > 0)
        removeMethodOverride = ImplementMethodOverride (context, @event.RemoveMethod);

      return context.ConcreteTarget.AddEvent (@event.Name, EventAttributes.None, addMethodOverride, removeMethodOverride);
    }

    public void ImplementOverridingMethods (TargetTypeModifierContext context, IList<ConcreteMixinType> concreteMixinTypes)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("concreteMixinTypes", concreteMixinTypes);

      var overriders = context.TargetClassDefinition.GetAllMethods().Where (methodDefinition => methodDefinition.Base != null);
      foreach (var overrider in overriders)
      {
        var mixin = overrider.Base.DeclaringClass as MixinDefinition;
        Assertion.IsNotNull (mixin, "We only support mixins as overriders of target class members.");
        var concreteMixinType = concreteMixinTypes[mixin.MixinIndex];
        Assertion.IsNotNull (concreteMixinType, "If a mixin method is overridden, a concrete type must have been created for it.");

        var methodInOverrideInterface = concreteMixinType.GetOverrideInterfaceMethod (overrider.Base.MethodInfo);

        // It's necessary to explicitly implement some members defined by the concrete mixins' override interfaces: implicit implementation doesn't
        // work if the overrider is non-public or generic. Because it's simpler, we just implement all the members explicitly.
        var methodToCall = overrider.MethodInfo;
        context.ConcreteTarget.AddExplicitOverride (
            methodInOverrideInterface,
            ctx => _expressionBuilder.CreateDelegation (ctx, ctx.This, methodToCall));
      }
    }

    private Expression AddDebuggerInvisibleField (MutableType concreteTarget, string name, Type type, FieldAttributes attributes)
    {
      var field = concreteTarget.AddField (name, attributes, type);
      _attributeGenerator.AddDebuggerBrowsableAttribute (field, DebuggerBrowsableState.Never);

      var instance = field.IsStatic ? null : new ThisExpression (concreteTarget);
      return Expression.Field (instance, field);
    }

    private BinaryExpression InitializeClassContextField (Expression classContextField, ClassContext classContext)
    {
      // __classContext = new ClassContext (...);

      return Expression.Assign (classContextField, _expressionBuilder.CreateNewClassContext (classContext));
    }

    private static BinaryExpression InitializeMixinArrayInitializerField (
        Expression mixinArrayInitializerField, Type targetType, IEnumerable<Type> concreteMixinTypes)
    {
      // __mixinArrayInitializer = new MixinArrayInitializer (targetType, concreteMixinTypes);

      return Expression.Assign (
          mixinArrayInitializerField,
          Expression.New (s_mixinArrayInitializerCtor, Expression.Constant (targetType), Expression.Constant (concreteMixinTypes.ToArray())));
    }

    private Expression ImplementSettingFirstNextCallProxy (ThisExpression @this, Expression firstField, ConstructorInfo nextCallProxyConstructor)
    {
      // __first = <NewNextCallProxy (0)>;

      return Expression.Assign (firstField, NewNextCallProxy (nextCallProxyConstructor, @this, depth: 0));
    }

    private Expression ImplementCreatingMixinInstances (Expression mixinArrayInitializerField, Expression extensionsField)
    {
      // __extensions = __mixinArrayInitializer.CreateMixinArray (MixedObjectInstantiationScope.Current.SuppliedMixinInstances);

      return Expression.Assign (
          extensionsField,
          Expression.Call (
              mixinArrayInitializerField,
              s_createMixinArrayMethod,
              Expression.Property (Expression.Property (null, s_currentMixedObjectInstantiationScopeProperty), s_suppliedMixinInstancesProperty)));
    }

    private Expression ImplementSettingMixinInstances (Expression mixinInstancs, Expression mixinArrayInitializerField, Expression extensionsField)
    {
      // __mixinArrayInitializer.CheckMixinArray (<arguments[0]>);
      // __extensions = <arguments[0]>;

      return Expression.Block (
          Expression.Call (mixinArrayInitializerField, s_checkMixinArrayMethod, mixinInstancs),
          Expression.Assign (extensionsField, mixinInstancs));
    }

    private Expression ImplementInitializingMixins (
        ThisExpression @this, List<Type> mixinTypes, Expression extensionsField, ConstructorInfo nextCallProxyConstructor, Expression deserialization)
    {
      var mixinInitExpressions = new List<Expression>();

      for (int i = 0; i < mixinTypes.Count; i++)
      {
        if (typeof (IInitializableMixin).IsAssignableFrom (mixinTypes[i]))
        {
          // ((IInitializableMixin) __extensions[i]).Initialize (mixinTargetInstance, <NewNextCallProxy (i + 1)>, deserialization);

          var initExpression = Expression.Call (
              Expression.Convert (
                  Expression.ArrayAccess (extensionsField, Expression.Constant (i)),
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
        Expression backingField,
        Expression initialization,
        PropertyInfo interfaceProperty,
        string debuggerDisplayNameString,
        string debuggerDisplayString)
    {
      var name = MemberImplementationUtility.GetNameForExplicitImplementation (interfaceProperty);
      var getMethod = concreteTarget.AddExplicitOverride (interfaceProperty.GetGetMethod(), ctx => Expression.Block (initialization, backingField));
      var property = concreteTarget.AddProperty (name, PropertyAttributes.None, getMethod, setMethod: null);
      _attributeGenerator.AddDebuggerDisplayAttribute (property, debuggerDisplayString, debuggerDisplayNameString);
    }

    private Expression GetIntroducedInterfaceImplementer (Expression extensionsField, InterfaceIntroductionDefinition introduction)
    {
      // ((InterfaceType) __extensions[implementerIndex])

      return Expression.Convert (
          Expression.ArrayAccess (extensionsField, Expression.Constant (introduction.Implementer.MixinIndex)),
          introduction.InterfaceType);
    }

    private string GetIntroducedMemberName (MemberVisibility visibility, MemberInfo interfaceMember)
    {
      return visibility == MemberVisibility.Public
                 ? interfaceMember.Name
                 : MemberImplementationUtility.GetNameForExplicitImplementation (interfaceMember);
    }

    private void ImplementRequiredDuckMethod (MutableType concreteTarget, RequiredMethodDefinition requiredMethod)
    {
      concreteTarget.AddExplicitOverride (
          requiredMethod.InterfaceMethod,
          ctx => Expression.Call (ctx.This, requiredMethod.ImplementingMethod.MethodInfo, ctx.Parameters.Cast<Expression>()));
    }
  }
}