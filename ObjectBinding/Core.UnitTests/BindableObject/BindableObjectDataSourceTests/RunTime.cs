using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.Development.UnitTesting;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.BindableObjectDataSourceTests
{
  [TestFixture]
  public class RunTime:TestBase
  {
    private BindableObjectDataSource _dataSource;
    private BindableObjectProvider _provider;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _dataSource = new BindableObjectDataSource();

      _provider = new BindableObjectProvider();
      BindableObjectProvider.SetCurrent (_provider);
    }

    [Test]
    public void GetAndSetBusinessObject ()
    {
      IBusinessObject businessObject = MockRepository.GenerateStub<IBusinessObject>();
      ((IBusinessObjectDataSource) _dataSource).BusinessObject = businessObject;
      Assert.That (((IBusinessObjectDataSource) _dataSource).BusinessObject, Is.SameAs (businessObject));
    }

    [Test]
    public void GetAndSetTypeName ()
    {
      Assert.That (_dataSource.TypeName, Is.Empty);
      string typeName = "Rubicon.ObjectBinding.UnitTests::BindableObject.TestDomain.SimpleBusinessObjectClass";
      _dataSource.TypeName = typeName;
      Assert.That (_dataSource.TypeName, Is.EqualTo (typeName));
    }

    [Test]
    public void GetAndSetTypeName_WithEmpty ()
    {
      _dataSource.TypeName = string.Empty;
      Assert.That (_dataSource.TypeName, Is.Empty);
    }

    [Test]
    public void GetAndSetTypeName_WithNull ()
    {
      _dataSource.TypeName = null;
      Assert.That (_dataSource.TypeName, Is.Empty);
    }

    [Test]
    public void GetType_WithValidTypeName ()
    {
      Assert.That (_dataSource.Type, Is.Null);
      _dataSource.TypeName = "Rubicon.ObjectBinding.UnitTests::BindableObject.TestDomain.SimpleBusinessObjectClass";
      Assert.That (_dataSource.Type, Is.SameAs (typeof (SimpleBusinessObjectClass)));
    }

    [Test]
    [ExpectedException (typeof (TypeLoadException))]
    public void GetType_WithInvalidValidTypeName ()
    {
      _dataSource.TypeName = "Invalid";
      Dev.Null = _dataSource.Type;
    }

    [Test]
    public void GetType_WithEmptyTypeName ()
    {
      Assert.That (_dataSource.Type, Is.Null);
      _dataSource.TypeName = string.Empty;
      Assert.That (_dataSource.Type, Is.Null);
    }

    [Test]
    public void GetBusinessObjectClass_WithoutType ()
    {
      Assert.That (_dataSource.Type, Is.Null);
      Assert.That (_dataSource.BusinessObjectClass, Is.Null);
    }

    [Test]
    public void GetBusinessObjectClass_WithValidType ()
    {
      _dataSource.TypeName = "Rubicon.ObjectBinding.UnitTests::BindableObject.TestDomain.SimpleBusinessObjectClass";
      Assert.That (_dataSource.BusinessObjectClass, Is.SameAs (_provider.GetBindableObjectClass (typeof (SimpleBusinessObjectClass))));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage =
        "Type 'Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain.SimpleReferenceType' does not implement the "
        + "'Rubicon.ObjectBinding.IBusinessObject' interface via the 'Rubicon.ObjectBinding.BindableObject.BindableObjectMixin'.\r\n"
        + "Parameter name: type")]
    public void GetBusinessObjectClass_WithTypeNotUsingBindableObjectMixin ()
    {
      _dataSource.TypeName = "Rubicon.ObjectBinding.UnitTests::BindableObject.TestDomain.SimpleReferenceType";
      Dev.Null = _dataSource.BusinessObjectClass;
    }
  }
}