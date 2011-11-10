// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation
{
  public class DerivedValidationDomainObjectClass : BaseValidationDomainObjectClass
  {
    public string Property { get; set; }

    [StorageClass (StorageClass.Persistent)]
    public string PropertyWithStorageClassPersistent { get; set; }

    [StorageClass (StorageClass.Transaction)]
    public string PropertyWithStorageClassTransaction { get; set; }

    [StorageClass (StorageClass.None)]
    public string PropertyWithStorageClassNone { get; set; }

    [StorageClass (StorageClass.Persistent)]
    public object PropertyWithTypeObjectWithStorageClassPersistent { get; set; }

    [StorageClass (StorageClass.Persistent)]
    public OtherDerivedValidationHierarchyClass RelationPropertyWithStorageClassPersistent { get; set; }
  }
}