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
using System.Collections.Generic;
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  public class ExtendedFileBuilder : FileBuilder
  {
    public ExtendedFileBuilder (Func<ScriptBuilderBase> scriptBuilderFactory)
        : base (scriptBuilderFactory, new ExtendedEntityDefinitionProvider())
    {
    }

    public override string GetScript (IEnumerable<ClassDefinition> classDefinitions)
    {
      var script = new StringBuilder (base.GetScript (classDefinitions));

      script.Insert (0, "IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'Test') BEGIN EXEC('CREATE SCHEMA Test') END\r\nGO\r\n");
      script.Insert (0, "--Extendend file-builder comment at the beginning\r\n");
      script.AppendLine ("--Extendend file-builder comment at the end");

      return script.ToString();
    }
   
  }
}