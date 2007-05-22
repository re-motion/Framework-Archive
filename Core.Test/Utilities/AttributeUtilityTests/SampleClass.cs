using System;

namespace Rubicon.Core.UnitTests.Utilities.AttributeUtilityTests
{
  [Inherited]
  public class SampleClass
  {
    [Inherited]
    public virtual string PropertyWithSingleAttribute
    {
      get { return null; }
    }

    [Multiple]
    public virtual string PropertyWithMultipleAttribute
    {
      get { return null; }
    }
  }
}