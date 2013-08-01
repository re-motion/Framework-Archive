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
using Rhino.Mocks;

namespace Remotion.UnitTests.Globalization
{
  [TestFixture]
  public class MemberInformationGlobalizationServiceTest
  {
    private IGlobalizationService _globalizationServiceMock;
    private MemberInformationGlobalizationService _service;
    private ITypeInformation _typeInformationStub;
    private IPropertyInformation _propertyInformationStub;
    private IResourceManager _resourceManagerMock;
    private string _shortPropertyResourceID;
    private string _longPropertyResourceID;
    private string _shortTypeResourceID;
    private string _longTypeResourceID;

    [SetUp]
    public void SetUp ()
    {
      _globalizationServiceMock = MockRepository.GenerateStrictMock<IGlobalizationService> ();
      _resourceManagerMock = MockRepository.GenerateStrictMock<IResourceManager> ();

      _typeInformationStub = MockRepository.GenerateStub<ITypeInformation> ();
      _typeInformationStub.Stub (stub => stub.Name).Return ("TypeName");
      _typeInformationStub.Stub (stub => stub.FullName).Return ("TypeFullName");

      _propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation> ();
      _propertyInformationStub.Stub (stub => stub.Name).Return ("PropertyName");

      _shortPropertyResourceID = "property:" + _propertyInformationStub.Name;
      _longPropertyResourceID = "property:" + _typeInformationStub.FullName + "." + _propertyInformationStub.Name;
      _shortTypeResourceID = "type:" + _typeInformationStub.Name;
      _longTypeResourceID = "type:" + _typeInformationStub.FullName;

      _service = new MemberInformationGlobalizationService (_globalizationServiceMock);
    }

    [Test]
    public void GetPropertyDisplayName_NoResourceFound ()
    {
      _globalizationServiceMock.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManagerMock);
      _resourceManagerMock.Expect (mock => mock.ContainsResource (_longPropertyResourceID)).Return (false);
      _resourceManagerMock.Expect (mock => mock.ContainsResource (_shortPropertyResourceID)).Return (false);

      var result = _service.GetPropertyDisplayName (_propertyInformationStub, _typeInformationStub);

      _globalizationServiceMock.VerifyAllExpectations();
      _resourceManagerMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("PropertyName"));
    }

    [Test]
    public void GetPropertyDisplayName_ResourceFoundByLongResourceID ()
    {
      _globalizationServiceMock.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManagerMock);
      _resourceManagerMock.Expect (mock => mock.ContainsResource (_longPropertyResourceID)).Return (true);
      _resourceManagerMock.Expect (mock => mock.GetString (_longPropertyResourceID)).Return ("FakeResultForLongPropertyName");

      var result = _service.GetPropertyDisplayName (_propertyInformationStub, _typeInformationStub);

      _globalizationServiceMock.VerifyAllExpectations ();
      _resourceManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("FakeResultForLongPropertyName"));
    }

    [Test]
    public void GetPropertyDisplayName_ResourceFoundByShortResourceID ()
    {
      _globalizationServiceMock.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManagerMock);
      _resourceManagerMock.Expect (mock => mock.ContainsResource (_longPropertyResourceID)).Return (false);
      _resourceManagerMock.Expect (mock => mock.ContainsResource (_shortPropertyResourceID)).Return (true);
      _resourceManagerMock.Expect (mock => mock.GetString (_shortPropertyResourceID)).Return ("FakeResultForShortPropertyName");

      var result = _service.GetPropertyDisplayName (_propertyInformationStub, _typeInformationStub);

      _globalizationServiceMock.VerifyAllExpectations ();
      _resourceManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("FakeResultForShortPropertyName"));
    }

    [Test]
    public void GetPropertyDisplayName_ResourceFoundByLongAndShortResourceID_ReturnedByLongResourceID ()
    {
      _globalizationServiceMock.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManagerMock);
      _resourceManagerMock.Expect (mock => mock.ContainsResource (_longPropertyResourceID)).Return (true);
      _resourceManagerMock.Stub (stub => stub.ContainsResource (_shortPropertyResourceID)).Return (true);
      _resourceManagerMock.Expect (mock => mock.GetString (_longPropertyResourceID)).Return ("FakeResultForLongPropertyName");

      var result = _service.GetPropertyDisplayName (_propertyInformationStub, _typeInformationStub);

      _globalizationServiceMock.VerifyAllExpectations ();
      _resourceManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("FakeResultForLongPropertyName"));
    }

    [Test]
    public void GetTypeDisplayName_NoResourceFound ()
    {
      _globalizationServiceMock.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManagerMock);
      _resourceManagerMock.Expect (mock => mock.ContainsResource (_longTypeResourceID)).Return (false);
      _resourceManagerMock.Expect (mock => mock.ContainsResource (_shortTypeResourceID)).Return (false);

      var result = _service.GetTypeDisplayName (_typeInformationStub);

      _globalizationServiceMock.VerifyAllExpectations ();
      _resourceManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("TypeName"));
    }

    [Test]
    public void GetTypeDisplayName_ResourceFoundByLongResourceID ()
    {
      _globalizationServiceMock.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManagerMock);
      _resourceManagerMock.Expect (mock => mock.ContainsResource (_longTypeResourceID)).Return (true);
      _resourceManagerMock.Expect (mock => mock.GetString (_longTypeResourceID)).Return ("FakeResultForLongTypeName");

      var result = _service.GetTypeDisplayName (_typeInformationStub);

      _globalizationServiceMock.VerifyAllExpectations ();
      _resourceManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("FakeResultForLongTypeName"));
    }

    [Test]
    public void GetTypeDisplayName_ResourceFoundByShortResourceID ()
    {
      _globalizationServiceMock.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManagerMock);
      _resourceManagerMock.Expect (mock => mock.ContainsResource (_longTypeResourceID)).Return (false);
      _resourceManagerMock.Expect (mock => mock.ContainsResource (_shortTypeResourceID)).Return (true);
      _resourceManagerMock.Expect (mock => mock.GetString (_shortTypeResourceID)).Return ("FakeResultForShortTypeName");

      var result = _service.GetTypeDisplayName (_typeInformationStub);

      _globalizationServiceMock.VerifyAllExpectations ();
      _resourceManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("FakeResultForShortTypeName"));
    }

    [Test]
    public void GetTypeDisplayName_ResourceFoundByLongAndShortResourceID_ReturnedByLongResourceID ()
    {
      _globalizationServiceMock.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManagerMock);
      _resourceManagerMock.Expect (mock => mock.ContainsResource (_longTypeResourceID)).Return (true);
      _resourceManagerMock.Stub (stub => stub.ContainsResource (_shortTypeResourceID)).Return (true);
      _resourceManagerMock.Expect (mock => mock.GetString (_longTypeResourceID)).Return ("FakeResultForLongTypeName");

      var result = _service.GetTypeDisplayName (_typeInformationStub);

      _globalizationServiceMock.VerifyAllExpectations ();
      _resourceManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("FakeResultForLongTypeName"));
    }
  }
}