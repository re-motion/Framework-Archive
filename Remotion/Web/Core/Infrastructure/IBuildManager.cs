﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using System.Web.Compilation;

namespace Remotion.Web.Infrastructure
{
  /// <summary>
  /// Represents a <see cref="BuildManager"/> compatible type.
  /// </summary>
  public interface IBuildManager
  {
    /// <summary>
    /// Compiles a file, given its virtual path, and returns the compiled type.
    /// </summary>
    /// <param name="virtualPath">The virtual path to build into a type.</param>
    /// <returns>A <see cref="Type"/> object that represents the type generated from compiling the virtual path.</returns>
    Type GetCompiledType (string virtualPath);
  }
}