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
  public class EnumerationPropertyTest : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;
    private IBusinessObjectClass _businessObjectClass;

    private CultureInfo _uiCultureBackup;
    private CultureInfo _cultureEnUs;
    private CultureInfo _cultureDeAt;

    [SetUp]
    public void SetUp ()
    {
      _businessObjectProvider = new BindableObjectProvider();
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithValueType<TestEnum>), _businessObjectProvider);
      _businessObjectClass = classReflector.GetMetadata();

      _cultureEnUs = new CultureInfo ("en-US");
      _cultureDeAt = new CultureInfo ("de-AT");

      _uiCultureBackup = Thread.CurrentThread.CurrentUICulture;
      Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
    }

    [TearDown]
    public void TearDown ()
    {
      Thread.CurrentThread.CurrentUICulture = _uiCultureBackup;
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetAllValues ()
    {
      IBusinessObjectEnumerationProperty property = (IBusinessObjectEnumerationProperty) _businessObjectClass.GetPropertyDefinition ("Scalar");
      EnumerationValueInfo[] expected = new EnumerationValueInfo[]
          {
              new EnumerationValueInfo (TestEnum.Value1, "Value1", "Value1", true),
              new EnumerationValueInfo (TestEnum.Value2, "Value2", "Value2", true),
              new EnumerationValueInfo (TestEnum.Value3, "Value3", "Value3", true)
          };

      CheckEnumerationValueInfos (property.GetAllValues(), expected);
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetAllValues_Nullable ()
    {
      IBusinessObjectBooleanProperty property = (IBusinessObjectBooleanProperty) _businessObjectClass.GetPropertyDefinition ("Nullable");
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetEnabledValues ()
    {
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetValueInfoByValue ()
    {
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetValueInfoByIdentifier ()
    {
    }

    private void CheckEnumerationValueInfos (IEnumerationValueInfo[] actual, EnumerationValueInfo[] expected)
    {
      ArgumentUtility.CheckNotNull ("expected", expected);

      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Length, Is.EqualTo (expected.Length));
      for (int i = 0; i < expected.Length; i++)
      {
        Assert.That (actual[i], Is.Not.Null);
        Assert.That (actual[i].Value, Is.EqualTo (expected[i].Value));
        Assert.That (actual[i].Identifier, Is.EqualTo (expected[i].Identifier));
        Assert.That (actual[i].IsEnabled, Is.EqualTo (expected[i].IsEnabled));
        Assert.That (actual[i].DisplayName, Is.EqualTo (expected[i].DisplayName));
      }
    }
  }
}