using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Definitions
{
  [Serializable]
  public class PropertyDefinition : MemberDefinition, IVisitableDefinition
  {
    private static SignatureChecker s_signatureChecker = new SignatureChecker();

    public readonly DefinitionItemCollection<Type, PropertyDefinition> Overrides =
        new DefinitionItemCollection<Type, PropertyDefinition> (delegate (PropertyDefinition m) { return m.DeclaringClass.Type; });

    private PropertyDefinition _base;
    private MethodDefinition _getMethod;
    private MethodDefinition _setMethod;

    public PropertyDefinition (PropertyInfo memberInfo, ClassDefinition declaringClass, MethodDefinition getMethod, MethodDefinition setMethod)
        : base (memberInfo, declaringClass)
    {
      _getMethod = getMethod;
      _setMethod = setMethod;
    }

    public PropertyInfo PropertyInfo
    {
      get { return (PropertyInfo) MemberInfo; }
    }

    public MethodDefinition GetMethod
    {
      get { return _getMethod; }
    }

    public MethodDefinition SetMethod
    {
      get { return _setMethod; }
    }

    public override MemberDefinition BaseAsMember
    {
      get { return _base; }
      set
      {
        if (value == null || value is PropertyDefinition)
        {
          _base = (PropertyDefinition) value;
          if (GetMethod != null)
            GetMethod.Base = _base == null ? null : _base.GetMethod;
          if (SetMethod != null)
            SetMethod.Base = _base == null ? null : _base.SetMethod;
        }
        else
          throw new ArgumentException ("Base must be PropertyDefinition or null.", "value");
      }
    }

    public PropertyDefinition Base
    {
      get { return _base; }
      set { BaseAsMember = value; }
    }

    protected override bool IsSignatureCompatibleWith (MemberDefinition overrider)
    {
      ArgumentUtility.CheckNotNull ("overrider", overrider);

      PropertyDefinition overriderProperty = overrider as PropertyDefinition;
      if (overriderProperty == null)
      {
        return false;
      }
      else
      {
        return IsSignatureCompatibleWithProperty (overriderProperty);
      }
    }

    private bool IsSignatureCompatibleWithProperty (PropertyDefinition overrider)
    {
      ArgumentUtility.CheckNotNull ("overrider", overrider);
      return s_signatureChecker.PropertySignaturesMatch (PropertyInfo, overrider.PropertyInfo);
    }

    internal override void AddOverride (MemberDefinition member)
    {
      ArgumentUtility.CheckNotNull ("member", member);

      PropertyDefinition overrider = member as PropertyDefinition;
      if (overrider == null)
      {
        string message = string.Format ("Member {0} cannot override property {1} - it is not a property.", member.FullName, FullName);
        throw new ArgumentException (message);
      }

      Overrides.Add (overrider);

      if (GetMethod != null && overrider.GetMethod != null)
        GetMethod.AddOverride (overrider.GetMethod);
      if (SetMethod != null && overrider.SetMethod != null)
        SetMethod.AddOverride (overrider.SetMethod);
    }

    public override IEnumerable<MemberDefinition> GetOverridesAsMemberDefinitions()
    {
      foreach (PropertyDefinition overrider in Overrides)
      {
        yield return overrider;
      }
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
      base.AcceptForChildren (visitor);

      if (GetMethod != null)
      {
        GetMethod.Accept (visitor);
      }
      if (SetMethod != null)
      {
        SetMethod.Accept (visitor);
      }
    }
  }
}