using System;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation.TestDomain
{
  public interface ITestDecoratorWithoutPublicConstructor
  {
  }


  [ImplementationFor (typeof (ITestDecoratorWithoutPublicConstructor), RegistrationType = RegistrationType.Decorator)]
  public class TestDecoratorWithoutPublicConstructor : ITestDecoratorWithoutPublicConstructor
  {
  }
}