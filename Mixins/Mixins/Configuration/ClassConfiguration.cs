using System;
using System.Collections.Generic;

namespace Mixins.Configuration
{
  public class ClassConfiguration
  {
    private Type _type;
    private List<MemberConfiguration> _members = new List<MemberConfiguration> ();

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

    public IEnumerable<MemberConfiguration> Members
    {
      get { return _members; }
    }

    public void AddMember (MemberConfiguration newMember)
    {
      _members.Add (newMember);
    }
  }
}
