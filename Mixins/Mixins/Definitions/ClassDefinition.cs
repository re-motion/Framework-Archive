using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Definitions
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
      get { return Type.FullName; }
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
