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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DbCommandBuilders.Specifications
{
  [TestFixture]
  public class ComparedColumnsSpecificationTest
  {
    private ColumnDefinition _column1;
    private object _value1;
    private IStorageTypeInformation _storageTypeInformationMock1;

    private ColumnDefinition _column2;
    private object _value2;
    private IStorageTypeInformation _storageTypeInformationMock2;
    private StringBuilder _statement;
    private IDataParameterCollection _parametersCollectionMock;
    private IDbCommand _commandStub;
    private ISqlDialect _sqlDialectStub;
    private Dictionary<ColumnValue, IDbDataParameter> _parameterCache;

    [SetUp]
    public void SetUp ()
    {
      _storageTypeInformationMock1 = MockRepository.GenerateStrictMock<IStorageTypeInformation>();
      _column1 = new ColumnDefinition ("First", typeof (int), _storageTypeInformationMock1, true, false);
      _value1 = 17;

      _storageTypeInformationMock2 = MockRepository.GenerateStrictMock<IStorageTypeInformation> ();
      _column2 = new ColumnDefinition ("Second", typeof (int), _storageTypeInformationMock2, true, false);
      _value2 = 18;

      _statement = new StringBuilder();

      _parametersCollectionMock = MockRepository.GenerateStrictMock<IDataParameterCollection>();
      _commandStub = MockRepository.GenerateStub<IDbCommand> ();
      _commandStub.Stub (stub => stub.Parameters).Return (_parametersCollectionMock);

      _parameterCache = new Dictionary<ColumnValue, IDbDataParameter>();
      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect> ();
    }

    [Test]
    public void Initialization_Empty ()
    {
      Assert.That (
          () => new ComparedColumnsSpecification (Enumerable.Empty <ColumnValue>()),
          Throws.Exception.TypeOf<ArgumentEmptyException>().With.Message.EqualTo (
              "The sequence of compared column values must contain at least one element.\r\nParameter name: comparedColumnValues"));
    }

    [Test]
    public void Initialization_NonEmpty ()
    {
      var specification = new ComparedColumnsSpecification (new[] { new ColumnValue(_column1, _value1) });
      Assert.That (specification.ComparedColumnValues, Is.EqualTo (new[] { new ColumnValue(_column1, _value1) }));
    }

    [Test]
    public void AddParameters_OneValue ()
    {
      var specification = new ComparedColumnsSpecification (new[] { new ColumnValue (_column1, _value1) });

      _sqlDialectStub.Stub (stub => stub.GetParameterName ("First")).Return ("pFirst");

      var parameterStrictMock = MockRepository.GenerateStrictMock<IDbDataParameter> ();
      parameterStrictMock.Expect (mock => mock.ParameterName = "pFirst");
      parameterStrictMock.Replay ();

      _parametersCollectionMock.Expect (mock => mock.Add (parameterStrictMock)).Return (0);
      _parametersCollectionMock.Replay ();

      _storageTypeInformationMock1
          .Expect (mock => mock.CreateDataParameter (_commandStub, _value1))
          .Return (parameterStrictMock);
      _storageTypeInformationMock1.Replay ();

      specification.AddParameters (_commandStub, _sqlDialectStub, null);

      _parametersCollectionMock.VerifyAllExpectations ();
      parameterStrictMock.VerifyAllExpectations ();
      _storageTypeInformationMock1.VerifyAllExpectations ();
    }

    [Test]
    public void AddParameters_MultipleValues ()
    {
      var columnValue1 = new ColumnValue (_column1, _value1);
      var columnValue2 = new ColumnValue (_column2, _value2);
      var specification = new ComparedColumnsSpecification (new[] { columnValue1, columnValue2 });

      _sqlDialectStub.Stub (stub => stub.GetParameterName ("First")).Return ("pFirst");
      _sqlDialectStub.Stub (stub => stub.GetParameterName ("Second")).Return ("pSecond");

      var parameterStrictMock1 = MockRepository.GenerateStrictMock<IDbDataParameter> ();
      parameterStrictMock1.Expect (mock => mock.ParameterName = "pFirst");
      parameterStrictMock1.Replay ();

      var parameterStrictMock2 = MockRepository.GenerateStrictMock<IDbDataParameter> ();
      parameterStrictMock2.Expect (mock => mock.ParameterName = "pSecond");
      parameterStrictMock2.Replay ();

      _parametersCollectionMock.Expect (mock => mock.Add (parameterStrictMock1)).Return (0);
      _parametersCollectionMock.Expect (mock => mock.Add (parameterStrictMock2)).Return (1);
      _parametersCollectionMock.Replay ();

      _storageTypeInformationMock1
          .Expect (mock => mock.CreateDataParameter (_commandStub, _value1))
          .Return (parameterStrictMock1);
      _storageTypeInformationMock1.Replay ();

      _storageTypeInformationMock2
          .Expect (mock => mock.CreateDataParameter (_commandStub, _value2))
          .Return (parameterStrictMock2);
      _storageTypeInformationMock2.Replay ();

      specification.AddParameters (_commandStub, _sqlDialectStub, _parameterCache);

      _parametersCollectionMock.VerifyAllExpectations ();
      parameterStrictMock1.VerifyAllExpectations ();
      parameterStrictMock2.VerifyAllExpectations ();
      _storageTypeInformationMock1.VerifyAllExpectations ();
      _storageTypeInformationMock2.VerifyAllExpectations ();
    }

    [Test]
    public void AppendComparisons_OneValue ()
    {
      var specification = new ComparedColumnsSpecification (new[] { new ColumnValue(_column1, _value1) });

      _statement.Append ("<existingtext>");

      _sqlDialectStub.Stub (stub => stub.GetParameterName ("First")).Return ("pFirst");

      var parameterStrictMock = MockRepository.GenerateStrictMock<IDbDataParameter> ();
      parameterStrictMock.Expect (mock => mock.ParameterName = "pFirst");
      parameterStrictMock.Replay();

      _parametersCollectionMock.Expect (mock => mock.Add (parameterStrictMock)).Return (0);
      _parametersCollectionMock.Replay();

      _storageTypeInformationMock1
          .Expect (mock => mock.CreateDataParameter (_commandStub, _value1))
          .Return (parameterStrictMock);
      _storageTypeInformationMock1.Replay();

      _sqlDialectStub.Stub (mock => mock.DelimitIdentifier ("First")).Return ("[First]");

      specification.AppendComparisons (_statement, _commandStub, _sqlDialectStub, null);

      _parametersCollectionMock.VerifyAllExpectations();
      parameterStrictMock.VerifyAllExpectations ();
      _storageTypeInformationMock1.VerifyAllExpectations();

      Assert.That (_statement.ToString(), Is.EqualTo ("<existingtext>[First] = pFirst"));
    }

    [Test]
    public void AppendComparisons_MultipleValues_EmptyParameterDictionary ()
    {
      var columnValue1 = new ColumnValue(_column1, _value1);
      var columnValue2 = new ColumnValue(_column2, _value2);
      var specification = new ComparedColumnsSpecification (new[] { columnValue1, columnValue2 });

      _statement.Append ("<existingtext>");

      _sqlDialectStub.Stub (stub => stub.GetParameterName ("First")).Return ("pFirst");
      _sqlDialectStub.Stub (stub => stub.GetParameterName ("Second")).Return ("pSecond");
      
      var parameterStrictMock1 = MockRepository.GenerateStrictMock<IDbDataParameter> ();
      parameterStrictMock1.Expect (mock => mock.ParameterName = "pFirst");
      parameterStrictMock1.Replay ();

      var parameterStrictMock2 = MockRepository.GenerateStrictMock<IDbDataParameter> ();
      parameterStrictMock2.Expect (mock => mock.ParameterName = "pSecond");
      parameterStrictMock2.Replay ();

      _parametersCollectionMock.Expect (mock => mock.Add (parameterStrictMock1)).Return (0);
      _parametersCollectionMock.Expect (mock => mock.Add (parameterStrictMock2)).Return (1);
      _parametersCollectionMock.Replay ();

      _storageTypeInformationMock1
          .Expect (mock => mock.CreateDataParameter (_commandStub, _value1))
          .Return (parameterStrictMock1);
      _storageTypeInformationMock1.Replay ();

      _storageTypeInformationMock2
          .Expect (mock => mock.CreateDataParameter (_commandStub, _value2))
          .Return (parameterStrictMock2);
      _storageTypeInformationMock2.Replay ();

      _sqlDialectStub.Stub (mock => mock.DelimitIdentifier ("First")).Return ("[First]");
      _sqlDialectStub.Stub (mock => mock.DelimitIdentifier ("Second")).Return ("[Second]");

      specification.AppendComparisons (_statement, _commandStub, _sqlDialectStub, _parameterCache);

      _parametersCollectionMock.VerifyAllExpectations ();
      parameterStrictMock1.VerifyAllExpectations ();
      parameterStrictMock2.VerifyAllExpectations ();
      _storageTypeInformationMock1.VerifyAllExpectations ();
      _storageTypeInformationMock2.VerifyAllExpectations ();
      Assert.That (_parameterCache.Count, Is.EqualTo (2));
      Assert.That (_parameterCache[columnValue1], Is.SameAs (parameterStrictMock1));
      Assert.That (_parameterCache[columnValue2], Is.SameAs (parameterStrictMock2));
      Assert.That (_statement.ToString (), Is.EqualTo ("<existingtext>[First] = pFirst AND [Second] = pSecond"));
    }

    [Test]
    public void AppendComparisons_GetParameterFromDictionary ()
    {
      var parameterStrictMock = MockRepository.GenerateStrictMock<IDbDataParameter> ();
      var columnValue = new ColumnValue (_column1, _value1);
      var specification = new ComparedColumnsSpecification (new[] { columnValue });

      _sqlDialectStub.Stub (stub => stub.GetParameterName ("First")).Return ("pParameter");

      _storageTypeInformationMock1.Replay();
      _parametersCollectionMock.Replay ();
      _parameterCache.Add (columnValue, parameterStrictMock);
      _sqlDialectStub.Stub (mock => mock.DelimitIdentifier ("First")).Return ("[First]");

      specification.AppendComparisons (_statement, _commandStub, _sqlDialectStub, _parameterCache);

      _storageTypeInformationMock1.VerifyAllExpectations();
      _parametersCollectionMock.VerifyAllExpectations ();
      parameterStrictMock.VerifyAllExpectations ();
      Assert.That (_statement.ToString (), Is.EqualTo ("[First] = pParameter"));
    }

  }
}