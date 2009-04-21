// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration
{
  [TestFixture]
  public class FakeDomainObjectsConfigurationTest
  {
    [Test]
    public void Initialize()
    {
      StorageConfiguration storage = new StorageConfiguration ();
      MappingLoaderConfiguration mappingLoader = new MappingLoaderConfiguration ();
      QueryConfiguration query = new QueryConfiguration ();
      IDomainObjectsConfiguration configuration = new FakeDomainObjectsConfiguration (mappingLoader, storage, query);
    
      Assert.AreSame (mappingLoader, configuration.MappingLoader);
      Assert.AreSame (storage, configuration.Storage);
      Assert.AreSame (query, configuration.Query);
    }
  }
}
