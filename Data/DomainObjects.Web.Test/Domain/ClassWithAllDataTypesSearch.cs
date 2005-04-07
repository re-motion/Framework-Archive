using System;
using System.Text;
using System.ComponentModel;

using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.Web.Test.Domain
{
public class ClassWithAllDataTypesQuery : BindableSearchObject
{
  private string _stringProperty;
  private NaByte _bytePropertyFrom;
  private NaByte _bytePropertyTo;
  private ClassWithAllDataTypes.EnumType _enumProperty;
  private NaDateTime _datePropertyFrom;
  private NaDateTime _datePropertyTo;

  public string StringProperty
  {
    get { return _stringProperty; }
    set { _stringProperty = value; }
  }

  public NaByte BytePropertyFrom
  {
    get { return _bytePropertyFrom; }
    set { _bytePropertyFrom = value; }
  }

  public NaByte BytePropertyTo
  {
    get { return _bytePropertyTo; }
    set { _bytePropertyTo = value; }
  }

  public ClassWithAllDataTypes.EnumType EnumProperty
  {
    get { return _enumProperty; }
    set { _enumProperty = value; }
  }

  [IsDateType (true)]
  public NaDateTime DatePropertyFrom
  {
    get { return _datePropertyFrom; }
    set { _datePropertyFrom = value; }
  }

  [IsDateType (true)]
  public NaDateTime DatePropertyTo
  {
    get { return _datePropertyTo; }
    set { _datePropertyTo = value; }
  }

  public override IQuery CreateQuery()
  {
    StringBuilder whereClauseBuilder = new StringBuilder ();
    QueryParameterCollection parameters = new QueryParameterCollection ();

    AddParameters (whereClauseBuilder, parameters);

    if (whereClauseBuilder.Length > 0)
      whereClauseBuilder.Insert (0, " WHERE ");

    return CreateQuery (string.Format ("SELECT [TableWithAllDataTypes].* FROM TableWithAllDataTypes{0};", whereClauseBuilder.ToString ()), parameters);
  }

  private Query CreateQuery (string sqlStatement, QueryParameterCollection parameters)
  {
    QueryDefinition definition = new QueryDefinition ("ClassWithAllDataTypesQuery", "RpaTest", sqlStatement, QueryType.Collection, typeof (DomainObjectCollection));
    return new Query (definition, parameters);
  }

  private void AddParameters (StringBuilder whereClauseBuilder, QueryParameterCollection parameters)
  {
    if (_stringProperty != null)
      AddParameter (whereClauseBuilder, parameters, "StringProperty =", "StringProperty", _stringProperty);

    if (!_bytePropertyFrom.IsNull)
      AddParameter (whereClauseBuilder, parameters, "Byte >=", "BytePropertyFrom", _bytePropertyFrom);

    if (!_bytePropertyTo.IsNull)
      AddParameter (whereClauseBuilder, parameters, "Byte <=", "BytePropertyTo", _bytePropertyTo);

    AddParameter (whereClauseBuilder, parameters, "Enum =", "EnumProperty", _enumProperty);

    if (!_datePropertyFrom.IsNull)
      AddParameter (whereClauseBuilder, parameters, "Date >=", "DatePropertyFrom", _datePropertyFrom);

    if (!_datePropertyTo.IsNull)
      AddParameter (whereClauseBuilder, parameters, "Date <=", "DatePropertyTo", _datePropertyTo);
  }

  private void AddParameter (
      StringBuilder whereClauseBuilder,
      QueryParameterCollection parameters,
      string whereClauseExpression,
      string parameterName,
      object parameterValue)
  {
    if (whereClauseBuilder.Length > 0)
      whereClauseBuilder.Append (" AND ");

    whereClauseBuilder.Append (string.Format ("{0} @{1}", whereClauseExpression, parameterName));

    parameters.Add (new QueryParameter (parameterName, parameterValue));
  }
}
}
