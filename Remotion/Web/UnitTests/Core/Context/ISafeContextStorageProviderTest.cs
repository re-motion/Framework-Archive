using System;
using System.Linq;
using NUnit.Framework;
using Remotion.Context;
using Remotion.ServiceLocation;
using Remotion.Web.Context;

namespace Remotion.Web.UnitTests.Core.Context
{
  [TestFixture]
  public class ISafeContextStorageProviderTest
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
      var instances = _serviceLocator.GetAllInstances<ISafeContextStorageProvider>();

      Assert.That (instances, Is.Not.Empty);
      Assert.That (instances.First(), Is.InstanceOf (typeof (HttpContextStorageProvider)));
    }

    [Test]
    public void GetInstance_Twice_ReturnsDifferentInstance ()
    {
      var instance1 = _serviceLocator.GetAllInstances<ISafeContextStorageProvider>().First();
      var instance2 = _serviceLocator.GetAllInstances<ISafeContextStorageProvider>().First();

      Assert.That (instance1, Is.Not.SameAs (instance2));
    }
  }
}