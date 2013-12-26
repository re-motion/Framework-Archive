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
using Remotion.Development.UnitTesting;
using Remotion.TypePipe;
using Rhino.Mocks;

namespace Remotion.Reflection.CodeGeneration.TypePipe.UnitTests
{
  [TestFixture]
  public class ServiceLocatorBasedPipelineRegistryTest
  {
    private ServiceLocatorScope _serviceLocatorScope;
    private IPipelineRegistry _serviceLocatorBasedPipelineRegistry;
    private IPipelineRegistry _pipelineRegistryStub;
    private IPipeline _pipelineStub;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocatorBasedPipelineRegistry = new ServiceLocatorBasedPipelineRegistry();
      _pipelineRegistryStub = MockRepository.GenerateStub<IPipelineRegistry>();
      _pipelineStub = MockRepository.GenerateStub<IPipeline>();
      _serviceLocatorScope = new ServiceLocatorScope (typeof (IPipelineRegistry), () => _pipelineRegistryStub);
    }

    [TearDown]
    public void TearDown ()
    {
      _serviceLocatorScope.Dispose();
    }

    [Test]
    public void GetDefaulPipeline_ReturnsPipelineFromServiceLocatorBasedRegistry ()
    {
      _pipelineRegistryStub.Stub (_ => _.DefaultPipeline).Return (_pipelineStub);

      Assert.That (_serviceLocatorBasedPipelineRegistry.DefaultPipeline, Is.SameAs (_pipelineStub));
    }

    [Test]
    public void SetDefaultPipeline_PassesPipelineToServiceLocatorBasedRegistry ()
    {
      _serviceLocatorBasedPipelineRegistry.SetDefaultPipeline (_pipelineStub);

      _pipelineRegistryStub.AssertWasCalled (_ => _.SetDefaultPipeline (_pipelineStub));
    }

    [Test]
    public void Get_ReturnsPipelineFromServiceLocatorBasedRegistry ()
    {
      _pipelineRegistryStub.Stub (_ => _.Get ("TheParticipantConfigurationID")).Return (_pipelineStub);

      Assert.That (_serviceLocatorBasedPipelineRegistry.Get ("TheParticipantConfigurationID"), Is.SameAs (_pipelineStub));
    }

    [Test]
    public void Register_PassesPipelineToServiceLocatorBasedRegistry ()
    {
      _serviceLocatorBasedPipelineRegistry.Register (_pipelineStub);

      _pipelineRegistryStub.AssertWasCalled (_ => _.Register (_pipelineStub));
    }

    [Test]
    public void Unregister_PassesPipelineIDToServiceLocatorBasedRegistry ()
    {
      _serviceLocatorBasedPipelineRegistry.Unregister ("TheParticipantConfigurationID");

      _pipelineRegistryStub.AssertWasCalled (_ => _.Unregister ("TheParticipantConfigurationID"));
    }
  }
}