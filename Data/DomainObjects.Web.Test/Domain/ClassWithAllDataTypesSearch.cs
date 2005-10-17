using System;
using System.Text;
using System.ComponentModel;

using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.Data.DomainObjects.Web.Test.Domain
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
    Query query = new Query ("QueryWithAllDataTypes");

    query.Parameters.Add ("@stringProperty", _stringProperty);
    query.Parameters.Add ("@bytePropertyFrom", _bytePropertyFrom);
    query.Parameters.Add ("@bytePropertyTo", _bytePropertyTo);
    query.Parameters.Add ("@enumProperty", _enumProperty);
    query.Parameters.Add ("@datePropertyFrom", _datePropertyFrom);
    query.Parameters.Add ("@datePropertyTo", _datePropertyTo);
    query.Parameters.Add ("@dateTimePropertyFrom", _dateTimePropertyFrom);
    query.Parameters.Add ("@dateTimePropertyTo", _dateTimePropertyTo);

    return query;
  }
}
}
