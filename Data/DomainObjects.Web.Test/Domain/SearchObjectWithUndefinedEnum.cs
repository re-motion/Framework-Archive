using System;

using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.Data.DomainObjects.Web.Test.Domain
{
public class SearchObjectWithUndefinedEnum : BindableSearchObject
{
  // types

  // static members and constants

  // member fields

  private UndefinedEnum _undefinedEnum;

  // construction and disposing

  public SearchObjectWithUndefinedEnum ()
  {
    _undefinedEnum = UndefinedEnum.Undefined;
  }

  // methods and properties

  public UndefinedEnum UndefinedEnum 
  {
    get { return _undefinedEnum; }
    set { _undefinedEnum = value; }
  }

  public override Rubicon.Data.DomainObjects.Queries.IQuery CreateQuery()
  {
    throw new NotImplementedException ();
  }

}
}
