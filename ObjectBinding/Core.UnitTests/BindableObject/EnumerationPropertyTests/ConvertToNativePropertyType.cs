using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.EnumerationPropertyTests
{
  [TestFixture]
  public class ConvertToNativePropertyType : EnumerationTestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = new BindableObjectProvider();

      _mockRepository = new MockRepository();
      _mockRepository.CreateMock<IBusinessObject>();
    }


    [Test]
    public void Test_WithValidValue ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.ConvertFromNativePropertyType (TestEnum.Value1), Is.EqualTo (TestEnum.Value1));
    }

    [Test]
    public void Test_WithNull ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.ConvertToNativePropertyType (null), Is.Null);
    }

    [Test]
    public void Test_WithUndefinedValue ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<EnumWithUndefinedValue>), "Scalar");

      Assert.That (property.ConvertToNativePropertyType (null), Is.EqualTo (EnumWithUndefinedValue.UndefinedValue));
    }

    [Test]
    public void Test_WithInvalidEnumValue ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.ConvertToNativePropertyType ((TestEnum) (-1)), Is.EqualTo ((TestEnum) (-1)));
    }

    [Test]
    public void Test_WithEnumValueFromOtherType ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.ConvertToNativePropertyType (EnumWithUndefinedValue.Value1), Is.EqualTo (EnumWithUndefinedValue.Value1));
    }


    private EnumerationProperty CreateProperty (Type type, string propertyName)
    {
      return new EnumerationProperty (new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (type, propertyName), null, false, false));
    }
  }
}