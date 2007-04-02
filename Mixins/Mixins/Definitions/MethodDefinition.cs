using System;
using System.Reflection;

namespace Mixins.Definitions
{
  public class MethodDefinition : MemberDefinition
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
      return s_signatureChecker.SignatureMatch (MethodInfo, overrider.MethodInfo);
    }
  }
}