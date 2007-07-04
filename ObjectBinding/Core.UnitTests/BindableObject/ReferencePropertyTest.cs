using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class ReferencePropertyTest : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;
    private IBusinessObjectClass _businessObjectClass;

    [SetUp]
    public void SetUp ()
    {
      _businessObjectProvider = new BindableObjectProvider();
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithReferenceType<SimpleClass>), _businessObjectProvider);
      _businessObjectClass = classReflector.GetMetadata();
    }

    [Test]
    public void GetReferenceClass ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar");

      Assert.That (property.ReferenceClass, Is.SameAs (_businessObjectProvider.GetBindableObjectClass (typeof (SimpleClass))));
      Assert.That (((IBusinessObjectReferenceProperty)property).ReferenceClass, Is.SameAs (property.ReferenceClass));
    }

    [Test]
    [Ignore ("TODO: test")]
    public void SupportsSearchAvailableObjects ()
    {
      
    }

    [Test]
    [Ignore ("TODO: test")]
    public void SearchAvailableObjects ()
    {
      
    }

    [Test]
    public void CreateIfNull ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar");

      Assert.That (property.CreateIfNull, Is.False);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException),
       ExpectedMessage = "Create method is not supported by 'Rubicon.ObjectBinding.BindableObject.ReferenceProperty'.")]
    public void Create ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar");

      property.Create (null);
    }

    private ReferenceProperty CreateProperty (string propertyName)
    {
      return new ReferenceProperty (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleClass>), propertyName), null, false));
    }
  }
}