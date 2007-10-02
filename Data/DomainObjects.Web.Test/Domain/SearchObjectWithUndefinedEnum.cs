using System;
using Rubicon.ObjectBinding;
using Rubicon.Data.DomainObjects.Queries;

namespace Rubicon.Data.DomainObjects.Web.Test.Domain
{
  [BindableObject]
  public class SearchObjectWithUndefinedEnum
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
  }
}