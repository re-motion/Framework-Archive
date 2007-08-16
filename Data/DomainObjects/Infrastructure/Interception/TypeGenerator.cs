using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Rubicon.CodeGeneration.DPExtensions;
using Rubicon.Utilities;
using Rubicon.CodeGeneration;

namespace Rubicon.Data.DomainObjects.Infrastructure.Interception
{
  internal class TypeGenerator
  {
    private const BindingFlags _propertyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    private const BindingFlags _infrastructureBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly MethodInfo s_getPublicDomainObjectTypeMethod =
        typeof (DomainObject).GetMethod ("GetPublicDomainObjectType", _infrastructureBindingFlags);
    private static readonly MethodInfo s_preparePropertyAccessMethod =
        typeof (DomainObject).GetMethod ("PreparePropertyAccess", _infrastructureBindingFlags);
    private static readonly MethodInfo s_propertyAccessFinishedMethod =
        typeof (DomainObject).GetMethod ("PropertyAccessFinished", _infrastructureBindingFlags);
    private static readonly MethodInfo s_getPropertiesMethod =
        typeof (DomainObject).GetMethod ("get_Properties", _infrastructureBindingFlags);
    private static readonly MethodInfo s_getPropertyAccessorMethod =
        typeof (PropertyIndexer).GetMethod ("get_Item", _infrastructureBindingFlags, null, new Type[] {typeof (string)}, null);
    private static readonly MethodInfo s_propertyGetValueMethod =
        typeof (PropertyAccessor).GetMethod ("GetValue", _infrastructureBindingFlags);
    private static readonly MethodInfo s_propertySetValueMethod =
        typeof (PropertyAccessor).GetMethod ("SetValue", _infrastructureBindingFlags);

    private readonly CustomClassEmitter _classEmitter;

    static TypeGenerator ()
    {
      Assertion.IsNotNull (s_getPublicDomainObjectTypeMethod);
      Assertion.IsNotNull (s_preparePropertyAccessMethod);
      Assertion.IsNotNull (s_propertyAccessFinishedMethod);
      Assertion.IsNotNull (s_getPropertiesMethod);
      Assertion.IsNotNull (s_getPropertyAccessorMethod);
      Assertion.IsNotNull (s_propertyGetValueMethod);
      Assertion.IsNotNull (s_propertySetValueMethod);
    }

    public TypeGenerator (Type baseType, ModuleScope scope)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      ArgumentUtility.CheckNotNull ("scope", scope);

      string typeName = baseType.Name + "_WithInterception_ "+ Guid.NewGuid().ToString ("N");
      TypeAttributes flags = TypeAttributes.Public;
      _classEmitter = new CustomClassEmitter (scope, typeName, baseType, Type.EmptyTypes, flags);

      OverrideGetPublicDomainObjectType ();
      ProcessProperties ();
    }

    public Type BuildType ()
    {
      return _classEmitter.BuildType ();
    }

    private void OverrideGetPublicDomainObjectType ()
    {
      _classEmitter.CreateMethodOverride (s_getPublicDomainObjectTypeMethod).ImplementByReturning (new TypeTokenExpression (_classEmitter.BaseType));
    }

    private void ProcessProperties ()
    {
      foreach (PropertyInfo property in _classEmitter.BaseType.GetProperties (_propertyBindingFlags))
      {
        MethodInfo getMethod = property.GetGetMethod ();
        MethodInfo setMethod = property.GetSetMethod ();
        if (IsOverridable (getMethod) || IsOverridable (setMethod))
        {
          CustomPropertyEmitter overrider = _classEmitter.CreatePropertyOverride (property);
          string propertyIdentifier = ReflectionUtility.GetPropertyName (property);

          if (getMethod != null)
          {
            if (getMethod.IsAbstract)
              overrider.GetMethod = ImplementAbstractGetAccessor (getMethod, propertyIdentifier);
            else if (IsOverridable (getMethod))
              overrider.GetMethod = OverrideAccessor (getMethod, propertyIdentifier);
          }

          if (setMethod != null)
          {
            if (setMethod.IsAbstract)
              overrider.SetMethod = ImplementAbstractSetAccessor (setMethod, propertyIdentifier, property.PropertyType);
            else if (IsOverridable (setMethod))
              overrider.SetMethod = OverrideAccessor (setMethod, propertyIdentifier);
          }
        }
      }
    }

    private bool IsOverridable (MethodInfo method)
    {
      return method != null && method.IsVirtual && !method.IsFinal;
    }

