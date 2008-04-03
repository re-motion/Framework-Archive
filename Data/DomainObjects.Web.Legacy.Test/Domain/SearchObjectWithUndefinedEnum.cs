using System;

using Remotion.Data.DomainObjects.ObjectBinding;

namespace Remotion.Data.DomainObjects.Web.Legacy.Test.Domain
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

  public override Remotion.Data.DomainObjects.Queries.IQuery CreateQuery()
  {
    throw new NotImplementedException ();
  }

}
}
