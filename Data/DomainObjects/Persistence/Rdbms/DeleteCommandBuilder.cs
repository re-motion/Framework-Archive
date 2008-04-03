using System;
using System.Data;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class DeleteCommandBuilder: CommandBuilder
  {
    // types

    // static members and constants

    // member fields

    private DataContainer _dataContainer;

    // construction and disposing

    public DeleteCommandBuilder (RdbmsProvider provider, DataContainer dataContainer)
        : base (provider)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      if (dataContainer.State != StateType.Deleted)
        throw CreateArgumentException ("dataContainer", "State of provided DataContainer must be 'Deleted', but is '{0}'.", dataContainer.State);

      _dataContainer = dataContainer;
    }

    // methods and properties

    public override IDbCommand Create()
    {
      IDbCommand command = Provider.CreateDbCommand();

      WhereClauseBuilder whereClauseBuilder = WhereClauseBuilder.Create (this, command);
      whereClauseBuilder.Add ("ID", _dataContainer.ID.Value);

      if (MustAddTimestampToWhereClause())
        whereClauseBuilder.Add ("Timestamp", _dataContainer.Timestamp);

      command.CommandText = string.Format (
          "DELETE FROM {0} WHERE {1}{2}",
          Provider.DelimitIdentifier (_dataContainer.ClassDefinition.GetEntityName()),
          whereClauseBuilder.ToString(),
          Provider.StatementDelimiter);

      return command;
    }

    private bool MustAddTimestampToWhereClause()
    {
      foreach (PropertyValue propertyValue in _dataContainer.PropertyValues)
      {
        if (propertyValue.PropertyType == typeof (ObjectID))
          return false;
      }

      return true;
    }

    protected override void AppendColumn (string columnName, string parameterName)
    {
      throw new NotSupportedException ("'AppendColumn' is not supported by 'QueryCommandBuilder'.");
    }
  }
}