using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Rubicon.Mixins.CodeGeneration.DynamicProxy.DPExtensions;

namespace Rubicon.Mixins.CodeGeneration.DynamicProxy
{
  internal class CustomPropertyEmitter : IAttributableEmitter
  {
    private PropertyBuilder builder;
    private MethodEmitter getMethod;
    private MethodEmitter setMethod;

    public CustomPropertyEmitter (
        AbstractTypeEmitter parentTypeEmitter, string name, PropertyAttributes attributes, bool hasThis, Type propertyType, Type[] indexParameters)
    {
      CallingConventions callingConvention = hasThis ? CallingConventions.HasThis : CallingConventions.Standard;
      builder = parentTypeEmitter.TypeBuilder.DefineProperty (name, attributes, callingConvention, propertyType, null, null, indexParameters,
          null, null);
    }

    public MethodEmitter GetMethod
    {
      get { return getMethod; }
      set
      {
        getMethod = value;
        if (getMethod != null)
          builder.SetGetMethod (getMethod.MethodBuilder);
      }
    }

    public MethodEmitter SetMethod
    {
      get { return setMethod; }
      set
      {
        setMethod = value;
        if (setMethod != null)
          builder.SetSetMethod (setMethod.MethodBuilder);
      }
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      builder.SetCustomAttribute (customAttribute);
    }

    public void ImplementPropertyWithField (FieldReference backingField)
    {
      if (GetMethod != null)
      {
        GetMethod.CodeBuilder.AddStatement (new ReturnStatement (backingField));
      }
      if (SetMethod != null)
      {
        SetMethod.CodeBuilder.AddStatement (
            new AssignStatement (backingField, SetMethod.Arguments[0].ToExpression()));
      }
    }
  }
}