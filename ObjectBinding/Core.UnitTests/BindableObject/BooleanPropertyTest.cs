using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BooleanPropertyTest : TestBase
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
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithValueType<bool>), _businessObjectProvider);
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
    public void GetDisplayName_EnUs ()
    {
      IBusinessObjectBooleanProperty property = (IBusinessObjectBooleanProperty) _businessObjectClass.GetPropertyDefinition ("Scalar");
      Thread.CurrentThread.CurrentUICulture = _cultureEnUs;
      
      Assert.That (property.GetDisplayName (true), Is.EqualTo ("Yes"));
      Assert.That (property.GetDisplayName (false), Is.EqualTo ("No"));
    }

    [Test]
    public void GetDisplayName_CultureDeAt ()
    {
      IBusinessObjectBooleanProperty property = (IBusinessObjectBooleanProperty) _businessObjectClass.GetPropertyDefinition ("Scalar");
      Thread.CurrentThread.CurrentUICulture = _cultureDeAt;
      
      Assert.That (property.GetDisplayName (true), Is.EqualTo ("Ja"));
      Assert.That (property.GetDisplayName (false), Is.EqualTo ("Nein"));
    }

    [Test]
    [Ignore ("TODO: test")]
    public void IBusinessObjectEnumerationProperty ()
    {
    }
  }
}