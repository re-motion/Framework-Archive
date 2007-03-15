using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests
{
  [TestFixture]
  public class DomainObjectClassTest : DatabaseTest
  {
    // types

    // static members and constants

    // member fields

    private DomainObjectClass _domainObjectClass;

    // construction and disposing

    public DomainObjectClassTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp();
      _domainObjectClass = new DomainObjectClass (typeof (Order));
    }

    [Test]
    public void GetPropertyDefinitionsDoesNotReturnIsDiscarded ()
    {
      AssertPropertyIsNotContained ("IsDiscarded", _domainObjectClass.GetPropertyDefinitions ());
    }

    [Test]
    public void GetPropertyDefinitionsDoesNotReturnState ()
    {
      AssertPropertyIsNotContained ("State", _domainObjectClass.GetPropertyDefinitions ());
    }

    [Test]
    public void GetPropertyDefinitionsDoesNotReturnClientTransaction ()
    {
      AssertPropertyIsNotContained ("ClientTransaction", _domainObjectClass.GetPropertyDefinitions ());
    }

    [Test]
    public void GetPropertyDefinitionsDoesNotReturnID ()
    {
      AssertPropertyIsNotContained ("ID", _domainObjectClass.GetPropertyDefinitions ());
    }

    private void AssertPropertyIsNotContained (string propertyName, IBusinessObjectProperty[] properties)
    {
      foreach (IBusinessObjectProperty property in properties)
      {
        if (propertyName == property.Identifier)
          Assert.Fail ("Provided array must not contain property '{0}'.", propertyName);
      }
    }
  }
}
