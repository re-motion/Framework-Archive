using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Rubicon.Data.DomainObjects.Legacy.CodeGenerator.UnitTests.IntegrationTests
{
  [TestFixture]
  public class EnumBuilderTest : MappingBaseTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EnumBuilderTest ()
    {
    }

    // methods and properties

    [Test]
    public void BuildOrderPriority ()
    {
      using (StringWriter stringWriter = new StringWriter ())
      {
        TypeName typeName = new TypeName (
            "Rubicon.Data.DomainObjects.Legacy.CodeGenerator.UnitTests.IntegrationTests.TestDomain.OrderPriority", "Rubicon.Data.DomainObjects.Legacy.CodeGenerator.UnitTests");

        EnumBuilder.Build (stringWriter, typeName, false);
        Assert.AreEqual (File.ReadAllText (@"OrderPriority.cs"), stringWriter.ToString ());
      }
    }

  }
}
