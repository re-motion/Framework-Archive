using System;
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

    protected override bool IsSignatureCompatibleWith (MemberDefinition overrider)
    {
      ArgumentUtility.CheckNotNull ("overrider", overrider);

      PropertyDefinition overriderProperty = overrider as PropertyDefinition;
      if (overrider == null)
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

    public override void AddOverride (MemberDefinition member)
    {
      ArgumentUtility.CheckNotNull ("member", member);

      PropertyDefinition property = member as PropertyDefinition;
      if (property == null)
      {
        string message = string.Format ("Member {0} cannot override property {1} - it is not a property.", member.FullName, FullName);
        throw new ArgumentException (message);
      }

      Overrides.Add (property);
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
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