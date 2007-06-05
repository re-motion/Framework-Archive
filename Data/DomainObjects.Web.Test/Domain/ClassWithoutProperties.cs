using System;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.Data.DomainObjects.Web.Test.Domain
{
  [Serializable]
  [DBTable ("TableWithoutColumns")]
  [Instantiable]
  [DBStorageGroup]
  public abstract class ClassWithoutProperties: BindableDomainObject
  {
    public static ClassWithoutProperties NewObject()
    {
      return DomainObject.NewObject<ClassWithoutProperties> ().With ();
    }

    protected ClassWithoutProperties()
    {
    }
  }
}