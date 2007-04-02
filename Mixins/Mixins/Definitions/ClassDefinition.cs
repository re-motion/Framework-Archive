using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mixins.Definitions
{
  public class ClassDefinition
  {
    private Type _type;
    private Dictionary<MemberInfo, MemberDefinition> _members = new Dictionary<MemberInfo, MemberDefinition> ();

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

    public IEnumerable<MemberDefinition> Members
    {
      get { return _members.Values; }
    }

    public bool HasMember (MemberInfo member)
    {
      return _members.ContainsKey (member);
    }

    public MemberDefinition GetMember (MemberInfo member)
    {
      return HasMember (member) ? _members[member] : null;
    }

    public void AddMember (MemberDefinition newMember)
    {
      if (HasMember (newMember.MemberInfo))
      {
        string message = string.Format ("Class {0} already has member {1}.", FullName, newMember.FullName);
        throw new InvalidOperationException (message);
      }
      _members.Add (newMember.MemberInfo, newMember);
    }
  }
}
