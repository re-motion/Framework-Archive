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
    [Test]
    public void BuildOrderPriority ()
    {
      using (StringWriter stringWriter = new StringWriter ())
      {
        TypeName typeName = new TypeName (
            "Rubicon.Data.DomainObjects.Legacy.CodeGenerator.UnitTests.IntegrationTests.TestDomain.OrderPriority", "Rubicon.Data.DomainObjects.Legacy.CodeGenerator.UnitTests");

        EnumBuilder.Build (stringWriter, typeName, false);
        Assert.AreEqual (GetFile (@"OrderPriority.cs"), stringWriter.ToString ());
      }
    }

    private string GetFile (string filename)
    {
      return ResourceManager.GetResourceString ("IntegrationTests.TestDomain." + filename);
    }
  }
}
