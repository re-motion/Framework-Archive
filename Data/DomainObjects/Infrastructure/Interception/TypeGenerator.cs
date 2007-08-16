using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Rubicon.CodeGeneration.DPExtensions;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;
using Rubicon.CodeGeneration;

namespace Rubicon.Data.DomainObjects.Infrastructure.Interception
{
  public class TypeGenerator
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

      // Store analysis results in a list to force analysis before ClassEmitter is created
      List<Tuple<PropertyInfo, string>> properties = new List<Tuple<PropertyInfo, string>> (AnalyzeAndValidateBaseType (baseType));

      string typeName = baseType.FullName + "_WithInterception_ "+ Guid.NewGuid().ToString ("N");
      TypeAttributes flags = TypeAttributes.Public;
      _classEmitter = new CustomClassEmitter (scope, typeName, baseType, Type.EmptyTypes, flags);

      _classEmitter.ReplicateBaseTypeConstructors ();
      OverrideGetPublicDomainObjectType ();
      ProcessProperties (properties);
    }

    public Type BuildType ()
    {
      return _classEmitter.BuildType ();
    }

    private IEnumerable<Tuple<PropertyInfo, string>> AnalyzeAndValidateBaseType (Type baseType)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      Set<MethodInfo> methodsBeingProcessed = new Set<MethodInfo> ();

      ValidateBaseType (baseType);

      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[baseType];
      ValidateClassDefinition (classDefinition, baseType);

      foreach (PropertyInfo property in baseType.GetProperties (_propertyBindingFlags))
      {
        string propertyIdentifier = ReflectionUtility.GetPropertyName (property);
        if (PropertyAccessor.IsValidProperty (classDefinition, propertyIdentifier))
        {
          MethodInfo getMethod = property.GetGetMethod (true);
          MethodInfo setMethod = property.GetSetMethod (true);

          ValidatePropertySetter(setMethod, property, propertyIdentifier, classDefinition);

          if (getMethod != null)
            methodsBeingProcessed.Add (getMethod);
          if (setMethod != null)
            methodsBeingProcessed.Add (setMethod);
          
          yield return new Tuple<PropertyInfo, string> (property, propertyIdentifier);
        }
        else ValidatePropertyNotInMapping(property, propertyIdentifier);
      }

      ValidateRemainingMethods(baseType, methodsBeingProcessed);
    }

    private void ValidateBaseType (Type baseType)
    {
      if (baseType.IsSealed)
        throw new NonInterceptableTypeException (string.Format ("Cannot instantiate type {0} as it is sealed.", baseType.FullName), baseType);
    }

    private void ValidateRemainingMethods (Type baseType, Set<MethodInfo> methodsBeingProcessed)
    {
      foreach (MethodInfo method in baseType.GetMethods (_infrastructureBindingFlags))
      {
        if (method.IsAbstract && !methodsBeingProcessed.Contains (method))
          throw new NonInterceptableTypeException (
              string.Format (
                  "Cannot instantiate type {0} as its member {1} is abstract (and not an automatic property).",
                  baseType.FullName,
                  method.Name),
              baseType);
      }
    }

    private void ValidateClassDefinition (ClassDefinition classDefinition, Type baseType)
    {
      if ( classDefinition == null)
        throw new NonInterceptableTypeException (string.Format ("Cannot instantiate type {0} as it is not part of the mapping.", baseType.FullName),
            baseType);
    }

    private void ValidatePropertyNotInMapping (PropertyInfo property, string propertyIdentifier)
    {
      Type baseType = property.DeclaringType;
      if (IsAbstract (property))
        throw new NonInterceptableTypeException (
            string.Format (
                "Cannot instantiate type {0}, property {1} is abstract but not defined in the mapping (assumed property id: {2}).",
                baseType.FullName,
                property.Name,
                propertyIdentifier),
            baseType);
    }

    private void ValidatePropertySetter (MethodInfo propertySetter, PropertyInfo property, string propertyIdentifier, ClassDefinition classDefinition)
    {
      Type baseType = property.DeclaringType;
      if (propertySetter != null && PropertyAccessor.GetPropertyKind (classDefinition, propertyIdentifier) == PropertyKind.RelatedObjectCollection)
      {
        throw new NonInterceptableTypeException (
            string.Format (
                "Cannot instantiate type {0}, automatic properties for related object collections cannot have setters: property '{1}', property id '{2}'.",
                baseType.FullName,
                property.Name,
                propertyIdentifier),
            baseType);
      }
    }

    private bool IsAbstract (PropertyInfo property)
    {
      MethodInfo getMethod = property.GetGetMethod (true);
      MethodInfo setMethod = property.GetSetMethod (true);
      return (getMethod != null && getMethod.IsAbstract) || (setMethod != null && setMethod.IsAbstract);
    }

    private void OverrideGetPublicDomainObjectType ()
    {
      _classEmitter.CreateMethodOverride (s_getPublicDomainObjectTypeMethod).ImplementByReturning (new TypeTokenExpression (_classEmitter.BaseType));
    }

    private void ProcessProperties (IEnumerable<Tuple<PropertyInfo, string>> properties)
    {
      foreach (Tuple<PropertyInfo, string> property in properties)
        ProcessProperty (property.A, property.B);
    }

    private void ProcessProperty (PropertyInfo property, string propertyIdentifier)
    {
      MethodInfo getMethod = property.GetGetMethod();
      MethodInfo setMethod = property.GetSetMethod();
      if (IsOverridable (getMethod) || IsOverridable (setMethod))
      {
        CustomPropertyEmitter overrider = _classEmitter.CreatePropertyOverride (property);

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
          new MethodInvocationExpression (SelfReference.Self, accessor, emitter.GetArgumentExpressions());

      ImplementWrappedAccessor (emitter, propertyIdentifier, baseCallExpression, accessor.ReturnType);

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
      TypedMethodInvocationExpression getValueMethodCall = new TypedMethodInvocationExpression (
          propertyAccessorReference,
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

      ExpressionReference propertyAccessorReference = CreatePropertyAccessorReference (propertyIdentifier, emitter);
      TypedMethodInvocationExpression setValueMethodCall = new TypedMethodInvocationExpression (
          propertyAccessorReference,
          s_propertySetValueMethod.MakeGenericMethod (propertyType),
          valueArgumentReference.ToExpression());

      ImplementWrappedAccessor (emitter, propertyIdentifier, setValueMethodCall, typeof (void));

      return emitter;
    }

    private ExpressionReference CreatePropertyAccessorReference (string propertyIdentifier, CustomMethodEmitter emitter)
    {
      ExpressionReference propertiesReference = new ExpressionReference (
          typeof (PropertyIndexer),
          new MethodInvocationExpression (SelfReference.Self, s_getPropertiesMethod),
          emitter);
      return new ExpressionReference (
          typeof (PropertyAccessor),
          new TypedMethodInvocationExpression (
              propertiesReference, s_getPropertyAccessorMethod, new ConstReference (propertyIdentifier).ToExpression()),
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
                  SelfReference.Self, s_preparePropertyAccessMethod, new ConstReference (propertyIdentifier).ToExpression())));

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