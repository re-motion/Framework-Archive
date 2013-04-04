﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.TypePipe.Implementation
{
  /// <summary>
  /// Manages the code generated by the pipeline by delegating to the contained <see cref="ICodeGenerator"/> and <see cref="ITypeCache"/> instance.
  /// </summary>
  public class CodeManager : ICodeManager
  {
    private readonly ICodeGenerator _codeGenerator;
    private readonly ITypeCache _typeCache;

    public CodeManager (ICodeGenerator codeGenerator, ITypeCache typeCache)
    {
      ArgumentUtility.CheckNotNull ("codeGenerator", codeGenerator);
      ArgumentUtility.CheckNotNull ("typeCache", typeCache);

      _codeGenerator = codeGenerator;
      _typeCache = typeCache;
    }

    public string AssemblyDirectory
    {
      get { return _codeGenerator.AssemblyDirectory; }
    }

    public string AssemblyName
    {
      get { return _codeGenerator.AssemblyName; }
    }

    public void SetAssemblyDirectory (string assemblyDirectory)
    {
      _codeGenerator.SetAssemblyDirectory (assemblyDirectory);
    }

    public void SetAssemblyName (string assemblyName)
    {
      _codeGenerator.SetAssemblyName (assemblyName);
    }

    public string FlushCodeToDisk ()
    {
      return _codeGenerator.FlushCodeToDisk (_typeCache.ParticipantConfigurationID);
    }

    // TODO Review: Nur eine Methode wird an _typeCache delegiert. 'LoadFlushedCode' doch wieder auf IObjectFactory?
    // und ohne 'ICodeManager'-Objekt arbeiten?
    public void LoadFlushedCode (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      LoadFlushedCode ((_Assembly) assembly);
    }

    [CLSCompliant (false)]
    public void LoadFlushedCode (_Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      var assemblyAttribute = (TypePipeAssemblyAttribute) assembly.GetCustomAttributes (typeof (TypePipeAssemblyAttribute), inherit: false).SingleOrDefault();
      if (assemblyAttribute == null)
        throw new ArgumentException ("The specified assembly was not generated by the pipeline.", "assembly");

      if (assemblyAttribute.ParticipantConfigurationID != _typeCache.ParticipantConfigurationID)
      {
        var message = string.Format (
            "The specified assembly was generated with a different participant configuration: '{0}'.", assemblyAttribute.ParticipantConfigurationID);
        throw new ArgumentException (message, "assembly");
      }

      _typeCache.LoadTypes (assembly.GetTypes());
    }
  }
}