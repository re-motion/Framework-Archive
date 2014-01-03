using System;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation.TestDomain
{
  public interface ITestCompoundWithoutPublicConstructor
  {
  }


  [ImplementationFor (typeof (ITestCompoundWithoutPublicConstructor), RegistrationType = RegistrationType.Compound)]
  public class TestCompoundWithoutPublicConstructor : ITestCompoundWithoutPublicConstructor
  {
  }
}