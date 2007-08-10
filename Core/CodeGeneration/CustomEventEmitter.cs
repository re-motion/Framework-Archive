using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Rubicon.CodeGeneration
{
  public class CustomEventEmitter : IAttributableEmitter
  {
    public readonly CustomClassEmitter DeclaringType;
    public readonly EventBuilder EventBuilder;

    private readonly string _name;
    private readonly Type _eventType;

    private CustomMethodEmitter _addMethod;
    private CustomMethodEmitter _removeMethod;

    public CustomEventEmitter (CustomClassEmitter declaringType, string name, EventAttributes attributes, Type eventType)
    {
      DeclaringType = declaringType;
      EventBuilder = declaringType.TypeBuilder.DefineEvent (name, attributes, eventType);
      _name = name;
      _eventType = eventType;
    }

    public CustomMethodEmitter AddMethod
    {
      get { return _addMethod; }
      set
      {
        if (value == null)
          throw new ArgumentException ("Due to limitations in Reflection.Emit, event accessors cannot be set to null.", "value");

        _addMethod = value;
        EventBuilder.SetAddOnMethod (_addMethod.MethodBuilder);
      }
    }

    public CustomMethodEmitter RemoveMethod
    {
      get { return _removeMethod; }
      set
      {
        if (value == null)
          throw new ArgumentException ("Due to limitations in Reflection.Emit, event accessors cannot be set to null.", "value");

        _removeMethod = value;
        EventBuilder.SetRemoveOnMethod (_removeMethod.MethodBuilder);
      }
    }

    public string Name
    {
      get { return _name; }
    }

    public Type EventType
    {
      get { return _eventType; }
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      EventBuilder.SetCustomAttribute (customAttribute);
    }

    public CustomMethodEmitter CreateAddMethod ()
    {
      if (AddMethod != null)
        throw new InvalidOperationException ("Thís event already has an add-on method");
      else
      {
        CustomMethodEmitter method =
            DeclaringType.CreateMethod ("add_" + Name, MethodAttributes.Public | MethodAttributes.SpecialName);
        method.SetParameterTypes (new Type[] { EventType });
        AddMethod = method;
        return method;
      }
    }

    public CustomMethodEmitter CreateRemoveMethod ()
    {
      if (RemoveMethod != null)
        throw new InvalidOperationException ("Thís event already has an remove-on method");
      else
      {
        CustomMethodEmitter method =
            DeclaringType.CreateMethod ("remove_" + Name, MethodAttributes.Public | MethodAttributes.SpecialName);
        method.SetParameterTypes (new Type[] { EventType });
        RemoveMethod = method;
        return method;
      }
    }
  }
}