    private CustomMethodEmitter OverrideAccessor (MethodInfo accessor, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("accessor", accessor);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);

      Assertion.IsFalse (accessor.IsAbstract);
      Assertion.IsTrue (IsOverridable (accessor));

      CustomMethodEmitter emitter = _classEmitter.CreateMethodOverride (accessor);
      MethodInvocationExpression baseCallExpression =
          new MethodInvocationExpression (SelfReference.Self, accessor, emitter.GetArgumentExpressions ());

      ImplementWrappedAccessor(emitter, propertyIdentifier, baseCallExpression, accessor.ReturnType);

      return emitter;
    }

    private CustomMethodEmitter ImplementAbstractGetAccessor (MethodInfo accessor, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("accessor", accessor);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);

      Assertion.IsTrue (accessor.IsAbstract);
      Assertion.IsTrue (accessor.ReturnType != typeof (void));

      CustomMethodEmitter emitter = _classEmitter.CreateMethodOverride (accessor);

      ExpressionReference propertyAccessorReference = CreatePropertyAccessorReference (propertyIdentifier, emitter);
      TypedMethodInvocationExpression getValueMethodCall = new TypedMethodInvocationExpression (propertyAccessorReference,
          s_propertyGetValueMethod.MakeGenericMethod (accessor.ReturnType));

      ImplementWrappedAccessor (emitter, propertyIdentifier, getValueMethodCall, accessor.ReturnType);

      return emitter;
    }

    private CustomMethodEmitter ImplementAbstractSetAccessor (MethodInfo accessor, string propertyIdentifier, Type propertyType)
    {
      ArgumentUtility.CheckNotNull ("accessor", accessor);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);

      Assertion.IsTrue (accessor.IsAbstract);
      Assertion.IsTrue (accessor.ReturnType == typeof (void));

      CustomMethodEmitter emitter = _classEmitter.CreateMethodOverride (accessor);

      Assertion.IsTrue (emitter.ArgumentReferences.Length > 0);
      Reference valueArgumentReference = emitter.ArgumentReferences[emitter.ArgumentReferences.Length - 1];

      ExpressionReference propertyAccessorReference = CreatePropertyAccessorReference(propertyIdentifier, emitter);
      TypedMethodInvocationExpression setValueMethodCall = new TypedMethodInvocationExpression (propertyAccessorReference,
          s_propertySetValueMethod.MakeGenericMethod (propertyType), valueArgumentReference.ToExpression());

      ImplementWrappedAccessor (emitter, propertyIdentifier, setValueMethodCall, typeof (void));

      return emitter;
    }

    private ExpressionReference CreatePropertyAccessorReference (string propertyIdentifier, CustomMethodEmitter emitter)
    {
      ExpressionReference propertiesReference = new ExpressionReference (typeof (PropertyIndexer),
          new MethodInvocationExpression (SelfReference.Self, s_getPropertiesMethod),
          emitter);
      return new ExpressionReference (typeof (PropertyAccessor),
          new TypedMethodInvocationExpression (propertiesReference, s_getPropertyAccessorMethod, new ConstReference (propertyIdentifier).ToExpression ()),
          emitter);
    }


    private void ImplementWrappedAccessor (CustomMethodEmitter emitter, string propertyIdentifier, Expression implementation, Type returnType)
    {
      ArgumentUtility.CheckNotNull ("emitter", emitter);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);
      ArgumentUtility.CheckNotNull ("implementation", implementation);
      ArgumentUtility.CheckNotNull ("returnType", returnType);

      emitter.AddStatement (
          new ExpressionStatement (
              new MethodInvocationExpression (
                  SelfReference.Self, s_preparePropertyAccessMethod, new ConstReference (propertyIdentifier).ToExpression ())));

      Statement baseCallStatement;
      LocalReference returnValueLocal = null;
      if (returnType != typeof (void))
      {
        returnValueLocal = emitter.DeclareLocal (returnType);
        baseCallStatement = new AssignStatement (returnValueLocal, implementation);
      }
      else
        baseCallStatement = new ExpressionStatement (implementation);

      Statement propertyAccessFinishedStatement = new ExpressionStatement (
          new MethodInvocationExpression (SelfReference.Self, s_propertyAccessFinishedMethod));

      emitter.AddStatement (new TryFinallyStatement (new Statement[] {baseCallStatement}, new Statement[] {propertyAccessFinishedStatement}));

      if (returnType != typeof (void))
        emitter.AddStatement (new ReturnStatement (returnValueLocal));
      else
        emitter.AddStatement (new ReturnStatement());
    }
  }
}