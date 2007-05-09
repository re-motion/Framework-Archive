using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters;
using Mixins.CodeGeneration.DynamicProxy.DPExtensions;

namespace Mixins.CodeGeneration.DynamicProxy
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
      set { addMethod = value; }
    }

    public MethodEmitter RemoveMethod
    {
      get { return removeMethod; }
      set { removeMethod = value; }
    }

    public void Generate ()
    {
      if (addMethod == null)
        throw new InvalidOperationException ("Cannot generate an event without add method.");
      if (removeMethod == null)
        throw new InvalidOperationException ("Cannot generate an event without remove method.");

      addMethod.Generate ();
      builder.SetAddOnMethod (addMethod.MethodBuilder);

      removeMethod.Generate ();
      builder.SetRemoveOnMethod (removeMethod.MethodBuilder);
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      builder.SetCustomAttribute (customAttribute);
    }
  }
}