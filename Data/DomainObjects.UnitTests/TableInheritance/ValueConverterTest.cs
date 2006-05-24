using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Data;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;
using Rubicon.Data.DomainObjects.Persistence;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
{
  [TestFixture]
  public class ValueConverterTest : TableInheritanceMappingTest
  {
    // types

    // static members and constants

    // member fields

    ValueConverter _converter = new ValueConverter ();
    StorageProviderManager _storageProviderManager;
    IDbConnection _connection;

    // construction and disposing

    public ValueConverterTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _converter = new ValueConverter ();

      _storageProviderManager = new StorageProviderManager ();
      RdbmsProvider provider = (RdbmsProvider) _storageProviderManager.GetMandatory ("TestDomain");
      provider.Connect ();
      _connection = provider.Connection;
    }


    [TearDown]
    public void TearDown ()
    {
      _storageProviderManager.Dispose ();
    }

    [Test]
    public void GetObjectIDValue ()
    {
      ClassDefinition personClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Person));
      PropertyDefinition clientProperty = personClass.GetMandatoryPropertyDefinition ("Client");
      ObjectID expectedID = DomainObjectIDs.Client;

      using (IDbCommand command = CreatePersonCommand ((Guid) DomainObjectIDs.Person.Value))
      {
        using (IDataReader reader = command.ExecuteReader ())
        {
          Assert.IsTrue (reader.Read ());

          int columnOrdinal = _converter.GetMandatoryOrdinal (reader, clientProperty.ColumnName);
          Assert.AreEqual (expectedID, _converter.GetValue (personClass, clientProperty, reader, columnOrdinal));
        }
      }
    }

    private IDbCommand CreatePersonCommand (Guid id)
    {
      return CreateCommand ("TableInheritance_Person", id, _connection);
    }
  }
}
