using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;
using Remotion.ServiceLocation;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries
{
  [TestFixture]
  public class ILinqProviderComponentFactoryTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = new DefaultServiceLocator();
    }

    [Test]
    public void GetInstance_Once ()
    {
      var factory = _serviceLocator.GetInstance<ILinqProviderComponentFactory>();

      Assert.That (factory, Is.Not.Null);
      Assert.That (factory, Is.TypeOf (typeof (LinqProviderComponentFactory)));
    }

    [Test]
    public void GetInstance_Twice ()
    {
      var factory1 = _serviceLocator.GetInstance<ILinqProviderComponentFactory>();
      var factory2 = _serviceLocator.GetInstance<ILinqProviderComponentFactory>();

      Assert.That (factory1, Is.Not.SameAs (factory2));
    }
  }
}