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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.UnitTests;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception
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

      var fakeDomainObjectType = ReflectionObjectMother.GetSomeType();
      _participantHelperMock.Expect (mock => mock.GetPublicDomainObjectType (proxyType.BaseType)).Return (fakeDomainObjectType);

      _participant.ModifyType (proxyType);

      _participantHelperMock.VerifyAllExpectations();

      Assert.That (proxyType.AddedInterfaces, Is.EqualTo (new[] { typeof (IInterceptedDomainObject) }));
      Assert.That (proxyType.AddedMethods, Has.Count.EqualTo (2));

      var performConstructorCheck = proxyType.AddedMethods.Single (m => m.Name == "PerformConstructorCheck");
      Assert.That (performConstructorCheck.Body, Is.TypeOf<DefaultExpression>().And.Property ("Type").SameAs (typeof (void)));
      var getPublicDomainObjectTypeImplementation = proxyType.AddedMethods.Single (m => m.Name == "GetPublicDomainObjectTypeImplementation");
      Assert.That (getPublicDomainObjectTypeImplementation.Body, Is.TypeOf<ConstantExpression>().And.Property ("Value").SameAs (fakeDomainObjectType));
    }

    private class MyDomainObject : DomainObject { }
  }
}