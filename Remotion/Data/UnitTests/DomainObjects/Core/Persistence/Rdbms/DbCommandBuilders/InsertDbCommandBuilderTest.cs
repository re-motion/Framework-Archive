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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class InsertDbCommandBuilderTest : SqlProviderBaseTest
  {
    private IValueConverter _valueConverterStub;
    private IInsertedColumnsSpecification _insertedColumnsSpecificationStub;
    private ISqlDialect _sqlDialectMock;
    private IDbCommand _dbCommandStub;
    private IDbDataParameter _dbDataParameterStub;
    private IDataParameterCollection _dataParameterCollectionMock;
    private IRdbmsProviderCommandExecutionContext _commandExecutionContextStub;

    public override void SetUp ()
    {
      base.SetUp ();

      _valueConverterStub = MockRepository.GenerateStub<IValueConverter> ();

      _insertedColumnsSpecificationStub = MockRepository.GenerateStub<IInsertedColumnsSpecification>();

      _sqlDialectMock = MockRepository.GenerateStrictMock<ISqlDialect> ();
      _sqlDialectMock.Stub (stub => stub.StatementDelimiter).Return (";");

      _dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter> ();
      _dataParameterCollectionMock = MockRepository.GenerateStrictMock<IDataParameterCollection> ();

      _dbCommandStub = MockRepository.GenerateStub<IDbCommand> ();
      _dbCommandStub.Stub (stub => stub.CreateParameter ()).Return (_dbDataParameterStub);
      _dbCommandStub.Stub (stub => stub.Parameters).Return (_dataParameterCollectionMock);

      _commandExecutionContextStub = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext> ();
      _commandExecutionContextStub.Stub (stub => stub.CreateDbCommand ()).Return (_dbCommandStub);
    }

    [Test]
    public void Create_DefaultSchema ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table"));
      var builder = new InsertDbCommandBuilder (tableDefinition, _insertedColumnsSpecificationStub, _sqlDialectMock, _valueConverterStub);

      _sqlDialectMock.Expect (stub => stub.DelimitIdentifier ("Table")).Return ("[Table]");
      _sqlDialectMock.Replay ();

      _insertedColumnsSpecificationStub
          .Stub (stub => stub.AppendColumnNames (Arg<StringBuilder>.Is.Anything, Arg.Is (_dbCommandStub), Arg.Is (_sqlDialectMock)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[Column1], [Column2], [Column3]"));
      _insertedColumnsSpecificationStub
          .Stub (stub => stub.AppendColumnValues (Arg<StringBuilder>.Is.Anything, Arg.Is (_dbCommandStub), Arg.Is (_sqlDialectMock)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("5, 'test', true"));

      var result = builder.Create (_commandExecutionContextStub);

      _sqlDialectMock.VerifyAllExpectations ();
      Assert.That (result.CommandText, Is.EqualTo ("INSERT INTO [Table] ([Column1], [Column2], [Column3]) VALUES (5, 'test', true);"));
    }

    [Test]
    public void Create_CustomSchema ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition ("customSchema", "Table"));
      var builder = new InsertDbCommandBuilder (tableDefinition, _insertedColumnsSpecificationStub, _sqlDialectMock, _valueConverterStub);

      _sqlDialectMock.Expect (mock => mock.DelimitIdentifier ("customSchema")).Return ("[customSchema]");
      _sqlDialectMock.Expect (mock => mock.DelimitIdentifier ("Table")).Return ("[Table]");
      _sqlDialectMock.Replay ();

      _insertedColumnsSpecificationStub
          .Stub (stub => stub.AppendColumnNames (Arg<StringBuilder>.Is.Anything, Arg.Is (_dbCommandStub), Arg.Is (_sqlDialectMock)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[Column1], [Column2], [Column3]"));
      _insertedColumnsSpecificationStub
          .Stub (stub => stub.AppendColumnValues (Arg<StringBuilder>.Is.Anything, Arg.Is (_dbCommandStub), Arg.Is (_sqlDialectMock)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("5, 'test', true"));

      var result = builder.Create (_commandExecutionContextStub);

      _sqlDialectMock.VerifyAllExpectations ();
      Assert.That (result.CommandText, Is.EqualTo ("INSERT INTO [customSchema].[Table] ([Column1], [Column2], [Column3]) VALUES (5, 'test', true);"));
    }

   
   
  }
}