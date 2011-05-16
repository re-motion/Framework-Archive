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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class ScriptGeneratorTest : SchemaGenerationTestBase
  {
    private IEntityDefinition _firstProviderStorageEntityDefinitionStub;
    private IEntityDefinition _secondProviderStorageEntityDefinitionStub;
    private IEntityDefinition _thirdProviderStorageEntityDefinitionStub;
    private ClassDefinition _classDefinitionForFirstStorageProvider1;
    private ClassDefinition _classDefinitionForFirstStorageProvider2;
    private ClassDefinition _classDefinitionForSecondStorageProvider;
    private ClassDefinition _classDefinitionForThirdStorageProvider;
    private IEntityDefinitionProvider _entityDefininitionProviderMock;
    private IScriptToStringConverter _scriptToStringConverterStub;
    private ScriptGenerator _scriptGenerator;
    private IEntityDefinition _fakeEntityDefinition1;
    private IEntityDefinition _fakeEntityDefinition2;
    private IEntityDefinition _fakeEntityDefinition3;
    private ScriptPair _fakeScriptResult1;
    private ScriptPair _fakeScriptResult2;
    private ScriptPair _fakeScriptResult3;
    private IScriptBuilder _scriptBuilderForFirstStorageProviderMock;
    private IScriptBuilder _scriptBuilderForSecondStorageProviderMock;
    private IScriptBuilder _scriptBuilderForThirdStorageProviderMock;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinitionForFirstStorageProvider1 = ClassDefinitionFactory.CreateClassDefinition (typeof (Order));
      _classDefinitionForFirstStorageProvider2 = ClassDefinitionFactory.CreateClassDefinition (typeof (OrderItem));
      _classDefinitionForSecondStorageProvider = ClassDefinitionFactory.CreateClassDefinition (typeof (Order));
      _classDefinitionForThirdStorageProvider = ClassDefinitionFactory.CreateClassDefinition (typeof (Customer));

      _firstProviderStorageEntityDefinitionStub = MockRepository.GenerateStub<IEntityDefinition>();
      _firstProviderStorageEntityDefinitionStub.Stub (stub => stub.StorageProviderDefinition).Return (SchemaGenerationFirstStorageProviderDefinition);

      _secondProviderStorageEntityDefinitionStub = MockRepository.GenerateStub<IEntityDefinition>();
      _secondProviderStorageEntityDefinitionStub.Stub (stub => stub.StorageProviderDefinition).Return (
          SchemaGenerationSecondStorageProviderDefinition);

      _thirdProviderStorageEntityDefinitionStub = MockRepository.GenerateStub<IEntityDefinition>();
      _thirdProviderStorageEntityDefinitionStub.Stub (stub => stub.StorageProviderDefinition).Return (
          new UnitTestStorageProviderStubDefinition ("Test"));

      _classDefinitionForFirstStorageProvider1.SetStorageEntity (_firstProviderStorageEntityDefinitionStub);
      _classDefinitionForFirstStorageProvider2.SetStorageEntity (_firstProviderStorageEntityDefinitionStub);
      _classDefinitionForSecondStorageProvider.SetStorageEntity (_secondProviderStorageEntityDefinitionStub);
      _classDefinitionForThirdStorageProvider.SetStorageEntity (_thirdProviderStorageEntityDefinitionStub);

      _fakeScriptResult1 = new ScriptPair ("CreateScript1", "DropScript1");
      _fakeScriptResult2 = new ScriptPair ("CreateScript2", "DropScript2");
      _fakeScriptResult3 = new ScriptPair ("CreateScript3", "DropScript3");
      _entityDefininitionProviderMock = MockRepository.GenerateStrictMock<IEntityDefinitionProvider>();
      _scriptBuilderForFirstStorageProviderMock = MockRepository.GenerateStrictMock<IScriptBuilder>();
      _scriptBuilderForSecondStorageProviderMock = MockRepository.GenerateStrictMock<IScriptBuilder>();
      _scriptBuilderForThirdStorageProviderMock = MockRepository.GenerateStrictMock<IScriptBuilder>();

      _scriptToStringConverterStub = MockRepository.GenerateStub<IScriptToStringConverter>();
      _scriptToStringConverterStub.Stub (stub => stub.Convert (_scriptBuilderForFirstStorageProviderMock)).Return (_fakeScriptResult1);
      _scriptToStringConverterStub.Stub (stub => stub.Convert (_scriptBuilderForSecondStorageProviderMock)).Return (_fakeScriptResult2);
      _scriptToStringConverterStub.Stub (stub => stub.Convert (_scriptBuilderForThirdStorageProviderMock)).Return (_fakeScriptResult3);


      _scriptGenerator = new ScriptGenerator (
          pd =>
          {
            switch (pd.Name)
            {
              case "SchemaGenerationFirstStorageProvider":
                return _scriptBuilderForFirstStorageProviderMock;
              case "SchemaGenerationSecondStorageProvider":
                return _scriptBuilderForSecondStorageProviderMock;
              case "SchemaGenerationThirdStorageProvider":
                return _scriptBuilderForThirdStorageProviderMock;
            }
            throw new InvalidOperationException ("Invalid storage provider!");
          },
          _entityDefininitionProviderMock,
          _scriptToStringConverterStub);

      _fakeEntityDefinition1 = MockRepository.GenerateStub<IEntityDefinition>();
      _fakeEntityDefinition2 = MockRepository.GenerateStub<IEntityDefinition>();
      _fakeEntityDefinition3 = MockRepository.GenerateStub<IEntityDefinition>();
    }

    [Test]
    public void GetScripts_NoClassDefinitions ()
    {
      var result = _scriptGenerator.GetScripts (new ClassDefinition[0]);

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void GetScript_ClassDefinitionWithoutRdbmsStorageProviderDefinition ()
    {
      var result = _scriptGenerator.GetScripts (new[] { _classDefinitionForThirdStorageProvider });

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void GetScripts_OneClassDefinitionWithRdbmsStorageProviderDefinition ()
    {
      _entityDefininitionProviderMock
          .Expect (
              mock => mock.GetEntityDefinitions (Arg<IEnumerable<ClassDefinition>>.List.Equal (new[] { _classDefinitionForSecondStorageProvider })))
          .Return (new[] { _fakeEntityDefinition1 });
      _entityDefininitionProviderMock.Replay();

      _scriptBuilderForSecondStorageProviderMock.Expect (mock => mock.AddEntityDefinition (_fakeEntityDefinition1));
      _scriptBuilderForSecondStorageProviderMock.Replay();

      var result = _scriptGenerator.GetScripts (new[] { _classDefinitionForSecondStorageProvider }).ToList();

      _entityDefininitionProviderMock.VerifyAllExpectations();
      _scriptBuilderForSecondStorageProviderMock.VerifyAllExpectations();
      Assert.That (result.Count, Is.EqualTo (1));
      Assert.That (result[0].StorageProviderDefinition, Is.SameAs (SchemaGenerationSecondStorageProviderDefinition));
      Assert.That (result[0].SetUpScript, Is.EqualTo ("CreateScript2"));
      Assert.That (result[0].TearDownScript, Is.EqualTo ("DropScript2"));
    }

    [Test]
    public void GetScripts_SeveralClassDefinitionWithSameRdbmsStorageProviderDefinitions ()
    {
      _entityDefininitionProviderMock
          .Expect (
              mock =>
              mock.GetEntityDefinitions (
                  Arg<IEnumerable<ClassDefinition>>.List.Equal (
                      new[] { _classDefinitionForFirstStorageProvider1, _classDefinitionForFirstStorageProvider2 })))
          .Return (new[] { _fakeEntityDefinition1, _fakeEntityDefinition2 });
      _entityDefininitionProviderMock.Replay();

      _scriptBuilderForFirstStorageProviderMock.Expect (mock => mock.AddEntityDefinition (_fakeEntityDefinition1));
      _scriptBuilderForFirstStorageProviderMock.Expect (mock => mock.AddEntityDefinition (_fakeEntityDefinition2));
      _scriptBuilderForFirstStorageProviderMock.Replay();

      var result = _scriptGenerator.GetScripts (new[] { _classDefinitionForFirstStorageProvider1, _classDefinitionForFirstStorageProvider2 }).ToList();

      _entityDefininitionProviderMock.VerifyAllExpectations();
      _scriptBuilderForFirstStorageProviderMock.VerifyAllExpectations();
      Assert.That (result.Count, Is.EqualTo (1));
      Assert.That (result[0].StorageProviderDefinition, Is.SameAs (SchemaGenerationFirstStorageProviderDefinition));
      Assert.That (result[0].SetUpScript, Is.EqualTo ("CreateScript1"));
      Assert.That (result[0].TearDownScript, Is.EqualTo ("DropScript1"));
    }

    [Test]
    public void GetScripts_SeveralClassDefinitionWithDifferentRdbmsStorageProviderDefinitions ()
    {
      _entityDefininitionProviderMock
          .Expect (
              mock =>
              mock.GetEntityDefinitions (
                  Arg<IEnumerable<ClassDefinition>>.List.Equal (
                      new[] { _classDefinitionForFirstStorageProvider1, _classDefinitionForFirstStorageProvider2 })))
          .Return (new[] { _fakeEntityDefinition1, _fakeEntityDefinition2 });
      _entityDefininitionProviderMock
          .Expect (
              mock =>
              mock.GetEntityDefinitions (
                  Arg<IEnumerable<ClassDefinition>>.List.Equal (
                      new[] { _classDefinitionForSecondStorageProvider })))
          .Return (new[] { _fakeEntityDefinition3 });
      _entityDefininitionProviderMock.Replay();

      _scriptBuilderForFirstStorageProviderMock.Expect (mock => mock.AddEntityDefinition (_fakeEntityDefinition1));
      _scriptBuilderForFirstStorageProviderMock.Expect (mock => mock.AddEntityDefinition (_fakeEntityDefinition2));
      _scriptBuilderForFirstStorageProviderMock.Replay();

      _scriptBuilderForSecondStorageProviderMock.Expect (mock => mock.AddEntityDefinition (_fakeEntityDefinition3));
      _scriptBuilderForSecondStorageProviderMock.Replay();

      var result =
          _scriptGenerator.GetScripts (
              new[]
              {
                  _classDefinitionForFirstStorageProvider1, 
                  _classDefinitionForFirstStorageProvider2, 
                  _classDefinitionForSecondStorageProvider,
                  _classDefinitionForThirdStorageProvider
              }).ToList();

      _entityDefininitionProviderMock.VerifyAllExpectations();
      _scriptBuilderForFirstStorageProviderMock.VerifyAllExpectations();
      _scriptBuilderForSecondStorageProviderMock.VerifyAllExpectations();
      Assert.That (result.Count, Is.EqualTo (2));
      Assert.That (result[0].StorageProviderDefinition, Is.SameAs (SchemaGenerationFirstStorageProviderDefinition));
      Assert.That (result[0].SetUpScript, Is.EqualTo ("CreateScript1"));
      Assert.That (result[0].TearDownScript, Is.EqualTo ("DropScript1"));
      Assert.That (result[1].StorageProviderDefinition, Is.SameAs (SchemaGenerationSecondStorageProviderDefinition));
      Assert.That (result[1].SetUpScript, Is.EqualTo ("CreateScript2"));
      Assert.That (result[1].TearDownScript, Is.EqualTo ("DropScript2"));
    }
  }
}