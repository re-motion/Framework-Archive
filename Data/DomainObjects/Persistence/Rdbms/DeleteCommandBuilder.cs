using System;
using System.Data;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
{
public class DeleteCommandBuilder : CommandBuilder
{
  // types

  // static members and constants

  // member fields

  private DataContainer _dataContainer;

  // construction and disposing

  public DeleteCommandBuilder (RdbmsProvider provider, DataContainer dataContainer) : base (provider)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    if (dataContainer.State != StateType.Deleted)
      throw CreateArgumentException ("dataContainer", "State of provided DataContainer must be 'Deleted', but is '{0}'.", dataContainer.State);

    _dataContainer = dataContainer;
  }

  // methods and properties

  public override System.Data.IDbCommand Create ()
  {
    IDbCommand command = CreateCommand ();

    WhereClauseBuilder whereClauseBuilder = new WhereClauseBuilder (this, command);
    whereClauseBuilder.Add ("ID", _dataContainer.ID.Value);

    if (MustAddTimestampToWhereClause ())
      whereClauseBuilder.Add ("Timestamp", _dataContainer.Timestamp);

    command.CommandText = string.Format ("DELETE FROM [{0}] WHERE {1};",
        _dataContainer.ClassDefinition.EntityName,
        whereClauseBuilder.ToString ());

    return command;
  }

  private bool MustAddTimestampToWhereClause ()
  {
    foreach (PropertyValue propertyValue in _dataContainer.PropertyValues)
    {
      if (propertyValue.PropertyType == typeof (ObjectID))
        return false;
    }

    return true;
  }
}
}
