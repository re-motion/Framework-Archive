using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.IntegrationTests
{
  [TestFixture]
  public class DomainObjectCollectionBuilderTest : MappingBaseTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public DomainObjectCollectionBuilderTest ()
    {
    }

    // methods and properties

    [Test]
    public void BuildOrderCollection ()
    {
      //TODO: use TypeName instead of a Type as parameter
      using (StringWriter stringWriter = new StringWriter ())
      {
        TypeName orderCollectionTypeName = new TypeName ("Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.TestDomain.OrderCollection", "Rubicon.Data.DomainObjects.CodeGenerator.UnitTests");

        DomainObjectCollectionBuilder.Build (stringWriter, orderCollectionTypeName, "Order", DomainObjectCollectionBuilder.DefaultBaseClass, false);

        Assert.AreEqual (File.ReadAllText (@"..\..\TestDomain\OrderCollection.cs"), stringWriter.ToString ());
      }
    }

  }
}
