// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Data;
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DbCommandBuilders
{
  // TODO Review 4058: Derive this (and other DbCommandBuilder tests) from StandardMappingTest
  [TestFixture]
  public class SingleIDLookupSelectDbCommandBuilderTest : SqlProviderBaseTest
  {
    private SimpleColumnDefinition _objectIDColumnDefinition;
    private ISelectedColumnsSpecification _selectedColumnsStub;
    private ISqlDialect _sqlDialectStub;
    private IDbCommandFactory _commandFactoryStub;
    private IDbCommand _dbCommandStub;
    private IDbDataParameter _dbDataParameterStub;
    private IDataParameterCollection _dataParameterCollectionMock;
    private Guid _guid;
    private IValueConverter _valueConverterStub;

    public override void SetUp ()
    {
      base.SetUp();

      _objectIDColumnDefinition = new SimpleColumnDefinition ("ID", typeof (Guid), "uniqueidentifier", false, true);
      
      _selectedColumnsStub = MockRepository.GenerateStub<ISelectedColumnsSpecification>();
      _selectedColumnsStub
        .Stub (stub => stub.AppendProjection (Arg<StringBuilder>.Is.Anything, Arg<ISqlDialect>.Is.Anything))
        .WhenCalled(mi=> ((StringBuilder) mi.Arguments[0]).Append(" [Column1], [Column2], [Column3] "));

      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect>();
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("dbo")).Return ("[dbo]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("customSchema")).Return ("[customSchema]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Table")).Return ("[Table]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("ID")).Return ("[ID]");
      _sqlDialectStub.Stub (stub => stub.GetParameterName ("ID")).Return ("@ID");
      _sqlDialectStub.Stub (stub => stub.GetParameterName ("@ID")).Return ("@ID");
      
      _dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter>();

      _dataParameterCollectionMock = MockRepository.GenerateStrictMock<IDataParameterCollection> ();
      _dataParameterCollectionMock.Expect (mock => mock.Add (_dbDataParameterStub)).Return(1);
      _dataParameterCollectionMock.Replay();

      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      _dbCommandStub.Stub(stub => stub.CreateParameter()).Return (_dbDataParameterStub);
      _dbCommandStub.Stub (stub => stub.Parameters).Return (_dataParameterCollectionMock);
      
      _commandFactoryStub = MockRepository.GenerateStub<IDbCommandFactory>();
      _commandFactoryStub.Stub (stub => stub.CreateDbCommand()).Return (_dbCommandStub);

      _guid = Guid.NewGuid ();

      _valueConverterStub = MockRepository.GenerateStub<IValueConverter> ();
      _valueConverterStub.Stub (stub => stub.GetDBValue (Arg<object>.Is.Anything)).Return (_guid);
    }

    [Test]
    public void Create_DefaultSchema ()
    {
      // TODO Review 4058: Add object mother for TableDefinition with Create (string schema, string table), Create (schema, table, objectIDColumnDef, classIDColumnDef, timestampColumnDef, params otherColumns)
      // TODO Review 4058: Also for UnionViewDefinition and FilterViewDefinition
      var tableDefinition = new TableDefinition (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "Table"),
          null,
          _objectIDColumnDefinition,
          new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", true, false),
          new SimpleColumnDefinition ("Timestamp", typeof (DateTime), "datetime", true, false),
          new SimpleColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);

      var builder = new SingleIDLookupSelectDbCommandBuilder (
          tableDefinition,
          _selectedColumnsStub,
          new ObjectID ("Order", _guid),
          _sqlDialectStub,
          _valueConverterStub);

      var result = builder.Create (_commandFactoryStub);

      Assert.That (result.CommandText, Is.EqualTo ("SELECT [Column1], [Column2], [Column3] FROM [Table] WHERE [ID] = @ID"));
      Assert.That (_dbDataParameterStub.Value, Is.EqualTo(_guid));
      _dataParameterCollectionMock.VerifyAllExpectations();
    }

    [Test]
    public void Create_CustomSchema ()
    {
      var tableDefinition = new TableDefinition (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition ("customSchema", "Table"),
          null,
          _objectIDColumnDefinition,
          new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", true, false),
          new SimpleColumnDefinition ("Timestamp", typeof (DateTime), "datetime", true, false),
          new SimpleColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);

      var builder = new SingleIDLookupSelectDbCommandBuilder (
          tableDefinition,
          _selectedColumnsStub,
          new ObjectID ("Order", _guid),
          _sqlDialectStub,
          _valueConverterStub);

      var result = builder.Create (_commandFactoryStub);

      Assert.That (result.CommandText, Is.EqualTo ("SELECT [Column1], [Column2], [Column3] FROM [customSchema].[Table] WHERE [ID] = @ID"));
      Assert.That (_dbDataParameterStub.Value, Is.EqualTo (_guid));
      _dataParameterCollectionMock.VerifyAllExpectations();
    }
  }
}