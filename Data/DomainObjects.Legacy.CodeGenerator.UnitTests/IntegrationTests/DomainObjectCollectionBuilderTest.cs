using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Remotion.Data.DomainObjects.Legacy.CodeGenerator.UnitTests.IntegrationTests
{
  [TestFixture]
  public class DomainObjectCollectionBuilderTest : MappingBaseTest
  {
    [Test]
    public void BuildOrderCollection ()
    {
      using (StringWriter stringWriter = new StringWriter ())
      {
        TypeName orderCollectionTypeName = new TypeName ("Remotion.Data.DomainObjects.Legacy.CodeGenerator.UnitTests.IntegrationTests.TestDomain.OrderCollection", "Remotion.Data.DomainObjects.Legacy.CodeGenerator.UnitTests");

        DomainObjectCollectionBuilder builder = new DomainObjectCollectionBuilder (stringWriter);
        builder.Build (orderCollectionTypeName, "Order", DomainObjectCollectionBuilder.DefaultBaseClass, false);

        Assert.AreEqual (GetFile (@"OrderCollection.cs"), stringWriter.ToString ());
      }
    }

    private string GetFile (string filename)
    {
      return ResourceManager.GetResourceString ("IntegrationTests.TestDomain." + filename);
    }
  }
}
