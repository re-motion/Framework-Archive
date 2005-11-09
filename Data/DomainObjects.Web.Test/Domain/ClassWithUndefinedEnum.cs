using System;

using Rubicon.Globalization;
using Rubicon.NullableValueTypes;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.Data.DomainObjects.Web.Test.Domain
{
[Serializable]
public class ClassWithUndefinedEnum : BindableDomainObject
{
  // types

  // static members and constants
  
  public static new ClassWithUndefinedEnum GetObject (ObjectID id)
  {
    return (ClassWithUndefinedEnum) BindableDomainObject.GetObject (id);
  }

  // member fields

  // construction and disposing
  
  public ClassWithUndefinedEnum ()
  {
  }

  public ClassWithUndefinedEnum (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected ClassWithUndefinedEnum (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public UndefinedEnum UndefinedEnum
  {
    get { return (UndefinedEnum) DataContainer.GetValue ("UndefinedEnum"); }
    set { DataContainer.SetValue ("UndefinedEnum", value); }
  }

}
}