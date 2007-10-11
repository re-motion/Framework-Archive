using System;
using NUnit.Framework;
using Rubicon.Security.Data.DomainObjects;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Security.UnitTests.Data.DomainObjects.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class SerializatiionTest
  {
    [Test]
    public void Serialization ()
    {
      SecurityClientTransactionExtension extension = new SecurityClientTransactionExtension ();
      SecurityClientTransactionExtension deserializedExtension = Serializer.SerializeAndDeserialize (extension);

      Assert.AreNotSame (extension, deserializedExtension);
    }
  }
}