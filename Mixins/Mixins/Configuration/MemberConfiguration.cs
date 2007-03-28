using System;
using System.Reflection;

namespace Mixins.Configuration
{
  public class MemberConfiguration
  {
    private MemberInfo _memberInfo;
    private ClassConfiguration _parentClass;

    public MemberConfiguration (MemberInfo memberInfo, ClassConfiguration parentClass)
    {
      _memberInfo = memberInfo;
      _parentClass = parentClass;
    }

    public MemberInfo Member
    {
      get { return _memberInfo; }
    }

    public ClassConfiguration ParentClass
    {
      get { return _parentClass; }
    }

    public bool IsProperty
    {
      get { return Member.MemberType == MemberTypes.Property; }
    }

    public bool IsMethod
    {
      get { return Member.MemberType == MemberTypes.Method; }
    }

    public bool IsEvent
    {
      get { return Member.MemberType == MemberTypes.Event; }
    }

    public string Name
    {
      get { return Member.Name; }
    }

    public string FullName
    {
      get { return string.Format ("{0}.{1}", ParentClass.FullName, Name); }
    }
  }
}
