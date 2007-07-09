using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.EnumerationPropertyTests
{
  [TestFixture]
  public class GetAllValues : EnumerationTestBase
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
    public void Enum ()
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
    public void NullableEnum ()
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
    public void UndefinedValueEnum ()
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
    public void GetDisplayNameFromGlobalizationSerivce ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");
      IBindableObjectGlobalizationService mockGlobalizationService = _mockRepository.CreateMock<IBindableObjectGlobalizationService>();
      _businessObjectProvider.AddService (typeof (IBindableObjectGlobalizationService), mockGlobalizationService);

      EnumerationValueInfo[] expected = new EnumerationValueInfo[]
          {
              new EnumerationValueInfo (TestEnum.Value1, "Value1", "MockValue1", true),
              new EnumerationValueInfo (TestEnum.Value2, "Value2", "MockValue2", true),
              new EnumerationValueInfo (TestEnum.Value3, "Value3", "MockValue3", true)
          };

      Expect.Call (mockGlobalizationService.GetEnumerationValueDisplayName (TestEnum.Value1)).Return ("MockValue1");
      Expect.Call (mockGlobalizationService.GetEnumerationValueDisplayName (TestEnum.Value2)).Return ("MockValue2");
      Expect.Call (mockGlobalizationService.GetEnumerationValueDisplayName (TestEnum.Value3)).Return ("MockValue3");
      _mockRepository.ReplayAll();

      IEnumerationValueInfo[] actual = property.GetAllValues();

      _mockRepository.VerifyAll();
      CheckEnumerationValueInfos (expected, actual);
    }

    private EnumerationProperty CreateProperty (Type type, string propertyName)
    {
      return new EnumerationProperty (new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (type, propertyName), null, false, false));
    }
  }
}