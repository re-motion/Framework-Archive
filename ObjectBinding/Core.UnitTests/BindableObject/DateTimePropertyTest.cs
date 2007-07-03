using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class DateTimePropertyTest : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    [SetUp]
    public void SetUp ()
    {
      _businessObjectProvider = new BindableObjectProvider ();
    }

    [Test]
    public void Initialize_DateProperty ()
    {
      IBusinessObjectDateTimeProperty property =
          new DateProperty (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "Date"), null, false);

      Assert.That (property.Type, Is.EqualTo (DateTimeType.Date));
    }

    [Test]
    public void Initialize_DateTimeProperty ()
    {
      IBusinessObjectDateTimeProperty property =
          new DateTimeProperty (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "DateTime"), null, false);

      Assert.That (property.Type, Is.EqualTo (DateTimeType.DateTime));
    }
  }
}