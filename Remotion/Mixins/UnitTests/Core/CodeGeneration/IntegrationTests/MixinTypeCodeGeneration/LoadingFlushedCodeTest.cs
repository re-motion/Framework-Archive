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
using System;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting.IO;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.TypePipe;
using System.Linq;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration
{
  [TestFixture]
  public class LoadingFlushedCodeTest : CodeGenerationBaseTest
  {
    private IPipeline _savedPipeline;

    public override void SetUp ()
    {
      base.SetUp ();

      // Use a dedicated pipeline to ensure that this test does not interfere with other tests.
      _savedPipeline = CreatePipeline ();
      PipelineRegistry.SetDefaultPipeline (_savedPipeline);
    }

    [Test]
    [Ignore ("TODO 5840: RebuildParticipantState is currently disabled")]
    public void LoadFlushedCode_IncludesGeneratedMixinTypes ()
    {
      var mixedInstance = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers));
      var mixin = Mixin.Get<MixinWithAbstractMembers> (mixedInstance);

      var identifier = GetConcreteMixinIdentifier(mixin);
      var assembly = FlushAndLoadAssemblyWithoutLocking();

      var pipelineForLoading = CreatePipeline();
      pipelineForLoading.CodeManager.LoadFlushedCode (assembly);

      var loadedMixinType = pipelineForLoading.ReflectionService.GetAdditionalType (identifier);
      Assert.That (loadedMixinType, Is.Not.Null);
      Assert.That (loadedMixinType.Assembly, Is.EqualTo (assembly));

      var mixedInstance2 = pipelineForLoading.Create<ClassOverridingMixinMembers>();
      var mixin2 = Mixin.Get<MixinWithAbstractMembers> (mixedInstance2);
      Assert.That (mixin2, Is.TypeOf (loadedMixinType));
    }

    [Test]
    public void LoadFlushedCodeTwice ()
    {
      CreateMixedType (typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      
      var assembly = FlushAndLoadAssemblyWithoutLocking ();

      var pipelineForLoading = CreatePipeline ();
      pipelineForLoading.CodeManager.LoadFlushedCode (assembly);
      Assert.That (() => pipelineForLoading.CodeManager.LoadFlushedCode (assembly), Throws.Nothing);
    }

    private IPipeline CreatePipeline ()
    {
      return PipelineFactory.Create (Pipeline.ParticipantConfigurationID, Pipeline.Participants.ToArray ());
    }

    private Assembly FlushAndLoadAssemblyWithoutLocking ()
    {
      var assemblyPaths = _savedPipeline.CodeManager.FlushCodeToDisk();
      var assemblyPath = assemblyPaths.Single();
      AddSavedAssembly (assemblyPath);
      var assembly = AssemblyLoader.LoadWithoutLocking (assemblyPath);
      return assembly;
    }

    private ConcreteMixinTypeIdentifier GetConcreteMixinIdentifier (MixinWithAbstractMembers mixin)
    {
      Assert.That (mixin, Is.AssignableTo<IGeneratedMixinType>());
      var attribute = (ConcreteMixinTypeAttribute) mixin.GetType().GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false).Single();

      return attribute.GetIdentifier();
    }
  }
}