using System;
using System.Text;
using System.ComponentModel;

using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.Web.Test.Domain
{
public class ClassWithAllDataTypesSearch : BindableSearchObject
{
  private string _stringProperty;
  private NaByte _bytePropertyFrom;
  private NaByte _bytePropertyTo;
  private ClassWithAllDataTypes.EnumType _enumProperty;
  private NaDateTime _datePropertyFrom;
  private NaDateTime _datePropertyTo;
  private NaDateTime _dateTimePropertyFrom;
  private NaDateTime _dateTimePropertyTo;

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

  [DateType (DateTypeEnum.Date)]
  public NaDateTime DatePropertyFrom
  {
    get { return _datePropertyFrom; }
    set { _datePropertyFrom = value; }
  }

  [DateType (DateTypeEnum.Date)]
  public NaDateTime DatePropertyTo
  {
    get { return _datePropertyTo; }
    set { _datePropertyTo = value; }
  }

  [DateType (DateTypeEnum.DateTime)]
  public NaDateTime DateTimePropertyFrom
  {
    get { return _dateTimePropertyFrom; }
    set { _dateTimePropertyFrom = value; }
  }

  [DateType (DateTypeEnum.DateTime)]
  public NaDateTime DateTimePropertyTo
  {
    get { return _dateTimePropertyTo; }
    set { _dateTimePropertyTo = value; }
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

    if (!_dateTimePropertyFrom.IsNull)
      AddParameter (whereClauseBuilder, parameters, "DateTime >=", "DateTimePropertyFrom", _dateTimePropertyFrom);

    if (!_dateTimePropertyTo.IsNull)
      AddParameter (whereClauseBuilder, parameters, "DateTime <=", "DateTimePropertyTo", _dateTimePropertyTo);
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
