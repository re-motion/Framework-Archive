using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Utilities;
using ReflectionUtility=Rubicon.Mixins.Utilities.ReflectionUtility;

namespace Rubicon.Mixins.Definitions
{
  [Serializable]
  public abstract class ClassDefinition : IVisitableDefinition, IAttributableDefinition
  {
    public readonly DefinitionItemCollection<MethodInfo, MethodDefinition> Methods =
        new DefinitionItemCollection<MethodInfo, MethodDefinition> (delegate (MethodDefinition m) { return m.MethodInfo; });
    public readonly DefinitionItemCollection<PropertyInfo, PropertyDefinition> Properties =
        new DefinitionItemCollection<PropertyInfo, PropertyDefinition> (delegate (PropertyDefinition p) { return p.PropertyInfo; });
    public readonly DefinitionItemCollection<EventInfo, EventDefinition> Events =
        new DefinitionItemCollection<EventInfo, EventDefinition> (delegate (EventDefinition p) { return p.EventInfo; });

    private MultiDefinitionItemCollection<Type, AttributeDefinition> _customAttributes =
        new MultiDefinitionItemCollection<Type, AttributeDefinition> (delegate (AttributeDefinition a) { return a.AttributeType; });

    private Type _type;

    public ClassDefinition (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      if (type.ContainsGenericParameters)
        throw new ArgumentException (string.Format ("The type {0} contains generic parameters, which is not allowed.", type.FullName), "type");
      _type = type;
    }

    public Type Type
    {
      get { return _type; }
    }

    public string Name
    {
      get { return Type.Name; }
    }

    public string FullName
    {
      get {
        if (Type.IsGenericType)
          return Type.GetGenericTypeDefinition ().FullName;
        else
          return Type.FullName;
      }
    }

    public InterfaceMapping GetAdjustedInterfaceMap(Type interfaceType)
    {
      const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

      InterfaceMapping mapping = Type.GetInterfaceMap (interfaceType);
      for (int i = 0; i < mapping.InterfaceMethods.Length; ++i)
      {
        MethodInfo targetMethod = mapping.TargetMethods[i];
        if (!targetMethod.DeclaringType.Equals (Type))
        {
          Type[] types = ReflectionUtility.GetMethodParameterTypes (targetMethod);
          mapping.TargetMethods[i] = targetMethod.DeclaringType.GetMethod (targetMethod.Name, bindingFlags, null, types, null);
        }
      }
      return mapping;
    }

    public abstract IVisitableDefinition Parent { get; }

    public IList<Type> ImplementedInterfaces
    {
      get { return Type.GetInterfaces(); }
    }

    public MultiDefinitionItemCollection<Type, AttributeDefinition> CustomAttributes
    {
      get { return _customAttributes; }
    }

    public IEnumerable<MemberDefinition> GetAllMembers()
    {
      foreach (MethodDefinition method in Methods)
        yield return method;
      foreach (PropertyDefinition property in Properties)
        yield return property;
      foreach (EventDefinition eventDefinition in Events)
        yield return eventDefinition;
    }

    public IEnumerable<MethodDefinition> GetAllMethods ()
    {
      foreach (MethodDefinition method in Methods)
        yield return method;
      foreach (PropertyDefinition property in Properties)
      {
        if (property.GetMethod != null)
          yield return property.GetMethod;
        if (property.SetMethod != null)
          yield return property.SetMethod;
      }
      foreach (EventDefinition eventDef in Events)
      {
        yield return eventDef.AddMethod;
        yield return eventDef.RemoveMethod;
      }
    }

    public abstract void Accept (IDefinitionVisitor visitor);

    protected void AcceptForChildren (IDefinitionVisitor visitor)
    {
      Methods.Accept (visitor);
      Properties.Accept (visitor);
      Events.Accept (visitor);
      CustomAttributes.Accept (visitor);
    }
  }
}
