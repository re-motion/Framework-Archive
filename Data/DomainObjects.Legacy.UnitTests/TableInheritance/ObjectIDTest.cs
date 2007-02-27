using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance
{
  [TestFixture]
  public class ObjectIDTest : TableInheritanceMappingTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public ObjectIDTest ()
    {
    }

    // methods and properties

    [Test]
    public void InitializeWithAbstractType ()
    {
      try
      {
        new ObjectID (typeof (DomainBase), Guid.NewGuid ());
        Assert.Fail ("ArgumentException was expected.");
      }
      catch (ArgumentException ex)
      {
        string expectedMessage = string.Format (
            "An ObjectID cannot be constructed for abstract type '{0}' of class '{1}'.\r\nParameter name: classType",
            typeof (DomainBase).AssemblyQualifiedName, "DomainBase");

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }
  }
}
