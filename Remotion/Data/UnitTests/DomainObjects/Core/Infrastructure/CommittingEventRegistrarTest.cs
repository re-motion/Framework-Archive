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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class CommittingEventRegistrarTest : StandardMappingTest
  {
    private DomainObject _domainObject1;
    private DomainObject _domainObject2;
    private DomainObject _domainObject3;

    private CommittingEventRegistrar _registrar;

    public override void SetUp ()
    {
      base.SetUp ();

      _domainObject1 = DomainObjectMother.CreateFakeObject ();
      _domainObject2 = DomainObjectMother.CreateFakeObject ();
      _domainObject3 = DomainObjectMother.CreateFakeObject ();

      _registrar = new CommittingEventRegistrar();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_registrar.RegisteredObjects, Is.Empty);
    }

    [Test]
    public void RegisterForAdditionalCommittingEvents ()
    {
      Assert.That (_registrar.RegisteredObjects, Is.Empty);

      _registrar.RegisterForAdditionalCommittingEvents (_domainObject1, _domainObject2, _domainObject1);

      Assert.That (_registrar.RegisteredObjects, Is.EquivalentTo (new[] { _domainObject1, _domainObject2 }));

      _registrar.RegisterForAdditionalCommittingEvents (_domainObject2, _domainObject3);

      Assert.That (_registrar.RegisteredObjects, Is.EquivalentTo (new[] { _domainObject1, _domainObject2, _domainObject3 }));
    }
  }
}