using System;

using Remotion.Globalization;
using Remotion.NullableValueTypes;

using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;

namespace Remotion.Data.DomainObjects.Web.Legacy.Test.Domain
{
[Serializable]
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

  protected ClassWithoutProperties (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

}
}