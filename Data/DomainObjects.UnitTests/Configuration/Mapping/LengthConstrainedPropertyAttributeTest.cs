using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class LengthConstrainedPropertyAttributeTest
  {
    private class StubLengthConstrainedPropertyAttribute : NullableLengthConstrainedPropertyAttribute
    {
      public StubLengthConstrainedPropertyAttribute ()
      {
      }
    }

    private NullableLengthConstrainedPropertyAttribute _attribute;
    private ILengthConstrainedPropertyAttribute _lengthConstraint;

    [SetUp]
    public void SetUp()
    {
      _attribute = new StubLengthConstrainedPropertyAttribute ();
      _lengthConstraint = _attribute;
    }

    [Test]
    public void GetMaximumLength_FromDefault()
    {
      Assert.IsFalse (_attribute.HasMaximumLength);
      Assert.IsNull (_lengthConstraint.MaximumLength);
    }

    [Test]
    public void GetMaximumLength_FromExplicitValue()
    {
      _attribute.MaximumLength = 100;
      Assert.IsTrue (_attribute.HasMaximumLength);
      Assert.AreEqual (100, _attribute.MaximumLength);
      Assert.AreEqual (100, _lengthConstraint.MaximumLength);
    }

    [Test]
    [ExpectedException(typeof (InvalidOperationException))]
    public void GetMaximumLength_FromDefault_WithInvalidOperationException ()
    {
      Dev.Null = _attribute.MaximumLength;
    }
  }
}