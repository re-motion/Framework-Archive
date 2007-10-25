using System;

namespace Rubicon.Mixins.UnitTests.Mixins.CodeGenSampleTypes
{
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public class InheritableAttribute : Attribute
  {
  }

  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class NonInheritableAttribute : Attribute
  {
  }

  [Inheritable]
  [NonInheritable]
  [CopyCustomAttributes (typeof (CopyTemplate))]
  public class MixinWithProtectedOverriderAndAttributes
  {
    [OverrideTarget]
    protected new string ToString ()
    {
      return "";
    }

    [SampleCopyTemplate]
    public class CopyTemplate {}
  }
}