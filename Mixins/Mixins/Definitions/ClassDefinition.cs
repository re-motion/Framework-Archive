using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mixins.Definitions
{
  public class ClassDefinition
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

    public IEnumerable<Type> ImplementedInterfaces
    {
      get { return Type.GetInterfaces(); }
    }
  }
}
