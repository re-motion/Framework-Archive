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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Remotion.Collections;
using Remotion.Logging;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Validation;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.TypePipe;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Mixins.MixerTools
{
  /// <summary>
  /// Provides functionality for pre-generating mixed types and saving them to disk to be later loaded via 
  /// <see cref="ConcreteTypeBuilder.LoadConcreteTypes(System.Reflection.Assembly)"/>.
  /// </summary>
  public class Mixer
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (Mixer));

    public static Mixer Create (string assemblyName, string assemblyOutputDirectory)
    {
      
      // TODO 5370:
      // create pipeline with default participants (from default pipeline) via RemotionPipelineFactory
      // assert that re-mix participant is part of the participants list
      // Log an info message with participant list
      var builderFactory = new MixerPipelineFactory (assemblyName);

      // Use a custom TypeDiscoveryService with the LoadAllAssemblyLoaderFilter so that mixed types within system assemblies are also considered.
      var assemblyLoader = new FilteringAssemblyLoader (new LoadAllAssemblyLoaderFilter ());
      var rootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain (false, assemblyLoader);
      var assemblyFinder = new CachingAssemblyFinderDecorator (new AssemblyFinder (rootAssemblyFinder, assemblyLoader));
      var typeDiscoveryService = new AssemblyFinderTypeDiscoveryService (assemblyFinder);

      var finder = new MixedTypeFinder (typeDiscoveryService);

      return new Mixer (finder, builderFactory, assemblyOutputDirectory);
    }

    public event EventHandler<TypeEventArgs> TypeBeingProcessed = delegate { };
    public event EventHandler<ValidationErrorEventArgs> ValidationErrorOccurred = delegate { };
    public event EventHandler<ErrorEventArgs> ErrorOccurred = delegate { };

    private readonly List<Tuple<Type, Exception>> _errors = new List<Tuple<Type, Exception>> ();
    private readonly HashSet<Type> _processedTypes = new HashSet<Type>();
    private readonly Dictionary<Type, Type> _finishedTypes = new Dictionary<Type, Type> ();

    private string _generatedFile = null;

    public Mixer (IMixedTypeFinder mixedTypeFinder, IMixerPipelineFactory mixerPipelineFactory, string assemblyOutputDirectory)
    {
      ArgumentUtility.CheckNotNull ("mixedTypeFinder", mixedTypeFinder);
      ArgumentUtility.CheckNotNull ("mixerPipelineFactory", mixerPipelineFactory);
      ArgumentUtility.CheckNotNull ("assemblyOutputDirectory", assemblyOutputDirectory);

      MixedTypeFinder = mixedTypeFinder;
      MixerPipelineFactory = mixerPipelineFactory;
      AssemblyOutputDirectory = assemblyOutputDirectory;
    }

    public IMixedTypeFinder MixedTypeFinder { get; private set; }
    public IMixerPipelineFactory MixerPipelineFactory { get; private set; }
    public string AssemblyOutputDirectory { get; private set; }

    public ReadOnlyCollection<Tuple<Type, Exception>> Errors
    {
      get { return _errors.AsReadOnly(); }
    }

    public ReadOnlyCollectionDecorator<Type> ProcessedTypes
    {
      get { return _processedTypes.AsReadOnly(); }
    }

    public ReadOnlyDictionary<Type, Type> FinishedTypes
    {
      get { return new ReadOnlyDictionary<Type, Type>(_finishedTypes); }
    }

    public string GeneratedFile
    {
      get { return _generatedFile; }
    }

    public void PrepareOutputDirectory ()
    {
      if (!Directory.Exists (AssemblyOutputDirectory))
      {
        s_log.InfoFormat ("Preparing output directory '{0}'.", AssemblyOutputDirectory);
        Directory.CreateDirectory (AssemblyOutputDirectory);
      }

      CleanupIfExists (MixerPipelineFactory.GetModulePath (AssemblyOutputDirectory));
    }

    // The MixinConfiguration is passed to Execute in order to be able to call PrepareOutputDirectory before analyzing the configuration (and potentially
    // locking old generated files).
    public void Execute (MixinConfiguration configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Time needed to mix and save all types: {elapsed}."))
      {
        _errors.Clear();
        _processedTypes.Clear();
        _finishedTypes.Clear();
        _generatedFile = null;

        s_log.InfoFormat ("The base directory is '{0}'.", AppDomain.CurrentDomain.BaseDirectory);

        var pipeline = MixerPipelineFactory.CreatePipeline (AssemblyOutputDirectory);

        var mixedTypes = MixedTypeFinder.FindMixedTypes (configuration).ToArray ();

        s_log.Info ("Generating types...");
        using (configuration.EnterScope())
        {
          foreach (var mixedType in mixedTypes)
            Generate (mixedType, pipeline);
        }

        s_log.Info ("Saving assemblies...");
        Save (pipeline);
      }

      s_log.InfoFormat ("Successfully generated concrete types for {0} target classes.", _finishedTypes.Count);
    }

    private void Generate (Type mixedType, IPipeline pipeline)
    {
      _processedTypes.Add (mixedType);

      try
      {
        TypeBeingProcessed (this, new TypeEventArgs (mixedType));

        Type concreteType = pipeline.ReflectionService.GetAssembledType (mixedType);
        _finishedTypes.Add (mixedType, concreteType);
      }
      catch (ValidationException validationException)
      {
        _errors.Add (new Tuple<Type, Exception> (mixedType, validationException));
        ValidationErrorOccurred (this, new ValidationErrorEventArgs (validationException));
      }
      catch (Exception ex)
      {
        _errors.Add (new Tuple<Type, Exception> (mixedType, ex));
        ErrorOccurred (this, new ErrorEventArgs (ex));
      }
    }

    private void Save (IPipeline pipeline)
    {
      _generatedFile = pipeline.CodeManager.FlushCodeToDisk();

      s_log.InfoFormat ("Generated assembly file {0}.", _generatedFile);
    }

    private void CleanupIfExists (string path)
    {
      if (File.Exists (path))
      {
        s_log.InfoFormat ("Removing file '{0}'.", path);
        File.Delete (path);
      }
    }
  }
}
