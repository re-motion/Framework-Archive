using System;
using Mixins.UnitTests.SampleTypes;

namespace Mixins.UnitTests.Configuration.ValidationSampleTypes
{
  public class DoubleImplementer : IBaseType2
  {
    public string IfcMethod ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}