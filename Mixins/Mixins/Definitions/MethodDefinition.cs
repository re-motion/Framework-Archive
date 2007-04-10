using System;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Definitions
{
  public class MethodDefinition : MemberDefinition, IVisitableDefinition
  {
    private static SignatureChecker s_signatureChecker = new SignatureChecker ();

    public MethodDefinition (MethodInfo memberInfo, ClassDefinition declaringClass)
        : base (memberInfo, declaringClass)
    {
    }

    public MethodInfo MethodInfo
    {
      get {
        return (MethodInfo) MemberInfo;
      }
    }

    protected override bool IsSignatureCompatibleWith (MemberDefinition overrider)
    {
      ArgumentUtility.CheckNotNull ("overrider", overrider);

      MethodDefinition overriderMethod = overrider as MethodDefinition;
      if (overrider == null)
      {
        return false;
      }
      else
      {
        return IsSignatureCompatibleWithMethod (overriderMethod);
      }
    }

    private bool IsSignatureCompatibleWithMethod (MethodDefinition overrider)
    {
      ArgumentUtility.CheckNotNull ("overrider", overrider);
      return s_signatureChecker.SignatureMatch (MethodInfo, overrider.MethodInfo);
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}