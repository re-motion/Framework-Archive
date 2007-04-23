using System;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Definitions
{
  [Serializable]
  public abstract class MemberDefinition : IVisitableDefinition
  {
    private MemberInfo _memberInfo;
    private ClassDefinition _declaringClass;

    private MemberDefinition _base = null;

    public MemberDefinition (MemberInfo memberInfo, ClassDefinition declaringClass)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);
      ArgumentUtility.CheckNotNull ("declaringClass", declaringClass);

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

    public IVisitableDefinition Parent
    {
      get { return DeclaringClass; }
    }

    public MemberDefinition Base
    {
      get { return _base; }
      set { _base = value; }
    }

    public virtual bool CanBeOverriddenBy (MemberDefinition overrider)
    {
      ArgumentUtility.CheckNotNull ("overrider", overrider);
      return MemberType == overrider.MemberType && IsSignatureCompatibleWith (overrider);
    }

    public abstract void AddOverride (MemberDefinition member);

    protected abstract bool IsSignatureCompatibleWith (MemberDefinition overrider);

    public abstract void Accept (IDefinitionVisitor visitor);
  }
}
