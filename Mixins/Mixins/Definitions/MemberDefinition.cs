using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;

namespace Mixins.Definitions
{
  public abstract class MemberDefinition
  {
    private MemberInfo _memberInfo;
    private ClassDefinition _declaringClass;

    private MemberDefinition _base = null;
    private Dictionary<Type, MemberDefinition> _overrides = new Dictionary<Type, MemberDefinition> ();

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

    public IEnumerable<MemberDefinition> Overrides
    {
      get { return _overrides.Values; }
    }

    public bool HasOverride (Type overrider)
    {
      return _overrides.ContainsKey (overrider);
    }

    public void AddOverride (MemberDefinition overridingMember)
    {
      if (HasOverride (overridingMember.DeclaringClass.Type))
      {
        string message = string.Format ("Type {0} already overrides member {1} with member {2}, cannot add new override {3}.",
          overridingMember.DeclaringClass.FullName, FullName, GetOverride (overridingMember.DeclaringClass.Type).FullName, overridingMember.FullName);
        throw new InvalidOperationException(message);
      }

      if (!CanBeOverriddenBy (overridingMember))
      {
        string message = string.Format ("Member {0} is not a compatible override for {1}.", overridingMember.FullName, FullName);
        throw new ArgumentException (message, "overridingMember");
      }

      Debug.Assert (overridingMember.Base == this);

      _overrides.Add (overridingMember.DeclaringClass.Type, overridingMember);
    }

    public MemberDefinition GetOverride (Type overrider)
    {
      return HasOverride (overrider) ? _overrides[overrider] : null;
    }

    public bool CanBeOverriddenBy (MemberDefinition overrider)
    {
      return MemberType == overrider.MemberType && IsSignatureCompatibleWith (overrider);
    }

    protected abstract bool IsSignatureCompatibleWith (MemberDefinition overrider);
  }
}
