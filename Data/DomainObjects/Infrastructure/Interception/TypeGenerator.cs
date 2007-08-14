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
    private static readonly MethodInfo s_propertyAccessFinished =
        typeof (DomainObject).GetMethod ("PropertyAccessFinished", _infrastructureBindingFlags);

    private readonly CustomClassEmitter _classEmitter;

    public TypeGenerator (Type baseType, ModuleScope scope)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      ArgumentUtility.CheckNotNull ("scope", scope);

      string typeName = baseType.Name + "_WithInterception_ "+ Guid.NewGuid().ToString ("N");
      TypeAttributes flags = TypeAttributes.Public;
      if (baseType.IsAbstract)
        flags |= TypeAttributes.Abstract;

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
          if (IsOverridable (getMethod) && !getMethod.IsAbstract)
          {
            CustomMethodEmitter accessor = OverrideAccessor (getMethod, propertyIdentifier);
            if (accessor != null)
              overrider.GetMethod = accessor;
          }
          if (IsOverridable (setMethod) && !setMethod.IsAbstract)
          {
            CustomMethodEmitter accessor = OverrideAccessor (setMethod, propertyIdentifier);
            if (accessor != null)
              overrider.SetMethod = accessor;
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
      if (accessor.IsVirtual && !accessor.IsFinal)
      {
        CustomMethodEmitter emitter = _classEmitter.CreateMethodOverride (accessor);

        emitter.AddStatement (new ExpressionStatement (
                new MethodInvocationExpression (
                    SelfReference.Self, s_preparePropertyAccessMethod, new ConstReference (propertyIdentifier).ToExpression())));

        MethodInvocationExpression baseCallExpression =
            new MethodInvocationExpression (SelfReference.Self, accessor, emitter.GetArgumentExpressions());

        Statement baseCallStatement;
        LocalReference returnValueLocal = null;
        if (accessor.ReturnType != typeof (void))
        {
          returnValueLocal = emitter.DeclareLocal (accessor.ReturnType);
          baseCallStatement = new AssignStatement (returnValueLocal, baseCallExpression);
        }
        else
          baseCallStatement = new ExpressionStatement (baseCallExpression);

        Statement propertyAccessFinishedStatement = new ExpressionStatement (
            new MethodInvocationExpression (SelfReference.Self, s_propertyAccessFinished));

        emitter.AddStatement (new TryFinallyStatement (new Statement[] { baseCallStatement }, new Statement[] { propertyAccessFinishedStatement }));

        if (accessor.ReturnType != typeof (void))
          emitter.AddStatement (new ReturnStatement (returnValueLocal));
        else
          emitter.AddStatement (new ReturnStatement());

        return emitter;
      }
      else
        return null;
    }
  }
}