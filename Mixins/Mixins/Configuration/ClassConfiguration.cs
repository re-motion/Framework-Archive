using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mixins.Configuration
{
  public class ClassConfiguration
  {
    private Type _type;
    private Dictionary<MemberInfo, MemberConfiguration> _members = new Dictionary<MemberInfo, MemberConfiguration> ();

    public ClassConfiguration (Type type)
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

    public IEnumerable<MemberConfiguration> Members
    {
      get { return _members.Values; }
    }

    public bool HasMember (MemberInfo member)
    {
      return _members.ContainsKey (member);
    }

    public MemberConfiguration GetMember (MemberInfo member)
    {
      return HasMember (member) ? _members[member] : null;
    }

    public void AddMember (MemberConfiguration newMember)
    {
      if (HasMember (newMember.Member))
      {
        string message = string.Format ("Class {0} already has member {1}.", FullName, newMember.FullName);
        throw new InvalidOperationException (message);
      }
      _members.Add (newMember.Member, newMember);
    }
  }
}
