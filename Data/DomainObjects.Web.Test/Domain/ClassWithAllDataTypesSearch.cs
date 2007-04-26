using System;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.Data.DomainObjects.Web.Test.Domain
{
[Serializable]
public class ClassWithAllDataTypesSearch : BindableSearchObject
{
  private string _stringProperty;
  private byte? _bytePropertyFrom;
  private byte? _bytePropertyTo;
  private ClassWithAllDataTypes.EnumType _enumProperty;
  private DateTime? _datePropertyFrom;
  private DateTime? _datePropertyTo;
  private DateTime? _dateTimePropertyFrom;
  private DateTime? _dateTimePropertyTo;

  public string StringProperty
  {
    get { return _stringProperty; }
    set { _stringProperty = value; }
  }

  public byte? BytePropertyFrom
  {
    get { return _bytePropertyFrom; }
    set { _bytePropertyFrom = value; }
  }

  public byte? BytePropertyTo
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
  public DateTime? DatePropertyFrom
  {
    get { return _datePropertyFrom; }
    set { _datePropertyFrom = value; }
  }

  [DateType (DateTypeEnum.Date)]
  public DateTime? DatePropertyTo
  {
    get { return _datePropertyTo; }
    set { _datePropertyTo = value; }
  }

  [DateType (DateTypeEnum.DateTime)]
  public DateTime? DateTimePropertyFrom
  {
    get { return _dateTimePropertyFrom; }
    set { _dateTimePropertyFrom = value; }
  }

  [DateType (DateTypeEnum.DateTime)]
  public DateTime? DateTimePropertyTo
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
