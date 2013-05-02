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
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.TypePipe;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.Diagnostics;
using Remotion.Reflection.TypeDiscovery;
using Remotion.TypePipe;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.Configuration;
using Remotion.TypePipe.Implementation;

namespace Remotion.Development.UnitTests.Core.TypePipe
{
  [TestFixture]
  public class DebuggerWorkaroundPipelineFactoryTest
  {
    private DebuggerWorkaroundPipelineFactory _factory;

    [SetUp]
    public void SetUp ()
    {
      _factory = new DebuggerWorkaroundPipelineFactory();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_factory.DebuggerInterface, Is.Not.Null.And.TypeOf<DebuggerInterface>());
      Assert.That (_factory.MaximumTypesPerAssembly, Is.EqualTo (11));
    }

    [Test]
    public void NewReflectionEmitCodeGenerator ()
    {
      var forceStrongNaming = BooleanObjectMother.GetRandomBoolean();
      var keyFilePath = "keyFilePath";
      _factory.MaximumTypesPerAssembly = 7;

      var result = _factory.Invoke<IReflectionEmitCodeGenerator> ("NewReflectionEmitCodeGenerator", forceStrongNaming, keyFilePath);

      Assert.That (result, Is.TypeOf<DebuggerWorkaroundCodeGenerator>());
      var debuggerWorkaroundCodeGenerator = (DebuggerWorkaroundCodeGenerator) result;
      Assert.That (debuggerWorkaroundCodeGenerator.MaximumTypesPerAssembly, Is.EqualTo (7));
    }

    [Test]
    public void Integration_CreatedPipeline_AddsNonApplicationAssemblyAttribute_OnModuleCreation ()
    {
      // Creates new in-memory assembly (avoid no-modification optimization).
      var settings = new PipelineSettings ("dummy id");
      var defaultPipeline = _factory.CreatePipeline (settings, new[] { new ModifyingParticipant() });
      var type = defaultPipeline.ReflectionService.GetAssembledType (typeof (RequestedType));

      Assert.That (type.Assembly.IsDefined (typeof (NonApplicationAssemblyAttribute), false), Is.True);
    }

    public class ModifyingParticipant : SimpleParticipantBase
    {
      public override void Participate (object id, ITypeAssemblyContext typeAssemblyContext)
      {
        typeAssemblyContext.ProxyType.AddField ("test", FieldAttributes.Private, typeof (int));
      }
    }

    public class RequestedType { }
  }
}