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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// <see cref="TableScriptBuilder"/> contains database-independent code to generate scripts to create and drop tables in a relational database.
  /// </summary>
  public class TableScriptBuilder : IScriptBuilder2
  {
    private readonly IScriptElementFactory<TableDefinition> _elementFactory;
    private readonly ScriptElementCollection _createScriptElements;
    private readonly ScriptElementCollection _dropScriptElements;

    private class EntityDefinitionVisitor : IEntityDefinitionVisitor
    {
      private readonly TableScriptBuilder _builder;

      public EntityDefinitionVisitor (TableScriptBuilder builder)
      {
        ArgumentUtility.CheckNotNull ("builder", builder);

        _builder = builder;
      }

      void IEntityDefinitionVisitor.VisitTableDefinition (TableDefinition tableDefinition)
      {
        ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);

        _builder.AddTableDefinition (tableDefinition);
      }

      void IEntityDefinitionVisitor.VisitUnionViewDefinition (UnionViewDefinition unionViewDefinition)
      {
        //Nothing to do
      }

      void IEntityDefinitionVisitor.VisitFilterViewDefinition (FilterViewDefinition filterViewDefinition)
      {
        //Nothing to do
      }

      void IEntityDefinitionVisitor.VisitNullEntityDefinition (NullEntityDefinition nullEntityDefinition)
      {
        //Nothing to do
      }
    }

    public TableScriptBuilder (IScriptElementFactory<TableDefinition> elementFactory)
    {
      ArgumentUtility.CheckNotNull ("elementFactory", elementFactory);

      _elementFactory = elementFactory;
      _createScriptElements = new ScriptElementCollection();
      _dropScriptElements = new ScriptElementCollection();
    }

    public void AddEntityDefinition (IEntityDefinition entityDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);

      var visitor = new EntityDefinitionVisitor (this);
      entityDefinition.Accept (visitor);
    }

    public ScriptElementCollection GetCreateScript ()
    {
      return _createScriptElements;
    }

    public ScriptElementCollection GetDropScript ()
    {
      return _dropScriptElements;
    }

    private void AddTableDefinition (TableDefinition tableDefinition)
    {
      _createScriptElements.AddElement (_elementFactory.GetCreateElement(tableDefinition));
      _dropScriptElements.AddElement (_elementFactory.GetDropElement (tableDefinition));
    }
  }
}