using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class DateTimePropertyTest : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    public override void SetUp ()
    {
      base.SetUp ();

      _businessObjectProvider = new BindableObjectProvider ();
    }

    [Test]
    public void Initialize_DateProperty ()
    {
      IBusinessObjectDateTimeProperty property = new DateProperty (
          new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "Date"), null, false, false));

      Assert.That (property.Type, Is.EqualTo (DateTimeType.Date));
    }

    [Test]
    public void Initialize_DateTimeProperty ()
    {
      IBusinessObjectDateTimeProperty property = new DateTimeProperty (
          new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "DateTime"), null, false, false));

      Assert.That (property.Type, Is.EqualTo (DateTimeType.DateTime));
    }
  }
}