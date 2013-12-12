using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.ServiceLocation;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  public class IDomainObjectCreatorTest
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
      var factory = _serviceLocator.GetInstance<IDomainObjectCreator>();

      Assert.That (factory, Is.Not.Null);
      Assert.That (factory, Is.TypeOf (typeof (DomainObjectCreator)));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var factory1 = _serviceLocator.GetInstance<IDomainObjectCreator>();
      var factory2 = _serviceLocator.GetInstance<IDomainObjectCreator>();

      Assert.That (factory1, Is.SameAs (factory2));
    }
  }
}