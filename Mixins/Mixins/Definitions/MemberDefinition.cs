using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;

namespace Mixins.Definitions
{
  public abstract class MemberDefinition : IVisitableDefinition
  {
    public readonly DefinitionItemCollection<Type, MemberDefinition> Overrides =
        new DefinitionItemCollection<Type, MemberDefinition> (delegate (MemberDefinition m) { return m.DeclaringClass.Type; });

    private MemberInfo _memberInfo;
    private ClassDefinition _declaringClass;

    private MemberDefinition _base = null;

    public MemberDefinition (MemberInfo memberInfo, ClassDefinition declaringClass)
    {
      _memberInfo = memberInfo;
      _declaringClass = declaringClass;
    }

    public MemberInfo MemberInfo
    {
      get { return _memberInfo; }
    }

    public ClassDefinition DeclaringClass
    {
      get { return _declaringClass; }
    }

    public MemberTypes MemberType
    {
      get { return MemberInfo.MemberType; }
    }

    public bool IsProperty
    {
      get { return MemberInfo.MemberType == MemberTypes.Property; }
    }

    public bool IsMethod
    {
      get { return MemberInfo.MemberType == MemberTypes.Method; }
    }

    public bool IsEvent
    {
      get { return MemberInfo.MemberType == MemberTypes.Event; }
    }

    public string Name
    {
      get { return MemberInfo.Name; }
    }

    public string FullName
    {
      get { return string.Format ("{0}.{1}", DeclaringClass.FullName, Name); }
    }

    public MemberDefinition Base
    {
      get { return _base; }
      set { _base = value; }
    }

    public bool CanBeOverriddenBy (MemberDefinition overrider)
    {
      return MemberType == overrider.MemberType && IsSignatureCompatibleWith (overrider);
    }

    protected abstract bool IsSignatureCompatibleWith (MemberDefinition overrider);

    public abstract void Accept (IDefinitionVisitor visitor);
  }
}
