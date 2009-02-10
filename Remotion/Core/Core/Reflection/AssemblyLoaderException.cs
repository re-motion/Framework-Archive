// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// This framework is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this framework; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Runtime.Serialization;

namespace Remotion.Reflection
{
  /// <summary>
  /// Thrown when the automatic assembly loading performed by <see cref="AssemblyLoader"/> and <see cref="AssemblyFinder"/> yields an error during a
  /// load operation.
  /// </summary>
  [Serializable]
  public class AssemblyLoaderException : Exception
  {
    public AssemblyLoaderException (string message, Exception innerException)
        : base (message, innerException)
    {
    }

    protected AssemblyLoaderException (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
    }
  }
}