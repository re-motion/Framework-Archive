using System;
using System.Data;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance
{
  [TestFixture]
  public class DataContainerFactoryTest : SqlProviderBaseTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public DataContainerFactoryTest ()
    {
    }

    // methods and properties

    [Test]
    public void RelationClassIDColumnRefersToAbstractClass ()
    {
      ObjectID id = new ObjectID (typeof (Order), new Guid ("{F404FD2C-B92F-46d8-BEAC-F92C0599BFD3}"));
      SelectCommandBuilder builder = SelectCommandBuilder.CreateForIDLookup (Provider, "*", "TableInheritance_Order", id);

      using (IDbCommand command = builder.Create ())
      {
        using (IDataReader reader = command.ExecuteReader ())
        {
          DataContainerFactory factory = new DataContainerFactory (Provider, reader);

          try
          {
            factory.CreateDataContainer ();
            Assert.Fail ("RdbmsProviderException was expected.");
          }
          catch (RdbmsProviderException ex)
          {
            string expectedBeginOfMessage = string.Format ("Error while reading property 'Customer' of object '{0}':", id);
            Assert.IsTrue (ex.Message.StartsWith (expectedBeginOfMessage));
          }
        }
      }
    }

  }
}
