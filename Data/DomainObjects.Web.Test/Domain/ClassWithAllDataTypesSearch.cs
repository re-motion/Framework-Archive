using System;
using System.Text;
using System.ComponentModel;

using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.Web.Test.Domain
{
public class ClassWithAllDataTypesQuery : BindableQuery
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

  public override QueryParameterCollection Parameters
  {
    get { return CreateParameterCollection (); }
  }

  public override QueryType QueryType
  {
    get { return QueryType.Collection; }
  }

  public override string Statement
  {
    get { return CreateStatement (); }
  }

  public override string StorageProviderID
  {
    get { return "RpaTest"; }
  }

  private string CreateStatement ()
  {
    StringBuilder whereClauseBuilder = new StringBuilder ();

    if (_stringProperty != null)
      AppendToWhereClauseBuilder (whereClauseBuilder, "StringProperty = @StringProperty");

    if (!_bytePropertyFrom.IsNull)
      AppendToWhereClauseBuilder (whereClauseBuilder, "Byte >= @BytePropertyFrom");

    if (!_bytePropertyTo.IsNull)
      AppendToWhereClauseBuilder (whereClauseBuilder, "Byte <= @BytePropertyTo");

    AppendToWhereClauseBuilder (whereClauseBuilder, "Enum = @EnumProperty");

    if (!_datePropertyFrom.IsNull)
      AppendToWhereClauseBuilder (whereClauseBuilder, "Date >= @DatePropertyFrom");

    if (!_datePropertyTo.IsNull)
      AppendToWhereClauseBuilder (whereClauseBuilder, "Date <= @DatePropertyTo");

    return string.Format ("SELECT [TableWithAllDataTypes].* FROM TableWithAllDataTypes WHERE {0};", whereClauseBuilder.ToString ());
  }

  private void AppendToWhereClauseBuilder (StringBuilder whereClauseBuilder, string condition)
  {
    if (whereClauseBuilder.Length > 0)
      whereClauseBuilder.Append (" AND ");

    whereClauseBuilder.Append (condition);
  }

  private QueryParameterCollection CreateParameterCollection ()
  {
    QueryParameterCollection parameterCollection = new QueryParameterCollection ();

    if (_stringProperty != null)
      parameterCollection.Add (new QueryParameter ("StringProperty", _stringProperty));

    if (!_bytePropertyFrom.IsNull)
      parameterCollection.Add (new QueryParameter ("BytePropertyFrom", _bytePropertyFrom));

    if (!_bytePropertyTo.IsNull)
      parameterCollection.Add (new QueryParameter ("BytePropertyTo", _bytePropertyTo));

    parameterCollection.Add (new QueryParameter ("EnumProperty", _enumProperty));

    if (!_datePropertyFrom.IsNull)
      parameterCollection.Add (new QueryParameter ("DatePropertyFrom", _datePropertyFrom));

    if (!_datePropertyTo.IsNull)
      parameterCollection.Add (new QueryParameter ("DatePropertyTo", _datePropertyTo));

    return parameterCollection;
  }
}
}
