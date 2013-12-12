using System;
using NUnit.Framework;
using Remotion.Diagnostics;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.Diagnostics
{
  public class IDebuggerInterfaceTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
    }

    [Test]
    public void GetInstance_Once ()
    {
      var factory = _serviceLocator.GetInstance<IDebuggerInterface>();

      Assert.That (factory, Is.Not.Null);
      Assert.That (factory, Is.TypeOf (typeof (DebuggerInterface)));
    }

    [Test]
    public void GetInstance_Twice ()
    {
      var factory1 = _serviceLocator.GetInstance<IDebuggerInterface>();
      var factory2 = _serviceLocator.GetInstance<IDebuggerInterface>();

      Assert.That (factory1, Is.Not.SameAs (factory2));
    }
  }
}