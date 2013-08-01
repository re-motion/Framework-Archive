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
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.UnitTests.Globalization.TestDomain;
using Rhino.Mocks;

namespace Remotion.UnitTests.Globalization
{
  [TestFixture]
  public class MemberInformationGlobalizationServiceTest
  {
    private IGlobalizationService _globalizationServiceMock1;
    private IGlobalizationService _globalizationServiceMock2;
    private MemberInformationGlobalizationService _service;
    private ITypeInformation _typeInformationStub;
    private IPropertyInformation _propertyInformationStub;
    private IResourceManager _resourceManager1Mock;
    private IResourceManager _resourceManager2Mock;
    private IMemberInfoNameResolver _memberInfoNameResolverStub;
    private string _shortPropertyResourceID;
    private string _longPropertyResourceID;
    private string _shortTypeResourceID;
    private string _longTypeResourceID;

    [SetUp]
    public void SetUp ()
    {
      _globalizationServiceMock1 = MockRepository.GenerateStrictMock<IGlobalizationService>();
      _globalizationServiceMock2 = MockRepository.GenerateStrictMock<IGlobalizationService>();
      _resourceManager1Mock = MockRepository.GenerateStrictMock<IResourceManager>();
      _resourceManager1Mock.Stub (stub => stub.IsNull).Return (false);
      _resourceManager1Mock.Stub (stub => stub.Name).Return ("RM1");
      _resourceManager2Mock = MockRepository.GenerateStrictMock<IResourceManager>();
      _resourceManager2Mock.Stub (stub => stub.IsNull).Return (false);
      _resourceManager2Mock.Stub (stub => stub.Name).Return ("RM2");

      _typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      _typeInformationStub.Stub (stub => stub.Name).Return ("TypeName");

      _propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      _propertyInformationStub.Stub (stub => stub.Name).Return ("PropertyName");

      _memberInfoNameResolverStub = MockRepository.GenerateStub<IMemberInfoNameResolver>();
      _memberInfoNameResolverStub.Stub (stub => stub.GetPropertyName (_propertyInformationStub)).Return ("FakePropertyFullName");
      _memberInfoNameResolverStub.Stub (stub => stub.GetTypeName (_typeInformationStub)).Return ("FakeTypeFullName");

      _shortPropertyResourceID = "property:PropertyName";
      _longPropertyResourceID = "property:FakePropertyFullName";
      _shortTypeResourceID = "type:TypeName";
      _longTypeResourceID = "type:FakeTypeFullName";

      _service = new MemberInformationGlobalizationService (
          new[] { _globalizationServiceMock1, _globalizationServiceMock2 }, _memberInfoNameResolverStub);
    }

    [Test]
    public void GetPropertyDisplayName_NoResourceFound ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager1Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager2Mock);

      _resourceManager1Mock.Expect (mock => mock.ContainsResource (_longPropertyResourceID)).Return (false);
      _resourceManager1Mock.Expect (mock => mock.ContainsResource (_shortPropertyResourceID)).Return (false);
      _resourceManager2Mock.Expect (mock => mock.ContainsResource (_longPropertyResourceID)).Return (false);
      _resourceManager2Mock.Expect (mock => mock.ContainsResource (_shortPropertyResourceID)).Return (false);

