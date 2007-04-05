using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mixins.Definitions
{
  public abstract class ClassDefinition : IVisitableDefinition
  {
    public readonly DefinitionItemCollection<MemberInfo, MemberDefinition> Members =
        new DefinitionItemCollection<MemberInfo, MemberDefinition> (delegate (MemberDefinition m) { return m.MemberInfo; });
    private Type _type;

    public ClassDefinition (Type type)
    {
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

    public IList<Type> ImplementedInterfaces
    {
      get { return Type.GetInterfaces(); }
    }

    public abstract void Accept (IDefinitionVisitor visitor);
  }
}
