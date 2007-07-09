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

    [TearDown]
    public void TearDown ()
    {
      Thread.CurrentThread.CurrentUICulture = _uiCultureBackup;
    }

    [Test]
    public void GetBooleanValueDisplayName_InvariantCulture ()
    {
      Assert.That (_globalizationService.GetBooleanValueDisplayName (true), Is.EqualTo ("Yes"));
      Assert.That (_globalizationService.GetBooleanValueDisplayName (false), Is.EqualTo ("No"));
    }

    [Test]
    public void GetEnumerationValueDisplayName_WithInvariantCulture ()
    {
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (EnumWithResources.Value1), Is.EqualTo ("Value 1"));
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (EnumWithResources.Value2), Is.EqualTo ("Value 2"));
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (EnumWithResources.ValueWithoutResource), Is.EqualTo ("ValueWithoutResource"));
    }

    [Test]
    public void GetAllValues_WithDescription ()
    {
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (EnumWithDescription.Value1), Is.EqualTo ("Value I"));
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (EnumWithDescription.Value2), Is.EqualTo ("Value II"));
      Assert.That (
          _globalizationService.GetEnumerationValueDisplayName (EnumWithDescription.ValueWithoutDescription),
          Is.EqualTo ("ValueWithoutDescription"));
    }
  }
}