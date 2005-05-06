using System;

using Rubicon.Globalization;
using Rubicon.NullableValueTypes;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.Web.Test.Domain
{
public class ClassWithoutProperties : BindableDomainObject
{
  // types

  // static members and constants
  
  public static new ClassWithoutProperties GetObject (ObjectID id)
  {
    return (ClassWithoutProperties) BindableDomainObject.GetObject (id);
  }

  // member fields

  // construction and disposing
  
  public ClassWithoutProperties ()
  {
  }

  public ClassWithoutProperties (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected ClassWithoutProperties (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

}
}