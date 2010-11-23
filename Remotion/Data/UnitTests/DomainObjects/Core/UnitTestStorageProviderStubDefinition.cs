// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Xml;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  public class UnitTestStorageProviderStubDefinition : StorageProviderDefinition
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public UnitTestStorageProviderStubDefinition (
        string storageProviderID,
        Type storageProviderType,
        Type storageFactoryType)
        : base (storageProviderID, storageFactoryType)
    {
    }

    public UnitTestStorageProviderStubDefinition (
        string storageProviderID,
        Type storageProviderType,
        Type storageFactoryType,
        XmlNode configurationNode)
        : base (storageProviderID, storageFactoryType)
    {
    }

    // methods and properties

    public override bool IsIdentityTypeSupported (Type identityType)
    {
      ArgumentUtility.CheckNotNull ("identityType", identityType);

      // Note: UnitTestStorageProviderStubDefinition supports all identity types for testing purposes.
      return true;
    }
  }
}
