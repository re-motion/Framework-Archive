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
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// Visits <see cref="IColumnDefinition"/> objects and generates a list of column names for the visited columns.
  /// </summary>
  public class SqlNameListColumnDefinitionVisitor : IColumnDefinitionVisitor
  {
    private readonly bool _allowNullColumns;
    private readonly StringBuilder _nameList = new StringBuilder ();

    public SqlNameListColumnDefinitionVisitor (bool allowNullColumns)
    {
      _allowNullColumns = allowNullColumns;
    }

    public string GetNameList ()
    {
      return _nameList.ToString ();
    }

    public void VisitSimpleColumnDefinition (SimpleColumnDefinition simpleColumnDefinition)
    {
      ArgumentUtility.CheckNotNull ("simpleColumnDefinition", simpleColumnDefinition);
      
      AppendSeparatorIfRequired();
      _nameList.Append ("[").Append (simpleColumnDefinition.Name).Append ("]");
    }

    public void VisitObjectIDWithClassIDColumnDefinition (ObjectIDWithClassIDColumnDefinition objectIDWithClassIDColumnDefinition)
    {
      ArgumentUtility.CheckNotNull ("objectIDWithClassIDColumnDefinition", objectIDWithClassIDColumnDefinition);

      objectIDWithClassIDColumnDefinition.ObjectIDColumn.Accept (this);
      objectIDWithClassIDColumnDefinition.ClassIDColumn.Accept (this);
    }

    public void VisitNullColumnDefinition (NullColumnDefinition nullColumnDefinition)
    {
      ArgumentUtility.CheckNotNull ("nullColumnDefinition", nullColumnDefinition);

      if (!_allowNullColumns)
        throw new NotSupportedException ("Null columns are not supported at this point.");

      AppendSeparatorIfRequired ();
      _nameList.Append ("NULL");
    }

    private void AppendSeparatorIfRequired ()
    {
      if (_nameList.Length > 0)
        _nameList.Append (", ");
    }
  }
}