using System;

namespace Rubicon.Mixins.UnitTests.Mixins.CodeGenSampleTypes
{
  [CopyCustomAttributes (typeof (CopyTemplate))]
  public class ClassWithCopyCustomAttributes
  {
    [SampleCopyTemplate]
    public class CopyTemplate { }
  }
}
