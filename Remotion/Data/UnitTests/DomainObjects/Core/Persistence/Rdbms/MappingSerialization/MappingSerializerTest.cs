﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingSerialization;
using Remotion.FunctionalProgramming;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.MappingSerialization
{
  [TestFixture]
  public class MappingSerializerTest : StandardMappingTest  
  {
    private IEnumSerializer _enumSerializerStub;
    private IStorageProviderSerializer _storageProviderSerializerStub;

    public override void SetUp ()
    {
      base.SetUp();
      _enumSerializerStub = MockRepository.GenerateStub<IEnumSerializer>();
      _storageProviderSerializerStub = MockRepository.GenerateStub<IStorageProviderSerializer>();
    }

    [Test]
    public void Serialize_CreatesXDocument ()
    {
      var mappingSerializer = new MappingSerializer(_storageProviderSerializerStub, _enumSerializerStub);
      var actual = mappingSerializer.Serialize();

      Assert.That (actual.Root.Name.LocalName, Is.EqualTo ("mapping"));
    }

    [Test]
    public void Serialize_AddsStorageProviderElements ()
    {
      var mappingSerializer = new MappingSerializer(_storageProviderSerializerStub, _enumSerializerStub);

      var expected = new[] { new XElement ("storageProvider") };
      _storageProviderSerializerStub.Stub (s => s.Serialize (Arg<IEnumerable<ClassDefinition>>.Is.NotNull, Arg<EnumTypeCollection>.Is.NotNull))
          .Return (expected);

      var actual = mappingSerializer.Serialize();

      Assert.That (actual.Root.Elements().ToArray(), Is.EqualTo (expected));
    }

    [Test]
    public void Serialize_AddsEnumTypes ()
    {
      var storageProviderSerializerStub = MockRepository.GenerateStub<IStorageProviderSerializer>();
      var mappingSerializer = new MappingSerializer (storageProviderSerializerStub, _enumSerializerStub);

      var storageProviderElement = new XElement ("storageProvider");
      storageProviderSerializerStub.Stub (s => s.Serialize (Arg<IEnumerable<ClassDefinition>>.Is.NotNull, Arg<EnumTypeCollection>.Is.NotNull))
          .Return (new[] { storageProviderElement });

      var enumTypeElement = new XElement ("enumType");
      _enumSerializerStub.Stub (s => s.Serialize (Arg<EnumTypeCollection>.Is.NotNull))
          .Return (new[] { enumTypeElement });

      var actual = mappingSerializer.Serialize();

      Assert.That (actual.Root.Elements().ToArray(), Is.EqualTo (new[] { storageProviderElement, enumTypeElement }));
    }

    [Test]
    [Ignore]
    public void Serialize_IntegrationTest ()
    {
      var mappingSerializer = new MappingSerializer (
          new StorageProviderSerializer (
              new ClassSerializer (new TableSerializer (new PropertySerializer (new ColumnSerializer())))),
          new EnumSerializer());

      var actual = mappingSerializer.Serialize();
      var xml = actual.ToString();

    }
  }
}