using System;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation.TestDomain
{
  public interface ITestCompoundWithInvalidConstructor
  {
  }


  [ImplementationFor (typeof (ITestCompoundWithInvalidConstructor), RegistrationType = RegistrationType.Compound)]
  public class TestCompoundWithInvalidConstructor : ITestCompoundWithInvalidConstructor
  {
    public TestCompoundWithInvalidConstructor (object invalidArgument)
    {
    }
  }
}