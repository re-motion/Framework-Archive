using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
[TestFixture]
public class ObjectDeletedExceptionTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public ObjectDeletedExceptionTest ()
  {
  }

  // methods and properties

  [Test]
  public void Serialization ()
  {
    ObjectDeletedException exception = new ObjectDeletedException (DomainObjectIDs.Order1);

    using (MemoryStream memoryStream = new MemoryStream ())
    {
      BinaryFormatter formatter = new BinaryFormatter();
      formatter.Serialize (memoryStream, exception);
      memoryStream.Seek (0, SeekOrigin.Begin);

      formatter = new BinaryFormatter();

      exception = (ObjectDeletedException) formatter.Deserialize (memoryStream);
      
      Assert.AreEqual (DomainObjectIDs.Order1, exception.ID);
    }    
  }
}
}
