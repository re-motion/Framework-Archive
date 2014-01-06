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
using System.Linq;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding
{
  /// <summary>
  /// Finds the root assemblies by looking up and loading files matching the given patterns in a specified directory.
  /// </summary>
  public class FilePatternRootAssemblyFinder : IRootAssemblyFinder
  {
    /// <summary>
    /// Holds a file path string as well as a flag indicating whether referenced assemblies should be followed or not. Equality comparisons of
    /// instances only check the file name, not the flag - this simplifies the algorithm to exclude file names in 
    /// <see cref="FilePatternRootAssemblyFinder.ConsolidateSpecifications"/>.
    /// </summary>
    private struct FileDescription
    {
      public FileDescription (string file, bool followReferences)
          : this()
      {
        FilePath = file;
        FollowReferences = followReferences;
      }

      public string FilePath { get; private set; }
      public bool FollowReferences { get; private set; }

      public override bool Equals (object obj)
      {
        return obj is FileDescription && Equals (FilePath, ((FileDescription) obj).FilePath);
      }

      public override int GetHashCode ()
      {
        return FilePath.GetHashCode ();
      }
    }

    private readonly string _searchPath;
    private readonly FilePatternSpecification[] _specifications;
    private readonly IFileSearchService _fileSearchService;
    private readonly IAssemblyLoader _assemblyLoader;

    public FilePatternRootAssemblyFinder (
        string searchPath, 
        IEnumerable<FilePatternSpecification> specifications, 
        IFileSearchService fileSearchService, 
        IAssemblyLoader assemblyLoader)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("searchPath", searchPath);
      ArgumentUtility.CheckNotNull ("specifications", specifications);
      ArgumentUtility.CheckNotNull ("fileSearchService", fileSearchService);
      ArgumentUtility.CheckNotNull ("assemblyLoader", assemblyLoader);

      _searchPath = searchPath;
      _specifications = specifications.ToArray();
      _fileSearchService = fileSearchService;
      _assemblyLoader = assemblyLoader;
    }

    public string SearchPath
    {
      get { return _searchPath; }
    }

    public ReadOnlyCollection<FilePatternSpecification> Specifications
    {
      get { return Array.AsReadOnly (_specifications); }
    }

    public IFileSearchService FileSearchService
    {
      get { return _fileSearchService; }
    }

    public IAssemblyLoader AssemblyLoader
    {
      get { return _assemblyLoader; }
    }

    public RootAssembly[] FindRootAssemblies ()
    {
      var fileDescriptions = ConsolidateSpecifications ();

      var rootAssemblies = from fileDescription in fileDescriptions
                           let assembly = _assemblyLoader.TryLoadAssembly (fileDescription.FilePath)
                           where assembly != null
                           select new RootAssembly (assembly, fileDescription.FollowReferences);
      return rootAssemblies.Distinct ().ToArray ();
    }

    private IEnumerable<FileDescription> ConsolidateSpecifications ()
    {
      var fileDescriptions = new HashSet<FileDescription> ();

      foreach (var specification in _specifications)
      {
        switch (specification.Kind)
        {
          case FilePatternSpecificationKind.IncludeNoFollow:
            var filesNotToFollow = _fileSearchService.GetFiles (_searchPath, specification.FilePattern, SearchOption.TopDirectoryOnly);
            fileDescriptions.UnionWith (filesNotToFollow.Select (f => new FileDescription (f, false)));
            break;
          case FilePatternSpecificationKind.IncludeFollowReferences:
            var filesToFollow = _fileSearchService.GetFiles (_searchPath, specification.FilePattern, SearchOption.TopDirectoryOnly);
            fileDescriptions.UnionWith (filesToFollow.Select (f => new FileDescription (f, true)));
            break;
          default:
            var filesToExclude = _fileSearchService.GetFiles (_searchPath, specification.FilePattern, SearchOption.TopDirectoryOnly);
            fileDescriptions.ExceptWith (filesToExclude.Select (f => new FileDescription (f, true))); // the "true" flag is ignored on comparisons
            break;
        }
      }

      return fileDescriptions;
    }
  }
}
