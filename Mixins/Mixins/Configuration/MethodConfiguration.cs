using System;
using System.Reflection;

namespace Mixins.Configuration
{
  public class MethodConfiguration : MemberConfiguration
  {
    private SignatureChecker signatureChecker = new SignatureChecker ();

    public MethodConfiguration (MethodInfo memberInfo, ClassConfiguration declaringClass)
        : base (memberInfo, declaringClass)
    {
    }

    public MethodInfo MethodInfo
    {
      get {
        return (MethodInfo) MemberInfo;
      }
    }

    protected override bool IsSignatureCompatibleWith (MemberConfiguration overrider)
    {
      MethodConfiguration overriderMethod = overrider as MethodConfiguration;
      if (overrider == null)
      {
        return false;
      }
      else
      {
        return IsSignatureCompatibleWithMethod (overriderMethod);
      }
    }

    private bool IsSignatureCompatibleWithMethod (MethodConfiguration overrider)
    {
      return signatureChecker.SignatureMatch (MethodInfo, overrider.MethodInfo);
    }
  }
}