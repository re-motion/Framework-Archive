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
using Remotion.Development.TypePipe;
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.ServiceLocation;
using Remotion.Text;
using Remotion.TypePipe;
using Remotion.TypePipe.Configuration;
using Remotion.TypePipe.Serialization;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private static readonly IPipelineRegistry s_pipelineRegistry = SafeServiceLocator.Current.GetInstance<IPipelineRegistry>();

    private static IPipeline s_pipeline;
    private static bool s_skipDeletion;

    private AssemblyTrackingCodeManager _assemblyTrackingCodeManager;

    public static IPipelineRegistry PipelineRegistry
    {
      get { return s_pipelineRegistry; }
    }

    public static IPipeline Pipeline
    {
      get
      {
        if (s_pipeline == null)
          throw new InvalidOperationException ("SetUp must be executed first.");
        return s_pipeline;
      }
    }

    /// <summary>
    /// Signals that the <see cref="SetUpFixture"/> should not delete the files it generates. Call this ad-hoc in a test to keep the files and inspect
    /// them with Reflector or ildasm.
    /// </summary>
    public static void SkipDeletion ()
    {
      s_skipDeletion = true;
    }

    [SetUp]
    public void SetUp ()
    {
      var assemblyTrackingPipelineFactory = new AssemblyTrackingPipelineFactory();
      var settings = new PipelineSettings ("re-mix-tests");
      var participants = new IParticipant[] { new MixinParticipant() };

      s_pipeline = assemblyTrackingPipelineFactory.CreatePipeline (settings, participants);
      _assemblyTrackingCodeManager = assemblyTrackingPipelineFactory.AssemblyTrackingCodeManager;
    }

    [TearDown]
    public void TearDown()
    {
#if !NO_PEVERIFY
      try
      {
        _assemblyTrackingCodeManager.FlushCodeToDisk();
      }
      catch (Exception ex)
      {
        Assert.Fail ("Error when saving assemblies: {0}", ex);
      }

      _assemblyTrackingCodeManager.PeVerifySavedAssemblies();
#endif

      if (!s_skipDeletion)
      {
        _assemblyTrackingCodeManager.DeleteSavedAssemblies(); // Delete assemblies if everything went fine.
      }
      else
      {
        Console.WriteLine (
            "Assemblies saved to: " + Environment.NewLine
            + SeparatedStringBuilder.Build (Environment.NewLine, _assemblyTrackingCodeManager.SavedAssemblies));
      }
    }
  }
}
