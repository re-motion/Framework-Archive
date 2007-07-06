using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests
{
  [TestFixture]
  public class Common : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    public override void SetUp ()
    {
      base.SetUp ();

      _businessObjectProvider = new BindableObjectProvider ();
    }

    [Test]
    public void CreateIfNull ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar");

      Assert.That (property.CreateIfNull, Is.False);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException),
        ExpectedMessage = "Create method is not supported by 'Rubicon.ObjectBinding.BindableObject.Properties.ReferenceProperty'.")]
    public void Create ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar");

      property.Create (null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Initialize_WithMissmatchedConcreteType ()
    {
      new ReferenceProperty (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleClass>), "Scalar"), null, false, false),
          TypeFactory.GetConcreteType (typeof (ClassWithAllDataTypes)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Initialize_WithConcreteTypeNotImplementingIBusinessObject ()
    {
      new ReferenceProperty (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleClass>), "Scalar"), null, false, false),
          typeof (SimpleClass));
    }

    private ReferenceProperty CreateProperty (string propertyName)
    {
      return new ReferenceProperty (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleClass>), propertyName), null, false, false),
          TypeFactory.GetConcreteType (typeof (SimpleClass)));
    }
  }
}