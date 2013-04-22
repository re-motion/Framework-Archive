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

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Remotion.TypePipe.Implementation;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.Mixins.UnitTests.Core
{
  // TODO 5370
  public class AssemblyTrackingCodeManager : ICodeManager
  {
    private readonly List<string> _savedAssemblies = new List<string> ();
    private readonly ICodeManager _codeManager;

    public AssemblyTrackingCodeManager (ICodeManager codeManager)
    {
      ArgumentUtility.CheckNotNull ("codeManager", codeManager);

      _codeManager = codeManager;
    }

    public IList<string> SavedAssemblies
    {
      get { return _savedAssemblies; }
    }

    public void DeleteSavedAssemblies ()
    {
      foreach (var savedAssemblyPath in _savedAssemblies)
      {
        FileUtility.DeleteAndWaitForCompletion (savedAssemblyPath);
        FileUtility.DeleteAndWaitForCompletion (Path.ChangeExtension (savedAssemblyPath, "pdb"));
      }
    }

    public string AssemblyDirectory
    {
      get { return _codeManager.AssemblyDirectory; }
    }

    public string AssemblyNamePattern
    {
      get { return _codeManager.AssemblyNamePattern; }
    }

    public void SetAssemblyDirectory (string assemblyDirectory)
    {
      _codeManager.SetAssemblyDirectory (assemblyDirectory);
    }

    public void SetAssemblyNamePattern (string assemblyNamePattern)
    {
      _codeManager.SetAssemblyNamePattern (assemblyNamePattern);
    }

    public string FlushCodeToDisk (IEnumerable<CustomAttributeDeclaration> assemblyAttributes)
    {
      var assemblyPath = _codeManager.FlushCodeToDisk (assemblyAttributes);

      if (assemblyPath != null)
        _savedAssemblies.Add (assemblyPath);

      return assemblyPath;
    }

    public string FlushCodeToDisk (params CustomAttributeDeclaration[] assemblyAttributes)
    {
      return FlushCodeToDisk ((IEnumerable<CustomAttributeDeclaration>) assemblyAttributes);
    }

    public void LoadFlushedCode (Assembly assembly)
    {
      _codeManager.LoadFlushedCode (assembly);
    }
  }
}