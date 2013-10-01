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
using System.Linq;
using System.Xml.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport
{
  public class MappingSerializer : IMappingSerializer
  {
    private readonly IStorageProviderSerializer _storageProviderSerializer;
    private readonly IEnumSerializer _enumSerializer;

    public MappingSerializer (IStorageProviderSerializer storageProviderSerializer, IEnumSerializer enumSerializer)
    {
      ArgumentUtility.CheckNotNull ("storageProviderSerializer", storageProviderSerializer);
      ArgumentUtility.CheckNotNull ("enumSerializer", enumSerializer);

      _storageProviderSerializer = storageProviderSerializer;
      _enumSerializer = enumSerializer;
    }

    public IStorageProviderSerializer StorageProviderSerializer
    {
      get { return _storageProviderSerializer; }
    }

    public IEnumSerializer EnumSerializer
    {
      get { return _enumSerializer; }
    }

    public XDocument Serialize (IEnumerable<ClassDefinition> typeDefinitions)
    {
      ArgumentUtility.CheckNotNull ("typeDefinitions", typeDefinitions);

      return new XDocument (
          new XElement (
              "mapping",
              GetStorageProviders(typeDefinitions),
              GetEnums()));
    }

    private IEnumerable<XElement> GetEnums ()
    {
      return _enumSerializer.Serialize ().ToList();
    }

    private IEnumerable<XElement> GetStorageProviders (IEnumerable<ClassDefinition> typeDefinitions)
    {
      return _storageProviderSerializer.Serialize (typeDefinitions).ToList();
    }
  }
}