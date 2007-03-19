namespace Rubicon.Core.UnitTests.Utilities
{
  public class DerivedSampleClass : SampleClass
  {
    public override string PropertyWithSingleAttribute
    {
      get { return null; }
    }

    [Multiple]
    public override string PropertyWithMultipleAttribute
    {
      get { return null; }
    }
  }
}