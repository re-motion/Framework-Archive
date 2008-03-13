using System;

namespace Rubicon.Mixins.UnitTests.CodeGeneration.SampleTypes
{
  [CopyCustomAttributes (typeof (CopyTemplate))]
  public class ClassWithCopyCustomAttributes
  {
    [SampleCopyTemplate]
    public class CopyTemplate { }
  }
}
