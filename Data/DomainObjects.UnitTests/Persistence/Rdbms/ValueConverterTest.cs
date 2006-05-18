using System;
using System.Data;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class ValueConverterTest : DatabaseTest
  {
    // types

    // static members and constants

    // member fields

    ValueConverter _converter = new ValueConverter ();
    StorageProviderManager _storageProviderManager;
    IDbConnection _connection;

    ClassDefinition _ceoDefinition;

    // construction and disposing

    public ValueConverterTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _converter = new ValueConverter ();
      _ceoDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Ceo");

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
    public void GetObjectIDWithGuidValue ()
    {
      Guid value = Guid.NewGuid ();
      ObjectID expectedID = new ObjectID ("Order", value);
      ObjectID actualID = _converter.GetObjectID (MappingConfiguration.Current.ClassDefinitions["Order"], value);

      Assert.AreEqual (expectedID, actualID);
    }

    [Test]
    [ExpectedException (typeof (ConverterException), "Invalid null value for not-nullable property 'Type' encountered. Class: 'Customer'.")]
    public void GetNullValueForEnum ()
    {
      ClassDefinition customerDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Customer");
      PropertyDefinition enumProperty = customerDefinition["Type"];

      _converter.GetValue (customerDefinition, enumProperty, DBNull.Value);
    }

    [Test]
    [ExpectedException (typeof (ConverterException), "Invalid null value for not-nullable relation property 'Company' encountered. Class: 'Ceo'.")]
    public void GetValueForCeoWithCompanyIDAndCompanyIDClassIDNull ()
    {
      IDbCommand command = CreateCeoCommand (new Guid ("{2927059E-AE59-49a7-8B59-B959E579C629}"));
      using (IDataReader reader = command.ExecuteReader ())
      {
        Assert.IsTrue (reader.Read ());

        int columnOrdinal = _converter.GetMandatoryOrdinal (reader, _ceoDefinition["Company"].ColumnName);
        _converter.GetValue (_ceoDefinition, _ceoDefinition["Company"], reader, columnOrdinal);
      }
    }

    [Test]
    [ExpectedException (typeof (ConverterException), "Invalid null value for not-nullable relation property 'Company' encountered. Class: 'Ceo'.")]
    public void GetValueForCeoWithCompanyIDNull ()
    {
      IDbCommand command = CreateCeoCommand (new Guid ("{523B490A-5B18-4f22-AF5B-BD9A4DA3F629}"));
      using (IDataReader reader = command.ExecuteReader ())
      {
        Assert.IsTrue (reader.Read ());

        int columnOrdinal = _converter.GetMandatoryOrdinal (reader, _ceoDefinition["Company"].ColumnName);
        _converter.GetValue (_ceoDefinition, _ceoDefinition["Company"], reader, columnOrdinal);
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), "Incorrect database value encountered. Column 'CompanyIDClassID' of entity 'Ceo' must not contain null.")]
    public void GetValueForCeoWithCompanyIDClassIDNull ()
    {
      IDbCommand command = CreateCeoCommand (new Guid ("{04341C7D-7B7C-49fc-82E6-8E481CDACA30}"));
      using (IDataReader reader = command.ExecuteReader ())
      {
        Assert.IsTrue (reader.Read ());

        int columnOrdinal = _converter.GetMandatoryOrdinal (reader, _ceoDefinition["Company"].ColumnName);
        _converter.GetValue (_ceoDefinition, _ceoDefinition["Company"], reader, columnOrdinal);
      }
    }

    [Test]
    public void GetValueForCeo ()
    {
      IDbCommand command = CreateCeoCommand ((Guid) DomainObjectIDs.Ceo1.Value);
      using (IDataReader reader = command.ExecuteReader ())
      {
        Assert.IsTrue (reader.Read ());

        int columnOrdinal = _converter.GetMandatoryOrdinal (reader, _ceoDefinition["Company"].ColumnName);
        ObjectID actualID = (ObjectID) _converter.GetValue (_ceoDefinition, _ceoDefinition["Company"], reader, columnOrdinal);

        Assert.AreEqual (DomainObjectIDs.Company1, actualID);
      }
    }

    [Test]
    public void GetValueForFolderWithoutParent ()
    {
      IDbCommand command = CreateFileSystemItemCommand (new Guid ("{976A6864-3344-4b3c-8F67-6348CF361D22}"));
      using (IDataReader reader = command.ExecuteReader ())
      {
        Assert.IsTrue (reader.Read ());

        ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Folder");
        PropertyDefinition parentFolderProperty = folderDefinition.GetMandatoryPropertyDefinition ("ParentFolder");

        int columnOrdinal = _converter.GetMandatoryOrdinal (reader, parentFolderProperty.ColumnName);
        Assert.IsNull (_converter.GetValue (folderDefinition, parentFolderProperty, reader, columnOrdinal));
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), "Incorrect database value encountered. Column 'ParentFolderIDClassID' of entity 'FileSystemItem' must not contain a value.")]
    public void GetValueForFileWithParentFolderIDNull ()
    {
      IDbCommand command = CreateFileSystemItemCommand (new Guid ("{DCBE9554-2724-49a6-AECA-B811E20E4110}"));
      using (IDataReader reader = command.ExecuteReader ())
      {
        Assert.IsTrue (reader.Read ());

        ClassDefinition fileDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("File");
        PropertyDefinition parentFolderProperty = fileDefinition.GetMandatoryPropertyDefinition ("ParentFolder");

        int columnOrdinal = _converter.GetMandatoryOrdinal (reader, parentFolderProperty.ColumnName);
        Assert.IsNull (_converter.GetValue (fileDefinition, parentFolderProperty, reader, columnOrdinal));
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), "Incorrect database value encountered. Column 'ParentFolderIDClassID' of entity 'FileSystemItem' must not contain null.")]
    public void GetValueForFileWithParentFolderIDClassIDNull ()
    {
      IDbCommand command = CreateFileSystemItemCommand (new Guid ("{A26B6A4E-D497-4b32-821B-74AFAD7EAD0A}"));
      using (IDataReader reader = command.ExecuteReader ())
      {
        Assert.IsTrue (reader.Read ());

        ClassDefinition fileDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("File");
        PropertyDefinition parentFolderProperty = fileDefinition.GetMandatoryPropertyDefinition ("ParentFolder");

        int columnOrdinal = _converter.GetMandatoryOrdinal (reader, parentFolderProperty.ColumnName);
        Assert.IsNull (_converter.GetValue (fileDefinition, parentFolderProperty, reader, columnOrdinal));
      }
    }

    private IDbCommand CreateCeoCommand (Guid id)
    {
      return CreateCommand ("Ceo", id);
    }

    private IDbCommand CreateFileSystemItemCommand (Guid id)
    {
      return CreateCommand ("FileSystemItem", id);
    }

    private IDbCommand CreateCommand (string table, Guid id)
    {
      IDbCommand command = _connection.CreateCommand ();
      command.CommandText = string.Format ("SELECT * FROM [{0}] where ID = @id", table);

      IDbDataParameter parameter = command.CreateParameter ();
      parameter.ParameterName = "@id";
      parameter.Value = id;
      command.Parameters.Add (parameter);

      return command;
    }
  }
}