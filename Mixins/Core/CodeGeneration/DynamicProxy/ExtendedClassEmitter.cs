using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Rubicon.Mixins.CodeGeneration.DynamicProxy.DPExtensions;
using Rubicon;
using Rubicon.Utilities;
using ReflectionUtility=Rubicon.Mixins.Utilities.ReflectionUtility;

namespace Rubicon.Mixins.CodeGeneration.DynamicProxy
{
  internal class ExtendedClassEmitter : IAttributableEmitter
  {
    private AbstractTypeEmitter _innerEmitter;

    public ExtendedClassEmitter (AbstractTypeEmitter innerEmitter)
    {
      _innerEmitter = innerEmitter;
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

    public CustomMethodEmitter CreateMethodOverride (MethodInfo baseMethod)
    {
      return CreateMethodOverrideOrInterfaceImplementation (baseMethod, true);
    }

    public CustomMethodEmitter CreateInterfaceMethodImplementation (MethodInfo interfaceMethod)
    {
      return CreateMethodOverrideOrInterfaceImplementation (interfaceMethod, false);
    }

    private CustomMethodEmitter CreateMethodOverrideOrInterfaceImplementation (MethodInfo baseOrInterfaceMethod, bool keepNameAndVisibility)
    {
      ArgumentUtility.CheckNotNull ("baseOrInterfaceMethod", baseOrInterfaceMethod);
      Assertion.IsTrue (baseOrInterfaceMethod.IsVirtual);

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
      CustomMethodEmitter methodDefinition = new CustomMethodEmitter (InnerEmitter, methodName, methodDefinitionAttributes);
      methodDefinition.CopyParametersAndReturnTypeFrom (baseOrInterfaceMethod);

      TypeBuilder.DefineMethodOverride (methodDefinition.MethodBuilder, baseOrInterfaceMethod);

      return methodDefinition;
    }

    // does not create the property's methods
    public CustomPropertyEmitter CreatePropertyOverride (PropertyInfo baseProperty)
    {
      return CreatePropertyOverrideOrInterfaceImplementation (baseProperty, true);
    }

    // does not create the property's methods
    public CustomPropertyEmitter CreateInterfacePropertyImplementation (PropertyInfo interfaceProperty)
    {
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

      CustomPropertyEmitter newProperty = new CustomPropertyEmitter (InnerEmitter, propertyName, PropertyAttributes.None,
          true, baseOrInterfaceProperty.PropertyType, indexParameterTypes);

      return newProperty;
    }

    // does not create the event's methods
    public CustomEventEmitter CreateEventOverride (EventInfo baseEvent)
    {
      return CreateEventOverrideOrInterfaceImplementation (baseEvent, true);
    }

    // does not create the event's methods
    public CustomEventEmitter CreateInterfaceEventImplementation (EventInfo interfaceEvent)
    {
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
      CustomEventEmitter newEvent = new CustomEventEmitter (InnerEmitter, eventName, EventAttributes.None, baseOrInterfaceEvent.EventHandlerType);
      return newEvent;
    }

    public LocalReference AddMakeReferenceOfExpressionStatements (CustomMethodEmitter methodDefinition, Type referenceType, Expression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      LocalReference reference = methodDefinition.InnerEmitter.CodeBuilder.DeclareLocal (referenceType);
      methodDefinition.InnerEmitter.CodeBuilder.AddStatement (new AssignStatement (reference, expression));
      return reference;
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      TypeBuilder.SetCustomAttribute (customAttribute);
    }

    public void ReplicateBaseTypeConstructors (params Statement[] postBaseCallInitializationStatements)
    {
      ConstructorInfo[] constructors = BaseType.GetConstructors (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      foreach (ConstructorInfo constructor in constructors)
      {
        if (ReflectionUtility.IsPublicOrProtected (constructor))
        {
          ReplicateBaseTypeConstructor (constructor, postBaseCallInitializationStatements);
        }
      }
    }

    private void ReplicateBaseTypeConstructor (ConstructorInfo constructor, params Statement[] postBaseCallInitializationStatements)
    {
      ArgumentUtility.CheckNotNull ("constructor", constructor);
      ArgumentReference[] arguments = ArgumentsUtil.ConvertToArgumentReference (constructor.GetParameters ());
      ConstructorEmitter newConstructor = InnerEmitter.CreateConstructor (arguments);

      Expression[] argumentExpressions = ArgumentsUtil.ConvertArgumentReferenceToExpression (arguments);
      newConstructor.CodeBuilder.AddStatement (new ConstructorInvocationStatement (constructor, argumentExpressions));

      foreach (Statement statement in postBaseCallInitializationStatements)
        newConstructor.CodeBuilder.AddStatement (statement);

      newConstructor.CodeBuilder.AddStatement (new ReturnStatement ());
    }

    public void ImplementGetObjectDataByDelegation(Func<MethodEmitter, bool, MethodInvocationExpression> delegatingMethodInvocationGetter)
    {
      ArgumentUtility.CheckNotNull ("delegatingMethodInvocationGetter", delegatingMethodInvocationGetter);

      bool baseIsISerializable = typeof (ISerializable).IsAssignableFrom (BaseType);

      MethodInfo getObjectDataMethod =
          typeof (ISerializable).GetMethod ("GetObjectData", new Type[] {typeof (SerializationInfo), typeof (StreamingContext)});
      MethodEmitter newMethod = CreateInterfaceMethodImplementation (getObjectDataMethod).InnerEmitter;

      MethodInvocationExpression delegatingMethodInvocation = delegatingMethodInvocationGetter(newMethod, baseIsISerializable);
      if (delegatingMethodInvocation != null)
        newMethod.CodeBuilder.AddStatement (new ExpressionStatement (delegatingMethodInvocation));

      if (baseIsISerializable)
      {
        ConstructorInfo baseConstructor = BaseType.GetConstructor (
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            CallingConventions.Any,
            new Type[] {typeof (SerializationInfo), typeof (StreamingContext)},
            null);
        if (baseConstructor == null || !ReflectionUtility.IsPublicOrProtected (baseConstructor))
        {
          string message = string.Format (
              "No public or protected deserialization constructor in type {0} - this is not supported.",
              BaseType.FullName);
          throw new NotSupportedException (message);
        }

        MethodInfo baseGetObjectDataMethod =
            BaseType.GetMethod ("GetObjectData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (baseGetObjectDataMethod == null || !ReflectionUtility.IsPublicOrProtected (baseGetObjectDataMethod))
        {
          string message = string.Format ("No public or protected GetObjectData in {0} - this is not supported.", BaseType.FullName);
          throw new NotSupportedException (message);
        }
        newMethod.CodeBuilder.AddStatement (
            new ExpressionStatement (
                new MethodInvocationExpression (
                    SelfReference.Self,
                    baseGetObjectDataMethod,
                    new ReferenceExpression (newMethod.Arguments[0]),
                    new ReferenceExpression (newMethod.Arguments[1]))));
      }
      newMethod.CodeBuilder.AddStatement (new ReturnStatement());
    }

    public LocalReference LoadCustomAttribute (AbstractCodeBuilder codeBuilder, Type attributeType, int index)
    {
      MethodInfo getCustomAttributesMethod = typeof (Type).GetMethod ("GetCustomAttributes", new Type[] { typeof (Type), typeof (bool) }, null);
      Assertion.IsNotNull (getCustomAttributesMethod);

      LocalReference typeLocal = codeBuilder.DeclareLocal (typeof (Type));
      codeBuilder.AddStatement (new AssignStatement (typeLocal, new TypeTokenExpression (InnerEmitter.TypeBuilder)));

      Expression getAttributesExpression = new CastClassExpression (attributeType.MakeArrayType(),
        new VirtualMethodInvocationExpression (typeLocal,
        getCustomAttributesMethod,
        new TypeTokenExpression (attributeType),
        new ConstReference (false).ToExpression()));

      LocalReference attributesLocal = codeBuilder.DeclareLocal (attributeType.MakeArrayType());
      codeBuilder.AddStatement (new AssignStatement (attributesLocal, getAttributesExpression));

      LocalReference singleAttributeLocal = codeBuilder.DeclareLocal (attributeType);
      codeBuilder.AddStatement (new AssignStatement (singleAttributeLocal,
          new LoadArrayElementExpression (new ConstReference (index), attributesLocal, attributeType)));
      return singleAttributeLocal;
    }
  }
}
