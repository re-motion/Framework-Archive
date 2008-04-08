using System;
using Remotion.Core.UnitTests.Mixins.SampleTypes;

namespace Remotion.Core.UnitTests.Mixins.ValidationTests.ValidationSampleTypes
{
  public class DoubleImplementer : IBaseType2
  {
    public string IfcMethod ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}