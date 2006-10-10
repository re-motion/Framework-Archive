using System;
using System.Data;
using System.Text;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
{
public class SelectCommandBuilder : CommandBuilder
{
  // types

  // static members and constants

  public static SelectCommandBuilder CreateForIDLookup (RdbmsProvider provider, string selectColumns, string entityName, ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("provider", provider);
    ArgumentUtility.CheckNotNullOrEmpty ("selectColumns", selectColumns);
    ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);
    ArgumentUtility.CheckNotNull ("id", id);

    return new SelectCommandBuilder (provider, selectColumns, entityName, "ID", new ObjectID[] { id }, false, null);
  }

  public static SelectCommandBuilder CreateForIDLookup (RdbmsProvider provider, string entityName, ObjectID[] ids)
  {
    ArgumentUtility.CheckNotNull ("provider", provider);
    ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);
    ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("ids", ids);

    return new SelectCommandBuilder (provider, "*", entityName, "ID", ids, false, null);
  }

  public static SelectCommandBuilder CreateForRelatedIDLookup (
      RdbmsProvider provider, 
      ClassDefinition classDefinition, 
      PropertyDefinition propertyDefinition,
      ObjectID relatedID)
  {
    ArgumentUtility.CheckNotNull ("provider", provider);
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
    ArgumentUtility.CheckNotNull ("relatedID", relatedID);

    VirtualRelationEndPointDefinition oppositeRelationEndPointDefinition =
        (VirtualRelationEndPointDefinition) classDefinition.GetMandatoryOppositeEndPointDefinition (propertyDefinition.PropertyName);

    return new SelectCommandBuilder (
        provider, "*", classDefinition.GetEntityName (), propertyDefinition.ColumnName, new ObjectID[] { relatedID }, true, oppositeRelationEndPointDefinition.SortExpression);
  }

  // member fields

  private string _selectColumns;
  private string _entityName;
  private string _whereClauseColumnName;
  private ObjectID[] _whereClauseIDs;
  private bool _whereClauseValueIsRelatedID;
  private string _orderExpression;

  // construction and disposing

  private SelectCommandBuilder (
      RdbmsProvider provider,
      string selectColumns,
      string entityName,
      string whereClauseColumnName,
      ObjectID[] whereClauseIDs,
      bool whereClauseValueIsRelatedID,
      string orderExpression) : base (provider)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("selectColumns", selectColumns);
    ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);
    ArgumentUtility.CheckNotNullOrEmpty ("whereClauseColumnName", whereClauseColumnName);
    ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("whereClauseIDs", whereClauseIDs);

    _selectColumns = selectColumns;
    _entityName = entityName;
    _whereClauseColumnName = whereClauseColumnName;
    _whereClauseIDs = whereClauseIDs;
    _whereClauseValueIsRelatedID = whereClauseValueIsRelatedID;
    _orderExpression = orderExpression;
  }

  // methods and properties

  public override IDbCommand Create ()
  {
    IDbCommand command = CreateCommand ();
    WhereClauseBuilder whereClauseBuilder = new WhereClauseBuilder (this, command);

    if (_whereClauseIDs.Length == 1)
      whereClauseBuilder.Add (_whereClauseColumnName, GetObjectIDValueForParameter (_whereClauseIDs[0]));
    else
      whereClauseBuilder.SetInExpression (_whereClauseColumnName, GetValueArrayForParameter (_whereClauseIDs));

    // TODO in case of integer primary keys: 
    // If RdbmsProvider or one of its derived classes will support integer primary keys in addition to GUIDs,
    // the code below must be selectively actived to run only for integer primary keys.
    // Note: This behaviour is not desired in case of GUID primary keys, because two same foreign key GUIDs pointing 
    //       to different classIDs must be an error! In this case PersistenceManager.CheckClassIDForVirtualEndPoint raises an exception. 
    //if (_whereClauseValueIsRelatedID && _whereClauseID.ClassDefinition.IsPartOfInheritanceHierarchy && IsOfSameStorageProvider (_whereClauseID))
    //  whereClauseBuilder.Add (RdbmsProvider.GetClassIDColumnName (_whereClauseColumnName), _whereClauseID.ClassID);

    command.CommandText = string.Format ("SELECT {0} FROM {1} WHERE {2}{3}{4}",
        _selectColumns, 
        Provider.DelimitIdentifier (_entityName), 
        whereClauseBuilder.ToString (), 
        GetOrderClause (_orderExpression),
        Provider.StatementDelimiter);

    return command;
  }

  protected override void AppendColumn (string columnName, string parameterName)
  {
    throw new NotSupportedException ("'AppendColumn' is not supported by 'SelectCommandBuilder'.");
  }

  private object[] GetValueArrayForParameter (ObjectID[] objectIDs)
  {
    object[] values = new object[objectIDs.Length];
    
    for (int i = 0; i < objectIDs.Length; i++)
      values[i] = GetObjectIDValueForParameter (objectIDs[i]);

    return values;
  }
}
}
