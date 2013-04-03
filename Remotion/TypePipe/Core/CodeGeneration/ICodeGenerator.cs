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

namespace Remotion.TypePipe.CodeGeneration
{
  /// <summary>
  /// Instances of this interface represent the code generator used by the pipeline.
  /// </summary>
  public interface ICodeGenerator
  {
    string AssemblyDirectory { get; }
    string AssemblyName { get; }

    /// <summary>
    /// Sets the directory in that assemblies will be saved, when <see cref="FlushCodeToDisk"/> is called.
    /// If <see langword="null"/> is specified, the assemblies are saved to the current working directory.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// If the pipeline already generated a type (because it was requested) into the new assembly, the assembly directory cannot be changed.
    /// </exception>
    /// <param name="assemblyDirectory">The assembly directory, or <see langword="null"/>.</param>
    void SetAssemblyDirectory (string assemblyDirectory);

    /// <summary>
    /// Sets the name for the next assembly.
    /// When using this API, the user is responsible for providing unique assembly names.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// If the pipeline already generated a type (because it was requested) into the new assembly, the assembly name cannot be changed.
    /// </exception>
    /// <remarks>
    /// Note that the provided value is overwritten after every call to <see cref="FlushCodeToDisk"/> to ensure uniqueness.
    /// </remarks>
    /// <param name="assemblyName">The assembly name (without <c>.dll</c>).</param>
    void SetAssemblyName (string assemblyName);

    /// <summary>
    /// Saves all types that have been generated since the last call to this method into a new assembly on disk.
    /// The file name of the assembly consists of <see cref="AssemblyName"/> plus the file ending <c>.dll</c>.
    /// The assembly is written to the directory defined by <see cref="AssemblyDirectory"/>.
    /// If <see cref="AssemblyDirectory"/> is <see langword="null"/> the assembly is saved in the current working directory.
    /// </summary>
    /// <param name="participantConfigurationID">The participant configuration identification.</param>
    /// <remarks>
    /// If no new types have been generated since the last call to <see cref="FlushCodeToDisk"/>, this method does nothing
    /// and returns <see langword="null"/>.
    /// This method also generates a new unique name for the next assembly.
    /// </remarks>
    /// <returns>The absolute path to the saved assembly file, or <see langword="null"/> if no assembly was saved.</returns>
    string FlushCodeToDisk (string participantConfigurationID);
  }
}