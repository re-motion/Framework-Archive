using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Definitions
{
  [Serializable]
  [DebuggerDisplay ("{MemberInfo}, DeclaringClass = {DeclaringClass.Type}")]
  public abstract class MemberDefinition : IVisitableDefinition, IAttributableDefinition
  {
    private MemberInfo _memberInfo;
    private ClassDefinition _declaringClass;

    private MultiDefinitionItemCollection<Type, AttributeDefinition> _customAttributes =
        new MultiDefinitionItemCollection<Type, AttributeDefinition> (delegate (AttributeDefinition a) { return a.AttributeType; });

    private IDefinitionItemCollection<Type, MemberDefinition> _internalOverridesWrapper = null;

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
      get { return string.Format ("{0}.{1}", MemberInfo.DeclaringType.FullName, Name); }
    }

    public abstract MemberDefinition BaseAsMember { get; set; }

    public IVisitableDefinition Parent
    {
      get { return DeclaringClass; }
    }

    public MultiDefinitionItemCollection<Type, AttributeDefinition> CustomAttributes
    {
      get { return _customAttributes; }
    }

    public IDefinitionItemCollection<Type, MemberDefinition> Overrides
    {
      get
      {
        if (_internalOverridesWrapper == null)
        {
          _internalOverridesWrapper = GetInternalOverridesWrapper();
        }
        return _internalOverridesWrapper;
      }
    }

    protected abstract IDefinitionItemCollection<Type, MemberDefinition> GetInternalOverridesWrapper();

    public virtual bool CanBeOverriddenBy (MemberDefinition overrider)
    {
      ArgumentUtility.CheckNotNull ("overrider", overrider);
      return MemberType == overrider.MemberType && IsSignatureCompatibleWith (overrider);
    }

    internal abstract void AddOverride (MemberDefinition member);
    public abstract IEnumerable<MemberDefinition> GetOverridesAsMemberDefinitions ();

    protected abstract bool IsSignatureCompatibleWith (MemberDefinition overrider);

    public abstract void Accept (IDefinitionVisitor visitor);

    protected void AcceptForChildren (IDefinitionVisitor visitor)
    {
      _customAttributes.Accept (visitor);
    }
  }
}
