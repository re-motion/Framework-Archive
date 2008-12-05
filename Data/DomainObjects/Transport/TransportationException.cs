// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.Transport
{
  /// <summary>
  /// Indicates a problem when exporting or importing <see cref="DomainObject"/> instances using <see cref="DomainObjectTransporter"/>. Usually,
  /// the data or objects either don't match the <see cref="IImportStrategy"/> or <see cref="IExportStrategy"/> being used, or the data has become
  /// corrupted.
  /// </summary>
  public class TransportationException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TransportationException"/> class.
    /// </summary>
    public TransportationException ()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TransportationException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public TransportationException (string message)
        : base (message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TransportationException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public TransportationException (string message, Exception innerException)
      : base (message, innerException)
    {
    }

  }
}
