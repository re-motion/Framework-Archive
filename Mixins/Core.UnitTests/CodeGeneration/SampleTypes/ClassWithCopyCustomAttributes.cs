using System;

namespace Remotion.Mixins.UnitTests.CodeGeneration.SampleTypes
{
  [CopyCustomAttributes (typeof (CopyTemplate))]
  public class ClassWithCopyCustomAttributes
  {
    [SampleCopyTemplate]
    public class CopyTemplate { }
  }
}
