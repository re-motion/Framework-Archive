using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.EnumerationPropertyTests
{
  [TestFixture]
  public class Common : EnumerationTestBase
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
      CreateProperty (typeof (ClassWithValueType<EnumWithUndefinedValueFromOtherType>), "Scalar");
    }

    private EnumerationProperty CreateProperty (Type type, string propertyName)
    {
      return new EnumerationProperty (new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (type, propertyName), null, false, false));
    }
  }
}