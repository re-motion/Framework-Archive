using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BindableObjectGlobalizationServiceTest : TestBase
  {
    private IBindableObjectGlobalizationService _globalizationService;
    private CultureInfo _uiCultureBackup;

    public override void SetUp ()
    {
      base.SetUp();

      _globalizationService = new BindableObjectGlobalizationService();

      _uiCultureBackup = Thread.CurrentThread.CurrentUICulture;
      Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
    }

    public override void TearDown ()
    {
      base.TearDown();
      Thread.CurrentThread.CurrentUICulture = _uiCultureBackup;
    }

    [Test]
    public void GetBooleanValueDisplayName ()
    {
      Assert.That (_globalizationService.GetBooleanValueDisplayName (true), Is.EqualTo ("Yes"));
      Assert.That (_globalizationService.GetBooleanValueDisplayName (false), Is.EqualTo ("No"));
    }

    [Test]
    public void GetEnumerationValueDisplayName ()
    {
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (EnumWithResources.Value1), Is.EqualTo ("Value 1"));
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (EnumWithResources.Value2), Is.EqualTo ("Value 2"));
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (EnumWithResources.ValueWithoutResource), Is.EqualTo ("ValueWithoutResource"));
    }

    [Test]
    public void GetEnumerationValueDisplayName_WithDescription ()
    {
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (EnumWithDescription.Value1), Is.EqualTo ("Value I"));
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (EnumWithDescription.Value2), Is.EqualTo ("Value II"));
      Assert.That (
          _globalizationService.GetEnumerationValueDisplayName (EnumWithDescription.ValueWithoutDescription),
          Is.EqualTo ("ValueWithoutDescription"));
    }

    [Test]
    public void GetEnumerationValueDisplayName_WithoutResources ()
    {
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (TestEnum.Value1), Is.EqualTo ("Value1"));
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (TestEnum.Value2), Is.EqualTo ("Value2"));
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (TestEnum.Value3), Is.EqualTo ("Value3"));
    }

    [Test]
    public void GetPropertyDisplayName ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithResources), "Value1");
      Assert.That (_globalizationService.GetPropertyDisplayName (propertyInfo), Is.EqualTo ("Value 1"));
    }

    [Test]
    public void GetPropertyDisplayName_WithoutMultiLingualResourcesAttribute ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String");
      Assert.That (_globalizationService.GetPropertyDisplayName (propertyInfo), Is.EqualTo ("String"));
    }

    [Test]
    public void GetPropertyDisplayName_WithoutResourceForProperty ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithResources), "ValueWithoutResource");
      Assert.That (_globalizationService.GetPropertyDisplayName (propertyInfo), Is.EqualTo ("ValueWithoutResource"));
    }
  }
}