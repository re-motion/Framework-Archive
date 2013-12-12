using System;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Rendering;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocBooleanValueImplementation.Rendering
{
  [TestFixture]
  public class IBocCheckBoxRendererTest
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
      var factory = _serviceLocator.GetInstance<IBocCheckBoxRenderer>();

      Assert.That (factory, Is.Not.Null);
      Assert.That (factory, Is.TypeOf (typeof (BocCheckBoxRenderer)));
    }

    [Test]
    public void GetInstance_Twice ()
    {
      var factory1 = _serviceLocator.GetInstance<IBocCheckBoxRenderer>();
      var factory2 = _serviceLocator.GetInstance<IBocCheckBoxRenderer>();

      Assert.That (factory1, Is.SameAs (factory2));
    }
  }
}