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
using System.ComponentModel;
using System.Data;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Factories
{
  public static class ColumnDefinitionObjectMother
  {
    public static readonly ColumnDefinition IDColumn =
        new ColumnDefinition (
            "ID", typeof (Guid), new StorageTypeInformation ("uniqueidentifier", DbType.Guid, typeof (Guid?), new DefaultConverter (typeof (Guid?))), false, true);

    public static readonly ColumnDefinition ClassIDColumn =
        new ColumnDefinition (
            "ClassID", typeof (string), new StorageTypeInformation ("varchar", DbType.String, typeof (string), new DefaultConverter(typeof (string))), true, false);

    public static readonly ColumnDefinition TimestampColumn =
        new ColumnDefinition (
            "Timestamp",
            typeof (DateTime),
            new StorageTypeInformation ("datetime", DbType.DateTime, typeof (DateTime), new DefaultConverter (typeof (DateTime))),
            true,
            false);

    public static ColumnDefinition CreateColumn ()
    {
      return new ColumnDefinition (
          Guid.NewGuid().ToString(),
          typeof (string),
          new StorageTypeInformation ("varchar", DbType.String, typeof (string), new DefaultConverter (typeof (string))),
          true,
          false);
    }

    public static ColumnDefinition CreateColumn (string columnName)
    {
      return new ColumnDefinition (
          columnName, typeof (string), new StorageTypeInformation ("varchar", DbType.String, typeof (string), new DefaultConverter (typeof (string))), true, false);
    }
  }
}