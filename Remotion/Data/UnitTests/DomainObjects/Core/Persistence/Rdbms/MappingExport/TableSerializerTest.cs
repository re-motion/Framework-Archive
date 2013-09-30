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
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.TableInheritance;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.MappingExport
{
  [TestFixture]
  public class TableSerializerTest : StandardMappingTest
  {
    private EnumTypeCollection _enumTypeCollection;

    public override void SetUp ()
    {
      base.SetUp();
      _enumTypeCollection = new EnumTypeCollection();
    }

    [Test]
    public void Serialize_CreatesTableElement ()
    {
      var tableSerializer = new TableSerializer (MockRepository.GenerateStub<IPropertySerializer>());

      var actual =
          tableSerializer.Serialize (MappingConfiguration.Current.GetTypeDefinition (typeof (ClassWithAllDataTypes)), _enumTypeCollection).Single();

      Assert.That (actual.Name.LocalName, Is.EqualTo ("table"));
      Assert.That (actual.Attributes().Select (a => a.Name.LocalName), Contains.Item ("name"));
      Assert.That (actual.Attribute ("name").Value, Is.EqualTo ("TableWithAllDataTypes"));
    }

    [Test]
    public void Serialize_CreatesPersistenceModelProvider ()
    {
      var propertySerializerMock = MockRepository.GenerateMock<IPropertySerializer>();
      var tableSerializer = new TableSerializer (propertySerializerMock);

      propertySerializerMock.Expect (
          s => s.Serialize (
              Arg<PropertyDefinition>.Is.NotNull,
              Arg<IRdbmsPersistenceModelProvider>.Is.NotNull,
              Arg<EnumTypeCollection>.Is.Same (_enumTypeCollection)))
          .Return (null)
          .Repeat.AtLeastOnce();

      propertySerializerMock.Replay();
      tableSerializer.Serialize (MappingConfiguration.Current.GetTypeDefinition (typeof (ClassWithAllDataTypes)), _enumTypeCollection).ToArray();
      propertySerializerMock.VerifyAllExpectations();
    }

    [Test]
    public void Serialize_AddsPropertyElements ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Ceo));
      var propertySerializerStub = MockRepository.GenerateStub<IPropertySerializer>();
      var expected1 = new XElement ("property1");
      var expected2 = new XElement ("property2");

      propertySerializerStub
          .Stub (
              _ => _.Serialize (
                  Arg.Is (classDefinition.GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Ceo.Name")),
                  Arg<IRdbmsPersistenceModelProvider>.Is.Anything,
                  Arg.Is (_enumTypeCollection)))
          .Return (expected1);
      propertySerializerStub
          .Stub (
              _ => _.Serialize (
                  Arg.Is (classDefinition.GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Ceo.Company")),
                  Arg<IRdbmsPersistenceModelProvider>.Is.Anything,
                  Arg.Is (_enumTypeCollection)))
          .Return (expected2);
      var tableSerializer = new TableSerializer (propertySerializerStub);

      var actual = tableSerializer.Serialize (classDefinition, _enumTypeCollection).Single();

      Assert.That (actual.Elements(), Is.EqualTo (new[] { expected1, expected2 }));
    }

    [Test]
    public void Serialize_OnlyAddsPersistentProperties ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Computer));
      var propertySerializerMock = MockRepository.GenerateStrictMock<IPropertySerializer>();

      Expression<Predicate<PropertyDefinition>> propertyDefinitionConstraint =
          p => p.StorageClass == StorageClass.Persistent;

      propertySerializerMock.Expect (
          s => s.Serialize (
              Arg<PropertyDefinition>.Matches (propertyDefinitionConstraint),
              Arg<IRdbmsPersistenceModelProvider>.Is.Anything,
              Arg<EnumTypeCollection>.Is.Anything))
          .Return (new XElement ("property"))
          .Repeat.AtLeastOnce();

      var tableSerializer = new TableSerializer (propertySerializerMock);

      propertySerializerMock.Replay();
      tableSerializer.Serialize (classDefinition, _enumTypeCollection).ToArray();
      propertySerializerMock.VerifyAllExpectations();
    }

    [Test]
    public void Serialize_AbstractDerivedClass_CreatesTableElementFromBaseClass ()
    {
      var tableSerializer = new TableSerializer (MockRepository.GenerateStub<IPropertySerializer>());

      var actual =
          tableSerializer.Serialize (
              MappingConfiguration.Current.GetTypeDefinition (typeof (DerivedClassWithEntityFromBaseClassWithHierarchy)),
              _enumTypeCollection).Single();

      Assert.That (actual.Name.LocalName, Is.EqualTo ("table"));
      Assert.That (actual.Attributes().Select (a => a.Name.LocalName), Contains.Item ("name"));
      Assert.That (actual.Attribute ("name").Value, Is.EqualTo ("TableInheritance_DerivedClassWithEntityWithHierarchy"));
    }
  }
}