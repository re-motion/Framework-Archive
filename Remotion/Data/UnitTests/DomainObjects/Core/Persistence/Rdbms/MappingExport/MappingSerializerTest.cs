// This file is part of the re-motion Core Framework (www.re-motion.org)
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

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.MappingExport
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
      _storageProviderSerializerStub.Stub (_ => _.Serialize (null)).IgnoreArguments().Return (new XElement[0]);
      _enumSerializerStub.Stub (_ => _.Serialize()).IgnoreArguments().Return (new XElement[0]);
      var mappingSerializer = new MappingSerializer (_storageProviderSerializerStub, _enumSerializerStub);
      var actual = mappingSerializer.Serialize (MappingConfiguration.Current.GetTypeDefinitions());

      Assert.That (actual.Root.Name.LocalName, Is.EqualTo ("mapping"));
    }

    [Test]
    public void Serialize_AddsStorageProviderElements ()
    {
      _enumSerializerStub.Stub (_ => _.Serialize()).IgnoreArguments().Return (new XElement[0]);
      var mappingSerializer = new MappingSerializer (_storageProviderSerializerStub, _enumSerializerStub);

      var expected = new[] { new XElement ("storageProvider") };
      _storageProviderSerializerStub
          .Stub (s => s.Serialize (Arg<IEnumerable<ClassDefinition>>.Is.NotNull))
          .Return (expected);

      var actual = mappingSerializer.Serialize (MappingConfiguration.Current.GetTypeDefinitions());

      Assert.That (actual.Root.Elements(), Is.EqualTo (expected));
    }

    [Test]
    public void Serialize_AddsEnumTypes ()
    {
      var mappingSerializer = new MappingSerializer (_storageProviderSerializerStub, _enumSerializerStub);

      var storageProviderElement = new XElement ("storageProvider");
      _storageProviderSerializerStub.Stub (s => s.Serialize (Arg<IEnumerable<ClassDefinition>>.Is.NotNull))
          .Return (new[] { storageProviderElement });

      var enumTypeElement = new XElement ("enumType");
      _enumSerializerStub
          .Stub (s => s.Serialize())
          .Return (new[] { enumTypeElement });

      var actual = mappingSerializer.Serialize (MappingConfiguration.Current.GetTypeDefinitions());

      Assert.That (actual.Root.Elements(), Is.EqualTo (new[] { storageProviderElement, enumTypeElement }));
    }
  }
}