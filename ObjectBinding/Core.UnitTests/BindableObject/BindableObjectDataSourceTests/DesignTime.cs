using System;
using System.ComponentModel;
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

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _dataSource = new BindableObjectDataSource();

      _mockRepository = new MockRepository();
      _stubSite = _mockRepository.Stub<ISite>();
      SetupResult.For (_stubSite.DesignMode).Return (true);
      _dataSource.Site = _stubSite;

      _provider = new BindableObjectProvider();
      BindableObjectProvider.SetCurrent (_provider);
    }

    [Test]
    public void GetType_WithValidTypeName ()
    {
      _mockRepository.ReplayAll();

      Assert.That (_dataSource.Type, Is.Null);
      _dataSource.TypeName = "Rubicon.ObjectBinding.UnitTests::BindableObject.TestDomain.SimpleBusinessObjectClass";
      Assert.That (_dataSource.Type, Is.SameAs (typeof (SimpleBusinessObjectClass)));

      _mockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (TypeLoadException))]
    public void GetType_WithInvalidValidTypeName ()
    {
      _mockRepository.ReplayAll ();

      _dataSource.TypeName = "Invalid";
      Dev.Null = _dataSource.Type;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetBusinessObjectClass_WithValidType ()
    {
      _mockRepository.ReplayAll ();

      _dataSource.TypeName = "Rubicon.ObjectBinding.UnitTests::BindableObject.TestDomain.SimpleBusinessObjectClass";

      IBusinessObjectClass actual = _dataSource.BusinessObjectClass;
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.BusinessObjectProvider, Is.Not.SameAs (_provider));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetBusinessObjectClass_NotSameTwice ()
    {
      _mockRepository.ReplayAll ();

      _dataSource.TypeName = "Rubicon.ObjectBinding.UnitTests::BindableObject.TestDomain.SimpleBusinessObjectClass";

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
      _mockRepository.ReplayAll ();

      _dataSource.TypeName = "Rubicon.ObjectBinding.UnitTests::BindableObject.TestDomain.SimpleReferenceType";
      Dev.Null = _dataSource.BusinessObjectClass;

      _mockRepository.VerifyAll ();
    }
  }
}