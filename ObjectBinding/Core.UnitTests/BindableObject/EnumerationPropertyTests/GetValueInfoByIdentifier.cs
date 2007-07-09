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
  public class GetValueInfoByIdentifier : EnumerationTestBase
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
    public void Test_WithValidEnumValue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      CheckEnumerationValueInfo (
          new EnumerationValueInfo (TestEnum.Value1, "Value1", "Value1", true),
          property.GetValueInfoByIdentifier ("Value1", null));
    }

    [Test]
    public void Test_WithNull ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.GetValueInfoByValue (null, null), Is.Null);
    }

    [Test]
    public void Test_WithUndefinedEnumValue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<EnumWithUndefinedValue>), "Scalar");

      Assert.That (property.GetValueInfoByIdentifier ("UndefinedValue", null), Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ParseException))]
    public void Test_WithInvalidIdentifier ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      property.GetValueInfoByIdentifier ("Invalid", null);
    }

    [Test]
    public void Test_WithGlobalizationSerivce ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");
      IBindableObjectGlobalizationService mockGlobalizationService = _mockRepository.CreateMock<IBindableObjectGlobalizationService>();
      _businessObjectProvider.AddService (typeof (IBindableObjectGlobalizationService), mockGlobalizationService);

      Expect.Call (mockGlobalizationService.GetEnumerationValueDisplayName (TestEnum.Value1)).Return ("MockValue1");
      _mockRepository.ReplayAll();

      IEnumerationValueInfo actual = property.GetValueInfoByIdentifier ("Value1", null);

      _mockRepository.VerifyAll();
      CheckEnumerationValueInfo (new EnumerationValueInfo (TestEnum.Value1, "Value1", "MockValue1", true), actual);
    }

    private EnumerationProperty CreateProperty (Type type, string propertyName)
    {
      return new EnumerationProperty (new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (type, propertyName), null, false, false));
    }
  }
}