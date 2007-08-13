using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Rubicon.CodeGeneration.DPExtensions;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Rubicon.CodeGeneration
{
  public class CustomClassEmitter : IAttributableEmitter
  {
    private readonly AbstractTypeEmitter _innerEmitter;
    private readonly Cache<MethodInfo, CustomMethodEmitter> _publicMethodWrappers = new Cache<MethodInfo, CustomMethodEmitter> ();
    private bool _hasBeenBuilt = false;

    public CustomClassEmitter (AbstractTypeEmitter innerEmitter)
    {
      ArgumentUtility.CheckNotNull ("innerEmitter", innerEmitter);
      _innerEmitter = innerEmitter;
    }

    public CustomClassEmitter (ModuleScope scope, string name, Type baseType)
      : this (scope, name, baseType, Type.EmptyTypes, TypeAttributes.Class | TypeAttributes.Public)
    {
    }

    public CustomClassEmitter (ModuleScope scope, string name, Type baseType, Type[] interfaces, TypeAttributes flags)
        : this (
            new ClassEmitterSupportingOpenGenericBaseType (
                ArgumentUtility.CheckNotNull ("scope", scope),
                ArgumentUtility.CheckNotNullOrEmpty ("name", name),
                CheckBaseType (baseType),
                CheckInterfaces (interfaces),
                flags))
    {
    }

    private static Type CheckBaseType (Type baseType)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      if (baseType.IsInterface)
        throw new ArgumentException ("Base type must not be an interface (" + baseType.FullName + ").", "baseType");
      if (baseType.IsSealed)
        throw new ArgumentException ("Base type must not be sealed (" + baseType.FullName + ").", "baseType");
      return baseType;
    }

    private static Type[] CheckInterfaces (Type[] interfaces)
    {
      ArgumentUtility.CheckNotNull ("interfaces", interfaces);
      foreach (Type interfaceType in interfaces)
      {
        if (!interfaceType.IsInterface)
          throw new ArgumentException ("Interface type must not be a class or value type (" + interfaceType.FullName + ").", "interfaces");
      }
      return interfaces;
    }

    public AbstractTypeEmitter InnerEmitter
    {
      get { return _innerEmitter; }
    }

    public Type BaseType
    {
      get { return TypeBuilder.BaseType; }
    }

    public TypeBuilder TypeBuilder
    {
      get { return InnerEmitter.TypeBuilder; }
    }

    public bool HasBeenBuilt
    {
      get { return _hasBeenBuilt; }
    }

    public ConstructorEmitter CreateConstructor (ArgumentReference[] arguments)
    {
      ArgumentUtility.CheckNotNull ("arguments", arguments);

      return InnerEmitter.CreateConstructor (arguments);
    }

    public ConstructorEmitter CreateConstructor (Type[] arguments)
    {
      ArgumentUtility.CheckNotNull ("arguments", arguments);

      ArgumentReference[] argumentReferences = ArgumentsUtil.ConvertToArgumentReference (arguments);
      return CreateConstructor (argumentReferences);
    }

    public void CreateDefaultConstructor ()
    {
      InnerEmitter.CreateDefaultConstructor ();
    }

    public ConstructorEmitter CreateTypeConstructor ()
    {
      return InnerEmitter.CreateTypeConstructor ();
    }

    public FieldReference CreateField (string name, Type fieldType)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNull ("fieldType", fieldType);

      return InnerEmitter.CreateField (name, fieldType);
    }

    public FieldReference CreateStaticField (string name, Type fieldType)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNull ("fieldType", fieldType);

      return InnerEmitter.CreateStaticField (name, fieldType);
    }

    public CustomMethodEmitter CreateMethod (string name, MethodAttributes attributes)
    {
      ArgumentUtility.CheckNotNull ("name", name);

      return new CustomMethodEmitter (this, name, attributes);
    }

    public CustomPropertyEmitter CreateProperty (string name, PropertyKind propertyKind, Type propertyType)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);

      return new CustomPropertyEmitter (this, name, propertyKind, propertyType, Type.EmptyTypes, PropertyAttributes.None);
    }

    public CustomPropertyEmitter CreateProperty (
        string name, PropertyKind propertyKind, Type propertyType, Type[] indexParameters, PropertyAttributes attributes)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);
      ArgumentUtility.CheckNotNull ("indexParameters", indexParameters);

      return new CustomPropertyEmitter (this, name, propertyKind, propertyType, indexParameters, attributes);
    }

    public CustomEventEmitter CreateEvent (string name, Type eventType, EventAttributes attributes)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNull ("eventType", eventType);

      return new CustomEventEmitter (this, name, eventType, attributes);
    }

    public CustomMethodEmitter CreateMethodOverride (MethodInfo baseMethod)
    {
      ArgumentUtility.CheckNotNull ("baseMethod", baseMethod);
      return CreateMethodOverrideOrInterfaceImplementation (baseMethod, true);
    }

    public CustomMethodEmitter CreateInterfaceMethodImplementation (MethodInfo interfaceMethod)
    {
      ArgumentUtility.CheckNotNull ("interfaceMethod", interfaceMethod);
      return CreateMethodOverrideOrInterfaceImplementation (interfaceMethod, false);
    }

    private CustomMethodEmitter CreateMethodOverrideOrInterfaceImplementation (MethodInfo baseOrInterfaceMethod, bool keepNameAndVisibility)
    {
      ArgumentUtility.CheckNotNull ("baseOrInterfaceMethod", baseOrInterfaceMethod);

      MethodAttributes methodDefinitionAttributes = MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final;
      if (keepNameAndVisibility)
        methodDefinitionAttributes |= (baseOrInterfaceMethod.Attributes & MethodAttributes.MemberAccessMask) | MethodAttributes.ReuseSlot;
      else
        methodDefinitionAttributes |= MethodAttributes.Private | MethodAttributes.NewSlot;

      if (baseOrInterfaceMethod.IsSpecialName)
      {
        methodDefinitionAttributes |= MethodAttributes.SpecialName;
      }

      string methodName;
      if (keepNameAndVisibility)
        methodName = baseOrInterfaceMethod.Name;
      else
        methodName = string.Format ("{0}.{1}", baseOrInterfaceMethod.DeclaringType.FullName, baseOrInterfaceMethod.Name);
      CustomMethodEmitter methodDefinition = CreateMethod (methodName, methodDefinitionAttributes);
      methodDefinition.CopyParametersAndReturnType (baseOrInterfaceMethod);

      TypeBuilder.DefineMethodOverride (methodDefinition.MethodBuilder, baseOrInterfaceMethod);

      return methodDefinition;
    }

    // does not create the property's methods
    public CustomPropertyEmitter CreatePropertyOverride (PropertyInfo baseProperty)
    {
      ArgumentUtility.CheckNotNull ("baseProperty", baseProperty);
      return CreatePropertyOverrideOrInterfaceImplementation (baseProperty, true);
    }

    // does not create the property's methods
    public CustomPropertyEmitter CreateInterfacePropertyImplementation (PropertyInfo interfaceProperty)
    {
      ArgumentUtility.CheckNotNull ("interfaceProperty", interfaceProperty);
      return CreatePropertyOverrideOrInterfaceImplementation (interfaceProperty, false);
    }

    // does not create the property's methods
    private CustomPropertyEmitter CreatePropertyOverrideOrInterfaceImplementation (PropertyInfo baseOrInterfaceProperty, bool keepName)
    {
      ArgumentUtility.CheckNotNull ("baseOrInterfaceProperty", baseOrInterfaceProperty);

      string propertyName;
      if (keepName)
        propertyName = baseOrInterfaceProperty.Name;
      else 
        propertyName = string.Format ("{0}.{1}", baseOrInterfaceProperty.DeclaringType.FullName, baseOrInterfaceProperty.Name);
      Type[] indexParameterTypes =
          Array.ConvertAll<ParameterInfo, Type> (baseOrInterfaceProperty.GetIndexParameters(), delegate (ParameterInfo p) { return p.ParameterType; });

      CustomPropertyEmitter newProperty = CreateProperty (propertyName, PropertyKind.Instance, baseOrInterfaceProperty.PropertyType,
          indexParameterTypes, PropertyAttributes.None);

      return newProperty;
    }

    // does not create the event's methods
    public CustomEventEmitter CreateEventOverride (EventInfo baseEvent)
    {
      ArgumentUtility.CheckNotNull ("baseEvent", baseEvent);
      return CreateEventOverrideOrInterfaceImplementation (baseEvent, true);
    }

    // does not create the event's methods
    public CustomEventEmitter CreateInterfaceEventImplementation (EventInfo interfaceEvent)
    {
      ArgumentUtility.CheckNotNull ("interfaceEvent", interfaceEvent);
      return CreateEventOverrideOrInterfaceImplementation (interfaceEvent, false);
    }

    // does not create the event's methods
    private CustomEventEmitter CreateEventOverrideOrInterfaceImplementation (EventInfo baseOrInterfaceEvent, bool keepName)
    {
      ArgumentUtility.CheckNotNull ("baseOrInterfaceEvent", baseOrInterfaceEvent);

      string eventName;
      if (keepName)
        eventName = baseOrInterfaceEvent.Name;
      else
        eventName = string.Format ("{0}.{1}", baseOrInterfaceEvent.DeclaringType.FullName, baseOrInterfaceEvent.Name);
      CustomEventEmitter newEvent = CreateEvent (eventName, baseOrInterfaceEvent.EventHandlerType, EventAttributes.None);
      return newEvent;
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      ArgumentUtility.CheckNotNull ("customAttribute", customAttribute);
      TypeBuilder.SetCustomAttribute (customAttribute);
    }

    private static bool IsPublicOrProtected (MethodBase member)
    {
      return member.IsPublic || member.IsFamily || member.IsFamilyOrAssembly;
    }

    public void ReplicateBaseTypeConstructors (params Statement[] postBaseCallInitializationStatements)
    {
      ArgumentUtility.CheckNotNull ("postBaseCallInitializationStatements", postBaseCallInitializationStatements);

      ConstructorInfo[] constructors = BaseType.GetConstructors (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      foreach (ConstructorInfo constructor in constructors)
      {
        if (IsPublicOrProtected (constructor))
          ReplicateBaseTypeConstructor (constructor, postBaseCallInitializationStatements);
      }
    }

    private void ReplicateBaseTypeConstructor (ConstructorInfo constructor, params Statement[] postBaseCallInitializationStatements)
    {
      ArgumentUtility.CheckNotNull ("constructor", constructor);
      ArgumentUtility.CheckNotNull ("postBaseCallInitializationStatements", postBaseCallInitializationStatements);

      ArgumentReference[] arguments = ArgumentsUtil.ConvertToArgumentReference (constructor.GetParameters());
      ConstructorEmitter newConstructor = InnerEmitter.CreateConstructor (arguments);

      Expression[] argumentExpressions = ArgumentsUtil.ConvertArgumentReferenceToExpression (arguments);
      newConstructor.CodeBuilder.AddStatement (new ConstructorInvocationStatement (constructor, argumentExpressions));

      foreach (Statement statement in postBaseCallInitializationStatements)
        newConstructor.CodeBuilder.AddStatement (statement);

      newConstructor.CodeBuilder.AddStatement (new ReturnStatement());
    }

    public CustomMethodEmitter GetPublicMethodWrapper (MethodInfo methodToBeWrapped)
    {
      ArgumentUtility.CheckNotNull ("methodToBeWrapped", methodToBeWrapped);

      return _publicMethodWrappers.GetOrCreateValue (
          methodToBeWrapped,
          delegate (MethodInfo method)
          {
            MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig;
            CustomMethodEmitter wrapper = CreateMethod ("__wrap__" + method.Name, attributes);
            wrapper.CopyParametersAndReturnType (method);
            wrapper.ImplementByDelegating (SelfReference.Self, method);
            return wrapper;
          });
    }

    public Type BuildType ()
    {
      _hasBeenBuilt = true;
      return InnerEmitter.BuildType();
    }
  }
}