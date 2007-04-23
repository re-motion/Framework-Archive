using System;
using System.ComponentModel;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.Design;

namespace Rubicon.Core.UnitTests.Design
{
  [TestFixture]
  public class DesignModeHelperBaseTest
  {
    private MockRepository _mockRepository;
    private ISite _mockSite;

    [SetUp]
    public void SetUp()
    {
      _mockRepository = new MockRepository();
      _mockSite = _mockRepository.CreateMock<ISite>();
    }

    [Test]
    public void Initialize()
    {
      Expect.Call (_mockSite.DesignMode).Return (true);
      _mockRepository.ReplayAll();

      DesignModeHelperBase stubDesginerHelper = new StubDesignModeHelper (_mockSite);

      _mockRepository.VerifyAll();
      Assert.That (stubDesginerHelper.Site, Is.SameAs (_mockSite));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The 'StubDesignModeHelper' requires that DesignMode is active for the site.\r\nParameter name: site")]
    public void Initialize_WithDesignModeFalse()
    {
      Expect.Call (_mockSite.DesignMode).Return (false);
      _mockRepository.ReplayAll();

      new StubDesignModeHelper (_mockSite);

      _mockRepository.VerifyAll();
    }
  }
}