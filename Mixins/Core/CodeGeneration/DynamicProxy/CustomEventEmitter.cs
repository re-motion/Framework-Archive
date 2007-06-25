using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters;
using Rubicon.Mixins.CodeGeneration.DynamicProxy.DPExtensions;

namespace Rubicon.Mixins.CodeGeneration.DynamicProxy
{
  public class CustomEventEmitter : IAttributableEmitter
  {
    private EventBuilder builder;
    private MethodEmitter addMethod;
    private MethodEmitter removeMethod;

    public CustomEventEmitter (AbstractTypeEmitter parentTypeEmitter, String name, EventAttributes attributes, Type eventType)
    {
      builder = parentTypeEmitter.TypeBuilder.DefineEvent(name, attributes, eventType);
    }

    public MethodEmitter AddMethod
    {
      get { return addMethod; }
      set
      {
        addMethod = value;
        builder.SetAddOnMethod (addMethod.MethodBuilder);
      }
    }

    public MethodEmitter RemoveMethod
    {
      get { return removeMethod; }
      set
      {
        removeMethod = value;
        builder.SetRemoveOnMethod (removeMethod.MethodBuilder);
      }
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      builder.SetCustomAttribute (customAttribute);
    }
  }
}