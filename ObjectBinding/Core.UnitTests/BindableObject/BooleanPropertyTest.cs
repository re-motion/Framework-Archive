using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BooleanPropertyTest : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;
    private IBusinessObjectClass _businessObjectClass;

    [SetUp]
    public void SetUp ()
    {
      _businessObjectProvider = new BindableObjectProvider ();
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithBoolean), _businessObjectProvider);
      _businessObjectClass = classReflector.GetMetadata();
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetDefaultValue_Boolean ()
    {
      IBusinessObjectBooleanProperty property = (IBusinessObjectBooleanProperty) _businessObjectClass.GetPropertyDefinition("Boolean");

      Assert.That (property.GetDefaultValue (_businessObjectClass), Is.False);
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetDefaultValue_NullableBoolean ()
    {
      IBusinessObjectBooleanProperty property = (IBusinessObjectBooleanProperty) _businessObjectClass.GetPropertyDefinition ("NullableBoolean");

      Assert.That (property.GetDefaultValue (_businessObjectClass), Is.Null);
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetDefaultValue_BooleanArray ()
    {
      IBusinessObjectBooleanProperty property = (IBusinessObjectBooleanProperty) _businessObjectClass.GetPropertyDefinition ("BooleanArray");

      Assert.That (property.GetDefaultValue (_businessObjectClass), Is.False);
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetDefaultValue_NullableBooleanArray ()
    {
      IBusinessObjectBooleanProperty property = (IBusinessObjectBooleanProperty) _businessObjectClass.GetPropertyDefinition ("NullableBooleanArray");

      Assert.That (property.GetDefaultValue (_businessObjectClass), Is.Null);
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetDisplayName ()
    {
    }

    [Test]
    [Ignore ("TODO: test")]
    public void IBusinessObjectEnumerationProperty ()
    {
    }
  }
}