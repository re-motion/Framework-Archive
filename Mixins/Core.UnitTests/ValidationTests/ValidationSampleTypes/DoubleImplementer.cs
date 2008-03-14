using System;
using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.ValidationTests.ValidationSampleTypes
{
  public class DoubleImplementer : IBaseType2
  {
    public string IfcMethod ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}