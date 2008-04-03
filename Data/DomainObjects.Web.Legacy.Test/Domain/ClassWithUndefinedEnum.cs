using System;

using Remotion.Globalization;
using Remotion.NullableValueTypes;

using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;

namespace Remotion.Data.DomainObjects.Web.Legacy.Test.Domain
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