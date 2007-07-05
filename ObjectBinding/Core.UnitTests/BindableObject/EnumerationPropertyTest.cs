using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class EnumerationPropertyTest : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    private CultureInfo _uiCultureBackup;

    private MockRepository _mockRepository;
    private IBusinessObject _mockBusinessObject;

    [SetUp]
    public void SetUp ()
    {
      _businessObjectProvider = new BindableObjectProvider();

      _uiCultureBackup = Thread.CurrentThread.CurrentUICulture;
      Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

      _mockRepository = new MockRepository();
      _mockBusinessObject = _mockRepository.CreateMock<IBusinessObject>();
    }

    [TearDown]
    public void TearDown ()
    {
      Thread.CurrentThread.CurrentUICulture = _uiCultureBackup;
    }

    [Test]
    public void GetAllValues ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");
      EnumerationValueInfo[] expected = new EnumerationValueInfo[]
          {
              new EnumerationValueInfo (TestEnum.Value1, "Value1", "Value1", true),
              new EnumerationValueInfo (TestEnum.Value2, "Value2", "Value2", true),
              new EnumerationValueInfo (TestEnum.Value3, "Value3", "Value3", true)
          };

      CheckEnumerationValueInfos (expected, property.GetAllValues());
    }

    [Test]
    public void GetAllValues_Nullable ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");
      EnumerationValueInfo[] expected = new EnumerationValueInfo[]
          {
              new EnumerationValueInfo (TestEnum.Value1, "Value1", "Value1", true),
              new EnumerationValueInfo (TestEnum.Value2, "Value2", "Value2", true),
              new EnumerationValueInfo (TestEnum.Value3, "Value3", "Value3", true)
          };

      CheckEnumerationValueInfos (expected, property.GetAllValues());
    }

    [Test]
    public void GetAllValues_WithUndefinedValue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<EnumWithUndefinedValue>), "Scalar");
      EnumerationValueInfo[] expected = new EnumerationValueInfo[]
          {
              new EnumerationValueInfo (EnumWithUndefinedValue.Value1, "Value1", "Value1", true),
              new EnumerationValueInfo (EnumWithUndefinedValue.Value2, "Value2", "Value2", true),
              new EnumerationValueInfo (EnumWithUndefinedValue.Value3, "Value3", "Value3", true)
          };

      CheckEnumerationValueInfos (expected, property.GetAllValues());
    }


    [Test]
    public void GetAllValues_WithInvariantCulture ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<EnumWithResources>), "Scalar");

      EnumerationValueInfo[] expected = new EnumerationValueInfo[]
          {
              new EnumerationValueInfo (EnumWithResources.Value1, "Value1", "Value 1", true),
              new EnumerationValueInfo (EnumWithResources.Value2, "Value2", "Value 2", true),
              new EnumerationValueInfo (EnumWithResources.ValueWithoutResource, "ValueWithoutResource", "ValueWithoutResource", true)
          };

      CheckEnumerationValueInfos (expected, property.GetAllValues());
    }

    [Test]
    public void GetAllValues_WithDescription ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<EnumWithDescription>), "Scalar");

      EnumerationValueInfo[] expected = new EnumerationValueInfo[]
          {
              new EnumerationValueInfo (EnumWithDescription.Value1, "Value1", "Value I", true),
              new EnumerationValueInfo (EnumWithDescription.Value2, "Value2", "Value II", true),
              new EnumerationValueInfo (EnumWithDescription.ValueWithoutDescription, "ValueWithoutDescription", "ValueWithoutDescription", true)
          };

      CheckEnumerationValueInfos (expected, property.GetAllValues());
    }


    [Test]
    public void GetEnabledValues ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");
      EnumerationValueInfo[] expected = new EnumerationValueInfo[]
          {
              new EnumerationValueInfo (TestEnum.Value1, "Value1", "Value1", true),
              new EnumerationValueInfo (TestEnum.Value2, "Value2", "Value2", true),
              new EnumerationValueInfo (TestEnum.Value3, "Value3", "Value3", true)
          };

      CheckEnumerationValueInfos (expected, property.GetEnabledValues (null));
    }


    [Test]
    public void GetValueInfoByValue_WithValidEnumValue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      CheckEnumerationValueInfo (
          new EnumerationValueInfo (TestEnum.Value1, "Value1", "Value1", true),
          property.GetValueInfoByValue (TestEnum.Value1, null));
    }

    [Test]
    public void GetValueInfoByValue_WithNull ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.GetValueInfoByValue (null, null), Is.Null);
    }

    [Test]
    public void GetValueInfoByValue_WithUndefinedEnumValue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<EnumWithUndefinedValue>), "Scalar");

      Assert.That (property.GetValueInfoByValue (EnumWithUndefinedValue.UndefinedValue, null), Is.Null);
    }

    [Test]
    public void GetValueInfoByValue_WithInvalidEnumValue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      CheckEnumerationValueInfo (
          new EnumerationValueInfo ((TestEnum) (-1), "-1", "-1", false),
          property.GetValueInfoByValue ((TestEnum) (-1), null));
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetValueInfoByValue_WithDisabledEnumValue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "DisabledFromProperty");
      _mockRepository.ReplayAll();

      IEnumerationValueInfo actual = property.GetValueInfoByValue (TestEnum.Value1, _mockBusinessObject);

      _mockRepository.VerifyAll();
      CheckEnumerationValueInfo (new EnumerationValueInfo (TestEnum.Value1, "Value1", "Value1", false), actual);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = 
        "Object must be the same type as the enum. The type passed in was 'Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain.EnumWithUndefinedValue'; "
        + "the enum type was 'Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain.TestEnum'.")]
    public void GetValueInfoByValue_WitEnumValueFromOtherType ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      property.GetValueInfoByValue (EnumWithUndefinedValue.Value1, null);
    }


    [Test]
    public void GetValueInfoByValue_WithInvariantCulture ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<EnumWithResources>), "Scalar");

      CheckEnumerationValueInfo (
          new EnumerationValueInfo (EnumWithResources.Value1, "Value1", "Value 1", true),
          property.GetValueInfoByValue (EnumWithResources.Value1, null));
    }

    [Test]
    public void GetValueInfoByValue_WithDescription ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<EnumWithDescription>), "Scalar");

      CheckEnumerationValueInfo (
          new EnumerationValueInfo (EnumWithDescription.Value1, "Value1", "Value I", true),
          property.GetValueInfoByValue (EnumWithDescription.Value1, null));
    }


    [Test]
    public void GetValueInfoByIdentifier_WithValidEnumValue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      CheckEnumerationValueInfo (
          new EnumerationValueInfo (TestEnum.Value1, "Value1", "Value1", true),
          property.GetValueInfoByIdentifier ("Value1", null));
    }

    [Test]
    public void GetValueInfoByIdentifier_WithNull ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.GetValueInfoByValue (null, null), Is.Null);
    }

    [Test]
    public void GetValueInfoByIdentifier_WithUndefinedEnumValue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<EnumWithUndefinedValue>), "Scalar");

      Assert.That (property.GetValueInfoByIdentifier ("UndefinedValue", null), Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ParseException))]
    public void GetValueInfoByIdentifier_WithInvalidIdentifier ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      property.GetValueInfoByIdentifier ("Invalid", null);
    }


    [Test]
    public void GetValueInfoByIdentifier_WithInvariantCulture ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<EnumWithResources>), "Scalar");

      CheckEnumerationValueInfo (
          new EnumerationValueInfo (EnumWithResources.Value1, "Value1", "Value 1", true),
          property.GetValueInfoByIdentifier ("Value1", null));
    }

    [Test]
    public void GetValueInfoByIdentifier_WithDescription ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<EnumWithDescription>), "Scalar");

      CheckEnumerationValueInfo (
          new EnumerationValueInfo (EnumWithDescription.Value1, "Value1", "Value I", true),
          property.GetValueInfoByIdentifier ("Value1", null));
    }


    [Test]
    public void ConvertFromNativePropertyType ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.ConvertFromNativePropertyType (TestEnum.Value1), Is.EqualTo (TestEnum.Value1));
    }

    [Test]
    public void ConvertFromNativePropertyType_WithNull ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.ConvertFromNativePropertyType (null), Is.Null);
    }

    [Test]
    public void ConvertFromNativePropertyType_WithUndefinedValue ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<EnumWithUndefinedValue>), "Scalar");

      Assert.That (property.ConvertFromNativePropertyType (EnumWithUndefinedValue.UndefinedValue), Is.Null);
    }

    [Test]
    public void ConvertFromNativePropertyType_WithInvalidEnumValue ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.ConvertFromNativePropertyType ((TestEnum) (-1)), Is.EqualTo ((TestEnum) (-1)));
    }

    [Test]
    public void ConvertFromNativePropertyType_WithEnumValueFromOtherType ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      property.ConvertFromNativePropertyType (EnumWithUndefinedValue.Value1);
      Assert.That (property.ConvertFromNativePropertyType (EnumWithUndefinedValue.Value1), Is.EqualTo (EnumWithUndefinedValue.Value1));
    }


    [Test]
    public void ConvertToNativePropertyType ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.ConvertFromNativePropertyType (TestEnum.Value1), Is.EqualTo (TestEnum.Value1));
    }

    [Test]
    public void ConvertToNativePropertyType_WithNull ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.ConvertToNativePropertyType (null), Is.Null);
    }

    [Test]
    public void ConvertToNativePropertyType_WithUndefinedValue ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<EnumWithUndefinedValue>), "Scalar");

      Assert.That (property.ConvertToNativePropertyType (null), Is.EqualTo (EnumWithUndefinedValue.UndefinedValue));
    }

    [Test]
    public void ConvertToNativePropertyType_WithInvalidEnumValue ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.ConvertToNativePropertyType ((TestEnum) (-1)), Is.EqualTo ((TestEnum) (-1)));
    }

    [Test]
    public void ConvertToNativePropertyType_WithEnumValueFromOtherType ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.ConvertToNativePropertyType (EnumWithUndefinedValue.Value1), Is.EqualTo (EnumWithUndefinedValue.Value1));
    }


    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage =
        "The property 'NullableScalar' defined on type 'Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain.ClassWithValueType`1[Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain.EnumWithUndefinedValue]'"
        + " must not be nullable since the property's type already defines a 'Rubicon.ObjectBinding.UndefinedEnumValueAttribute'.")]
    public void Initialize_NullableWithUndefinedValue ()
    {
      CreateProperty (typeof (ClassWithValueType<EnumWithUndefinedValue>), "NullableScalar");
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage =
        "The enum type 'Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain.EnumWithUndefinedValueFromOtherType' "
        + "defines a 'Rubicon.ObjectBinding.UndefinedEnumValueAttribute' with an enum value that belongs to a different enum type.")]
    public void Initialize_WithUndefinedEnumValueFromOtherType ()
    {
      CreateProperty(typeof (ClassWithValueType<EnumWithUndefinedValueFromOtherType>), "Scalar");
    }

    private EnumerationProperty CreateProperty (Type type, string propertyName)
    {
      return new EnumerationProperty (new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (type, propertyName),  null, false));
    }

    private void CheckEnumerationValueInfos (EnumerationValueInfo[] expected, IEnumerationValueInfo[] actual)
    {
      ArgumentUtility.CheckNotNull ("expected", expected);

      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Length, Is.EqualTo (expected.Length));
      for (int i = 0; i < expected.Length; i++)
        CheckEnumerationValueInfo (expected[i], actual[i]);
    }

    private void CheckEnumerationValueInfo (EnumerationValueInfo expected, IEnumerationValueInfo actual)
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