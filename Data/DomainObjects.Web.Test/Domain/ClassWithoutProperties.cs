using System;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.Data.DomainObjects.Web.Test.Domain
{
  [Serializable]
  [DBTable (Name = "TableWithoutColumns")]
  [RpaTest]
  [NotAbstract]
  public abstract class ClassWithoutProperties: BindableDomainObject
  {
    public static ClassWithoutProperties NewObject()
    {
      return DomainObject.NewObject<ClassWithoutProperties> ().With ();
    }

    protected ClassWithoutProperties()
    {
    }

    protected ClassWithoutProperties (DataContainer dataContainer)
        : base (dataContainer)
    {
    }
  }
}