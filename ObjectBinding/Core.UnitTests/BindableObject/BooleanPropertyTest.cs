using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BooleanPropertyTest : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;
    private IBusinessObjectClass _businessObjectClass;

    private CultureInfo _uiCultureBackup;

    [SetUp]
    public void SetUp ()
    {
      _businessObjectProvider = new BindableObjectProvider();
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithValueType<bool>), _businessObjectProvider);
      _businessObjectClass = classReflector.GetMetadata();

      _uiCultureBackup = Thread.CurrentThread.CurrentUICulture;
      Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
    }

    [TearDown]
    public void TearDown ()
    {
      Thread.CurrentThread.CurrentUICulture = _uiCultureBackup;
    }

    [Test]
    public void GetDefaultValue_Scalar ()
    {
      IBusinessObjectBooleanProperty property = (IBusinessObjectBooleanProperty) _businessObjectClass.GetPropertyDefinition ("Scalar");

      Assert.That (property.GetDefaultValue (_businessObjectClass), Is.False);
    }

    [Test]
    public void GetDefaultValue_NullableScalar ()
    {
      IBusinessObjectBooleanProperty property = (IBusinessObjectBooleanProperty) _businessObjectClass.GetPropertyDefinition ("NullableScalar");

      Assert.That (property.GetDefaultValue (_businessObjectClass), Is.Null);
    }

    [Test]
    public void GetDefaultValue_Array ()
    {
      IBusinessObjectBooleanProperty property = (IBusinessObjectBooleanProperty) _businessObjectClass.GetPropertyDefinition ("Array");

      Assert.That (property.GetDefaultValue (_businessObjectClass), Is.False);
    }

    [Test]
    public void GetDefaultValue_NullableArray ()
    {
      IBusinessObjectBooleanProperty property = (IBusinessObjectBooleanProperty) _businessObjectClass.GetPropertyDefinition ("NullableArray");

      Assert.That (property.GetDefaultValue (_businessObjectClass), Is.Null);
    }

    [Test]
    public void GetDisplayName_InvariantCulture ()
    {
      IBusinessObjectBooleanProperty property = (IBusinessObjectBooleanProperty) _businessObjectClass.GetPropertyDefinition ("Scalar");

      Assert.That (property.GetDisplayName (true), Is.EqualTo ("Yes"));
      Assert.That (property.GetDisplayName (false), Is.EqualTo ("No"));
    }


    [Test]
    public void IBusinessObjectEnumerationProperty_GetAllValues ()
    {
      IBusinessObjectEnumerationProperty property = (IBusinessObjectEnumerationProperty) _businessObjectClass.GetPropertyDefinition ("NullableScalar");
      BooleanEnumerationValueInfo[] expected = new BooleanEnumerationValueInfo[]
          {
              new BooleanEnumerationValueInfo (true, (IBusinessObjectBooleanProperty) property),
              new BooleanEnumerationValueInfo (false, (IBusinessObjectBooleanProperty) property)
          };

      CheckEnumerationValueInfos (expected, property.GetAllValues());
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetEnabledValues ()
    {
      IBusinessObjectEnumerationProperty property = (IBusinessObjectEnumerationProperty) _businessObjectClass.GetPropertyDefinition ("NullableScalar");
      BooleanEnumerationValueInfo[] expected = new BooleanEnumerationValueInfo[]
          {
              new BooleanEnumerationValueInfo (true, (IBusinessObjectBooleanProperty) property),
              new BooleanEnumerationValueInfo (false, (IBusinessObjectBooleanProperty) property)
          };

      CheckEnumerationValueInfos (expected, property.GetEnabledValues (null));
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByValue_WithTrue ()
    {
      IBusinessObjectEnumerationProperty property = (IBusinessObjectEnumerationProperty) _businessObjectClass.GetPropertyDefinition ("Scalar");

      CheckEnumerationValueInfo (
          new BooleanEnumerationValueInfo (true, (IBusinessObjectBooleanProperty) property),
          property.GetValueInfoByValue (true, null));
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByValue_WithFalse ()
    {
      IBusinessObjectEnumerationProperty property = (IBusinessObjectEnumerationProperty) _businessObjectClass.GetPropertyDefinition ("Scalar");

      CheckEnumerationValueInfo (
          new BooleanEnumerationValueInfo (false, (IBusinessObjectBooleanProperty) property),
          property.GetValueInfoByValue (false, null));
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByValue_WithNull ()
    {
      IBusinessObjectEnumerationProperty property = (IBusinessObjectEnumerationProperty) _businessObjectClass.GetPropertyDefinition ("Scalar");

      Assert.That (property.GetValueInfoByValue (null, null), Is.Null);
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByIdentifier_WithTrue ()
    {
      IBusinessObjectEnumerationProperty property = (IBusinessObjectEnumerationProperty) _businessObjectClass.GetPropertyDefinition ("Scalar");

      CheckEnumerationValueInfo (
          new BooleanEnumerationValueInfo (true, (IBusinessObjectBooleanProperty) property),
          property.GetValueInfoByIdentifier ("True", null));
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByIdentifier_WithFalse ()
    {
      IBusinessObjectEnumerationProperty property = (IBusinessObjectEnumerationProperty) _businessObjectClass.GetPropertyDefinition ("Scalar");

      CheckEnumerationValueInfo (
          new BooleanEnumerationValueInfo (false, (IBusinessObjectBooleanProperty) property),
          property.GetValueInfoByIdentifier ("False", null));
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByIdentifier_WithNull ()
    {
      IBusinessObjectEnumerationProperty property = (IBusinessObjectEnumerationProperty) _businessObjectClass.GetPropertyDefinition ("Scalar");

      Assert.That (property.GetValueInfoByIdentifier (null, null), Is.Null);
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByIdentifier_WithEmptyString ()
    {
      IBusinessObjectEnumerationProperty property = (IBusinessObjectEnumerationProperty) _businessObjectClass.GetPropertyDefinition ("Scalar");

      Assert.That (property.GetValueInfoByIdentifier (string.Empty, null), Is.Null);
    }


    private void CheckEnumerationValueInfos (BooleanEnumerationValueInfo[] expected, IEnumerationValueInfo[] actual)
    {
      ArgumentUtility.CheckNotNull ("expected", expected);

      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Length, Is.EqualTo (expected.Length));
      for (int i = 0; i < expected.Length; i++)
        CheckEnumerationValueInfo (expected[i], actual[i]);
    }

    private void CheckEnumerationValueInfo (BooleanEnumerationValueInfo expected, IEnumerationValueInfo actual)
    {
      ArgumentUtility.CheckNotNull ("expected", expected);

      Assert.That (actual, Is.InstanceOfType (expected.GetType()));
      Assert.That (actual.Value, Is.EqualTo (expected.Value));
      Assert.That (actual.Identifier, Is.EqualTo (expected.Identifier));
      Assert.That (actual.IsEnabled, Is.EqualTo (expected.IsEnabled));
      Assert.That (actual.DisplayName, Is.EqualTo (expected.DisplayName));
    }
  }
}