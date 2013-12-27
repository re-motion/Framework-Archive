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
using System.Reflection;
using Remotion.TypePipe.MutableReflection;

namespace Remotion.TypePipe.Implementation
{
  /// <summary>
  /// Manages the code generated by the pipeline. Supports saving and loading of generated code.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  public interface ICodeManager
  {
    /// <summary>
    /// Saves all types that have been generated since the last call to this method into a new <see cref="Assembly"/> on disk.
    /// The file name of the assembly is derived from <see cref="PipelineSettings.AssemblyNamePattern"/> plus the file ending <c>.dll</c>.
    /// The assembly is written to the directory defined by <see cref="PipelineSettings.AssemblyDirectory"/>.
    /// If <see cref="PipelineSettings.AssemblyDirectory"/> is <see langword="null"/> the assembly is saved in the current working directory.
    /// </summary>
    /// <param name="assemblyAttributes">A number of custom <see cref="Attribute"/>s that are attached to the assembly.</param>
    /// <remarks>
    /// If no new types have been generated since the last call to <see cref="FlushCodeToDisk"/>, this method does nothing
    /// and returns <see langword="null"/>.
    /// </remarks>
    /// <returns>The absolute paths to the saved assembly files, or an empty array if no assembly was saved.</returns>
    string[] FlushCodeToDisk (params CustomAttributeDeclaration[] assemblyAttributes);

    /// <summary>
    /// Attempts to load all types in a previously flushed <see cref="Assembly"/>.
    /// Note that only assemblies that were generated by the pipeline with an equivalent participant configuration can be loaded.
    /// </summary>
    /// <param name="assembly">The flushed assembly which should be loaded.</param>
    /// <exception cref="ArgumentException">
    /// If the assembly was not generated by the pipeline; or if it was generated with a different participant configuration.
    /// </exception>
    void LoadFlushedCode (Assembly assembly);
  }
}