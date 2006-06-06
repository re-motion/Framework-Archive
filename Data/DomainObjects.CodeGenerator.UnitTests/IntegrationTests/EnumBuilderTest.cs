using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.IntegrationTests
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
      //TODO: use TypeName instead of a Type as parameter
      using (StringWriter stringWriter = new StringWriter ())
      {
        TypeName typeName = new TypeName (
            "Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.TestDomain.OrderPriority", "Rubicon.Data.DomainObjects.CodeGenerator.UnitTests");

        EnumBuilder.Build (stringWriter, typeName, false);
        Assert.AreEqual (File.ReadAllText (@"..\..\TestDomain\OrderPriority.cs"), stringWriter.ToString ());
      }
    }

  }
}
