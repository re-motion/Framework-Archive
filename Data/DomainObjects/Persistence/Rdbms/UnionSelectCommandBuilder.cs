using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
{
  public class UnionSelectCommandBuilder : CommandBuilder
  {
    // types

    // static members and constants

  public static UnionSelectCommandBuilder CreateForRelatedIDLookup (
      RdbmsProvider provider, 
      ClassDefinition classDefinition, 
      PropertyDefinition propertyDefinition,
      ObjectID relatedID)
  {
    ArgumentUtility.CheckNotNull ("provider", provider);
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
    ArgumentUtility.CheckNotNull ("relatedID", relatedID);

    return new UnionSelectCommandBuilder (provider, classDefinition, propertyDefinition, relatedID);
  }

    // member fields

    private ClassDefinition _classDefinition;
    private PropertyDefinition _propertyDefinition;
    private ObjectID _relatedID;

    // construction and disposing

    private UnionSelectCommandBuilder (      
        RdbmsProvider provider, 
        ClassDefinition classDefinition, 
        PropertyDefinition propertyDefinition,
        ObjectID relatedID) : base (provider)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("relatedID", relatedID);

      _classDefinition = classDefinition;
      _propertyDefinition = propertyDefinition;
      _relatedID = relatedID;
    }

    // methods and properties

    public override IDbCommand Create ()
    {
      string[] allConcreteEntityNames = _classDefinition.GetAllConcreteEntityNames ();
      if (allConcreteEntityNames.Length == 0)
        return null;

      IDbCommand command = CreateCommand ();
      WhereClauseBuilder whereClauseBuilder = new WhereClauseBuilder (this, command);
      whereClauseBuilder.Add (_propertyDefinition.ColumnName, GetObjectIDValueForParameter (_relatedID));

      VirtualRelationEndPointDefinition oppositeRelationEndPointDefinition =
          (VirtualRelationEndPointDefinition) _classDefinition.GetMandatoryOppositeEndPointDefinition (_propertyDefinition.PropertyName);

      string columnsFromSortExpression = GetColumnsFromSortExpression (oppositeRelationEndPointDefinition.SortExpression);

      StringBuilder commandTextStringBuilder = new StringBuilder ();
      string selectTemplate = "SELECT [ID], [ClassID]{0} FROM [{1}] WHERE {2}";
      foreach (string entityName in allConcreteEntityNames)
      {
        if (commandTextStringBuilder.Length > 0)
          commandTextStringBuilder.Append ("\nUNION ALL ");

        commandTextStringBuilder.AppendFormat (selectTemplate, columnsFromSortExpression, entityName, whereClauseBuilder.ToString ());
      }

      commandTextStringBuilder.Append (GetOrderClause (oppositeRelationEndPointDefinition.SortExpression));
      commandTextStringBuilder.Append (";");
      
      command.CommandText = commandTextStringBuilder.ToString ();

      return command;
    }

    private string GetColumnsFromSortExpression (string sortExpression)
    {
      if (StringUtility.IsNullOrEmpty (sortExpression))
        return string.Empty;

      return ", " + Provider.GetColumnsFromSortExpression (sortExpression);
    }
  }
}
