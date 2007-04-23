using System;
using System.ComponentModel;
using System.Configuration;
using System.Web.UI.Design;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.Web.UI.Design;

namespace Rubicon.Web.UnitTests.UI.Design
{
  [TestFixture]
  public class WebDesignModeHelperTest
  {
    private MockRepository _mockRepository;
    private ISite _mockSite;
    private IWebApplication _mockWebApplication;

    [SetUp]
    public void SetUp()
    {
      _mockRepository = new MockRepository();
      _mockSite = _mockRepository.CreateMock<ISite>();
      _mockWebApplication = _mockRepository.CreateMock<IWebApplication>();

      SetupResult.For (_mockSite.DesignMode).Return (true);
    }

    [Test]
    public void Initialize()
    {
      _mockRepository.ReplayAll();

      WebDesginModeHelper webDesginModeHelper = new WebDesginModeHelper (_mockSite);

      _mockRepository.VerifyAll();
      Assert.That (webDesginModeHelper.Site, Is.SameAs (_mockSite));
    }

    [Test]
    public void GetConfiguration()
    {
      System.Configuration.Configuration expected = ConfigurationManager.OpenExeConfiguration (ConfigurationUserLevel.None);
      Expect.Call (_mockSite.GetService (typeof (IWebApplication))).Return (_mockWebApplication);
      Expect.Call (_mockWebApplication.OpenWebConfiguration (true)).Return (expected);
      _mockRepository.ReplayAll();

      WebDesginModeHelper webDesginModeHelper = new WebDesginModeHelper (_mockSite);

      System.Configuration.Configuration actual = webDesginModeHelper.GetConfiguration();

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void GetProjectPath()
    {
      IProjectItem mockProjectItem = _mockRepository.CreateMock<IProjectItem>();

      Expect.Call (_mockSite.GetService (typeof (IWebApplication))).Return (_mockWebApplication);
      SetupResult.For (_mockWebApplication.RootProjectItem).Return (mockProjectItem);
      Expect.Call (mockProjectItem.PhysicalPath).Return ("TheProjectPath");
      _mockRepository.ReplayAll();

      WebDesginModeHelper webDesginModeHelper = new WebDesginModeHelper (_mockSite);

      string actual = webDesginModeHelper.GetProjectPath();

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("TheProjectPath"));
    }
  }
}