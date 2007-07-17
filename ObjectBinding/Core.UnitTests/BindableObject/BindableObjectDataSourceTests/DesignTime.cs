using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.Development.UnitTesting;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.BindableObjectDataSourceTests
{
  [TestFixture]
  public class DesignTime : TestBase
  {
    private BindableObjectDataSource _dataSource;
    private BindableObjectProvider _provider;
    private MockRepository _mockRepository;
    private ISite _stubSite;
    private IDesignerHost _mockDesignerHost;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _dataSource = new BindableObjectDataSource();

      _mockRepository = new MockRepository();
      _stubSite = _mockRepository.Stub<ISite>();
      SetupResult.For (_stubSite.DesignMode).Return (true);
      _dataSource.Site = _stubSite;

      _mockDesignerHost = _mockRepository.CreateMock<IDesignerHost>();
      SetupResult.For (_stubSite.GetService(typeof (IDesignerHost))).Return (_mockDesignerHost);

      _provider = new BindableObjectProvider();
      BindableObjectProvider.SetCurrent (_provider);
    }

    [Test]
    public void GetType_WithValidTypeName ()
    {
      Expect.Call (_mockDesignerHost.GetType ("Assembly.TheTypeName, Assembly")).Return (typeof (SimpleBusinessObjectClass));
      _mockRepository.ReplayAll();

      Assert.That (_dataSource.Type, Is.Null);
      _dataSource.TypeName = "Assembly::TheTypeName";
      Assert.That (_dataSource.Type, Is.SameAs (typeof (SimpleBusinessObjectClass)));

      _mockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (TypeLoadException), ExpectedMessage = "Could not load type 'Assembly.Invalid, Assembly'.")]
    public void GetType_WithInvalidValidTypeName ()
    {
      Expect.Call (_mockDesignerHost.GetType ("Assembly.Invalid, Assembly")).Return (null);
      _mockRepository.ReplayAll ();

      _dataSource.TypeName = "Assembly::Invalid";
      Assert.That (_dataSource.Type, Is.Null);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetBusinessObjectClass_WithValidType ()
    {
      Expect.Call (_mockDesignerHost.GetType ("Assembly.TheTypeName, Assembly")).Return (typeof (SimpleBusinessObjectClass)).Repeat.AtLeastOnce ();
      _mockRepository.ReplayAll ();

      _dataSource.TypeName = "Assembly::TheTypeName";

      IBusinessObjectClass actual = _dataSource.BusinessObjectClass;
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.BusinessObjectProvider, Is.Not.SameAs (_provider));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetBusinessObjectClass_NotSameTwice ()
    {
      SetupResult.For (_mockDesignerHost.GetType ("Assembly.TheTypeName, Assembly")).Return (typeof (SimpleBusinessObjectClass));
      _mockRepository.ReplayAll ();

      _dataSource.TypeName = "Assembly::TheTypeName";

      IBusinessObjectClass actual = _dataSource.BusinessObjectClass;
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual, Is.Not.SameAs (_dataSource.BusinessObjectClass));

      _mockRepository.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage =
        "Type 'Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain.SimpleReferenceType' does not implement the "
        + "'Rubicon.ObjectBinding.IBusinessObject' interface via the 'Rubicon.ObjectBinding.BindableObject.BindableObjectMixin'.\r\n"
        + "Parameter name: type")]
    public void GetBusinessObjectClass_WithTypeNotUsingBindableObjectMixin ()
    {
      Expect.Call (_mockDesignerHost.GetType ("Assembly.TheTypeName, Assembly")).Return (typeof (SimpleReferenceType)).Repeat.AtLeastOnce ();
      _mockRepository.ReplayAll ();

      _dataSource.TypeName = "Assembly::TheTypeName";
      Dev.Null = _dataSource.BusinessObjectClass;

      _mockRepository.VerifyAll ();
    }
  }
}