      var result = _service.GetPropertyDisplayName (_propertyInformationStub, _typeInformationStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("PropertyName"));
    }

    [Test]
    public void GetPropertyDisplayName_ResourceFoundByFirstResourceManager_LongResourceID ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager1Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager2Mock);

      _resourceManager1Mock.Expect (mock => mock.ContainsResource (_longPropertyResourceID)).Return (true);
      _resourceManager1Mock.Expect (mock => mock.GetString (_longPropertyResourceID)).Return ("FakeResultForLongPropertyName1");
      _resourceManager2Mock.Stub (stub => stub.ContainsResource (_longPropertyResourceID)).Return (false);

      var result = _service.GetPropertyDisplayName (_propertyInformationStub, _typeInformationStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("FakeResultForLongPropertyName1"));
    }

    [Test]
    public void GetPropertyDisplayName_ResourceFoundBySecondResourceManager_LongResourceID ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager1Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager2Mock);

      _resourceManager1Mock.Expect (mock => mock.ContainsResource (_longPropertyResourceID)).Return (false);
      _resourceManager2Mock.Expect (mock => mock.ContainsResource (_longPropertyResourceID)).Return (true);
      _resourceManager1Mock.Expect (mock => mock.GetString (_longPropertyResourceID)).Return ("FakeResultForLongPropertyName2");

      var result = _service.GetPropertyDisplayName (_propertyInformationStub, _typeInformationStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("FakeResultForLongPropertyName2"));
    }

    [Test]
    public void GetPropertyDisplayName_ResourceFoundByBothResourceManager_LongResourceID ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager1Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager2Mock);

      _resourceManager1Mock.Expect (mock => mock.ContainsResource (_longPropertyResourceID)).Return (true);
      _resourceManager1Mock.Expect (mock => mock.GetString (_longPropertyResourceID)).Return ("FakeResultForLongPropertyName1");
      _resourceManager2Mock.Stub (stub => stub.ContainsResource (_longPropertyResourceID)).Return (true);

      var result = _service.GetPropertyDisplayName (_propertyInformationStub, _typeInformationStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("FakeResultForLongPropertyName1"));
    }

    [Test]
    public void GetPropertyDisplayName_ResourceFoundByFirstResourceManager_ShortResourceID ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager1Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager2Mock);

      _resourceManager1Mock.Expect (mock => mock.ContainsResource (_longPropertyResourceID)).Return (false);
      _resourceManager1Mock.Expect (mock => mock.ContainsResource (_shortPropertyResourceID)).Return (true);
      _resourceManager1Mock.Expect (mock => mock.GetString (_shortPropertyResourceID)).Return ("FakeResultForShortPropertyName1");
      _resourceManager2Mock.Expect (mock => mock.ContainsResource (_longPropertyResourceID)).Return (false);

      var result = _service.GetPropertyDisplayName (_propertyInformationStub, _typeInformationStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("FakeResultForShortPropertyName1"));
    }

    [Test]
    public void GetPropertyDisplayName_ResourceFoundBySecondResourceManager_ShortResourceID ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager1Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager2Mock);

      _resourceManager1Mock.Expect (mock => mock.ContainsResource (_longPropertyResourceID)).Return (false);
      _resourceManager1Mock.Expect (mock => mock.ContainsResource (_shortPropertyResourceID)).Return (false);
      _resourceManager1Mock.Expect (mock => mock.GetString (_shortPropertyResourceID)).Return (null);
      _resourceManager2Mock.Expect (mock => mock.ContainsResource (_longPropertyResourceID)).Return (false);
      _resourceManager2Mock.Expect (mock => mock.GetString (_shortPropertyResourceID)).Return ("FakeResultForShortPropertyName2");
      _resourceManager2Mock.Expect (mock => mock.ContainsResource (_shortPropertyResourceID)).Return (true);

      var result = _service.GetPropertyDisplayName (_propertyInformationStub, _typeInformationStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("FakeResultForShortPropertyName2"));
    }

    [Test]
    public void GetPropertyDisplayName_ResourceFoundByBothResourceManager_LongAndShortResourceID_ReturnedByLongResourceID ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager1Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager2Mock);

      _resourceManager1Mock.Expect (mock => mock.ContainsResource (_longPropertyResourceID)).Return (true);
      _resourceManager1Mock.Stub (stub => stub.ContainsResource (_shortPropertyResourceID)).Return (true);
      _resourceManager1Mock.Expect (mock => mock.GetString (_longPropertyResourceID)).Return ("FakeResultForLongPropertyName1");
      _resourceManager2Mock.Stub (stub => stub.ContainsResource (_longPropertyResourceID)).Return (true);
      _resourceManager2Mock.Stub (stub => stub.ContainsResource (_shortPropertyResourceID)).Return (true);

      var result = _service.GetPropertyDisplayName (_propertyInformationStub, _typeInformationStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("FakeResultForLongPropertyName1"));
    }

    [Test]
    public void GetTypeDisplayName_NoResourceFound ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager1Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager2Mock);

      _resourceManager1Mock.Expect (mock => mock.ContainsResource (_longTypeResourceID)).Return (false);
      _resourceManager1Mock.Expect (mock => mock.ContainsResource (_shortTypeResourceID)).Return (false);
      _resourceManager2Mock.Expect (mock => mock.ContainsResource (_longTypeResourceID)).Return (false);
      _resourceManager2Mock.Expect (mock => mock.ContainsResource (_shortTypeResourceID)).Return (false);

      var result = _service.GetTypeDisplayName (_typeInformationStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations ();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("TypeName"));
    }

    [Test]
    public void GetTypeDisplayName_ResourceFoundByLongResourceID ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager1Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager2Mock);

      _resourceManager1Mock.Expect (mock => mock.ContainsResource (_longTypeResourceID)).Return (true);
      _resourceManager1Mock.Expect (mock => mock.GetString (_longTypeResourceID)).Return ("FakeResultForLongTypeName");

      var result = _service.GetTypeDisplayName (_typeInformationStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations ();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("FakeResultForLongTypeName"));
    }

    [Test]
    public void GetTypeDisplayName_ResourceFoundByShortResourceID ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager1Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager2Mock);

      _resourceManager1Mock.Expect (mock => mock.ContainsResource (_longTypeResourceID)).Return (false);
      _resourceManager1Mock.Expect (mock => mock.ContainsResource (_shortTypeResourceID)).Return (true);
      _resourceManager1Mock.Expect (mock => mock.GetString (_shortTypeResourceID)).Return ("FakeResultForShortTypeName");
      _resourceManager2Mock.Expect (mock => mock.ContainsResource (_longTypeResourceID)).Return (false);
      
      var result = _service.GetTypeDisplayName (_typeInformationStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations ();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("FakeResultForShortTypeName"));
    }

    [Test]
    public void GetTypeDisplayName_ResourceFoundByLongAndShortResourceID_ReturnedByLongResourceID ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager1Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager2Mock);

      _resourceManager1Mock.Expect (mock => mock.ContainsResource (_longTypeResourceID)).Return (true);
      _resourceManager1Mock.Stub (stub => stub.ContainsResource (_shortTypeResourceID)).Return (true);
      _resourceManager1Mock.Expect (mock => mock.GetString (_longTypeResourceID)).Return ("FakeResultForLongTypeName");

      var result = _service.GetTypeDisplayName (_typeInformationStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations ();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("FakeResultForLongTypeName"));
    }

    [Test]
    public void GetEnumerationValueDisplayName ()
    {
      Assert.That (_service.GetEnumerationValueDisplayName (EnumWithResources.Value1), Is.EqualTo ("Value 1"));
      Assert.That (_service.GetEnumerationValueDisplayName (EnumWithResources.Value2), Is.EqualTo ("Value 2"));
      Assert.That (_service.GetEnumerationValueDisplayName (EnumWithResources.ValueWithoutResource), Is.EqualTo ("ValueWithoutResource"));
    }

    [Test]
    public void GetEnumerationValueDisplayName_WithDescription ()
    {
      Assert.That (_service.GetEnumerationValueDisplayName (EnumWithDescription.Value1), Is.EqualTo ("Value I"));
      Assert.That (_service.GetEnumerationValueDisplayName (EnumWithDescription.Value2), Is.EqualTo ("Value II"));
      Assert.That (
          _service.GetEnumerationValueDisplayName (EnumWithDescription.ValueWithoutDescription),
          Is.EqualTo ("ValueWithoutDescription"));
    }

    [Test]
    public void GetEnumerationValueDisplayName_WithoutResources ()
    {
      Assert.That (_service.GetEnumerationValueDisplayName (TestEnum.Value1), Is.EqualTo ("Value1"));
      Assert.That (_service.GetEnumerationValueDisplayName (TestEnum.Value2), Is.EqualTo ("Value2"));
      Assert.That (_service.GetEnumerationValueDisplayName (TestEnum.Value3), Is.EqualTo ("Value3"));
    }

    [Test]
    public void GetExtensibleEnumerationValueDisplayName ()
    {
      Assert.That (_service.GetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.Value1()), Is.EqualTo ("Wert1"));
      Assert.That (_service.GetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.Value2()), Is.EqualTo ("Wert2"));
      Assert.That (
          _service.GetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.ValueWithoutResource()),
          Is.EqualTo ("Remotion.UnitTests.Globalization.TestDomain.ExtensibleEnumWithResourcesExtensions.ValueWithoutResource"));
    }
  }
}