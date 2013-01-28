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
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.UnitTests;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.TypePipe
{
  [TestFixture]
  public class InterceptedDomainObjectParticipantTest
  {
    private IParticipantHelper _participantHelperMock;

    private InterceptedDomainObjectParticipant _participant;

    [SetUp]
    public void SetUp ()
    {
      _participantHelperMock = MockRepository.GenerateStrictMock<IParticipantHelper>();

      _participant = new InterceptedDomainObjectParticipant (_participantHelperMock);
    }

    [Test]
    public void ModifyType ()
    {
      var proxyType = ProxyTypeObjectMother.Create (typeof (MyDomainObject));
      var property = NormalizingMemberInfoFromExpressionUtility.GetProperty ((MyDomainObject o) => o.SomeProperty);
      var getter = property.GetGetMethod();
      var setter = property.GetSetMethod();
      var fakeProperties = new[] { Tuple.Create (property, "propertyIdentifier") };
      var fakeDomainObjectType = ReflectionObjectMother.GetSomeType();
      _participantHelperMock.Expect (mock => mock.GetPublicDomainObjectType (proxyType.BaseType)).Return (fakeDomainObjectType);
      _participantHelperMock.Expect (mock => mock.GetInterceptedProperties (fakeDomainObjectType)).Return (fakeProperties);
      _participantHelperMock.Expect (mock => mock.GetMostDerivedMethodOverride (getter, proxyType)).Return(ReflectionObjectMother.getSome);
      _participantHelperMock.Expect (mock => mock.GetMostDerivedMethodOverride (setter, proxyType));

      _participant.ModifyType (proxyType);

      _participantHelperMock.VerifyAllExpectations();
      Assert.That (proxyType.AddedInterfaces, Is.EqualTo (new[] { typeof (IInterceptedDomainObject) }));
      Assert.That (proxyType.AddedMethods, Has.Count.EqualTo (2));

      var performConstructorCheck = proxyType.AddedMethods.Single (m => m.Name == "PerformConstructorCheck");
      Assert.That (performConstructorCheck.Body, Is.TypeOf<DefaultExpression>().And.Property ("Type").SameAs (typeof (void)));
      var getPublicDomainObjectTypeImplementation = proxyType.AddedMethods.Single (m => m.Name == "GetPublicDomainObjectTypeImplementation");
      Assert.That (getPublicDomainObjectTypeImplementation.Body, Is.TypeOf<ConstantExpression>().And.Property ("Value").SameAs (fakeDomainObjectType));
    }

    [IgnoreForMappingConfiguration]
    private class MyDomainObject : DomainObject
    {
      public string SomeProperty { get; set; }
      protected string ProtectedProperty { get; set; }

      public string ReadOnlyProperty
      {
        get { return ""; }
      }

      public string WriteOnlyProperty
      {
        set { Dev.Null = value; }
      }
    }
  }
}