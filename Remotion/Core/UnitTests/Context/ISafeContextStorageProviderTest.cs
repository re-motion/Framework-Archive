using System.Linq;
using NUnit.Framework;
using Remotion.Context;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.Context
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
      var instance = _serviceLocator.GetAllInstances<ISafeContextStorageProvider>();

      Assert.That (instance, Is.Not.Null);
      Assert.That (instance, Is.All.TypeOf (typeof (CallContextStorageProvider)));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var isntance1 = _serviceLocator.GetAllInstances<ISafeContextStorageProvider>().First();
      var instance2 = _serviceLocator.GetAllInstances<ISafeContextStorageProvider>().First();

      Assert.That (isntance1, Is.Not.SameAs (instance2));
    }
  }
}